namespace IndustrialPark
{
    partial class ChoosePlatformDialog
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.comboBoxAssetTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(15, 94);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(223, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "Confirm";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // comboBoxAssetTypes
            // 
            this.comboBoxAssetTypes.FormattingEnabled = true;
            this.comboBoxAssetTypes.Items.AddRange(new object[] {
            "GameCube",
            "Xbox",
            "Playstation 2"});
            this.comboBoxAssetTypes.Location = new System.Drawing.Point(15, 67);
            this.comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            this.comboBoxAssetTypes.Size = new System.Drawing.Size(223, 21);
            this.comboBoxAssetTypes.TabIndex = 1;
            this.comboBoxAssetTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxAssetTypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 55);
            this.label1.TabIndex = 6;
            this.label1.Text = "I have not been able to detect the platform of your HIP/HOP file.\r\nPlease pick it" +
    " from the dropdown (this will only be asked once upon saving)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ChoosePlatformDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 133);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxAssetTypes);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "ChoosePlatformDialog";
            this.ShowIcon = false;
            this.Text = "Choose Platform";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ComboBox comboBoxAssetTypes;
        private System.Windows.Forms.Label label1;
    }
}