namespace IndustrialPark
{
    partial class ApplyVertexColors
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplyVertexColors));
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            colorPickerBtn = new System.Windows.Forms.Button();
            numericUpDownW = new System.Windows.Forms.NumericUpDown();
            numericUpDownZ = new System.Windows.Forms.NumericUpDown();
            numericUpDownY = new System.Windows.Forms.NumericUpDown();
            numericUpDownX = new System.Windows.Forms.NumericUpDown();
            groupBox2 = new System.Windows.Forms.GroupBox();
            comboBoxOperation = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            pnlColorPreview = new System.Windows.Forms.Panel();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownW).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownX).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            resources.ApplyResources(button2, "button2");
            button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button2.Name = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(colorPickerBtn);
            groupBox1.Controls.Add(numericUpDownW);
            groupBox1.Controls.Add(numericUpDownZ);
            groupBox1.Controls.Add(numericUpDownY);
            groupBox1.Controls.Add(numericUpDownX);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // colorPickerBtn
            // 
            resources.ApplyResources(colorPickerBtn, "colorPickerBtn");
            colorPickerBtn.Name = "colorPickerBtn";
            colorPickerBtn.UseVisualStyleBackColor = true;
            colorPickerBtn.Click += colorPickerBtn_Click;
            // 
            // numericUpDownW
            // 
            resources.ApplyResources(numericUpDownW, "numericUpDownW");
            numericUpDownW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownW.DecimalPlaces = 6;
            numericUpDownW.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownW.Name = "numericUpDownW";
            numericUpDownW.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numericUpDownZ
            // 
            resources.ApplyResources(numericUpDownZ, "numericUpDownZ");
            numericUpDownZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownZ.DecimalPlaces = 6;
            numericUpDownZ.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownZ.Name = "numericUpDownZ";
            numericUpDownZ.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownZ.ValueChanged += numericUpDown_ValueChanged;
            // 
            // numericUpDownY
            // 
            resources.ApplyResources(numericUpDownY, "numericUpDownY");
            numericUpDownY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownY.DecimalPlaces = 6;
            numericUpDownY.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownY.Name = "numericUpDownY";
            numericUpDownY.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownY.ValueChanged += numericUpDown_ValueChanged;
            // 
            // numericUpDownX
            // 
            resources.ApplyResources(numericUpDownX, "numericUpDownX");
            numericUpDownX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownX.DecimalPlaces = 6;
            numericUpDownX.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDownX.Name = "numericUpDownX";
            numericUpDownX.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownX.ValueChanged += numericUpDown_ValueChanged;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(comboBoxOperation);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // comboBoxOperation
            // 
            resources.ApplyResources(comboBoxOperation, "comboBoxOperation");
            comboBoxOperation.FormattingEnabled = true;
            comboBoxOperation.Name = "comboBoxOperation";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // pnlColorPreview
            // 
            resources.ApplyResources(pnlColorPreview, "pnlColorPreview");
            pnlColorPreview.BackColor = System.Drawing.Color.White;
            pnlColorPreview.Name = "pnlColorPreview";
            // 
            // ApplyVertexColors
            // 
            AcceptButton = button1;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = button2;
            Controls.Add(pnlColorPreview);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(button2);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ApplyVertexColors";
            ShowIcon = false;
            ShowInTaskbar = false;
            FormClosing += ApplyVertexColors_FormClosing;
            Shown += ApplyVertexColors_Shown;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDownW).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownX).EndInit();
            groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.NumericUpDown numericUpDownW;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBoxOperation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button colorPickerBtn;
        private System.Windows.Forms.Panel pnlColorPreview;
    }
}