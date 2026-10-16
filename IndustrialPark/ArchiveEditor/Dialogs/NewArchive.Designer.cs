namespace IndustrialPark
{
    partial class NewArchive
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
            comboBoxGame = new System.Windows.Forms.ComboBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            groupBox2 = new System.Windows.Forms.GroupBox();
            comboBoxPlatform = new System.Windows.Forms.ComboBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            textBoxPCRT = new System.Windows.Forms.TextBox();
            checkBoxDefaultAssets = new System.Windows.Forms.CheckBox();
            checkBoxNoLayers = new System.Windows.Forms.CheckBox();
            labelActionUndone = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // comboBoxGame
            // 
            comboBoxGame.FormattingEnabled = true;
            comboBoxGame.Location = new System.Drawing.Point(6, 19);
            comboBoxGame.Name = "comboBoxGame";
            comboBoxGame.Size = new System.Drawing.Size(281, 21);
            comboBoxGame.TabIndex = 1;
            comboBoxGame.SelectedIndexChanged += comboBox_SelectedIndexChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBoxGame);
            groupBox1.Location = new System.Drawing.Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(293, 48);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Game";
            // 
            // buttonOK
            // 
            buttonOK.Enabled = false;
            buttonOK.Location = new System.Drawing.Point(97, 220);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(98, 23);
            buttonOK.TabIndex = 9;
            buttonOK.Text = "Create Archive";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(201, 220);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(98, 23);
            buttonCancel.TabIndex = 10;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(comboBoxPlatform);
            groupBox2.Location = new System.Drawing.Point(12, 66);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(293, 48);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Platform";
            // 
            // comboBoxPlatform
            // 
            comboBoxPlatform.FormattingEnabled = true;
            comboBoxPlatform.Location = new System.Drawing.Point(6, 19);
            comboBoxPlatform.Name = "comboBoxPlatform";
            comboBoxPlatform.Size = new System.Drawing.Size(281, 21);
            comboBoxPlatform.TabIndex = 3;
            comboBoxPlatform.SelectedIndexChanged += comboBox_SelectedIndexChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(dateTimePicker1);
            groupBox3.Controls.Add(textBoxPCRT);
            groupBox3.Location = new System.Drawing.Point(12, 120);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(287, 74);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Date/Time Information";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new System.Drawing.Point(6, 19);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new System.Drawing.Size(275, 20);
            dateTimePicker1.TabIndex = 5;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
            // 
            // textBoxPCRT
            // 
            textBoxPCRT.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxPCRT.Location = new System.Drawing.Point(6, 45);
            textBoxPCRT.Name = "textBoxPCRT";
            textBoxPCRT.Size = new System.Drawing.Size(275, 20);
            textBoxPCRT.TabIndex = 6;
            // 
            // checkBoxDefaultAssets
            // 
            checkBoxDefaultAssets.AutoSize = true;
            checkBoxDefaultAssets.Location = new System.Drawing.Point(18, 200);
            checkBoxDefaultAssets.Name = "checkBoxDefaultAssets";
            checkBoxDefaultAssets.Size = new System.Drawing.Size(116, 17);
            checkBoxDefaultAssets.TabIndex = 7;
            checkBoxDefaultAssets.Text = "Add Default Assets";
            checkBoxDefaultAssets.UseVisualStyleBackColor = true;
            // 
            // checkBoxNoLayers
            // 
            checkBoxNoLayers.AutoSize = true;
            checkBoxNoLayers.Checked = true;
            checkBoxNoLayers.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxNoLayers.Location = new System.Drawing.Point(140, 200);
            checkBoxNoLayers.Name = "checkBoxNoLayers";
            checkBoxNoLayers.Size = new System.Drawing.Size(74, 17);
            checkBoxNoLayers.TabIndex = 8;
            checkBoxNoLayers.Text = "No Layers";
            checkBoxNoLayers.UseVisualStyleBackColor = true;
            // 
            // labelActionUndone
            // 
            labelActionUndone.AutoSize = true;
            labelActionUndone.Location = new System.Drawing.Point(18, 201);
            labelActionUndone.Name = "labelActionUndone";
            labelActionUndone.Size = new System.Drawing.Size(149, 13);
            labelActionUndone.TabIndex = 11;
            labelActionUndone.Text = "This action cannot be undone";
            // 
            // NewArchive
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(317, 255);
            Controls.Add(checkBoxNoLayers);
            Controls.Add(groupBox3);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(checkBoxDefaultAssets);
            Controls.Add(labelActionUndone);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "NewArchive";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "PACK Settings";
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxGame;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBoxPlatform;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxPCRT;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox checkBoxDefaultAssets;
        private System.Windows.Forms.CheckBox checkBoxNoLayers;
        private System.Windows.Forms.Label labelActionUndone;
    }
}