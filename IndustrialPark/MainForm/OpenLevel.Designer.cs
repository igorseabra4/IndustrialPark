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
            this.ctxLocalization = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pastePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.grpImportSettings.SuspendLayout();
            this.ctxLocalization.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnImport
            // 
            resources.ApplyResources(this.btnImport, "btnImport");
            this.btnImport.Name = "btnImport";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // grpImportSettings
            // 
            resources.ApplyResources(this.grpImportSettings, "grpImportSettings");
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
            this.grpImportSettings.Name = "grpImportSettings";
            this.grpImportSettings.TabStop = false;
            // 
            // lvwLocalization
            // 
            resources.ApplyResources(this.lvwLocalization, "lvwLocalization");
            this.lvwLocalization.CheckBoxes = true;
            this.lvwLocalization.ContextMenuStrip = this.ctxLocalization;
            this.lvwLocalization.HideSelection = false;
            this.lvwLocalization.Name = "lvwLocalization";
            this.lvwLocalization.SmallImageList = this.imgFlagIcons;
            this.lvwLocalization.UseCompatibleStateImageBehavior = false;
            this.lvwLocalization.View = System.Windows.Forms.View.List;
            this.lvwLocalization.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkBoxUpdatedInList);
            this.lvwLocalization.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvwLocalization_KeyDown);
            // 
            // ctxLocalization
            // 
            resources.ApplyResources(this.ctxLocalization, "ctxLocalization");
            this.ctxLocalization.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxLocalization.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyPathToolStripMenuItem,
            this.pastePathToolStripMenuItem});
            this.ctxLocalization.Name = "ctxLocalization";
            this.ctxLocalization.Opening += new System.ComponentModel.CancelEventHandler(this.ctxLocalization_Opening);
            // 
            // copyPathToolStripMenuItem
            // 
            resources.ApplyResources(this.copyPathToolStripMenuItem, "copyPathToolStripMenuItem");
            this.copyPathToolStripMenuItem.Name = "copyPathToolStripMenuItem";
            this.copyPathToolStripMenuItem.Click += new System.EventHandler(this.copyPathToolStripMenuItem_Click);
            // 
            // pastePathToolStripMenuItem
            // 
            resources.ApplyResources(this.pastePathToolStripMenuItem, "pastePathToolStripMenuItem");
            this.pastePathToolStripMenuItem.Name = "pastePathToolStripMenuItem";
            this.pastePathToolStripMenuItem.Click += new System.EventHandler(this.pastePathToolStripMenuItem_Click);
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
            resources.ApplyResources(this.btnResetAll, "btnResetAll");
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnAddLocalizationFile
            // 
            resources.ApplyResources(this.btnAddLocalizationFile, "btnAddLocalizationFile");
            this.btnAddLocalizationFile.Name = "btnAddLocalizationFile";
            this.btnAddLocalizationFile.UseVisualStyleBackColor = true;
            this.btnAddLocalizationFile.Click += new System.EventHandler(this.btnAddLocalizationFile_Click);
            // 
            // btnBOOTSelect
            // 
            resources.ApplyResources(this.btnBOOTSelect, "btnBOOTSelect");
            this.btnBOOTSelect.Name = "btnBOOTSelect";
            this.btnBOOTSelect.UseVisualStyleBackColor = true;
            this.btnBOOTSelect.Click += new System.EventHandler(this.btnBOOTSelect_Click);
            // 
            // btnHOPSelect
            // 
            resources.ApplyResources(this.btnHOPSelect, "btnHOPSelect");
            this.btnHOPSelect.Name = "btnHOPSelect";
            this.btnHOPSelect.UseVisualStyleBackColor = true;
            this.btnHOPSelect.Click += new System.EventHandler(this.btnHOPSelect_Click);
            // 
            // btnHIPSelect
            // 
            resources.ApplyResources(this.btnHIPSelect, "btnHIPSelect");
            this.btnHIPSelect.Name = "btnHIPSelect";
            this.btnHIPSelect.UseVisualStyleBackColor = true;
            this.btnHIPSelect.Click += new System.EventHandler(this.btnHIPSelect_Click);
            // 
            // txtBOOT
            // 
            resources.ApplyResources(this.txtBOOT, "txtBOOT");
            this.txtBOOT.Name = "txtBOOT";
            // 
            // txtHOP
            // 
            resources.ApplyResources(this.txtHOP, "txtHOP");
            this.txtHOP.Name = "txtHOP";
            // 
            // txtHIP
            // 
            resources.ApplyResources(this.txtHIP, "txtHIP");
            this.txtHIP.Name = "txtHIP";
            // 
            // lblLocalization
            // 
            resources.ApplyResources(this.lblLocalization, "lblLocalization");
            this.lblLocalization.Name = "lblLocalization";
            // 
            // chkHIP
            // 
            resources.ApplyResources(this.chkHIP, "chkHIP");
            this.chkHIP.Name = "chkHIP";
            this.chkHIP.UseVisualStyleBackColor = true;
            this.chkHIP.CheckedChanged += new System.EventHandler(this.checkBoxUpdated);
            // 
            // chkHOP
            // 
            resources.ApplyResources(this.chkHOP, "chkHOP");
            this.chkHOP.Name = "chkHOP";
            this.chkHOP.UseVisualStyleBackColor = true;
            this.chkHOP.CheckedChanged += new System.EventHandler(this.checkBoxUpdated);
            // 
            // chkBOOT
            // 
            resources.ApplyResources(this.chkBOOT, "chkBOOT");
            this.chkBOOT.Name = "chkBOOT";
            this.chkBOOT.UseVisualStyleBackColor = true;
            this.chkBOOT.CheckedChanged += new System.EventHandler(this.checkBoxUpdated);
            // 
            // OpenLevel
            // 
            this.AcceptButton = this.btnImport;
            resources.ApplyResources(this, "$this");
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.grpImportSettings);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.Name = "OpenLevel";
            this.ShowIcon = false;
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