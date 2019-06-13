using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormControl.Controls.LvTab
{
    public partial class LvTab : Panel
    {
        private int selectedIndex;
        private Color lightColor, darkColor;
        private Brush foreBrush, backBrush;
        public int cornerRadius;
        public bool AdjustableItemWidth;
        public float ItemWidth;
        public Color LightColor { get { return this.lightColor; } set { this.lightColor = value; this.backBrush = new SolidBrush(value); } }
        public Color DarkColor { get { return this.darkColor; } set { this.darkColor = value; this.foreBrush = new SolidBrush(value); } }

        public TabItemSelectedEvent OnTabItemSelectedHandler;
        public List<TabItem> Items;
        public TabItem SelectedItem { get { return this.Items[this.SelectedIndex]; } }
        public int SelectedIndex { get { return selectedIndex; } set { SelectIndex(value); } } 
        public LvTab()
        {

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Items = new List<TabItem>();
            this.OnTabItemSelectedHandler = new TabItemSelectedEvent(this.OnTabItemSelect);
            AdjustableItemWidth = true;
            ItemWidth = 32.0f;
            cornerRadius = 0;
            this.foreBrush = new SolidBrush(this.darkColor);
            this.backBrush = new SolidBrush(this.lightColor);

            this.AddItem("0", "default");
            this.SelectIndex(0);
        }
        public void OnTabItemSelect(int index)
        {
            this.Refresh();
        }
        public void AddItem(string name,string title)
        {
            Items.Add(new TabItem(title, name));
            UpdateLayout();
            this.Refresh();

        }
        public void RemoveItem(int index)
        {
            if(index<this.Items.Count){
                this.Items.RemoveAt(index);
                UpdateLayout();
                if (this.selectedIndex == index)
                {
                    this.SelectIndex(0);
                }
                else
                {
                    this.Refresh();
                }
            }
            
        }
        public void RemoveItem(TabItem item)
        {
            if (this.Items.Contains(item))
            {
                this.Items.Remove(item);
                UpdateLayout();
                if (item == this.SelectedItem)
                {
                    this.SelectIndex(0);
                }
                else
                {
                    this.Refresh();
                }
            }
        }

        public void SelectIndex(int index)
        {
            if (index >= Items.Count)
            {
                return ;
            }
            this.selectedIndex = index;
            this.OnTabItemSelectedHandler(index);
        }
        public void SelectItem(string name)
        {
            for(int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Name == name)
                {
                    this.selectedIndex = i;
                    this.OnTabItemSelectedHandler(i);
                }
            }
        }

        public void UpdateLayout()
        {
            int count = CountVisibleItem();
            if (count == 0) { count = 1; }
            if (AdjustableItemWidth)
            {
                ItemWidth = (float)(this.Width-2*cornerRadius) / count;
            }
            int index = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Visible)
                {
                    Items[i].Left = (int)(ItemWidth * index + cornerRadius);
                    index++;
                }
            }
        }
        public int CountVisibleItem()
        {
            int count = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                count += Items[i].Visible ? 1 : 0;
            }
            return count;

        }

        private void LvTab_Paint(object sender, PaintEventArgs e)
        {
            Pen darkPen=new Pen(this.darkColor);
            //画两边区域
            if(cornerRadius>0){
                e.Graphics.FillEllipse(this.SelectedIndex==0?this.foreBrush: this.backBrush, 0, 0, cornerRadius * 2, cornerRadius * 2);
                e.Graphics.FillEllipse(this.SelectedIndex == 0 ? this.foreBrush : this.backBrush, 0, this.Height - cornerRadius * 2-1, cornerRadius * 2, cornerRadius * 2);
                e.Graphics.FillEllipse(this.SelectedIndex==Items.Count-1?foreBrush:backBrush, this.Width-cornerRadius*2-1, 0, cornerRadius * 2, cornerRadius * 2);
                e.Graphics.FillEllipse(this.SelectedIndex == Items.Count - 1 ? foreBrush : this.backBrush, this.Width - cornerRadius * 2-1, this.Height - cornerRadius * 2-1, cornerRadius * 2, cornerRadius * 2);
                e.Graphics.DrawEllipse(darkPen, 0, 0, cornerRadius * 2, cornerRadius * 2);
                e.Graphics.DrawEllipse(darkPen, 0, this.Height - cornerRadius * 2 - 1, cornerRadius * 2, cornerRadius * 2);
                e.Graphics.DrawEllipse(darkPen, this.Width - cornerRadius * 2-1, 0, cornerRadius * 2, cornerRadius * 2);
                e.Graphics.DrawEllipse(darkPen, this.Width - cornerRadius * 2-1, this.Height - cornerRadius * 2 - 1, cornerRadius * 2, cornerRadius * 2);
                if (this.Height > cornerRadius * 2)
                {
                    e.Graphics.FillRectangle(this.SelectedIndex == 0 ? this.foreBrush : this.backBrush, 0, cornerRadius, this.cornerRadius, this.Height - cornerRadius * 2);
                    e.Graphics.FillRectangle(this.SelectedIndex == Items.Count - 1 ? foreBrush : backBrush, this.Width - cornerRadius, cornerRadius, this.cornerRadius, this.Height - cornerRadius * 2);
                    
                }
            }
            //画中间背景
            e.Graphics.FillRectangle(backBrush, cornerRadius, 0, this.Width - cornerRadius * 2, this.Height-1);
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Visible)
                {
                    if (Items[i] == this.SelectedItem)
                    {
                        e.Graphics.FillRectangle(foreBrush, Items[i].Left, Items[i].Top, this.ItemWidth, this.Height);
                        SizeF size=e.Graphics.MeasureString(Items[i].Title,Items[i].Font);
                        PointF titleLocation=new PointF(Items[i].Left + ItemWidth / 2 - size.Width / 2, this.Height / 2 - size.Height / 2+1);
                        e.Graphics.DrawString(Items[i].Title,this.Font,backBrush,titleLocation);
                    }
                    else
                    {
                        //e.Graphics.FillRectangle(backBrush, Items[i].Left, Items[i].Top, this.ItemWidth, this.Height);
                        SizeF size = e.Graphics.MeasureString(Items[i].Title, Items[i].Font);
                        PointF titleLocation = new PointF(Items[i].Left + ItemWidth / 2 - size.Width / 2, this.Height / 2 - size.Height / 2+1);
                        e.Graphics.DrawString(Items[i].Title, this.Font, foreBrush, titleLocation);
                    }
                }
            }
            //画分割线
            for (int i = 0; i < Items.Count-1; i++)
            {
                e.Graphics.DrawLine(darkPen, Items[i].Left + ItemWidth - 1, 0, Items[i].Left + ItemWidth - 1, Height - 1);
            }
            e.Graphics.DrawLine(darkPen, cornerRadius, 0, Width - cornerRadius - 1,0);
            e.Graphics.DrawLine(darkPen, cornerRadius, Height - 1, Width - cornerRadius - 1, Height - 1);
            e.Graphics.DrawLine(darkPen, 0, cornerRadius, 0, this.Height - cornerRadius);
            e.Graphics.DrawLine(darkPen, this.Width - 1, cornerRadius, this.Width - 1, this.Height - cornerRadius);

        }

        private void LvTab_Click(object sender, EventArgs e)
        { 
        }

        private void LvTab_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < this.cornerRadius)
            {
                SelectIndex(0);
            }
            else if (e.X > this.cornerRadius + this.Items.Count * ItemWidth)
            {
                SelectIndex(this.Items.Count - 1);
            }
            else
            {
                int index = (int)((e.X - cornerRadius) / ItemWidth);
                SelectIndex(index);
            }
        }
    }

    #region event
    public delegate void TabItemSelectedEvent(int index);

    #endregion

    public class TabItem:Label
    {
        public string Title { get; set; }
        public string Name { get; set; }

        public TabItem(string title,string name)
        {
            this.Title = title;
            this.Name = name;
            this.Enabled = true;
            this.Visible = true;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.AutoSize = false;

        }
    }
}
