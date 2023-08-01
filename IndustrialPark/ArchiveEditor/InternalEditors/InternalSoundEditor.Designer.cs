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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InternalSoundEditor));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonImportRaw = new System.Windows.Forms.Button();
            this.checkBoxSendToSNDI = new System.Windows.Forms.CheckBox();
            this.buttonGenerateJawData = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.propertyGridSoundData = new System.Windows.Forms.PropertyGrid();
            this.buttonImportSound = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindCallers, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.button2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonImportRaw, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxSendToSNDI, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonPlay, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridSoundData, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonImportSound, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonGenerateJawData, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(344, 441);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHelp.Location = new System.Drawing.Point(3, 415);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(166, 23);
            this.buttonHelp.TabIndex = 17;
            this.buttonHelp.Text = "Open Wiki Page";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindCallers.Location = new System.Drawing.Point(175, 415);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(166, 23);
            this.buttonFindCallers.TabIndex = 11;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(3, 387);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(166, 22);
            this.button2.TabIndex = 10;
            this.button2.Text = "Export Sound Data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.richTextBox1, 2);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 81);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(338, 147);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // buttonImportRaw
            // 
            this.buttonImportRaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonImportRaw.Location = new System.Drawing.Point(3, 53);
            this.buttonImportRaw.Name = "buttonImportRaw";
            this.buttonImportRaw.Size = new System.Drawing.Size(166, 22);
            this.buttonImportRaw.TabIndex = 7;
            this.buttonImportRaw.Text = "Import Raw Sound";
            this.buttonImportRaw.UseVisualStyleBackColor = true;
            this.buttonImportRaw.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBoxSendToSNDI
            // 
            this.checkBoxSendToSNDI.AutoSize = true;
            this.checkBoxSendToSNDI.Checked = true;
            this.checkBoxSendToSNDI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.checkBoxSendToSNDI, 2);
            this.checkBoxSendToSNDI.Location = new System.Drawing.Point(3, 3);
            this.checkBoxSendToSNDI.Name = "checkBoxSendToSNDI";
            this.checkBoxSendToSNDI.Size = new System.Drawing.Size(178, 16);
            this.checkBoxSendToSNDI.TabIndex = 8;
            this.checkBoxSendToSNDI.Text = "Trim header and send it to SNDI";
            this.checkBoxSendToSNDI.UseVisualStyleBackColor = true;
            // 
            // buttonGenerateJawData
            // 
            this.buttonGenerateJawData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonGenerateJawData.Location = new System.Drawing.Point(175, 25);
            this.buttonGenerateJawData.Name = "buttonGenerateJawData";
            this.buttonGenerateJawData.Size = new System.Drawing.Size(166, 22);
            this.buttonGenerateJawData.TabIndex = 12;
            this.buttonGenerateJawData.Text = "Generate Jaw Data";
            this.buttonGenerateJawData.UseVisualStyleBackColor = true;
            this.buttonGenerateJawData.Click += new System.EventHandler(this.buttonImportJawData_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPlay.Location = new System.Drawing.Point(175, 387);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(166, 22);
            this.buttonPlay.TabIndex = 18;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // propertyGridSoundData
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridSoundData, 2);
            this.propertyGridSoundData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridSoundData.HelpVisible = false;
            this.propertyGridSoundData.Location = new System.Drawing.Point(3, 234);
            this.propertyGridSoundData.Name = "propertyGridSoundData";
            this.propertyGridSoundData.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridSoundData.Size = new System.Drawing.Size(338, 147);
            this.propertyGridSoundData.TabIndex = 19;
            this.propertyGridSoundData.ToolbarVisible = false;
            this.propertyGridSoundData.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridSoundData_PropertyValueChanged);
            // 
            // buttonImportSound
            // 
            this.buttonImportSound.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonImportSound.Location = new System.Drawing.Point(3, 25);
            this.buttonImportSound.Name = "buttonImportSound";
            this.buttonImportSound.Size = new System.Drawing.Size(166, 22);
            this.buttonImportSound.TabIndex = 20;
            this.buttonImportSound.Text = "Import Sound";
            this.buttonImportSound.UseVisualStyleBackColor = true;
            this.buttonImportSound.Click += new System.EventHandler(this.buttonImportSound_Click);
            // 
            // InternalSoundEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 441);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "InternalSoundEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalAssetEditor_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonImportRaw;
        private System.Windows.Forms.CheckBox checkBoxSendToSNDI;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonGenerateJawData;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.PropertyGrid propertyGridSoundData;
        private System.Windows.Forms.Button buttonImportSound;
    }
}