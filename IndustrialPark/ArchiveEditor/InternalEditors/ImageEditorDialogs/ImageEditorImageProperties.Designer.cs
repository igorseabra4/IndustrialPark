namespace IndustrialPark
{
    partial class ImageEditorImageProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditorImageProperties));
            this.label1 = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.grpExtras = new System.Windows.Forms.GroupBox();
            this.txtFormat = new System.Windows.Forms.TextBox();
            this.grpExtras.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image Width:";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(103, 6);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.ReadOnly = true;
            this.txtWidth.Size = new System.Drawing.Size(125, 22);
            this.txtWidth.TabIndex = 1;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(103, 34);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.ReadOnly = true;
            this.txtHeight.Size = new System.Drawing.Size(125, 22);
            this.txtHeight.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Image Height:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Image Format:";
            // 
            // grpExtras
            // 
            this.grpExtras.Controls.Add(this.txtFormat);
            this.grpExtras.Controls.Add(this.label3);
            this.grpExtras.Location = new System.Drawing.Point(12, 74);
            this.grpExtras.Name = "grpExtras";
            this.grpExtras.Size = new System.Drawing.Size(429, 88);
            this.grpExtras.TabIndex = 5;
            this.grpExtras.TabStop = false;
            this.grpExtras.Text = "Format Data";
            // 
            // txtFormat
            // 
            this.txtFormat.Location = new System.Drawing.Point(105, 15);
            this.txtFormat.Name = "txtFormat";
            this.txtFormat.ReadOnly = true;
            this.txtFormat.Size = new System.Drawing.Size(318, 22);
            this.txtFormat.TabIndex = 6;
            // 
            // ImageEditorImageProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 168);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpExtras);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImageEditorImageProperties";
            this.Text = "Image Properties";
            this.Load += new System.EventHandler(this.ImageEditorImageProperties_Load);
            this.grpExtras.ResumeLayout(false);
            this.grpExtras.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grpExtras;
        private System.Windows.Forms.TextBox txtFormat;
    }
}