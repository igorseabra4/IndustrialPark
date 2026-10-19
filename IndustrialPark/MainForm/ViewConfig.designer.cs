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
            buttonResetSpeeds = new System.Windows.Forms.Button();
            buttonResetView = new System.Windows.Forms.Button();
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
            groupBox2.Location = new System.Drawing.Point(10, 63);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(363, 47);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "View (Yaw, Pitch, Roll)";
            // 
            // NumericCameraRoll
            // 
            NumericCameraRoll.DecimalPlaces = 4;
            NumericCameraRoll.Location = new System.Drawing.Point(242, 19);
            NumericCameraRoll.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraRoll.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraRoll.Name = "NumericCameraRoll";
            NumericCameraRoll.Size = new System.Drawing.Size(112, 23);
            NumericCameraRoll.TabIndex = 8;
            NumericCameraRoll.ValueChanged += NumericCameraRot_ValueChanged;
            // 
            // NumericCameraPitch
            // 
            NumericCameraPitch.DecimalPlaces = 4;
            NumericCameraPitch.Location = new System.Drawing.Point(124, 19);
            NumericCameraPitch.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraPitch.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraPitch.Name = "NumericCameraPitch";
            NumericCameraPitch.Size = new System.Drawing.Size(112, 23);
            NumericCameraPitch.TabIndex = 7;
            NumericCameraPitch.ValueChanged += NumericCameraRot_ValueChanged;
            // 
            // NumericCameraYaw
            // 
            NumericCameraYaw.DecimalPlaces = 4;
            NumericCameraYaw.Location = new System.Drawing.Point(6, 19);
            NumericCameraYaw.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraYaw.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraYaw.Name = "NumericCameraYaw";
            NumericCameraYaw.Size = new System.Drawing.Size(112, 23);
            NumericCameraYaw.TabIndex = 6;
            NumericCameraYaw.ValueChanged += NumericCameraRot_ValueChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(NumericIntervalRotation);
            groupBox4.Controls.Add(NumericInterval);
            groupBox4.Location = new System.Drawing.Point(10, 116);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(242, 47);
            groupBox4.TabIndex = 9;
            groupBox4.TabStop = false;
            groupBox4.Text = "Move Speed, Rotate Speed";
            // 
            // NumericIntervalRotation
            // 
            NumericIntervalRotation.DecimalPlaces = 4;
            NumericIntervalRotation.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            NumericIntervalRotation.Location = new System.Drawing.Point(124, 19);
            NumericIntervalRotation.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            NumericIntervalRotation.Name = "NumericIntervalRotation";
            NumericIntervalRotation.Size = new System.Drawing.Size(112, 23);
            NumericIntervalRotation.TabIndex = 11;
            NumericIntervalRotation.Value = new decimal(new int[] { 5, 0, 0, 0 });
            NumericIntervalRotation.ValueChanged += NumericIntervalRotation_ValueChanged;
            // 
            // NumericInterval
            // 
            NumericInterval.DecimalPlaces = 4;
            NumericInterval.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            NumericInterval.Location = new System.Drawing.Point(6, 19);
            NumericInterval.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            NumericInterval.Name = "NumericInterval";
            NumericInterval.Size = new System.Drawing.Size(112, 23);
            NumericInterval.TabIndex = 10;
            NumericInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            NumericInterval.ValueChanged += NumericInterval_ValueChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(NumericFOV);
            groupBox3.Controls.Add(NumericDrawD);
            groupBox3.Location = new System.Drawing.Point(12, 173);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(240, 47);
            groupBox3.TabIndex = 12;
            groupBox3.TabStop = false;
            groupBox3.Text = "Draw Distance, Field of View";
            // 
            // NumericFOV
            // 
            NumericFOV.DecimalPlaces = 4;
            NumericFOV.Location = new System.Drawing.Point(122, 19);
            NumericFOV.Maximum = new decimal(new int[] { 1799999, 0, 0, 262144 });
            NumericFOV.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            NumericFOV.Name = "NumericFOV";
            NumericFOV.Size = new System.Drawing.Size(112, 23);
            NumericFOV.TabIndex = 14;
            NumericFOV.Value = new decimal(new int[] { 60, 0, 0, 0 });
            NumericFOV.ValueChanged += NumericFOV_ValueChanged;
            // 
            // NumericDrawD
            // 
            NumericDrawD.DecimalPlaces = 4;
            NumericDrawD.Location = new System.Drawing.Point(6, 19);
            NumericDrawD.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            NumericDrawD.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumericDrawD.Name = "NumericDrawD";
            NumericDrawD.Size = new System.Drawing.Size(110, 23);
            NumericDrawD.TabIndex = 13;
            NumericDrawD.Value = new decimal(new int[] { 500000, 0, 0, 0 });
            NumericDrawD.ValueChanged += NumericDrawD_ValueChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(NumericCameraZ);
            groupBox1.Controls.Add(NumericCameraY);
            groupBox1.Controls.Add(NumericCameraX);
            groupBox1.Location = new System.Drawing.Point(10, 11);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(363, 47);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "View (X, Y, Z)";
            // 
            // NumericCameraZ
            // 
            NumericCameraZ.DecimalPlaces = 4;
            NumericCameraZ.Location = new System.Drawing.Point(242, 19);
            NumericCameraZ.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraZ.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraZ.Name = "NumericCameraZ";
            NumericCameraZ.Size = new System.Drawing.Size(112, 23);
            NumericCameraZ.TabIndex = 4;
            NumericCameraZ.ValueChanged += NumericCamera_ValueChanged;
            // 
            // NumericCameraY
            // 
            NumericCameraY.DecimalPlaces = 4;
            NumericCameraY.Location = new System.Drawing.Point(124, 19);
            NumericCameraY.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraY.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraY.Name = "NumericCameraY";
            NumericCameraY.Size = new System.Drawing.Size(112, 23);
            NumericCameraY.TabIndex = 3;
            NumericCameraY.ValueChanged += NumericCamera_ValueChanged;
            // 
            // NumericCameraX
            // 
            NumericCameraX.DecimalPlaces = 4;
            NumericCameraX.Location = new System.Drawing.Point(6, 19);
            NumericCameraX.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            NumericCameraX.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            NumericCameraX.Name = "NumericCameraX";
            NumericCameraX.Size = new System.Drawing.Size(112, 23);
            NumericCameraX.TabIndex = 2;
            NumericCameraX.ValueChanged += NumericCamera_ValueChanged;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(numericGridZ);
            groupBox7.Controls.Add(numericGridY);
            groupBox7.Controls.Add(numericGridX);
            groupBox7.Location = new System.Drawing.Point(10, 226);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new System.Drawing.Size(363, 47);
            groupBox7.TabIndex = 15;
            groupBox7.TabStop = false;
            groupBox7.Text = "Grid Size (X, Y, Z)";
            // 
            // numericGridZ
            // 
            numericGridZ.DecimalPlaces = 4;
            numericGridZ.Location = new System.Drawing.Point(242, 19);
            numericGridZ.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            numericGridZ.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numericGridZ.Name = "numericGridZ";
            numericGridZ.Size = new System.Drawing.Size(112, 23);
            numericGridZ.TabIndex = 18;
            numericGridZ.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericGridZ.ValueChanged += numericGrid_ValueChanged;
            // 
            // numericGridY
            // 
            numericGridY.DecimalPlaces = 4;
            numericGridY.Location = new System.Drawing.Point(124, 19);
            numericGridY.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            numericGridY.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numericGridY.Name = "numericGridY";
            numericGridY.Size = new System.Drawing.Size(112, 23);
            numericGridY.TabIndex = 17;
            numericGridY.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericGridY.ValueChanged += numericGrid_ValueChanged;
            // 
            // numericGridX
            // 
            numericGridX.DecimalPlaces = 4;
            numericGridX.Location = new System.Drawing.Point(6, 19);
            numericGridX.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            numericGridX.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numericGridX.Name = "numericGridX";
            numericGridX.Size = new System.Drawing.Size(112, 23);
            numericGridX.TabIndex = 16;
            numericGridX.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericGridX.ValueChanged += numericGrid_ValueChanged;
            // 
            // buttonViewControls
            // 
            buttonViewControls.Location = new System.Drawing.Point(263, 173);
            buttonViewControls.Name = "buttonViewControls";
            buttonViewControls.Size = new System.Drawing.Size(106, 23);
            buttonViewControls.TabIndex = 21;
            buttonViewControls.Text = "View Controls";
            buttonViewControls.UseVisualStyleBackColor = true;
            buttonViewControls.Click += buttonViewControls_Click;
            // 
            // buttonAbout
            // 
            buttonAbout.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            buttonAbout.Location = new System.Drawing.Point(263, 202);
            buttonAbout.Name = "buttonAbout";
            buttonAbout.Size = new System.Drawing.Size(106, 23);
            buttonAbout.TabIndex = 22;
            buttonAbout.Text = "About";
            buttonAbout.UseVisualStyleBackColor = true;
            buttonAbout.Click += buttonAbout_Click;
            // 
            // buttonResetSpeeds
            // 
            buttonResetSpeeds.Location = new System.Drawing.Point(263, 144);
            buttonResetSpeeds.Name = "buttonResetSpeeds";
            buttonResetSpeeds.Size = new System.Drawing.Size(106, 23);
            buttonResetSpeeds.TabIndex = 20;
            buttonResetSpeeds.Text = "Reset Speeds";
            buttonResetSpeeds.UseVisualStyleBackColor = true;
            buttonResetSpeeds.Click += buttonResetSpeeds_Click;
            // 
            // buttonResetView
            // 
            buttonResetView.Location = new System.Drawing.Point(263, 115);
            buttonResetView.Name = "buttonResetView";
            buttonResetView.Size = new System.Drawing.Size(106, 23);
            buttonResetView.TabIndex = 19;
            buttonResetView.Text = "Reset View";
            buttonResetView.UseVisualStyleBackColor = true;
            buttonResetView.Click += buttonResetView_Click;
            // 
            // ViewConfig
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(383, 281);
            Controls.Add(buttonResetView);
            Controls.Add(buttonResetSpeeds);
            Controls.Add(buttonAbout);
            Controls.Add(buttonViewControls);
            Controls.Add(groupBox7);
            Controls.Add(groupBox2);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox1);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "ViewConfig";
            ShowIcon = false;
            Text = "View Config";
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
        private System.Windows.Forms.Button buttonResetSpeeds;
        private System.Windows.Forms.Button buttonResetView;
    }
}