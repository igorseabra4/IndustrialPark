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
            groupBox2 = new System.Windows.Forms.GroupBox();
            NumericCameraRoll = new System.Windows.Forms.NumericUpDown();
            NumericCameraPitch = new System.Windows.Forms.NumericUpDown();
            NumericCameraYaw = new System.Windows.Forms.NumericUpDown();
            groupBox4 = new System.Windows.Forms.GroupBox();
            NumericIntervalRotation = new System.Windows.Forms.NumericUpDown();
            NumericInterval = new System.Windows.Forms.NumericUpDown();
            groupBox3 = new System.Windows.Forms.GroupBox();
            NumericFOV = new System.Windows.Forms.NumericUpDown();
            NumericDrawD = new System.Windows.Forms.NumericUpDown();
            groupBox1 = new System.Windows.Forms.GroupBox();
            NumericCameraZ = new System.Windows.Forms.NumericUpDown();
            NumericCameraY = new System.Windows.Forms.NumericUpDown();
            NumericCameraX = new System.Windows.Forms.NumericUpDown();
            groupBox7 = new System.Windows.Forms.GroupBox();
            numericGridZ = new System.Windows.Forms.NumericUpDown();
            numericGridY = new System.Windows.Forms.NumericUpDown();
            numericGridX = new System.Windows.Forms.NumericUpDown();
            buttonViewControls = new System.Windows.Forms.Button();
            buttonAbout = new System.Windows.Forms.Button();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericCameraRoll).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraPitch).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraYaw).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericIntervalRotation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericInterval).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericFOV).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericDrawD).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericCameraZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraX).BeginInit();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericGridZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericGridY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericGridX).BeginInit();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(NumericCameraRoll);
            groupBox2.Controls.Add(NumericCameraPitch);
            groupBox2.Controls.Add(NumericCameraYaw);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // NumericCameraRoll
            // 
            NumericCameraRoll.DecimalPlaces = 4;
            resources.ApplyResources(NumericCameraRoll, "NumericCameraRoll");
            NumericCameraRoll.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraRoll.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraRoll.Name = "NumericCameraRoll";
            NumericCameraRoll.ValueChanged += NumericCameraRot_ValueChanged;
            // 
            // NumericCameraPitch
            // 
            NumericCameraPitch.DecimalPlaces = 4;
            resources.ApplyResources(NumericCameraPitch, "NumericCameraPitch");
            NumericCameraPitch.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraPitch.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraPitch.Name = "NumericCameraPitch";
            NumericCameraPitch.ValueChanged += NumericCameraRot_ValueChanged;
            // 
            // NumericCameraYaw
            // 
            NumericCameraYaw.DecimalPlaces = 4;
            resources.ApplyResources(NumericCameraYaw, "NumericCameraYaw");
            NumericCameraYaw.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraYaw.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraYaw.Name = "NumericCameraYaw";
            NumericCameraYaw.ValueChanged += NumericCameraRot_ValueChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(NumericIntervalRotation);
            groupBox4.Controls.Add(NumericInterval);
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // NumericIntervalRotation
            // 
            NumericIntervalRotation.DecimalPlaces = 4;
            resources.ApplyResources(NumericIntervalRotation, "NumericIntervalRotation");
            NumericIntervalRotation.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            NumericIntervalRotation.Name = "NumericIntervalRotation";
            NumericIntervalRotation.Value = new decimal(new int[] { 5, 0, 0, 0 });
            NumericIntervalRotation.ValueChanged += NumericIntervalRotation_ValueChanged;
            // 
            // NumericInterval
            // 
            NumericInterval.DecimalPlaces = 4;
            resources.ApplyResources(NumericInterval, "NumericInterval");
            NumericInterval.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            NumericInterval.Name = "NumericInterval";
            NumericInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            NumericInterval.ValueChanged += NumericInterval_ValueChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(NumericFOV);
            groupBox3.Controls.Add(NumericDrawD);
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // NumericFOV
            // 
            NumericFOV.DecimalPlaces = 4;
            resources.ApplyResources(NumericFOV, "NumericFOV");
            NumericFOV.Maximum = new decimal(new int[] { 1799999, 0, 0, 262144 });
            NumericFOV.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            NumericFOV.Name = "NumericFOV";
            NumericFOV.Value = new decimal(new int[] { 60, 0, 0, 0 });
            NumericFOV.ValueChanged += NumericFOV_ValueChanged;
            // 
            // NumericDrawD
            // 
            NumericDrawD.DecimalPlaces = 4;
            resources.ApplyResources(NumericDrawD, "NumericDrawD");
            NumericDrawD.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            NumericDrawD.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumericDrawD.Name = "NumericDrawD";
            NumericDrawD.Value = new decimal(new int[] { 500000, 0, 0, 0 });
            NumericDrawD.ValueChanged += NumericDrawD_ValueChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(NumericCameraZ);
            groupBox1.Controls.Add(NumericCameraY);
            groupBox1.Controls.Add(NumericCameraX);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // NumericCameraZ
            // 
            NumericCameraZ.DecimalPlaces = 4;
            resources.ApplyResources(NumericCameraZ, "NumericCameraZ");
            NumericCameraZ.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraZ.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraZ.Name = "NumericCameraZ";
            NumericCameraZ.ValueChanged += NumericCamera_ValueChanged;
            // 
            // NumericCameraY
            // 
            NumericCameraY.DecimalPlaces = 4;
            resources.ApplyResources(NumericCameraY, "NumericCameraY");
            NumericCameraY.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraY.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraY.Name = "NumericCameraY";
            NumericCameraY.ValueChanged += NumericCamera_ValueChanged;
            // 
            // NumericCameraX
            // 
            NumericCameraX.DecimalPlaces = 4;
            resources.ApplyResources(NumericCameraX, "NumericCameraX");
            NumericCameraX.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraX.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraX.Name = "NumericCameraX";
            NumericCameraX.ValueChanged += NumericCamera_ValueChanged;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(numericGridZ);
            groupBox7.Controls.Add(numericGridY);
            groupBox7.Controls.Add(numericGridX);
            resources.ApplyResources(groupBox7, "groupBox7");
            groupBox7.Name = "groupBox7";
            groupBox7.TabStop = false;
            // 
            // numericGridZ
            // 
            numericGridZ.DecimalPlaces = 4;
            resources.ApplyResources(numericGridZ, "numericGridZ");
            numericGridZ.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            numericGridZ.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numericGridZ.Name = "numericGridZ";
            numericGridZ.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericGridZ.ValueChanged += numericGrid_ValueChanged;
            // 
            // numericGridY
            // 
            numericGridY.DecimalPlaces = 4;
            resources.ApplyResources(numericGridY, "numericGridY");
            numericGridY.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            numericGridY.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numericGridY.Name = "numericGridY";
            numericGridY.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericGridY.ValueChanged += numericGrid_ValueChanged;
            // 
            // numericGridX
            // 
            numericGridX.DecimalPlaces = 4;
            resources.ApplyResources(numericGridX, "numericGridX");
            numericGridX.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            numericGridX.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numericGridX.Name = "numericGridX";
            numericGridX.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericGridX.ValueChanged += numericGrid_ValueChanged;
            // 
            // buttonViewControls
            // 
            resources.ApplyResources(buttonViewControls, "buttonViewControls");
            buttonViewControls.Name = "buttonViewControls";
            buttonViewControls.UseVisualStyleBackColor = true;
            buttonViewControls.Click += buttonViewControls_Click;
            // 
            // buttonAbout
            // 
            resources.ApplyResources(buttonAbout, "buttonAbout");
            buttonAbout.Name = "buttonAbout";
            buttonAbout.UseVisualStyleBackColor = true;
            buttonAbout.Click += buttonAbout_Click;
            // 
            // ViewConfig
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(buttonAbout);
            Controls.Add(buttonViewControls);
            Controls.Add(groupBox7);
            Controls.Add(groupBox2);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "ViewConfig";
            ShowIcon = false;
            Load += ViewConfig_Load;
            VisibleChanged += ViewConfig_VisibleChanged;
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NumericCameraRoll).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraPitch).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraYaw).EndInit();
            groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NumericIntervalRotation).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericInterval).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NumericFOV).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericDrawD).EndInit();
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NumericCameraZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraY).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraX).EndInit();
            groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericGridZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericGridY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericGridX).EndInit();
            ResumeLayout(false);

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
        public System.Windows.Forms.NumericUpDown NumericFOV;
        public System.Windows.Forms.NumericUpDown NumericIntervalRotation;
        private System.Windows.Forms.GroupBox groupBox7;
        public System.Windows.Forms.NumericUpDown numericGridZ;
        public System.Windows.Forms.NumericUpDown numericGridY;
        public System.Windows.Forms.NumericUpDown numericGridX;
        public System.Windows.Forms.NumericUpDown NumericCameraRoll;
        private System.Windows.Forms.Button buttonViewControls;
        private System.Windows.Forms.Button buttonAbout;
    }
}