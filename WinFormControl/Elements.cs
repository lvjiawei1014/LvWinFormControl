using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinFormControl
{
    public abstract class Element:IComparable<Element>
    {
        //private PointF location = new PointF(0f, 0f);
        protected float x, y, z;
        public string Name { get; set; }
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Z { get { return z; } set { z = value; } }
        public PointF Location
        {
            get { return new PointF(X, Y); }
            set { this.X = value.X; this.Y = value.Y; }
        }
        public Element ParentElement{get;set;}
        public bool Visible{get;set;}

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
    /// <summary>
    /// 矩形元素
    /// </summary>
    public class Rectangle:Element
    {
        public const float RECTANGLE_DEFAULT_Z = 2f;

        private float height = 100, width = 100;

        public float X { get { return x; } set { x = value; OnRectChange(); } }
        public float Y { get { return y; } set { y = value; OnRectChange(); } }

        public float Width { get { return width; } set { width = value; OnRectChange(); } }

        public float Height { get { return height; } set { height = value; OnRectChange(); } }

        public TractionPoint leftTopPoint,leftBottomPoint,rightTopPoint,rightBottomPoint;

        public delegate void RectChangeEvent();
        public event RectChangeEvent RectChangeEventHandler;

        public Rectangle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.z = RECTANGLE_DEFAULT_Z;
            this.width = width;
            this.width = height;

            this.leftBottomPoint = new TractionPoint(X, Y+Height,this);
            this.leftTopPoint = new TractionPoint(X, Y,this);
            this.rightBottomPoint = new TractionPoint(X + Width, Y + Height,this);
            this.rightTopPoint = new TractionPoint(X + Width, Y,this);

            this.leftTopPoint.OnTractionEventHandler += OnLeftTopPointTraction;
            this.leftBottomPoint.OnTractionEventHandler += OnLeftBottomPointTraction;
            this.rightTopPoint.OnTractionEventHandler += OnRightTopPointTraction;
            this.rightBottomPoint.OnTractionEventHandler += OnRightBottomPointTraction;

            RectChangeEventHandler = new RectChangeEvent(OnRectChange);
            Visible = true;
            ParentElement = null;
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
            this.X = x;
            this.Height = y - this.Y;
        }
        public void OnRightTopPointTraction(float x, float y)
        {
            this.Width = x - this.X;
            this.Y = y;
        }
        public void OnRightBottomPointTraction(float x, float y)
        {
            this.Width = x - this.X;
            this.Height = y - this.Y;
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



        public TractionPoint(float x,float y)
        {
            this.Size = 6f;
            this.X = x;
            this.Y = y;
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

        public override void Move(float x, float y)
        {
            Traction(x, y);
        }

        public override void Move(PointF p)
        {
            Traction(p);
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
            bool b  = Math.Abs(this.X - x) < this.Size / 2 && Math.Abs(this.Y - y) < this.Size / 2;
            return b;
        }

        


    }
}
