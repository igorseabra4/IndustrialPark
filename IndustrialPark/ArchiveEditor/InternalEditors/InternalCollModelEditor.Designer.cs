namespace IndustrialPark
{
    partial class InternalCollModelEditor
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
            this.groupBoxCollTree = new System.Windows.Forms.GroupBox();
            this.removeCollTreeButton = new System.Windows.Forms.Button();
            this.buildCollTreeButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.groupBoxStats = new System.Windows.Forms.GroupBox();
            this.numTrisLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numVertsLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxCollTree.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxStats.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCollTree
            // 
            this.groupBoxCollTree.Controls.Add(this.removeCollTreeButton);
            this.groupBoxCollTree.Controls.Add(this.buildCollTreeButton);
            this.groupBoxCollTree.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCollTree.Name = "groupBoxCollTree";
            this.groupBoxCollTree.Size = new System.Drawing.Size(110, 79);
            this.groupBoxCollTree.TabIndex = 0;
            this.groupBoxCollTree.TabStop = false;
            this.groupBoxCollTree.Text = "Collision Tree";
            // 
            // removeCollTreeButton
            // 
            this.removeCollTreeButton.Location = new System.Drawing.Point(6, 48);
            this.removeCollTreeButton.Name = "removeCollTreeButton";
            this.removeCollTreeButton.Size = new System.Drawing.Size(100, 23);
            this.removeCollTreeButton.TabIndex = 1;
            this.removeCollTreeButton.Text = "Remove";
            this.removeCollTreeButton.UseVisualStyleBackColor = true;
            this.removeCollTreeButton.Click += new System.EventHandler(this.removeCollTreeButton_Click);
            // 
            // buildCollTreeButton
            // 
            this.buildCollTreeButton.Location = new System.Drawing.Point(6, 19);
            this.buildCollTreeButton.Name = "buildCollTreeButton";
            this.buildCollTreeButton.Size = new System.Drawing.Size(100, 23);
            this.buildCollTreeButton.TabIndex = 1;
            this.buildCollTreeButton.Text = "Build";
            this.buildCollTreeButton.UseVisualStyleBackColor = true;
            this.buildCollTreeButton.Click += new System.EventHandler(this.buildCollTreeButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.exportButton);
            this.groupBox1.Controls.Add(this.importButton);
            this.groupBox1.Location = new System.Drawing.Point(130, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(110, 79);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import/Export";
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(6, 48);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(97, 23);
            this.exportButton.TabIndex = 2;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(6, 19);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(97, 23);
            this.importButton.TabIndex = 0;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // groupBoxStats
            // 
            this.groupBoxStats.Controls.Add(this.numTrisLabel);
            this.groupBoxStats.Controls.Add(this.label2);
            this.groupBoxStats.Controls.Add(this.numVertsLabel);
            this.groupBoxStats.Controls.Add(this.label1);
            this.groupBoxStats.Location = new System.Drawing.Point(12, 97);
            this.groupBoxStats.Name = "groupBoxStats";
            this.groupBoxStats.Size = new System.Drawing.Size(228, 58);
            this.groupBoxStats.TabIndex = 2;
            this.groupBoxStats.TabStop = false;
            this.groupBoxStats.Text = "Statistics";
            // 
            // numTrisLabel
            // 
            this.numTrisLabel.AutoSize = true;
            this.numTrisLabel.Location = new System.Drawing.Point(57, 34);
            this.numTrisLabel.Name = "numTrisLabel";
            this.numTrisLabel.Size = new System.Drawing.Size(13, 13);
            this.numTrisLabel.TabIndex = 3;
            this.numTrisLabel.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Triangles:";
            // 
            // numVertsLabel
            // 
            this.numVertsLabel.AutoSize = true;
            this.numVertsLabel.Location = new System.Drawing.Point(51, 16);
            this.numVertsLabel.Name = "numVertsLabel";
            this.numVertsLabel.Size = new System.Drawing.Size(13, 13);
            this.numVertsLabel.TabIndex = 1;
            this.numVertsLabel.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Vertices:";
            // 
            // InternalCollModelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 170);
            this.Controls.Add(this.groupBoxStats);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxCollTree);
            this.Name = "InternalCollModelEditor";
            this.ShowIcon = false;
            this.Text = "Collision Model";
            this.TopMost = true;
            this.groupBoxCollTree.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBoxStats.ResumeLayout(false);
            this.groupBoxStats.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCollTree;
        private System.Windows.Forms.Button removeCollTreeButton;
        private System.Windows.Forms.Button buildCollTreeButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.GroupBox groupBoxStats;
        private System.Windows.Forms.Label numVertsLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label numTrisLabel;
        private System.Windows.Forms.Label label2;
    }
}