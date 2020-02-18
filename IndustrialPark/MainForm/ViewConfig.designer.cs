namespace IndustrialPark
{
    partial class ViewConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewConfig));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.NumericCameraYaw = new System.Windows.Forms.NumericUpDown();
            this.NumericCameraPitch = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.NumericInterval = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.NumericDrawD = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NumericCameraZ = new System.Windows.Forms.NumericUpDown();
            this.NumericCameraY = new System.Windows.Forms.NumericUpDown();
            this.NumericCameraX = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.NumericFOV = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.NumericIntervalRotation = new System.Windows.Forms.NumericUpDown();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.numericGridZ = new System.Windows.Forms.NumericUpDown();
            this.numericGridY = new System.Windows.Forms.NumericUpDown();
            this.numericGridX = new System.Windows.Forms.NumericUpDown();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraYaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraPitch)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericInterval)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericDrawD)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraX)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericFOV)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericIntervalRotation)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericGridZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGridY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGridX)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.NumericCameraYaw);
            this.groupBox2.Controls.Add(this.NumericCameraPitch);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // NumericCameraYaw
            // 
            this.NumericCameraYaw.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericCameraYaw, "NumericCameraYaw");
            this.NumericCameraYaw.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.NumericCameraYaw.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.NumericCameraYaw.Name = "NumericCameraYaw";
            this.NumericCameraYaw.ValueChanged += new System.EventHandler(this.NumericCameraRot_ValueChanged);
            // 
            // NumericCameraPitch
            // 
            this.NumericCameraPitch.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericCameraPitch, "NumericCameraPitch");
            this.NumericCameraPitch.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.NumericCameraPitch.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.NumericCameraPitch.Name = "NumericCameraPitch";
            this.NumericCameraPitch.ValueChanged += new System.EventHandler(this.NumericCameraRot_ValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.NumericInterval);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // NumericInterval
            // 
            this.NumericInterval.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericInterval, "NumericInterval");
            this.NumericInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.NumericInterval.Name = "NumericInterval";
            this.NumericInterval.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NumericInterval.ValueChanged += new System.EventHandler(this.NumericInterval_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.NumericDrawD);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // NumericDrawD
            // 
            this.NumericDrawD.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericDrawD, "NumericDrawD");
            this.NumericDrawD.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NumericDrawD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericDrawD.Name = "NumericDrawD";
            this.NumericDrawD.Value = new decimal(new int[] {
            500000,
            0,
            0,
            0});
            this.NumericDrawD.ValueChanged += new System.EventHandler(this.NumericDrawD_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NumericCameraZ);
            this.groupBox1.Controls.Add(this.NumericCameraY);
            this.groupBox1.Controls.Add(this.NumericCameraX);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // NumericCameraZ
            // 
            this.NumericCameraZ.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericCameraZ, "NumericCameraZ");
            this.NumericCameraZ.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.NumericCameraZ.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.NumericCameraZ.Name = "NumericCameraZ";
            this.NumericCameraZ.ValueChanged += new System.EventHandler(this.NumericCamera_ValueChanged);
            // 
            // NumericCameraY
            // 
            this.NumericCameraY.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericCameraY, "NumericCameraY");
            this.NumericCameraY.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.NumericCameraY.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.NumericCameraY.Name = "NumericCameraY";
            this.NumericCameraY.ValueChanged += new System.EventHandler(this.NumericCamera_ValueChanged);
            // 
            // NumericCameraX
            // 
            this.NumericCameraX.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericCameraX, "NumericCameraX");
            this.NumericCameraX.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.NumericCameraX.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.NumericCameraX.Name = "NumericCameraX";
            this.NumericCameraX.ValueChanged += new System.EventHandler(this.NumericCamera_ValueChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.NumericFOV);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // NumericFOV
            // 
            this.NumericFOV.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericFOV, "NumericFOV");
            this.NumericFOV.Maximum = new decimal(new int[] {
            1799999,
            0,
            0,
            262144});
            this.NumericFOV.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.NumericFOV.Name = "NumericFOV";
            this.NumericFOV.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.NumericFOV.ValueChanged += new System.EventHandler(this.NumericFOV_ValueChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.NumericIntervalRotation);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // NumericIntervalRotation
            // 
            this.NumericIntervalRotation.DecimalPlaces = 4;
            resources.ApplyResources(this.NumericIntervalRotation, "NumericIntervalRotation");
            this.NumericIntervalRotation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.NumericIntervalRotation.Name = "NumericIntervalRotation";
            this.NumericIntervalRotation.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NumericIntervalRotation.ValueChanged += new System.EventHandler(this.NumericIntervalRotation_ValueChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.numericGridZ);
            this.groupBox7.Controls.Add(this.numericGridY);
            this.groupBox7.Controls.Add(this.numericGridX);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // numericGridZ
            // 
            this.numericGridZ.DecimalPlaces = 4;
            resources.ApplyResources(this.numericGridZ, "numericGridZ");
            this.numericGridZ.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericGridZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericGridZ.Name = "numericGridZ";
            this.numericGridZ.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericGridZ.ValueChanged += new System.EventHandler(this.numericGrid_ValueChanged);
            // 
            // numericGridY
            // 
            this.numericGridY.DecimalPlaces = 4;
            resources.ApplyResources(this.numericGridY, "numericGridY");
            this.numericGridY.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericGridY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericGridY.Name = "numericGridY";
            this.numericGridY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericGridY.ValueChanged += new System.EventHandler(this.numericGrid_ValueChanged);
            // 
            // numericGridX
            // 
            this.numericGridX.DecimalPlaces = 4;
            resources.ApplyResources(this.numericGridX, "numericGridX");
            this.numericGridX.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericGridX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numericGridX.Name = "numericGridX";
            this.numericGridX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericGridX.ValueChanged += new System.EventHandler(this.numericGrid_ValueChanged);
            // 
            // ViewConfig
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ViewConfig";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.ViewConfig_Load);
            this.VisibleChanged += new System.EventHandler(this.ViewConfig_VisibleChanged);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraYaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraPitch)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericInterval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericDrawD)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericCameraX)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericFOV)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumericIntervalRotation)).EndInit();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericGridZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGridY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericGridX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.NumericUpDown NumericCameraYaw;
        public System.Windows.Forms.NumericUpDown NumericCameraPitch;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.NumericUpDown NumericInterval;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.NumericUpDown NumericDrawD;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.NumericUpDown NumericCameraY;
        public System.Windows.Forms.NumericUpDown NumericCameraX;
        public System.Windows.Forms.NumericUpDown NumericCameraZ;
        private System.Windows.Forms.GroupBox groupBox5;
        public System.Windows.Forms.NumericUpDown NumericFOV;
        private System.Windows.Forms.GroupBox groupBox6;
        public System.Windows.Forms.NumericUpDown NumericIntervalRotation;
        private System.Windows.Forms.GroupBox groupBox7;
        public System.Windows.Forms.NumericUpDown numericGridZ;
        public System.Windows.Forms.NumericUpDown numericGridY;
        public System.Windows.Forms.NumericUpDown numericGridX;
    }
}