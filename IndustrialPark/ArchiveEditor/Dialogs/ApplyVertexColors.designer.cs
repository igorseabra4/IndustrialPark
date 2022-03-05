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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownW = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownZ = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownX = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxOperation = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.colorPickerBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(361, 145);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(280, 145);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.colorPickerBtn);
            this.groupBox1.Controls.Add(this.numericUpDownW);
            this.groupBox1.Controls.Add(this.numericUpDownZ);
            this.groupBox1.Controls.Add(this.numericUpDownY);
            this.groupBox1.Controls.Add(this.numericUpDownX);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 75);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color (Red, Green, Blue, Alpha)";
            // 
            // numericUpDownW
            // 
            this.numericUpDownW.DecimalPlaces = 6;
            this.numericUpDownW.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownW.Location = new System.Drawing.Point(266, 19);
            this.numericUpDownW.Name = "numericUpDownW";
            this.numericUpDownW.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownW.TabIndex = 7;
            this.numericUpDownW.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownZ
            // 
            this.numericUpDownZ.DecimalPlaces = 6;
            this.numericUpDownZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownZ.Location = new System.Drawing.Point(178, 19);
            this.numericUpDownZ.Name = "numericUpDownZ";
            this.numericUpDownZ.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownZ.TabIndex = 6;
            this.numericUpDownZ.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownZ.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownY
            // 
            this.numericUpDownY.DecimalPlaces = 6;
            this.numericUpDownY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownY.Location = new System.Drawing.Point(92, 19);
            this.numericUpDownY.Name = "numericUpDownY";
            this.numericUpDownY.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownY.TabIndex = 5;
            this.numericUpDownY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownY.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownX
            // 
            this.numericUpDownX.DecimalPlaces = 6;
            this.numericUpDownX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownX.Location = new System.Drawing.Point(6, 19);
            this.numericUpDownX.Name = "numericUpDownX";
            this.numericUpDownX.Size = new System.Drawing.Size(80, 20);
            this.numericUpDownX.TabIndex = 4;
            this.numericUpDownX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownX.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBoxOperation);
            this.groupBox2.Location = new System.Drawing.Point(272, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(164, 47);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Operation";
            // 
            // comboBoxOperation
            // 
            this.comboBoxOperation.FormattingEnabled = true;
            this.comboBoxOperation.Location = new System.Drawing.Point(6, 19);
            this.comboBoxOperation.Name = "comboBoxOperation";
            this.comboBoxOperation.Size = new System.Drawing.Size(150, 21);
            this.comboBoxOperation.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 78);
            this.label1.TabIndex = 9;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // colorPickerBtn
            // 
            this.colorPickerBtn.Location = new System.Drawing.Point(237, 46);
            this.colorPickerBtn.Name = "colorPickerBtn";
            this.colorPickerBtn.Size = new System.Drawing.Size(109, 23);
            this.colorPickerBtn.TabIndex = 10;
            this.colorPickerBtn.Text = "Color Picker...";
            this.colorPickerBtn.UseVisualStyleBackColor = true;
            this.colorPickerBtn.Click += new System.EventHandler(this.colorPickerBtn_Click);
            // 
            // ApplyVertexColors
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(442, 180);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplyVertexColors";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Apply Vertex Colors";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ApplyVertexColors_FormClosing);
            this.Shown += new System.EventHandler(this.ApplyVertexColors_Shown);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}