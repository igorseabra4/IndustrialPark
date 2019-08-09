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
            this.buttonChooseRoot = new System.Windows.Forms.Button();
            this.labelRootDir = new System.Windows.Forms.Label();
            this.buttonPerform = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonChooseFile = new System.Windows.Forms.Button();
            this.checkedListBoxMethods = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxNotRecommended = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonRandomSeed = new System.Windows.Forms.Button();
            this.labelSeed = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSeed = new System.Windows.Forms.TextBox();
            this.buttonSaveJson = new System.Windows.Forms.Button();
            this.buttonLoadJson = new System.Windows.Forms.Button();
            this.labelRandoJson = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.checkedListBoxSBINI = new System.Windows.Forms.CheckedListBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonProbs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonChooseRoot
            // 
            this.buttonChooseRoot.Location = new System.Drawing.Point(12, 77);
            this.buttonChooseRoot.Name = "buttonChooseRoot";
            this.buttonChooseRoot.Size = new System.Drawing.Size(170, 23);
            this.buttonChooseRoot.TabIndex = 0;
            this.buttonChooseRoot.Text = "Choose Root Directory";
            this.buttonChooseRoot.UseVisualStyleBackColor = true;
            this.buttonChooseRoot.Click += new System.EventHandler(this.buttonChooseRoot_Click);
            // 
            // labelRootDir
            // 
            this.labelRootDir.AutoSize = true;
            this.labelRootDir.Location = new System.Drawing.Point(12, 61);
            this.labelRootDir.Name = "labelRootDir";
            this.labelRootDir.Size = new System.Drawing.Size(107, 13);
            this.labelRootDir.TabIndex = 1;
            this.labelRootDir.Text = "Root Directory: None";
            // 
            // buttonPerform
            // 
            this.buttonPerform.Enabled = false;
            this.buttonPerform.Location = new System.Drawing.Point(364, 311);
            this.buttonPerform.Name = "buttonPerform";
            this.buttonPerform.Size = new System.Drawing.Size(170, 23);
            this.buttonPerform.TabIndex = 8;
            this.buttonPerform.Text = "Perform";
            this.buttonPerform.UseVisualStyleBackColor = true;
            this.buttonPerform.Click += new System.EventHandler(this.buttonPerform_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 340);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(522, 23);
            this.progressBar1.TabIndex = 10;
            // 
            // buttonChooseFile
            // 
            this.buttonChooseFile.Location = new System.Drawing.Point(188, 77);
            this.buttonChooseFile.Name = "buttonChooseFile";
            this.buttonChooseFile.Size = new System.Drawing.Size(170, 23);
            this.buttonChooseFile.TabIndex = 12;
            this.buttonChooseFile.Text = "Choose Single File";
            this.buttonChooseFile.UseVisualStyleBackColor = true;
            this.buttonChooseFile.Click += new System.EventHandler(this.ButtonChooseFile_Click);
            // 
            // checkedListBoxMethods
            // 
            this.checkedListBoxMethods.FormattingEnabled = true;
            this.checkedListBoxMethods.Location = new System.Drawing.Point(12, 106);
            this.checkedListBoxMethods.Name = "checkedListBoxMethods";
            this.checkedListBoxMethods.Size = new System.Drawing.Size(170, 199);
            this.checkedListBoxMethods.TabIndex = 13;
            this.checkedListBoxMethods.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxMethods_ItemCheck);
            // 
            // checkedListBoxNotRecommended
            // 
            this.checkedListBoxNotRecommended.FormattingEnabled = true;
            this.checkedListBoxNotRecommended.Location = new System.Drawing.Point(188, 121);
            this.checkedListBoxNotRecommended.Name = "checkedListBoxNotRecommended";
            this.checkedListBoxNotRecommended.Size = new System.Drawing.Size(170, 184);
            this.checkedListBoxNotRecommended.TabIndex = 21;
            this.checkedListBoxNotRecommended.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxNotRecommended_ItemCheck);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Not recommended:";
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(452, 77);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(82, 23);
            this.buttonHelp.TabIndex = 25;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.ButtonHelp_Click);
            // 
            // buttonRandomSeed
            // 
            this.buttonRandomSeed.Location = new System.Drawing.Point(364, 77);
            this.buttonRandomSeed.Name = "buttonRandomSeed";
            this.buttonRandomSeed.Size = new System.Drawing.Size(82, 23);
            this.buttonRandomSeed.TabIndex = 29;
            this.buttonRandomSeed.Text = "Random";
            this.buttonRandomSeed.UseVisualStyleBackColor = true;
            this.buttonRandomSeed.Click += new System.EventHandler(this.ButtonRandomSeed_Click);
            // 
            // labelSeed
            // 
            this.labelSeed.AutoSize = true;
            this.labelSeed.Location = new System.Drawing.Point(364, 61);
            this.labelSeed.Name = "labelSeed";
            this.labelSeed.Size = new System.Drawing.Size(35, 13);
            this.labelSeed.TabIndex = 28;
            this.labelSeed.Text = "Seed:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(364, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 26);
            this.label3.TabIndex = 27;
            this.label3.Text = "Type something to generate a seed\r\nor click the button for a random one.";
            // 
            // textBoxSeed
            // 
            this.textBoxSeed.Location = new System.Drawing.Point(364, 38);
            this.textBoxSeed.Name = "textBoxSeed";
            this.textBoxSeed.Size = new System.Drawing.Size(170, 20);
            this.textBoxSeed.TabIndex = 0;
            this.textBoxSeed.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // buttonSaveJson
            // 
            this.buttonSaveJson.Location = new System.Drawing.Point(188, 25);
            this.buttonSaveJson.Name = "buttonSaveJson";
            this.buttonSaveJson.Size = new System.Drawing.Size(170, 23);
            this.buttonSaveJson.TabIndex = 28;
            this.buttonSaveJson.Text = "Save Settings";
            this.buttonSaveJson.UseVisualStyleBackColor = true;
            this.buttonSaveJson.Click += new System.EventHandler(this.ButtonSaveJson_Click);
            // 
            // buttonLoadJson
            // 
            this.buttonLoadJson.Location = new System.Drawing.Point(12, 25);
            this.buttonLoadJson.Name = "buttonLoadJson";
            this.buttonLoadJson.Size = new System.Drawing.Size(170, 23);
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
            this.labelRandoJson.Size = new System.Drawing.Size(72, 13);
            this.labelRandoJson.TabIndex = 29;
            this.labelRandoJson.Text = "No file loaded";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(364, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 30;
            this.label4.Text = "INI mods:";
            // 
            // checkedListBoxSBINI
            // 
            this.checkedListBoxSBINI.FormattingEnabled = true;
            this.checkedListBoxSBINI.Location = new System.Drawing.Point(364, 121);
            this.checkedListBoxSBINI.Name = "checkedListBoxSBINI";
            this.checkedListBoxSBINI.Size = new System.Drawing.Size(170, 184);
            this.checkedListBoxSBINI.TabIndex = 31;
            this.checkedListBoxSBINI.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxSBINI_ItemCheck);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(188, 311);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(170, 23);
            this.buttonClear.TabIndex = 32;
            this.buttonClear.Text = "Clear Checkboxes";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.ButtonClear_Click);
            // 
            // buttonProbs
            // 
            this.buttonProbs.Location = new System.Drawing.Point(12, 311);
            this.buttonProbs.Name = "buttonProbs";
            this.buttonProbs.Size = new System.Drawing.Size(170, 23);
            this.buttonProbs.TabIndex = 33;
            this.buttonProbs.Text = "Other Settings";
            this.buttonProbs.UseVisualStyleBackColor = true;
            this.buttonProbs.Click += new System.EventHandler(this.ButtonProbs_Click);
            // 
            // RandomizerMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 370);
            this.Controls.Add(this.labelSeed);
            this.Controls.Add(this.buttonRandomSeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonProbs);
            this.Controls.Add(this.textBoxSeed);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.checkedListBoxSBINI);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelRandoJson);
            this.Controls.Add(this.buttonSaveJson);
            this.Controls.Add(this.buttonLoadJson);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkedListBoxNotRecommended);
            this.Controls.Add(this.checkedListBoxMethods);
            this.Controls.Add(this.buttonChooseFile);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonPerform);
            this.Controls.Add(this.labelRootDir);
            this.Controls.Add(this.buttonChooseRoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RandomizerMenu";
            this.ShowIcon = false;
            this.Text = "Randomizer";
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
        private System.Windows.Forms.CheckedListBox checkedListBoxNotRecommended;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonRandomSeed;
        private System.Windows.Forms.Label labelSeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSeed;
        private System.Windows.Forms.Button buttonSaveJson;
        private System.Windows.Forms.Button buttonLoadJson;
        private System.Windows.Forms.Label labelRandoJson;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckedListBox checkedListBoxSBINI;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonProbs;
    }
}