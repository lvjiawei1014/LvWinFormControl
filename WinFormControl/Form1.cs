using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            lvImageView1.AddRectangle(new Rectangle(100f, 100f, 400f, 300f));
            image= Image.FromFile("1.jpg");
            image2 = Image.FromFile("2.jpg");
            lvImageView1.Image = image;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            lvImageView1.Image = image;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lvImageView1.Image = image2;
        }
    }
}
