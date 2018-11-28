namespace IndustrialPark
{
    partial class ArchiveEditor
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hipHopToolExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importHIPArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMultipleAssetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTXDArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTXDArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelCurrentFilename = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelSelectionCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonArrowDown = new System.Windows.Forms.Button();
            this.buttonArrowUp = new System.Windows.Forms.Button();
            this.comboBoxLayerTypes = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonRemoveLayer = new System.Windows.Forms.Button();
            this.buttonAddLayer = new System.Windows.Forms.Button();
            this.comboBoxLayers = new System.Windows.Forms.ComboBox();
            this.listBoxAssets = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelTemplateFocus = new System.Windows.Forms.Label();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.textBoxFindAsset = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonInternalEdit = new System.Windows.Forms.Button();
            this.buttonView = new System.Windows.Forms.Button();
            this.buttonExportRaw = new System.Windows.Forms.Button();
            this.buttonEditAsset = new System.Windows.Forms.Button();
            this.buttonDuplicate = new System.Windows.Forms.Button();
            this.buttonRemoveAsset = new System.Windows.Forms.Button();
            this.buttonAddAsset = new System.Windows.Forms.Button();
            this.comboBoxAssetTypes = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip_ListBoxAssets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Add = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_AddMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem_View = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Export = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_EditHeader = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_EditData = new System.Windows.Forms.ToolStripMenuItem();
            this.hideButtonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip_ListBoxAssets.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(624, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.hideButtonsToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hipHopToolExportToolStripMenuItem,
            this.importHIPArchiveToolStripMenuItem,
            this.importMultipleAssetsToolStripMenuItem,
            this.exportTXDArchiveToolStripMenuItem,
            this.importTXDArchiveToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // hipHopToolExportToolStripMenuItem
            // 
            this.hipHopToolExportToolStripMenuItem.Enabled = false;
            this.hipHopToolExportToolStripMenuItem.Name = "hipHopToolExportToolStripMenuItem";
            this.hipHopToolExportToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.hipHopToolExportToolStripMenuItem.Text = "Export Assets + INI";
            this.hipHopToolExportToolStripMenuItem.Click += new System.EventHandler(this.hipHopToolExportToolStripMenuItem_Click);
            // 
            // importHIPArchiveToolStripMenuItem
            // 
            this.importHIPArchiveToolStripMenuItem.Enabled = false;
            this.importHIPArchiveToolStripMenuItem.Name = "importHIPArchiveToolStripMenuItem";
            this.importHIPArchiveToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.importHIPArchiveToolStripMenuItem.Text = "Import HIP Archive";
            this.importHIPArchiveToolStripMenuItem.Click += new System.EventHandler(this.importHIPArchiveToolStripMenuItem_Click);
            // 
            // importMultipleAssetsToolStripMenuItem
            // 
            this.importMultipleAssetsToolStripMenuItem.Enabled = false;
            this.importMultipleAssetsToolStripMenuItem.Name = "importMultipleAssetsToolStripMenuItem";
            this.importMultipleAssetsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.importMultipleAssetsToolStripMenuItem.Text = "Import Multiple Assets";
            this.importMultipleAssetsToolStripMenuItem.Click += new System.EventHandler(this.importMultipleAssetsToolStripMenuItem_Click);
            // 
            // exportTXDArchiveToolStripMenuItem
            // 
            this.exportTXDArchiveToolStripMenuItem.Enabled = false;
            this.exportTXDArchiveToolStripMenuItem.Name = "exportTXDArchiveToolStripMenuItem";
            this.exportTXDArchiveToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.exportTXDArchiveToolStripMenuItem.Text = "Export TXD Archive";
            this.exportTXDArchiveToolStripMenuItem.Click += new System.EventHandler(this.exportTXDArchiveToolStripMenuItem_Click);
            // 
            // importTXDArchiveToolStripMenuItem
            // 
            this.importTXDArchiveToolStripMenuItem.Enabled = false;
            this.importTXDArchiveToolStripMenuItem.Name = "importTXDArchiveToolStripMenuItem";
            this.importTXDArchiveToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.importTXDArchiveToolStripMenuItem.Text = "Import TXD Archive";
            this.importTXDArchiveToolStripMenuItem.Click += new System.EventHandler(this.importTXDArchiveToolStripMenuItem1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelCurrentFilename,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabelSelectionCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 419);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(624, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelCurrentFilename
            // 
            this.toolStripStatusLabelCurrentFilename.Name = "toolStripStatusLabelCurrentFilename";
            this.toolStripStatusLabelCurrentFilename.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel1.Text = "|";
            // 
            // toolStripStatusLabelSelectionCount
            // 
            this.toolStripStatusLabelSelectionCount.Name = "toolStripStatusLabelSelectionCount";
            this.toolStripStatusLabelSelectionCount.Size = new System.Drawing.Size(0, 17);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonArrowDown);
            this.groupBox1.Controls.Add(this.buttonArrowUp);
            this.groupBox1.Controls.Add(this.comboBoxLayerTypes);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonRemoveLayer);
            this.groupBox1.Controls.Add(this.buttonAddLayer);
            this.groupBox1.Controls.Add(this.comboBoxLayers);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(600, 47);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Layer";
            // 
            // buttonArrowDown
            // 
            this.buttonArrowDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonArrowDown.Enabled = false;
            this.buttonArrowDown.Location = new System.Drawing.Point(568, 19);
            this.buttonArrowDown.Name = "buttonArrowDown";
            this.buttonArrowDown.Size = new System.Drawing.Size(22, 22);
            this.buttonArrowDown.TabIndex = 6;
            this.buttonArrowDown.Text = "↓";
            this.buttonArrowDown.UseVisualStyleBackColor = true;
            this.buttonArrowDown.Click += new System.EventHandler(this.buttonArrowDown_Click);
            // 
            // buttonArrowUp
            // 
            this.buttonArrowUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonArrowUp.Enabled = false;
            this.buttonArrowUp.Location = new System.Drawing.Point(540, 19);
            this.buttonArrowUp.Name = "buttonArrowUp";
            this.buttonArrowUp.Size = new System.Drawing.Size(22, 22);
            this.buttonArrowUp.TabIndex = 5;
            this.buttonArrowUp.Text = "↑";
            this.buttonArrowUp.UseVisualStyleBackColor = true;
            this.buttonArrowUp.Click += new System.EventHandler(this.buttonArrowUp_Click);
            // 
            // comboBoxLayerTypes
            // 
            this.comboBoxLayerTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLayerTypes.FormattingEnabled = true;
            this.comboBoxLayerTypes.Location = new System.Drawing.Point(320, 19);
            this.comboBoxLayerTypes.Name = "comboBoxLayerTypes";
            this.comboBoxLayerTypes.Size = new System.Drawing.Size(91, 21);
            this.comboBoxLayerTypes.TabIndex = 3;
            this.comboBoxLayerTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayerTypes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(280, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Type:";
            // 
            // buttonRemoveLayer
            // 
            this.buttonRemoveLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveLayer.Enabled = false;
            this.buttonRemoveLayer.Location = new System.Drawing.Point(471, 19);
            this.buttonRemoveLayer.Name = "buttonRemoveLayer";
            this.buttonRemoveLayer.Size = new System.Drawing.Size(63, 21);
            this.buttonRemoveLayer.TabIndex = 4;
            this.buttonRemoveLayer.Text = "Remove";
            this.buttonRemoveLayer.UseVisualStyleBackColor = true;
            this.buttonRemoveLayer.Click += new System.EventHandler(this.buttonRemoveLayer_Click);
            // 
            // buttonAddLayer
            // 
            this.buttonAddLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddLayer.Enabled = false;
            this.buttonAddLayer.Location = new System.Drawing.Point(417, 19);
            this.buttonAddLayer.Name = "buttonAddLayer";
            this.buttonAddLayer.Size = new System.Drawing.Size(48, 21);
            this.buttonAddLayer.TabIndex = 3;
            this.buttonAddLayer.Text = "Add";
            this.buttonAddLayer.UseVisualStyleBackColor = true;
            this.buttonAddLayer.Click += new System.EventHandler(this.buttonAddLayer_Click);
            // 
            // comboBoxLayers
            // 
            this.comboBoxLayers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLayers.FormattingEnabled = true;
            this.comboBoxLayers.Location = new System.Drawing.Point(6, 19);
            this.comboBoxLayers.Name = "comboBoxLayers";
            this.comboBoxLayers.Size = new System.Drawing.Size(268, 21);
            this.comboBoxLayers.TabIndex = 3;
            this.comboBoxLayers.SelectedIndexChanged += new System.EventHandler(this.comboBoxLayers_SelectedIndexChanged);
            // 
            // listBoxAssets
            // 
            this.listBoxAssets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAssets.FormattingEnabled = true;
            this.listBoxAssets.Location = new System.Drawing.Point(6, 40);
            this.listBoxAssets.Name = "listBoxAssets";
            this.listBoxAssets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxAssets.Size = new System.Drawing.Size(507, 290);
            this.listBoxAssets.TabIndex = 3;
            this.listBoxAssets.SelectedIndexChanged += new System.EventHandler(this.listBoxAssets_SelectedIndexChanged);
            this.listBoxAssets.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxAssets_KeyDown);
            this.listBoxAssets.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxAssets_MouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelTemplateFocus);
            this.groupBox2.Controls.Add(this.buttonCopy);
            this.groupBox2.Controls.Add(this.buttonPaste);
            this.groupBox2.Controls.Add(this.textBoxFindAsset);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.buttonInternalEdit);
            this.groupBox2.Controls.Add(this.buttonView);
            this.groupBox2.Controls.Add(this.buttonExportRaw);
            this.groupBox2.Controls.Add(this.buttonEditAsset);
            this.groupBox2.Controls.Add(this.buttonDuplicate);
            this.groupBox2.Controls.Add(this.buttonRemoveAsset);
            this.groupBox2.Controls.Add(this.buttonAddAsset);
            this.groupBox2.Controls.Add(this.comboBoxAssetTypes);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.listBoxAssets);
            this.groupBox2.Location = new System.Drawing.Point(12, 80);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(600, 336);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Assets";
            // 
            // labelTemplateFocus
            // 
            this.labelTemplateFocus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTemplateFocus.AutoSize = true;
            this.labelTemplateFocus.ForeColor = System.Drawing.Color.Red;
            this.labelTemplateFocus.Location = new System.Drawing.Point(516, 11);
            this.labelTemplateFocus.Name = "labelTemplateFocus";
            this.labelTemplateFocus.Size = new System.Drawing.Size(83, 26);
            this.labelTemplateFocus.TabIndex = 20;
            this.labelTemplateFocus.Text = "Template Focus\r\nOFF\r\n";
            this.labelTemplateFocus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTemplateFocus.Click += new System.EventHandler(this.labelTemplateFocus_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopy.Enabled = false;
            this.buttonCopy.Location = new System.Drawing.Point(519, 98);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(75, 23);
            this.buttonCopy.TabIndex = 19;
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPaste.Enabled = false;
            this.buttonPaste.Location = new System.Drawing.Point(519, 127);
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(75, 23);
            this.buttonPaste.TabIndex = 18;
            this.buttonPaste.Text = "Paste";
            this.buttonPaste.UseVisualStyleBackColor = true;
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // textBoxFindAsset
            // 
            this.textBoxFindAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFindAsset.Location = new System.Drawing.Point(366, 13);
            this.textBoxFindAsset.Name = "textBoxFindAsset";
            this.textBoxFindAsset.Size = new System.Drawing.Size(147, 20);
            this.textBoxFindAsset.TabIndex = 17;
            this.textBoxFindAsset.TextChanged += new System.EventHandler(this.textBoxFindAsset_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(301, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Find Asset:";
            // 
            // buttonInternalEdit
            // 
            this.buttonInternalEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInternalEdit.Enabled = false;
            this.buttonInternalEdit.Location = new System.Drawing.Point(519, 307);
            this.buttonInternalEdit.Name = "buttonInternalEdit";
            this.buttonInternalEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonInternalEdit.TabIndex = 15;
            this.buttonInternalEdit.Text = "Edit Data";
            this.buttonInternalEdit.UseVisualStyleBackColor = true;
            this.buttonInternalEdit.Click += new System.EventHandler(this.buttonInternalEdit_Click);
            // 
            // buttonView
            // 
            this.buttonView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonView.Enabled = false;
            this.buttonView.Location = new System.Drawing.Point(519, 220);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(75, 23);
            this.buttonView.TabIndex = 14;
            this.buttonView.Text = "View";
            this.buttonView.UseVisualStyleBackColor = true;
            this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
            // 
            // buttonExportRaw
            // 
            this.buttonExportRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExportRaw.Enabled = false;
            this.buttonExportRaw.Location = new System.Drawing.Point(519, 249);
            this.buttonExportRaw.Name = "buttonExportRaw";
            this.buttonExportRaw.Size = new System.Drawing.Size(75, 23);
            this.buttonExportRaw.TabIndex = 13;
            this.buttonExportRaw.Text = "Export Raw";
            this.buttonExportRaw.UseVisualStyleBackColor = true;
            this.buttonExportRaw.Click += new System.EventHandler(this.buttonExportRaw_Click);
            // 
            // buttonEditAsset
            // 
            this.buttonEditAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditAsset.Enabled = false;
            this.buttonEditAsset.Location = new System.Drawing.Point(519, 278);
            this.buttonEditAsset.Name = "buttonEditAsset";
            this.buttonEditAsset.Size = new System.Drawing.Size(75, 23);
            this.buttonEditAsset.TabIndex = 12;
            this.buttonEditAsset.Text = "Edit Header";
            this.buttonEditAsset.UseVisualStyleBackColor = true;
            this.buttonEditAsset.Click += new System.EventHandler(this.buttonEditAsset_Click);
            // 
            // buttonDuplicate
            // 
            this.buttonDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDuplicate.Enabled = false;
            this.buttonDuplicate.Location = new System.Drawing.Point(519, 69);
            this.buttonDuplicate.Name = "buttonDuplicate";
            this.buttonDuplicate.Size = new System.Drawing.Size(75, 23);
            this.buttonDuplicate.TabIndex = 8;
            this.buttonDuplicate.Text = "Duplicate";
            this.buttonDuplicate.UseVisualStyleBackColor = true;
            this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
            // 
            // buttonRemoveAsset
            // 
            this.buttonRemoveAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemoveAsset.Enabled = false;
            this.buttonRemoveAsset.Location = new System.Drawing.Point(519, 156);
            this.buttonRemoveAsset.Name = "buttonRemoveAsset";
            this.buttonRemoveAsset.Size = new System.Drawing.Size(75, 23);
            this.buttonRemoveAsset.TabIndex = 7;
            this.buttonRemoveAsset.Text = "Remove";
            this.buttonRemoveAsset.UseVisualStyleBackColor = true;
            this.buttonRemoveAsset.Click += new System.EventHandler(this.buttonRemoveAsset_Click);
            // 
            // buttonAddAsset
            // 
            this.buttonAddAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddAsset.Enabled = false;
            this.buttonAddAsset.Location = new System.Drawing.Point(519, 40);
            this.buttonAddAsset.Name = "buttonAddAsset";
            this.buttonAddAsset.Size = new System.Drawing.Size(75, 23);
            this.buttonAddAsset.TabIndex = 6;
            this.buttonAddAsset.Text = "Add...";
            this.buttonAddAsset.UseVisualStyleBackColor = true;
            this.buttonAddAsset.Click += new System.EventHandler(this.buttonAddAsset_Click);
            // 
            // comboBoxAssetTypes
            // 
            this.comboBoxAssetTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAssetTypes.FormattingEnabled = true;
            this.comboBoxAssetTypes.Location = new System.Drawing.Point(90, 13);
            this.comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            this.comboBoxAssetTypes.Size = new System.Drawing.Size(205, 21);
            this.comboBoxAssetTypes.TabIndex = 5;
            this.comboBoxAssetTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxAssetTypes_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Show by Type:";
            // 
            // contextMenuStrip_ListBoxAssets
            // 
            this.contextMenuStrip_ListBoxAssets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Add,
            this.toolStripMenuItem_AddMulti,
            this.toolStripMenuItem_Duplicate,
            this.toolStripMenuItem_Copy,
            this.toolStripMenuItem_Paste,
            this.toolStripMenuItem_Remove,
            this.toolStripSeparator2,
            this.toolStripMenuItem_View,
            this.toolStripMenuItem_Export,
            this.toolStripMenuItem_EditHeader,
            this.toolStripMenuItem_EditData});
            this.contextMenuStrip_ListBoxAssets.Name = "contextMenuStrip_ListBoxAssets";
            this.contextMenuStrip_ListBoxAssets.Size = new System.Drawing.Size(189, 230);
            // 
            // toolStripMenuItem_Add
            // 
            this.toolStripMenuItem_Add.Name = "toolStripMenuItem_Add";
            this.toolStripMenuItem_Add.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_Add.Text = "Add (Ctrl + I)";
            this.toolStripMenuItem_Add.Click += new System.EventHandler(this.toolStripMenuItem_Add_Click);
            // 
            // toolStripMenuItem_AddMulti
            // 
            this.toolStripMenuItem_AddMulti.Name = "toolStripMenuItem_AddMulti";
            this.toolStripMenuItem_AddMulti.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_AddMulti.Text = "Add Multiple";
            this.toolStripMenuItem_AddMulti.Click += new System.EventHandler(this.toolStripMenuItem_AddMulti_Click);
            // 
            // toolStripMenuItem_Duplicate
            // 
            this.toolStripMenuItem_Duplicate.Name = "toolStripMenuItem_Duplicate";
            this.toolStripMenuItem_Duplicate.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_Duplicate.Text = "Duplicate (Ctrl + D)";
            this.toolStripMenuItem_Duplicate.Click += new System.EventHandler(this.toolStripMenuItem_Duplicate_Click);
            // 
            // toolStripMenuItem_Copy
            // 
            this.toolStripMenuItem_Copy.Name = "toolStripMenuItem_Copy";
            this.toolStripMenuItem_Copy.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_Copy.Text = "Copy (Ctrl + C)";
            this.toolStripMenuItem_Copy.Click += new System.EventHandler(this.toolStripMenuItem_Copy_Click);
            // 
            // toolStripMenuItem_Paste
            // 
            this.toolStripMenuItem_Paste.Name = "toolStripMenuItem_Paste";
            this.toolStripMenuItem_Paste.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_Paste.Text = "Paste (Ctrl + V)";
            this.toolStripMenuItem_Paste.Click += new System.EventHandler(this.toolStripMenuItem_Paste_Click);
            // 
            // toolStripMenuItem_Remove
            // 
            this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
            this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_Remove.Text = "Remove (Del)";
            this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // toolStripMenuItem_View
            // 
            this.toolStripMenuItem_View.Name = "toolStripMenuItem_View";
            this.toolStripMenuItem_View.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_View.Text = "View";
            this.toolStripMenuItem_View.Click += new System.EventHandler(this.toolStripMenuItem_View_Click);
            // 
            // toolStripMenuItem_Export
            // 
            this.toolStripMenuItem_Export.Name = "toolStripMenuItem_Export";
            this.toolStripMenuItem_Export.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_Export.Text = "Export";
            this.toolStripMenuItem_Export.Click += new System.EventHandler(this.toolStripMenuItem_Export_Click);
            // 
            // toolStripMenuItem_EditHeader
            // 
            this.toolStripMenuItem_EditHeader.Name = "toolStripMenuItem_EditHeader";
            this.toolStripMenuItem_EditHeader.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_EditHeader.Text = "Edit Header (Ctrl + H)";
            this.toolStripMenuItem_EditHeader.Click += new System.EventHandler(this.toolStripMenuItem_EditHeader_Click);
            // 
            // toolStripMenuItem_EditData
            // 
            this.toolStripMenuItem_EditData.Name = "toolStripMenuItem_EditData";
            this.toolStripMenuItem_EditData.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem_EditData.Text = "Edit Data (Ctrl + G)";
            this.toolStripMenuItem_EditData.Click += new System.EventHandler(this.toolStripMenuItem_EditData_Click);
            // 
            // hideButtonsToolStripMenuItem
            // 
            this.hideButtonsToolStripMenuItem.Name = "hideButtonsToolStripMenuItem";
            this.hideButtonsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.hideButtonsToolStripMenuItem.Text = "Hide Buttons";
            this.hideButtonsToolStripMenuItem.Click += new System.EventHandler(this.hideButtonsToolStripMenuItem_Click);
            // 
            // ArchiveEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "ArchiveEditor";
            this.ShowIcon = false;
            this.Text = "Archive Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.contextMenuStrip_ListBoxAssets.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCurrentFilename;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonRemoveLayer;
        private System.Windows.Forms.Button buttonAddLayer;
        private System.Windows.Forms.ComboBox comboBoxLayers;
        private System.Windows.Forms.ComboBox comboBoxLayerTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxAssets;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonAddAsset;
        private System.Windows.Forms.ComboBox comboBoxAssetTypes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDuplicate;
        private System.Windows.Forms.Button buttonRemoveAsset;
        private System.Windows.Forms.Button buttonEditAsset;
        private System.Windows.Forms.Button buttonExportRaw;
        private System.Windows.Forms.Button buttonView;
        private System.Windows.Forms.Button buttonInternalEdit;
        private System.Windows.Forms.TextBox textBoxFindAsset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Button buttonArrowDown;
        private System.Windows.Forms.Button buttonArrowUp;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportTXDArchiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTXDArchiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem importHIPArchiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hipHopToolExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_ListBoxAssets;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Add;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Duplicate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Copy;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Paste;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_View;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Export;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_EditHeader;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_EditData;
        private System.Windows.Forms.Label labelTemplateFocus;
        private System.Windows.Forms.ToolStripMenuItem importMultipleAssetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_AddMulti;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelSelectionCount;
        private System.Windows.Forms.ToolStripMenuItem hideButtonsToolStripMenuItem;
    }
}