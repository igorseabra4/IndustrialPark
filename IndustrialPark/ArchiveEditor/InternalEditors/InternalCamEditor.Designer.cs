namespace IndustrialPark
{
    partial class InternalCamEditor
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
            this.buttonGetDir = new System.Windows.Forms.Button();
            this.buttonGetPos = new System.Windows.Forms.Button();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.propertyGridCamSpecific = new System.Windows.Forms.PropertyGrid();
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
            this.propertyGridAsset.Size = new System.Drawing.Size(324, 339);
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
            this.tableLayoutPanel1.Controls.Add(this.buttonGetDir, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonGetPos, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridAsset, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAssetName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindCallers, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridCamSpecific, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 77F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 525);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHelp.Location = new System.Drawing.Point(3, 499);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(159, 23);
            this.buttonHelp.TabIndex = 13;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonGetDir
            // 
            this.buttonGetDir.AutoSize = true;
            this.buttonGetDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonGetDir.Location = new System.Drawing.Point(3, 471);
            this.buttonGetDir.Name = "buttonGetDir";
            this.buttonGetDir.Size = new System.Drawing.Size(159, 22);
            this.buttonGetDir.TabIndex = 10;
            this.buttonGetDir.Text = "Get View Direction";
            this.buttonGetDir.UseVisualStyleBackColor = true;
            this.buttonGetDir.Click += new System.EventHandler(this.buttonGetDir_Click);
            // 
            // buttonGetPos
            // 
            this.buttonGetPos.AutoSize = true;
            this.buttonGetPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonGetPos.Location = new System.Drawing.Point(168, 471);
            this.buttonGetPos.Name = "buttonGetPos";
            this.buttonGetPos.Size = new System.Drawing.Size(159, 22);
            this.buttonGetPos.TabIndex = 11;
            this.buttonGetPos.Text = "Get View Position";
            this.buttonGetPos.UseVisualStyleBackColor = true;
            this.buttonGetPos.Click += new System.EventHandler(this.buttonGetPos_Click);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindCallers.Location = new System.Drawing.Point(168, 499);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(159, 23);
            this.buttonFindCallers.TabIndex = 12;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // propertyGridCamSpecific
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridCamSpecific, 2);
            this.propertyGridCamSpecific.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridCamSpecific.HelpVisible = false;
            this.propertyGridCamSpecific.Location = new System.Drawing.Point(3, 368);
            this.propertyGridCamSpecific.Name = "propertyGridCamSpecific";
            this.propertyGridCamSpecific.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridCamSpecific.Size = new System.Drawing.Size(324, 97);
            this.propertyGridCamSpecific.TabIndex = 14;
            this.propertyGridCamSpecific.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridCamSpecific_PropertyValueChanged);
            // 
            // InternalCamEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 525);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "InternalCamEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalCamEditor_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridAsset;
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonGetDir;
        private System.Windows.Forms.Button buttonGetPos;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.PropertyGrid propertyGridCamSpecific;
    }
}