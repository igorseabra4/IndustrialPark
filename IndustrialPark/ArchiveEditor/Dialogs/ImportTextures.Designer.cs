namespace IndustrialPark
{
    partial class ImportTextures
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
            this.buttonImportRawData = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.checkBoxFlipTextures = new System.Windows.Forms.CheckBox();
            this.checkBoxMipmaps = new System.Windows.Forms.CheckBox();
            this.checkBoxCompress = new System.Windows.Forms.CheckBox();
            this.checkBoxRW3 = new System.Windows.Forms.CheckBox();
            this.checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            this.checkBoxTransFix = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonImportRawData
            // 
            this.buttonImportRawData.Location = new System.Drawing.Point(18, 12);
            this.buttonImportRawData.Name = "buttonImportRawData";
            this.buttonImportRawData.Size = new System.Drawing.Size(190, 23);
            this.buttonImportRawData.TabIndex = 4;
            this.buttonImportRawData.Text = "Select Textures...";
            this.buttonImportRawData.UseVisualStyleBackColor = true;
            this.buttonImportRawData.Click += new System.EventHandler(this.buttonImportRawData_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(18, 303);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(92, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "Import";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(116, 303);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(92, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(12, 41);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(202, 164);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Assets";
            // 
            // listBox1
            // 
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(190, 132);
            this.listBox1.TabIndex = 9;
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            // 
            // checkBoxFlipTextures
            // 
            this.checkBoxFlipTextures.AutoSize = true;
            this.checkBoxFlipTextures.Location = new System.Drawing.Point(18, 280);
            this.checkBoxFlipTextures.Name = "checkBoxFlipTextures";
            this.checkBoxFlipTextures.Size = new System.Drawing.Size(87, 17);
            this.checkBoxFlipTextures.TabIndex = 7;
            this.checkBoxFlipTextures.Text = "Flip Vertically";
            this.checkBoxFlipTextures.UseVisualStyleBackColor = true;
            // 
            // checkBoxMipmaps
            // 
            this.checkBoxMipmaps.AutoSize = true;
            this.checkBoxMipmaps.Checked = true;
            this.checkBoxMipmaps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMipmaps.Location = new System.Drawing.Point(18, 234);
            this.checkBoxMipmaps.Name = "checkBoxMipmaps";
            this.checkBoxMipmaps.Size = new System.Drawing.Size(115, 17);
            this.checkBoxMipmaps.TabIndex = 8;
            this.checkBoxMipmaps.Text = "Generate Mipmaps";
            this.checkBoxMipmaps.UseVisualStyleBackColor = true;
            // 
            // checkBoxCompress
            // 
            this.checkBoxCompress.AutoSize = true;
            this.checkBoxCompress.Checked = true;
            this.checkBoxCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCompress.Location = new System.Drawing.Point(18, 257);
            this.checkBoxCompress.Name = "checkBoxCompress";
            this.checkBoxCompress.Size = new System.Drawing.Size(72, 17);
            this.checkBoxCompress.TabIndex = 9;
            this.checkBoxCompress.Text = "Compress";
            this.checkBoxCompress.UseVisualStyleBackColor = true;
            this.checkBoxCompress.CheckedChanged += new System.EventHandler(this.checkBoxCompress_CheckedChanged);
            // 
            // checkBoxRW3
            // 
            this.checkBoxRW3.AutoSize = true;
            this.checkBoxRW3.Checked = true;
            this.checkBoxRW3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRW3.Location = new System.Drawing.Point(18, 211);
            this.checkBoxRW3.Name = "checkBoxRW3";
            this.checkBoxRW3.Size = new System.Drawing.Size(94, 17);
            this.checkBoxRW3.TabIndex = 10;
            this.checkBoxRW3.Text = "Append .RW3";
            this.checkBoxRW3.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverwrite
            // 
            this.checkBoxOverwrite.AutoSize = true;
            this.checkBoxOverwrite.Checked = true;
            this.checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOverwrite.Location = new System.Drawing.Point(137, 280);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(71, 17);
            this.checkBoxOverwrite.TabIndex = 11;
            this.checkBoxOverwrite.Text = "Overwrite";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // checkBoxTransFix
            // 
            this.checkBoxTransFix.AutoSize = true;
            this.checkBoxTransFix.Location = new System.Drawing.Point(101, 257);
            this.checkBoxTransFix.Name = "checkBoxTransFix";
            this.checkBoxTransFix.Size = new System.Drawing.Size(113, 17);
            this.checkBoxTransFix.TabIndex = 12;
            this.checkBoxTransFix.Text = "Has Transparency";
            this.checkBoxTransFix.UseVisualStyleBackColor = true;
            this.checkBoxTransFix.CheckedChanged += new System.EventHandler(this.checkBoxTransFix_CheckedChanged);
            // 
            // ImportTextures
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(224, 335);
            this.Controls.Add(this.checkBoxTransFix);
            this.Controls.Add(this.checkBoxOverwrite);
            this.Controls.Add(this.checkBoxRW3);
            this.Controls.Add(this.checkBoxCompress);
            this.Controls.Add(this.checkBoxMipmaps);
            this.Controls.Add(this.checkBoxFlipTextures);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonImportRawData);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ImportTextures";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import Textures";
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonImportRawData;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox checkBoxFlipTextures;
        private System.Windows.Forms.CheckBox checkBoxMipmaps;
        private System.Windows.Forms.CheckBox checkBoxCompress;
        private System.Windows.Forms.CheckBox checkBoxRW3;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;
        private System.Windows.Forms.CheckBox checkBoxTransFix;
    }
}