namespace IndustrialPark
{
    partial class ImportModel
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
            this.comboBoxAssetTypes = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonImportRawData = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.checkBoxFlipUVs = new System.Windows.Forms.CheckBox();
            this.checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            this.checkBoxGenSimps = new System.Windows.Forms.CheckBox();
            this.checkBoxEnableVcolors = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxAssetTypes
            // 
            this.comboBoxAssetTypes.FormattingEnabled = true;
            this.comboBoxAssetTypes.Location = new System.Drawing.Point(6, 19);
            this.comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            this.comboBoxAssetTypes.Size = new System.Drawing.Size(190, 21);
            this.comboBoxAssetTypes.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxAssetTypes);
            this.groupBox1.Location = new System.Drawing.Point(12, 211);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(202, 50);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Asset Type";
            this.groupBox1.Visible = false;
            // 
            // buttonImportRawData
            // 
            this.buttonImportRawData.Location = new System.Drawing.Point(18, 12);
            this.buttonImportRawData.Name = "buttonImportRawData";
            this.buttonImportRawData.Size = new System.Drawing.Size(190, 23);
            this.buttonImportRawData.TabIndex = 4;
            this.buttonImportRawData.Text = "Import";
            this.buttonImportRawData.UseVisualStyleBackColor = true;
            this.buttonImportRawData.Click += new System.EventHandler(this.buttonImportRawData_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(116, 313);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(92, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(18, 313);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(92, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 41);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(202, 164);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Assets";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(190, 134);
            this.listBox1.TabIndex = 9;
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            // 
            // checkBoxFlipUVs
            // 
            this.checkBoxFlipUVs.AutoSize = true;
            this.checkBoxFlipUVs.Location = new System.Drawing.Point(18, 267);
            this.checkBoxFlipUVs.Name = "checkBoxFlipUVs";
            this.checkBoxFlipUVs.Size = new System.Drawing.Size(65, 17);
            this.checkBoxFlipUVs.TabIndex = 7;
            this.checkBoxFlipUVs.Text = "Flip UVs";
            this.checkBoxFlipUVs.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverwrite
            // 
            this.checkBoxOverwrite.AutoSize = true;
            this.checkBoxOverwrite.Checked = true;
            this.checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOverwrite.Location = new System.Drawing.Point(137, 290);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(71, 17);
            this.checkBoxOverwrite.TabIndex = 12;
            this.checkBoxOverwrite.Text = "Overwrite";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // checkBoxGenSimps
            // 
            this.checkBoxGenSimps.AutoSize = true;
            this.checkBoxGenSimps.Location = new System.Drawing.Point(18, 290);
            this.checkBoxGenSimps.Name = "checkBoxGenSimps";
            this.checkBoxGenSimps.Size = new System.Drawing.Size(104, 17);
            this.checkBoxGenSimps.TabIndex = 13;
            this.checkBoxGenSimps.Text = "Generate SIMPs";
            this.checkBoxGenSimps.UseVisualStyleBackColor = true;
            // 
            // checkBoxEnableVcolors
            // 
            this.checkBoxEnableVcolors.AutoSize = true;
            this.checkBoxEnableVcolors.Checked = true;
            this.checkBoxEnableVcolors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnableVcolors.Location = new System.Drawing.Point(110, 267);
            this.checkBoxEnableVcolors.Name = "checkBoxEnableVcolors";
            this.checkBoxEnableVcolors.Size = new System.Drawing.Size(98, 17);
            this.checkBoxEnableVcolors.TabIndex = 14;
            this.checkBoxEnableVcolors.Text = "Enable VColors";
            this.checkBoxEnableVcolors.UseVisualStyleBackColor = true;
            // 
            // ImportModel
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(224, 345);
            this.Controls.Add(this.checkBoxEnableVcolors);
            this.Controls.Add(this.checkBoxGenSimps);
            this.Controls.Add(this.checkBoxOverwrite);
            this.Controls.Add(this.checkBoxFlipUVs);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonImportRawData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ImportModel";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Models";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxAssetTypes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonImportRawData;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox checkBoxFlipUVs;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;
        private System.Windows.Forms.CheckBox checkBoxGenSimps;
        private System.Windows.Forms.CheckBox checkBoxEnableVcolors;
    }
}