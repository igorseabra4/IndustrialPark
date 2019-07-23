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
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericPlatSpeedMax = new System.Windows.Forms.NumericUpDown();
            this.numericPlatSpeedMin = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericPlatTimeMax = new System.Windows.Forms.NumericUpDown();
            this.numericPlatTimeMin = new System.Windows.Forms.NumericUpDown();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMin)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMin)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonChooseRoot
            // 
            this.buttonChooseRoot.Location = new System.Drawing.Point(12, 25);
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
            this.labelRootDir.Location = new System.Drawing.Point(12, 9);
            this.labelRootDir.Name = "labelRootDir";
            this.labelRootDir.Size = new System.Drawing.Size(107, 13);
            this.labelRootDir.TabIndex = 1;
            this.labelRootDir.Text = "Root Directory: None";
            // 
            // buttonPerform
            // 
            this.buttonPerform.Enabled = false;
            this.buttonPerform.Location = new System.Drawing.Point(12, 540);
            this.buttonPerform.Name = "buttonPerform";
            this.buttonPerform.Size = new System.Drawing.Size(402, 23);
            this.buttonPerform.TabIndex = 8;
            this.buttonPerform.Text = "Perform";
            this.buttonPerform.UseVisualStyleBackColor = true;
            this.buttonPerform.Click += new System.EventHandler(this.buttonPerform_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 569);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(402, 23);
            this.progressBar1.TabIndex = 10;
            // 
            // buttonChooseFile
            // 
            this.buttonChooseFile.Location = new System.Drawing.Point(216, 25);
            this.buttonChooseFile.Name = "buttonChooseFile";
            this.buttonChooseFile.Size = new System.Drawing.Size(198, 23);
            this.buttonChooseFile.TabIndex = 12;
            this.buttonChooseFile.Text = "Choose Single File";
            this.buttonChooseFile.UseVisualStyleBackColor = true;
            this.buttonChooseFile.Click += new System.EventHandler(this.ButtonChooseFile_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 54);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(402, 214);
            this.checkedListBox1.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericPlatSpeedMax);
            this.groupBox1.Controls.Add(this.numericPlatSpeedMin);
            this.groupBox1.Location = new System.Drawing.Point(12, 460);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(198, 74);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PLAT Speed Multiplier (Min, Max)";
            // 
            // numericPlatSpeedMax
            // 
            this.numericPlatSpeedMax.DecimalPlaces = 6;
            this.numericPlatSpeedMax.Location = new System.Drawing.Point(6, 45);
            this.numericPlatSpeedMax.Name = "numericPlatSpeedMax";
            this.numericPlatSpeedMax.Size = new System.Drawing.Size(186, 20);
            this.numericPlatSpeedMax.TabIndex = 16;
            this.numericPlatSpeedMax.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // numericPlatSpeedMin
            // 
            this.numericPlatSpeedMin.DecimalPlaces = 6;
            this.numericPlatSpeedMin.Location = new System.Drawing.Point(6, 19);
            this.numericPlatSpeedMin.Name = "numericPlatSpeedMin";
            this.numericPlatSpeedMin.Size = new System.Drawing.Size(186, 20);
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
            this.groupBox2.Location = new System.Drawing.Point(216, 460);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(198, 74);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PLAT Time Multiplier (Min, Max)";
            // 
            // numericPlatTimeMax
            // 
            this.numericPlatTimeMax.DecimalPlaces = 6;
            this.numericPlatTimeMax.Location = new System.Drawing.Point(6, 45);
            this.numericPlatTimeMax.Name = "numericPlatTimeMax";
            this.numericPlatTimeMax.Size = new System.Drawing.Size(186, 20);
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
            this.numericPlatTimeMin.Size = new System.Drawing.Size(186, 20);
            this.numericPlatTimeMin.TabIndex = 15;
            this.numericPlatTimeMin.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 287);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(402, 167);
            this.richTextBox1.TabIndex = 18;
            this.richTextBox1.Text = "mn\nsp\npg\nhb00\nhb01\nhb02\nb2\nb3\ndb05\nh00\ns006";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 271);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Patterns and files to skip:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(287, 270);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(127, 17);
            this.checkBox1.TabIndex = 20;
            this.checkBox1.Text = "Only randomize those";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // Randomizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 598);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkedListBox1);
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
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Randomizer_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatSpeedMin)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericPlatTimeMin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChooseRoot;
        private System.Windows.Forms.Label labelRootDir;
        private System.Windows.Forms.Button buttonPerform;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonChooseFile;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericPlatSpeedMax;
        private System.Windows.Forms.NumericUpDown numericPlatSpeedMin;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericPlatTimeMax;
        private System.Windows.Forms.NumericUpDown numericPlatTimeMin;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}