namespace ElementDetector
{
    partial class ElementDetector
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
            this.openPage = new System.Windows.Forms.Button();
            this.openFirefox = new System.Windows.Forms.Button();
            this.screenshotGroupBox = new System.Windows.Forms.GroupBox();
            this.screenshotPictureBox = new System.Windows.Forms.PictureBox();
            this.closeFirefox = new System.Windows.Forms.Button();
            this.getScreenshot = new System.Windows.Forms.Button();
            this.screenshotGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // openPage
            // 
            this.openPage.Location = new System.Drawing.Point(223, 8);
            this.openPage.Name = "openPage";
            this.openPage.Size = new System.Drawing.Size(78, 29);
            this.openPage.TabIndex = 0;
            this.openPage.Text = "Open Page";
            this.openPage.UseVisualStyleBackColor = true;
            // 
            // openFirefox
            // 
            this.openFirefox.Location = new System.Drawing.Point(13, 8);
            this.openFirefox.Name = "openFirefox";
            this.openFirefox.Size = new System.Drawing.Size(78, 29);
            this.openFirefox.TabIndex = 1;
            this.openFirefox.Text = "Open Firefox";
            this.openFirefox.UseVisualStyleBackColor = true;
            this.openFirefox.Click += new System.EventHandler(this.openBrowser_Click);
            // 
            // screenshotGroupBox
            // 
            this.screenshotGroupBox.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.screenshotGroupBox.Controls.Add(this.screenshotPictureBox);
            this.screenshotGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.screenshotGroupBox.Location = new System.Drawing.Point(11, 41);
            this.screenshotGroupBox.Name = "screenshotGroupBox";
            this.screenshotGroupBox.Size = new System.Drawing.Size(696, 409);
            this.screenshotGroupBox.TabIndex = 2;
            this.screenshotGroupBox.TabStop = false;
            this.screenshotGroupBox.Text = "Screenshot";
            // 
            // screenshotPictureBox
            // 
            this.screenshotPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.screenshotPictureBox.Location = new System.Drawing.Point(3, 16);
            this.screenshotPictureBox.Name = "screenshotPictureBox";
            this.screenshotPictureBox.Size = new System.Drawing.Size(690, 390);
            this.screenshotPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.screenshotPictureBox.TabIndex = 0;
            this.screenshotPictureBox.TabStop = false;
            // 
            // closeFirefox
            // 
            this.closeFirefox.Location = new System.Drawing.Point(314, 8);
            this.closeFirefox.Name = "closeFirefox";
            this.closeFirefox.Size = new System.Drawing.Size(99, 29);
            this.closeFirefox.TabIndex = 3;
            this.closeFirefox.Text = "Close Firefox";
            this.closeFirefox.UseVisualStyleBackColor = true;
            this.closeFirefox.Click += new System.EventHandler(this.button3_Click);
            // 
            // getScreenshot
            // 
            this.getScreenshot.Location = new System.Drawing.Point(104, 8);
            this.getScreenshot.Name = "getScreenshot";
            this.getScreenshot.Size = new System.Drawing.Size(106, 29);
            this.getScreenshot.TabIndex = 4;
            this.getScreenshot.Text = "Get Screenshot";
            this.getScreenshot.UseVisualStyleBackColor = true;
            this.getScreenshot.Click += new System.EventHandler(this.getScreenshot_Click);
            // 
            // ElementDetector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 458);
            this.Controls.Add(this.getScreenshot);
            this.Controls.Add(this.closeFirefox);
            this.Controls.Add(this.screenshotGroupBox);
            this.Controls.Add(this.openFirefox);
            this.Controls.Add(this.openPage);
            this.Name = "ElementDetector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Element Detector";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.screenshotGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.screenshotPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button openPage;
        private System.Windows.Forms.Button openFirefox;
        private System.Windows.Forms.GroupBox screenshotGroupBox;
        private System.Windows.Forms.Button closeFirefox;
        private System.Windows.Forms.Button getScreenshot;
        private System.Windows.Forms.PictureBox screenshotPictureBox;
    }
}

