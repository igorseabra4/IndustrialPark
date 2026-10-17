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
            buttonImportRawData = new System.Windows.Forms.Button();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            listBox1 = new System.Windows.Forms.ListBox();
            checkBoxFlipTextures = new System.Windows.Forms.CheckBox();
            checkBoxMipmaps = new System.Windows.Forms.CheckBox();
            checkBoxCompress = new System.Windows.Forms.CheckBox();
            checkBoxRW3 = new System.Windows.Forms.CheckBox();
            checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            checkBoxTransFix = new System.Windows.Forms.CheckBox();
            labelFiles = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // buttonImportRawData
            // 
            buttonImportRawData.Location = new System.Drawing.Point(12, 12);
            buttonImportRawData.Name = "buttonImportRawData";
            buttonImportRawData.Size = new System.Drawing.Size(200, 23);
            buttonImportRawData.TabIndex = 1;
            buttonImportRawData.Text = "Select Textures...";
            buttonImportRawData.UseVisualStyleBackColor = true;
            buttonImportRawData.Click += buttonImportRawData_Click;
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.Location = new System.Drawing.Point(18, 303);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(92, 23);
            buttonOK.TabIndex = 9;
            buttonOK.Text = "Import";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(116, 303);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(92, 23);
            buttonCancel.TabIndex = 10;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new System.Drawing.Point(12, 54);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(200, 145);
            listBox1.TabIndex = 2;
            listBox1.DragDrop += listBox1_DragDrop;
            listBox1.DragEnter += listBox1_DragEnter;
            listBox1.KeyDown += listBox1_KeyDown;
            // 
            // checkBoxFlipTextures
            // 
            checkBoxFlipTextures.AutoSize = true;
            checkBoxFlipTextures.Location = new System.Drawing.Point(18, 280);
            checkBoxFlipTextures.Name = "checkBoxFlipTextures";
            checkBoxFlipTextures.Size = new System.Drawing.Size(87, 17);
            checkBoxFlipTextures.TabIndex = 7;
            checkBoxFlipTextures.Text = "Flip Vertically";
            checkBoxFlipTextures.UseVisualStyleBackColor = true;
            // 
            // checkBoxMipmaps
            // 
            checkBoxMipmaps.AutoSize = true;
            checkBoxMipmaps.Checked = true;
            checkBoxMipmaps.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxMipmaps.Location = new System.Drawing.Point(18, 234);
            checkBoxMipmaps.Name = "checkBoxMipmaps";
            checkBoxMipmaps.Size = new System.Drawing.Size(115, 17);
            checkBoxMipmaps.TabIndex = 4;
            checkBoxMipmaps.Text = "Generate Mipmaps";
            checkBoxMipmaps.UseVisualStyleBackColor = true;
            // 
            // checkBoxCompress
            // 
            checkBoxCompress.AutoSize = true;
            checkBoxCompress.Checked = true;
            checkBoxCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxCompress.Location = new System.Drawing.Point(18, 257);
            checkBoxCompress.Name = "checkBoxCompress";
            checkBoxCompress.Size = new System.Drawing.Size(72, 17);
            checkBoxCompress.TabIndex = 5;
            checkBoxCompress.Text = "Compress";
            checkBoxCompress.UseVisualStyleBackColor = true;
            checkBoxCompress.CheckedChanged += checkBoxCompress_CheckedChanged;
            // 
            // checkBoxRW3
            // 
            checkBoxRW3.AutoSize = true;
            checkBoxRW3.Checked = true;
            checkBoxRW3.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxRW3.Location = new System.Drawing.Point(18, 211);
            checkBoxRW3.Name = "checkBoxRW3";
            checkBoxRW3.Size = new System.Drawing.Size(94, 17);
            checkBoxRW3.TabIndex = 3;
            checkBoxRW3.Text = "Append .RW3";
            checkBoxRW3.UseVisualStyleBackColor = true;
            // 
            // checkBoxOverwrite
            // 
            checkBoxOverwrite.AutoSize = true;
            checkBoxOverwrite.Checked = true;
            checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxOverwrite.Location = new System.Drawing.Point(137, 280);
            checkBoxOverwrite.Name = "checkBoxOverwrite";
            checkBoxOverwrite.Size = new System.Drawing.Size(71, 17);
            checkBoxOverwrite.TabIndex = 8;
            checkBoxOverwrite.Text = "Overwrite";
            checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // checkBoxTransFix
            // 
            checkBoxTransFix.AutoSize = true;
            checkBoxTransFix.Location = new System.Drawing.Point(101, 257);
            checkBoxTransFix.Name = "checkBoxTransFix";
            checkBoxTransFix.Size = new System.Drawing.Size(113, 17);
            checkBoxTransFix.TabIndex = 6;
            checkBoxTransFix.Text = "Has Transparency";
            checkBoxTransFix.UseVisualStyleBackColor = true;
            checkBoxTransFix.CheckedChanged += checkBoxTransFix_CheckedChanged;
            // 
            // labelFiles
            // 
            labelFiles.AutoSize = true;
            labelFiles.Location = new System.Drawing.Point(12, 38);
            labelFiles.Name = "labelFiles";
            labelFiles.Size = new System.Drawing.Size(28, 13);
            labelFiles.TabIndex = 13;
            labelFiles.Text = "Files";
            // 
            // ImportTextures
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(224, 335);
            Controls.Add(labelFiles);
            Controls.Add(listBox1);
            Controls.Add(checkBoxTransFix);
            Controls.Add(checkBoxOverwrite);
            Controls.Add(checkBoxRW3);
            Controls.Add(checkBoxCompress);
            Controls.Add(checkBoxMipmaps);
            Controls.Add(checkBoxFlipTextures);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(buttonImportRawData);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "ImportTextures";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Import Textures";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonImportRawData;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox checkBoxFlipTextures;
        private System.Windows.Forms.CheckBox checkBoxMipmaps;
        private System.Windows.Forms.CheckBox checkBoxCompress;
        private System.Windows.Forms.CheckBox checkBoxRW3;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;
        private System.Windows.Forms.CheckBox checkBoxTransFix;
        private System.Windows.Forms.Label labelFiles;
    }
}