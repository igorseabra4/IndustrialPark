namespace IndustrialPark
{
    partial class ViewControls
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
            dataGridView1 = new System.Windows.Forms.DataGridView();
            textBoxSearch = new System.Windows.Forms.TextBox();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            buttonDefaults = new System.Windows.Forms.Button();
            labelEditHelp = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new System.Drawing.Point(12, 38);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new System.Drawing.Size(600, 478);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
            // 
            // textBoxSearch
            // 
            textBoxSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxSearch.Location = new System.Drawing.Point(12, 12);
            textBoxSearch.Name = "textBoxSearch";
            textBoxSearch.PlaceholderText = "Search controls...";
            textBoxSearch.Size = new System.Drawing.Size(600, 20);
            textBoxSearch.TabIndex = 1;
            textBoxSearch.TextChanged += textBoxSearch_TextChanged;
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new System.Drawing.Point(12, 523);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new System.Drawing.Size(169, 13);
            linkLabel1.TabIndex = 2;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Click here to check out the tutorial";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // buttonDefaults
            // 
            buttonDefaults.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonDefaults.Location = new System.Drawing.Point(537, 519);
            buttonDefaults.Name = "buttonDefaults";
            buttonDefaults.Size = new System.Drawing.Size(75, 20);
            buttonDefaults.TabIndex = 3;
            buttonDefaults.Text = "Reset All";
            buttonDefaults.UseVisualStyleBackColor = true;
            buttonDefaults.Click += buttonDefaults_Click;
            // 
            // labelEditHelp
            // 
            labelEditHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            labelEditHelp.AutoSize = true;
            labelEditHelp.Location = new System.Drawing.Point(257, 523);
            labelEditHelp.Name = "labelEditHelp";
            labelEditHelp.Size = new System.Drawing.Size(274, 13);
            labelEditHelp.TabIndex = 4;
            labelEditHelp.Text = "Double-click blue commands to edit, middle click to reset";
            // 
            // ViewControls
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(624, 541);
            Controls.Add(labelEditHelp);
            Controls.Add(buttonDefaults);
            Controls.Add(linkLabel1);
            Controls.Add(textBoxSearch);
            Controls.Add(dataGridView1);
            MinimumSize = new System.Drawing.Size(280, 280);
            Name = "ViewControls";
            ShowIcon = false;
            Text = "View Controls";
            KeyDown += ViewControls_KeyDown;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button buttonDefaults;
        private System.Windows.Forms.Label labelEditHelp;
    }
}