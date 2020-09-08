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
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonRandomSeed = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSeed = new System.Windows.Forms.TextBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.propertyGridSettings = new System.Windows.Forms.PropertyGrid();
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
            this.richTextBoxHelp = new System.Windows.Forms.RichTextBox();
            this.buttonReset = new System.Windows.Forms.Button();
            this.comboBoxGame = new System.Windows.Forms.ComboBox();
            this.checkForUpdatesOnEditorFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.buttonPerform.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPerform.Enabled = false;
            this.buttonPerform.Location = new System.Drawing.Point(12, 535);
            this.buttonPerform.Name = "buttonPerform";
            this.buttonPerform.Size = new System.Drawing.Size(431, 23);
            this.buttonPerform.TabIndex = 8;
            this.buttonPerform.Text = "Perform";
            this.buttonPerform.UseVisualStyleBackColor = true;
            this.buttonPerform.Click += new System.EventHandler(this.buttonPerform_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonHelp.Location = new System.Drawing.Point(391, 132);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(52, 23);
            this.buttonHelp.TabIndex = 25;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.ButtonHelp_Click);
            // 
            // buttonRandomSeed
            // 
            this.buttonRandomSeed.Location = new System.Drawing.Point(12, 132);
            this.buttonRandomSeed.Name = "buttonRandomSeed";
            this.buttonRandomSeed.Size = new System.Drawing.Size(95, 23);
            this.buttonRandomSeed.TabIndex = 29;
            this.buttonRandomSeed.Text = "Random Seed";
            this.buttonRandomSeed.UseVisualStyleBackColor = true;
            this.buttonRandomSeed.Click += new System.EventHandler(this.ButtonRandomSeed_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "You can type something to generate a seed:";
            // 
            // textBoxSeed
            // 
            this.textBoxSeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSeed.Location = new System.Drawing.Point(12, 93);
            this.textBoxSeed.Name = "textBoxSeed";
            this.textBoxSeed.Size = new System.Drawing.Size(431, 20);
            this.textBoxSeed.TabIndex = 0;
            this.textBoxSeed.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(318, 132);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(67, 23);
            this.buttonClear.TabIndex = 32;
            this.buttonClear.Text = "Disable All";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClear_Click);
            // 
            // propertyGridSettings
            // 
            this.propertyGridSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridSettings.HelpVisible = false;
            this.propertyGridSettings.Location = new System.Drawing.Point(12, 161);
            this.propertyGridSettings.Name = "propertyGridSettings";
            this.propertyGridSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridSettings.Size = new System.Drawing.Size(431, 266);
            this.propertyGridSettings.TabIndex = 34;
            this.propertyGridSettings.ToolbarVisible = false;
            this.propertyGridSettings.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGridAsset_SelectedGridItemChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsFileToolStripMenuItem,
            this.rootDirectoryToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(455, 24);
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
            this.checkForUpdatesOnStartupToolStripMenuItem,
            this.checkForUpdatesOnEditorFilesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // checkForUpdatesOnStartupToolStripMenuItem
            // 
            this.checkForUpdatesOnStartupToolStripMenuItem.Name = "checkForUpdatesOnStartupToolStripMenuItem";
            this.checkForUpdatesOnStartupToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
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
            this.labelSeed.Location = new System.Drawing.Point(12, 116);
            this.labelSeed.Name = "labelSeed";
            this.labelSeed.Size = new System.Drawing.Size(38, 13);
            this.labelSeed.TabIndex = 37;
            this.labelSeed.Text = "Seed: ";
            // 
            // richTextBoxHelp
            // 
            this.richTextBoxHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxHelp.Location = new System.Drawing.Point(12, 433);
            this.richTextBoxHelp.Name = "richTextBoxHelp";
            this.richTextBoxHelp.ReadOnly = true;
            this.richTextBoxHelp.Size = new System.Drawing.Size(431, 96);
            this.richTextBoxHelp.TabIndex = 38;
            this.richTextBoxHelp.Text = "";
            // 
            // buttonReset
            // 
            this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReset.Location = new System.Drawing.Point(261, 132);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(51, 23);
            this.buttonReset.TabIndex = 39;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // comboBoxGame
            // 
            this.comboBoxGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxGame.FormattingEnabled = true;
            this.comboBoxGame.Items.AddRange(new object[] {
            "SpongeBob SquarePants: Battle For Bikini Bottom",
            "Scooby-Doo: Night of 100 Frights",
            "The SpongeBob SquarePants Movie"});
            this.comboBoxGame.Location = new System.Drawing.Point(12, 53);
            this.comboBoxGame.Name = "comboBoxGame";
            this.comboBoxGame.Size = new System.Drawing.Size(431, 21);
            this.comboBoxGame.TabIndex = 40;
            this.comboBoxGame.SelectedIndexChanged += new System.EventHandler(this.comboBoxGame_SelectedIndexChanged);
            // 
            // checkForUpdatesOnEditorFilesToolStripMenuItem
            // 
            this.checkForUpdatesOnEditorFilesToolStripMenuItem.Name = "checkForUpdatesOnEditorFilesToolStripMenuItem";
            this.checkForUpdatesOnEditorFilesToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.checkForUpdatesOnEditorFilesToolStripMenuItem.Text = "Check For Updates On EditorFiles...";
            this.checkForUpdatesOnEditorFilesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesOnEditorFilesToolStripMenuItem_Click);
            // 
            // RandomizerMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 570);
            this.Controls.Add(this.comboBoxGame);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.richTextBoxHelp);
            this.Controls.Add(this.labelSeed);
            this.Controls.Add(this.labelBackupDir);
            this.Controls.Add(this.propertyGridSettings);
            this.Controls.Add(this.buttonRandomSeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSeed);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonPerform);
            this.Controls.Add(this.labelRootDir);
            this.Controls.Add(this.menuStrip1);
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
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonRandomSeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSeed;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.PropertyGrid propertyGridSettings;
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
        private System.Windows.Forms.RichTextBox richTextBoxHelp;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.ComboBox comboBoxGame;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesOnEditorFilesToolStripMenuItem;
    }
}