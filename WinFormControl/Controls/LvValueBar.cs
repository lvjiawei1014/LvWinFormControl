using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormControl.Controls
{
    public partial class LvValueBar : Panel
    {
        public Color MainColor;

        public Color BaseColor;
        public Color BorderColor;
        public Color BlockColor;

        public float blockWidth = 20;
        private float blockHeight=20;
        private float maxValue = 100;
        public float MaxValue { get { return maxValue; } set { this.maxValue = value; Refresh(); } }
        private float minValue = 0;
        public float MinValue { get { return minValue; } set { this.minValue = value; Refresh(); } }
        private float value = 50;
        public float Value { get { return value; } set { this.value = value; Refresh(); } }
        public LvValueBar()
        {
            InitializeComponent();
            this.BaseColor = Color.LightGray;
            this.MainColor = Color.BlueViolet;
            this.BorderColor = Color.Black;
            this.BlockColor = Color.White;
            this.maxValue = 100;
            this.minValue=0;
            this.value=50;
        }

        private void LvValueBar_Paint(object sender, PaintEventArgs e)
        {
            blockHeight = this.Height * 0.8f;
            Brush baseBrush = new SolidBrush(BaseColor);
            Brush mainBrush = new SolidBrush(MainColor);
            Brush blockBrush=new SolidBrush(BlockColor);
            Pen borderPen = new Pen(BorderColor);

            e.Graphics.FillRectangle(baseBrush, this.blockWidth * 0.6f, this.Height * 0.3f, this.Width - blockWidth * 1.2f, this.Height * 0.4f);
            
            e.Graphics.FillRectangle(mainBrush, this.blockWidth * 0.6f, this.Height * 0.3f, (this.Width - blockWidth * 1.2f)*(value/(maxValue-minValue)), this.Height * 0.4f);
            e.Graphics.DrawRectangle(borderPen, this.blockWidth * 0.6f, this.Height * 0.3f, this.Width - blockWidth * 1.2f, this.Height * 0.4f);

            e.Graphics.FillRectangle(blockBrush, (this.Width - blockWidth * 1.2f) * (value / (maxValue - minValue)) + 0.6f * blockWidth - 0.5f * blockWidth, 0.1f * this.Height,blockWidth, blockHeight);
            e.Graphics.DrawRectangle(borderPen, (this.Width - blockWidth * 1.2f) * (value / (maxValue - minValue)) + 0.6f * blockWidth - 0.5f * blockWidth, 0.1f * this.Height, blockWidth, blockHeight);


        }
    }
}
