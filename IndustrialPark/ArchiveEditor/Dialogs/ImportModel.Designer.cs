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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportModel));
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
            resources.ApplyResources(this.comboBoxAssetTypes, "comboBoxAssetTypes");
            this.comboBoxAssetTypes.FormattingEnabled = true;
            this.comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            this.comboBoxAssetTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxAssetTypes_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.comboBoxAssetTypes);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // buttonImportRawData
            // 
            resources.ApplyResources(this.buttonImportRawData, "buttonImportRawData");
            this.buttonImportRawData.Name = "buttonImportRawData";
            this.buttonImportRawData.UseVisualStyleBackColor = true;
            this.buttonImportRawData.Click += new System.EventHandler(this.buttonImportRawData_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // listBox1
            // 
            resources.ApplyResources(this.listBox1, "listBox1");
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Name = "listBox1";
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            // 
            // checkBoxFlipUVs
            // 
            resources.ApplyResources(this.checkBoxFlipUVs, "checkBoxFlipUVs");
            this.checkBoxFlipUVs.Name = "checkBoxFlipUVs";
            this.checkBoxFlipUVs.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverwrite
            // 
            resources.ApplyResources(this.checkBoxOverwrite, "checkBoxOverwrite");
            this.checkBoxOverwrite.Checked = true;
            this.checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // checkBoxGenSimps
            // 
            resources.ApplyResources(this.checkBoxGenSimps, "checkBoxGenSimps");
            this.checkBoxGenSimps.Name = "checkBoxGenSimps";
            this.checkBoxGenSimps.UseVisualStyleBackColor = true;
            this.checkBoxGenSimps.CheckedChanged += new System.EventHandler(this.checkBoxGenSimps_CheckedChanged);
            // 
            // checkBoxEnableVcolors
            // 
            resources.ApplyResources(this.checkBoxEnableVcolors, "checkBoxEnableVcolors");
            this.checkBoxEnableVcolors.Name = "checkBoxEnableVcolors";
            this.checkBoxEnableVcolors.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreMeshColors
            // 
            resources.ApplyResources(this.checkBoxIgnoreMeshColors, "checkBoxIgnoreMeshColors");
            this.checkBoxIgnoreMeshColors.Checked = true;
            this.checkBoxIgnoreMeshColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreMeshColors.Name = "checkBoxIgnoreMeshColors";
            this.checkBoxIgnoreMeshColors.UseVisualStyleBackColor = true;
            // 
            // checkBoxLedgeGrab
            // 
            resources.ApplyResources(this.checkBoxLedgeGrab, "checkBoxLedgeGrab");
            this.checkBoxLedgeGrab.Name = "checkBoxLedgeGrab";
            this.checkBoxLedgeGrab.UseVisualStyleBackColor = true;
            // 
            // checkBoxSolidSimps
            // 
            resources.ApplyResources(this.checkBoxSolidSimps, "checkBoxSolidSimps");
            this.checkBoxSolidSimps.Checked = true;
            this.checkBoxSolidSimps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSolidSimps.Name = "checkBoxSolidSimps";
            this.checkBoxSolidSimps.UseVisualStyleBackColor = true;
            // 
            // grpImportSettings
            // 
            resources.ApplyResources(this.grpImportSettings, "grpImportSettings");
            this.grpImportSettings.Controls.Add(this.checkBoxIgnoreMeshColors);
            this.grpImportSettings.Controls.Add(this.checkBoxFlipUVs);
            this.grpImportSettings.Controls.Add(this.checkBoxEnableVcolors);
            this.grpImportSettings.Controls.Add(this.checkBoxOverwrite);
            this.grpImportSettings.Name = "grpImportSettings";
            this.grpImportSettings.TabStop = false;
            // 
            // grpSIMP
            // 
            resources.ApplyResources(this.grpSIMP, "grpSIMP");
            this.grpSIMP.Controls.Add(this.checkBoxLedgeGrab);
            this.grpSIMP.Controls.Add(this.checkBoxGenSimps);
            this.grpSIMP.Controls.Add(this.checkBoxSolidSimps);
            this.grpSIMP.Name = "grpSIMP";
            this.grpSIMP.TabStop = false;
            // 
            // ImportModel
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
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