namespace AniPang
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.PanelMain = new System.Windows.Forms.Panel();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.txt_interval = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.labelTime = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.PanelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelMain
            // 
            this.PanelMain.Controls.Add(this.panelCenter);
            this.PanelMain.Controls.Add(this.panelRight);
            this.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelMain.Location = new System.Drawing.Point(0, 0);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(655, 570);
            this.PanelMain.TabIndex = 0;
            // 
            // panelCenter
            // 
            this.panelCenter.BackColor = System.Drawing.Color.Black;
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(0, 0);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(570, 570);
            this.panelCenter.TabIndex = 1;
            this.panelCenter.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCenter_Paint);
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.label1);
            this.panelRight.Controls.Add(this.txt_interval);
            this.panelRight.Controls.Add(this.button3);
            this.panelRight.Controls.Add(this.labelTime);
            this.panelRight.Controls.Add(this.button2);
            this.panelRight.Controls.Add(this.button1);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(570, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(85, 570);
            this.panelRight.TabIndex = 0;
            this.panelRight.Click += new System.EventHandler(this.button3_Click);
            this.panelRight.Paint += new System.Windows.Forms.PaintEventHandler(this.panelRight_Paint);
            // 
            // txt_interval
            // 
            this.txt_interval.Location = new System.Drawing.Point(45, 41);
            this.txt_interval.Name = "txt_interval";
            this.txt_interval.Size = new System.Drawing.Size(35, 21);
            this.txt_interval.TabIndex = 5;
            this.txt_interval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_interval.TextChanged += new System.EventHandler(this.txt_interval_TextChanged);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(5, 106);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 36);
            this.button3.TabIndex = 4;
            this.button3.Text = "Stop(F3)\r\nRestart";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(29, 91);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(29, 12);
            this.labelTime.TabIndex = 3;
            this.labelTime.Text = "----";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(5, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Start(F2)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(5, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load(F1)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 95;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "timer";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 570);
            this.Controls.Add(this.PanelMain);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "AniPang - TestVersion";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.PanelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelMain;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txt_interval;
        private System.Windows.Forms.Label label1;
    }
}

