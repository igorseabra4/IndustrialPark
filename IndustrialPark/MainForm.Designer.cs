using System.Windows.Forms;

namespace IndustrialPark
{
    partial class MainForm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noCullingCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BackgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.renderPanel = new System.Windows.Forms.Panel();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHIPHOPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1263, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewConfigToolStripMenuItem,
            this.toolStripSeparator1,
            this.noCullingCToolStripMenuItem,
            this.wireframeFToolStripMenuItem,
            this.BackgroundColorToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // noCullingCToolStripMenuItem
            // 
            this.noCullingCToolStripMenuItem.Name = "noCullingCToolStripMenuItem";
            this.noCullingCToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.noCullingCToolStripMenuItem.Text = "No Culling (C)";
            this.noCullingCToolStripMenuItem.Click += new System.EventHandler(this.noCullingCToolStripMenuItem_Click);
            // 
            // wireframeFToolStripMenuItem
            // 
            this.wireframeFToolStripMenuItem.Name = "wireframeFToolStripMenuItem";
            this.wireframeFToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.wireframeFToolStripMenuItem.Text = "Wireframe (F)";
            this.wireframeFToolStripMenuItem.Click += new System.EventHandler(this.wireframeFToolStripMenuItem_Click);
            // 
            // BackgroundColorToolStripMenuItem
            // 
            this.BackgroundColorToolStripMenuItem.Name = "BackgroundColorToolStripMenuItem";
            this.BackgroundColorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.BackgroundColorToolStripMenuItem.Text = "Background Color...";
            this.BackgroundColorToolStripMenuItem.Click += new System.EventHandler(this.BackgroundColorToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 816);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1263, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // renderPanel
            // 
            this.renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel.Location = new System.Drawing.Point(0, 24);
            this.renderPanel.Name = "renderPanel";
            this.renderPanel.Size = new System.Drawing.Size(1263, 792);
            this.renderPanel.TabIndex = 4;
            this.renderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveControl);
            this.renderPanel.Resize += new System.EventHandler(this.ResetMouseCenter);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openHIPHOPToolStripMenuItem,
            this.exportTexturesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openHIPHOPToolStripMenuItem
            // 
            this.openHIPHOPToolStripMenuItem.Name = "openHIPHOPToolStripMenuItem";
            this.openHIPHOPToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openHIPHOPToolStripMenuItem.Text = "Open HIP/HOP...";
            this.openHIPHOPToolStripMenuItem.Click += new System.EventHandler(this.openHIPHOPToolStripMenuItem_Click);
            // 
            // exportTexturesToolStripMenuItem
            // 
            this.exportTexturesToolStripMenuItem.Name = "exportTexturesToolStripMenuItem";
            this.exportTexturesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportTexturesToolStripMenuItem.Text = "Export Textures";
            this.exportTexturesToolStripMenuItem.Click += new System.EventHandler(this.exportTexturesToolStripMenuItem_Click);
            // 
            // viewConfigToolStripMenuItem
            // 
            this.viewConfigToolStripMenuItem.Name = "viewConfigToolStripMenuItem";
            this.viewConfigToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.viewConfigToolStripMenuItem.Text = "View Config (F1)";
            this.viewConfigToolStripMenuItem.Click += new System.EventHandler(this.viewConfigToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 838);
            this.Controls.Add(this.renderPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Industrial Park";
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseMoveControl);
            this.Move += new System.EventHandler(this.ResetMouseCenter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel renderPanel;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem noCullingCToolStripMenuItem;
        private ToolStripMenuItem wireframeFToolStripMenuItem;
        private ToolStripMenuItem BackgroundColorToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openHIPHOPToolStripMenuItem;
        private ToolStripMenuItem exportTexturesToolStripMenuItem;
        private ToolStripMenuItem viewConfigToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
    }
}

