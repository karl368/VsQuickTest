namespace VsQuickTest
{
    partial class QuickForm
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
            this.helloCSharp = new System.Windows.Forms.Button();
            this.linqStatement = new System.Windows.Forms.Button();
            this.defineAndCallFunc = new System.Windows.Forms.Button();
            this.xmlToSql = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // helloCSharp
            // 
            this.helloCSharp.Location = new System.Drawing.Point(12, 12);
            this.helloCSharp.Name = "helloCSharp";
            this.helloCSharp.Size = new System.Drawing.Size(128, 28);
            this.helloCSharp.TabIndex = 0;
            this.helloCSharp.Text = "Hello C#";
            this.helloCSharp.UseVisualStyleBackColor = true;
            this.helloCSharp.Click += new System.EventHandler(this.button1_Click);
            // 
            // linqStatement
            // 
            this.linqStatement.Location = new System.Drawing.Point(12, 60);
            this.linqStatement.Name = "linqStatement";
            this.linqStatement.Size = new System.Drawing.Size(128, 28);
            this.linqStatement.TabIndex = 1;
            this.linqStatement.Text = "LinQ Statement";
            this.linqStatement.UseVisualStyleBackColor = true;
            this.linqStatement.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // defineAndCallFunc
            // 
            this.defineAndCallFunc.Location = new System.Drawing.Point(12, 115);
            this.defineAndCallFunc.Name = "defineAndCallFunc";
            this.defineAndCallFunc.Size = new System.Drawing.Size(128, 28);
            this.defineAndCallFunc.TabIndex = 3;
            this.defineAndCallFunc.Text = "Define And Call Func";
            this.defineAndCallFunc.UseVisualStyleBackColor = true;
            this.defineAndCallFunc.Click += new System.EventHandler(this.defineAndCallFunc_Click);
            // 
            // xmlToSql
            // 
            this.xmlToSql.Location = new System.Drawing.Point(12, 166);
            this.xmlToSql.Name = "xmlToSql";
            this.xmlToSql.Size = new System.Drawing.Size(128, 28);
            this.xmlToSql.TabIndex = 4;
            this.xmlToSql.Text = "WebClient Usage";
            this.xmlToSql.UseVisualStyleBackColor = true;
            this.xmlToSql.Click += new System.EventHandler(this.button4_Click);
            // 
            // QuickForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 382);
            this.Controls.Add(this.xmlToSql);
            this.Controls.Add(this.defineAndCallFunc);
            this.Controls.Add(this.linqStatement);
            this.Controls.Add(this.helloCSharp);
            this.Name = "QuickForm";
            this.Text = "QuickForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button helloCSharp;
        private System.Windows.Forms.Button linqStatement;
        private System.Windows.Forms.Button defineAndCallFunc;
        private System.Windows.Forms.Button xmlToSql;
    }
}