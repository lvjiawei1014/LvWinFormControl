using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LvWinFormControlDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.lvTab1.DarkColor = Color.BlueViolet;
            this.lvTab1.LightColor = Color.White;
            this.lvTab1.cornerRadius = 0;
            this.lvTab1.AddItem("1", "tab1");
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lvTab1.AddItem("2", "asdasd");
            lvTab1.SelectIndex(1);
        }
    }
}
