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
            this.checkBoxIgnoreMeshColors = new System.Windows.Forms.CheckBox();
            this.checkBoxLedgeGrab = new System.Windows.Forms.CheckBox();
            this.checkBoxSolidSimps = new System.Windows.Forms.CheckBox();
            this.grpImportSettings = new System.Windows.Forms.GroupBox();
            this.grpSIMP = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpImportSettings.SuspendLayout();
            this.grpSIMP.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxAssetTypes
            // 
            this.comboBoxAssetTypes.FormattingEnabled = true;
            this.comboBoxAssetTypes.Location = new System.Drawing.Point(6, 19);
            this.comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            this.comboBoxAssetTypes.Size = new System.Drawing.Size(248, 21);
            this.comboBoxAssetTypes.TabIndex = 1;
            this.comboBoxAssetTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxAssetTypes_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxAssetTypes);
            this.groupBox1.Location = new System.Drawing.Point(225, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 50);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Asset Type";
            // 
            // buttonImportRawData
            // 
            this.buttonImportRawData.Location = new System.Drawing.Point(18, 12);
            this.buttonImportRawData.Name = "buttonImportRawData";
            this.buttonImportRawData.Size = new System.Drawing.Size(190, 23);
            this.buttonImportRawData.TabIndex = 4;
            this.buttonImportRawData.Text = "Select Models...";
            this.buttonImportRawData.UseVisualStyleBackColor = true;
            this.buttonImportRawData.Click += new System.EventHandler(this.buttonImportRawData_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(295, 231);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(92, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "Import";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(393, 231);
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
            this.groupBox2.Size = new System.Drawing.Size(202, 213);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Assets";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(190, 186);
            this.listBox1.TabIndex = 9;
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            // 
            // checkBoxFlipUVs
            // 
            this.checkBoxFlipUVs.AutoSize = true;
            this.checkBoxFlipUVs.Location = new System.Drawing.Point(15, 19);
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
            this.checkBoxOverwrite.Location = new System.Drawing.Point(150, 37);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(71, 17);
            this.checkBoxOverwrite.TabIndex = 12;
            this.checkBoxOverwrite.Text = "Overwrite";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // checkBoxGenSimps
            // 
            this.checkBoxGenSimps.AutoSize = true;
            this.checkBoxGenSimps.Location = new System.Drawing.Point(15, 19);
            this.checkBoxGenSimps.Name = "checkBoxGenSimps";
            this.checkBoxGenSimps.Size = new System.Drawing.Size(104, 17);
            this.checkBoxGenSimps.TabIndex = 13;
            this.checkBoxGenSimps.Text = "Generate SIMPs";
            this.checkBoxGenSimps.UseVisualStyleBackColor = true;
            this.checkBoxGenSimps.CheckedChanged += new System.EventHandler(this.checkBoxGenSimps_CheckedChanged);
            // 
            // checkBoxEnableVcolors
            // 
            this.checkBoxEnableVcolors.AutoSize = true;
            this.checkBoxEnableVcolors.Location = new System.Drawing.Point(150, 19);
            this.checkBoxEnableVcolors.Name = "checkBoxEnableVcolors";
            this.checkBoxEnableVcolors.Size = new System.Drawing.Size(84, 17);
            this.checkBoxEnableVcolors.TabIndex = 14;
            this.checkBoxEnableVcolors.Text = "Create PIPT";
            this.checkBoxEnableVcolors.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreMeshColors
            // 
            this.checkBoxIgnoreMeshColors.AutoSize = true;
            this.checkBoxIgnoreMeshColors.Checked = true;
            this.checkBoxIgnoreMeshColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreMeshColors.Location = new System.Drawing.Point(15, 37);
            this.checkBoxIgnoreMeshColors.Name = "checkBoxIgnoreMeshColors";
            this.checkBoxIgnoreMeshColors.Size = new System.Drawing.Size(117, 17);
            this.checkBoxIgnoreMeshColors.TabIndex = 15;
            this.checkBoxIgnoreMeshColors.Text = "Ignore Mesh Colors";
            this.checkBoxIgnoreMeshColors.UseVisualStyleBackColor = true;
            // 
            // checkBoxLedgeGrab
            // 
            this.checkBoxLedgeGrab.AutoSize = true;
            this.checkBoxLedgeGrab.Enabled = false;
            this.checkBoxLedgeGrab.Location = new System.Drawing.Point(43, 37);
            this.checkBoxLedgeGrab.Name = "checkBoxLedgeGrab";
            this.checkBoxLedgeGrab.Size = new System.Drawing.Size(116, 17);
            this.checkBoxLedgeGrab.TabIndex = 16;
            this.checkBoxLedgeGrab.Text = "Ledge Grab SIMPs";
            this.checkBoxLedgeGrab.UseVisualStyleBackColor = true;
            // 
            // checkBoxSolidSimps
            // 
            this.checkBoxSolidSimps.AutoSize = true;
            this.checkBoxSolidSimps.Checked = true;
            this.checkBoxSolidSimps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSolidSimps.Enabled = false;
            this.checkBoxSolidSimps.Location = new System.Drawing.Point(43, 55);
            this.checkBoxSolidSimps.Name = "checkBoxSolidSimps";
            this.checkBoxSolidSimps.Size = new System.Drawing.Size(83, 17);
            this.checkBoxSolidSimps.TabIndex = 17;
            this.checkBoxSolidSimps.Text = "Solid SIMPs";
            this.checkBoxSolidSimps.UseVisualStyleBackColor = true;
            // 
            // grpImportSettings
            // 
            this.grpImportSettings.Controls.Add(this.checkBoxIgnoreMeshColors);
            this.grpImportSettings.Controls.Add(this.checkBoxFlipUVs);
            this.grpImportSettings.Controls.Add(this.checkBoxEnableVcolors);
            this.grpImportSettings.Controls.Add(this.checkBoxOverwrite);
            this.grpImportSettings.Location = new System.Drawing.Point(225, 68);
            this.grpImportSettings.Name = "grpImportSettings";
            this.grpImportSettings.Size = new System.Drawing.Size(261, 66);
            this.grpImportSettings.TabIndex = 18;
            this.grpImportSettings.TabStop = false;
            this.grpImportSettings.Text = "Import Settings";
            // 
            // grpSIMP
            // 
            this.grpSIMP.Controls.Add(this.checkBoxLedgeGrab);
            this.grpSIMP.Controls.Add(this.checkBoxGenSimps);
            this.grpSIMP.Controls.Add(this.checkBoxSolidSimps);
            this.grpSIMP.Location = new System.Drawing.Point(225, 140);
            this.grpSIMP.Name = "grpSIMP";
            this.grpSIMP.Size = new System.Drawing.Size(261, 82);
            this.grpSIMP.TabIndex = 19;
            this.grpSIMP.TabStop = false;
            this.grpSIMP.Text = "SIMP Settings";
            // 
            // ImportModel
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(497, 266);
            this.Controls.Add(this.grpSIMP);
            this.Controls.Add(this.grpImportSettings);
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
            this.grpImportSettings.ResumeLayout(false);
            this.grpImportSettings.PerformLayout();
            this.grpSIMP.ResumeLayout(false);
            this.grpSIMP.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.CheckBox checkBoxIgnoreMeshColors;
        private System.Windows.Forms.CheckBox checkBoxLedgeGrab;
        private System.Windows.Forms.CheckBox checkBoxSolidSimps;
        private System.Windows.Forms.GroupBox grpImportSettings;
        private System.Windows.Forms.GroupBox grpSIMP;
    }
}