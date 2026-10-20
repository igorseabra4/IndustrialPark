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
            propertyGridAsset = new System.Windows.Forms.PropertyGrid();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            buttonHelp = new System.Windows.Forms.Button();
            buttonExport = new System.Windows.Forms.Button();
            buttonImport = new System.Windows.Forms.Button();
            buttonFindCallers = new System.Windows.Forms.Button();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            checkBoxFlipTextures = new System.Windows.Forms.CheckBox();
            checkBoxTransFix = new System.Windows.Forms.CheckBox();
            checkBoxCompress = new System.Windows.Forms.CheckBox();
            checkBoxMipmaps = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // propertyGridAsset
            // 
            propertyGridAsset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGridAsset.BackColor = System.Drawing.SystemColors.Control;
            tableLayoutPanel1.SetColumnSpan(propertyGridAsset, 2);
            propertyGridAsset.HelpVisible = false;
            propertyGridAsset.Location = new System.Drawing.Point(0, 0);
            propertyGridAsset.Margin = new System.Windows.Forms.Padding(0);
            propertyGridAsset.Name = "propertyGridAsset";
            propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridAsset.Size = new System.Drawing.Size(344, 127);
            propertyGridAsset.TabIndex = 5;
            propertyGridAsset.PropertyValueChanged += propertyGridAsset_PropertyValueChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonHelp, 0, 6);
            tableLayoutPanel1.Controls.Add(buttonExport, 0, 5);
            tableLayoutPanel1.Controls.Add(buttonImport, 0, 5);
            tableLayoutPanel1.Controls.Add(propertyGridAsset, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonFindCallers, 1, 6);
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(checkBoxFlipTextures, 1, 3);
            tableLayoutPanel1.Controls.Add(checkBoxTransFix, 1, 4);
            tableLayoutPanel1.Controls.Add(checkBoxCompress, 0, 4);
            tableLayoutPanel1.Controls.Add(checkBoxMipmaps, 0, 3);
            tableLayoutPanel1.Controls.Add(label1, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(344, 441);
            tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonHelp
            // 
            buttonHelp.AutoSize = true;
            buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonHelp.Location = new System.Drawing.Point(3, 415);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(166, 23);
            buttonHelp.TabIndex = 13;
            buttonHelp.Text = "Open Wiki Page";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // buttonExport
            // 
            buttonExport.AutoSize = true;
            buttonExport.Location = new System.Drawing.Point(3, 387);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new System.Drawing.Size(166, 22);
            buttonExport.TabIndex = 10;
            buttonExport.Text = "Export";
            buttonExport.UseVisualStyleBackColor = true;
            buttonExport.Click += buttonExport_Click;
            // 
            // buttonImport
            // 
            buttonImport.AutoSize = true;
            buttonImport.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonImport.Location = new System.Drawing.Point(175, 387);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new System.Drawing.Size(166, 22);
            buttonImport.TabIndex = 11;
            buttonImport.Text = "Import";
            buttonImport.UseVisualStyleBackColor = true;
            buttonImport.Click += buttonImport_Click;
            // 
            // buttonFindCallers
            // 
            buttonFindCallers.AutoSize = true;
            buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonFindCallers.Location = new System.Drawing.Point(175, 415);
            buttonFindCallers.Name = "buttonFindCallers";
            buttonFindCallers.Size = new System.Drawing.Size(166, 23);
            buttonFindCallers.TabIndex = 12;
            buttonFindCallers.Text = "Find Who Targets Me";
            buttonFindCallers.UseVisualStyleBackColor = true;
            buttonFindCallers.Click += buttonFindCallers_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            tableLayoutPanel1.SetColumnSpan(pictureBox1, 2);
            pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureBox1.Location = new System.Drawing.Point(3, 130);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(338, 185);
            pictureBox1.TabIndex = 14;
            pictureBox1.TabStop = false;
            // 
            // checkBoxFlipTextures
            // 
            checkBoxFlipTextures.AutoSize = true;
            checkBoxFlipTextures.Location = new System.Drawing.Point(175, 343);
            checkBoxFlipTextures.Name = "checkBoxFlipTextures";
            checkBoxFlipTextures.Size = new System.Drawing.Size(87, 16);
            checkBoxFlipTextures.TabIndex = 17;
            checkBoxFlipTextures.Text = "Flip Vertically";
            checkBoxFlipTextures.UseVisualStyleBackColor = true;
            // 
            // checkBoxTransFix
            // 
            checkBoxTransFix.AutoSize = true;
            checkBoxTransFix.Location = new System.Drawing.Point(175, 365);
            checkBoxTransFix.Name = "checkBoxTransFix";
            checkBoxTransFix.Size = new System.Drawing.Size(113, 16);
            checkBoxTransFix.TabIndex = 18;
            checkBoxTransFix.Text = "Has Transparency";
            checkBoxTransFix.UseVisualStyleBackColor = true;
            checkBoxTransFix.CheckedChanged += checkBoxTransFix_CheckedChanged;
            // 
            // checkBoxCompress
            // 
            checkBoxCompress.AutoSize = true;
            checkBoxCompress.Checked = true;
            checkBoxCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxCompress.Location = new System.Drawing.Point(3, 365);
            checkBoxCompress.Name = "checkBoxCompress";
            checkBoxCompress.Size = new System.Drawing.Size(72, 16);
            checkBoxCompress.TabIndex = 16;
            checkBoxCompress.Text = "Compress";
            checkBoxCompress.UseVisualStyleBackColor = true;
            checkBoxCompress.CheckedChanged += checkBoxCompress_CheckedChanged;
            // 
            // checkBoxMipmaps
            // 
            checkBoxMipmaps.AutoSize = true;
            checkBoxMipmaps.Checked = true;
            checkBoxMipmaps.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxMipmaps.Dock = System.Windows.Forms.DockStyle.Fill;
            checkBoxMipmaps.Location = new System.Drawing.Point(3, 343);
            checkBoxMipmaps.Name = "checkBoxMipmaps";
            checkBoxMipmaps.Size = new System.Drawing.Size(166, 16);
            checkBoxMipmaps.TabIndex = 15;
            checkBoxMipmaps.Text = "Generate Mipmaps";
            checkBoxMipmaps.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label1, 2);
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(3, 318);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(338, 22);
            label1.TabIndex = 19;
            label1.Text = "Import settings:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InternalTextureEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(344, 441);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            Name = "InternalTextureEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Asset Data Editor";
            FormClosing += InternalTextureEditor_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);

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