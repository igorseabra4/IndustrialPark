namespace IndustrialPark
{
    partial class MaterialEffectEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxMaterials = new System.Windows.Forms.ListBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panelColor = new System.Windows.Forms.Panel();
            this.propertyGridTextureInfo = new System.Windows.Forms.PropertyGrid();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBoxMatEffects = new System.Windows.Forms.ComboBox();
            this.propertyGridMatEffects = new System.Windows.Forms.PropertyGrid();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxMaterials);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(170, 354);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Materials";
            // 
            // listBoxMaterials
            // 
            this.listBoxMaterials.FormattingEnabled = true;
            this.listBoxMaterials.Location = new System.Drawing.Point(6, 19);
            this.listBoxMaterials.Name = "listBoxMaterials";
            this.listBoxMaterials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxMaterials.Size = new System.Drawing.Size(158, 329);
            this.listBoxMaterials.TabIndex = 1;
            this.listBoxMaterials.SelectedIndexChanged += new System.EventHandler(this.listBoxMaterials_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(18, 372);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(76, 23);
            this.buttonOK.TabIndex = 18;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(100, 372);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(76, 23);
            this.buttonCancel.TabIndex = 19;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(188, 65);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(324, 125);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Texture";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelColor);
            this.groupBox2.Location = new System.Drawing.Point(188, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(324, 47);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Material Color";
            // 
            // panelColor
            // 
            this.panelColor.Location = new System.Drawing.Point(6, 19);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(312, 22);
            this.panelColor.TabIndex = 20;
            this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
            // 
            // propertyGridTextureInfo
            // 
            this.propertyGridTextureInfo.HelpVisible = false;
            this.propertyGridTextureInfo.Location = new System.Drawing.Point(194, 84);
            this.propertyGridTextureInfo.Name = "propertyGridTextureInfo";
            this.propertyGridTextureInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridTextureInfo.Size = new System.Drawing.Size(312, 100);
            this.propertyGridTextureInfo.TabIndex = 22;
            this.propertyGridTextureInfo.ToolbarVisible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxMatEffects);
            this.groupBox3.Controls.Add(this.propertyGridMatEffects);
            this.groupBox3.Location = new System.Drawing.Point(188, 196);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(324, 199);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Material Effects";
            // 
            // comboBoxMatEffects
            // 
            this.comboBoxMatEffects.FormattingEnabled = true;
            this.comboBoxMatEffects.Location = new System.Drawing.Point(6, 19);
            this.comboBoxMatEffects.Name = "comboBoxMatEffects";
            this.comboBoxMatEffects.Size = new System.Drawing.Size(312, 21);
            this.comboBoxMatEffects.TabIndex = 24;
            this.comboBoxMatEffects.SelectedIndexChanged += new System.EventHandler(this.comboBoxMatEffects_SelectedIndexChanged);
            // 
            // propertyGridMatEffects
            // 
            this.propertyGridMatEffects.HelpVisible = false;
            this.propertyGridMatEffects.Location = new System.Drawing.Point(6, 46);
            this.propertyGridMatEffects.Name = "propertyGridMatEffects";
            this.propertyGridMatEffects.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridMatEffects.Size = new System.Drawing.Size(312, 147);
            this.propertyGridMatEffects.TabIndex = 23;
            this.propertyGridMatEffects.ToolbarVisible = false;
            // 
            // MaterialEffectEditor
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(520, 402);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.propertyGridTextureInfo);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MaterialEffectEditor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Material Effects Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxMaterials;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Panel panelColor;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PropertyGrid propertyGridTextureInfo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.PropertyGrid propertyGridMatEffects;
        private System.Windows.Forms.ComboBox comboBoxMatEffects;
    }
}