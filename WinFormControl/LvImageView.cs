using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormControl
{
    public partial class LvImageView : Panel
    {
        #region 静态成员
        private static float MaxScale = 10f;
        private static float MinScale = 0.1f;

        #endregion
        #region 成员变量
        //private Image image = null;
        private ImageElement imageElement;
        /// <summary>
        /// 交互元素
        /// </summary>
        private List<Element> elements;
        private List<Element> baseElements;
        private Element operatedElement;
        private Element drawingElement;
        public Element selectedElement;
        public Element pointedElement;

        private Point operationStartControlPoint;
        private PointF operationStartImagePoint;
        private PointF targetStartImagePoint;
        #endregion
        #region 属性
        public Image Image {
            get
            {
                return imageElement.Image;
            }
            set
            {
                this.imageElement.Image = value;
                OnImageSet(ref value);
            }
        }
        public ImageViewState ImageViewState { get; set; }
        public ElementType DrawingElementType { get; set; }
        public MouseState MouseState { get; set; }
        public float ImageScale { get { return imageElement.Scale; } set { this.imageElement.Scale = value; } }
        public PointF ImageLocation { get { return imageElement.Location; } set { imageElement.X = value.X; imageElement.Y = value.Y; } }
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

            elements = new List<Element>();
            baseElements = new List<Element>();
            ImageViewState = ImageViewState.Normal;
            MouseState = MouseState.Idle;

            InitializeComponent();
            this.imageElement = new ImageElement();

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
                if (drawingElement != null && drawingElement.Visible)
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
        private void OnImageSet(ref Image image)
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
            if (element is Rectangle)
            {
                AddRectangle(element as Rectangle);
                return;
            }
            if (element is Line)
            {
                AddLine(element as Line);
                return;

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
        public void AddRectangle(Rectangle rect)
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

        #endregion
        #region GDI绘制
        private void PaintElement(Graphics g, Pen pen, Element element)
        {
            if (element is Rectangle)
            {
                PaintElement(g, pen, element as Rectangle);
            }
        }
        private void PaintElement(Graphics g,Pen pen,Rectangle rect)
        {
            PointF loca = Coordinate.CoordinateTransport(rect.Location,imageElement.coordinate, Coordinate.BaseCoornidate);
            g.DrawRectangle(pen, loca.X, loca.Y, rect.Width * ImageScale, rect.Height * ImageScale);
            if (rect.Selected)
            {
                PaintElement(g, pen, rect.leftTopPoint);
                PaintElement(g, pen, rect.leftBottomPoint);
                PaintElement(g, pen, rect.rightBottomPoint);
                PaintElement(g, pen, rect.rightTopPoint);
            }
        }
        /// <summary>
        /// 绘制拖拽点
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="tractionPoint"></param>
        private void  PaintElement(Graphics g,Pen pen,TractionPoint tractionPoint)
        {
            PointF loca = Coordinate.CoordinateTransport(tractionPoint.Location, imageElement.coordinate, Coordinate.BaseCoornidate);
            g.FillRectangle(Brushes.Black, loca.X - tractionPoint.Size / 2, loca.Y - tractionPoint.Size / 2, tractionPoint.Size, tractionPoint.Size);
            g.DrawRectangle(Pens.White, loca.X - tractionPoint.Size / 2, loca.Y - tractionPoint.Size / 2, tractionPoint.Size, tractionPoint.Size);
        }
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
                    this.drawingElement = new Rectangle();
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

        private void LvImageView_MouseClick(object sender, MouseEventArgs e)
        {
            PointF p = Coordinate.CoordinateTransport(e.Location, Coordinate.BaseCoornidate, imageElement.coordinate);
            if (ImageViewState == ImageViewState.Draw)//绘制模式
            {
                this.SelectElement(this.drawingElement);
                if(MouseState==MouseState.Idle)
                {
                    MouseState = MouseState.Operating;
                    drawingElement.Visible = true;
                }
                //如果添加了最后一个点
                if (drawingElement.AddKeyPoint(p))
                {
                    MouseState = MouseState.Idle;
                    this.ElementCreateEventHandler(drawingElement);//触发事件
                    CreateElement(this.DrawingElementType);
                }
                this.Refresh();
            }
            if (ImageViewState == ImageViewState.Edit)
            {
                //编辑模式不处理Click
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
                    drawingElement.AdjustLastKeyPoint(p);
                    this.Refresh();
                }
            }
            if(ImageViewState == ImageViewState.Edit)
            {
                if (MouseState == MouseState.Operating)
                {
                    if (operatedElement != imageElement)
                    {
                        operatedElement.Move(this.targetStartImagePoint.X + p.X - operationStartImagePoint.X, this.targetStartImagePoint.Y + p.Y - operationStartImagePoint.Y);
                    }
                    else
                    {
                        AbsoluteMove(new PointF(e.X - operationStartImagePoint.X * ImageScale, e.Y - operationStartImagePoint.Y * ImageScale));

                    }
                    this.Refresh();
                }
                else
                {
                    this.Cursor = targetElement.ElememtCursor;
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
                this.targetStartImagePoint = targetElement.Location;
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

    }

    
}
