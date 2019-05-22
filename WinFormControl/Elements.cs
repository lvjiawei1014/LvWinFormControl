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


        public ElementChangeEvent ElementChangeEventHandler;

        public abstract void OnElementChange(Element element);

        public Element()
        {
            this.ElementChangeEventHandler = new ElementChangeEvent(OnElementChange);
        }
        public virtual bool AddKeyPoint(PointF point)
        {
            return false;
        }
        public virtual void AdjustLastKeyPoint(PointF point)
        {

        }

        public virtual void Draw(Graphics g, Pen p)
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
        public MainElement() : base() { }
    }
    /// <summary>
    /// 矩形元素
    /// </summary>
    public class Rectangle : MainElement
    {
        public static Cursor ElementDefaultCursor = Cursors.SizeAll;
        public const float RECTANGLE_DEFAULT_Z = 2f;
        public new static int PointAmount = 2;
        public new float X { get { return base.X; } set { base.X = value; OnElementChange(null); } }
        public new float Y { get { return base.Y; } set { base.Y = value; OnElementChange(null); } }

        public new float Width { get { return base.Width; } set { base.Width = value; OnElementChange(null); } }
        public new float Height { get { return base.Height; } set { base.Height = value; OnElementChange(null); } }

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
        public Rectangle(float x, float y, float width, float height):base()
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
            Visible = true;
            ParentElement = null;
        }
        public override void OnElementChange(Element element)
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

        public override void Draw(Graphics g, Pen p)
        {
            PointF loca = Coordinate.CoordinateTransport(this.Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
            g.DrawRectangle(p, loca.X, loca.Y, this.Width * ParentCoordinate.Scale, this.Height * ParentCoordinate.Scale);
            if (this.Selected)
            {
                this.leftBottomPoint.Draw(g, p);
                this.leftTopPoint.Draw(g, p);
                this.rightBottomPoint.Draw(g, p);
                this.rightTopPoint.Draw(g, p);
            }
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
        public void OnLeftTopPointTraction(TractionPoint element, float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public void OnLeftBottomPointTraction(TractionPoint element, float x, float y)
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
        public void OnRightTopPointTraction(TractionPoint element, float x, float y)
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
        public void OnRightBottomPointTraction(TractionPoint element, float x, float y)
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
            OnElementChange(this);
        }
        public override void Move(PointF p)
        {
            base.Move(p);
            OnElementChange(this);
        }

    }
    public class Line : MainElement
    {
        public static Cursor ElementDefaultCursor = Cursors.SizeAll;
        public const float LINE_DEFAULT_Z = 2f;
        public new static int PointAmount = 2;//关键点数量
        private float x2, y2;
        private List<TractionPoint> tractionPointList = new List<TractionPoint>();

        public List<TractionPoint> TractionPoints { get { return this.tractionPointList; } }
        public new float X { get { return base.X; } set { base.X = value; OnElementChange(null); } }
        public new float Y { get { return base.Y; } set { base.Y = value; OnElementChange(null); } }
        public float X2 { get { return x2; } set { this.x2 = value; OnElementChange(null); } }
        public float Y2 { get { return y2; } set { this.y2 = value; OnElementChange(null); } }

        public PointF P2 { get { return new PointF(X2, Y2); } }
        public new Coordinate ParentCoordinate
        {
            get
            {
                return base.ParentCoordinate;
            }
            set
            {
                base.ParentCoordinate = value;
                for (int i = 0; i < tractionPointList.Count; i++) { tractionPointList[i].ParentCoordinate = value; }
            }
        }

        public Line() : this(-10f, -10f, -5f, -5f) { }

        public Line(float x1, float y1, float x2, float y2):base()
        {
            this.ElememtCursor = Rectangle.ElementDefaultCursor;
            for (int i = 0; i < Line.PointAmount; i++)
            {
                tractionPointList.Add( new TractionPoint(this));
                tractionPointList[i].ElememtCursor = Cursors.SizeAll;
                tractionPointList[i].Name = "tractionPoint" + i;
                tractionPointList[i].OnTractionEventHandler += OnLinePointTraction;
                tractionPointList[i].ParentElement = this;
            }
            this.X = x1;
            this.Y = y1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Z = LINE_DEFAULT_Z;
            this.Visible = true;
            this.ParentElement = null;

        }
        public override void OnElementChange(Element element)
        {
            tractionPointList[0].X = this.X;
            tractionPointList[0].Y = this.Y;
            tractionPointList[1].X = this.X2;
            tractionPointList[1].Y = this.Y2;
        }

        public override void Draw(Graphics g, Pen p)
        {
            PointF p1 = Coordinate.CoordinateTransport(this.Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
            PointF p2 = Coordinate.CoordinateTransport(this.P2, this.ParentCoordinate, Coordinate.BaseCoornidate);

            g.DrawLine(p, p1, p2);
            if (true)
            {
                for (int i = 0; i < Line.PointAmount; i++)
                {
                    tractionPointList[i].Draw(g, p);
                }
            }
        }

        public override void BeSelect(bool b)
        {
            for (int i = 0; i < tractionPointList.Count; i++)
            {
                tractionPointList[i].Visible = true;
            }
        }

        public void OnLinePointTraction(TractionPoint tp, float x, float y)
        {
            switch (tp.Name)
            {
                case "tractionPoint0":
                    this.X = x;
                    this.Y = y;
                    break;
                case "tractionPoint1":
                    this.X2 = x;
                    this.Y2 = y;
                    break;
                default:
                    break;
            }
        }

        public override bool AddKeyPoint(PointF point)
        {
            if (!this.IsComplete)
            {
                switch (PointCount)
                {
                    case 0:
                        this.Location = point;
                        this.X2 = point.X;
                        this.Y2 = point.Y;
                        PointCount++;
                        break;
                    case 1:
                        this.X2 = point.X;
                        this.Y2 = point.Y;
                        PointCount++;
                        break;
                    default:
                        break;
                }
                if (PointCount == Line.PointAmount)
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
                    this.X2 = point.X;
                    this.Y2 = point.Y;
                    break;
            }
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);
            OnElementChange(this);
        }

        public override void Move(PointF p)
        {
            base.Move(p);
            OnElementChange(this);
        }

        public override bool IsIn(float x, float y)
        {
            return false;
        }


    }

    public class Ellipse : MainElement
    {
        public static Cursor ElementDefaultCursor = Cursors.SizeAll;
        public const float ELLIPSE_DEFAULT_Z = 2f;
        public new static int PointAmount = 2;//关键点数量
        private float x2, y2;
        private List<TractionPoint> tractionPointList = new List<TractionPoint>();

        public List<TractionPoint> TractionPoints { get { return this.tractionPointList; } }
        public new float X { get { return base.X; } set { base.X = value;  } }
        public new float Y { get { return base.Y; } set { base.Y = value;  } }
        public float X2 { get { return x2; } set { this.x2 = value;  } }
        public float Y2 { get { return y2; } set { this.y2 = value;  } }

        public PointF P2 { get { return new PointF(X2, Y2); } }
        public new Coordinate ParentCoordinate
        {
            get
            {
                return base.ParentCoordinate;
            }
            set
            {
                base.ParentCoordinate = value;
                for (int i = 0; i < tractionPointList.Count; i++) { tractionPointList[i].ParentCoordinate = value; }
            }
        }

        /// <summary>
        /// 核心构造方法
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public Ellipse(float x1, float y1, float x2, float y2):base()
        {
            this.ElememtCursor = Rectangle.ElementDefaultCursor;
            for (int i = 0; i < Line.PointAmount; i++)
            {
                tractionPointList.Add(new TractionPoint(this));
                tractionPointList[i].ElememtCursor = Cursors.SizeAll;
                tractionPointList[i].Name = "tractionPoint" + i;
                tractionPointList[i].OnTractionEventHandler += OnTraction;
                tractionPointList[i].ParentElement = this;
            }
            this.X = x1;
            this.Y = y1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Z = ELLIPSE_DEFAULT_Z;
            this.Visible = true;
            this.ParentElement = null;
        }
        public void OnTraction(TractionPoint tp, float x, float y)
        {
            switch (tp.Name)
            {
                case "tractionPoint0":
                    this.X = x;
                    this.Y = y;
                    break;
                case "tractionPoint1":
                    this.X2 = x;
                    this.Y2 = y;
                    break;
                default:
                    break;
            }
        }


        public override void Draw(Graphics g, Pen p)
        {
            //g.DrawEllipse()//
        }

        public override bool AddKeyPoint(PointF point)
        {
            return base.AddKeyPoint(point);
        }
        public override void AdjustLastKeyPoint(PointF point)
        {
            base.AdjustLastKeyPoint(point);
        }
        public override void BeSelect(bool b)
        {
            throw new NotImplementedException();
        }
        public override bool IsIn(float x, float y)
        {
            return base.IsIn(x, y);
        }

        public override void Move(float x, float y)
        {
            base.Move(x, y);
        }
        public override void Move(PointF p)
        {
            
            base.Move(p);
        }
        public override void OnElementChange(Element element)
        {
            throw new NotImplementedException();
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
        public delegate void TractionEvent(TractionPoint element, float x, float y);
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
        public TractionPoint(float x, float y,Element parent):base()
        {
            this.Size = 4f;
            this.X = x;
            this.Y = y;
            ParentElement = parent;
            this.Z = parent.Z + 0.01f;
        }

        public override void OnElementChange(Element element)
        {
            throw new NotImplementedException();
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
                OnTractionEventHandler(this,this.X, this.Y);
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
                OnTractionEventHandler(this,x, y);
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

        public override void Draw(Graphics g, Pen p)
        {
            PointF loca = Coordinate.CoordinateTransport(this.Location, this.ParentElement.ParentCoordinate, Coordinate.BaseCoornidate);
            g.FillRectangle(Brushes.Black, loca.X - this.Size / 2, loca.Y - this.Size / 2, this.Size, this.Size);
            g.DrawRectangle(Pens.White, loca.X - this.Size / 2, loca.Y - this.Size / 2, this.Size, this.Size);
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
        public ImageElement():base()
        {

        }

        public override void OnElementChange(Element element)
        {
            throw new NotImplementedException();
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


    public delegate void ElementChangeEvent(Element element);


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
