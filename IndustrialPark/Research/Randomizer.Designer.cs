namespace IndustrialPark
{
    partial class Randomizer
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
            this.buttonChooseRoot = new System.Windows.Forms.Button();
            this.labelRootDir = new System.Windows.Forms.Label();
            this.buttonPerform = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonChooseFile = new System.Windows.Forms.Button();
            this.checkedListBoxMethods = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericPlatSpeedMax = new System.Windows.Forms.NumericUpDown();
            this.numericPlatSpeedMin = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericPlatTimeMax = new System.Windows.Forms.NumericUpDown();
            this.numericPlatTimeMin = new System.Windows.Forms.NumericUpDown();
            this.richTextBoxSkip = new System.Windows.Forms.RichTextBox();
            this.labelSkip = new System.Windows.Forms.Label();
            this.checkedListBoxNotRecommended = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonRandomSeed = new System.Windows.Forms.Button();
            this.labelSeed = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSeed = new System.Windows.Forms.TextBox();
            this.buttonSaveJson = new System.Windows.Forms.Button();
            this.buttonLoadJson = new System.Windows.Forms.Button();
            this.labelRandoJson = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkedListBoxSBINI = new System.Windows.Forms.CheckedListBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMin)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMin)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonChooseRoot
            // 
            this.buttonChooseRoot.Location = new System.Drawing.Point(12, 82);
            this.buttonChooseRoot.Name = "buttonChooseRoot";
            this.buttonChooseRoot.Size = new System.Drawing.Size(198, 23);
            this.buttonChooseRoot.TabIndex = 0;
            this.buttonChooseRoot.Text = "Choose Root Directory";
            this.buttonChooseRoot.UseVisualStyleBackColor = true;
            this.buttonChooseRoot.Click += new System.EventHandler(this.buttonChooseRoot_Click);
            // 
            // labelRootDir
            // 
            this.labelRootDir.AutoSize = true;
            this.labelRootDir.Location = new System.Drawing.Point(12, 66);
            this.labelRootDir.Name = "labelRootDir";
            this.labelRootDir.Size = new System.Drawing.Size(107, 13);
            this.labelRootDir.TabIndex = 1;
            this.labelRootDir.Text = "Root Directory: None";
            // 
            // buttonPerform
            // 
            this.buttonPerform.Enabled = false;
            this.buttonPerform.Location = new System.Drawing.Point(624, 371);
            this.buttonPerform.Name = "buttonPerform";
            this.buttonPerform.Size = new System.Drawing.Size(191, 23);
            this.buttonPerform.TabIndex = 8;
            this.buttonPerform.Text = "Perform";
            this.buttonPerform.UseVisualStyleBackColor = true;
            this.buttonPerform.Click += new System.EventHandler(this.buttonPerform_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 371);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(606, 23);
            this.progressBar1.TabIndex = 10;
            // 
            // buttonChooseFile
            // 
            this.buttonChooseFile.Location = new System.Drawing.Point(216, 82);
            this.buttonChooseFile.Name = "buttonChooseFile";
            this.buttonChooseFile.Size = new System.Drawing.Size(198, 23);
            this.buttonChooseFile.TabIndex = 12;
            this.buttonChooseFile.Text = "Choose Single File";
            this.buttonChooseFile.UseVisualStyleBackColor = true;
            this.buttonChooseFile.Click += new System.EventHandler(this.ButtonChooseFile_Click);
            // 
            // checkedListBoxMethods
            // 
            this.checkedListBoxMethods.FormattingEnabled = true;
            this.checkedListBoxMethods.Location = new System.Drawing.Point(12, 111);
            this.checkedListBoxMethods.Name = "checkedListBoxMethods";
            this.checkedListBoxMethods.Size = new System.Drawing.Size(170, 199);
            this.checkedListBoxMethods.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericPlatSpeedMax);
            this.groupBox1.Controls.Add(this.numericPlatSpeedMin);
            this.groupBox1.Location = new System.Drawing.Point(12, 316);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(402, 49);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PLAT Speed Multiplier (Min, Max)";
            // 
            // numericPlatSpeedMax
            // 
            this.numericPlatSpeedMax.DecimalPlaces = 6;
            this.numericPlatSpeedMax.Location = new System.Drawing.Point(204, 19);
            this.numericPlatSpeedMax.Name = "numericPlatSpeedMax";
            this.numericPlatSpeedMax.Size = new System.Drawing.Size(192, 20);
            this.numericPlatSpeedMax.TabIndex = 16;
            this.numericPlatSpeedMax.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // numericPlatSpeedMin
            // 
            this.numericPlatSpeedMin.DecimalPlaces = 6;
            this.numericPlatSpeedMin.Location = new System.Drawing.Point(6, 19);
            this.numericPlatSpeedMin.Name = "numericPlatSpeedMin";
            this.numericPlatSpeedMin.Size = new System.Drawing.Size(192, 20);
            this.numericPlatSpeedMin.TabIndex = 15;
            this.numericPlatSpeedMin.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericPlatTimeMax);
            this.groupBox2.Controls.Add(this.numericPlatTimeMin);
            this.groupBox2.Location = new System.Drawing.Point(420, 316);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(402, 49);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PLAT Time Multiplier (Min, Max)";
            // 
            // numericPlatTimeMax
            // 
            this.numericPlatTimeMax.DecimalPlaces = 6;
            this.numericPlatTimeMax.Location = new System.Drawing.Point(204, 19);
            this.numericPlatTimeMax.Name = "numericPlatTimeMax";
            this.numericPlatTimeMax.Size = new System.Drawing.Size(191, 20);
            this.numericPlatTimeMax.TabIndex = 16;
            this.numericPlatTimeMax.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericPlatTimeMin
            // 
            this.numericPlatTimeMin.DecimalPlaces = 6;
            this.numericPlatTimeMin.Location = new System.Drawing.Point(6, 19);
            this.numericPlatTimeMin.Name = "numericPlatTimeMin";
            this.numericPlatTimeMin.Size = new System.Drawing.Size(192, 20);
            this.numericPlatTimeMin.TabIndex = 15;
            this.numericPlatTimeMin.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // richTextBoxSkip
            // 
            this.richTextBoxSkip.Location = new System.Drawing.Point(364, 126);
            this.richTextBoxSkip.Name = "richTextBoxSkip";
            this.richTextBoxSkip.Size = new System.Drawing.Size(135, 184);
            this.richTextBoxSkip.TabIndex = 18;
            this.richTextBoxSkip.Text = "font\nboot\nplat\nmn\nsp\npl\nhb00\nhb10\ndb05\nb301\ns006";
            // 
            // labelSkip
            // 
            this.labelSkip.AutoSize = true;
            this.labelSkip.Location = new System.Drawing.Point(361, 107);
            this.labelSkip.Name = "labelSkip";
            this.labelSkip.Size = new System.Drawing.Size(125, 13);
            this.labelSkip.TabIndex = 19;
            this.labelSkip.Text = "Patterns and files to skip:";
            // 
            // checkedListBoxNotRecommended
            // 
            this.checkedListBoxNotRecommended.FormattingEnabled = true;
            this.checkedListBoxNotRecommended.Location = new System.Drawing.Point(188, 126);
            this.checkedListBoxNotRecommended.Name = "checkedListBoxNotRecommended";
            this.checkedListBoxNotRecommended.Size = new System.Drawing.Size(170, 184);
            this.checkedListBoxNotRecommended.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Not recommended:";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(505, 126);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(134, 184);
            this.richTextBox2.TabIndex = 23;
            this.richTextBox2.Text = "pg";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(500, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Skip for Warps and Level Files:";
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(660, 103);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(155, 21);
            this.buttonHelp.TabIndex = 25;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.ButtonHelp_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonRandomSeed);
            this.groupBox3.Controls.Add(this.labelSeed);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.textBoxSeed);
            this.groupBox3.Location = new System.Drawing.Point(420, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(395, 88);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Seed";
            // 
            // buttonRandomSeed
            // 
            this.buttonRandomSeed.Location = new System.Drawing.Point(330, 45);
            this.buttonRandomSeed.Name = "buttonRandomSeed";
            this.buttonRandomSeed.Size = new System.Drawing.Size(59, 20);
            this.buttonRandomSeed.TabIndex = 29;
            this.buttonRandomSeed.Text = "Random";
            this.buttonRandomSeed.UseVisualStyleBackColor = true;
            this.buttonRandomSeed.Click += new System.EventHandler(this.ButtonRandomSeed_Click);
            // 
            // labelSeed
            // 
            this.labelSeed.AutoSize = true;
            this.labelSeed.Location = new System.Drawing.Point(6, 68);
            this.labelSeed.Name = "labelSeed";
            this.labelSeed.Size = new System.Drawing.Size(35, 13);
            this.labelSeed.TabIndex = 28;
            this.labelSeed.Text = "Seed:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(348, 26);
            this.label3.TabIndex = 27;
            this.label3.Text = "Type something to generate a seed or click the button for a random one.\r\nA seed i" +
    "s a number between 0 and 4.294.967.295.";
            // 
            // textBoxSeed
            // 
            this.textBoxSeed.Location = new System.Drawing.Point(6, 45);
            this.textBoxSeed.Name = "textBoxSeed";
            this.textBoxSeed.Size = new System.Drawing.Size(318, 20);
            this.textBoxSeed.TabIndex = 0;
            this.textBoxSeed.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // buttonSaveJson
            // 
            this.buttonSaveJson.Location = new System.Drawing.Point(216, 28);
            this.buttonSaveJson.Name = "buttonSaveJson";
            this.buttonSaveJson.Size = new System.Drawing.Size(198, 23);
            this.buttonSaveJson.TabIndex = 28;
            this.buttonSaveJson.Text = "Save Settings";
            this.buttonSaveJson.UseVisualStyleBackColor = true;
            this.buttonSaveJson.Click += new System.EventHandler(this.ButtonSaveJson_Click);
            // 
            // buttonLoadJson
            // 
            this.buttonLoadJson.Location = new System.Drawing.Point(12, 28);
            this.buttonLoadJson.Name = "buttonLoadJson";
            this.buttonLoadJson.Size = new System.Drawing.Size(198, 23);
            this.buttonLoadJson.TabIndex = 27;
            this.buttonLoadJson.Text = "Load Settings";
            this.buttonLoadJson.UseVisualStyleBackColor = true;
            this.buttonLoadJson.Click += new System.EventHandler(this.ButtonLoadJson_Click);
            // 
            // labelRandoJson
            // 
            this.labelRandoJson.AutoSize = true;
            this.labelRandoJson.Location = new System.Drawing.Point(12, 9);
            this.labelRandoJson.Name = "labelRandoJson";
            this.labelRandoJson.Size = new System.Drawing.Size(0, 13);
            this.labelRandoJson.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(642, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "sb.ini mods:";
            // 
            // checkedListBoxSBINI
            // 
            this.checkedListBoxSBINI.FormattingEnabled = true;
            this.checkedListBoxSBINI.Location = new System.Drawing.Point(645, 141);
            this.checkedListBoxSBINI.Name = "checkedListBoxSBINI";
            this.checkedListBoxSBINI.Size = new System.Drawing.Size(170, 169);
            this.checkedListBoxSBINI.TabIndex = 31;
            // 
            // Randomizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 402);
            this.Controls.Add(this.checkedListBoxSBINI);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelRandoJson);
            this.Controls.Add(this.buttonSaveJson);
            this.Controls.Add(this.buttonLoadJson);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkedListBoxNotRecommended);
            this.Controls.Add(this.labelSkip);
            this.Controls.Add(this.richTextBoxSkip);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkedListBoxMethods);
            this.Controls.Add(this.buttonChooseFile);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonPerform);
            this.Controls.Add(this.labelRootDir);
            this.Controls.Add(this.buttonChooseRoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Randomizer";
            this.ShowIcon = false;
            this.Text = "Randomizer";
            this.Load += new System.EventHandler(this.Randomizer_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMin)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMin)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChooseRoot;
        private System.Windows.Forms.Label labelRootDir;
        private System.Windows.Forms.Button buttonPerform;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonChooseFile;
        private System.Windows.Forms.CheckedListBox checkedListBoxMethods;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericPlatSpeedMax;
        private System.Windows.Forms.NumericUpDown numericPlatSpeedMin;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericPlatTimeMax;
        private System.Windows.Forms.NumericUpDown numericPlatTimeMin;
        private System.Windows.Forms.RichTextBox richTextBoxSkip;
        private System.Windows.Forms.Label labelSkip;
        private System.Windows.Forms.CheckedListBox checkedListBoxNotRecommended;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonRandomSeed;
        private System.Windows.Forms.Label labelSeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSeed;
        private System.Windows.Forms.Button buttonSaveJson;
        private System.Windows.Forms.Button buttonLoadJson;
        private System.Windows.Forms.Label labelRandoJson;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox checkedListBoxSBINI;
    }
}