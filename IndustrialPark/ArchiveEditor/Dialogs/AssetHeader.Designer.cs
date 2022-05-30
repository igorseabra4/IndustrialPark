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
            this.comboBoxAssetTypes = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelRawDataSize = new System.Windows.Forms.Label();
            this.buttonImportRawData = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkWriteT = new System.Windows.Forms.CheckBox();
            this.checkReadT = new System.Windows.Forms.CheckBox();
            this.checkSourceVirtual = new System.Windows.Forms.CheckBox();
            this.checkSourceFile = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxAssetID = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxAssetName = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxAssetFileName = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxChecksum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxAssetTypes
            // 
            this.comboBoxAssetTypes.FormattingEnabled = true;
            this.comboBoxAssetTypes.Location = new System.Drawing.Point(6, 19);
            this.comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            this.comboBoxAssetTypes.Size = new System.Drawing.Size(146, 21);
            this.comboBoxAssetTypes.TabIndex = 1;
            this.comboBoxAssetTypes.SelectedIndexChanged += new System.EventHandler(this.comboBoxAssetTypes_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxAssetTypes);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(158, 50);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Asset Type";
            // 
            // labelRawDataSize
            // 
            this.labelRawDataSize.AutoSize = true;
            this.labelRawDataSize.Location = new System.Drawing.Point(179, 235);
            this.labelRawDataSize.Name = "labelRawDataSize";
            this.labelRawDataSize.Size = new System.Drawing.Size(90, 13);
            this.labelRawDataSize.TabIndex = 3;
            this.labelRawDataSize.Text = "Raw Data Size: 0";
            // 
            // buttonImportRawData
            // 
            this.buttonImportRawData.Location = new System.Drawing.Point(182, 209);
            this.buttonImportRawData.Name = "buttonImportRawData";
            this.buttonImportRawData.Size = new System.Drawing.Size(172, 23);
            this.buttonImportRawData.TabIndex = 10;
            this.buttonImportRawData.Text = "Import Raw Data";
            this.buttonImportRawData.UseVisualStyleBackColor = true;
            this.buttonImportRawData.Click += new System.EventHandler(this.buttonImportRawData_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkWriteT);
            this.groupBox2.Controls.Add(this.checkReadT);
            this.groupBox2.Controls.Add(this.checkSourceVirtual);
            this.groupBox2.Controls.Add(this.checkSourceFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(158, 112);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Flags";
            // 
            // checkWriteT
            // 
            this.checkWriteT.AutoSize = true;
            this.checkWriteT.Location = new System.Drawing.Point(6, 88);
            this.checkWriteT.Name = "checkWriteT";
            this.checkWriteT.Size = new System.Drawing.Size(136, 17);
            this.checkWriteT.TabIndex = 8;
            this.checkWriteT.Text = "WRITE_TRANSFORM";
            this.checkWriteT.UseVisualStyleBackColor = true;
            // 
            // checkReadT
            // 
            this.checkReadT.AutoSize = true;
            this.checkReadT.Location = new System.Drawing.Point(6, 65);
            this.checkReadT.Name = "checkReadT";
            this.checkReadT.Size = new System.Drawing.Size(130, 17);
            this.checkReadT.TabIndex = 7;
            this.checkReadT.Text = "READ_TRANSFORM";
            this.checkReadT.UseVisualStyleBackColor = true;
            // 
            // checkSourceVirtual
            // 
            this.checkSourceVirtual.AutoSize = true;
            this.checkSourceVirtual.Location = new System.Drawing.Point(6, 42);
            this.checkSourceVirtual.Name = "checkSourceVirtual";
            this.checkSourceVirtual.Size = new System.Drawing.Size(123, 17);
            this.checkSourceVirtual.TabIndex = 6;
            this.checkSourceVirtual.Text = "SOURCE_VIRTUAL";
            this.checkSourceVirtual.UseVisualStyleBackColor = true;
            // 
            // checkSourceFile
            // 
            this.checkSourceFile.AutoSize = true;
            this.checkSourceFile.Location = new System.Drawing.Point(6, 19);
            this.checkSourceFile.Name = "checkSourceFile";
            this.checkSourceFile.Size = new System.Drawing.Size(99, 17);
            this.checkSourceFile.TabIndex = 5;
            this.checkSourceFile.Text = "SOURCE_FILE";
            this.checkSourceFile.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(182, 255);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(83, 23);
            this.buttonOK.TabIndex = 12;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(271, 255);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(83, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxAssetID);
            this.groupBox3.Location = new System.Drawing.Point(12, 68);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(158, 50);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Asset ID (hex)";
            // 
            // textBoxAssetID
            // 
            this.textBoxAssetID.Location = new System.Drawing.Point(6, 19);
            this.textBoxAssetID.Name = "textBoxAssetID";
            this.textBoxAssetID.Size = new System.Drawing.Size(146, 20);
            this.textBoxAssetID.TabIndex = 3;
            this.textBoxAssetID.TextChanged += new System.EventHandler(this.textBoxAssetID_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxAssetName);
            this.groupBox4.Location = new System.Drawing.Point(176, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(184, 50);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Asset Name";
            // 
            // textBoxAssetName
            // 
            this.textBoxAssetName.Location = new System.Drawing.Point(6, 19);
            this.textBoxAssetName.Name = "textBoxAssetName";
            this.textBoxAssetName.Size = new System.Drawing.Size(172, 20);
            this.textBoxAssetName.TabIndex = 2;
            this.textBoxAssetName.TextChanged += new System.EventHandler(this.textBoxAssetName_TextChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxAssetFileName);
            this.groupBox5.Location = new System.Drawing.Point(176, 68);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(184, 50);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Asset Filename";
            // 
            // textBoxAssetFileName
            // 
            this.textBoxAssetFileName.Location = new System.Drawing.Point(6, 19);
            this.textBoxAssetFileName.Name = "textBoxAssetFileName";
            this.textBoxAssetFileName.Size = new System.Drawing.Size(172, 20);
            this.textBoxAssetFileName.TabIndex = 4;
            this.textBoxAssetFileName.TextChanged += new System.EventHandler(this.textBoxAssetFilename_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBoxChecksum);
            this.groupBox6.Location = new System.Drawing.Point(176, 124);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(184, 50);
            this.groupBox6.TabIndex = 6;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Checksum (hex)";
            // 
            // textBoxChecksum
            // 
            this.textBoxChecksum.Location = new System.Drawing.Point(6, 19);
            this.textBoxChecksum.Name = "textBoxChecksum";
            this.textBoxChecksum.Size = new System.Drawing.Size(172, 20);
            this.textBoxChecksum.TabIndex = 9;
            this.textBoxChecksum.TextChanged += new System.EventHandler(this.textBoxChecksum_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 239);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 39);
            this.label1.TabIndex = 8;
            this.label1.Text = "Note: I have not been able to\r\nfigure out the flags automatically\r\nfor this asset" +
    " type.";
            this.label1.Visible = false;
            // 
            // AssetHeader
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(372, 288);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonImportRawData);
            this.Controls.Add(this.labelRawDataSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AssetHeader";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asset Header";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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