namespace IndustrialPark
{
    partial class ChangeImageScale
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeImageScale));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.lblPixels = new System.Windows.Forms.Label();
            this.btnApplyScale = new System.Windows.Forms.Button();
            this.btnAutoScale = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Height:";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(75, 45);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(142, 22);
            this.txtWidth.TabIndex = 2;
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(75, 73);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(142, 22);
            this.txtHeight.TabIndex = 3;
            // 
            // lblPixels
            // 
            this.lblPixels.AutoSize = true;
            this.lblPixels.Location = new System.Drawing.Point(12, 9);
            this.lblPixels.Name = "lblPixels";
            this.lblPixels.Size = new System.Drawing.Size(232, 16);
            this.lblPixels.TabIndex = 4;
            this.lblPixels.Text = "Change Scale Manually (In Pixels, PX)";
            // 
            // btnApplyScale
            // 
            this.btnApplyScale.Location = new System.Drawing.Point(12, 120);
            this.btnApplyScale.Name = "btnApplyScale";
            this.btnApplyScale.Size = new System.Drawing.Size(232, 35);
            this.btnApplyScale.TabIndex = 5;
            this.btnApplyScale.Text = "Apply Changes";
            this.btnApplyScale.UseVisualStyleBackColor = true;
            this.btnApplyScale.Click += new System.EventHandler(this.btnApplyScale_Click);
            // 
            // btnAutoScale
            // 
            this.btnAutoScale.Location = new System.Drawing.Point(12, 161);
            this.btnAutoScale.Name = "btnAutoScale";
            this.btnAutoScale.Size = new System.Drawing.Size(232, 35);
            this.btnAutoScale.TabIndex = 6;
            this.btnAutoScale.Text = "AutoScale to 128 x 128";
            this.btnAutoScale.UseVisualStyleBackColor = true;
            this.btnAutoScale.Click += new System.EventHandler(this.btnAutoScale_Click);
            // 
            // ChangeImageScale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 223);
            this.Controls.Add(this.btnAutoScale);
            this.Controls.Add(this.btnApplyScale);
            this.Controls.Add(this.lblPixels);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.txtWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChangeImageScale";
            this.Text = "Rescale Image";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label lblPixels;
        private System.Windows.Forms.Button btnApplyScale;
        private System.Windows.Forms.Button btnAutoScale;
    }
}