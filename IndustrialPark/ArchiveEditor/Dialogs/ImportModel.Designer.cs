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
            comboBoxAssetTypes = new System.Windows.Forms.ComboBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            buttonImportRawData = new System.Windows.Forms.Button();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            groupBox2 = new System.Windows.Forms.GroupBox();
            listBox1 = new System.Windows.Forms.ListBox();
            checkBoxFlipUVs = new System.Windows.Forms.CheckBox();
            checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            checkBoxGenSimps = new System.Windows.Forms.CheckBox();
            checkBoxCreatePIPT = new System.Windows.Forms.CheckBox();
            checkBoxIgnoreMeshColors = new System.Windows.Forms.CheckBox();
            checkBoxLedgeGrab = new System.Windows.Forms.CheckBox();
            checkBoxSolidSimps = new System.Windows.Forms.CheckBox();
            grpImportSettings = new System.Windows.Forms.GroupBox();
            checkBoxSplitClump = new System.Windows.Forms.CheckBox();
            checkBoxNoMaterials = new System.Windows.Forms.CheckBox();
            grpSIMP = new System.Windows.Forms.GroupBox();
            checkBoxUseExistingDefaultLayer = new System.Windows.Forms.CheckBox();
            groupBoxModelSettings = new System.Windows.Forms.GroupBox();
            checkBoxBinMesh = new System.Windows.Forms.CheckBox();
            checkBoxMultiAtomic = new System.Windows.Forms.CheckBox();
            checkBoxNativeData = new System.Windows.Forms.CheckBox();
            checkBoxTriStrips = new System.Windows.Forms.CheckBox();
            checkBoxCollTree = new System.Windows.Forms.CheckBox();
            checkBoxGeoTriangles = new System.Windows.Forms.CheckBox();
            checkBoxVertexColors = new System.Windows.Forms.CheckBox();
            checkBoxNormals = new System.Windows.Forms.CheckBox();
            checkBoxTexCoords = new System.Windows.Forms.CheckBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            grpImportSettings.SuspendLayout();
            grpSIMP.SuspendLayout();
            groupBoxModelSettings.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxAssetTypes
            // 
            comboBoxAssetTypes.FormattingEnabled = true;
            resources.ApplyResources(comboBoxAssetTypes, "comboBoxAssetTypes");
            comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            comboBoxAssetTypes.SelectedIndexChanged += comboBoxAssetTypes_SelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBoxAssetTypes);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // buttonImportRawData
            // 
            resources.ApplyResources(buttonImportRawData, "buttonImportRawData");
            buttonImportRawData.Name = "buttonImportRawData";
            buttonImportRawData.UseVisualStyleBackColor = true;
            buttonImportRawData.Click += buttonImportRawData_Click;
            // 
            // buttonOK
            // 
            resources.ApplyResources(buttonOK, "buttonOK");
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.Name = "buttonOK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(listBox1);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // listBox1
            // 
            resources.ApplyResources(listBox1, "listBox1");
            listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBox1.FormattingEnabled = true;
            listBox1.Name = "listBox1";
            listBox1.KeyDown += listBox1_KeyDown;
            // 
            // checkBoxFlipUVs
            // 
            resources.ApplyResources(checkBoxFlipUVs, "checkBoxFlipUVs");
            checkBoxFlipUVs.Name = "checkBoxFlipUVs";
            checkBoxFlipUVs.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverwrite
            // 
            resources.ApplyResources(checkBoxOverwrite, "checkBoxOverwrite");
            checkBoxOverwrite.Checked = true;
            checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxOverwrite.Name = "checkBoxOverwrite";
            checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // checkBoxGenSimps
            // 
            resources.ApplyResources(checkBoxGenSimps, "checkBoxGenSimps");
            checkBoxGenSimps.Name = "checkBoxGenSimps";
            checkBoxGenSimps.UseVisualStyleBackColor = true;
            checkBoxGenSimps.CheckedChanged += checkBoxGenSimps_CheckedChanged;
            // 
            // checkBoxCreatePIPT
            // 
            resources.ApplyResources(checkBoxCreatePIPT, "checkBoxCreatePIPT");
            checkBoxCreatePIPT.Name = "checkBoxCreatePIPT";
            checkBoxCreatePIPT.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreMeshColors
            // 
            resources.ApplyResources(checkBoxIgnoreMeshColors, "checkBoxIgnoreMeshColors");
            checkBoxIgnoreMeshColors.Checked = true;
            checkBoxIgnoreMeshColors.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxIgnoreMeshColors.Name = "checkBoxIgnoreMeshColors";
            checkBoxIgnoreMeshColors.UseVisualStyleBackColor = true;
            // 
            // checkBoxLedgeGrab
            // 
            resources.ApplyResources(checkBoxLedgeGrab, "checkBoxLedgeGrab");
            checkBoxLedgeGrab.Name = "checkBoxLedgeGrab";
            checkBoxLedgeGrab.UseVisualStyleBackColor = true;
            // 
            // checkBoxSolidSimps
            // 
            resources.ApplyResources(checkBoxSolidSimps, "checkBoxSolidSimps");
            checkBoxSolidSimps.Checked = true;
            checkBoxSolidSimps.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxSolidSimps.Name = "checkBoxSolidSimps";
            checkBoxSolidSimps.UseVisualStyleBackColor = true;
            // 
            // grpImportSettings
            // 
            grpImportSettings.Controls.Add(checkBoxSplitClump);
            grpImportSettings.Controls.Add(checkBoxNoMaterials);
            grpImportSettings.Controls.Add(checkBoxFlipUVs);
            grpImportSettings.Controls.Add(checkBoxCreatePIPT);
            grpImportSettings.Controls.Add(checkBoxOverwrite);
            resources.ApplyResources(grpImportSettings, "grpImportSettings");
            grpImportSettings.Name = "grpImportSettings";
            grpImportSettings.TabStop = false;
            // 
            // checkBoxSplitClump
            // 
            resources.ApplyResources(checkBoxSplitClump, "checkBoxSplitClump");
            checkBoxSplitClump.Name = "checkBoxSplitClump";
            checkBoxSplitClump.UseVisualStyleBackColor = true;
            // 
            // checkBoxNoMaterials
            // 
            resources.ApplyResources(checkBoxNoMaterials, "checkBoxNoMaterials");
            checkBoxNoMaterials.Name = "checkBoxNoMaterials";
            checkBoxNoMaterials.UseVisualStyleBackColor = true;
            // 
            // grpSIMP
            // 
            grpSIMP.Controls.Add(checkBoxUseExistingDefaultLayer);
            grpSIMP.Controls.Add(checkBoxGenSimps);
            grpSIMP.Controls.Add(checkBoxSolidSimps);
            grpSIMP.Controls.Add(checkBoxLedgeGrab);
            resources.ApplyResources(grpSIMP, "grpSIMP");
            grpSIMP.Name = "grpSIMP";
            grpSIMP.TabStop = false;
            // 
            // checkBoxUseExistingDefaultLayer
            // 
            resources.ApplyResources(checkBoxUseExistingDefaultLayer, "checkBoxUseExistingDefaultLayer");
            checkBoxUseExistingDefaultLayer.Checked = true;
            checkBoxUseExistingDefaultLayer.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxUseExistingDefaultLayer.Name = "checkBoxUseExistingDefaultLayer";
            checkBoxUseExistingDefaultLayer.UseVisualStyleBackColor = true;
            // 
            // groupBoxModelSettings
            // 
            groupBoxModelSettings.Controls.Add(checkBoxBinMesh);
            groupBoxModelSettings.Controls.Add(checkBoxIgnoreMeshColors);
            groupBoxModelSettings.Controls.Add(checkBoxMultiAtomic);
            groupBoxModelSettings.Controls.Add(checkBoxNativeData);
            groupBoxModelSettings.Controls.Add(checkBoxTriStrips);
            groupBoxModelSettings.Controls.Add(checkBoxCollTree);
            groupBoxModelSettings.Controls.Add(checkBoxGeoTriangles);
            groupBoxModelSettings.Controls.Add(checkBoxVertexColors);
            groupBoxModelSettings.Controls.Add(checkBoxNormals);
            groupBoxModelSettings.Controls.Add(checkBoxTexCoords);
            resources.ApplyResources(groupBoxModelSettings, "groupBoxModelSettings");
            groupBoxModelSettings.Name = "groupBoxModelSettings";
            groupBoxModelSettings.TabStop = false;
            // 
            // checkBoxBinMesh
            // 
            resources.ApplyResources(checkBoxBinMesh, "checkBoxBinMesh");
            checkBoxBinMesh.Checked = true;
            checkBoxBinMesh.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxBinMesh.Name = "checkBoxBinMesh";
            checkBoxBinMesh.UseVisualStyleBackColor = true;
            // 
            // checkBoxMultiAtomic
            // 
            resources.ApplyResources(checkBoxMultiAtomic, "checkBoxMultiAtomic");
            checkBoxMultiAtomic.Name = "checkBoxMultiAtomic";
            checkBoxMultiAtomic.UseVisualStyleBackColor = true;
            // 
            // checkBoxNativeData
            // 
            resources.ApplyResources(checkBoxNativeData, "checkBoxNativeData");
            checkBoxNativeData.Name = "checkBoxNativeData";
            checkBoxNativeData.UseVisualStyleBackColor = true;
            checkBoxNativeData.CheckedChanged += checkBoxNativeData_CheckedChanged;
            // 
            // checkBoxTriStrips
            // 
            resources.ApplyResources(checkBoxTriStrips, "checkBoxTriStrips");
            checkBoxTriStrips.Name = "checkBoxTriStrips";
            checkBoxTriStrips.UseVisualStyleBackColor = true;
            // 
            // checkBoxCollTree
            // 
            resources.ApplyResources(checkBoxCollTree, "checkBoxCollTree");
            checkBoxCollTree.Checked = true;
            checkBoxCollTree.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxCollTree.Name = "checkBoxCollTree";
            checkBoxCollTree.UseVisualStyleBackColor = true;
            // 
            // checkBoxGeoTriangles
            // 
            resources.ApplyResources(checkBoxGeoTriangles, "checkBoxGeoTriangles");
            checkBoxGeoTriangles.Checked = true;
            checkBoxGeoTriangles.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxGeoTriangles.Name = "checkBoxGeoTriangles";
            checkBoxGeoTriangles.UseVisualStyleBackColor = true;
            checkBoxGeoTriangles.CheckedChanged += checkBoxGeoTriangles_CheckedChanged;
            // 
            // checkBoxVertexColors
            // 
            resources.ApplyResources(checkBoxVertexColors, "checkBoxVertexColors");
            checkBoxVertexColors.Name = "checkBoxVertexColors";
            checkBoxVertexColors.UseVisualStyleBackColor = true;
            // 
            // checkBoxNormals
            // 
            resources.ApplyResources(checkBoxNormals, "checkBoxNormals");
            checkBoxNormals.Checked = true;
            checkBoxNormals.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxNormals.Name = "checkBoxNormals";
            checkBoxNormals.UseVisualStyleBackColor = true;
            // 
            // checkBoxTexCoords
            // 
            resources.ApplyResources(checkBoxTexCoords, "checkBoxTexCoords");
            checkBoxTexCoords.Checked = true;
            checkBoxTexCoords.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxTexCoords.Name = "checkBoxTexCoords";
            checkBoxTexCoords.UseVisualStyleBackColor = true;
            // 
            // ImportModel
            // 
            AcceptButton = buttonOK;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            Controls.Add(groupBoxModelSettings);
            Controls.Add(grpSIMP);
            Controls.Add(grpImportSettings);
            Controls.Add(groupBox2);
            Controls.Add(buttonCancel);
            Controls.Add(groupBox1);
            Controls.Add(buttonOK);
            Controls.Add(buttonImportRawData);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ImportModel";
            ShowIcon = false;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            grpImportSettings.ResumeLayout(false);
            grpImportSettings.PerformLayout();
            grpSIMP.ResumeLayout(false);
            grpSIMP.PerformLayout();
            groupBoxModelSettings.ResumeLayout(false);
            groupBoxModelSettings.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox checkBoxUseExistingDefaultLayer;

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
        private System.Windows.Forms.CheckBox checkBoxCreatePIPT;
        private System.Windows.Forms.CheckBox checkBoxIgnoreMeshColors;
        private System.Windows.Forms.CheckBox checkBoxLedgeGrab;
        private System.Windows.Forms.CheckBox checkBoxSolidSimps;
        private System.Windows.Forms.GroupBox grpImportSettings;
        private System.Windows.Forms.GroupBox grpSIMP;
        private System.Windows.Forms.GroupBox groupBoxModelSettings;
        private System.Windows.Forms.CheckBox checkBoxCollTree;
        private System.Windows.Forms.CheckBox checkBoxGeoTriangles;
        private System.Windows.Forms.CheckBox checkBoxVertexColors;
        private System.Windows.Forms.CheckBox checkBoxNormals;
        private System.Windows.Forms.CheckBox checkBoxTexCoords;
        private System.Windows.Forms.CheckBox checkBoxMultiAtomic;
        private System.Windows.Forms.CheckBox checkBoxNativeData;
        private System.Windows.Forms.CheckBox checkBoxTriStrips;
        private System.Windows.Forms.CheckBox checkBoxBinMesh;
        private System.Windows.Forms.CheckBox checkBoxNoMaterials;
        private System.Windows.Forms.CheckBox checkBoxSplitClump;
    }
}