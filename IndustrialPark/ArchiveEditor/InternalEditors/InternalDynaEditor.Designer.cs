namespace IndustrialPark
{
    partial class InternalDynaEditor
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
            this.propertyGridAsset = new System.Windows.Forms.PropertyGrid();
            this.labelAssetName = new System.Windows.Forms.Label();
            this.propertyGridDynaType = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridAsset
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridAsset, 2);
            this.propertyGridAsset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridAsset.HelpVisible = false;
            this.propertyGridAsset.Location = new System.Drawing.Point(3, 23);
            this.propertyGridAsset.Name = "propertyGridAsset";
            this.propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridAsset.Size = new System.Drawing.Size(338, 274);
            this.propertyGridAsset.TabIndex = 5;
            this.propertyGridAsset.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridAsset_PropertyValueChanged);
            // 
            // labelAssetName
            // 
            this.labelAssetName.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelAssetName, 2);
            this.labelAssetName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelAssetName.Location = new System.Drawing.Point(3, 0);
            this.labelAssetName.Name = "labelAssetName";
            this.labelAssetName.Size = new System.Drawing.Size(0, 20);
            this.labelAssetName.TabIndex = 6;
            this.labelAssetName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // propertyGridDynaType
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridDynaType, 2);
            this.propertyGridDynaType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridDynaType.HelpVisible = false;
            this.propertyGridDynaType.Location = new System.Drawing.Point(3, 303);
            this.propertyGridDynaType.Name = "propertyGridDynaType";
            this.propertyGridDynaType.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridDynaType.Size = new System.Drawing.Size(338, 247);
            this.propertyGridDynaType.TabIndex = 7;
            this.propertyGridDynaType.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridDynaType_PropertyValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindCallers, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridAsset, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAssetName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridDynaType, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(344, 581);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHelp.Location = new System.Drawing.Point(3, 556);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(166, 22);
            this.buttonHelp.TabIndex = 14;
            this.buttonHelp.Text = "Open Wiki Page";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindCallers.Location = new System.Drawing.Point(175, 556);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(166, 22);
            this.buttonFindCallers.TabIndex = 8;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // InternalDynaEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 581);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "InternalDynaEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalDynaEditor_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridAsset;
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.PropertyGrid propertyGridDynaType;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonHelp;
    }
}