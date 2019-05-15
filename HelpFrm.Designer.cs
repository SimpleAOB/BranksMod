namespace BranksMod
{
    partial class HelpFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpFrm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.RightBtn = new System.Windows.Forms.PictureBox();
            this.LeftBtn = new System.Windows.Forms.PictureBox();
            this.TroubleshootingBtn = new System.Windows.Forms.Label();
            this.ConsoleBtn = new System.Windows.Forms.Label();
            this.DollycamBtn = new System.Windows.Forms.Label();
            this.PageLbl = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.CurveballBtn = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RightBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LeftBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel1.Controls.Add(this.CurveballBtn);
            this.panel1.Controls.Add(this.DollycamBtn);
            this.panel1.Controls.Add(this.ConsoleBtn);
            this.panel1.Controls.Add(this.TroubleshootingBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(653, 35);
            this.panel1.TabIndex = 4;
            // 
            // RightBtn
            // 
            this.RightBtn.BackgroundImage = global::BranksMod.Properties.Resources.Right_Arrow;
            this.RightBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.RightBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RightBtn.Location = new System.Drawing.Point(591, 817);
            this.RightBtn.Name = "RightBtn";
            this.RightBtn.Size = new System.Drawing.Size(50, 50);
            this.RightBtn.TabIndex = 2;
            this.RightBtn.TabStop = false;
            // 
            // LeftBtn
            // 
            this.LeftBtn.BackgroundImage = global::BranksMod.Properties.Resources.Left_Arrow;
            this.LeftBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.LeftBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LeftBtn.Location = new System.Drawing.Point(12, 817);
            this.LeftBtn.Name = "LeftBtn";
            this.LeftBtn.Size = new System.Drawing.Size(50, 50);
            this.LeftBtn.TabIndex = 3;
            this.LeftBtn.TabStop = false;
            // 
            // TroubleshootingBtn
            // 
            this.TroubleshootingBtn.BackColor = System.Drawing.Color.Transparent;
            this.TroubleshootingBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.TroubleshootingBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TroubleshootingBtn.ForeColor = System.Drawing.Color.Black;
            this.TroubleshootingBtn.Location = new System.Drawing.Point(0, 0);
            this.TroubleshootingBtn.Name = "TroubleshootingBtn";
            this.TroubleshootingBtn.Size = new System.Drawing.Size(150, 35);
            this.TroubleshootingBtn.TabIndex = 15;
            this.TroubleshootingBtn.Text = "Troubleshooting";
            this.TroubleshootingBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TroubleshootingBtn.Click += new System.EventHandler(this.TroubleshootingBtn_Click);
            // 
            // ConsoleBtn
            // 
            this.ConsoleBtn.BackColor = System.Drawing.Color.Transparent;
            this.ConsoleBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.ConsoleBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConsoleBtn.ForeColor = System.Drawing.Color.Black;
            this.ConsoleBtn.Location = new System.Drawing.Point(150, 0);
            this.ConsoleBtn.Name = "ConsoleBtn";
            this.ConsoleBtn.Size = new System.Drawing.Size(150, 35);
            this.ConsoleBtn.TabIndex = 16;
            this.ConsoleBtn.Text = "Console";
            this.ConsoleBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ConsoleBtn.Click += new System.EventHandler(this.ConsoleBtn_Click);
            // 
            // DollycamBtn
            // 
            this.DollycamBtn.BackColor = System.Drawing.Color.White;
            this.DollycamBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.DollycamBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DollycamBtn.ForeColor = System.Drawing.Color.Black;
            this.DollycamBtn.Location = new System.Drawing.Point(300, 0);
            this.DollycamBtn.Name = "DollycamBtn";
            this.DollycamBtn.Size = new System.Drawing.Size(150, 35);
            this.DollycamBtn.TabIndex = 17;
            this.DollycamBtn.Text = "Dollycam";
            this.DollycamBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DollycamBtn.Click += new System.EventHandler(this.DollycamBtn_Click);
            // 
            // PageLbl
            // 
            this.PageLbl.BackColor = System.Drawing.Color.Transparent;
            this.PageLbl.Cursor = System.Windows.Forms.Cursors.Default;
            this.PageLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PageLbl.ForeColor = System.Drawing.Color.Black;
            this.PageLbl.Location = new System.Drawing.Point(68, 817);
            this.PageLbl.Name = "PageLbl";
            this.PageLbl.Size = new System.Drawing.Size(517, 50);
            this.PageLbl.TabIndex = 18;
            this.PageLbl.Text = "Page 1";
            this.PageLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::BranksMod.Properties.Resources.Dollycam_1;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Location = new System.Drawing.Point(0, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(653, 844);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // CurveballBtn
            // 
            this.CurveballBtn.BackColor = System.Drawing.Color.Transparent;
            this.CurveballBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.CurveballBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurveballBtn.ForeColor = System.Drawing.Color.Black;
            this.CurveballBtn.Location = new System.Drawing.Point(450, 0);
            this.CurveballBtn.Name = "CurveballBtn";
            this.CurveballBtn.Size = new System.Drawing.Size(150, 35);
            this.CurveballBtn.TabIndex = 18;
            this.CurveballBtn.Text = "Curveball";
            this.CurveballBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CurveballBtn.Click += new System.EventHandler(this.CurveballBtn_Click);
            // 
            // HelpFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(653, 879);
            this.Controls.Add(this.PageLbl);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.RightBtn);
            this.Controls.Add(this.LeftBtn);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BranksMod - Help";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.HelpFrm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RightBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LeftBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox RightBtn;
        private System.Windows.Forms.PictureBox LeftBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label DollycamBtn;
        private System.Windows.Forms.Label ConsoleBtn;
        private System.Windows.Forms.Label TroubleshootingBtn;
        private System.Windows.Forms.Label PageLbl;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label CurveballBtn;
    }
}