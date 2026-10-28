namespace IndustrialPark
{
    partial class ConnectMarkers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectMarkers));
            label1 = new System.Windows.Forms.Label();
            radioLine = new System.Windows.Forms.RadioButton();
            radioCircle = new System.Windows.Forms.RadioButton();
            radioArc = new System.Windows.Forms.RadioButton();
            labelNumber = new System.Windows.Forms.Label();
            numericNumber = new System.Windows.Forms.NumericUpDown();
            numericAngle = new System.Windows.Forms.NumericUpDown();
            labelAngle = new System.Windows.Forms.Label();
            numericArc = new System.Windows.Forms.NumericUpDown();
            labelArc = new System.Windows.Forms.Label();
            buttonCreate = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            checkDeleteMarkers = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)numericNumber).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericAngle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericArc).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(41, 15);
            label1.TabIndex = 0;
            label1.Text = "Mode:";
            // 
            // radioLine
            // 
            radioLine.AutoSize = true;
            radioLine.Location = new System.Drawing.Point(59, 12);
            radioLine.Name = "radioLine";
            radioLine.Size = new System.Drawing.Size(47, 19);
            radioLine.TabIndex = 1;
            radioLine.TabStop = true;
            radioLine.Text = "Line";
            radioLine.UseVisualStyleBackColor = true;
            // 
            // radioCircle
            // 
            radioCircle.AutoSize = true;
            radioCircle.Location = new System.Drawing.Point(112, 12);
            radioCircle.Name = "radioCircle";
            radioCircle.Size = new System.Drawing.Size(55, 19);
            radioCircle.TabIndex = 2;
            radioCircle.TabStop = true;
            radioCircle.Text = "Circle";
            radioCircle.UseVisualStyleBackColor = true;
            // 
            // radioArc
            // 
            radioArc.AutoSize = true;
            radioArc.Location = new System.Drawing.Point(173, 12);
            radioArc.Name = "radioArc";
            radioArc.Size = new System.Drawing.Size(43, 19);
            radioArc.TabIndex = 3;
            radioArc.TabStop = true;
            radioArc.Text = "Arc";
            radioArc.UseVisualStyleBackColor = true;
            // 
            // labelNumber
            // 
            labelNumber.AutoSize = true;
            labelNumber.Location = new System.Drawing.Point(12, 39);
            labelNumber.Name = "labelNumber";
            labelNumber.Size = new System.Drawing.Size(54, 15);
            labelNumber.TabIndex = 4;
            labelNumber.Text = "Number:";
            // 
            // numericNumber
            // 
            numericNumber.Location = new System.Drawing.Point(72, 37);
            numericNumber.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericNumber.Name = "numericNumber";
            numericNumber.Size = new System.Drawing.Size(158, 23);
            numericNumber.TabIndex = 5;
            // 
            // numericAngle
            // 
            numericAngle.DecimalPlaces = 4;
            numericAngle.Location = new System.Drawing.Point(72, 66);
            numericAngle.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numericAngle.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
            numericAngle.Name = "numericAngle";
            numericAngle.Size = new System.Drawing.Size(158, 23);
            numericAngle.TabIndex = 7;
            // 
            // labelAngle
            // 
            labelAngle.AutoSize = true;
            labelAngle.Location = new System.Drawing.Point(12, 68);
            labelAngle.Name = "labelAngle";
            labelAngle.Size = new System.Drawing.Size(41, 15);
            labelAngle.TabIndex = 6;
            labelAngle.Text = "Angle:";
            // 
            // numericArc
            // 
            numericArc.DecimalPlaces = 4;
            numericArc.Location = new System.Drawing.Point(72, 95);
            numericArc.Maximum = new decimal(new int[] { 3599999, 0, 0, 262144 });
            numericArc.Minimum = new decimal(new int[] { 3599999, 0, 0, -2147221504 });
            numericArc.Name = "numericArc";
            numericArc.Size = new System.Drawing.Size(158, 23);
            numericArc.TabIndex = 9;
            numericArc.Value = new decimal(new int[] { 180, 0, 0, 0 });
            // 
            // labelArc
            // 
            labelArc.AutoSize = true;
            labelArc.Location = new System.Drawing.Point(12, 97);
            labelArc.Name = "labelArc";
            labelArc.Size = new System.Drawing.Size(28, 15);
            labelArc.TabIndex = 8;
            labelArc.Text = "Arc:";
            // 
            // buttonCreate
            // 
            buttonCreate.Location = new System.Drawing.Point(12, 149);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new System.Drawing.Size(218, 29);
            buttonCreate.TabIndex = 11;
            buttonCreate.Text = "Place Templates";
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += buttonCreate_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new System.Drawing.Point(12, 184);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(218, 29);
            buttonCancel.TabIndex = 12;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // checkDeleteMarkers
            // 
            checkDeleteMarkers.AutoSize = true;
            checkDeleteMarkers.Checked = true;
            checkDeleteMarkers.CheckState = System.Windows.Forms.CheckState.Checked;
            checkDeleteMarkers.Location = new System.Drawing.Point(12, 124);
            checkDeleteMarkers.Name = "checkDeleteMarkers";
            checkDeleteMarkers.Size = new System.Drawing.Size(104, 19);
            checkDeleteMarkers.TabIndex = 10;
            checkDeleteMarkers.Text = "Delete Markers";
            checkDeleteMarkers.UseVisualStyleBackColor = true;
            // 
            // ConnectMarkers
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(241, 224);
            ControlBox = false;
            Controls.Add(checkDeleteMarkers);
            Controls.Add(buttonCancel);
            Controls.Add(buttonCreate);
            Controls.Add(numericArc);
            Controls.Add(labelArc);
            Controls.Add(numericAngle);
            Controls.Add(labelAngle);
            Controls.Add(numericNumber);
            Controls.Add(labelNumber);
            Controls.Add(radioArc);
            Controls.Add(radioCircle);
            Controls.Add(radioLine);
            Controls.Add(label1);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConnectMarkers";
            ShowIcon = false;
            Text = "Connect Markers";
            ((System.ComponentModel.ISupportInitialize)numericNumber).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericAngle).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericArc).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioLine;
        private System.Windows.Forms.RadioButton radioCircle;
        private System.Windows.Forms.RadioButton radioArc;
        private System.Windows.Forms.Label labelNumber;
        private System.Windows.Forms.NumericUpDown numericNumber;
        private System.Windows.Forms.NumericUpDown numericAngle;
        private System.Windows.Forms.Label labelAngle;
        private System.Windows.Forms.NumericUpDown numericArc;
        private System.Windows.Forms.Label labelArc;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkDeleteMarkers;
    }
}