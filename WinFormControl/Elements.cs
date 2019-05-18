using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinFormControl
{
    public abstract class Element
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
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




    }
    /// <summary>
    /// 矩形元素
    /// </summary>
    public class Rectangle:Element
    {
        public float X { get; set; }
        public float Y { get; set; }
        
        public float Width { get; set; }

        public float Height { get; set; }

 

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Visible = true;
            ParentElement = null;
        }

    }

    public class TractionPoint:Element
    {


        public float Size { get; set; }
        /// <summary>
        /// 点位拖拽事件
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public delegate void TractionEvent(float x, float y);
        public TractionEvent OnTractionEventHandler;



        public TractionPoint(float x,float y)
        {
            this.Size = 4f;
            this.X = x;
            this.Y = y;
        }
        /// <summary>
        /// 拖拽移动点位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Traction(float x,float y)
        {
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
            return Math.Abs(this.X - x) < this.Size / 2 && Math.Abs(this.Y - y) < this.Size / 2;

        }

        


    }
}
