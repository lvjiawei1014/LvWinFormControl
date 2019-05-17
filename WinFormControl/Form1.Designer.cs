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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lvImageView1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvImageView1
            // 
            this.lvImageView1.AutoFit = false;
            this.lvImageView1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lvImageView1.Controls.Add(this.button2);
            this.lvImageView1.Controls.Add(this.button1);
            this.lvImageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvImageView1.Image = null;
            this.lvImageView1.ImageLocation = ((System.Drawing.PointF)(resources.GetObject("lvImageView1.ImageLocation")));
            this.lvImageView1.ImageScale = 1F;
            this.lvImageView1.Location = new System.Drawing.Point(0, 0);
            this.lvImageView1.Name = "lvImageView1";
            this.lvImageView1.Size = new System.Drawing.Size(1264, 681);
            this.lvImageView1.TabIndex = 0;
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
            this.ResumeLayout(false);

        }

        #endregion

        private LvImageView lvImageView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

