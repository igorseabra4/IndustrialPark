namespace IndustrialPark
{
    partial class InternalMultiAssetEditor
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
            labelAssetName = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // propertyGridAsset
            // 
            propertyGridAsset.BackColor = System.Drawing.SystemColors.Control;
            tableLayoutPanel1.SetColumnSpan(propertyGridAsset, 2);
            propertyGridAsset.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGridAsset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            propertyGridAsset.Location = new System.Drawing.Point(0, 20);
            propertyGridAsset.Margin = new System.Windows.Forms.Padding(0);
            propertyGridAsset.Name = "propertyGridAsset";
            propertyGridAsset.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridAsset.Size = new System.Drawing.Size(344, 421);
            propertyGridAsset.TabIndex = 5;
            propertyGridAsset.PropertyValueChanged += propertyGridAsset_PropertyValueChanged;
            // 
            // labelAssetName
            // 
            labelAssetName.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(labelAssetName, 2);
            labelAssetName.Dock = System.Windows.Forms.DockStyle.Left;
            labelAssetName.Location = new System.Drawing.Point(3, 0);
            labelAssetName.Name = "labelAssetName";
            labelAssetName.Size = new System.Drawing.Size(0, 20);
            labelAssetName.TabIndex = 6;
            labelAssetName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(propertyGridAsset, 0, 1);
            tableLayoutPanel1.Controls.Add(labelAssetName, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(344, 441);
            tableLayoutPanel1.TabIndex = 7;
            // 
            // InternalMultiAssetEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(344, 441);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            Name = "InternalMultiAssetEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Multi Asset Data Editor";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridAsset;
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}