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
            buttonPerform = new System.Windows.Forms.Button();
            buttonRandomSeed = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            textBoxSeed = new System.Windows.Forms.TextBox();
            buttonClear = new System.Windows.Forms.Button();
            propertyGridSettings = new System.Windows.Forms.PropertyGrid();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            settingsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            setHeavyModManagerPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            checkForUpdatesOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            checkForUpdatesOnEditorFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            labelHmmPath = new System.Windows.Forms.Label();
            labelSeed = new System.Windows.Forms.Label();
            richTextBoxHelp = new System.Windows.Forms.RichTextBox();
            buttonReset = new System.Windows.Forms.Button();
            comboBoxGame = new System.Windows.Forms.ComboBox();
            radioButtonHmm = new System.Windows.Forms.RadioButton();
            radioButtonFiles = new System.Windows.Forms.RadioButton();
            buttonSelectMainFiles = new System.Windows.Forms.Button();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // buttonPerform
            // 
            buttonPerform.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonPerform.Enabled = false;
            buttonPerform.Location = new System.Drawing.Point(11, 667);
            buttonPerform.Name = "buttonPerform";
            buttonPerform.Size = new System.Drawing.Size(503, 30);
            buttonPerform.TabIndex = 8;
            buttonPerform.Text = "Run";
            buttonPerform.UseVisualStyleBackColor = true;
            buttonPerform.Click += buttonPerform_Click;
            // 
            // buttonRandomSeed
            // 
            buttonRandomSeed.Location = new System.Drawing.Point(12, 173);
            buttonRandomSeed.Name = "buttonRandomSeed";
            buttonRandomSeed.Size = new System.Drawing.Size(111, 27);
            buttonRandomSeed.TabIndex = 29;
            buttonRandomSeed.Text = "Random Seed";
            buttonRandomSeed.UseVisualStyleBackColor = true;
            buttonRandomSeed.Click += ButtonRandomSeed_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 102);
            label3.Margin = new System.Windows.Forms.Padding(3);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(237, 15);
            label3.TabIndex = 27;
            label3.Text = "You can type something to generate a seed:";
            // 
            // textBoxSeed
            // 
            textBoxSeed.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxSeed.Location = new System.Drawing.Point(12, 123);
            textBoxSeed.Name = "textBoxSeed";
            textBoxSeed.Size = new System.Drawing.Size(502, 23);
            textBoxSeed.TabIndex = 0;
            textBoxSeed.TextChanged += TextBox1_TextChanged;
            // 
            // buttonClear
            // 
            buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonClear.Location = new System.Drawing.Point(436, 173);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new System.Drawing.Size(78, 27);
            buttonClear.TabIndex = 32;
            buttonClear.Text = "Disable All";
            buttonClear.UseVisualStyleBackColor = true;
            buttonClear.Click += ButtonClear_Click;
            // 
            // propertyGridSettings
            // 
            propertyGridSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGridSettings.BackColor = System.Drawing.SystemColors.Control;
            propertyGridSettings.HelpVisible = false;
            propertyGridSettings.Location = new System.Drawing.Point(11, 206);
            propertyGridSettings.Name = "propertyGridSettings";
            propertyGridSettings.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridSettings.Size = new System.Drawing.Size(503, 339);
            propertyGridSettings.TabIndex = 34;
            propertyGridSettings.ToolbarVisible = false;
            propertyGridSettings.SelectedGridItemChanged += propertyGridAsset_SelectedGridItemChanged;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { settingsFileToolStripMenuItem, optionsToolStripMenuItem, toolStripMenuItemHelp, aboutToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(526, 24);
            menuStrip1.TabIndex = 35;
            menuStrip1.Text = "menuStrip1";
            // 
            // settingsFileToolStripMenuItem
            // 
            settingsFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { loadToolStripMenuItem, saveToolStripMenuItem });
            settingsFileToolStripMenuItem.Name = "settingsFileToolStripMenuItem";
            settingsFileToolStripMenuItem.Size = new System.Drawing.Size(82, 20);
            settingsFileToolStripMenuItem.Text = "Settings File";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += LoadToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { setHeavyModManagerPathToolStripMenuItem, toolStripSeparator1, checkForUpdatesOnStartupToolStripMenuItem, checkForUpdatesOnEditorFilesToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // setHeavyModManagerPathToolStripMenuItem
            // 
            setHeavyModManagerPathToolStripMenuItem.Name = "setHeavyModManagerPathToolStripMenuItem";
            setHeavyModManagerPathToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            setHeavyModManagerPathToolStripMenuItem.Text = "Set Heavy Mod Manager Path";
            setHeavyModManagerPathToolStripMenuItem.Click += setHeavyModManagerPathToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(255, 6);
            // 
            // checkForUpdatesOnStartupToolStripMenuItem
            // 
            checkForUpdatesOnStartupToolStripMenuItem.Name = "checkForUpdatesOnStartupToolStripMenuItem";
            checkForUpdatesOnStartupToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            checkForUpdatesOnStartupToolStripMenuItem.Text = "Check For Updates On Startup";
            checkForUpdatesOnStartupToolStripMenuItem.Click += CheckForUpdatesOnStartupToolStripMenuItem_Click;
            // 
            // checkForUpdatesOnEditorFilesToolStripMenuItem
            // 
            checkForUpdatesOnEditorFilesToolStripMenuItem.Name = "checkForUpdatesOnEditorFilesToolStripMenuItem";
            checkForUpdatesOnEditorFilesToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            checkForUpdatesOnEditorFilesToolStripMenuItem.Text = "Check For Updates On EditorFiles...";
            checkForUpdatesOnEditorFilesToolStripMenuItem.Click += checkForUpdatesOnEditorFilesToolStripMenuItem_Click;
            // 
            // toolStripMenuItemHelp
            // 
            toolStripMenuItemHelp.Name = "toolStripMenuItemHelp";
            toolStripMenuItemHelp.Size = new System.Drawing.Size(44, 20);
            toolStripMenuItemHelp.Text = "Help";
            toolStripMenuItemHelp.Click += toolStripMenuItemHelp_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += ButtonHelp_Click;
            // 
            // labelHmmPath
            // 
            labelHmmPath.AutoSize = true;
            labelHmmPath.Location = new System.Drawing.Point(12, 52);
            labelHmmPath.Margin = new System.Windows.Forms.Padding(3);
            labelHmmPath.Name = "labelHmmPath";
            labelHmmPath.Size = new System.Drawing.Size(184, 15);
            labelHmmPath.TabIndex = 36;
            labelHmmPath.Text = "Heavy Mod Manager path not set";
            // 
            // labelSeed
            // 
            labelSeed.AutoSize = true;
            labelSeed.Location = new System.Drawing.Point(12, 152);
            labelSeed.Margin = new System.Windows.Forms.Padding(3);
            labelSeed.Name = "labelSeed";
            labelSeed.Size = new System.Drawing.Size(38, 15);
            labelSeed.TabIndex = 37;
            labelSeed.Text = "Seed: ";
            // 
            // richTextBoxHelp
            // 
            richTextBoxHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBoxHelp.Location = new System.Drawing.Point(11, 551);
            richTextBoxHelp.Name = "richTextBoxHelp";
            richTextBoxHelp.ReadOnly = true;
            richTextBoxHelp.Size = new System.Drawing.Size(503, 110);
            richTextBoxHelp.TabIndex = 38;
            richTextBoxHelp.Text = "";
            // 
            // buttonReset
            // 
            buttonReset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonReset.Location = new System.Drawing.Point(326, 173);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new System.Drawing.Size(104, 27);
            buttonReset.TabIndex = 39;
            buttonReset.Text = "Default Settings";
            buttonReset.UseVisualStyleBackColor = true;
            buttonReset.Click += buttonReset_Click;
            // 
            // comboBoxGame
            // 
            comboBoxGame.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            comboBoxGame.FormattingEnabled = true;
            comboBoxGame.Items.AddRange(new object[] { "SpongeBob SquarePants: Battle For Bikini Bottom", "Scooby-Doo: Night of 100 Frights", "The SpongeBob SquarePants Movie" });
            comboBoxGame.Location = new System.Drawing.Point(12, 73);
            comboBoxGame.Name = "comboBoxGame";
            comboBoxGame.Size = new System.Drawing.Size(502, 23);
            comboBoxGame.TabIndex = 40;
            comboBoxGame.SelectedIndexChanged += comboBoxGame_SelectedIndexChanged;
            // 
            // radioButtonHmm
            // 
            radioButtonHmm.AutoSize = true;
            radioButtonHmm.Checked = true;
            radioButtonHmm.Location = new System.Drawing.Point(12, 27);
            radioButtonHmm.Name = "radioButtonHmm";
            radioButtonHmm.Size = new System.Drawing.Size(201, 19);
            radioButtonHmm.TabIndex = 41;
            radioButtonHmm.TabStop = true;
            radioButtonHmm.Text = "Create Heavy Mod Manager Mod";
            radioButtonHmm.UseVisualStyleBackColor = true;
            radioButtonHmm.CheckedChanged += radioButtonHmm_CheckedChanged;
            // 
            // radioButtonFiles
            // 
            radioButtonFiles.AutoSize = true;
            radioButtonFiles.Location = new System.Drawing.Point(219, 27);
            radioButtonFiles.Name = "radioButtonFiles";
            radioButtonFiles.Size = new System.Drawing.Size(110, 19);
            radioButtonFiles.TabIndex = 42;
            radioButtonFiles.Text = "Randomize Files";
            radioButtonFiles.UseVisualStyleBackColor = true;
            radioButtonFiles.CheckedChanged += radioButtonFiles_CheckedChanged;
            // 
            // buttonSelectMainFiles
            // 
            buttonSelectMainFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonSelectMainFiles.Enabled = false;
            buttonSelectMainFiles.Location = new System.Drawing.Point(354, 27);
            buttonSelectMainFiles.Name = "buttonSelectMainFiles";
            buttonSelectMainFiles.Size = new System.Drawing.Size(160, 27);
            buttonSelectMainFiles.TabIndex = 43;
            buttonSelectMainFiles.Text = "Select main.dol from files";
            buttonSelectMainFiles.UseVisualStyleBackColor = true;
            buttonSelectMainFiles.Click += buttonSelectMainFiles_Click;
            // 
            // RandomizerMenu
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(526, 709);
            Controls.Add(buttonSelectMainFiles);
            Controls.Add(radioButtonFiles);
            Controls.Add(radioButtonHmm);
            Controls.Add(comboBoxGame);
            Controls.Add(buttonReset);
            Controls.Add(richTextBoxHelp);
            Controls.Add(labelSeed);
            Controls.Add(labelHmmPath);
            Controls.Add(propertyGridSettings);
            Controls.Add(buttonRandomSeed);
            Controls.Add(label3);
            Controls.Add(textBoxSeed);
            Controls.Add(buttonClear);
            Controls.Add(buttonPerform);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "RandomizerMenu";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Randomizer";
            FormClosing += RandomizerMenu_FormClosing;
            Load += RandomizerMenu_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonPerform;
        private System.Windows.Forms.Button buttonRandomSeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSeed;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.PropertyGrid propertyGridSettings;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesOnStartupToolStripMenuItem;
        private System.Windows.Forms.Label labelHmmPath;
        private System.Windows.Forms.Label labelSeed;
        private System.Windows.Forms.RichTextBox richTextBoxHelp;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.ComboBox comboBoxGame;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesOnEditorFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setHeavyModManagerPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.RadioButton radioButtonHmm;
        private System.Windows.Forms.RadioButton radioButtonFiles;
        private System.Windows.Forms.Button buttonSelectMainFiles;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}