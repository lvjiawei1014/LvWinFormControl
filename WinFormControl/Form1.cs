using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LvControl.ImageView;
using LvControl.ImageView.Elements;

namespace WinFormControl
{
    public partial class Form1 : Form
    {
        Image image;
        Image image2;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lvImageView1.AutoFit = true;
            LvControl.ImageView.Elements.Rectangle rect = new LvControl.ImageView.Elements.Rectangle(100f, 100f, 400f, 300f);
            rect.Name = "rect";
            lvImageView1.AddRectangle(rect);
            image= Image.FromFile("1.jpg");
            image2 = Image.FromFile("2.jpg");
            lvImageView1.Image = image;
            //lvImageView1.CreateElement(ElementType.Rectangle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            lvImageView1.Image = image;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lvImageView1.Image = image2;
        }

        private void btnDrawRect_Click(object sender, EventArgs e)
        {
            lvImageView1.CreateElement(ElementType.Rectangle);
        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            lvImageView1.ChangeMode(ImageViewState.Normal);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            lvImageView1.ImageViewState = ImageViewState.Edit;
            lvImageView1.MouseState = MouseState.Idle;
        }

        private void lvImageView1_MouseMove(object sender, MouseEventArgs e)
        {
            this.textBox1.Text = lvImageView1.pointedElement.GetType().ToString();
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            lvImageView1.CreateElement(ElementType.Line);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            lvImageView1.DeleteElement(lvImageView1.selectedElement);

        }

    }
}
