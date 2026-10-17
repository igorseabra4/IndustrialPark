namespace IndustrialPark
{
    partial class AddMultipleAssets
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
            buttonImportRawData = new System.Windows.Forms.Button();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            groupBox2 = new System.Windows.Forms.GroupBox();
            listBox1 = new System.Windows.Forms.ListBox();
            checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxAssetTypes
            // 
            comboBoxAssetTypes.FormattingEnabled = true;
            comboBoxAssetTypes.Location = new System.Drawing.Point(6, 19);
            comboBoxAssetTypes.Name = "comboBoxAssetTypes";
            comboBoxAssetTypes.Size = new System.Drawing.Size(190, 21);
            comboBoxAssetTypes.TabIndex = 1;
            comboBoxAssetTypes.SelectedIndexChanged += comboBoxAssetTypes_SelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBoxAssetTypes);
            groupBox1.Location = new System.Drawing.Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(202, 50);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Asset Type";
            // 
            // buttonImportRawData
            // 
            buttonImportRawData.Location = new System.Drawing.Point(18, 68);
            buttonImportRawData.Name = "buttonImportRawData";
            buttonImportRawData.Size = new System.Drawing.Size(190, 23);
            buttonImportRawData.TabIndex = 3;
            buttonImportRawData.Text = "Import Raw Data";
            buttonImportRawData.UseVisualStyleBackColor = true;
            buttonImportRawData.Click += buttonImportRawData_Click;
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonOK.Location = new System.Drawing.Point(18, 290);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(92, 23);
            buttonOK.TabIndex = 5;
            buttonOK.Text = "Import";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(116, 290);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(92, 23);
            buttonCancel.TabIndex = 6;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 316);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(199, 26);
            label1.TabIndex = 8;
            label1.Text = "Note: I have not been able to figure out\r\nthe flags automatically for this asset type.";
            label1.Visible = false;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(listBox1);
            groupBox2.Location = new System.Drawing.Point(12, 97);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(202, 164);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Assets";
            // 
            // listBox1
            // 
            listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new System.Drawing.Point(6, 19);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(190, 132);
            listBox1.TabIndex = 2;
            listBox1.KeyDown += listBox1_KeyDown;
            // 
            // checkBoxOverwrite
            // 
            checkBoxOverwrite.AutoSize = true;
            checkBoxOverwrite.Checked = true;
            checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxOverwrite.Location = new System.Drawing.Point(18, 267);
            checkBoxOverwrite.Name = "checkBoxOverwrite";
            checkBoxOverwrite.Size = new System.Drawing.Size(71, 17);
            checkBoxOverwrite.TabIndex = 4;
            checkBoxOverwrite.Text = "Overwrite";
            checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // AddMultipleAssets
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(224, 351);
            Controls.Add(checkBoxOverwrite);
            Controls.Add(buttonImportRawData);
            Controls.Add(groupBox2);
            Controls.Add(label1);
            Controls.Add(buttonCancel);
            Controls.Add(groupBox1);
            Controls.Add(buttonOK);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "AddMultipleAssets";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Add Multiple Assets";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxAssetTypes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonImportRawData;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;
    }
}