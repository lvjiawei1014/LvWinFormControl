using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LvControl.ImageView.Elements;

namespace LvControl.ImageView
{
    [ToolboxItem(true)]
    public partial class LvImageView : Panel
    {
        #region 静态成员
        private static float MaxScale = 10f;
        private static float MinScale = 0.1f;

        #endregion
        #region 成员变量
        public List<DisplayItem> Items = new List<DisplayItem>();
        public DisplayItem curItem;

        public ImageElement imageElement { get { return curItem.imageElement; } set { curItem.imageElement = value; } }
        public Element operatedElement { get { return curItem.operatedElement; } set { curItem.operatedElement = value; } }
        public Element drawingElement { get { return curItem.drawingElement; } set { curItem.drawingElement = value; } }
        public Element selectedElement { get { return curItem.selectedElement; } set { curItem.selectedElement = value; } }
        public Element pointedElement { get { return curItem.pointedElement; } set { curItem.pointedElement = value; } }
        public Point operationStartControlPoint { get { return curItem.operationStartControlPoint; } set { curItem.operationStartControlPoint = value; } }
        public PointF operationStartImagePoint { get { return curItem.operationStartImagePoint; } set { curItem.operationStartImagePoint = value; } }
        public PointF operatedElementStartPoint { get { return curItem.operatedElementStartPoint; } set { curItem.operatedElementStartPoint = value; } }//被操作的element的初始位置

        public List<Element> elements { get { return curItem.elements; } set { curItem.elements = value; } }
        public List<Element> baseElements { get { return curItem.baseElements; } set { curItem.baseElements = value; } }

        #endregion
        #region 属性
        public bool ContinuousDraw { get; set; }
        public Bitmap Image {
            get
            {
                return curItem.imageElement.Image;
            }
            set
            {
                this.curItem.imageElement.Image = value;
                OnImageSet(ref value);
            }
        }
        public ImageViewState ImageViewState { get; set; }
        public ElementType DrawingElementType { get; set; }
        public MouseState MouseState { get; set; }
        public float ImageScale { get { return curItem.imageElement.Scale; } set { this.curItem.imageElement.Scale = value; } }
        public PointF ImageLocation { get { return curItem.imageElement.Location; } set { curItem.imageElement.X = value.X; curItem.imageElement.Y = value.Y; } }
        public bool AutoFit { get; set; }

        #endregion
        #region 事件
        public delegate void ElementCreateEvent(Element element);
        public event ElementCreateEvent ElementCreateEventHandler;

        #endregion
        #region 初始化
        public LvImageView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            curItem = new DisplayItem("default");
            Items.Add(curItem);

            ImageViewState = ImageViewState.Normal;
            MouseState = MouseState.Idle;

            InitializeComponent();

            this.MouseWheel += LvImageView_MouseWheel;
            this.ElementCreateEventHandler += this.OnElementCreate;
        }
        #endregion
        #region 事件处理
        void LvImageView_MouseWheel(object sender, MouseEventArgs e)
        {
            System.Console.WriteLine(e.Delta+"  "+e.Location);
            this.OnScale(e.Location, this.imageElement.Scale * (e.Delta > 0 ? 0.8f : 1.25f));
        }

        /// <summary>
        /// 绘图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvImageView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (imageElement != null)
            {
                if(imageElement.Image!=null)
                {
                    g.DrawImage(imageElement.Image, imageElement.X, imageElement.Y, imageElement.Width * imageElement.Scale, imageElement.Height * imageElement.Scale);
                }
                
                Pen p=new Pen(Color.Blue);
                foreach (Element element in elements)
                {
                    if (element.Visible)
                    {
                        element.Draw(g, p);
                    } 
                }
                if (drawingElement != null && drawingElement.Visible && this.ImageViewState==ImageViewState.Draw)
                {
                    drawingElement.Draw(g, p);
                }
                
            }

        }

        private void OnElementCreate(Element element)
        {
            AddElement(element);
        }
        #endregion
        #region 核心逻辑
        public void AddDisplayItem(string name)
        {
            this.Items.Add(new DisplayItem(name));
        }
        public void AddDisplayItem(DisplayItem item)
        {
            this.Items.Add(item);
        }
        public DisplayItem GetDisplayItem(string name)
        {
            for(int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name == name)
                {
                    return Items[i];
                }
            }
            return null;
        }
        public void SwitchDisplayItem(string name)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if(Items[i].Name==name)
                {
                    this.curItem = Items[i];
                    break;
                }
            }
            this.MouseState = MouseState.Idle;
            this.ImageViewState = ImageViewState.Normal;
            this.Refresh();
        }
        public void SwitchDisplayItem(int index)
        {
            if (index < Items.Count && index >= 0)
            {
                this.curItem = Items[index];
                this.MouseState = MouseState.Idle;
                this.ImageViewState = ImageViewState.Normal;
                this.Refresh();
            }
            
        }
        public void DeleteDisplayItem(string name)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name == name)
                {
                    Items.Remove(Items[i]);
                    break;
                }
            }
        }
        private void OnImageSet(ref Bitmap image)
        {
            if(image!=null)
            {
                if(AutoFit)
                {
                    imageElement.FitToWindow(this.Width, this.Height);
                }
                else
                {
                    this.imageElement.Scale = Math.Min(imageElement.Scale*imageElement.Height / image.Height, imageElement.Scale*imageElement.Width / image.Width);
                }
                
                this.Refresh();
            }
        }
        public void ChangeMode(ImageViewState mode)
        {
            if (mode != this.ImageViewState)
            {
                this.MouseState = MouseState.Idle;
            }
            this.ImageViewState = mode;
            switch (mode)
            {
                case ImageViewState.Normal:
                    this.Cursor = Cursors.Hand;
                    break;
                case ImageViewState.Edit:
                    break;
                case ImageViewState.Draw:
                    break;
                default:
                    break;
            }
            

        }

        private void OnScale(Point mouseLocation, float scale)
        {
            if(scale>MaxScale || scale<MinScale)
            {
                return;
            }
            if(imageElement!=null)
            {
                imageElement.ScaleImage(mouseLocation, scale);
            }

            this.Refresh();
        }
        public void AddElement(Element element)
        {
            if (element is RectElement)
            {
                AddRectangle(element as RectElement);
                return;
            }
            if (element is Line)
            {
                AddLine(element as Line);
                return;

            }
            if(element is PolygonElement)
            {
                AddPolypon(element as PolygonElement);
            }
            if(element is EllipseElement)
            {
                AddEllipse(element as EllipseElement);
            }
        }
        public void AddLine(Line line)
        {
            line.ParentCoordinate = imageElement.coordinate;
            line.ParentElement = imageElement;
            elements.Add(line);
            baseElements.Add(line);
            baseElements.Add(line.TractionPoints[0]);
            baseElements.Add(line.TractionPoints[1]);
            baseElements.Sort();
        }
        public void AddPolypon(PolygonElement polygon)
        {
            polygon.ParentCoordinate = imageElement.coordinate;
            polygon.ParentElement = imageElement;
            elements.Add(polygon);
            baseElements.Add(polygon);
            for(int i=0;i<polygon.keyPointList.Count;i++)
            {
                baseElements.Add(polygon.keyPointList[i].tractionPoint);
            }
            baseElements.Sort();
        }
        public void AddRectangle(RectElement rect)
        {
            rect.ParentCoordinate = imageElement.coordinate;
            rect.ParentElement = imageElement;
            elements.Add(rect);
            baseElements.Add(rect);
            baseElements.Add(rect.leftTopPoint);
            baseElements.Add(rect.leftBottomPoint);
            baseElements.Add(rect.rightTopPoint);
            baseElements.Add(rect.rightBottomPoint);
            baseElements.Sort();
        }
        public void AddEllipse(EllipseElement ellipse)
        {
            ellipse.ParentCoordinate = imageElement.coordinate;
            ellipse.ParentElement = imageElement;
            elements.Add(ellipse);
            baseElements.Add(ellipse);
            baseElements.Add(ellipse.leftTopPoint);
            baseElements.Add(ellipse.leftBottomPoint);
            baseElements.Add(ellipse.rightTopPoint);
            baseElements.Add(ellipse.rightBottomPoint);
            baseElements.Sort();
        }
        public Element GetTargetElement(float x, float y)
        {

            for (int i = 0; i <baseElements.Count;i++ )
            {
                if (baseElements[i].IsIn(x, y))
                {
                    return baseElements[i];
                }
            }  
            return imageElement;
        }

        public void AbsoluteMove(PointF  p)
        {
            this.imageElement.Location = p;
        }


        public void DeleteElement(Element element)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if(element==elements[i])
                {
                    elements.RemoveAt(i);
                }
            }

            for (int i = 0; i < baseElements.Count; i++)
            {
                if (element == baseElements[i].ParentElement)
                {
                    baseElements.RemoveAt(i);
                }
            }


            this.Refresh();
        }

        #endregion
        #region GDI绘制

        #endregion 
        #region 鼠标绘制图形
        public void CreateElement(ElementType type)
        {
            CreateElement(type, "");
        }
        /// <summary>
        /// 在绘制前先创建好对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public void CreateElement(ElementType type, string name)
        {
            switch (type)
            {
                case ElementType.Image:
                    break;
                case ElementType.Point:
                    break;
                case ElementType.Line:
                    this.drawingElement = new Line();
                    break;
                case ElementType.Rectangle:
                    this.drawingElement = new RectElement();
                    break;
                case ElementType.Polygon:
                    this.drawingElement=new PolygonElement();
                    break;
                case ElementType.Ellipse:
                    this.drawingElement = new EllipseElement();
                    break;
                default:
                    break;
            }
            this.drawingElement.Name=name;
            this.drawingElement.ParentElement = this.imageElement;
            this.drawingElement.ParentCoordinate = this.imageElement.coordinate;
            this.MouseState = MouseState.Idle;
            this.ImageViewState = ImageViewState.Draw;
            this.DrawingElementType = type;
            this.drawingElement.Visible = false;
        }
        public void SelectElement(Element element)
        {
            if (this.selectedElement != null && element is MainElement)
            {
                this.selectedElement.Selected = false;
            }
            this.selectedElement = element;
            this.selectedElement.Selected = true;
            this.Refresh();
        }

        #endregion
        #region 辅助方法
        //private PointF ControlLocationToImage(PointF p)
        //{
        //    PointF point = new PointF();
        //    point.X = (float)(p.X - imageLocationRectF.X) / ImageScale;
        //    point.Y = (float)(p.Y - imageLocationRectF.Y) / ImageScale;
        //    return point;
        //}
        //private PointF ControlLocationToImage(Point p)
        //{
        //    PointF point = new PointF();
        //    point.X = (float)((float)p.X - imageLocationRectF.X) / ImageScale;
        //    point.Y = (float)((float)p.Y - imageLocationRectF.Y) / ImageScale;
        //    return point;
        //}

        //private PointF ImageLocationToControl(PointF p)
        //{
        //    Point point = new Point();
        //    point.X = (int)(p.X*ImageScale + imageLocationRectF.X);
        //    point.Y = (int)(p.Y * ImageScale + imageLocationRectF.Y);
        //    return point;
        //}
        #endregion
        #region 鼠标事件
        private void LvImageView_MouseClick(object sender, MouseEventArgs e)
        {
            PointF p = Coordinate.CoordinateTransport(e.Location, Coordinate.BaseCoornidate, imageElement.coordinate);
            if (ImageViewState == ImageViewState.Draw)//绘制模式
            {
                if(e.Button==MouseButtons.Left)
                {
                    this.SelectElement(this.drawingElement);
                    if (MouseState == MouseState.Idle)
                    {
                        MouseState = MouseState.Operating;
                        drawingElement.Visible = true;
                    }
                    //如果添加了最后一个点
                    if (drawingElement.AddKeyPoint(p))
                    {
                        MouseState = MouseState.Idle;
                        this.ElementCreateEventHandler(drawingElement);//触发事件
                        if (this.ContinuousDraw)
                        {
                            CreateElement(this.DrawingElementType);
                        }
                        else
                        {
                            drawingElement = null;
                            this.ImageViewState = ImageViewState.Normal;
                        }
                    }
                }else if(e.Button==MouseButtons.Right)
                {
                    if(drawingElement is PolygonElement)
                    {
                        PolygonElement polygon = drawingElement as PolygonElement;
                        if (polygon.Complete())
                        {
                            MouseState = MouseState.Idle;
                            this.ElementCreateEventHandler(drawingElement);
                            if (this.ContinuousDraw)
                            {
                                CreateElement(this.DrawingElementType);
                            }
                            else
                            {
                                drawingElement = null;
                                this.ImageViewState = ImageViewState.Normal;
                            }
                        }
                        else
                        {
                            MouseState = MouseState.Idle;
                            this.drawingElement = null;
                        }
                    }
                }
                
                this.Refresh();
            }

        }

        private void LvImageView_MouseMove(object sender, MouseEventArgs e)
        {
            PointF p = Coordinate.CoordinateTransport(e.Location, Coordinate.BaseCoornidate, imageElement.coordinate);
            Element targetElement =this.GetTargetElement(p.X, p.Y);
            pointedElement = targetElement;
            if (ImageViewState == ImageViewState.Normal)
            {
                if (MouseState == MouseState.Operating)
                {
                    if (operatedElement != imageElement)
                    {
                        //operatedElement.Move(p);
                    }
                    else
                    {
                        AbsoluteMove(new PointF(e.X - operationStartImagePoint.X * ImageScale, e.Y - operationStartImagePoint.Y * ImageScale));

                    }
                    this.Refresh();
                }
                else
                {
                    //this.Cursor = targetElement.ElememtCursor;
                }
                return;
            }
            if(ImageViewState == ImageViewState.Draw)
            {
                if (MouseState == MouseState.Operating)
                {
                    drawingElement.AdjustNextKeyPoint(p);
                    this.Refresh();
                }
            }
            if(ImageViewState == ImageViewState.Edit)
            {
                if (MouseState == MouseState.Operating)
                {
                    if (operatedElement != imageElement)
                    {
                        operatedElement.Move(this.operatedElementStartPoint.X + p.X - operationStartImagePoint.X, this.operatedElementStartPoint.Y + p.Y - operationStartImagePoint.Y);
                    }
                    else
                    {
                        AbsoluteMove(new PointF(e.X - operationStartImagePoint.X * ImageScale, e.Y - operationStartImagePoint.Y * ImageScale));

                    }
                    this.Refresh();
                }
                else
                {
                    this.Cursor = targetElement.ElementCursor;
                }
                return;
            }

        }
        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvImageView_MouseDown(object sender, MouseEventArgs e)
        {

            PointF p = Coordinate.CoordinateTransport(e.Location, Coordinate.BaseCoornidate, imageElement.coordinate);
            Element targetElement =this.GetTargetElement(p.X, p.Y);
            if (ImageViewState == ImageViewState.Normal)
            {
                if (MouseState == MouseState.Operating)
                {

                }
                else
                {

                }
                //图片拖动准备
                this.operatedElement = targetElement;
                this.MouseState = MouseState.Operating;
                this.operationStartControlPoint = e.Location;
                this.operationStartImagePoint = p;
            }
            if (ImageViewState == ImageViewState.Draw)
            {
                //绘图模式暂时不处理鼠标按下事件
            } 
            if (ImageViewState == ImageViewState.Edit)//编辑模式 确定编辑对象
            {
                //
                this.operatedElement = targetElement;
                this.MouseState = MouseState.Operating;
                this.operationStartControlPoint = e.Location;
                this.operationStartImagePoint = p;
                this.operatedElementStartPoint = targetElement.Location;
                System.Console.WriteLine("operationStartImagePoint:" + operationStartImagePoint.X + "  " + operationStartImagePoint.Y);
                this.SelectElement(operatedElement);
            }
            
        }

        private void LvImageView_MouseUp(object sender, MouseEventArgs e)
        {
            if (ImageViewState == ImageViewState.Normal)
            {
                if (MouseState == MouseState.Operating)
                {
                    MouseState = MouseState.Idle;
                }
                else
                {

                }

            }
            if (ImageViewState == ImageViewState.Draw)
            {

            }
            if (ImageViewState == ImageViewState.Edit)
            {
                MouseState = MouseState.Idle;
                operatedElement = null;
            }
            
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class DisplayItem
    {
        public string Name { get; set; }
        public List<Element> elements=new List<Element>();
        public List<Element> baseElements=new List<Element>();
        public ImageElement imageElement;
        public Element operatedElement;
        public Element drawingElement;
        public Element selectedElement;
        public Element pointedElement;
        public Point operationStartControlPoint;
        public PointF operationStartImagePoint;
        public PointF operatedElementStartPoint;//被操作的element的初始位置
        public Bitmap Image { get { return imageElement.Image; }set { imageElement.Image = value; } }
        public DisplayItem(string name)
        {
            this.Name = name;
            this.imageElement = new ImageElement();
        }
        public void AddElement(Element element)
        {
            if (element is RectElement)
            {
                AddRectangle(element as RectElement);
                return;
            }
            if (element is Line)
            {
                AddLine(element as Line);
                return;

            }
            if (element is PolygonElement)
            {
                AddPolypon(element as PolygonElement);
            }
            if (element is EllipseElement)
            {
                AddEllipse(element as EllipseElement);
            }
        }
        public void AddLine(Line line)
        {
            line.ParentCoordinate = imageElement.coordinate;
            line.ParentElement = imageElement;
            elements.Add(line);
            baseElements.Add(line);
            baseElements.Add(line.TractionPoints[0]);
            baseElements.Add(line.TractionPoints[1]);
            baseElements.Sort();
        }
        public void AddPolypon(PolygonElement polygon)
        {
            polygon.ParentCoordinate = imageElement.coordinate;
            polygon.ParentElement = imageElement;
            elements.Add(polygon);
            baseElements.Add(polygon);
            for (int i = 0; i < polygon.keyPointList.Count; i++)
            {
                baseElements.Add(polygon.keyPointList[i].tractionPoint);
            }
            baseElements.Sort();
        }
        public void AddRectangle(RectElement rect)
        {
            rect.ParentCoordinate = imageElement.coordinate;
            rect.ParentElement = imageElement;
            elements.Add(rect);
            baseElements.Add(rect);
            baseElements.Add(rect.leftTopPoint);
            baseElements.Add(rect.leftBottomPoint);
            baseElements.Add(rect.rightTopPoint);
            baseElements.Add(rect.rightBottomPoint);
            baseElements.Sort();
        }
        public void AddEllipse(EllipseElement ellipse)
        {
            ellipse.ParentCoordinate = imageElement.coordinate;
            ellipse.ParentElement = imageElement;
            elements.Add(ellipse);
            baseElements.Add(ellipse);
            baseElements.Add(ellipse.leftTopPoint);
            baseElements.Add(ellipse.leftBottomPoint);
            baseElements.Add(ellipse.rightTopPoint);
            baseElements.Add(ellipse.rightBottomPoint);
            baseElements.Sort();
        }
        public void DeleteElement(Element element)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (element == elements[i])
                {
                    elements.RemoveAt(i);
                }
            }

            for (int i = 0; i < baseElements.Count; i++)
            {
                if (element == baseElements[i].ParentElement)
                {
                    baseElements.RemoveAt(i);
                }
            }
        }

        public void DeleteAllElement()
        {
            elements.Clear();
            baseElements.Clear();
        }


    }

    public enum ImageViewState
    {
        Normal=0,
        Edit=1,
        Draw=2,
    }

    public enum MouseState
    {
        Idle=0,
        Operating=1,
    }

    public enum ElementType
    {
        Image=0,
        Point=1,
        Line=2,
        Rectangle=3,
        Polygon=4,
        Ellipse=5,
    }

    
}
