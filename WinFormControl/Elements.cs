using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormControl
{
    public abstract class Element:IComparable<Element>
    {
        private bool selected = false;

        public static Cursor ElememtDefaultCursor = Cursors.Cross;
        /// <summary>
        /// 元素坐标系
        /// </summary>
        public Coordinate coordinate=new Coordinate();
        public bool isComplete = false;
        public static int PointAmount;
        public int PointCount;
        public Cursor ElememtCursor = ElememtDefaultCursor;
        public bool IsComplete { get; set; }
        public CoordinateType CoordinateType { get; set; }
        public string Name { get; set; }
        public float X { get { return coordinate.X; } set { coordinate.X = value; } }
        public float Y { get { return coordinate.Y; } set { coordinate.Y = value; } }
        public float Z { get { return coordinate.Z; } set { coordinate.Z = value; } }
        public float Width { get { return coordinate.Width; } set { coordinate.Width = value; } }
        public float Height { get { return coordinate.Height; } set { coordinate.Height = value; } }
        public PointF Location
        {
            get { return coordinate.Location; }
            set { this.X = value.X; this.Y = value.Y; }
        }
        public float Scale { get { return coordinate.Scale; } set { coordinate.Scale = value; } }
        /// <summary>
        /// 父元素
        /// </summary>
        public Element ParentElement{get;set;}
        public Coordinate ParentCoordinate { get; set; }
        /// <summary>
        /// 可见性
        /// </summary>
        public bool Visible{get;set;}
        public bool Selected { get { return selected; } set { this.selected = value; BeSelect(value); } }

        public virtual bool AddKeyPoint(PointF point)
        {
            return false;
        }
        public virtual void AdjustLastKeyPoint(PointF point)
        {

        }

        public abstract void BeSelect(bool b);

        public virtual bool IsIn(float x, float y)
        {
            return false;
        }
        /// <summary>
        /// 移动元素
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void Move(float x,float y)
        {
            this.X = x;
            this.Y = y;
        }
        public virtual void Move(PointF p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }

        int IComparable<Element>.CompareTo(Element other)
        {
            return this.Z == other.Z ? 0 : (this.Z > other.Z ? 1 : -1);
        }
    }

    public abstract class MainElement : Element
    {

    }
    /// <summary>
    /// 矩形元素
    /// </summary>
    public class Rectangle : MainElement
    {
        public static Cursor ElementDefaultCursor = Cursors.SizeAll;
        public const float RECTANGLE_DEFAULT_Z = 2f;
        public static int PointAmount = 2;
        public new float X { get { return base.X; } set { base.X = value; OnRectChange(); } }
        public new float Y { get { return base.Y; } set { base.Y = value; OnRectChange(); } }

        public new float Width { get { return base.Width; } set { base.Width = value; OnRectChange(); } }
        public new float Height { get { return base.Height; } set { base.Height = value; OnRectChange(); } }

        public new Coordinate ParentCoordinate 
        { 
            get 
            {
                return base.ParentCoordinate;
            }
            set
            {
                base.ParentCoordinate = value;
                leftTopPoint.ParentCoordinate = value;
                leftBottomPoint.ParentCoordinate = value;
                rightBottomPoint.ParentCoordinate = value;
                rightTopPoint.ParentCoordinate = value;
            } 
        }

        public TractionPoint leftTopPoint,leftBottomPoint,rightTopPoint,rightBottomPoint;

        public delegate void RectChangeEvent();
        public event RectChangeEvent RectChangeEventHandler;

        public Rectangle()
            : this(0f, 0f, 1f, 1f)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(float x, float y, float width, float height)
        {
            this.ElememtCursor = Rectangle.ElementDefaultCursor;
            this.leftTopPoint = new TractionPoint(this);
            this.leftBottomPoint = new TractionPoint(this);
            this.rightTopPoint = new TractionPoint(this);
            this.rightBottomPoint = new TractionPoint(this);

            this.leftTopPoint.ElememtCursor = Cursors.SizeAll;
            this.leftBottomPoint.ElememtCursor = Cursors.SizeNESW;
            this.rightBottomPoint.ElememtCursor = Cursors.SizeNWSE;
            this.rightTopPoint.ElememtCursor = Cursors.SizeNESW;

            this.X = x;
            Y = y;
            Z = RECTANGLE_DEFAULT_Z;
            this.Width = width;
            this.Height = height;

            this.leftTopPoint.OnTractionEventHandler += OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler += OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler += OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler += OnRightBottomPointTraction;

            RectChangeEventHandler = new RectChangeEvent(OnRectChange);
            Visible = true;
            ParentElement = null;
        }

        /// <summary>
        /// 基类 的Selected属性被设置时触发
        /// </summary>
        /// <param name="b"></param>
        public override void BeSelect(bool b)
        {
            this.rightBottomPoint.Visible = b;
            this.rightTopPoint.Visible = b;
            this.leftBottomPoint.Visible = b;
            this.leftTopPoint.Visible = b;
        }
        public void OnRectChange()
        {
            this.leftTopPoint.X = X;
            this.leftTopPoint.Y = Y;
            this.leftBottomPoint.X = X;
            this.leftBottomPoint.Y = Y + Height;
            this.rightTopPoint.X = X + Width;
            this.rightTopPoint.Y = Y;
            this.rightBottomPoint.X = X + Width;
            this.rightBottomPoint.Y = Y + Height;
        }

        public void OnLeftTopPointTraction(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public void OnLeftBottomPointTraction(float x, float y)
        {
            float right = this.X + this.Width;
            this.X = x;
            this.Width = right - this.X;
            this.Height = y - this.Y;
            if (this.Width <= 0 || this.Height <= 0)
            {
                leftBottomPoint.MoveBack();
            }
        }
        public void OnRightTopPointTraction(float x, float y)
        {
            float bottom = this.Y + this.Height;
            this.Width = x - this.X;
            this.Y = y;
            this.Height = bottom - this.Y;
            if (this.Width <= 0 || this.Height <= 0)
            {
                rightTopPoint.MoveBack();
            }
        }
        public void OnRightBottomPointTraction(float x, float y)
        {
            this.Width = x - this.X;
            this.Height = y - this.Y;
            if (this.Width <= 0 || this.Height <= 0)
            {
                rightBottomPoint.MoveBack();
            }
        }
        public override bool AddKeyPoint(PointF point)
        {
            if(!this.IsComplete)
            {
                switch(PointCount)
                {
                    case 0:
                        this.Location = point;
                        PointCount++;
                        break;
                    case 1:
                        this.Width = point.X - this.X;
                        this.Height = point.Y - this.Y;
                        PointCount++;
                        break;
                    default:
                        break;
                }
                if(PointCount==Rectangle.PointAmount)
                {
                    isComplete = true;
                }
            }
            return isComplete;
        }
        public override void AdjustLastKeyPoint(PointF point)
        {
            switch (PointCount)
            {
                case 1:
                    this.Width = point.X - this.X;
                    this.Height = point.Y - this.Y;
                    break;
            }
        }
        public override bool IsIn(float x, float y)
        {
            return (x < (X + Width) && x > X && Math.Abs(y - Y) < 6 / ParentCoordinate.Scale)
                || (x < (X + Width) && x > X && Math.Abs(y - (Y + Height)) < 6 / ParentCoordinate.Scale)
                || (y > Y && y < (Y + Height) && Math.Abs(x - X) < 6 / ParentCoordinate.Scale)
                || (y > Y && y < (Y + Height) && Math.Abs(x - (X + Width)) < 6 / ParentCoordinate.Scale);
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);
            OnRectChange();
        }
        public override void Move(PointF p)
        {
            base.Move(p);
            OnRectChange();
        }

        private void SetTractionPointsVisiual(bool b)
        {
            leftBottomPoint.Visible = b;
        }
    }

    public class TractionPoint:Element
    {

        private PointF previousLocation=new PointF(0f,0f);
        public float Size { get; set; }
        /// <summary>
        /// 点位拖拽事件
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public delegate void TractionEvent(float x, float y);
        public event TractionEvent OnTractionEventHandler;



        public TractionPoint(Element parent)
        {
            ParentElement = parent;
            this.Size = 6f;
        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="parent"></param>
        public TractionPoint(float x, float y,Element parent)
        {
            this.Size = 4f;
            this.X = x;
            this.Y = y;
            ParentElement = parent;
            this.Z = parent.Z + 0.01f;
        }

        public override void BeSelect(bool b)
        {
            
        }
        public override void Move(float x, float y)
        {
            Traction(x, y);
        }

        public override void Move(PointF p)
        {
            Traction(p);
        }

        public void MoveBack()
        {
            this.X = previousLocation.X;
            this.Y = previousLocation.Y;
            if (OnTractionEventHandler != null)
            {
                OnTractionEventHandler(this.X, this.Y);
            }
        }
        /// <summary>
        /// 拖拽移动点位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Traction(float x,float y)
        {
            previousLocation.X = this.X;
            previousLocation.Y = this.Y;
            this.X = x;
            this.Y = y;
            if(OnTractionEventHandler!=null)
            {
                OnTractionEventHandler(x, y);
            }
        }

        public void Traction(PointF location)
        {
            Traction(location.X, location.Y);
        }

        public override bool IsIn(float x, float y)
        {
            if (this is TractionPoint)
            {
                bool b = Math.Abs(this.X - x) * ParentCoordinate.Scale < this.Size / 2 && Math.Abs(this.Y - y) * ParentCoordinate.Scale < this.Size / 2;
                return b && this.Visible;
            }
            return false;
        }

    }

    public class ImageElement : MainElement
    {
        private Image image;

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public Image Image 
        { 
            get { return image; }

            set { this.image = value; this.Width = (Image == null) ? 0 : image.Width; this.Height = (Image == null) ? 0 : image.Height; }
        }
        //public float ImageScale { get { return imageScale; } set { imageScale = value; } }
        public ImageElement()
        {

        }
        public void FitToWindow(int w,int h)
        {
            this.Scale = Math.Min(h / (float)image.Height, w / (float)image.Width);
            this.X = Math.Max(0, (w - image.Width *this.Scale) / 2);
            this.Y = Math.Max(0, (h - image.Height * this.Scale) / 2);
        }

        public void ScaleImage(Point anchor,float scale)
        {
            PointF imageAnchor = Coordinate.CoordinateTransport(anchor, Coordinate.BaseCoornidate, this.coordinate);
            this.X = anchor.X - scale * imageAnchor.X;
            this.Y = anchor.Y - scale * imageAnchor.Y;
            this.Scale = scale;
        }

        public override void BeSelect(bool b)
        {
            
        }

    }
    public enum CoordinateType
    {
        Base=0,
        Image=1,
    }

    /// <summary>
    /// 坐标系和尺寸
    /// </summary>
    public class Coordinate
    {

        /// <summary>
        /// 基本几何参数
        /// </summary>
        private float x, y, z, width, height,scale;
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Z { get { return z; } set { z = value; } }
        public float Width { get { return width; } set { width = value; } }
        public float Height { get { return height; } set { height = value; } }
        public PointF Location
        {
            get { return new PointF(X, Y); }
            set { this.X = value.X; this.Y = value.Y; }
        }
        public float Scale { get { return scale; } set { scale = value; } }
        public Coordinate(float x, float y, float scale)
        {
            Location = new PointF(x, y);
            Scale = scale;
        }
        public Coordinate(PointF location, float scale)
        {
            Location = location;
            Scale = scale;
        }
        public Coordinate()
        {
            Location = new PointF(0f,0f);
            Scale = 1f;
        }

        public static Coordinate BaseCoornidate=new Coordinate(0f,0f,1f);

        public static PointF CoordinateTransport(PointF p,Coordinate src,Coordinate dst)
        {
            PointF point = new PointF();
            point.X = (src.Location.X + p.X * src.Scale - dst.Location.X) / dst.Scale;
            point.Y = (src.Location.Y + p.Y * src.Scale - dst.Location.Y) / dst.Scale;
            return point;
        }
    }
}
