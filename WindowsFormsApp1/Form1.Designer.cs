namespace WindowsFormsApp1
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.另存位SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据DataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.视图ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.原始数据RawDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.光滑SmoothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.参数设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.参数自动ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件FToolStripMenuItem,
            this.数据DataToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开OpenToolStripMenuItem,
            this.保存SaveToolStripMenuItem,
            this.另存位SaveAsToolStripMenuItem});
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            this.文件FToolStripMenuItem.Size = new System.Drawing.Size(67, 21);
            this.文件FToolStripMenuItem.Text = "文件 File";
            // 
            // 打开OpenToolStripMenuItem
            // 
            this.打开OpenToolStripMenuItem.Name = "打开OpenToolStripMenuItem";
            this.打开OpenToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.打开OpenToolStripMenuItem.Text = "打开 Open";
            this.打开OpenToolStripMenuItem.Click += new System.EventHandler(this.打开OpenToolStripMenuItem_Click);
            // 
            // 保存SaveToolStripMenuItem
            // 
            this.保存SaveToolStripMenuItem.Name = "保存SaveToolStripMenuItem";
            this.保存SaveToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.保存SaveToolStripMenuItem.Text = "保存 Save";
            // 
            // 另存位SaveAsToolStripMenuItem
            // 
            this.另存位SaveAsToolStripMenuItem.Name = "另存位SaveAsToolStripMenuItem";
            this.另存位SaveAsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.另存位SaveAsToolStripMenuItem.Text = "另存位 Save As";
            this.另存位SaveAsToolStripMenuItem.Click += new System.EventHandler(this.另存位SaveAsToolStripMenuItem_Click);
            // 
            // 数据DataToolStripMenuItem
            // 
            this.数据DataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.视图ViewToolStripMenuItem});
            this.数据DataToolStripMenuItem.Name = "数据DataToolStripMenuItem";
            this.数据DataToolStripMenuItem.Size = new System.Drawing.Size(75, 21);
            this.数据DataToolStripMenuItem.Text = "数据 Data";
            // 
            // 视图ViewToolStripMenuItem
            // 
            this.视图ViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.原始数据RawDataToolStripMenuItem,
            this.光滑SmoothToolStripMenuItem});
            this.视图ViewToolStripMenuItem.Name = "视图ViewToolStripMenuItem";
            this.视图ViewToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.视图ViewToolStripMenuItem.Text = "视图 View";
            // 
            // 原始数据RawDataToolStripMenuItem
            // 
            this.原始数据RawDataToolStripMenuItem.Name = "原始数据RawDataToolStripMenuItem";
            this.原始数据RawDataToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.原始数据RawDataToolStripMenuItem.Text = "原始数据 Raw Data";
            this.原始数据RawDataToolStripMenuItem.Click += new System.EventHandler(this.原始数据RawDataToolStripMenuItem_Click);
            // 
            // 光滑SmoothToolStripMenuItem
            // 
            this.光滑SmoothToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.参数设置ToolStripMenuItem,
            this.参数自动ToolStripMenuItem});
            this.光滑SmoothToolStripMenuItem.Name = "光滑SmoothToolStripMenuItem";
            this.光滑SmoothToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.光滑SmoothToolStripMenuItem.Text = "光滑 Smooth";
            this.光滑SmoothToolStripMenuItem.Click += new System.EventHandler(this.光滑SmoothToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 424);
            this.panel1.TabIndex = 2;
            this.panel1.SizeChanged += new System.EventHandler(this.panel1_SizeChanged);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(531, 3);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 22);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(600, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(60, 21);
            this.textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(671, 3);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(60, 21);
            this.textBox2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(660, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "-";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(737, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(63, 21);
            this.button2.TabIndex = 7;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // 参数设置ToolStripMenuItem
            // 
            this.参数设置ToolStripMenuItem.Name = "参数设置ToolStripMenuItem";
            this.参数设置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.参数设置ToolStripMenuItem.Text = "点数设置";
            this.参数设置ToolStripMenuItem.Click += new System.EventHandler(this.参数设置ToolStripMenuItem_Click);
            // 
            // 参数自动ToolStripMenuItem
            // 
            this.参数自动ToolStripMenuItem.Name = "参数自动ToolStripMenuItem";
            this.参数自动ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.参数自动ToolStripMenuItem.Text = "点数：自动";
            this.参数自动ToolStripMenuItem.Click += new System.EventHandler(this.参数自动ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存SaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 另存位SaveAsToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem 数据DataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 视图ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 原始数据RawDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 光滑SmoothToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 参数设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 参数自动ToolStripMenuItem;
    }
}

