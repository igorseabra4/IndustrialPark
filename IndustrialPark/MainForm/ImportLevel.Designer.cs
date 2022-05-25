namespace IndustrialPark
{
    partial class ImportLevel
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.grpImportSettings = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(367, 257);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(263, 257);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(98, 23);
            this.btnImport.TabIndex = 1;
            this.btnImport.Text = "Import Level";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // grpImportSettings
            // 
            this.grpImportSettings.Location = new System.Drawing.Point(12, 12);
            this.grpImportSettings.Name = "grpImportSettings";
            this.grpImportSettings.Size = new System.Drawing.Size(453, 239);
            this.grpImportSettings.TabIndex = 2;
            this.grpImportSettings.TabStop = false;
            this.grpImportSettings.Text = "Import Settings";
            // 
            // ImportLevel
            // 
            this.AcceptButton = this.btnImport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(477, 292);
            this.Controls.Add(this.grpImportSettings);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ImportLevel";
            this.ShowIcon = false;
            this.Text = "Import Level";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.GroupBox grpImportSettings;
    }
}