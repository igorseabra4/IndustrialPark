namespace IndustrialPark
{
    partial class InternalShrapnelEditor
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
            this.buttonAdd9 = new System.Windows.Forms.Button();
            this.buttonAdd8 = new System.Windows.Forms.Button();
            this.buttonAdd6 = new System.Windows.Forms.Button();
            this.buttonFindWhoTargetsMe = new System.Windows.Forms.Button();
            this.buttonAdd4 = new System.Windows.Forms.Button();
            this.buttonAdd3 = new System.Windows.Forms.Button();
            this.buttonAdd5 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridAsset
            // 
            this.propertyGridAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridAsset, 3);
            this.propertyGridAsset.HelpVisible = false;
            this.propertyGridAsset.Location = new System.Drawing.Point(3, 23);
            this.propertyGridAsset.Name = "propertyGridAsset";
            this.propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridAsset.Size = new System.Drawing.Size(311, 362);
            this.propertyGridAsset.TabIndex = 5;
            this.propertyGridAsset.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridAsset_PropertyValueChanged);
            // 
            // labelAssetName
            // 
            this.labelAssetName.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelAssetName, 3);
            this.labelAssetName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelAssetName.Location = new System.Drawing.Point(3, 0);
            this.labelAssetName.Name = "labelAssetName";
            this.labelAssetName.Size = new System.Drawing.Size(0, 20);
            this.labelAssetName.TabIndex = 6;
            this.labelAssetName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd9, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd8, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd6, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindWhoTargetsMe, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridAsset, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAssetName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd5, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(317, 472);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHelp.Location = new System.Drawing.Point(3, 447);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(99, 22);
            this.buttonHelp.TabIndex = 21;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonAdd9
            // 
            this.buttonAdd9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAdd9.Location = new System.Drawing.Point(213, 419);
            this.buttonAdd9.Name = "buttonAdd9";
            this.buttonAdd9.Size = new System.Drawing.Size(101, 22);
            this.buttonAdd9.TabIndex = 20;
            this.buttonAdd9.Text = "Add Type 9";
            this.buttonAdd9.UseVisualStyleBackColor = true;
            this.buttonAdd9.Click += new System.EventHandler(this.buttonAdd9_Click);
            // 
            // buttonAdd8
            // 
            this.buttonAdd8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAdd8.Location = new System.Drawing.Point(108, 419);
            this.buttonAdd8.Name = "buttonAdd8";
            this.buttonAdd8.Size = new System.Drawing.Size(99, 22);
            this.buttonAdd8.TabIndex = 19;
            this.buttonAdd8.Text = "Add Type 8";
            this.buttonAdd8.UseVisualStyleBackColor = true;
            this.buttonAdd8.Click += new System.EventHandler(this.buttonAdd8_Click);
            // 
            // buttonAdd6
            // 
            this.buttonAdd6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAdd6.Location = new System.Drawing.Point(3, 419);
            this.buttonAdd6.Name = "buttonAdd6";
            this.buttonAdd6.Size = new System.Drawing.Size(99, 22);
            this.buttonAdd6.TabIndex = 18;
            this.buttonAdd6.Text = "Add Type 6";
            this.buttonAdd6.UseVisualStyleBackColor = true;
            this.buttonAdd6.Click += new System.EventHandler(this.buttonAdd6_Click);
            // 
            // buttonFindWhoTargetsMe
            // 
            this.buttonFindWhoTargetsMe.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.buttonFindWhoTargetsMe, 2);
            this.buttonFindWhoTargetsMe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindWhoTargetsMe.Location = new System.Drawing.Point(108, 447);
            this.buttonFindWhoTargetsMe.Name = "buttonFindWhoTargetsMe";
            this.buttonFindWhoTargetsMe.Size = new System.Drawing.Size(206, 22);
            this.buttonFindWhoTargetsMe.TabIndex = 17;
            this.buttonFindWhoTargetsMe.Text = "Find Who Targets Me";
            this.buttonFindWhoTargetsMe.UseVisualStyleBackColor = true;
            this.buttonFindWhoTargetsMe.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // buttonAdd4
            // 
            this.buttonAdd4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAdd4.Location = new System.Drawing.Point(108, 391);
            this.buttonAdd4.Name = "buttonAdd4";
            this.buttonAdd4.Size = new System.Drawing.Size(99, 22);
            this.buttonAdd4.TabIndex = 9;
            this.buttonAdd4.Text = "Add Type 4";
            this.buttonAdd4.UseVisualStyleBackColor = true;
            this.buttonAdd4.Click += new System.EventHandler(this.buttonAdd4_Click);
            // 
            // buttonAdd3
            // 
            this.buttonAdd3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAdd3.Location = new System.Drawing.Point(3, 391);
            this.buttonAdd3.Name = "buttonAdd3";
            this.buttonAdd3.Size = new System.Drawing.Size(99, 22);
            this.buttonAdd3.TabIndex = 8;
            this.buttonAdd3.Text = "Add Type 3";
            this.buttonAdd3.UseVisualStyleBackColor = true;
            this.buttonAdd3.Click += new System.EventHandler(this.buttonAdd3_Click);
            // 
            // buttonAdd5
            // 
            this.buttonAdd5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAdd5.Location = new System.Drawing.Point(213, 391);
            this.buttonAdd5.Name = "buttonAdd5";
            this.buttonAdd5.Size = new System.Drawing.Size(101, 22);
            this.buttonAdd5.TabIndex = 10;
            this.buttonAdd5.Text = "Add Type 5";
            this.buttonAdd5.UseVisualStyleBackColor = true;
            this.buttonAdd5.Click += new System.EventHandler(this.buttonAdd5_Click);
            // 
            // InternalShrapnelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 472);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "InternalShrapnelEditor";
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
        private System.Windows.Forms.Button buttonAdd4;
        private System.Windows.Forms.Button buttonAdd3;
        private System.Windows.Forms.Button buttonAdd5;
        private System.Windows.Forms.Button buttonFindWhoTargetsMe;
        private System.Windows.Forms.Button buttonAdd9;
        private System.Windows.Forms.Button buttonAdd8;
        private System.Windows.Forms.Button buttonAdd6;
        private System.Windows.Forms.Button buttonHelp;
    }
}