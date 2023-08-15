namespace IndustrialPark
{
    partial class InternalTextureEditor
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBoxFlipTextures = new System.Windows.Forms.CheckBox();
            this.checkBoxTransFix = new System.Windows.Forms.CheckBox();
            this.checkBoxCompress = new System.Windows.Forms.CheckBox();
            this.checkBoxMipmaps = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyGridAsset
            // 
            this.propertyGridAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridAsset, 2);
            this.propertyGridAsset.HelpVisible = false;
            this.propertyGridAsset.Location = new System.Drawing.Point(4, 4);
            this.propertyGridAsset.Margin = new System.Windows.Forms.Padding(4);
            this.propertyGridAsset.Name = "propertyGridAsset";
            this.propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridAsset.Size = new System.Drawing.Size(451, 152);
            this.propertyGridAsset.TabIndex = 5;
            this.propertyGridAsset.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridAsset_PropertyValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.buttonExport, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.buttonImport, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridAsset, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindCallers, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxFlipTextures, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxTransFix, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxCompress, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxMipmaps, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(459, 543);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHelp.Location = new System.Drawing.Point(4, 516);
            this.buttonHelp.Margin = new System.Windows.Forms.Padding(4);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(221, 23);
            this.buttonHelp.TabIndex = 13;
            this.buttonHelp.Text = "Open Wiki Page";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonExport
            // 
            this.buttonExport.AutoSize = true;
            this.buttonExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonExport.Location = new System.Drawing.Point(4, 485);
            this.buttonExport.Margin = new System.Windows.Forms.Padding(4);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(221, 23);
            this.buttonExport.TabIndex = 10;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonImport
            // 
            this.buttonImport.AutoSize = true;
            this.buttonImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonImport.Location = new System.Drawing.Point(233, 485);
            this.buttonImport.Margin = new System.Windows.Forms.Padding(4);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(222, 23);
            this.buttonImport.TabIndex = 11;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindCallers.Location = new System.Drawing.Point(233, 516);
            this.buttonFindCallers.Margin = new System.Windows.Forms.Padding(4);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(222, 23);
            this.buttonFindCallers.TabIndex = 12;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 2);
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(4, 164);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(451, 232);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // checkBoxFlipTextures
            // 
            this.checkBoxFlipTextures.AutoSize = true;
            this.checkBoxFlipTextures.Location = new System.Drawing.Point(233, 431);
            this.checkBoxFlipTextures.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxFlipTextures.Name = "checkBoxFlipTextures";
            this.checkBoxFlipTextures.Size = new System.Drawing.Size(109, 19);
            this.checkBoxFlipTextures.TabIndex = 17;
            this.checkBoxFlipTextures.Text = "Flip Vertically";
            this.checkBoxFlipTextures.UseVisualStyleBackColor = true;
            // 
            // checkBoxTransFix
            // 
            this.checkBoxTransFix.AutoSize = true;
            this.checkBoxTransFix.Location = new System.Drawing.Point(233, 458);
            this.checkBoxTransFix.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxTransFix.Name = "checkBoxTransFix";
            this.checkBoxTransFix.Size = new System.Drawing.Size(141, 19);
            this.checkBoxTransFix.TabIndex = 18;
            this.checkBoxTransFix.Text = "Has Transparency";
            this.checkBoxTransFix.UseVisualStyleBackColor = true;
            this.checkBoxTransFix.CheckedChanged += new System.EventHandler(this.checkBoxTransFix_CheckedChanged);
            // 
            // checkBoxCompress
            // 
            this.checkBoxCompress.AutoSize = true;
            this.checkBoxCompress.Checked = true;
            this.checkBoxCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCompress.Location = new System.Drawing.Point(4, 458);
            this.checkBoxCompress.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxCompress.Name = "checkBoxCompress";
            this.checkBoxCompress.Size = new System.Drawing.Size(91, 19);
            this.checkBoxCompress.TabIndex = 16;
            this.checkBoxCompress.Text = "Compress";
            this.checkBoxCompress.UseVisualStyleBackColor = true;
            this.checkBoxCompress.CheckedChanged += new System.EventHandler(this.checkBoxCompress_CheckedChanged);
            // 
            // checkBoxMipmaps
            // 
            this.checkBoxMipmaps.AutoSize = true;
            this.checkBoxMipmaps.Checked = true;
            this.checkBoxMipmaps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMipmaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxMipmaps.Location = new System.Drawing.Point(4, 431);
            this.checkBoxMipmaps.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxMipmaps.Name = "checkBoxMipmaps";
            this.checkBoxMipmaps.Size = new System.Drawing.Size(221, 19);
            this.checkBoxMipmaps.TabIndex = 15;
            this.checkBoxMipmaps.Text = "Generate Mipmaps";
            this.checkBoxMipmaps.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 400);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(451, 27);
            this.label1.TabIndex = 19;
            this.label1.Text = "Import settings:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InternalTextureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 543);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "InternalTextureEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalTextureEditor_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridAsset;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox checkBoxMipmaps;
        private System.Windows.Forms.CheckBox checkBoxCompress;
        private System.Windows.Forms.CheckBox checkBoxTransFix;
        private System.Windows.Forms.CheckBox checkBoxFlipTextures;
        private System.Windows.Forms.Label label1;
    }
}