namespace LvWinFormControlDemo
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lvValueBar1 = new WinFormControl.Controls.LvValueBar();
            this.signalLevel1 = new WinFormControl.SignalLevel();
            this.lvTab1 = new WinFormControl.Controls.LvTab.LvTab();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(37, 60);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(487, 141);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lvValueBar1
            // 
            this.lvValueBar1.Location = new System.Drawing.Point(108, 194);
            this.lvValueBar1.MaxValue = 100F;
            this.lvValueBar1.MinValue = 0F;
            this.lvValueBar1.Name = "lvValueBar1";
            this.lvValueBar1.Size = new System.Drawing.Size(293, 24);
            this.lvValueBar1.TabIndex = 5;
            this.lvValueBar1.Value = 50F;
            // 
            // signalLevel1
            // 
            this.signalLevel1.Location = new System.Drawing.Point(243, 86);
            this.signalLevel1.Name = "signalLevel1";
            this.signalLevel1.Size = new System.Drawing.Size(80, 20);
            this.signalLevel1.TabIndex = 4;
            this.signalLevel1.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lvTab1
            // 
            this.lvTab1.DarkColor = System.Drawing.Color.Empty;
            this.lvTab1.LightColor = System.Drawing.Color.Empty;
            this.lvTab1.Location = new System.Drawing.Point(72, 141);
            this.lvTab1.Name = "lvTab1";
            this.lvTab1.SelectedIndex = 0;
            this.lvTab1.Size = new System.Drawing.Size(393, 35);
            this.lvTab1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 274);
            this.Controls.Add(this.lvValueBar1);
            this.Controls.Add(this.signalLevel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lvTab1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private WinFormControl.Controls.LvTab.LvTab lvTab1;
        private System.Windows.Forms.Button button1;
        private WinFormControl.SignalLevel signalLevel1;
        private WinFormControl.Controls.LvValueBar lvValueBar1;

    }
}

