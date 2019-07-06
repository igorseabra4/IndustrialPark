namespace IndustrialPark
{
    partial class InternalPlatEditor
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.propertyGrid_PlatSpecific = new System.Windows.Forms.PropertyGrid();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.propertyGrid_Motion = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridAsset
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridAsset, 2);
            this.propertyGridAsset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridAsset.Location = new System.Drawing.Point(3, 23);
            this.propertyGridAsset.Name = "propertyGridAsset";
            this.propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridAsset.Size = new System.Drawing.Size(324, 190);
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
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.propertyGrid_PlatSpecific, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridAsset, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAssetName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindCallers, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.propertyGrid_Motion, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 539);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHelp.Location = new System.Drawing.Point(3, 513);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(159, 23);
            this.buttonHelp.TabIndex = 16;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // propertyGrid_PlatSpecific
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGrid_PlatSpecific, 2);
            this.propertyGrid_PlatSpecific.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_PlatSpecific.Location = new System.Drawing.Point(3, 219);
            this.propertyGrid_PlatSpecific.Name = "propertyGrid_PlatSpecific";
            this.propertyGrid_PlatSpecific.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid_PlatSpecific.Size = new System.Drawing.Size(324, 141);
            this.propertyGrid_PlatSpecific.TabIndex = 8;
            this.propertyGrid_PlatSpecific.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PlatSpecific_PropertyValueChanged);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindCallers.Location = new System.Drawing.Point(168, 513);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(159, 23);
            this.buttonFindCallers.TabIndex = 7;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // propertyGrid_Motion
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGrid_Motion, 2);
            this.propertyGrid_Motion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_Motion.Location = new System.Drawing.Point(3, 366);
            this.propertyGrid_Motion.Name = "propertyGrid_Motion";
            this.propertyGrid_Motion.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid_Motion.Size = new System.Drawing.Size(324, 141);
            this.propertyGrid_Motion.TabIndex = 17;
            this.propertyGrid_Motion.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_Motion_PropertyValueChanged);
            // 
            // InternalPlatEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 539);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "InternalPlatEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalAssetEditor_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridAsset;
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.PropertyGrid propertyGrid_PlatSpecific;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.PropertyGrid propertyGrid_Motion;
    }
}