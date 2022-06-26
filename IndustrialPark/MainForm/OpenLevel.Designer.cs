namespace IndustrialPark
{
    partial class OpenLevel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OpenLevel));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.grpImportSettings = new System.Windows.Forms.GroupBox();
            this.lvwLocalization = new System.Windows.Forms.ListView();
            this.imgFlagIcons = new System.Windows.Forms.ImageList(this.components);
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnAddLocalizationFile = new System.Windows.Forms.Button();
            this.btnBOOTSelect = new System.Windows.Forms.Button();
            this.btnHOPSelect = new System.Windows.Forms.Button();
            this.btnHIPSelect = new System.Windows.Forms.Button();
            this.txtBOOT = new System.Windows.Forms.TextBox();
            this.txtHOP = new System.Windows.Forms.TextBox();
            this.txtHIP = new System.Windows.Forms.TextBox();
            this.lblLocalization = new System.Windows.Forms.Label();
            this.chkHIP = new System.Windows.Forms.CheckBox();
            this.chkHOP = new System.Windows.Forms.CheckBox();
            this.chkBOOT = new System.Windows.Forms.CheckBox();
            this.ctxLocalization = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pastePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpImportSettings.SuspendLayout();
            this.ctxLocalization.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(326, 263);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnImport
            // 
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(222, 263);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(98, 23);
            this.btnImport.TabIndex = 13;
            this.btnImport.Text = "Open Level";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // grpImportSettings
            // 
            this.grpImportSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpImportSettings.Controls.Add(this.lvwLocalization);
            this.grpImportSettings.Controls.Add(this.btnResetAll);
            this.grpImportSettings.Controls.Add(this.btnAddLocalizationFile);
            this.grpImportSettings.Controls.Add(this.btnBOOTSelect);
            this.grpImportSettings.Controls.Add(this.btnHOPSelect);
            this.grpImportSettings.Controls.Add(this.btnHIPSelect);
            this.grpImportSettings.Controls.Add(this.txtBOOT);
            this.grpImportSettings.Controls.Add(this.txtHOP);
            this.grpImportSettings.Controls.Add(this.txtHIP);
            this.grpImportSettings.Controls.Add(this.lblLocalization);
            this.grpImportSettings.Controls.Add(this.chkHIP);
            this.grpImportSettings.Controls.Add(this.chkHOP);
            this.grpImportSettings.Controls.Add(this.chkBOOT);
            this.grpImportSettings.Location = new System.Drawing.Point(12, 12);
            this.grpImportSettings.Name = "grpImportSettings";
            this.grpImportSettings.Size = new System.Drawing.Size(412, 245);
            this.grpImportSettings.TabIndex = 0;
            this.grpImportSettings.TabStop = false;
            this.grpImportSettings.Text = "Select Files";
            // 
            // lvwLocalization
            // 
            this.lvwLocalization.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwLocalization.CheckBoxes = true;
            this.lvwLocalization.ContextMenuStrip = this.ctxLocalization;
            this.lvwLocalization.HideSelection = false;
            this.lvwLocalization.Location = new System.Drawing.Point(28, 129);
            this.lvwLocalization.Name = "lvwLocalization";
            this.lvwLocalization.Size = new System.Drawing.Size(357, 81);
            this.lvwLocalization.SmallImageList = this.imgFlagIcons;
            this.lvwLocalization.TabIndex = 16;
            this.lvwLocalization.UseCompatibleStateImageBehavior = false;
            this.lvwLocalization.View = System.Windows.Forms.View.List;
            this.lvwLocalization.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkBoxUpdatedInList);
            this.lvwLocalization.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvwLocalization_KeyDown);
            // 
            // imgFlagIcons
            // 
            this.imgFlagIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgFlagIcons.ImageStream")));
            this.imgFlagIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.imgFlagIcons.Images.SetKeyName(0, "be.png");
            this.imgFlagIcons.Images.SetKeyName(1, "ch.png");
            this.imgFlagIcons.Images.SetKeyName(2, "cz.png");
            this.imgFlagIcons.Images.SetKeyName(3, "de.png");
            this.imgFlagIcons.Images.SetKeyName(4, "dk.png");
            this.imgFlagIcons.Images.SetKeyName(5, "es.png");
            this.imgFlagIcons.Images.SetKeyName(6, "fi.png");
            this.imgFlagIcons.Images.SetKeyName(7, "fr.png");
            this.imgFlagIcons.Images.SetKeyName(8, "it.png");
            this.imgFlagIcons.Images.SetKeyName(9, "jp.png");
            this.imgFlagIcons.Images.SetKeyName(10, "kr.png");
            this.imgFlagIcons.Images.SetKeyName(11, "nl.png");
            this.imgFlagIcons.Images.SetKeyName(12, "no.png");
            this.imgFlagIcons.Images.SetKeyName(13, "pl.png");
            this.imgFlagIcons.Images.SetKeyName(14, "pt.png");
            this.imgFlagIcons.Images.SetKeyName(15, "ru.png");
            this.imgFlagIcons.Images.SetKeyName(16, "se.png");
            this.imgFlagIcons.Images.SetKeyName(17, "sk.png");
            this.imgFlagIcons.Images.SetKeyName(18, "tw.png");
            this.imgFlagIcons.Images.SetKeyName(19, "uk.png");
            this.imgFlagIcons.Images.SetKeyName(20, "us.png");
            // 
            // btnResetAll
            // 
            this.btnResetAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnResetAll.Location = new System.Drawing.Point(28, 216);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(91, 23);
            this.btnResetAll.TabIndex = 15;
            this.btnResetAll.Text = "Reset All";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnAddLocalizationFile
            // 
            this.btnAddLocalizationFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddLocalizationFile.Location = new System.Drawing.Point(270, 216);
            this.btnAddLocalizationFile.Name = "btnAddLocalizationFile";
            this.btnAddLocalizationFile.Size = new System.Drawing.Size(115, 23);
            this.btnAddLocalizationFile.TabIndex = 12;
            this.btnAddLocalizationFile.Text = "Add Files Manually...";
            this.btnAddLocalizationFile.UseVisualStyleBackColor = true;
            this.btnAddLocalizationFile.Click += new System.EventHandler(this.btnAddLocalizationFile_Click);
            // 
            // btnBOOTSelect
            // 
            this.btnBOOTSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBOOTSelect.Location = new System.Drawing.Point(355, 72);
            this.btnBOOTSelect.Name = "btnBOOTSelect";
            this.btnBOOTSelect.Size = new System.Drawing.Size(30, 22);
            this.btnBOOTSelect.TabIndex = 9;
            this.btnBOOTSelect.Text = "...";
            this.btnBOOTSelect.UseVisualStyleBackColor = true;
            this.btnBOOTSelect.Click += new System.EventHandler(this.btnBOOTSelect_Click);
            // 
            // btnHOPSelect
            // 
            this.btnHOPSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHOPSelect.Location = new System.Drawing.Point(355, 45);
            this.btnHOPSelect.Name = "btnHOPSelect";
            this.btnHOPSelect.Size = new System.Drawing.Size(30, 22);
            this.btnHOPSelect.TabIndex = 6;
            this.btnHOPSelect.Text = "...";
            this.btnHOPSelect.UseVisualStyleBackColor = true;
            this.btnHOPSelect.Click += new System.EventHandler(this.btnHOPSelect_Click);
            // 
            // btnHIPSelect
            // 
            this.btnHIPSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHIPSelect.Location = new System.Drawing.Point(355, 18);
            this.btnHIPSelect.Name = "btnHIPSelect";
            this.btnHIPSelect.Size = new System.Drawing.Size(30, 22);
            this.btnHIPSelect.TabIndex = 3;
            this.btnHIPSelect.Text = "...";
            this.btnHIPSelect.UseVisualStyleBackColor = true;
            this.btnHIPSelect.Click += new System.EventHandler(this.btnHIPSelect_Click);
            // 
            // txtBOOT
            // 
            this.txtBOOT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBOOT.Location = new System.Drawing.Point(111, 73);
            this.txtBOOT.Name = "txtBOOT";
            this.txtBOOT.Size = new System.Drawing.Size(238, 20);
            this.txtBOOT.TabIndex = 8;
            // 
            // txtHOP
            // 
            this.txtHOP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHOP.Location = new System.Drawing.Point(111, 46);
            this.txtHOP.Name = "txtHOP";
            this.txtHOP.Size = new System.Drawing.Size(238, 20);
            this.txtHOP.TabIndex = 5;
            // 
            // txtHIP
            // 
            this.txtHIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHIP.Location = new System.Drawing.Point(111, 19);
            this.txtHIP.Name = "txtHIP";
            this.txtHIP.Size = new System.Drawing.Size(238, 20);
            this.txtHIP.TabIndex = 2;
            // 
            // lblLocalization
            // 
            this.lblLocalization.AutoSize = true;
            this.lblLocalization.Location = new System.Drawing.Point(25, 113);
            this.lblLocalization.Name = "lblLocalization";
            this.lblLocalization.Size = new System.Drawing.Size(87, 13);
            this.lblLocalization.TabIndex = 10;
            this.lblLocalization.Text = "Localization Files";
            // 
            // chkHIP
            // 
            this.chkHIP.AutoSize = true;
            this.chkHIP.Location = new System.Drawing.Point(28, 21);
            this.chkHIP.Name = "chkHIP";
            this.chkHIP.Size = new System.Drawing.Size(63, 17);
            this.chkHIP.TabIndex = 1;
            this.chkHIP.Text = "HIP File";
            this.chkHIP.UseVisualStyleBackColor = true;
            this.chkHIP.CheckedChanged += new System.EventHandler(this.checkBoxUpdated);
            // 
            // chkHOP
            // 
            this.chkHOP.AutoSize = true;
            this.chkHOP.Location = new System.Drawing.Point(28, 48);
            this.chkHOP.Name = "chkHOP";
            this.chkHOP.Size = new System.Drawing.Size(68, 17);
            this.chkHOP.TabIndex = 4;
            this.chkHOP.Text = "HOP File";
            this.chkHOP.UseVisualStyleBackColor = true;
            this.chkHOP.CheckedChanged += new System.EventHandler(this.checkBoxUpdated);
            // 
            // chkBOOT
            // 
            this.chkBOOT.AutoSize = true;
            this.chkBOOT.Location = new System.Drawing.Point(28, 75);
            this.chkBOOT.Name = "chkBOOT";
            this.chkBOOT.Size = new System.Drawing.Size(56, 17);
            this.chkBOOT.TabIndex = 7;
            this.chkBOOT.Text = "BOOT";
            this.chkBOOT.UseVisualStyleBackColor = true;
            this.chkBOOT.CheckedChanged += new System.EventHandler(this.checkBoxUpdated);
            // 
            // ctxLocalization
            // 
            this.ctxLocalization.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPathToolStripMenuItem,
            this.pastePathToolStripMenuItem});
            this.ctxLocalization.Name = "ctxLocalization";
            this.ctxLocalization.Size = new System.Drawing.Size(130, 48);
            this.ctxLocalization.Opening += new System.ComponentModel.CancelEventHandler(this.ctxLocalization_Opening);
            // 
            // copyPathToolStripMenuItem
            // 
            this.copyPathToolStripMenuItem.Name = "copyPathToolStripMenuItem";
            this.copyPathToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.copyPathToolStripMenuItem.Text = "Copy Path";
            this.copyPathToolStripMenuItem.Click += new System.EventHandler(this.copyPathToolStripMenuItem_Click);
            // 
            // pastePathToolStripMenuItem
            // 
            this.pastePathToolStripMenuItem.Name = "pastePathToolStripMenuItem";
            this.pastePathToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.pastePathToolStripMenuItem.Text = "Paste Path";
            this.pastePathToolStripMenuItem.Click += new System.EventHandler(this.pastePathToolStripMenuItem_Click);
            // 
            // OpenLevel
            // 
            this.AcceptButton = this.btnImport;
            this.AccessibleDescription = "Import a level into Industrial Park";
            this.AccessibleName = "Import Level";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(436, 298);
            this.Controls.Add(this.grpImportSettings);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(305, 280);
            this.Name = "OpenLevel";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Level";
            this.grpImportSettings.ResumeLayout(false);
            this.grpImportSettings.PerformLayout();
            this.ctxLocalization.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.GroupBox grpImportSettings;
        private System.Windows.Forms.CheckBox chkBOOT;
        private System.Windows.Forms.CheckBox chkHIP;
        private System.Windows.Forms.CheckBox chkHOP;
        private System.Windows.Forms.Label lblLocalization;
        private System.Windows.Forms.TextBox txtHIP;
        private System.Windows.Forms.TextBox txtBOOT;
        private System.Windows.Forms.TextBox txtHOP;
        private System.Windows.Forms.Button btnHIPSelect;
        private System.Windows.Forms.Button btnBOOTSelect;
        private System.Windows.Forms.Button btnHOPSelect;
        private System.Windows.Forms.Button btnAddLocalizationFile;
        private System.Windows.Forms.ContextMenuStrip ctxLocalization;
        private System.Windows.Forms.ToolStripMenuItem copyPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pastePathToolStripMenuItem;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.ListView lvwLocalization;
        private System.Windows.Forms.ImageList imgFlagIcons;
    }
}