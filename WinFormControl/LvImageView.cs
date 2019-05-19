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
        private Image image = null;
        private RectangleF imageLocationRectF;
        /// <summary>
        /// 交互元素
        /// </summary>
        private List<Element> elements;
        private List<Element> baseElements;
        private Element operatedElement;

        private MouseState mouseState;

        private Point operationStartControlPoint;
        private PointF operationStartImagePoint;
        #endregion
        #region 属性
        public Image Image {
            get
            {
                return image;
            }
            set
            {
                this.image = value;
                OnImageSet(ref value);
            }
        }
        public ImageViewState State { get; set; }
        public float ImageScale { get; set; }
        public PointF ImageLocation { get { return imageLocationRectF.Location; } set { imageLocationRectF.Location = value; } }
        public bool AutoFit { get; set; }

        #endregion
        #region 初始化

        public LvImageView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);


            elements = new List<Element>();
            baseElements = new List<Element>();
            State = ImageViewState.Normal;
            mouseState = MouseState.Idle;

            InitializeComponent();

            this.ImageLocation = new Point(0, 0);
            this.ImageScale = 1;
            imageLocationRectF = new RectangleF(0, 0, this.Width, this.Height);

            this.MouseWheel += LvImageView_MouseWheel;
        }
        #endregion
        #region 事件处理
        void LvImageView_MouseWheel(object sender, MouseEventArgs e)
        {
            System.Console.WriteLine(e.Delta+"  "+e.Location);
            this.OnScale(e.Location, this.ImageScale * (e.Delta > 0 ? 0.8f : 1.25f));
        }

        /// <summary>
        /// 绘图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvImageView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (image != null)
            {
                g.DrawImage(image,imageLocationRectF);
                Pen p=new Pen(Color.Blue);
                foreach (Element element in elements)
                {
                    if (element is Rectangle)
                    {
                        Rectangle rect =element as Rectangle;
                        PaintElement( g,  p,  rect);
                    }
                }
            }

        }
        #endregion
        #region 核心逻辑
        private void OnImageSet(ref Image image)
        {
            if(image!=null)
            {
                if(AutoFit)
                {
                    this.ImageScale = Math.Min(this.Height/(float)image.Height, this.Width/(float)image.Width);
                    this.imageLocationRectF.X = Math.Max(0, (this.Width - image.Width * ImageScale) / 2);
                    this.imageLocationRectF.Y = Math.Max(0, (this.Height - image.Height * ImageScale) / 2);
                    this.imageLocationRectF.Width = image.Width*ImageScale;
                    this.imageLocationRectF.Height = image.Height*ImageScale;
                }
                else
                {
                    this.ImageScale = Math.Min((float)imageLocationRectF.Height / image.Height, (float)imageLocationRectF.Width / image.Width);
                }
                
                this.Refresh();
            }
        }

        private void OnScale(Point mouseLocation, float scale)
        {
            if(scale>MaxScale || scale<MinScale)
            {
                return;
            }
            if(image!=null)
            {
                PointF imageAnchor = this.ControlLocationToImage(mouseLocation);

                imageLocationRectF.X = mouseLocation.X - scale * imageAnchor.X;
                imageLocationRectF.Y = mouseLocation.Y - scale * imageAnchor.Y;
                imageLocationRectF.Width = image.Width * scale;
                imageLocationRectF.Height = image.Height * scale;

            }
            ImageScale = scale;
            this.Refresh();
        }

        public void AddElement(Element element)
        {
            this.elements.Add(element);
        }
        public void AddRectangle(Rectangle rect)
        {
            AddElement(rect);
            baseElements.Add(rect.leftTopPoint);
            baseElements.Add(rect.leftBottomPoint);
            baseElements.Add(rect.rightTopPoint);
            baseElements.Add(rect.rightBottomPoint);

            baseElements.Sort();
        }
        public Element GetTargetElement(float x, float y)
        {

            for (int i = 0; i < baseElements.Count;i++ )
            {
                if (baseElements[i].IsIn(x, y))
                {
                    return baseElements[i];
                }
            }
            return null;
        }

        public void AbsoluteMove(PointF  p)
        {
            this.imageLocationRectF.Location = p;
        }
        /// <summary>
        /// 不宜在mouse move事件中使用
        /// </summary>
        /// <param name="p"></param>
        public void RelativeMove(PointF p)
        {
            this.imageLocationRectF.X += p.X;
            this.imageLocationRectF.Y += p.Y;
            //System.Console.WriteLine("p:" + p.X + "  " + p.Y);
        }


        #endregion
        #region 绘制

        private void PaintElement(Graphics g,Pen pen,Rectangle rect)
        {
            PointF loca = this.ImageLocationToControl(rect.Location);
            g.DrawRectangle(pen, loca.X, loca.Y, rect.Width * ImageScale, rect.Height * ImageScale);
            PaintElement( g,  pen,  rect.leftTopPoint);
            PaintElement( g,  pen,  rect.leftBottomPoint);
            PaintElement( g,  pen,  rect.rightBottomPoint);
            PaintElement( g,  pen,  rect.rightTopPoint);

        }
        /// <summary>
        /// 绘制拖拽点
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="tractionPoint"></param>
        private void  PaintElement(Graphics g,Pen pen,TractionPoint tractionPoint)
        {
            PointF loca = this.ImageLocationToControl(tractionPoint.Location);
            g.FillRectangle(Brushes.Black, loca.X - tractionPoint.Size / 2, loca.Y - tractionPoint.Size / 2, tractionPoint.Size, tractionPoint.Size);
            g.DrawRectangle(Pens.White, loca.X - tractionPoint.Size / 2, loca.Y - tractionPoint.Size / 2, tractionPoint.Size, tractionPoint.Size);
        }
        #endregion
        #region

        #endregion
        #region 辅助方法
        private PointF ControlLocationToImage(PointF p)
        {
            PointF point = new PointF();
            point.X = (float)(p.X - imageLocationRectF.X) / ImageScale;
            point.Y = (float)(p.Y - imageLocationRectF.Y) / ImageScale;
            return point;
        }
        private PointF ControlLocationToImage(Point p)
        {
            PointF point = new PointF();
            point.X = (float)((float)p.X - imageLocationRectF.X) / ImageScale;
            point.Y = (float)((float)p.Y - imageLocationRectF.Y) / ImageScale;
            return point;
        }

        private PointF ImageLocationToControl(PointF p)
        {
            Point point = new Point();
            point.X = (int)(p.X*ImageScale + imageLocationRectF.X);
            point.Y = (int)(p.Y * ImageScale + imageLocationRectF.Y);
            return point;
        }
        #endregion

        private void LvImageView_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void LvImageView_MouseMove(object sender, MouseEventArgs e)
        {
            PointF p=ControlLocationToImage(e.Location);
            //System.Console.WriteLine("p:" + p.X + "  " + p.Y);
            if (mouseState == MouseState.Operating)
            {
                if (operatedElement != null)
                {
                    operatedElement.Move(ControlLocationToImage(e.Location));
                }
                else
                {
                    AbsoluteMove(new PointF(e.X - operationStartImagePoint.X * ImageScale, e.Y - operationStartImagePoint.Y * ImageScale));
                    
                }
                this.Refresh();
            }

        }

        private void LvImageView_MouseDown(object sender, MouseEventArgs e)
        {
            PointF p = ControlLocationToImage(new PointF(e.X, e.Y));
            this.operatedElement = this.GetTargetElement(p.X, p.Y);
            this.mouseState = MouseState.Operating;
            this.operationStartControlPoint = e.Location;
            this.operationStartImagePoint = ControlLocationToImage(e.Location);
            System.Console.WriteLine("operationStartImagePoint:" + operationStartImagePoint.X + "  " + operationStartImagePoint.Y);
        }

        private void LvImageView_MouseUp(object sender, MouseEventArgs e)
        {
            mouseState = MouseState.Idle;
            operatedElement = null;
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
}
