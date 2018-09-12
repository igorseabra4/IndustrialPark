namespace IndustrialPark
{
    partial class InternalTextEditor
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
            this.labelAssetName = new System.Windows.Forms.Label();
            this.richTextBoxAssetText = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // labelAssetName
            // 
            this.labelAssetName.AutoSize = true;
            this.labelAssetName.Location = new System.Drawing.Point(12, 9);
            this.labelAssetName.Name = "labelAssetName";
            this.labelAssetName.Size = new System.Drawing.Size(0, 13);
            this.labelAssetName.TabIndex = 6;
            // 
            // richTextBoxAssetText
            // 
            this.richTextBoxAssetText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxAssetText.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxAssetText.Location = new System.Drawing.Point(12, 25);
            this.richTextBoxAssetText.Name = "richTextBoxAssetText";
            this.richTextBoxAssetText.Size = new System.Drawing.Size(320, 202);
            this.richTextBoxAssetText.TabIndex = 7;
            this.richTextBoxAssetText.Text = "";
            this.richTextBoxAssetText.TextChanged += new System.EventHandler(this.richTextBoxAssetText_TextChanged);
            // 
            // InternalTextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 239);
            this.Controls.Add(this.richTextBoxAssetText);
            this.Controls.Add(this.labelAssetName);
            this.MaximizeBox = false;
            this.Name = "InternalTextEditor";
            this.ShowIcon = false;
            this.Text = "Internal Asset Editor";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.RichTextBox richTextBoxAssetText;
    }
}