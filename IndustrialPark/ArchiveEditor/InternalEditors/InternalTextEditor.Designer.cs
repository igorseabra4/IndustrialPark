namespace IndustrialPark
{
    partial class InternalTextEditor
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
            richTextBoxAssetText = new System.Windows.Forms.RichTextBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            buttonHelp = new System.Windows.Forms.Button();
            buttonFindCallers = new System.Windows.Forms.Button();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openWikiPageviewFormatTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            findWhoTargetsMeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tableLayoutPanel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBoxAssetText
            // 
            richTextBoxAssetText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel1.SetColumnSpan(richTextBoxAssetText, 2);
            richTextBoxAssetText.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxAssetText.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            richTextBoxAssetText.Location = new System.Drawing.Point(0, 0);
            richTextBoxAssetText.Margin = new System.Windows.Forms.Padding(0);
            richTextBoxAssetText.Name = "richTextBoxAssetText";
            richTextBoxAssetText.Size = new System.Drawing.Size(624, 229);
            richTextBoxAssetText.TabIndex = 7;
            richTextBoxAssetText.Text = "";
            richTextBoxAssetText.TextChanged += richTextBoxAssetText_TextChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonHelp, 0, 1);
            tableLayoutPanel1.Controls.Add(buttonFindCallers, 1, 1);
            tableLayoutPanel1.Controls.Add(richTextBoxAssetText, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(624, 257);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonHelp
            // 
            buttonHelp.AutoSize = true;
            buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonHelp.Location = new System.Drawing.Point(3, 232);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(306, 22);
            buttonHelp.TabIndex = 18;
            buttonHelp.Text = "Open wiki page (view format tags)";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // buttonFindCallers
            // 
            buttonFindCallers.AutoSize = true;
            buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonFindCallers.Location = new System.Drawing.Point(315, 232);
            buttonFindCallers.Name = "buttonFindCallers";
            buttonFindCallers.Size = new System.Drawing.Size(306, 22);
            buttonFindCallers.TabIndex = 8;
            buttonFindCallers.Text = "Find who targets me";
            buttonFindCallers.UseVisualStyleBackColor = true;
            buttonFindCallers.Click += buttonFindCallers_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { editToolStripMenuItem, toolsToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(624, 24);
            menuStrip1.TabIndex = 9;
            menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { editHeaderToolStripMenuItem, copyAssetToolStripMenuItem, deleteAssetToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // editHeaderToolStripMenuItem
            // 
            editHeaderToolStripMenuItem.Name = "editHeaderToolStripMenuItem";
            editHeaderToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            editHeaderToolStripMenuItem.Text = "Edit Header";
            editHeaderToolStripMenuItem.Click += editHeaderToolStripMenuItem_Click;
            // 
            // copyAssetToolStripMenuItem
            // 
            copyAssetToolStripMenuItem.Name = "copyAssetToolStripMenuItem";
            copyAssetToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            copyAssetToolStripMenuItem.Text = "Copy Asset";
            copyAssetToolStripMenuItem.Click += copyAssetToolStripMenuItem_Click;
            // 
            // deleteAssetToolStripMenuItem
            // 
            deleteAssetToolStripMenuItem.Name = "deleteAssetToolStripMenuItem";
            deleteAssetToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            deleteAssetToolStripMenuItem.Text = "Delete Asset";
            deleteAssetToolStripMenuItem.Click += deleteAssetToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openWikiPageviewFormatTagsToolStripMenuItem, findWhoTargetsMeToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // openWikiPageviewFormatTagsToolStripMenuItem
            // 
            openWikiPageviewFormatTagsToolStripMenuItem.Name = "openWikiPageviewFormatTagsToolStripMenuItem";
            openWikiPageviewFormatTagsToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            openWikiPageviewFormatTagsToolStripMenuItem.Text = "Open wiki page (view format tags)";
            openWikiPageviewFormatTagsToolStripMenuItem.Click += buttonHelp_Click;
            // 
            // findWhoTargetsMeToolStripMenuItem
            // 
            findWhoTargetsMeToolStripMenuItem.Name = "findWhoTargetsMeToolStripMenuItem";
            findWhoTargetsMeToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            findWhoTargetsMeToolStripMenuItem.Text = "Find who targets me";
            findWhoTargetsMeToolStripMenuItem.Click += buttonFindCallers_Click;
            // 
            // InternalTextEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(624, 281);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "InternalTextEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Asset Data Editor";
            FormClosing += InternalTextEditor_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBoxAssetText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAssetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWikiPageviewFormatTagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findWhoTargetsMeToolStripMenuItem;
    }
}