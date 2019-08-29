namespace IndustrialPark.Randomizer
{
    partial class RandomizerMenu
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
            this.labelRootDir = new System.Windows.Forms.Label();
            this.buttonPerform = new System.Windows.Forms.Button();
            this.checkedListBoxMethods = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxNotRecommended = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonRandomSeed = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSeed = new System.Windows.Forms.TextBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.propertyGridAsset = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settingsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rootDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseBackupDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useBackupDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseRootDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseSingleFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.labelBackupDir = new System.Windows.Forms.Label();
            this.labelSeed = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelRootDir
            // 
            this.labelRootDir.AutoSize = true;
            this.labelRootDir.Location = new System.Drawing.Point(12, 37);
            this.labelRootDir.Name = "labelRootDir";
            this.labelRootDir.Size = new System.Drawing.Size(107, 13);
            this.labelRootDir.TabIndex = 1;
            this.labelRootDir.Text = "Root Directory: None";
            // 
            // buttonPerform
            // 
            this.buttonPerform.Enabled = false;
            this.buttonPerform.Location = new System.Drawing.Point(226, 409);
            this.buttonPerform.Name = "buttonPerform";
            this.buttonPerform.Size = new System.Drawing.Size(210, 23);
            this.buttonPerform.TabIndex = 8;
            this.buttonPerform.Text = "Perform";
            this.buttonPerform.UseVisualStyleBackColor = true;
            this.buttonPerform.Click += new System.EventHandler(this.buttonPerform_Click);
            // 
            // checkedListBoxMethods
            // 
            this.checkedListBoxMethods.ColumnWidth = 162;
            this.checkedListBoxMethods.FormattingEnabled = true;
            this.checkedListBoxMethods.Location = new System.Drawing.Point(12, 53);
            this.checkedListBoxMethods.MultiColumn = true;
            this.checkedListBoxMethods.Name = "checkedListBoxMethods";
            this.checkedListBoxMethods.Size = new System.Drawing.Size(208, 379);
            this.checkedListBoxMethods.TabIndex = 13;
            this.checkedListBoxMethods.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxMethods_ItemCheck);
            // 
            // checkedListBoxNotRecommended
            // 
            this.checkedListBoxNotRecommended.FormattingEnabled = true;
            this.checkedListBoxNotRecommended.Location = new System.Drawing.Point(226, 150);
            this.checkedListBoxNotRecommended.Name = "checkedListBoxNotRecommended";
            this.checkedListBoxNotRecommended.Size = new System.Drawing.Size(210, 229);
            this.checkedListBoxNotRecommended.TabIndex = 21;
            this.checkedListBoxNotRecommended.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxNotRecommended_ItemCheck);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(226, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Not recommended:";
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(334, 108);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(102, 23);
            this.buttonHelp.TabIndex = 25;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.ButtonHelp_Click);
            // 
            // buttonRandomSeed
            // 
            this.buttonRandomSeed.Location = new System.Drawing.Point(226, 108);
            this.buttonRandomSeed.Name = "buttonRandomSeed";
            this.buttonRandomSeed.Size = new System.Drawing.Size(102, 23);
            this.buttonRandomSeed.TabIndex = 29;
            this.buttonRandomSeed.Text = "Random Seed";
            this.buttonRandomSeed.UseVisualStyleBackColor = true;
            this.buttonRandomSeed.Click += new System.EventHandler(this.ButtonRandomSeed_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(226, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "You can type something to generate a seed.";
            // 
            // textBoxSeed
            // 
            this.textBoxSeed.Location = new System.Drawing.Point(226, 69);
            this.textBoxSeed.Name = "textBoxSeed";
            this.textBoxSeed.Size = new System.Drawing.Size(210, 20);
            this.textBoxSeed.TabIndex = 0;
            this.textBoxSeed.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(226, 380);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(210, 23);
            this.buttonClear.TabIndex = 32;
            this.buttonClear.Text = "Disable Everything";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClear_Click);
            // 
            // propertyGridAsset
            // 
            this.propertyGridAsset.Location = new System.Drawing.Point(442, 27);
            this.propertyGridAsset.Name = "propertyGridAsset";
            this.propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridAsset.Size = new System.Drawing.Size(272, 405);
            this.propertyGridAsset.TabIndex = 34;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsFileToolStripMenuItem,
            this.rootDirectoryToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(722, 24);
            this.menuStrip1.TabIndex = 35;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settingsFileToolStripMenuItem
            // 
            this.settingsFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.settingsFileToolStripMenuItem.Name = "settingsFileToolStripMenuItem";
            this.settingsFileToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            this.settingsFileToolStripMenuItem.Text = "Settings File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // rootDirectoryToolStripMenuItem
            // 
            this.rootDirectoryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chooseBackupDirectoryToolStripMenuItem,
            this.useBackupDirectoryToolStripMenuItem,
            this.chooseRootDirectoryToolStripMenuItem,
            this.chooseSingleFileToolStripMenuItem});
            this.rootDirectoryToolStripMenuItem.Name = "rootDirectoryToolStripMenuItem";
            this.rootDirectoryToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.rootDirectoryToolStripMenuItem.Text = "Game Directory";
            // 
            // chooseBackupDirectoryToolStripMenuItem
            // 
            this.chooseBackupDirectoryToolStripMenuItem.Name = "chooseBackupDirectoryToolStripMenuItem";
            this.chooseBackupDirectoryToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.chooseBackupDirectoryToolStripMenuItem.Text = "Choose Backup Directory";
            this.chooseBackupDirectoryToolStripMenuItem.Click += new System.EventHandler(this.ChooseBackupDirectoryToolStripMenuItem_Click);
            // 
            // useBackupDirectoryToolStripMenuItem
            // 
            this.useBackupDirectoryToolStripMenuItem.Name = "useBackupDirectoryToolStripMenuItem";
            this.useBackupDirectoryToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.useBackupDirectoryToolStripMenuItem.Text = "Use Backup Directory";
            this.useBackupDirectoryToolStripMenuItem.Click += new System.EventHandler(this.UseBackupDirectoryToolStripMenuItem_Click);
            // 
            // chooseRootDirectoryToolStripMenuItem
            // 
            this.chooseRootDirectoryToolStripMenuItem.Name = "chooseRootDirectoryToolStripMenuItem";
            this.chooseRootDirectoryToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.chooseRootDirectoryToolStripMenuItem.Text = "Choose Game Directory";
            this.chooseRootDirectoryToolStripMenuItem.Click += new System.EventHandler(this.ChooseRootDirectoryToolStripMenuItem_Click);
            // 
            // chooseSingleFileToolStripMenuItem
            // 
            this.chooseSingleFileToolStripMenuItem.Name = "chooseSingleFileToolStripMenuItem";
            this.chooseSingleFileToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.chooseSingleFileToolStripMenuItem.Text = "Choose Single File";
            this.chooseSingleFileToolStripMenuItem.Click += new System.EventHandler(this.ChooseSingleFileToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesOnStartupToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // checkForUpdatesOnStartupToolStripMenuItem
            // 
            this.checkForUpdatesOnStartupToolStripMenuItem.Name = "checkForUpdatesOnStartupToolStripMenuItem";
            this.checkForUpdatesOnStartupToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.checkForUpdatesOnStartupToolStripMenuItem.Text = "Check For Updates On Startup";
            this.checkForUpdatesOnStartupToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesOnStartupToolStripMenuItem_Click);
            // 
            // labelBackupDir
            // 
            this.labelBackupDir.AutoSize = true;
            this.labelBackupDir.Location = new System.Drawing.Point(12, 24);
            this.labelBackupDir.Name = "labelBackupDir";
            this.labelBackupDir.Size = new System.Drawing.Size(121, 13);
            this.labelBackupDir.TabIndex = 36;
            this.labelBackupDir.Text = "Backup Directory: None";
            // 
            // labelSeed
            // 
            this.labelSeed.AutoSize = true;
            this.labelSeed.Location = new System.Drawing.Point(226, 92);
            this.labelSeed.Name = "labelSeed";
            this.labelSeed.Size = new System.Drawing.Size(38, 13);
            this.labelSeed.TabIndex = 37;
            this.labelSeed.Text = "Seed: ";
            // 
            // RandomizerMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 439);
            this.Controls.Add(this.labelSeed);
            this.Controls.Add(this.labelBackupDir);
            this.Controls.Add(this.propertyGridAsset);
            this.Controls.Add(this.buttonRandomSeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSeed);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkedListBoxNotRecommended);
            this.Controls.Add(this.checkedListBoxMethods);
            this.Controls.Add(this.buttonPerform);
            this.Controls.Add(this.labelRootDir);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "RandomizerMenu";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Randomizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RandomizerMenu_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelRootDir;
        private System.Windows.Forms.Button buttonPerform;
        private System.Windows.Forms.CheckedListBox checkedListBoxMethods;
        private System.Windows.Forms.CheckedListBox checkedListBoxNotRecommended;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonRandomSeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSeed;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.PropertyGrid propertyGridAsset;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rootDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseBackupDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useBackupDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseRootDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseSingleFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesOnStartupToolStripMenuItem;
        private System.Windows.Forms.Label labelBackupDir;
        private System.Windows.Forms.Label labelSeed;
    }
}