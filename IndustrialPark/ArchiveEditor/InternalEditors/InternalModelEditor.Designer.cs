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
            buttonHelp = new System.Windows.Forms.Button();
            buttonFindCallers = new System.Windows.Forms.Button();
            buttonApplyRotation = new System.Windows.Forms.Button();
            buttonApplyVertexColors = new System.Windows.Forms.Button();
            buttonApplyScale = new System.Windows.Forms.Button();
            groupBoxImport = new System.Windows.Forms.GroupBox();
            buttonImport = new System.Windows.Forms.Button();
            groupBoxExport = new System.Windows.Forms.GroupBox();
            buttonExport = new System.Windows.Forms.Button();
            checkBoxExportTextures = new System.Windows.Forms.CheckBox();
            buttonMaterialEditor = new System.Windows.Forms.Button();
            flowLayoutPanelTextures = new System.Windows.Forms.FlowLayoutPanel();
            groupBoxTextures = new System.Windows.Forms.GroupBox();
            groupBoxAtomics = new System.Windows.Forms.GroupBox();
            tableLayoutPanelAtomics = new System.Windows.Forms.TableLayoutPanel();
            buttonEditAtomics = new System.Windows.Forms.Button();
            groupBoxPipeInfo = new System.Windows.Forms.GroupBox();
            labelPipeInfos = new System.Windows.Forms.Label();
            buttonArrowDown = new System.Windows.Forms.Button();
            buttonArrowUp = new System.Windows.Forms.Button();
            buttonDeletePipeInfo = new System.Windows.Forms.Button();
            propertyGridPipeInfo = new System.Windows.Forms.PropertyGrid();
            buttonCreatePipeInfo = new System.Windows.Forms.Button();
            groupBoxLevelOfDetail = new System.Windows.Forms.GroupBox();
            propertyGridLevelOfDetail = new System.Windows.Forms.PropertyGrid();
            buttonCreateLevelOfDetail = new System.Windows.Forms.Button();
            groupBoxShadow = new System.Windows.Forms.GroupBox();
            propertyGridShadow = new System.Windows.Forms.PropertyGrid();
            buttonCreateShadow = new System.Windows.Forms.Button();
            groupBoxCollisionModel = new System.Windows.Forms.GroupBox();
            buttonImportColl = new System.Windows.Forms.Button();
            propertyGridCollision = new System.Windows.Forms.PropertyGrid();
            buttonCreateCollision = new System.Windows.Forms.Button();
            checkBoxUseTemplates = new System.Windows.Forms.CheckBox();
            buildCollTreeButton = new System.Windows.Forms.Button();
            removeCollPlgButton = new System.Windows.Forms.Button();
            groupBoxImport.SuspendLayout();
            groupBoxExport.SuspendLayout();
            groupBoxTextures.SuspendLayout();
            groupBoxAtomics.SuspendLayout();
            tableLayoutPanelAtomics.SuspendLayout();
            groupBoxPipeInfo.SuspendLayout();
            groupBoxLevelOfDetail.SuspendLayout();
            groupBoxShadow.SuspendLayout();
            groupBoxCollisionModel.SuspendLayout();
            SuspendLayout();
            // 
            // buttonHelp
            // 
            buttonHelp.Location = new System.Drawing.Point(396, 554);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(120, 22);
            buttonHelp.TabIndex = 27;
            buttonHelp.Text = "Open Wiki Page";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // buttonFindCallers
            // 
            buttonFindCallers.Location = new System.Drawing.Point(396, 582);
            buttonFindCallers.Name = "buttonFindCallers";
            buttonFindCallers.Size = new System.Drawing.Size(120, 22);
            buttonFindCallers.TabIndex = 28;
            buttonFindCallers.Text = "Find Who Targets Me";
            buttonFindCallers.UseVisualStyleBackColor = true;
            buttonFindCallers.Click += buttonFindCallers_Click;
            // 
            // buttonApplyRotation
            // 
            buttonApplyRotation.Location = new System.Drawing.Point(18, 554);
            buttonApplyRotation.Name = "buttonApplyRotation";
            buttonApplyRotation.Size = new System.Drawing.Size(120, 22);
            buttonApplyRotation.TabIndex = 30;
            buttonApplyRotation.Text = "Apply Rotation";
            buttonApplyRotation.UseVisualStyleBackColor = true;
            buttonApplyRotation.Click += buttonApplyRotation_Click;
            // 
            // buttonApplyVertexColors
            // 
            buttonApplyVertexColors.Location = new System.Drawing.Point(143, 554);
            buttonApplyVertexColors.Name = "buttonApplyVertexColors";
            buttonApplyVertexColors.Size = new System.Drawing.Size(120, 22);
            buttonApplyVertexColors.TabIndex = 29;
            buttonApplyVertexColors.Text = "Apply Vertex Colors";
            buttonApplyVertexColors.UseVisualStyleBackColor = true;
            buttonApplyVertexColors.Click += buttonApplyVertexColors_Click;
            // 
            // buttonApplyScale
            // 
            buttonApplyScale.Location = new System.Drawing.Point(18, 582);
            buttonApplyScale.Name = "buttonApplyScale";
            buttonApplyScale.Size = new System.Drawing.Size(120, 22);
            buttonApplyScale.TabIndex = 31;
            buttonApplyScale.Text = "Apply Scale";
            buttonApplyScale.UseVisualStyleBackColor = true;
            buttonApplyScale.Click += buttonApplyScale_Click;
            // 
            // groupBoxImport
            // 
            groupBoxImport.Controls.Add(buttonImport);
            groupBoxImport.Location = new System.Drawing.Point(396, 376);
            groupBoxImport.Name = "groupBoxImport";
            groupBoxImport.Size = new System.Drawing.Size(124, 54);
            groupBoxImport.TabIndex = 32;
            groupBoxImport.TabStop = false;
            groupBoxImport.Text = "Import Model";
            // 
            // buttonImport
            // 
            buttonImport.Location = new System.Drawing.Point(6, 20);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new System.Drawing.Size(112, 22);
            buttonImport.TabIndex = 33;
            buttonImport.Text = "Import";
            buttonImport.UseVisualStyleBackColor = true;
            buttonImport.Click += buttonImport_Click;
            // 
            // groupBoxExport
            // 
            groupBoxExport.Controls.Add(buttonExport);
            groupBoxExport.Controls.Add(checkBoxExportTextures);
            groupBoxExport.Location = new System.Drawing.Point(396, 436);
            groupBoxExport.Name = "groupBoxExport";
            groupBoxExport.Size = new System.Drawing.Size(124, 73);
            groupBoxExport.TabIndex = 34;
            groupBoxExport.TabStop = false;
            groupBoxExport.Text = "Export Model";
            // 
            // buttonExport
            // 
            buttonExport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonExport.Location = new System.Drawing.Point(6, 45);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new System.Drawing.Size(112, 22);
            buttonExport.TabIndex = 33;
            buttonExport.Text = "Export";
            buttonExport.UseVisualStyleBackColor = true;
            buttonExport.Click += buttonExport_Click;
            // 
            // checkBoxExportTextures
            // 
            checkBoxExportTextures.AutoSize = true;
            checkBoxExportTextures.Location = new System.Drawing.Point(6, 19);
            checkBoxExportTextures.Name = "checkBoxExportTextures";
            checkBoxExportTextures.Size = new System.Drawing.Size(100, 17);
            checkBoxExportTextures.TabIndex = 0;
            checkBoxExportTextures.Text = "Export Textures";
            checkBoxExportTextures.UseVisualStyleBackColor = true;
            // 
            // buttonMaterialEditor
            // 
            buttonMaterialEditor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonMaterialEditor.Location = new System.Drawing.Point(6, 144);
            buttonMaterialEditor.Name = "buttonMaterialEditor";
            buttonMaterialEditor.Size = new System.Drawing.Size(171, 22);
            buttonMaterialEditor.TabIndex = 35;
            buttonMaterialEditor.Text = "Open Material Editor";
            buttonMaterialEditor.UseVisualStyleBackColor = true;
            buttonMaterialEditor.Click += buttonMaterialEditor_Click;
            // 
            // flowLayoutPanelTextures
            // 
            flowLayoutPanelTextures.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            flowLayoutPanelTextures.AutoScroll = true;
            flowLayoutPanelTextures.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanelTextures.Location = new System.Drawing.Point(6, 19);
            flowLayoutPanelTextures.Name = "flowLayoutPanelTextures";
            flowLayoutPanelTextures.Size = new System.Drawing.Size(171, 119);
            flowLayoutPanelTextures.TabIndex = 36;
            flowLayoutPanelTextures.WrapContents = false;
            // 
            // groupBoxTextures
            // 
            groupBoxTextures.Controls.Add(flowLayoutPanelTextures);
            groupBoxTextures.Controls.Add(buttonMaterialEditor);
            groupBoxTextures.Location = new System.Drawing.Point(12, 376);
            groupBoxTextures.Name = "groupBoxTextures";
            groupBoxTextures.Size = new System.Drawing.Size(183, 172);
            groupBoxTextures.TabIndex = 34;
            groupBoxTextures.TabStop = false;
            groupBoxTextures.Text = "Textures";
            // 
            // groupBoxAtomics
            // 
            groupBoxAtomics.Controls.Add(tableLayoutPanelAtomics);
            groupBoxAtomics.Location = new System.Drawing.Point(201, 376);
            groupBoxAtomics.Name = "groupBoxAtomics";
            groupBoxAtomics.Size = new System.Drawing.Size(189, 172);
            groupBoxAtomics.TabIndex = 37;
            groupBoxAtomics.TabStop = false;
            groupBoxAtomics.Text = "Atomics";
            // 
            // tableLayoutPanelAtomics
            // 
            tableLayoutPanelAtomics.AutoScroll = true;
            tableLayoutPanelAtomics.ColumnCount = 2;
            tableLayoutPanelAtomics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanelAtomics.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanelAtomics.Controls.Add(buttonEditAtomics, 1, 0);
            tableLayoutPanelAtomics.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanelAtomics.Location = new System.Drawing.Point(3, 16);
            tableLayoutPanelAtomics.Name = "tableLayoutPanelAtomics";
            tableLayoutPanelAtomics.RowCount = 1;
            tableLayoutPanelAtomics.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanelAtomics.Size = new System.Drawing.Size(183, 153);
            tableLayoutPanelAtomics.TabIndex = 42;
            // 
            // buttonEditAtomics
            // 
            buttonEditAtomics.Dock = System.Windows.Forms.DockStyle.Top;
            buttonEditAtomics.Location = new System.Drawing.Point(3, 3);
            buttonEditAtomics.Name = "buttonEditAtomics";
            buttonEditAtomics.Size = new System.Drawing.Size(177, 23);
            buttonEditAtomics.TabIndex = 38;
            buttonEditAtomics.Text = "Edit";
            buttonEditAtomics.UseVisualStyleBackColor = true;
            buttonEditAtomics.Click += buttonEditAtomics_Click;
            // 
            // groupBoxPipeInfo
            // 
            groupBoxPipeInfo.Controls.Add(labelPipeInfos);
            groupBoxPipeInfo.Controls.Add(buttonArrowDown);
            groupBoxPipeInfo.Controls.Add(buttonArrowUp);
            groupBoxPipeInfo.Controls.Add(buttonDeletePipeInfo);
            groupBoxPipeInfo.Controls.Add(propertyGridPipeInfo);
            groupBoxPipeInfo.Controls.Add(buttonCreatePipeInfo);
            groupBoxPipeInfo.Location = new System.Drawing.Point(12, 12);
            groupBoxPipeInfo.Name = "groupBoxPipeInfo";
            groupBoxPipeInfo.Size = new System.Drawing.Size(251, 258);
            groupBoxPipeInfo.TabIndex = 41;
            groupBoxPipeInfo.TabStop = false;
            groupBoxPipeInfo.Text = "Pipe Info";
            // 
            // labelPipeInfos
            // 
            labelPipeInfos.AutoSize = true;
            labelPipeInfos.Location = new System.Drawing.Point(6, 21);
            labelPipeInfos.Name = "labelPipeInfos";
            labelPipeInfos.Size = new System.Drawing.Size(73, 13);
            labelPipeInfos.TabIndex = 46;
            labelPipeInfos.Text = "labelPipeInfos";
            // 
            // buttonArrowDown
            // 
            buttonArrowDown.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonArrowDown.Enabled = false;
            buttonArrowDown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            buttonArrowDown.Location = new System.Drawing.Point(161, 16);
            buttonArrowDown.Name = "buttonArrowDown";
            buttonArrowDown.Size = new System.Drawing.Size(22, 22);
            buttonArrowDown.TabIndex = 45;
            buttonArrowDown.Text = "▼";
            buttonArrowDown.UseVisualStyleBackColor = true;
            buttonArrowDown.Click += buttonArrowDown_Click;
            // 
            // buttonArrowUp
            // 
            buttonArrowUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonArrowUp.Enabled = false;
            buttonArrowUp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            buttonArrowUp.Location = new System.Drawing.Point(133, 16);
            buttonArrowUp.Name = "buttonArrowUp";
            buttonArrowUp.Size = new System.Drawing.Size(22, 22);
            buttonArrowUp.TabIndex = 44;
            buttonArrowUp.Text = "▲";
            buttonArrowUp.UseVisualStyleBackColor = true;
            buttonArrowUp.Click += buttonArrowUp_Click;
            // 
            // buttonDeletePipeInfo
            // 
            buttonDeletePipeInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonDeletePipeInfo.Location = new System.Drawing.Point(195, 16);
            buttonDeletePipeInfo.Name = "buttonDeletePipeInfo";
            buttonDeletePipeInfo.Size = new System.Drawing.Size(22, 22);
            buttonDeletePipeInfo.TabIndex = 43;
            buttonDeletePipeInfo.Text = "-";
            buttonDeletePipeInfo.UseVisualStyleBackColor = true;
            buttonDeletePipeInfo.Click += buttonDeletePipeInfo_Click;
            // 
            // propertyGridPipeInfo
            // 
            propertyGridPipeInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGridPipeInfo.Enabled = false;
            propertyGridPipeInfo.HelpVisible = false;
            propertyGridPipeInfo.Location = new System.Drawing.Point(6, 44);
            propertyGridPipeInfo.Name = "propertyGridPipeInfo";
            propertyGridPipeInfo.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridPipeInfo.Size = new System.Drawing.Size(239, 208);
            propertyGridPipeInfo.TabIndex = 42;
            propertyGridPipeInfo.ToolbarVisible = false;
            propertyGridPipeInfo.PropertyValueChanged += propertyGridPipeInfo_PropertyValueChanged;
            // 
            // buttonCreatePipeInfo
            // 
            buttonCreatePipeInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonCreatePipeInfo.Location = new System.Drawing.Point(223, 16);
            buttonCreatePipeInfo.Name = "buttonCreatePipeInfo";
            buttonCreatePipeInfo.Size = new System.Drawing.Size(22, 22);
            buttonCreatePipeInfo.TabIndex = 42;
            buttonCreatePipeInfo.Text = "+";
            buttonCreatePipeInfo.UseVisualStyleBackColor = true;
            buttonCreatePipeInfo.Click += buttonCreatePipeInfo_Click;
            // 
            // groupBoxLevelOfDetail
            // 
            groupBoxLevelOfDetail.Controls.Add(propertyGridLevelOfDetail);
            groupBoxLevelOfDetail.Controls.Add(buttonCreateLevelOfDetail);
            groupBoxLevelOfDetail.Location = new System.Drawing.Point(269, 12);
            groupBoxLevelOfDetail.Name = "groupBoxLevelOfDetail";
            groupBoxLevelOfDetail.Size = new System.Drawing.Size(251, 258);
            groupBoxLevelOfDetail.TabIndex = 43;
            groupBoxLevelOfDetail.TabStop = false;
            groupBoxLevelOfDetail.Text = "Level of Detail";
            // 
            // propertyGridLevelOfDetail
            // 
            propertyGridLevelOfDetail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGridLevelOfDetail.Enabled = false;
            propertyGridLevelOfDetail.HelpVisible = false;
            propertyGridLevelOfDetail.Location = new System.Drawing.Point(6, 45);
            propertyGridLevelOfDetail.Name = "propertyGridLevelOfDetail";
            propertyGridLevelOfDetail.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridLevelOfDetail.Size = new System.Drawing.Size(239, 207);
            propertyGridLevelOfDetail.TabIndex = 42;
            propertyGridLevelOfDetail.ToolbarVisible = false;
            propertyGridLevelOfDetail.PropertyValueChanged += propertyGridLevelOfDetail_PropertyValueChanged;
            // 
            // buttonCreateLevelOfDetail
            // 
            buttonCreateLevelOfDetail.Location = new System.Drawing.Point(6, 16);
            buttonCreateLevelOfDetail.Name = "buttonCreateLevelOfDetail";
            buttonCreateLevelOfDetail.Size = new System.Drawing.Size(65, 22);
            buttonCreateLevelOfDetail.TabIndex = 42;
            buttonCreateLevelOfDetail.Text = "Create";
            buttonCreateLevelOfDetail.UseVisualStyleBackColor = true;
            buttonCreateLevelOfDetail.Click += buttonCreateLevelOfDetail_Click;
            // 
            // groupBoxShadow
            // 
            groupBoxShadow.Controls.Add(propertyGridShadow);
            groupBoxShadow.Controls.Add(buttonCreateShadow);
            groupBoxShadow.Location = new System.Drawing.Point(269, 276);
            groupBoxShadow.Name = "groupBoxShadow";
            groupBoxShadow.Size = new System.Drawing.Size(251, 94);
            groupBoxShadow.TabIndex = 44;
            groupBoxShadow.TabStop = false;
            groupBoxShadow.Text = "Shadow Model";
            // 
            // propertyGridShadow
            // 
            propertyGridShadow.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGridShadow.Enabled = false;
            propertyGridShadow.HelpVisible = false;
            propertyGridShadow.Location = new System.Drawing.Point(6, 41);
            propertyGridShadow.Name = "propertyGridShadow";
            propertyGridShadow.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            propertyGridShadow.Size = new System.Drawing.Size(239, 47);
            propertyGridShadow.TabIndex = 42;
            propertyGridShadow.ToolbarVisible = false;
            propertyGridShadow.PropertyValueChanged += propertyGridShadow_PropertyValueChanged;
            // 
            // buttonCreateShadow
            // 
            buttonCreateShadow.Location = new System.Drawing.Point(6, 16);
            buttonCreateShadow.Name = "buttonCreateShadow";
            buttonCreateShadow.Size = new System.Drawing.Size(65, 22);
            buttonCreateShadow.TabIndex = 42;
            buttonCreateShadow.Text = "Create";
            buttonCreateShadow.UseVisualStyleBackColor = true;
            buttonCreateShadow.Click += buttonCreateShadow_Click;
            // 
            // groupBoxCollisionModel
            // 
            groupBoxCollisionModel.Controls.Add(buttonImportColl);
            groupBoxCollisionModel.Controls.Add(propertyGridCollision);
            groupBoxCollisionModel.Controls.Add(buttonCreateCollision);
            groupBoxCollisionModel.Location = new System.Drawing.Point(12, 276);
            groupBoxCollisionModel.Name = "groupBoxCollisionModel";
            groupBoxCollisionModel.Size = new System.Drawing.Size(251, 94);
            groupBoxCollisionModel.TabIndex = 45;
            groupBoxCollisionModel.TabStop = false;
            groupBoxCollisionModel.Text = "Collision Model";
            // 
            // buttonImportColl
            // 
            buttonImportColl.Enabled = false;
            buttonImportColl.Location = new System.Drawing.Point(77, 16);
            buttonImportColl.Name = "buttonImportColl";
            buttonImportColl.Size = new System.Drawing.Size(75, 22);
            buttonImportColl.TabIndex = 43;
            buttonImportColl.Text = "Import";
            buttonImportColl.UseVisualStyleBackColor = true;
            buttonImportColl.Click += buttonImportColl_Click;
            // 
            // propertyGridCollision
            // 
            propertyGridCollision.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGridCollision.Enabled = false;
            propertyGridCollision.HelpVisible = false;
            propertyGridCollision.Location = new System.Drawing.Point(6, 41);
            propertyGridCollision.Name = "propertyGridCollision";
            propertyGridCollision.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            propertyGridCollision.Size = new System.Drawing.Size(239, 47);
            propertyGridCollision.TabIndex = 42;
            propertyGridCollision.ToolbarVisible = false;
            propertyGridCollision.PropertyValueChanged += propertyGridCollision_PropertyValueChanged;
            // 
            // buttonCreateCollision
            // 
            buttonCreateCollision.Location = new System.Drawing.Point(6, 16);
            buttonCreateCollision.Name = "buttonCreateCollision";
            buttonCreateCollision.Size = new System.Drawing.Size(65, 22);
            buttonCreateCollision.TabIndex = 42;
            buttonCreateCollision.Text = "Create";
            buttonCreateCollision.UseVisualStyleBackColor = true;
            buttonCreateCollision.Click += buttonCreateCollision_Click;
            // 
            // checkBoxUseTemplates
            // 
            checkBoxUseTemplates.AutoSize = true;
            checkBoxUseTemplates.Location = new System.Drawing.Point(12, 610);
            checkBoxUseTemplates.Name = "checkBoxUseTemplates";
            checkBoxUseTemplates.Size = new System.Drawing.Size(213, 17);
            checkBoxUseTemplates.TabIndex = 46;
            checkBoxUseTemplates.Text = "Use this model when placing a template";
            checkBoxUseTemplates.UseVisualStyleBackColor = true;
            checkBoxUseTemplates.Click += checkBoxUseTemplates_Click;
            // 
            // buildCollTreeButton
            // 
            buildCollTreeButton.Location = new System.Drawing.Point(269, 554);
            buildCollTreeButton.Name = "buildCollTreeButton";
            buildCollTreeButton.Size = new System.Drawing.Size(120, 22);
            buildCollTreeButton.TabIndex = 47;
            buildCollTreeButton.Text = "Build Collision PLG";
            buildCollTreeButton.UseVisualStyleBackColor = true;
            buildCollTreeButton.Click += buildCollTreeButton_Click;
            // 
            // removeCollPlgButton
            // 
            removeCollPlgButton.Enabled = false;
            removeCollPlgButton.Location = new System.Drawing.Point(269, 582);
            removeCollPlgButton.Name = "removeCollPlgButton";
            removeCollPlgButton.Size = new System.Drawing.Size(120, 22);
            removeCollPlgButton.TabIndex = 48;
            removeCollPlgButton.Text = "Remove Collision PLG";
            removeCollPlgButton.UseVisualStyleBackColor = true;
            removeCollPlgButton.Click += removeCollPlgButton_Click;
            // 
            // InternalModelEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(527, 633);
            Controls.Add(removeCollPlgButton);
            Controls.Add(buildCollTreeButton);
            Controls.Add(checkBoxUseTemplates);
            Controls.Add(groupBoxCollisionModel);
            Controls.Add(groupBoxShadow);
            Controls.Add(groupBoxLevelOfDetail);
            Controls.Add(groupBoxPipeInfo);
            Controls.Add(groupBoxAtomics);
            Controls.Add(groupBoxTextures);
            Controls.Add(groupBoxExport);
            Controls.Add(groupBoxImport);
            Controls.Add(buttonApplyScale);
            Controls.Add(buttonApplyRotation);
            Controls.Add(buttonApplyVertexColors);
            Controls.Add(buttonFindCallers);
            Controls.Add(buttonHelp);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "InternalModelEditor";
            ShowIcon = false;
            Text = "Asset Data Editor";
            FormClosing += InternalAssetEditor_FormClosing;
            groupBoxImport.ResumeLayout(false);
            groupBoxExport.ResumeLayout(false);
            groupBoxExport.PerformLayout();
            groupBoxTextures.ResumeLayout(false);
            groupBoxAtomics.ResumeLayout(false);
            tableLayoutPanelAtomics.ResumeLayout(false);
            groupBoxPipeInfo.ResumeLayout(false);
            groupBoxPipeInfo.PerformLayout();
            groupBoxLevelOfDetail.ResumeLayout(false);
            groupBoxShadow.ResumeLayout(false);
            groupBoxCollisionModel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonApplyRotation;
        private System.Windows.Forms.Button buttonApplyVertexColors;
        private System.Windows.Forms.Button buttonApplyScale;
        private System.Windows.Forms.GroupBox groupBoxImport;
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
        private System.Windows.Forms.GroupBox groupBoxShadow;
        private System.Windows.Forms.PropertyGrid propertyGridShadow;
        private System.Windows.Forms.Button buttonCreateShadow;
        private System.Windows.Forms.GroupBox groupBoxCollisionModel;
        private System.Windows.Forms.PropertyGrid propertyGridCollision;
        private System.Windows.Forms.Button buttonCreateCollision;
        private System.Windows.Forms.CheckBox checkBoxUseTemplates;
        private System.Windows.Forms.Button buttonDeletePipeInfo;
        private System.Windows.Forms.Button buttonArrowDown;
        private System.Windows.Forms.Button buttonArrowUp;
        private System.Windows.Forms.Label labelPipeInfos;
        private System.Windows.Forms.Button buildCollTreeButton;
        private System.Windows.Forms.Button removeCollPlgButton;
        private System.Windows.Forms.Button buttonImportColl;
    }
}