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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelAssetName
            // 
            this.labelAssetName.AutoSize = true;
            this.labelAssetName.Location = new System.Drawing.Point(3, 0);
            this.labelAssetName.Name = "labelAssetName";
            this.labelAssetName.Size = new System.Drawing.Size(0, 13);
            this.labelAssetName.TabIndex = 6;
            // 
            // richTextBoxAssetText
            // 
            this.richTextBoxAssetText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxAssetText.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxAssetText.Location = new System.Drawing.Point(3, 23);
            this.richTextBoxAssetText.Name = "richTextBoxAssetText";
            this.richTextBoxAssetText.Size = new System.Drawing.Size(557, 222);
            this.richTextBoxAssetText.TabIndex = 7;
            this.richTextBoxAssetText.Text = "";
            this.richTextBoxAssetText.TextChanged += new System.EventHandler(this.richTextBoxAssetText_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelAssetName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.richTextBoxAssetText, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(563, 248);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // InternalTextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 248);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "InternalTextEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.RichTextBox richTextBoxAssetText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}