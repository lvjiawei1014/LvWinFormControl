using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormControl
{   [ToolboxItem(true),ToolboxItemFilter("LvControl")]
    public partial class SignalLevel : Panel
    {
        private decimal value = 0;
        public decimal Value { get { return value; } set { this.value = value;Refresh(); } }
        //public Font Font { get; set; }
        public SignalLevel()
        {
            InitializeComponent();
            //Font = new Font("Arial",this.Height*0.75f, FontStyle.Regular);
        }

        private void SignalLevel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), 0f, 0f, this.Width, this.Height);
            SizeF size = e.Graphics.MeasureString(Value.ToString("0.0"), Font);
            if (Value <= 10)
            {
                e.Graphics.FillRectangle(Brushes.Black, 0f, 0f, this.Width / 4f, this.Height);
                e.Graphics.DrawString(Value.ToString("0.0"), Font, Brushes.White, (this.Width - size.Width) / 2f, (this.Height - size.Height) / 2f);
            }
            else if (Value <= 40)
            {
                e.Graphics.FillRectangle(Brushes.Blue, 0f, 0f, this.Width / 2f, this.Height);
                e.Graphics.DrawString(Value.ToString("0.0"), Font, Brushes.White, (this.Width - size.Width) / 2f, (this.Height - size.Height) / 2f);
            }
            else if (Value <= 90)
            {
                e.Graphics.FillRectangle(Brushes.Lime, 0f, 0f, this.Width*3 / 4f, this.Height);
                e.Graphics.DrawString(Value.ToString("0.0"), Font, Brushes.Black, (this.Width - size.Width) / 2f, (this.Height - size.Height) / 2f);
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.Red, 0f, 0f, this.Width , this.Height);
                e.Graphics.DrawString(Value.ToString("0.0"), Font, Brushes.Black, (this.Width - size.Width) / 2f, (this.Height - size.Height) / 2f);
            }

            e.Graphics.DrawRectangle(Pens.Black, 0f, 0f, this.Width-1, this.Height-1);
            
            

        }
    }
}
