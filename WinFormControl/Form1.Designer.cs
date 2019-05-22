namespace WinFormControl
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lvImageView1 = new WinFormControl.LvImageView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnNormal = new System.Windows.Forms.Button();
            this.btnDrawRect = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnLine = new System.Windows.Forms.Button();
            this.lvImageView1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvImageView1
            // 
            this.lvImageView1.AutoFit = false;
            this.lvImageView1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lvImageView1.Controls.Add(this.btnLine);
            this.lvImageView1.Controls.Add(this.textBox1);
            this.lvImageView1.Controls.Add(this.btnEdit);
            this.lvImageView1.Controls.Add(this.btnNormal);
            this.lvImageView1.Controls.Add(this.btnDrawRect);
            this.lvImageView1.Controls.Add(this.button2);
            this.lvImageView1.Controls.Add(this.button1);
            this.lvImageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvImageView1.DrawingElementType = WinFormControl.ElementType.Image;
            this.lvImageView1.Image = null;
            this.lvImageView1.ImageLocation = ((System.Drawing.PointF)(resources.GetObject("lvImageView1.ImageLocation")));
            this.lvImageView1.ImageScale = 1F;
            this.lvImageView1.ImageViewState = WinFormControl.ImageViewState.Normal;
            this.lvImageView1.Location = new System.Drawing.Point(0, 0);
            this.lvImageView1.MouseState = WinFormControl.MouseState.Idle;
            this.lvImageView1.Name = "lvImageView1";
            this.lvImageView1.Size = new System.Drawing.Size(1264, 681);
            this.lvImageView1.TabIndex = 0;
            this.lvImageView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvImageView1_MouseMove);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 380);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(170, 21);
            this.textBox1.TabIndex = 6;
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(12, 270);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 5;
            this.btnEdit.Text = "编辑";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnNormal
            // 
            this.btnNormal.Location = new System.Drawing.Point(12, 241);
            this.btnNormal.Name = "btnNormal";
            this.btnNormal.Size = new System.Drawing.Size(75, 23);
            this.btnNormal.TabIndex = 4;
            this.btnNormal.Text = "查看";
            this.btnNormal.UseVisualStyleBackColor = true;
            this.btnNormal.Click += new System.EventHandler(this.btnNormal_Click);
            // 
            // btnDrawRect
            // 
            this.btnDrawRect.Location = new System.Drawing.Point(12, 70);
            this.btnDrawRect.Name = "btnDrawRect";
            this.btnDrawRect.Size = new System.Drawing.Size(75, 23);
            this.btnDrawRect.TabIndex = 3;
            this.btnDrawRect.Text = "画矩形";
            this.btnDrawRect.UseVisualStyleBackColor = true;
            this.btnDrawRect.Click += new System.EventHandler(this.btnDrawRect_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 41);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnLine
            // 
            this.btnLine.Location = new System.Drawing.Point(12, 99);
            this.btnLine.Name = "btnLine";
            this.btnLine.Size = new System.Drawing.Size(75, 23);
            this.btnLine.TabIndex = 7;
            this.btnLine.Text = "画直线";
            this.btnLine.UseVisualStyleBackColor = true;
            this.btnLine.Click += new System.EventHandler(this.btnLine_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.lvImageView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.lvImageView1.ResumeLayout(false);
            this.lvImageView1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private LvImageView lvImageView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnNormal;
        private System.Windows.Forms.Button btnDrawRect;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnLine;
    }
}

