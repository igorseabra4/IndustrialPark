namespace IndustrialPark
{
    partial class PlatSearch
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
            this.buttonChooseRoot = new System.Windows.Forms.Button();
            this.labelRootDir = new System.Windows.Forms.Label();
            this.buttonPerform = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // buttonChooseRoot
            // 
            this.buttonChooseRoot.Location = new System.Drawing.Point(12, 25);
            this.buttonChooseRoot.Name = "buttonChooseRoot";
            this.buttonChooseRoot.Size = new System.Drawing.Size(194, 23);
            this.buttonChooseRoot.TabIndex = 0;
            this.buttonChooseRoot.Text = "Choose Root Directory";
            this.buttonChooseRoot.UseVisualStyleBackColor = true;
            this.buttonChooseRoot.Click += new System.EventHandler(this.buttonChooseRoot_Click);
            // 
            // labelRootDir
            // 
            this.labelRootDir.AutoSize = true;
            this.labelRootDir.Location = new System.Drawing.Point(12, 9);
            this.labelRootDir.Name = "labelRootDir";
            this.labelRootDir.Size = new System.Drawing.Size(107, 13);
            this.labelRootDir.TabIndex = 1;
            this.labelRootDir.Text = "Root Directory: None";
            // 
            // buttonPerform
            // 
            this.buttonPerform.Location = new System.Drawing.Point(224, 25);
            this.buttonPerform.Name = "buttonPerform";
            this.buttonPerform.Size = new System.Drawing.Size(190, 23);
            this.buttonPerform.TabIndex = 8;
            this.buttonPerform.Text = "Perform Search";
            this.buttonPerform.UseVisualStyleBackColor = true;
            this.buttonPerform.Click += new System.EventHandler(this.buttonPerform_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(12, 83);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(402, 338);
            this.richTextBox2.TabIndex = 9;
            this.richTextBox2.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 54);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(402, 23);
            this.progressBar1.TabIndex = 10;
            // 
            // PlatSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 433);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.buttonPerform);
            this.Controls.Add(this.labelRootDir);
            this.Controls.Add(this.buttonChooseRoot);
            this.MaximizeBox = false;
            this.Name = "PlatSearch";
            this.ShowIcon = false;
            this.Text = "Plat Search";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EventSearch_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChooseRoot;
        private System.Windows.Forms.Label labelRootDir;
        private System.Windows.Forms.Button buttonPerform;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}