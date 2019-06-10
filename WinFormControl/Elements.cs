using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace LvControl.ImageView.Elements
{
    [Serializable()]
    public class KeyPoint
    {


        public delegate void KeyPointChangeEvent(KeyPoint kp, float x, float y);
        public KeyPointChangeEvent KeyPointChangeEventHandler;
        public TractionPoint tractionPoint;
        public Element Parent { get; set; }
        public float X { get { return tractionPoint.X; } set { tractionPoint.X = value; } }
        public float Y { get { return tractionPoint.Y; } set { tractionPoint.Y = value; } }
        public PointF Location { get { return new PointF(X, Y); } set { this.X = value.X; this.Y = value.Y; } }
        public KeyPoint(float x,float y,Element parent)
        {
            this.Parent = parent;
            this.tractionPoint = new TractionPoint(x, y, parent);
            tractionPoint.ParentCoordinate = parent.ParentCoordinate;
            tractionPoint.ParentElement = parent;
            tractionPoint.ElementCursor=Cursors.SizeAll;
            tractionPoint.Z=tractionPoint.ParentElement.Z-0.01f;
            tractionPoint.Visible = true;
            
            this.tractionPoint.OnTractionEventHandler += OnTraction;
            
        }

        public void OnTraction(TractionPoint elemant, float x, float y)
        {
            if (KeyPointChangeEventHandler != null)
            {
                KeyPointChangeEventHandler(this, X, Y);
            }
        }
    }

    [Serializable()]
    public abstract class Element:IComparable<Element>
    {
        #region 成员
        private bool selected = false;
        public Color ElementColor = Color.Blue;
        public string info;
        public static Cursor ElememtDefaultCursor = Cursors.Cross;
        /// <summary>
        /// 元素坐标系
        /// </summary>
        public Coordinate coordinate = new Coordinate();
        public bool isComplete = false;
        public static int PointAmount;
        public int PointCount;
        public Cursor ElementCursor = ElememtDefaultCursor;
        #endregion
        #region 属性
        public bool IsComplete { get { return isComplete; } set { this.isComplete = value; } }
        public string Name { get; set; }
        public virtual float X { get { return coordinate.X; } set { coordinate.X = value; } }
        public virtual float Y { get { return coordinate.Y; } set { coordinate.Y = value; } }
        public virtual float Z { get { return coordinate.Z; } set { coordinate.Z = value; } }
        public virtual float Width { get { return coordinate.Width; } set { coordinate.Width = value; } }
        public virtual float Height { get { return coordinate.Height; } set { coordinate.Height = value; } }
        public virtual PointF Location
        {
            get { return coordinate.Location; }
            set { this.X = value.X; this.Y = value.Y; }
        }
        public float Scale { get { return coordinate.Scale; } set { coordinate.Scale = value; } }
        /// <summary>
        /// 父元素
        /// </summary>
        public Element ParentElement { get; set; }
        /// <summary>
        /// 父坐标系
        /// </summary>
        public Coordinate ParentCoordinate { get; set; }
        /// <summary>
        /// 可见性
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool Selected { get { return selected; } set { this.selected = value; BeSelect(value); } }

        #endregion
        #region 事件
        public ElementChangeEvent ElementChangeEventHandler;
        #endregion

        public abstract void OnElementChange(Element element);

        public Element()
        {
            this.ElementChangeEventHandler = new ElementChangeEvent(OnElementChange);
        }
        public virtual bool AddKeyPoint(PointF point)
        {
            return false;
        }
        public virtual void AdjustNextKeyPoint(PointF point)
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
    [Serializable()]
    public abstract class MainElement : Element
    {
        public MainElement() : base() { }
    }
    [Serializable()]
    public abstract class KeyPointElement : MainElement
    {
        public static int KeyPointAmount;

        protected int keyPointCount;

        public List<KeyPoint> keyPointList = new List<KeyPoint>();

        public int KeyPointCount { get { return keyPointCount; }  }
        public KeyPointElement()
            : base()
        { }
    }
    [Serializable()]
    public class RectElement:KeyPointElement
    {
        public static Font DefaultFont = new Font("微软雅黑", 12f, FontStyle.Bold);
        public static Cursor ElementDefaultCursor = Cursors.SizeAll;
        public const float RECTANGLE_DEFAULT_Z = 2f;
        public new static int PointAmount = 2;
        public KeyPoint keyPoint1, keyPoint2;
        public TractionPoint leftTopPoint, leftBottomPoint, rightTopPoint, rightBottomPoint;
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
        public override float X
        {
            get
            {
                return leftTopPoint.X;
            }
            set
            {
                leftTopPoint.X = value;
                leftBottomPoint.X = value;
                OnElementChange(this);
            }
        }
        public override float Y
        {
            get
            {
                return leftTopPoint.Y;
            }
            set
            {
                leftTopPoint.Y = value;
                rightTopPoint.Y = value;
                OnElementChange(this);
            }
        }

        public override float Width
        {
            get
            {
                return rightBottomPoint.X - leftTopPoint.X;
            }
            set
            {
                rightTopPoint.X = value + leftTopPoint.X;
                rightBottomPoint.X = rightTopPoint.X;
                OnElementChange(this);
            }
        }

        public override float Height
        {
            get
            {
                return rightBottomPoint.Y - leftTopPoint.Y;
            }
            set
            {
                leftBottomPoint.Y = value + leftTopPoint.Y;
                rightBottomPoint.Y = leftBottomPoint.Y;
                OnElementChange(this);
            }
        }
        public override PointF Location
        {
            get
            {
                return new PointF(this.X, this.Y);
            }
            set
            {
                this.Move(value);
            }
        }
        public RectElement(): this(0f, 0f, 1f, 1f){}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public RectElement(float x, float y, float width, float height):base()
        {
            this.ElementCursor = RectElement.ElementDefaultCursor;
            this.leftTopPoint = new TractionPoint(this);
            this.leftBottomPoint = new TractionPoint(this);
            this.rightTopPoint = new TractionPoint(this);
            this.rightBottomPoint = new TractionPoint(this);
            keyPoint1 = new KeyPoint(0, 0, this);
            keyPoint2 = new KeyPoint(0, 0, this);
            keyPoint1.tractionPoint = leftTopPoint;
            keyPoint2.tractionPoint = rightBottomPoint;

            this.leftTopPoint.ElementCursor = Cursors.SizeNWSE;
            this.leftBottomPoint.ElementCursor = Cursors.SizeNESW;
            this.rightBottomPoint.ElementCursor = Cursors.SizeNWSE;
            this.rightTopPoint.ElementCursor = Cursors.SizeNESW;

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

        public void FlipVertical()
        {

            this.leftTopPoint.OnTractionEventHandler -= OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler -= OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler -= OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler -= OnRightBottomPointTraction;
            System.Console.WriteLine("vFlip");
            TractionPoint tmp = leftTopPoint;
            leftTopPoint = leftBottomPoint;
            leftBottomPoint = tmp;
            tmp = rightTopPoint;
            rightTopPoint = rightBottomPoint;
            rightBottomPoint = tmp;
            this.leftTopPoint.OnTractionEventHandler += OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler += OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler += OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler += OnRightBottomPointTraction;
            this.leftTopPoint.ElementCursor = Cursors.SizeNWSE;
            this.leftBottomPoint.ElementCursor = Cursors.SizeNESW;
            this.rightBottomPoint.ElementCursor = Cursors.SizeNWSE;
            this.rightTopPoint.ElementCursor = Cursors.SizeNESW;
            System.Console.WriteLine("lt==lb:" + object.ReferenceEquals(leftTopPoint ,leftBottomPoint));
        }
        public void FlipHorizontal()
        {
            this.leftTopPoint.OnTractionEventHandler -= OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler -= OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler -= OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler -= OnRightBottomPointTraction;
            System.Console.WriteLine("hFlip");
            TractionPoint tmp = leftTopPoint;
            leftTopPoint = rightTopPoint;
            rightTopPoint = tmp;
            tmp = leftBottomPoint;
            leftBottomPoint = rightBottomPoint;
            rightBottomPoint = tmp;

            this.leftTopPoint.OnTractionEventHandler += OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler += OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler += OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler += OnRightBottomPointTraction;
            this.leftTopPoint.ElementCursor = Cursors.SizeNWSE;
            this.leftBottomPoint.ElementCursor = Cursors.SizeNESW;
            this.rightBottomPoint.ElementCursor = Cursors.SizeNWSE;
            this.rightTopPoint.ElementCursor = Cursors.SizeNESW;
        }
        public void OnLeftTopPointTraction(TractionPoint element, float x, float y)
        {
            leftBottomPoint.X = x;
            rightTopPoint.Y = y;
            if (this.Height < 0)
            {
                FlipVertical();
            }
            if (this.Width < 0)
            {
                FlipHorizontal();
            }
            OnElementChange(this);
        }
        public void OnLeftBottomPointTraction(TractionPoint element, float x, float y)
        {
            leftTopPoint.X = x;
            rightBottomPoint.Y = y;
            if (this.Height<0)
            {
                FlipVertical();
            }
            if (this.Width<0)
            {
                FlipHorizontal();
            }
            OnElementChange(this);
        }
        public void OnRightTopPointTraction(TractionPoint element, float x, float y)
        {
            leftTopPoint.Y = y;
            rightBottomPoint.X = x;
            if (this.Width<0)
            {
                FlipHorizontal();
            }
            if (this.Height<0)
            {
                FlipVertical();
            }
            OnElementChange(this);
        }
        public void OnRightBottomPointTraction(TractionPoint element, float x, float y)
        {
            leftBottomPoint.Y = y;
            rightTopPoint.X = x;
            if (this.Width < 0)
            {
                FlipHorizontal();
            }
            if (this.Height < 0)
            {
                FlipVertical();
            }
            OnElementChange(this);
        }
        public override void BeSelect(bool b)
        {
            this.rightBottomPoint.Visible = b;
            this.rightTopPoint.Visible = b;
            this.leftBottomPoint.Visible = b;
            this.leftTopPoint.Visible = b;
        }
        public override void OnElementChange(Element element)
        {
            System.Console.WriteLine("leftTop:" + leftTopPoint.X + " " + leftTopPoint.Y);
            System.Console.WriteLine("leftBottom:" + leftBottomPoint.X + " " + leftBottomPoint.Y);
            System.Console.WriteLine("rightTop:" + rightTopPoint.X + " " + rightTopPoint.Y);
            System.Console.WriteLine("rightBottom:" + rightBottomPoint.X + " " + rightBottomPoint.Y);
        }

        public override void Draw(Graphics g, Pen p)
        {
            PointF loca = Coordinate.CoordinateTransport(this.Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
            g.DrawRectangle(p, loca.X, loca.Y, this.Width * ParentCoordinate.Scale, this.Height * ParentCoordinate.Scale);
            g.DrawString(this.info, RectElement.DefaultFont, Brushes.Blue, loca.X + 10, loca.Y + 10);
            if (this.Selected)
            {
                this.leftBottomPoint.Draw(g, p);
                this.leftTopPoint.Draw(g, p);
                this.rightBottomPoint.Draw(g, p);
                this.rightTopPoint.Draw(g, p);
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
            float h = this.Height;
            float w=this.Width;
            this.X = x;
            this.Y = y;
            this.Width = w;
            this.Height = h;
        }
        public override void Move(PointF p)
        {
            this.Move(p.X, p.Y);
        }
        public override bool AddKeyPoint(PointF point)
        {
            if (!this.IsComplete)
            {
                switch (PointCount)
                {
                    case 0:
                        this.Location = point;
                        PointCount++;
                        break;
                    case 1:
                        PointCount++;
                        break;
                    default:
                        break;
                }
                if (PointCount == RectElement.PointAmount)
                {
                    isComplete = true;
                }
            }
            return isComplete;
        }
        public override void AdjustNextKeyPoint(PointF point)
        {
            switch (PointCount)
            {
                case 1:
                    //this.Width = point.X - this.X;
                    //this.Height = point.Y - this.Y;
                    this.keyPoint2.tractionPoint.Traction(point);
                    break;
            }
        }
    }

    [Serializable()]
    public class EllipseElement : KeyPointElement
    {
        public static Font DefaultFont = new Font("微软雅黑", 12f, FontStyle.Bold);
        public static Cursor ElementDefaultCursor = Cursors.SizeAll;
        public const float CIRCLE_DEFAULT_Z = 2f;
        public new static int PointAmount = 2;
        public KeyPoint keyPoint1, keyPoint2;
        public TractionPoint leftTopPoint, leftBottomPoint, rightTopPoint, rightBottomPoint;
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
        public override float X
        {
            get
            {
                return leftTopPoint.X;
            }
            set
            {
                leftTopPoint.X = value;
                leftBottomPoint.X = value;
                OnElementChange(this);
            }
        }
        public override float Y
        {
            get
            {
                return leftTopPoint.Y;
            }
            set
            {
                leftTopPoint.Y = value;
                rightTopPoint.Y = value;
                OnElementChange(this);
            }
        }

        public override float Width
        {
            get
            {
                return rightBottomPoint.X - leftTopPoint.X;
            }
            set
            {
                rightTopPoint.X = value + leftTopPoint.X;
                rightBottomPoint.X = rightTopPoint.X;
                OnElementChange(this);
            }
        }

        public override float Height
        {
            get
            {
                return rightBottomPoint.Y - leftTopPoint.Y;
            }
            set
            {
                leftBottomPoint.Y = value + leftTopPoint.Y;
                rightBottomPoint.Y = leftBottomPoint.Y;
                OnElementChange(this);
            }
        }
        public override PointF Location
        {
            get
            {
                return new PointF(this.X, this.Y);
            }
            set
            {
                this.Move(value);
            }
        }
        public EllipseElement() : this(0f, 0f, 1f, 1f) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public EllipseElement(float x, float y, float width, float height) : base()
        {
            this.ElementCursor = RectElement.ElementDefaultCursor;
            this.leftTopPoint = new TractionPoint(this);
            this.leftBottomPoint = new TractionPoint(this);
            this.rightTopPoint = new TractionPoint(this);
            this.rightBottomPoint = new TractionPoint(this);
            keyPoint1 = new KeyPoint(0, 0, this);
            keyPoint2 = new KeyPoint(0, 0, this);
            keyPoint1.tractionPoint = leftTopPoint;
            keyPoint2.tractionPoint = rightBottomPoint;

            this.leftTopPoint.ElementCursor = Cursors.SizeNWSE;
            this.leftBottomPoint.ElementCursor = Cursors.SizeNESW;
            this.rightBottomPoint.ElementCursor = Cursors.SizeNWSE;
            this.rightTopPoint.ElementCursor = Cursors.SizeNESW;

            this.X = x;
            Y = y;
            Z = CIRCLE_DEFAULT_Z;
            this.Width = width;
            this.Height = height;

            this.leftTopPoint.OnTractionEventHandler += OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler += OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler += OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler += OnRightBottomPointTraction;
            Visible = true;
            ParentElement = null;
        }

        public void FlipVertical()
        {

            this.leftTopPoint.OnTractionEventHandler -= OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler -= OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler -= OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler -= OnRightBottomPointTraction;
            System.Console.WriteLine("vFlip");
            TractionPoint tmp = leftTopPoint;
            leftTopPoint = leftBottomPoint;
            leftBottomPoint = tmp;
            tmp = rightTopPoint;
            rightTopPoint = rightBottomPoint;
            rightBottomPoint = tmp;
            this.leftTopPoint.OnTractionEventHandler += OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler += OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler += OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler += OnRightBottomPointTraction;
            this.leftTopPoint.ElementCursor = Cursors.SizeNWSE;
            this.leftBottomPoint.ElementCursor = Cursors.SizeNESW;
            this.rightBottomPoint.ElementCursor = Cursors.SizeNWSE;
            this.rightTopPoint.ElementCursor = Cursors.SizeNESW;
            System.Console.WriteLine("lt==lb:" + object.ReferenceEquals(leftTopPoint, leftBottomPoint));
        }
        public void FlipHorizontal()
        {
            this.leftTopPoint.OnTractionEventHandler -= OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler -= OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler -= OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler -= OnRightBottomPointTraction;
            System.Console.WriteLine("hFlip");
            TractionPoint tmp = leftTopPoint;
            leftTopPoint = rightTopPoint;
            rightTopPoint = tmp;
            tmp = leftBottomPoint;
            leftBottomPoint = rightBottomPoint;
            rightBottomPoint = tmp;

            this.leftTopPoint.OnTractionEventHandler += OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler += OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler += OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler += OnRightBottomPointTraction;
            this.leftTopPoint.ElementCursor = Cursors.SizeNWSE;
            this.leftBottomPoint.ElementCursor = Cursors.SizeNESW;
            this.rightBottomPoint.ElementCursor = Cursors.SizeNWSE;
            this.rightTopPoint.ElementCursor = Cursors.SizeNESW;
        }
        public void OnLeftTopPointTraction(TractionPoint element, float x, float y)
        {
            leftBottomPoint.X = x;
            rightTopPoint.Y = y;
            if (this.Height < 0)
            {
                FlipVertical();
            }
            if (this.Width < 0)
            {
                FlipHorizontal();
            }
            OnElementChange(this);
        }
        public void OnLeftBottomPointTraction(TractionPoint element, float x, float y)
        {
            leftTopPoint.X = x;
            rightBottomPoint.Y = y;
            if (this.Height < 0)
            {
                FlipVertical();
            }
            if (this.Width < 0)
            {
                FlipHorizontal();
            }
            OnElementChange(this);
        }
        public void OnRightTopPointTraction(TractionPoint element, float x, float y)
        {
            leftTopPoint.Y = y;
            rightBottomPoint.X = x;
            if (this.Width < 0)
            {
                FlipHorizontal();
            }
            if (this.Height < 0)
            {
                FlipVertical();
            }
            OnElementChange(this);
        }
        public void OnRightBottomPointTraction(TractionPoint element, float x, float y)
        {
            leftBottomPoint.Y = y;
            rightTopPoint.X = x;
            if (this.Width < 0)
            {
                FlipHorizontal();
            }
            if (this.Height < 0)
            {
                FlipVertical();
            }
            OnElementChange(this);
        }
        public override void BeSelect(bool b)
        {
            this.rightBottomPoint.Visible = b;
            this.rightTopPoint.Visible = b;
            this.leftBottomPoint.Visible = b;
            this.leftTopPoint.Visible = b;
        }
        public override void OnElementChange(Element element)
        {
            System.Console.WriteLine("leftTop:" + leftTopPoint.X + " " + leftTopPoint.Y);
            System.Console.WriteLine("leftBottom:" + leftBottomPoint.X + " " + leftBottomPoint.Y);
            System.Console.WriteLine("rightTop:" + rightTopPoint.X + " " + rightTopPoint.Y);
            System.Console.WriteLine("rightBottom:" + rightBottomPoint.X + " " + rightBottomPoint.Y);
        }

        public override void Draw(Graphics g, Pen p)
        {
            PointF loca = Coordinate.CoordinateTransport(this.Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
            g.DrawEllipse(p, loca.X, loca.Y, this.Width * ParentCoordinate.Scale, this.Height * ParentCoordinate.Scale);
            g.DrawString(this.info, RectElement.DefaultFont, Brushes.Blue, loca.X + 10, loca.Y + 10);
            if (this.Selected)
            {
                this.leftBottomPoint.Draw(g, p);
                this.leftTopPoint.Draw(g, p);
                this.rightBottomPoint.Draw(g, p);
                this.rightTopPoint.Draw(g, p);
            }
        }
        public override bool IsIn(float x, float y)
        {
            float d = 6 / ParentCoordinate.Scale;
            float a = this.Width / 2;
            float b = this.Height / 2;
            float m = x - (this.X + a);
            float n = y - (this.Y + b);

            double tmp = m * m + a * a * n * n / (b * b);
            return (tmp > (a - d) * (a - d) && tmp < (a + d) * (a + d));
        }
        public override void Move(float x, float y)
        {
            float h = this.Height;
            float w = this.Width;
            this.X = x;
            this.Y = y;
            this.Width = w;
            this.Height = h;
        }
        public override void Move(PointF p)
        {
            this.Move(p.X, p.Y);
        }
        public override bool AddKeyPoint(PointF point)
        {
            if (!this.IsComplete)
            {
                switch (PointCount)
                {
                    case 0:
                        this.Location = point;
                        PointCount++;
                        break;
                    case 1:
                        PointCount++;
                        break;
                    default:
                        break;
                }
                if (PointCount == RectElement.PointAmount)
                {
                    isComplete = true;
                }
            }
            return isComplete;
        }
        public override void AdjustNextKeyPoint(PointF point)
        {
            switch (PointCount)
            {
                case 1:
                    //this.Width = point.X - this.X;
                    //this.Height = point.Y - this.Y;
                    this.keyPoint2.tractionPoint.Traction(point);
                    break;
            }
        }
    }


    public class PolygonElement:KeyPointElement
    {
        private KeyPoint tmpPoint;//绘图时的临时关键点

        public override float X
        {
            get
            {
                if (keyPointList.Count > 0) 
                {
                    return keyPointList[0].X; 
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (keyPointList.Count > 0)
                {
                    keyPointList[0].X=value;
                }
            }
        }

        public override float Y
        {
            get
            {
                if (keyPointList.Count > 0)
                {
                    return keyPointList[0].Y;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (keyPointList.Count > 0)
                {
                    keyPointList[0].Y = value;
                }
            }
        }

        public override PointF Location { get { return new PointF(this.X, this.Y); } set { this.X = value.X; this.Y = value.Y; } }
        public PolygonElement():base()
        {
            Z = 2f;
            this.ElementCursor = Cursors.SizeAll;
            tmpPoint = new KeyPoint(0, 0, this);
             
        }


        public void OnKeyPointChange(KeyPoint keyPoint,float x,float y)
        {


        }

        public bool Check()
        {
            return true;
        }
        public override bool AddKeyPoint(PointF point)
        {
            if (this.keyPointList.Count == 0)
            {
                this.tmpPoint.X = point.X;
                this.tmpPoint.Y = point.Y;
            }
            KeyPoint kp = new KeyPoint(point.X, point.Y,this);
            kp.KeyPointChangeEventHandler += OnKeyPointChange;
            this.keyPointList.Add(kp);
            return false;
        }
        public override void AdjustNextKeyPoint(PointF point)
        {
            tmpPoint.X = point.X;
            tmpPoint.Y = point.Y;
        }
        public override void Draw(Graphics g, Pen p)
        {
            if(p==null)
            {
                p = new Pen(this.ElementColor);
            }
            for(int i= 0;i<keyPointList.Count-1;i++)
            {
                PointF p1 = Coordinate.CoordinateTransport(keyPointList[i].Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
                PointF p2 = Coordinate.CoordinateTransport(keyPointList[i + 1].Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
                g.DrawLine(p, p1, p2);
            }
            if(this.isComplete && keyPointList.Count>0)
            {
                PointF p1 = Coordinate.CoordinateTransport(keyPointList.Last().Location,this.ParentCoordinate, Coordinate.BaseCoornidate);
                PointF p2 = Coordinate.CoordinateTransport(keyPointList.First().Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
                g.DrawLine(p, p1, p2);
            }
            if (this.Selected)
            {
                for (int i = 0; i < keyPointList.Count; i++)
                {
                    keyPointList[i].tractionPoint.Draw(g, p);
                }
            }
            if (tmpPoint != null && isComplete == false&& keyPointList.Count>0)
            {
                PointF p1 = Coordinate.CoordinateTransport(keyPointList.Last().Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
                PointF p2 = Coordinate.CoordinateTransport(tmpPoint.Location, this.ParentCoordinate, Coordinate.BaseCoornidate);
                g.DrawLine(p, p1, p2);
            }
        }


        public override bool IsIn(float x, float y)
        {
            PointF p=new PointF(x,y);
            for(int i=0;i<this.keyPointList.Count-1;i++){
                if(Geometry.IsPointOnLine(p,keyPointList[i].Location,keyPointList[i+1].Location,6f))
                {
                    return true;
                }
            }
            if(keyPointList.Count>2&&  Geometry.IsPointOnLine(p,keyPointList.Last().Location,keyPointList.First().Location,6f))
            {
                return true;
            }
            return false;
        }
        public override void BeSelect(bool b)
        {
            for (int i = 0; i < keyPointList.Count; i++)
            {
                keyPointList[i].tractionPoint.Visible = b;
            }
        }

        public override void Move(float x, float y)
        {
            float dx = x - this.X;
            float dy = y - this.Y;
            for(int i=0;i<keyPointList.Count;i++)
            {
                keyPointList[i].X += dx;
                keyPointList[i].Y += dy;
            }
        }

        public override void Move(PointF p)
        {
            this.Move(p.X,p.Y);
        }
        public override void OnElementChange(Element element)
        {
            
        }

        public bool Complete()
        {
             if(Check())
             {
                 this.IsComplete = true;
                 return true;
             }else
             {
                 return false;
             }
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
        public override float X { get { return base.X; } set { base.X = value; OnElementChange(null); } }
        public override float Y { get { return base.Y; } set { base.Y = value; OnElementChange(null); } }
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
            this.ElementCursor = RectElement.ElementDefaultCursor;
            for (int i = 0; i < Line.PointAmount; i++)
            {
                tractionPointList.Add( new TractionPoint(this));
                tractionPointList[i].ElementCursor = Cursors.SizeAll;
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

        public override void AdjustNextKeyPoint(PointF point)
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
            float dx = x - this.X;
            float dy = y - this.Y;
            this.X += dx;
            this.Y += dy;
            this.x2 += dx;
            this.y2 += dy;
            OnElementChange(this);
        }

        public override void Move(PointF p)
        {
            this.Move(p.X, p.Y);
        }

        public override bool IsIn(float x, float y)
        {
            PointF p=new PointF(x,y);
            return (Geometry.IsPointOnLine(p, this.Location, this.P2, 6f));
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
            this.Size = 6f;
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
    [Serializable()]
    public class ImageElement : MainElement
    {
        private Bitmap image;

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public Bitmap Image 
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


    [Serializable()]
    public delegate void ElementChangeEvent(Element element);
    [Serializable()]
    public delegate void ElementCreateEvent(Element elemant);
    


    /// <summary>
    /// 坐标系和尺寸
    /// </summary>
    [Serializable()]
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


    public class Geometry
    {
        public static bool IsPointOnLine(PointF p,PointF linePoint1,PointF linePoint2,float lineWidth)
        {
            float lineLength=(float)Math.Sqrt(Math.Pow(linePoint1.X-linePoint2.X,2)+Math.Pow(linePoint1.Y-linePoint2.Y,2));
            PointF center = new PointF(0.5f * (linePoint1.X + linePoint2.X), 0.5f * (linePoint1.Y + linePoint2.Y));
            if (linePoint1.X == linePoint2.X)
            {
                if (Math.Abs(p.X - center.X) < 0.5 * lineWidth && Math.Abs(p.Y - center.Y) < 0.5 * lineLength)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                float k1 = (linePoint1.Y - linePoint2.Y) / (linePoint1.X - linePoint2.X);
                float k2 = -1 / k1;
                float b1 = center.Y - k1 * center.X;
                float b2 = center.Y - k2 * center.X;
                float d1 = (float)Math.Abs((k1 * p.X - p.Y + b1) / Math.Sqrt(k1 * k1 + 1));
                float d2 = (float)Math.Abs((k2 * p.X - p.Y + b2) / Math.Sqrt(k2 * k2 + 1));
                if (d1 < 0.5 * lineWidth && d2 < 0.5 * lineLength)
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            
        }
    }
}
