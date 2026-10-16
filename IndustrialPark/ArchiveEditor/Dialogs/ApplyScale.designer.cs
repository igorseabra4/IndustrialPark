namespace IndustrialPark
{
    partial class ApplyScale
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
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            numericUpDownZ = new System.Windows.Forms.NumericUpDown();
            numericUpDownY = new System.Windows.Forms.NumericUpDown();
            numericUpDownX = new System.Windows.Forms.NumericUpDown();
            checkedListBoxAssetTypes = new System.Windows.Forms.CheckedListBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            checkBoxBakeScales = new System.Windows.Forms.CheckBox();
            checkBoxBakeNpcScales = new System.Windows.Forms.CheckBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownX).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(168, 303);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 10;
            button1.Text = "Apply";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button2.Location = new System.Drawing.Point(249, 303);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(75, 23);
            button2.TabIndex = 11;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(numericUpDownZ);
            groupBox1.Controls.Add(numericUpDownY);
            groupBox1.Controls.Add(numericUpDownX);
            groupBox1.Location = new System.Drawing.Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(319, 49);
            groupBox1.TabIndex = 3;
            groupBox1.TabStop = false;
            groupBox1.Text = "Scale (X, Y, Z)";
            // 
            // numericUpDownZ
            // 
            numericUpDownZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownZ.DecimalPlaces = 4;
            numericUpDownZ.Location = new System.Drawing.Point(214, 19);
            numericUpDownZ.Name = "numericUpDownZ";
            numericUpDownZ.Size = new System.Drawing.Size(98, 20);
            numericUpDownZ.TabIndex = 6;
            numericUpDownZ.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownZ.ValueChanged += numericUpDown_ValueChanged;
            // 
            // numericUpDownY
            // 
            numericUpDownY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownY.DecimalPlaces = 4;
            numericUpDownY.Location = new System.Drawing.Point(110, 19);
            numericUpDownY.Name = "numericUpDownY";
            numericUpDownY.Size = new System.Drawing.Size(98, 20);
            numericUpDownY.TabIndex = 5;
            numericUpDownY.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownY.ValueChanged += numericUpDown_ValueChanged;
            // 
            // numericUpDownX
            // 
            numericUpDownX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownX.DecimalPlaces = 4;
            numericUpDownX.Location = new System.Drawing.Point(6, 19);
            numericUpDownX.Name = "numericUpDownX";
            numericUpDownX.Size = new System.Drawing.Size(98, 20);
            numericUpDownX.TabIndex = 4;
            numericUpDownX.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownX.ValueChanged += numericUpDown_ValueChanged;
            // 
            // checkedListBoxAssetTypes
            // 
            checkedListBoxAssetTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            checkedListBoxAssetTypes.FormattingEnabled = true;
            checkedListBoxAssetTypes.Location = new System.Drawing.Point(12, 95);
            checkedListBoxAssetTypes.Name = "checkedListBoxAssetTypes";
            checkedListBoxAssetTypes.Size = new System.Drawing.Size(312, 167);
            checkedListBoxAssetTypes.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 64);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(35, 13);
            label1.TabIndex = 5;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(15, 79);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(104, 13);
            label2.TabIndex = 6;
            label2.Text = "Asset types to scale:";
            // 
            // checkBoxBakeScales
            // 
            checkBoxBakeScales.AutoSize = true;
            checkBoxBakeScales.Checked = true;
            checkBoxBakeScales.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxBakeScales.Location = new System.Drawing.Point(18, 267);
            checkBoxBakeScales.Name = "checkBoxBakeScales";
            checkBoxBakeScales.Size = new System.Drawing.Size(132, 30);
            checkBoxBakeScales.TabIndex = 8;
            checkBoxBakeScales.Text = "Bake unproportional\r\nscales on entity assets";
            checkBoxBakeScales.UseVisualStyleBackColor = true;
            // 
            // checkBoxBakeNpcScales
            // 
            checkBoxBakeNpcScales.AutoSize = true;
            checkBoxBakeNpcScales.Location = new System.Drawing.Point(18, 300);
            checkBoxBakeNpcScales.Name = "checkBoxBakeNpcScales";
            checkBoxBakeNpcScales.Size = new System.Drawing.Size(128, 30);
            checkBoxBakeNpcScales.TabIndex = 9;
            checkBoxBakeNpcScales.Text = "Apply and bake scale\r\nto NPCs";
            checkBoxBakeNpcScales.UseVisualStyleBackColor = true;
            // 
            // ApplyScale
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = button2;
            ClientSize = new System.Drawing.Size(336, 337);
            Controls.Add(checkBoxBakeNpcScales);
            Controls.Add(checkBoxBakeScales);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkedListBoxAssetTypes);
            Controls.Add(groupBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ApplyScale";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Apply Scale";
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDownZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownX).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDownZ;
        private System.Windows.Forms.NumericUpDown numericUpDownY;
        private System.Windows.Forms.NumericUpDown numericUpDownX;
        private System.Windows.Forms.CheckedListBox checkedListBoxAssetTypes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxBakeScales;
        private System.Windows.Forms.CheckBox checkBoxBakeNpcScales;
    }
}