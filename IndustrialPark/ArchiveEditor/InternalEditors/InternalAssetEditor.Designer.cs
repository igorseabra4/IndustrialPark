namespace IndustrialPark
{
    partial class InternalAssetEditor
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
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            assetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tableLayoutPanel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // propertyGridAsset
            // 
            propertyGridAsset.BackColor = System.Drawing.SystemColors.Control;
            tableLayoutPanel1.SetColumnSpan(propertyGridAsset, 2);
            propertyGridAsset.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGridAsset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            propertyGridAsset.Location = new System.Drawing.Point(0, 0);
            propertyGridAsset.Margin = new System.Windows.Forms.Padding(0);
            propertyGridAsset.Name = "propertyGridAsset";
            propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridAsset.Size = new System.Drawing.Size(344, 417);
            propertyGridAsset.TabIndex = 5;
            propertyGridAsset.PropertyValueChanged += propertyGridAsset_PropertyValueChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(propertyGridAsset, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(344, 417);
            tableLayoutPanel1.TabIndex = 7;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { assetToolStripMenuItem, toolsToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(344, 24);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // assetToolStripMenuItem
            // 
            assetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { editHeaderToolStripMenuItem, viewToolStripMenuItem, copyToolStripMenuItem, removeToolStripMenuItem });
            assetToolStripMenuItem.Name = "assetToolStripMenuItem";
            assetToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            assetToolStripMenuItem.Text = "Edit";
            // 
            // editHeaderToolStripMenuItem
            // 
            editHeaderToolStripMenuItem.Name = "editHeaderToolStripMenuItem";
            editHeaderToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            editHeaderToolStripMenuItem.Text = "Edit Header";
            editHeaderToolStripMenuItem.Click += editHeaderToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            viewToolStripMenuItem.Text = "View Asset";
            viewToolStripMenuItem.Click += viewToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            copyToolStripMenuItem.Text = "Copy Asset";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // removeToolStripMenuItem
            // 
            removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            removeToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            removeToolStripMenuItem.Text = "Delete Asset";
            removeToolStripMenuItem.Click += removeToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // InternalAssetEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(344, 441);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "InternalAssetEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Asset Data Editor";
            FormClosing += InternalAssetEditor_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridAsset;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem assetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
    }
}