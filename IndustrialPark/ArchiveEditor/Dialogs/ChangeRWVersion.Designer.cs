namespace IndustrialPark.Dialogs
{
    partial class ChangeRWVersion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeRWVersion));
            label1 = new System.Windows.Forms.Label();
            numericUpDownRenderware = new System.Windows.Forms.NumericUpDown();
            numericUpDownMajor = new System.Windows.Forms.NumericUpDown();
            numericUpDownMinor = new System.Windows.Forms.NumericUpDown();
            numericUpDownBinary = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            buttonCancel = new System.Windows.Forms.Button();
            buttonChange = new System.Windows.Forms.Button();
            labelCurVersion = new System.Windows.Forms.Label();
            checkBoxRemember = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDownRenderware).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMajor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMinor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBinary).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(286, 104);
            label1.TabIndex = 0;
            label1.Text = resources.GetString("label1.Text");
            // 
            // numericUpDownRenderware
            // 
            numericUpDownRenderware.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            numericUpDownRenderware.Location = new System.Drawing.Point(97, 137);
            numericUpDownRenderware.Name = "numericUpDownRenderware";
            numericUpDownRenderware.ReadOnly = true;
            numericUpDownRenderware.Size = new System.Drawing.Size(29, 20);
            numericUpDownRenderware.TabIndex = 1;
            numericUpDownRenderware.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // numericUpDownMajor
            // 
            numericUpDownMajor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            numericUpDownMajor.Location = new System.Drawing.Point(132, 137);
            numericUpDownMajor.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            numericUpDownMajor.Name = "numericUpDownMajor";
            numericUpDownMajor.Size = new System.Drawing.Size(29, 20);
            numericUpDownMajor.TabIndex = 2;
            numericUpDownMajor.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // numericUpDownMinor
            // 
            numericUpDownMinor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            numericUpDownMinor.Location = new System.Drawing.Point(167, 137);
            numericUpDownMinor.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            numericUpDownMinor.Name = "numericUpDownMinor";
            numericUpDownMinor.Size = new System.Drawing.Size(29, 20);
            numericUpDownMinor.TabIndex = 3;
            // 
            // numericUpDownBinary
            // 
            numericUpDownBinary.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            numericUpDownBinary.Location = new System.Drawing.Point(202, 137);
            numericUpDownBinary.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numericUpDownBinary.Name = "numericUpDownBinary";
            numericUpDownBinary.Size = new System.Drawing.Size(29, 20);
            numericUpDownBinary.TabIndex = 4;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 114);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(82, 13);
            label2.TabIndex = 5;
            label2.Text = "Current Version:";
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 137);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(79, 13);
            label3.TabIndex = 6;
            label3.Text = "Target Version:";
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(219, 167);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(75, 23);
            buttonCancel.TabIndex = 7;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonChange
            // 
            buttonChange.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonChange.DialogResult = System.Windows.Forms.DialogResult.OK;
            buttonChange.Location = new System.Drawing.Point(138, 167);
            buttonChange.Name = "buttonChange";
            buttonChange.Size = new System.Drawing.Size(75, 23);
            buttonChange.TabIndex = 8;
            buttonChange.Text = "Change";
            buttonChange.UseVisualStyleBackColor = true;
            // 
            // labelCurVersion
            // 
            labelCurVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelCurVersion.AutoSize = true;
            labelCurVersion.Location = new System.Drawing.Point(97, 114);
            labelCurVersion.Name = "labelCurVersion";
            labelCurVersion.Size = new System.Drawing.Size(35, 13);
            labelCurVersion.TabIndex = 9;
            labelCurVersion.Text = "label4";
            // 
            // checkBoxRemember
            // 
            checkBoxRemember.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            checkBoxRemember.AutoSize = true;
            checkBoxRemember.Location = new System.Drawing.Point(12, 171);
            checkBoxRemember.Name = "checkBoxRemember";
            checkBoxRemember.Size = new System.Drawing.Size(105, 17);
            checkBoxRemember.TabIndex = 10;
            checkBoxRemember.Text = "Remember for all";
            checkBoxRemember.UseVisualStyleBackColor = true;
            // 
            // ChangeRWVersion
            // 
            AcceptButton = buttonChange;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(298, 202);
            Controls.Add(checkBoxRemember);
            Controls.Add(labelCurVersion);
            Controls.Add(buttonChange);
            Controls.Add(buttonCancel);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(numericUpDownBinary);
            Controls.Add(numericUpDownMinor);
            Controls.Add(numericUpDownMajor);
            Controls.Add(numericUpDownRenderware);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChangeRWVersion";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Change RW version";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)numericUpDownRenderware).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMajor).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMinor).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownBinary).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownRenderware;
        private System.Windows.Forms.NumericUpDown numericUpDownMajor;
        private System.Windows.Forms.NumericUpDown numericUpDownMinor;
        private System.Windows.Forms.NumericUpDown numericUpDownBinary;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonChange;
        private System.Windows.Forms.Label labelCurVersion;
        private System.Windows.Forms.CheckBox checkBoxRemember;
    }
}