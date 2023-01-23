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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaterialEffectEditor));
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
            this.buttonAddTexture = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxMaterials);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // listBoxMaterials
            // 
            this.listBoxMaterials.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxMaterials.FormattingEnabled = true;
            resources.ApplyResources(this.listBoxMaterials, "listBoxMaterials");
            this.listBoxMaterials.Name = "listBoxMaterials";
            this.listBoxMaterials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxMaterials.SelectedIndexChanged += new System.EventHandler(this.listBoxMaterials_SelectedIndexChanged);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelColor);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // panelColor
            // 
            resources.ApplyResources(this.panelColor, "panelColor");
            this.panelColor.Name = "panelColor";
            this.panelColor.Click += new System.EventHandler(this.panelColor_Click);
            // 
            // propertyGridTextureInfo
            // 
            resources.ApplyResources(this.propertyGridTextureInfo, "propertyGridTextureInfo");
            this.propertyGridTextureInfo.Name = "propertyGridTextureInfo";
            this.propertyGridTextureInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridTextureInfo.ToolbarVisible = false;
            this.propertyGridTextureInfo.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridTextureInfo_PropertyValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBoxMatEffects);
            this.groupBox3.Controls.Add(this.propertyGridMatEffects);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // comboBoxMatEffects
            // 
            this.comboBoxMatEffects.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxMatEffects, "comboBoxMatEffects");
            this.comboBoxMatEffects.Name = "comboBoxMatEffects";
            this.comboBoxMatEffects.SelectedIndexChanged += new System.EventHandler(this.comboBoxMatEffects_SelectedIndexChanged);
            // 
            // propertyGridMatEffects
            // 
            resources.ApplyResources(this.propertyGridMatEffects, "propertyGridMatEffects");
            this.propertyGridMatEffects.Name = "propertyGridMatEffects";
            this.propertyGridMatEffects.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridMatEffects.ToolbarVisible = false;
            // 
            // buttonAddTexture
            // 
            resources.ApplyResources(this.buttonAddTexture, "buttonAddTexture");
            this.buttonAddTexture.Name = "buttonAddTexture";
            this.buttonAddTexture.UseVisualStyleBackColor = true;
            this.buttonAddTexture.Click += new System.EventHandler(this.buttonAddTexture_Click);
            // 
            // MaterialEffectEditor
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonAddTexture);
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
        private System.Windows.Forms.Button buttonAddTexture;
    }
}