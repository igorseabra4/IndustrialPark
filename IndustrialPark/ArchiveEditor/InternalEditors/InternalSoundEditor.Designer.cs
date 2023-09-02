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
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonImportRaw = new System.Windows.Forms.Button();
            this.checkBoxSendToSNDI = new System.Windows.Forms.CheckBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.propertyGridSoundData = new System.Windows.Forms.PropertyGrid();
            this.buttonImportSound = new System.Windows.Forms.Button();
            this.buttonGenerateJawData = new System.Windows.Forms.Button();
            this.groupBoxImportRaw = new System.Windows.Forms.GroupBox();
            this.groupBoxImportSound = new System.Windows.Forms.GroupBox();
            this.groupBoxJaw = new System.Windows.Forms.GroupBox();
            this.numericUpDownJawMultiplier = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxExport = new System.Windows.Forms.GroupBox();
            this.groupBoxImportRaw.SuspendLayout();
            this.groupBoxImportSound.SuspendLayout();
            this.groupBoxJaw.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJawMultiplier)).BeginInit();
            this.groupBoxExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonImportJawData
            // 
            this.buttonImportJawData.Location = new System.Drawing.Point(6, 68);
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
            this.buttonHelp.Location = new System.Drawing.Point(18, 406);
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
            this.buttonFindCallers.Location = new System.Drawing.Point(181, 406);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(145, 23);
            this.buttonFindCallers.TabIndex = 11;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.Location = new System.Drawing.Point(6, 16);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(145, 22);
            this.buttonExport.TabIndex = 10;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonImportRaw
            // 
            this.buttonImportRaw.Location = new System.Drawing.Point(6, 42);
            this.buttonImportRaw.Name = "buttonImportRaw";
            this.buttonImportRaw.Size = new System.Drawing.Size(145, 22);
            this.buttonImportRaw.TabIndex = 7;
            this.buttonImportRaw.Text = "Import";
            this.buttonImportRaw.UseVisualStyleBackColor = true;
            this.buttonImportRaw.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBoxSendToSNDI
            // 
            this.checkBoxSendToSNDI.AutoSize = true;
            this.checkBoxSendToSNDI.Checked = true;
            this.checkBoxSendToSNDI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSendToSNDI.Location = new System.Drawing.Point(6, 19);
            this.checkBoxSendToSNDI.Name = "checkBoxSendToSNDI";
            this.checkBoxSendToSNDI.Size = new System.Drawing.Size(128, 17);
            this.checkBoxSendToSNDI.TabIndex = 8;
            this.checkBoxSendToSNDI.Text = "Send header to SNDI";
            this.checkBoxSendToSNDI.UseVisualStyleBackColor = true;
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(18, 378);
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
            this.propertyGridSoundData.Size = new System.Drawing.Size(320, 232);
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
            this.buttonImportSound.Text = "Import";
            this.buttonImportSound.UseVisualStyleBackColor = true;
            this.buttonImportSound.Click += new System.EventHandler(this.buttonImportSound_Click);
            // 
            // buttonGenerateJawData
            // 
            this.buttonGenerateJawData.Location = new System.Drawing.Point(6, 40);
            this.buttonGenerateJawData.Name = "buttonGenerateJawData";
            this.buttonGenerateJawData.Size = new System.Drawing.Size(145, 22);
            this.buttonGenerateJawData.TabIndex = 12;
            this.buttonGenerateJawData.Text = "Generate";
            this.buttonGenerateJawData.UseVisualStyleBackColor = true;
            this.buttonGenerateJawData.Click += new System.EventHandler(this.buttonGenerateJawData_Click);
            // 
            // groupBoxImportRaw
            // 
            this.groupBoxImportRaw.Controls.Add(this.checkBoxSendToSNDI);
            this.groupBoxImportRaw.Controls.Add(this.buttonImportRaw);
            this.groupBoxImportRaw.Location = new System.Drawing.Point(12, 302);
            this.groupBoxImportRaw.Name = "groupBoxImportRaw";
            this.groupBoxImportRaw.Size = new System.Drawing.Size(157, 70);
            this.groupBoxImportRaw.TabIndex = 33;
            this.groupBoxImportRaw.TabStop = false;
            this.groupBoxImportRaw.Text = "Import Raw Sound";
            // 
            // groupBoxImportSound
            // 
            this.groupBoxImportSound.Controls.Add(this.buttonImportSound);
            this.groupBoxImportSound.Location = new System.Drawing.Point(12, 250);
            this.groupBoxImportSound.Name = "groupBoxImportSound";
            this.groupBoxImportSound.Size = new System.Drawing.Size(157, 46);
            this.groupBoxImportSound.TabIndex = 34;
            this.groupBoxImportSound.TabStop = false;
            this.groupBoxImportSound.Text = "Import Sound";
            // 
            // groupBoxJaw
            // 
            this.groupBoxJaw.Controls.Add(this.numericUpDownJawMultiplier);
            this.groupBoxJaw.Controls.Add(this.label1);
            this.groupBoxJaw.Controls.Add(this.buttonImportJawData);
            this.groupBoxJaw.Controls.Add(this.buttonGenerateJawData);
            this.groupBoxJaw.Location = new System.Drawing.Point(175, 302);
            this.groupBoxJaw.Name = "groupBoxJaw";
            this.groupBoxJaw.Size = new System.Drawing.Size(157, 98);
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
            this.groupBoxExport.Controls.Add(this.buttonExport);
            this.groupBoxExport.Location = new System.Drawing.Point(175, 250);
            this.groupBoxExport.Name = "groupBoxExport";
            this.groupBoxExport.Size = new System.Drawing.Size(157, 46);
            this.groupBoxExport.TabIndex = 35;
            this.groupBoxExport.TabStop = false;
            this.groupBoxExport.Text = "Export Raw Sound";
            // 
            // InternalSoundEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 441);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.groupBoxExport);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonFindCallers);
            this.Controls.Add(this.groupBoxJaw);
            this.Controls.Add(this.propertyGridSoundData);
            this.Controls.Add(this.groupBoxImportSound);
            this.Controls.Add(this.groupBoxImportRaw);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "InternalSoundEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalAssetEditor_FormClosing);
            this.groupBoxImportRaw.ResumeLayout(false);
            this.groupBoxImportRaw.PerformLayout();
            this.groupBoxImportSound.ResumeLayout(false);
            this.groupBoxJaw.ResumeLayout(false);
            this.groupBoxJaw.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJawMultiplier)).EndInit();
            this.groupBoxExport.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonImportRaw;
        private System.Windows.Forms.CheckBox checkBoxSendToSNDI;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonGenerateJawData;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.PropertyGrid propertyGridSoundData;
        private System.Windows.Forms.Button buttonImportSound;
        private System.Windows.Forms.Button buttonImportJawData;
        private System.Windows.Forms.GroupBox groupBoxImportRaw;
        private System.Windows.Forms.GroupBox groupBoxImportSound;
        private System.Windows.Forms.GroupBox groupBoxJaw;
        private System.Windows.Forms.NumericUpDown numericUpDownJawMultiplier;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxExport;
    }
}