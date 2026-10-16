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
            groupBox1 = new System.Windows.Forms.GroupBox();
            listBoxMaterials = new System.Windows.Forms.ListBox();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            groupBox4 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            panelColor = new System.Windows.Forms.Panel();
            propertyGridTextureInfo = new System.Windows.Forms.PropertyGrid();
            groupBox3 = new System.Windows.Forms.GroupBox();
            comboBoxMatEffects = new System.Windows.Forms.ComboBox();
            propertyGridMatEffects = new System.Windows.Forms.PropertyGrid();
            buttonAddTexture = new System.Windows.Forms.Button();
            buttonSelectSimilar = new System.Windows.Forms.Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(listBoxMaterials);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // listBoxMaterials
            // 
            listBoxMaterials.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBoxMaterials.FormattingEnabled = true;
            resources.ApplyResources(listBoxMaterials, "listBoxMaterials");
            listBoxMaterials.Name = "listBoxMaterials";
            listBoxMaterials.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            listBoxMaterials.SelectedIndexChanged += listBoxMaterials_SelectedIndexChanged;
            // 
            // buttonOK
            // 
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(buttonOK, "buttonOK");
            buttonOK.Name = "buttonOK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(buttonCancel, "buttonCancel");
            buttonCancel.Name = "buttonCancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // groupBox4
            // 
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(panelColor);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // panelColor
            // 
            resources.ApplyResources(panelColor, "panelColor");
            panelColor.Name = "panelColor";
            panelColor.TabStop = true;
            panelColor.Click += panelColor_Click;
            // 
            // propertyGridTextureInfo
            // 
            propertyGridTextureInfo.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(propertyGridTextureInfo, "propertyGridTextureInfo");
            propertyGridTextureInfo.Name = "propertyGridTextureInfo";
            propertyGridTextureInfo.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            propertyGridTextureInfo.ToolbarVisible = false;
            propertyGridTextureInfo.PropertyValueChanged += propertyGridTextureInfo_PropertyValueChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(comboBoxMatEffects);
            groupBox3.Controls.Add(propertyGridMatEffects);
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // comboBoxMatEffects
            // 
            comboBoxMatEffects.FormattingEnabled = true;
            resources.ApplyResources(comboBoxMatEffects, "comboBoxMatEffects");
            comboBoxMatEffects.Name = "comboBoxMatEffects";
            comboBoxMatEffects.SelectedIndexChanged += comboBoxMatEffects_SelectedIndexChanged;
            // 
            // propertyGridMatEffects
            // 
            propertyGridMatEffects.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(propertyGridMatEffects, "propertyGridMatEffects");
            propertyGridMatEffects.Name = "propertyGridMatEffects";
            propertyGridMatEffects.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            propertyGridMatEffects.ToolbarVisible = false;
            // 
            // buttonAddTexture
            // 
            resources.ApplyResources(buttonAddTexture, "buttonAddTexture");
            buttonAddTexture.Name = "buttonAddTexture";
            buttonAddTexture.UseVisualStyleBackColor = true;
            buttonAddTexture.Click += buttonAddTexture_Click;
            // 
            // buttonSelectSimilar
            // 
            resources.ApplyResources(buttonSelectSimilar, "buttonSelectSimilar");
            buttonSelectSimilar.Name = "buttonSelectSimilar";
            buttonSelectSimilar.UseVisualStyleBackColor = true;
            buttonSelectSimilar.Click += buttonSelectSimilar_Click;
            buttonSelectSimilar.MouseHover += buttonSelectSimilar_MouseHover;
            // 
            // MaterialEffectEditor
            // 
            AcceptButton = buttonOK;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            Controls.Add(buttonSelectSimilar);
            Controls.Add(buttonAddTexture);
            Controls.Add(groupBox3);
            Controls.Add(propertyGridTextureInfo);
            Controls.Add(groupBox2);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
            Controls.Add(groupBox4);
            Controls.Add(groupBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "MaterialEffectEditor";
            ShowIcon = false;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);

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
        private System.Windows.Forms.Button buttonSelectSimilar;
    }
}