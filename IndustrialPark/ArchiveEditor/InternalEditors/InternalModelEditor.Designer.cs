namespace IndustrialPark
{
    partial class InternalModelEditor
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
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.buttonApplyRotation = new System.Windows.Forms.Button();
            this.buttonApplyVertexColors = new System.Windows.Forms.Button();
            this.buttonApplyScale = new System.Windows.Forms.Button();
            this.groupBoxImport = new System.Windows.Forms.GroupBox();
            this.buttonImport = new System.Windows.Forms.Button();
            this.checkBoxIgnoreMeshColors = new System.Windows.Forms.CheckBox();
            this.checkBoxFilpUvs = new System.Windows.Forms.CheckBox();
            this.groupBoxExport = new System.Windows.Forms.GroupBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.checkBoxExportTextures = new System.Windows.Forms.CheckBox();
            this.buttonMaterialEditor = new System.Windows.Forms.Button();
            this.flowLayoutPanelTextures = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBoxTextures = new System.Windows.Forms.GroupBox();
            this.groupBoxAtomics = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelAtomics = new System.Windows.Forms.TableLayoutPanel();
            this.buttonEditAtomics = new System.Windows.Forms.Button();
            this.groupBoxPipeInfo = new System.Windows.Forms.GroupBox();
            this.comboBoxPiptPreset = new System.Windows.Forms.ComboBox();
            this.labelPreset = new System.Windows.Forms.Label();
            this.propertyGridPipeInfo = new System.Windows.Forms.PropertyGrid();
            this.buttonCreatePipeInfo = new System.Windows.Forms.Button();
            this.groupBoxLevelOfDetail = new System.Windows.Forms.GroupBox();
            this.propertyGridLevelOfDetail = new System.Windows.Forms.PropertyGrid();
            this.buttonCreateLevelOfDetail = new System.Windows.Forms.Button();
            this.groupBoxShadow = new System.Windows.Forms.GroupBox();
            this.propertyGridShadow = new System.Windows.Forms.PropertyGrid();
            this.buttonCreateShadow = new System.Windows.Forms.Button();
            this.groupBoxCollisionModel = new System.Windows.Forms.GroupBox();
            this.propertyGridCollision = new System.Windows.Forms.PropertyGrid();
            this.buttonCreateCollision = new System.Windows.Forms.Button();
            this.checkBoxUseTemplates = new System.Windows.Forms.CheckBox();
            this.groupBoxImport.SuspendLayout();
            this.groupBoxExport.SuspendLayout();
            this.groupBoxTextures.SuspendLayout();
            this.groupBoxAtomics.SuspendLayout();
            this.tableLayoutPanelAtomics.SuspendLayout();
            this.groupBoxPipeInfo.SuspendLayout();
            this.groupBoxLevelOfDetail.SuspendLayout();
            this.groupBoxShadow.SuspendLayout();
            this.groupBoxCollisionModel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(396, 554);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(120, 22);
            this.buttonHelp.TabIndex = 27;
            this.buttonHelp.Text = "Open Wiki Page";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.Location = new System.Drawing.Point(396, 582);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(120, 22);
            this.buttonFindCallers.TabIndex = 28;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // buttonApplyRotation
            // 
            this.buttonApplyRotation.Location = new System.Drawing.Point(18, 554);
            this.buttonApplyRotation.Name = "buttonApplyRotation";
            this.buttonApplyRotation.Size = new System.Drawing.Size(120, 22);
            this.buttonApplyRotation.TabIndex = 30;
            this.buttonApplyRotation.Text = "Apply Rotation";
            this.buttonApplyRotation.UseVisualStyleBackColor = true;
            this.buttonApplyRotation.Click += new System.EventHandler(this.buttonApplyRotation_Click);
            // 
            // buttonApplyVertexColors
            // 
            this.buttonApplyVertexColors.Location = new System.Drawing.Point(270, 554);
            this.buttonApplyVertexColors.Name = "buttonApplyVertexColors";
            this.buttonApplyVertexColors.Size = new System.Drawing.Size(120, 22);
            this.buttonApplyVertexColors.TabIndex = 29;
            this.buttonApplyVertexColors.Text = "Apply Vertex Colors";
            this.buttonApplyVertexColors.UseVisualStyleBackColor = true;
            this.buttonApplyVertexColors.Click += new System.EventHandler(this.buttonApplyVertexColors_Click);
            // 
            // buttonApplyScale
            // 
            this.buttonApplyScale.Location = new System.Drawing.Point(144, 554);
            this.buttonApplyScale.Name = "buttonApplyScale";
            this.buttonApplyScale.Size = new System.Drawing.Size(120, 22);
            this.buttonApplyScale.TabIndex = 31;
            this.buttonApplyScale.Text = "Apply Scale";
            this.buttonApplyScale.UseVisualStyleBackColor = true;
            this.buttonApplyScale.Click += new System.EventHandler(this.buttonApplyScale_Click);
            // 
            // groupBoxImport
            // 
            this.groupBoxImport.Controls.Add(this.buttonImport);
            this.groupBoxImport.Controls.Add(this.checkBoxIgnoreMeshColors);
            this.groupBoxImport.Controls.Add(this.checkBoxFilpUvs);
            this.groupBoxImport.Location = new System.Drawing.Point(396, 376);
            this.groupBoxImport.Name = "groupBoxImport";
            this.groupBoxImport.Size = new System.Drawing.Size(124, 93);
            this.groupBoxImport.TabIndex = 32;
            this.groupBoxImport.TabStop = false;
            this.groupBoxImport.Text = "Import Model";
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImport.Location = new System.Drawing.Point(6, 65);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(112, 22);
            this.buttonImport.TabIndex = 33;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // checkBoxIgnoreMeshColors
            // 
            this.checkBoxIgnoreMeshColors.AutoSize = true;
            this.checkBoxIgnoreMeshColors.Checked = true;
            this.checkBoxIgnoreMeshColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreMeshColors.Location = new System.Drawing.Point(6, 42);
            this.checkBoxIgnoreMeshColors.Name = "checkBoxIgnoreMeshColors";
            this.checkBoxIgnoreMeshColors.Size = new System.Drawing.Size(117, 17);
            this.checkBoxIgnoreMeshColors.TabIndex = 1;
            this.checkBoxIgnoreMeshColors.Text = "Ignore Mesh Colors";
            this.checkBoxIgnoreMeshColors.UseVisualStyleBackColor = true;
            // 
            // checkBoxFilpUvs
            // 
            this.checkBoxFilpUvs.AutoSize = true;
            this.checkBoxFilpUvs.Location = new System.Drawing.Point(6, 19);
            this.checkBoxFilpUvs.Name = "checkBoxFilpUvs";
            this.checkBoxFilpUvs.Size = new System.Drawing.Size(65, 17);
            this.checkBoxFilpUvs.TabIndex = 0;
            this.checkBoxFilpUvs.Text = "Flip UVs";
            this.checkBoxFilpUvs.UseVisualStyleBackColor = true;
            // 
            // groupBoxExport
            // 
            this.groupBoxExport.Controls.Add(this.buttonExport);
            this.groupBoxExport.Controls.Add(this.checkBoxExportTextures);
            this.groupBoxExport.Location = new System.Drawing.Point(396, 475);
            this.groupBoxExport.Name = "groupBoxExport";
            this.groupBoxExport.Size = new System.Drawing.Size(124, 73);
            this.groupBoxExport.TabIndex = 34;
            this.groupBoxExport.TabStop = false;
            this.groupBoxExport.Text = "Export Model";
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.Location = new System.Drawing.Point(6, 45);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(112, 22);
            this.buttonExport.TabIndex = 33;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // checkBoxExportTextures
            // 
            this.checkBoxExportTextures.AutoSize = true;
            this.checkBoxExportTextures.Location = new System.Drawing.Point(6, 19);
            this.checkBoxExportTextures.Name = "checkBoxExportTextures";
            this.checkBoxExportTextures.Size = new System.Drawing.Size(100, 17);
            this.checkBoxExportTextures.TabIndex = 0;
            this.checkBoxExportTextures.Text = "Export Textures";
            this.checkBoxExportTextures.UseVisualStyleBackColor = true;
            // 
            // buttonMaterialEditor
            // 
            this.buttonMaterialEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMaterialEditor.Location = new System.Drawing.Point(6, 144);
            this.buttonMaterialEditor.Name = "buttonMaterialEditor";
            this.buttonMaterialEditor.Size = new System.Drawing.Size(171, 22);
            this.buttonMaterialEditor.TabIndex = 35;
            this.buttonMaterialEditor.Text = "Open Material Editor";
            this.buttonMaterialEditor.UseVisualStyleBackColor = true;
            this.buttonMaterialEditor.Click += new System.EventHandler(this.buttonMaterialEditor_Click);
            // 
            // flowLayoutPanelTextures
            // 
            this.flowLayoutPanelTextures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelTextures.AutoScroll = true;
            this.flowLayoutPanelTextures.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelTextures.Location = new System.Drawing.Point(6, 19);
            this.flowLayoutPanelTextures.Name = "flowLayoutPanelTextures";
            this.flowLayoutPanelTextures.Size = new System.Drawing.Size(171, 119);
            this.flowLayoutPanelTextures.TabIndex = 36;
            this.flowLayoutPanelTextures.WrapContents = false;
            // 
            // groupBoxTextures
            // 
            this.groupBoxTextures.Controls.Add(this.flowLayoutPanelTextures);
            this.groupBoxTextures.Controls.Add(this.buttonMaterialEditor);
            this.groupBoxTextures.Location = new System.Drawing.Point(12, 376);
            this.groupBoxTextures.Name = "groupBoxTextures";
            this.groupBoxTextures.Size = new System.Drawing.Size(183, 172);
            this.groupBoxTextures.TabIndex = 34;
            this.groupBoxTextures.TabStop = false;
            this.groupBoxTextures.Text = "Textures";
            // 
            // groupBoxAtomics
            // 
            this.groupBoxAtomics.Controls.Add(this.tableLayoutPanelAtomics);
            this.groupBoxAtomics.Location = new System.Drawing.Point(201, 376);
            this.groupBoxAtomics.Name = "groupBoxAtomics";
            this.groupBoxAtomics.Size = new System.Drawing.Size(189, 172);
            this.groupBoxAtomics.TabIndex = 37;
            this.groupBoxAtomics.TabStop = false;
            this.groupBoxAtomics.Text = "Atomics";
            // 
            // tableLayoutPanelAtomics
            // 
            this.tableLayoutPanelAtomics.AutoScroll = true;
            this.tableLayoutPanelAtomics.ColumnCount = 2;
            this.tableLayoutPanelAtomics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelAtomics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelAtomics.Controls.Add(this.buttonEditAtomics, 1, 0);
            this.tableLayoutPanelAtomics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelAtomics.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanelAtomics.Name = "tableLayoutPanelAtomics";
            this.tableLayoutPanelAtomics.RowCount = 1;
            this.tableLayoutPanelAtomics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelAtomics.Size = new System.Drawing.Size(183, 153);
            this.tableLayoutPanelAtomics.TabIndex = 42;
            // 
            // buttonEditAtomics
            // 
            this.buttonEditAtomics.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonEditAtomics.Location = new System.Drawing.Point(3, 3);
            this.buttonEditAtomics.Name = "buttonEditAtomics";
            this.buttonEditAtomics.Size = new System.Drawing.Size(177, 23);
            this.buttonEditAtomics.TabIndex = 38;
            this.buttonEditAtomics.Text = "Edit";
            this.buttonEditAtomics.UseVisualStyleBackColor = true;
            this.buttonEditAtomics.Click += new System.EventHandler(this.buttonEditAtomics_Click);
            // 
            // groupBoxPipeInfo
            // 
            this.groupBoxPipeInfo.Controls.Add(this.comboBoxPiptPreset);
            this.groupBoxPipeInfo.Controls.Add(this.labelPreset);
            this.groupBoxPipeInfo.Controls.Add(this.propertyGridPipeInfo);
            this.groupBoxPipeInfo.Controls.Add(this.buttonCreatePipeInfo);
            this.groupBoxPipeInfo.Location = new System.Drawing.Point(12, 12);
            this.groupBoxPipeInfo.Name = "groupBoxPipeInfo";
            this.groupBoxPipeInfo.Size = new System.Drawing.Size(251, 258);
            this.groupBoxPipeInfo.TabIndex = 41;
            this.groupBoxPipeInfo.TabStop = false;
            this.groupBoxPipeInfo.Text = "Pipe Info";
            // 
            // comboBoxPiptPreset
            // 
            this.comboBoxPiptPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPiptPreset.FormattingEnabled = true;
            this.comboBoxPiptPreset.Location = new System.Drawing.Point(123, 18);
            this.comboBoxPiptPreset.Name = "comboBoxPiptPreset";
            this.comboBoxPiptPreset.Size = new System.Drawing.Size(122, 21);
            this.comboBoxPiptPreset.TabIndex = 44;
            this.comboBoxPiptPreset.SelectedIndexChanged += new System.EventHandler(this.comboBoxPiptPreset_SelectedIndexChanged);
            // 
            // labelPreset
            // 
            this.labelPreset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPreset.AutoSize = true;
            this.labelPreset.Location = new System.Drawing.Point(77, 21);
            this.labelPreset.Name = "labelPreset";
            this.labelPreset.Size = new System.Drawing.Size(40, 13);
            this.labelPreset.TabIndex = 44;
            this.labelPreset.Text = "Preset:";
            // 
            // propertyGridPipeInfo
            // 
            this.propertyGridPipeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridPipeInfo.Enabled = false;
            this.propertyGridPipeInfo.HelpVisible = false;
            this.propertyGridPipeInfo.Location = new System.Drawing.Point(6, 45);
            this.propertyGridPipeInfo.Name = "propertyGridPipeInfo";
            this.propertyGridPipeInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridPipeInfo.Size = new System.Drawing.Size(239, 207);
            this.propertyGridPipeInfo.TabIndex = 42;
            this.propertyGridPipeInfo.ToolbarVisible = false;
            this.propertyGridPipeInfo.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridPipeInfo_PropertyValueChanged);
            // 
            // buttonCreatePipeInfo
            // 
            this.buttonCreatePipeInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreatePipeInfo.Location = new System.Drawing.Point(6, 16);
            this.buttonCreatePipeInfo.Name = "buttonCreatePipeInfo";
            this.buttonCreatePipeInfo.Size = new System.Drawing.Size(65, 22);
            this.buttonCreatePipeInfo.TabIndex = 42;
            this.buttonCreatePipeInfo.Text = "Create";
            this.buttonCreatePipeInfo.UseVisualStyleBackColor = true;
            this.buttonCreatePipeInfo.Click += new System.EventHandler(this.buttonCreatePipeInfo_Click);
            // 
            // groupBoxLevelOfDetail
            // 
            this.groupBoxLevelOfDetail.Controls.Add(this.propertyGridLevelOfDetail);
            this.groupBoxLevelOfDetail.Controls.Add(this.buttonCreateLevelOfDetail);
            this.groupBoxLevelOfDetail.Location = new System.Drawing.Point(269, 12);
            this.groupBoxLevelOfDetail.Name = "groupBoxLevelOfDetail";
            this.groupBoxLevelOfDetail.Size = new System.Drawing.Size(251, 258);
            this.groupBoxLevelOfDetail.TabIndex = 43;
            this.groupBoxLevelOfDetail.TabStop = false;
            this.groupBoxLevelOfDetail.Text = "Level of Detail";
            // 
            // propertyGridLevelOfDetail
            // 
            this.propertyGridLevelOfDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridLevelOfDetail.Enabled = false;
            this.propertyGridLevelOfDetail.HelpVisible = false;
            this.propertyGridLevelOfDetail.Location = new System.Drawing.Point(6, 45);
            this.propertyGridLevelOfDetail.Name = "propertyGridLevelOfDetail";
            this.propertyGridLevelOfDetail.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridLevelOfDetail.Size = new System.Drawing.Size(239, 207);
            this.propertyGridLevelOfDetail.TabIndex = 42;
            this.propertyGridLevelOfDetail.ToolbarVisible = false;
            this.propertyGridLevelOfDetail.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridLevelOfDetail_PropertyValueChanged);
            // 
            // buttonCreateLevelOfDetail
            // 
            this.buttonCreateLevelOfDetail.Location = new System.Drawing.Point(6, 16);
            this.buttonCreateLevelOfDetail.Name = "buttonCreateLevelOfDetail";
            this.buttonCreateLevelOfDetail.Size = new System.Drawing.Size(65, 22);
            this.buttonCreateLevelOfDetail.TabIndex = 42;
            this.buttonCreateLevelOfDetail.Text = "Create";
            this.buttonCreateLevelOfDetail.UseVisualStyleBackColor = true;
            this.buttonCreateLevelOfDetail.Click += new System.EventHandler(this.buttonCreateLevelOfDetail_Click);
            // 
            // groupBoxShadow
            // 
            this.groupBoxShadow.Controls.Add(this.propertyGridShadow);
            this.groupBoxShadow.Controls.Add(this.buttonCreateShadow);
            this.groupBoxShadow.Location = new System.Drawing.Point(269, 276);
            this.groupBoxShadow.Name = "groupBoxShadow";
            this.groupBoxShadow.Size = new System.Drawing.Size(251, 94);
            this.groupBoxShadow.TabIndex = 44;
            this.groupBoxShadow.TabStop = false;
            this.groupBoxShadow.Text = "Shadow Model";
            // 
            // propertyGridShadow
            // 
            this.propertyGridShadow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridShadow.Enabled = false;
            this.propertyGridShadow.HelpVisible = false;
            this.propertyGridShadow.Location = new System.Drawing.Point(6, 41);
            this.propertyGridShadow.Name = "propertyGridShadow";
            this.propertyGridShadow.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridShadow.Size = new System.Drawing.Size(239, 47);
            this.propertyGridShadow.TabIndex = 42;
            this.propertyGridShadow.ToolbarVisible = false;
            this.propertyGridShadow.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridShadow_PropertyValueChanged);
            // 
            // buttonCreateShadow
            // 
            this.buttonCreateShadow.Location = new System.Drawing.Point(6, 16);
            this.buttonCreateShadow.Name = "buttonCreateShadow";
            this.buttonCreateShadow.Size = new System.Drawing.Size(65, 22);
            this.buttonCreateShadow.TabIndex = 42;
            this.buttonCreateShadow.Text = "Create";
            this.buttonCreateShadow.UseVisualStyleBackColor = true;
            this.buttonCreateShadow.Click += new System.EventHandler(this.buttonCreateShadow_Click);
            // 
            // groupBoxCollisionModel
            // 
            this.groupBoxCollisionModel.Controls.Add(this.propertyGridCollision);
            this.groupBoxCollisionModel.Controls.Add(this.buttonCreateCollision);
            this.groupBoxCollisionModel.Location = new System.Drawing.Point(12, 276);
            this.groupBoxCollisionModel.Name = "groupBoxCollisionModel";
            this.groupBoxCollisionModel.Size = new System.Drawing.Size(251, 94);
            this.groupBoxCollisionModel.TabIndex = 45;
            this.groupBoxCollisionModel.TabStop = false;
            this.groupBoxCollisionModel.Text = "Collision Model";
            // 
            // propertyGridCollision
            // 
            this.propertyGridCollision.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridCollision.Enabled = false;
            this.propertyGridCollision.HelpVisible = false;
            this.propertyGridCollision.Location = new System.Drawing.Point(6, 41);
            this.propertyGridCollision.Name = "propertyGridCollision";
            this.propertyGridCollision.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridCollision.Size = new System.Drawing.Size(239, 47);
            this.propertyGridCollision.TabIndex = 42;
            this.propertyGridCollision.ToolbarVisible = false;
            this.propertyGridCollision.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridCollision_PropertyValueChanged);
            // 
            // buttonCreateCollision
            // 
            this.buttonCreateCollision.Location = new System.Drawing.Point(6, 16);
            this.buttonCreateCollision.Name = "buttonCreateCollision";
            this.buttonCreateCollision.Size = new System.Drawing.Size(65, 22);
            this.buttonCreateCollision.TabIndex = 42;
            this.buttonCreateCollision.Text = "Create";
            this.buttonCreateCollision.UseVisualStyleBackColor = true;
            this.buttonCreateCollision.Click += new System.EventHandler(this.buttonCreateCollision_Click);
            // 
            // checkBoxUseTemplates
            // 
            this.checkBoxUseTemplates.AutoSize = true;
            this.checkBoxUseTemplates.Location = new System.Drawing.Point(18, 586);
            this.checkBoxUseTemplates.Name = "checkBoxUseTemplates";
            this.checkBoxUseTemplates.Size = new System.Drawing.Size(213, 17);
            this.checkBoxUseTemplates.TabIndex = 46;
            this.checkBoxUseTemplates.Text = "Use this model when placing a template";
            this.checkBoxUseTemplates.UseVisualStyleBackColor = true;
            // 
            // InternalModelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 612);
            this.Controls.Add(this.checkBoxUseTemplates);
            this.Controls.Add(this.groupBoxCollisionModel);
            this.Controls.Add(this.groupBoxShadow);
            this.Controls.Add(this.groupBoxLevelOfDetail);
            this.Controls.Add(this.groupBoxPipeInfo);
            this.Controls.Add(this.groupBoxAtomics);
            this.Controls.Add(this.groupBoxTextures);
            this.Controls.Add(this.groupBoxExport);
            this.Controls.Add(this.groupBoxImport);
            this.Controls.Add(this.buttonApplyScale);
            this.Controls.Add(this.buttonApplyRotation);
            this.Controls.Add(this.buttonApplyVertexColors);
            this.Controls.Add(this.buttonFindCallers);
            this.Controls.Add(this.buttonHelp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "InternalModelEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalAssetEditor_FormClosing);
            this.groupBoxImport.ResumeLayout(false);
            this.groupBoxImport.PerformLayout();
            this.groupBoxExport.ResumeLayout(false);
            this.groupBoxExport.PerformLayout();
            this.groupBoxTextures.ResumeLayout(false);
            this.groupBoxAtomics.ResumeLayout(false);
            this.tableLayoutPanelAtomics.ResumeLayout(false);
            this.groupBoxPipeInfo.ResumeLayout(false);
            this.groupBoxPipeInfo.PerformLayout();
            this.groupBoxLevelOfDetail.ResumeLayout(false);
            this.groupBoxShadow.ResumeLayout(false);
            this.groupBoxCollisionModel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonApplyRotation;
        private System.Windows.Forms.Button buttonApplyVertexColors;
        private System.Windows.Forms.Button buttonApplyScale;
        private System.Windows.Forms.GroupBox groupBoxImport;
        private System.Windows.Forms.CheckBox checkBoxIgnoreMeshColors;
        private System.Windows.Forms.CheckBox checkBoxFilpUvs;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.GroupBox groupBoxExport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.CheckBox checkBoxExportTextures;
        private System.Windows.Forms.Button buttonMaterialEditor;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTextures;
        private System.Windows.Forms.GroupBox groupBoxTextures;
        private System.Windows.Forms.GroupBox groupBoxAtomics;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelAtomics;
        private System.Windows.Forms.Button buttonEditAtomics;
        private System.Windows.Forms.GroupBox groupBoxPipeInfo;
        private System.Windows.Forms.Button buttonCreatePipeInfo;
        private System.Windows.Forms.PropertyGrid propertyGridPipeInfo;
        private System.Windows.Forms.GroupBox groupBoxLevelOfDetail;
        private System.Windows.Forms.PropertyGrid propertyGridLevelOfDetail;
        private System.Windows.Forms.Button buttonCreateLevelOfDetail;
        private System.Windows.Forms.ComboBox comboBoxPiptPreset;
        private System.Windows.Forms.Label labelPreset;
        private System.Windows.Forms.GroupBox groupBoxShadow;
        private System.Windows.Forms.PropertyGrid propertyGridShadow;
        private System.Windows.Forms.Button buttonCreateShadow;
        private System.Windows.Forms.GroupBox groupBoxCollisionModel;
        private System.Windows.Forms.PropertyGrid propertyGridCollision;
        private System.Windows.Forms.Button buttonCreateCollision;
        private System.Windows.Forms.CheckBox checkBoxUseTemplates;
    }
}