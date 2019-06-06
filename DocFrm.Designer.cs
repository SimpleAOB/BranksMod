namespace BranksMod
{
    partial class DocFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocFrm));
            this.TitleLbl = new System.Windows.Forms.Label();
            this.CommandsTitle = new System.Windows.Forms.Label();
            this.NotesTitle = new System.Windows.Forms.Label();
            this.CommandsBox = new System.Windows.Forms.RichTextBox();
            this.NotesBox = new System.Windows.Forms.RichTextBox();
            this.DescriptionBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // TitleLbl
            // 
            this.TitleLbl.BackColor = System.Drawing.Color.Transparent;
            this.TitleLbl.Cursor = System.Windows.Forms.Cursors.Default;
            this.TitleLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLbl.ForeColor = System.Drawing.Color.Black;
            this.TitleLbl.Location = new System.Drawing.Point(12, 9);
            this.TitleLbl.Name = "TitleLbl";
            this.TitleLbl.Size = new System.Drawing.Size(500, 25);
            this.TitleLbl.TabIndex = 25;
            this.TitleLbl.Text = "Title";
            this.TitleLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CommandsTitle
            // 
            this.CommandsTitle.BackColor = System.Drawing.Color.Transparent;
            this.CommandsTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.CommandsTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommandsTitle.ForeColor = System.Drawing.Color.Black;
            this.CommandsTitle.Location = new System.Drawing.Point(12, 90);
            this.CommandsTitle.Name = "CommandsTitle";
            this.CommandsTitle.Size = new System.Drawing.Size(500, 25);
            this.CommandsTitle.TabIndex = 27;
            this.CommandsTitle.Text = "Commands";
            this.CommandsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NotesTitle
            // 
            this.NotesTitle.BackColor = System.Drawing.Color.Transparent;
            this.NotesTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.NotesTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotesTitle.ForeColor = System.Drawing.Color.Black;
            this.NotesTitle.Location = new System.Drawing.Point(12, 371);
            this.NotesTitle.Name = "NotesTitle";
            this.NotesTitle.Size = new System.Drawing.Size(500, 25);
            this.NotesTitle.TabIndex = 29;
            this.NotesTitle.Text = "Additional notes";
            this.NotesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CommandsBox
            // 
            this.CommandsBox.BackColor = System.Drawing.Color.White;
            this.CommandsBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CommandsBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommandsBox.ForeColor = System.Drawing.Color.Black;
            this.CommandsBox.Location = new System.Drawing.Point(12, 118);
            this.CommandsBox.Name = "CommandsBox";
            this.CommandsBox.ReadOnly = true;
            this.CommandsBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.CommandsBox.Size = new System.Drawing.Size(500, 250);
            this.CommandsBox.TabIndex = 31;
            this.CommandsBox.Text = "";
            // 
            // NotesBox
            // 
            this.NotesBox.BackColor = System.Drawing.Color.White;
            this.NotesBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NotesBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotesBox.ForeColor = System.Drawing.Color.Black;
            this.NotesBox.Location = new System.Drawing.Point(12, 399);
            this.NotesBox.Name = "NotesBox";
            this.NotesBox.ReadOnly = true;
            this.NotesBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.NotesBox.Size = new System.Drawing.Size(500, 100);
            this.NotesBox.TabIndex = 32;
            this.NotesBox.Text = "";
            // 
            // DescriptionBox
            // 
            this.DescriptionBox.BackColor = System.Drawing.Color.White;
            this.DescriptionBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DescriptionBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionBox.ForeColor = System.Drawing.Color.Black;
            this.DescriptionBox.Location = new System.Drawing.Point(12, 37);
            this.DescriptionBox.Name = "DescriptionBox";
            this.DescriptionBox.ReadOnly = true;
            this.DescriptionBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.DescriptionBox.Size = new System.Drawing.Size(500, 50);
            this.DescriptionBox.TabIndex = 33;
            this.DescriptionBox.Text = "";
            // 
            // DocFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(524, 511);
            this.Controls.Add(this.DescriptionBox);
            this.Controls.Add(this.NotesBox);
            this.Controls.Add(this.CommandsBox);
            this.Controls.Add(this.NotesTitle);
            this.Controls.Add(this.CommandsTitle);
            this.Controls.Add(this.TitleLbl);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BranksMod - Plugin";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DocFrm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label TitleLbl;
        private System.Windows.Forms.Label CommandsTitle;
        private System.Windows.Forms.Label NotesTitle;
        private System.Windows.Forms.RichTextBox CommandsBox;
        private System.Windows.Forms.RichTextBox NotesBox;
        private System.Windows.Forms.RichTextBox DescriptionBox;
    }
}