namespace IndustrialPark
{
    partial class AssetHeader
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
            comboBoxAssetTypes = new System.Windows.Forms.ComboBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            labelRawDataSize = new System.Windows.Forms.Label();
            buttonImportRawData = new System.Windows.Forms.Button();
            groupBox2 = new System.Windows.Forms.GroupBox();
            checkWriteT = new System.Windows.Forms.CheckBox();
            checkReadT = new System.Windows.Forms.CheckBox();
            checkSourceVirtual = new System.Windows.Forms.CheckBox();
            checkSourceFile = new System.Windows.Forms.CheckBox();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            groupBox3 = new System.Windows.Forms.GroupBox();
            textBoxAssetID = new System.Windows.Forms.TextBox();
            groupBox4 = new System.Windows.Forms.GroupBox();
            textBoxAssetName = new System.Windows.Forms.TextBox();
            groupBox5 = new System.Windows.Forms.GroupBox();
            textBoxAssetFileName = new System.Windows.Forms.TextBox();
            groupBox6 = new System.Windows.Forms.GroupBox();
            textBoxChecksum = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxAssetTypes
            // 
            comboBoxAssetTypes.FormattingEnabled = true;
            comboBoxAssetTypes.Location = new System.Drawing.Point(6, 19);
            comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            comboBoxAssetTypes.Size = new System.Drawing.Size(146, 21);
            comboBoxAssetTypes.TabIndex = 1;
            comboBoxAssetTypes.SelectedIndexChanged += comboBoxAssetTypes_SelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBoxAssetTypes);
            groupBox1.Location = new System.Drawing.Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(158, 50);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Asset Type";
            // 
            // labelRawDataSize
            // 
            labelRawDataSize.AutoSize = true;
            labelRawDataSize.Location = new System.Drawing.Point(179, 235);
            labelRawDataSize.Name = "labelRawDataSize";
            labelRawDataSize.Size = new System.Drawing.Size(90, 13);
            labelRawDataSize.TabIndex = 3;
            labelRawDataSize.Text = "Raw Data Size: 0";
            // 
            // buttonImportRawData
            // 
            buttonImportRawData.Location = new System.Drawing.Point(182, 209);
            buttonImportRawData.Name = "buttonImportRawData";
            buttonImportRawData.Size = new System.Drawing.Size(172, 23);
            buttonImportRawData.TabIndex = 10;
            buttonImportRawData.Text = "Import Raw Data";
            buttonImportRawData.UseVisualStyleBackColor = true;
            buttonImportRawData.Click += buttonImportRawData_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(checkWriteT);
            groupBox2.Controls.Add(checkReadT);
            groupBox2.Controls.Add(checkSourceVirtual);
            groupBox2.Controls.Add(checkSourceFile);
            groupBox2.Location = new System.Drawing.Point(12, 124);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(158, 112);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Flags";
            // 
            // checkWriteT
            // 
            checkWriteT.AutoSize = true;
            checkWriteT.Location = new System.Drawing.Point(6, 88);
            checkWriteT.Name = "checkWriteT";
            checkWriteT.Size = new System.Drawing.Size(136, 17);
            checkWriteT.TabIndex = 8;
            checkWriteT.Text = "WRITE_TRANSFORM";
            checkWriteT.UseVisualStyleBackColor = true;
            // 
            // checkReadT
            // 
            checkReadT.AutoSize = true;
            checkReadT.Location = new System.Drawing.Point(6, 65);
            checkReadT.Name = "checkReadT";
            checkReadT.Size = new System.Drawing.Size(130, 17);
            checkReadT.TabIndex = 7;
            checkReadT.Text = "READ_TRANSFORM";
            checkReadT.UseVisualStyleBackColor = true;
            // 
            // checkSourceVirtual
            // 
            checkSourceVirtual.AutoSize = true;
            checkSourceVirtual.Location = new System.Drawing.Point(6, 42);
            checkSourceVirtual.Name = "checkSourceVirtual";
            checkSourceVirtual.Size = new System.Drawing.Size(123, 17);
            checkSourceVirtual.TabIndex = 6;
            checkSourceVirtual.Text = "SOURCE_VIRTUAL";
            checkSourceVirtual.UseVisualStyleBackColor = true;
            // 
            // checkSourceFile
            // 
            checkSourceFile.AutoSize = true;
            checkSourceFile.Location = new System.Drawing.Point(6, 19);
            checkSourceFile.Name = "checkSourceFile";
            checkSourceFile.Size = new System.Drawing.Size(99, 17);
            checkSourceFile.TabIndex = 5;
            checkSourceFile.Text = "SOURCE_FILE";
            checkSourceFile.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.Location = new System.Drawing.Point(182, 255);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(83, 23);
            buttonOK.TabIndex = 11;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(271, 255);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(83, 23);
            buttonCancel.TabIndex = 12;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(textBoxAssetID);
            groupBox3.Location = new System.Drawing.Point(12, 68);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(158, 50);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "Asset ID (hex)";
            // 
            // textBoxAssetID
            // 
            textBoxAssetID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxAssetID.Location = new System.Drawing.Point(6, 19);
            textBoxAssetID.Name = "textBoxAssetID";
            textBoxAssetID.Size = new System.Drawing.Size(146, 20);
            textBoxAssetID.TabIndex = 3;
            textBoxAssetID.TextChanged += textBoxAssetID_TextChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(textBoxAssetName);
            groupBox4.Location = new System.Drawing.Point(176, 12);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(184, 50);
            groupBox4.TabIndex = 4;
            groupBox4.TabStop = false;
            groupBox4.Text = "Asset Name";
            // 
            // textBoxAssetName
            // 
            textBoxAssetName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxAssetName.Location = new System.Drawing.Point(6, 19);
            textBoxAssetName.Name = "textBoxAssetName";
            textBoxAssetName.Size = new System.Drawing.Size(172, 20);
            textBoxAssetName.TabIndex = 2;
            textBoxAssetName.TextChanged += textBoxAssetName_TextChanged;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(textBoxAssetFileName);
            groupBox5.Location = new System.Drawing.Point(176, 68);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(184, 50);
            groupBox5.TabIndex = 5;
            groupBox5.TabStop = false;
            groupBox5.Text = "Asset Filename";
            // 
            // textBoxAssetFileName
            // 
            textBoxAssetFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxAssetFileName.Location = new System.Drawing.Point(6, 19);
            textBoxAssetFileName.Name = "textBoxAssetFileName";
            textBoxAssetFileName.Size = new System.Drawing.Size(172, 20);
            textBoxAssetFileName.TabIndex = 4;
            textBoxAssetFileName.TextChanged += textBoxAssetFilename_TextChanged;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(textBoxChecksum);
            groupBox6.Location = new System.Drawing.Point(176, 124);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new System.Drawing.Size(184, 50);
            groupBox6.TabIndex = 6;
            groupBox6.TabStop = false;
            groupBox6.Text = "Checksum (hex)";
            // 
            // textBoxChecksum
            // 
            textBoxChecksum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxChecksum.Location = new System.Drawing.Point(6, 19);
            textBoxChecksum.Name = "textBoxChecksum";
            textBoxChecksum.Size = new System.Drawing.Size(172, 20);
            textBoxChecksum.TabIndex = 9;
            textBoxChecksum.TextChanged += textBoxChecksum_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 239);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(158, 39);
            label1.TabIndex = 8;
            label1.Text = "Note: I have not been able to\r\nfigure out the flags automatically\r\nfor this asset type.";
            label1.Visible = false;
            // 
            // AssetHeader
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(372, 288);
            Controls.Add(label1);
            Controls.Add(groupBox6);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(buttonCancel);
            Controls.Add(groupBox1);
            Controls.Add(groupBox2);
            Controls.Add(buttonOK);
            Controls.Add(buttonImportRawData);
            Controls.Add(labelRawDataSize);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "AssetHeader";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Asset Header";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxAssetTypes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelRawDataSize;
        private System.Windows.Forms.Button buttonImportRawData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkWriteT;
        private System.Windows.Forms.CheckBox checkReadT;
        private System.Windows.Forms.CheckBox checkSourceVirtual;
        private System.Windows.Forms.CheckBox checkSourceFile;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxAssetID;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBoxAssetName;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxAssetFileName;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxChecksum;
        private System.Windows.Forms.Label label1;
    }
}