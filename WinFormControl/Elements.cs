using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinFormControl
{
    public class Element
    {

    }
    public class Rectangle:Element
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }

        public float Height { get; set; }

        public PointF Location
        {
            get
            {
                return new PointF(X, Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

    }
}
