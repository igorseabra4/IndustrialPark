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
            this.colorPickerBtn = new System.Windows.Forms.Button();
            this.numericUpDownW = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownZ = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownX = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxOperation = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlColorPreview = new System.Windows.Forms.Panel();
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
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.colorPickerBtn);
            this.groupBox1.Controls.Add(this.numericUpDownW);
            this.groupBox1.Controls.Add(this.numericUpDownZ);
            this.groupBox1.Controls.Add(this.numericUpDownY);
            this.groupBox1.Controls.Add(this.numericUpDownX);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // colorPickerBtn
            // 
            resources.ApplyResources(this.colorPickerBtn, "colorPickerBtn");
            this.colorPickerBtn.Name = "colorPickerBtn";
            this.colorPickerBtn.UseVisualStyleBackColor = true;
            this.colorPickerBtn.Click += new System.EventHandler(this.colorPickerBtn_Click);
            // 
            // numericUpDownW
            // 
            resources.ApplyResources(this.numericUpDownW, "numericUpDownW");
            this.numericUpDownW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownW.DecimalPlaces = 6;
            this.numericUpDownW.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownW.Name = "numericUpDownW";
            this.numericUpDownW.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownZ
            // 
            resources.ApplyResources(this.numericUpDownZ, "numericUpDownZ");
            this.numericUpDownZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownZ.DecimalPlaces = 6;
            this.numericUpDownZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownZ.Name = "numericUpDownZ";
            this.numericUpDownZ.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownZ.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownY
            // 
            resources.ApplyResources(this.numericUpDownY, "numericUpDownY");
            this.numericUpDownY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownY.DecimalPlaces = 6;
            this.numericUpDownY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownY.Name = "numericUpDownY";
            this.numericUpDownY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownY.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // numericUpDownX
            // 
            resources.ApplyResources(this.numericUpDownX, "numericUpDownX");
            this.numericUpDownX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownX.DecimalPlaces = 6;
            this.numericUpDownX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownX.Name = "numericUpDownX";
            this.numericUpDownX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownX.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.comboBoxOperation);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // comboBoxOperation
            // 
            resources.ApplyResources(this.comboBoxOperation, "comboBoxOperation");
            this.comboBoxOperation.FormattingEnabled = true;
            this.comboBoxOperation.Name = "comboBoxOperation";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // pnlColorPreview
            // 
            resources.ApplyResources(this.pnlColorPreview, "pnlColorPreview");
            this.pnlColorPreview.BackColor = System.Drawing.Color.White;
            this.pnlColorPreview.Name = "pnlColorPreview";
            // 
            // ApplyVertexColors
            // 
            this.AcceptButton = this.button1;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.Controls.Add(this.pnlColorPreview);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplyVertexColors";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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
        private System.Windows.Forms.Panel pnlColorPreview;
    }
}