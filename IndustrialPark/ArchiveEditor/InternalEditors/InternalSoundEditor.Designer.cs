namespace IndustrialPark
{
    partial class InternalSoundEditor
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
            this.buttonImportJawData = new System.Windows.Forms.Button();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.buttonExportRaw = new System.Windows.Forms.Button();
            this.buttonImportRaw = new System.Windows.Forms.Button();
            this.checkBoxSendToSNDI = new System.Windows.Forms.CheckBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.propertyGridSoundData = new System.Windows.Forms.PropertyGrid();
            this.buttonImportSound = new System.Windows.Forms.Button();
            this.buttonGenerateJawData = new System.Windows.Forms.Button();
            this.groupBoxImport = new System.Windows.Forms.GroupBox();
            this.compressCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.samplerateNumeric = new System.Windows.Forms.NumericUpDown();
            this.forceSampleRateCheckbox = new System.Windows.Forms.CheckBox();
            this.forceMonoCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBoxPS2Looping = new System.Windows.Forms.CheckBox();
            this.groupBoxJaw = new System.Windows.Forms.GroupBox();
            this.numericUpDownJawMultiplier = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxExport = new System.Windows.Forms.GroupBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.groupBoxPlayer = new System.Windows.Forms.GroupBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.importSoundStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.soundLengthLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.soundSizeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxImport.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.samplerateNumeric)).BeginInit();
            this.groupBoxJaw.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJawMultiplier)).BeginInit();
            this.groupBoxExport.SuspendLayout();
            this.groupBoxPlayer.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonImportJawData
            // 
            this.buttonImportJawData.Location = new System.Drawing.Point(169, 39);
            this.buttonImportJawData.Name = "buttonImportJawData";
            this.buttonImportJawData.Size = new System.Drawing.Size(145, 22);
            this.buttonImportJawData.TabIndex = 21;
            this.buttonImportJawData.Text = "Import";
            this.buttonImportJawData.UseVisualStyleBackColor = true;
            this.buttonImportJawData.Click += new System.EventHandler(this.buttonImportJawData_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Location = new System.Drawing.Point(18, 497);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(145, 23);
            this.buttonHelp.TabIndex = 17;
            this.buttonHelp.Text = "Open Wiki Page";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Location = new System.Drawing.Point(181, 497);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(145, 23);
            this.buttonFindCallers.TabIndex = 11;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // buttonExportRaw
            // 
            this.buttonExportRaw.Location = new System.Drawing.Point(6, 44);
            this.buttonExportRaw.Name = "buttonExportRaw";
            this.buttonExportRaw.Size = new System.Drawing.Size(145, 22);
            this.buttonExportRaw.TabIndex = 10;
            this.buttonExportRaw.Text = "Export (raw)";
            this.buttonExportRaw.UseVisualStyleBackColor = true;
            this.buttonExportRaw.Click += new System.EventHandler(this.buttonExportRaw_Click);
            // 
            // buttonImportRaw
            // 
            this.buttonImportRaw.Location = new System.Drawing.Point(163, 16);
            this.buttonImportRaw.Name = "buttonImportRaw";
            this.buttonImportRaw.Size = new System.Drawing.Size(145, 22);
            this.buttonImportRaw.TabIndex = 7;
            this.buttonImportRaw.Text = "Import (raw)";
            this.buttonImportRaw.UseVisualStyleBackColor = true;
            this.buttonImportRaw.Click += new System.EventHandler(this.buttonImportRaw_Click);
            // 
            // checkBoxSendToSNDI
            // 
            this.checkBoxSendToSNDI.AutoSize = true;
            this.checkBoxSendToSNDI.Checked = true;
            this.checkBoxSendToSNDI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSendToSNDI.Location = new System.Drawing.Point(163, 44);
            this.checkBoxSendToSNDI.Name = "checkBoxSendToSNDI";
            this.checkBoxSendToSNDI.Size = new System.Drawing.Size(128, 30);
            this.checkBoxSendToSNDI.TabIndex = 8;
            this.checkBoxSendToSNDI.Text = "Send header to SNDI\r\nwhen importing raw";
            this.checkBoxSendToSNDI.UseVisualStyleBackColor = true;
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(6, 16);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(145, 22);
            this.buttonPlay.TabIndex = 18;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // propertyGridSoundData
            // 
            this.propertyGridSoundData.HelpVisible = false;
            this.propertyGridSoundData.Location = new System.Drawing.Point(12, 12);
            this.propertyGridSoundData.Name = "propertyGridSoundData";
            this.propertyGridSoundData.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridSoundData.Size = new System.Drawing.Size(320, 187);
            this.propertyGridSoundData.TabIndex = 19;
            this.propertyGridSoundData.ToolbarVisible = false;
            this.propertyGridSoundData.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridSoundData_PropertyValueChanged);
            // 
            // buttonImportSound
            // 
            this.buttonImportSound.Location = new System.Drawing.Point(6, 16);
            this.buttonImportSound.Name = "buttonImportSound";
            this.buttonImportSound.Size = new System.Drawing.Size(145, 22);
            this.buttonImportSound.TabIndex = 20;
            this.buttonImportSound.Text = "Import (common file types)";
            this.buttonImportSound.UseVisualStyleBackColor = true;
            this.buttonImportSound.Click += new System.EventHandler(this.buttonImportSound_Click);
            // 
            // buttonGenerateJawData
            // 
            this.buttonGenerateJawData.Location = new System.Drawing.Point(169, 11);
            this.buttonGenerateJawData.Name = "buttonGenerateJawData";
            this.buttonGenerateJawData.Size = new System.Drawing.Size(145, 22);
            this.buttonGenerateJawData.TabIndex = 12;
            this.buttonGenerateJawData.Text = "Generate (experimental)";
            this.buttonGenerateJawData.UseVisualStyleBackColor = true;
            this.buttonGenerateJawData.Click += new System.EventHandler(this.buttonGenerateJawData_Click);
            // 
            // groupBoxImport
            // 
            this.groupBoxImport.Controls.Add(this.compressCheckBox);
            this.groupBoxImport.Controls.Add(this.groupBox1);
            this.groupBoxImport.Controls.Add(this.forceMonoCheckBox);
            this.groupBoxImport.Controls.Add(this.checkBoxPS2Looping);
            this.groupBoxImport.Controls.Add(this.buttonImportRaw);
            this.groupBoxImport.Controls.Add(this.buttonImportSound);
            this.groupBoxImport.Controls.Add(this.checkBoxSendToSNDI);
            this.groupBoxImport.Location = new System.Drawing.Point(12, 205);
            this.groupBoxImport.Name = "groupBoxImport";
            this.groupBoxImport.Size = new System.Drawing.Size(320, 134);
            this.groupBoxImport.TabIndex = 34;
            this.groupBoxImport.TabStop = false;
            this.groupBoxImport.Text = "Import";
            // 
            // compressCheckBox
            // 
            this.compressCheckBox.AutoSize = true;
            this.compressCheckBox.Checked = true;
            this.compressCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.compressCheckBox.Enabled = false;
            this.compressCheckBox.Location = new System.Drawing.Point(6, 68);
            this.compressCheckBox.Name = "compressCheckBox";
            this.compressCheckBox.Size = new System.Drawing.Size(72, 17);
            this.compressCheckBox.TabIndex = 30;
            this.compressCheckBox.Text = "Compress";
            this.compressCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.samplerateNumeric);
            this.groupBox1.Controls.Add(this.forceSampleRateCheckbox);
            this.groupBox1.Location = new System.Drawing.Point(163, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(145, 44);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sample Rate";
            // 
            // samplerateNumeric
            // 
            this.samplerateNumeric.Enabled = false;
            this.samplerateNumeric.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.samplerateNumeric.Location = new System.Drawing.Point(72, 16);
            this.samplerateNumeric.Maximum = new decimal(new int[] {
            48000,
            0,
            0,
            0});
            this.samplerateNumeric.Name = "samplerateNumeric";
            this.samplerateNumeric.Size = new System.Drawing.Size(67, 20);
            this.samplerateNumeric.TabIndex = 28;
            this.samplerateNumeric.ThousandsSeparator = true;
            // 
            // forceSampleRateCheckbox
            // 
            this.forceSampleRateCheckbox.AutoSize = true;
            this.forceSampleRateCheckbox.Location = new System.Drawing.Point(6, 18);
            this.forceSampleRateCheckbox.Name = "forceSampleRateCheckbox";
            this.forceSampleRateCheckbox.Size = new System.Drawing.Size(65, 17);
            this.forceSampleRateCheckbox.TabIndex = 29;
            this.forceSampleRateCheckbox.Text = "Force to";
            this.forceSampleRateCheckbox.UseVisualStyleBackColor = true;
            this.forceSampleRateCheckbox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // forceMonoCheckBox
            // 
            this.forceMonoCheckBox.AutoSize = true;
            this.forceMonoCheckBox.Enabled = false;
            this.forceMonoCheckBox.Location = new System.Drawing.Point(6, 44);
            this.forceMonoCheckBox.Name = "forceMonoCheckBox";
            this.forceMonoCheckBox.Size = new System.Drawing.Size(82, 17);
            this.forceMonoCheckBox.TabIndex = 22;
            this.forceMonoCheckBox.Text = "Force mono\r\n";
            this.forceMonoCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkBoxPS2Looping
            // 
            this.checkBoxPS2Looping.AutoSize = true;
            this.checkBoxPS2Looping.Location = new System.Drawing.Point(6, 88);
            this.checkBoxPS2Looping.Name = "checkBoxPS2Looping";
            this.checkBoxPS2Looping.Size = new System.Drawing.Size(135, 30);
            this.checkBoxPS2Looping.TabIndex = 21;
            this.checkBoxPS2Looping.Text = "PS2: Import as looping \r\nsound";
            this.checkBoxPS2Looping.UseVisualStyleBackColor = true;
            // 
            // groupBoxJaw
            // 
            this.groupBoxJaw.Controls.Add(this.numericUpDownJawMultiplier);
            this.groupBoxJaw.Controls.Add(this.label1);
            this.groupBoxJaw.Controls.Add(this.buttonImportJawData);
            this.groupBoxJaw.Controls.Add(this.buttonGenerateJawData);
            this.groupBoxJaw.Location = new System.Drawing.Point(12, 425);
            this.groupBoxJaw.Name = "groupBoxJaw";
            this.groupBoxJaw.Size = new System.Drawing.Size(320, 66);
            this.groupBoxJaw.TabIndex = 34;
            this.groupBoxJaw.TabStop = false;
            this.groupBoxJaw.Text = "Jaw Data";
            // 
            // numericUpDownJawMultiplier
            // 
            this.numericUpDownJawMultiplier.Location = new System.Drawing.Point(60, 14);
            this.numericUpDownJawMultiplier.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownJawMultiplier.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownJawMultiplier.Name = "numericUpDownJawMultiplier";
            this.numericUpDownJawMultiplier.Size = new System.Drawing.Size(91, 20);
            this.numericUpDownJawMultiplier.TabIndex = 35;
            this.numericUpDownJawMultiplier.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Multiplier:";
            // 
            // groupBoxExport
            // 
            this.groupBoxExport.Controls.Add(this.buttonExportRaw);
            this.groupBoxExport.Controls.Add(this.buttonExport);
            this.groupBoxExport.Location = new System.Drawing.Point(12, 345);
            this.groupBoxExport.Name = "groupBoxExport";
            this.groupBoxExport.Size = new System.Drawing.Size(157, 74);
            this.groupBoxExport.TabIndex = 36;
            this.groupBoxExport.TabStop = false;
            this.groupBoxExport.Text = "Export";
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(6, 16);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(145, 22);
            this.buttonExport.TabIndex = 20;
            this.buttonExport.Text = "Export (WAV)";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(6, 43);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(145, 23);
            this.buttonStop.TabIndex = 37;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // groupBoxPlayer
            // 
            this.groupBoxPlayer.Controls.Add(this.buttonPlay);
            this.groupBoxPlayer.Controls.Add(this.buttonStop);
            this.groupBoxPlayer.Location = new System.Drawing.Point(175, 345);
            this.groupBoxPlayer.Name = "groupBoxPlayer";
            this.groupBoxPlayer.Size = new System.Drawing.Size(157, 74);
            this.groupBoxPlayer.TabIndex = 38;
            this.groupBoxPlayer.TabStop = false;
            this.groupBoxPlayer.Text = "Player";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importSoundStatusLabel,
            this.soundLengthLabel,
            this.soundSizeLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 526);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(344, 22);
            this.statusStrip1.TabIndex = 39;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // importSoundStatusLabel
            // 
            this.importSoundStatusLabel.Name = "importSoundStatusLabel";
            this.importSoundStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // soundLengthLabel
            // 
            this.soundLengthLabel.Name = "soundLengthLabel";
            this.soundLengthLabel.Size = new System.Drawing.Size(0, 17);
            this.soundLengthLabel.ToolTipText = "MM:SS.MS";
            // 
            // soundSizeLabel
            // 
            this.soundSizeLabel.Name = "soundSizeLabel";
            this.soundSizeLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // InternalSoundEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 548);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBoxPlayer);
            this.Controls.Add(this.groupBoxExport);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonFindCallers);
            this.Controls.Add(this.groupBoxJaw);
            this.Controls.Add(this.propertyGridSoundData);
            this.Controls.Add(this.groupBoxImport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "InternalSoundEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalAssetEditor_FormClosing);
            this.groupBoxImport.ResumeLayout(false);
            this.groupBoxImport.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.samplerateNumeric)).EndInit();
            this.groupBoxJaw.ResumeLayout(false);
            this.groupBoxJaw.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJawMultiplier)).EndInit();
            this.groupBoxExport.ResumeLayout(false);
            this.groupBoxPlayer.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonImportRaw;
        private System.Windows.Forms.CheckBox checkBoxSendToSNDI;
        private System.Windows.Forms.Button buttonExportRaw;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonGenerateJawData;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.PropertyGrid propertyGridSoundData;
        private System.Windows.Forms.Button buttonImportSound;
        private System.Windows.Forms.Button buttonImportJawData;
        private System.Windows.Forms.GroupBox groupBoxImport;
        private System.Windows.Forms.GroupBox groupBoxJaw;
        private System.Windows.Forms.NumericUpDown numericUpDownJawMultiplier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxExport;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.CheckBox checkBoxPS2Looping;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.CheckBox forceMonoCheckBox;
        private System.Windows.Forms.GroupBox groupBoxPlayer;
        private System.Windows.Forms.NumericUpDown samplerateNumeric;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox forceSampleRateCheckbox;
        private System.Windows.Forms.CheckBox compressCheckBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel soundLengthLabel;
        private System.Windows.Forms.ToolStripStatusLabel soundSizeLabel;
        private System.Windows.Forms.ToolStripStatusLabel importSoundStatusLabel;
    }
}