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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archiveEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTextureFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.noCullingCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BackgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.levelModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bUTNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dSTRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRKRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mVPTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pKUPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pLATToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pLYRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sIMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tRIGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vILToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.renderPanel = new System.Windows.Forms.Panel();
            this.bOULToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archiveEditorToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.displayToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1263, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archiveEditorToolStripMenuItem
            // 
            this.archiveEditorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripSeparator3});
            this.archiveEditorToolStripMenuItem.Name = "archiveEditorToolStripMenuItem";
            this.archiveEditorToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.archiveEditorToolStripMenuItem.Text = "Archive Editor";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.newToolStripMenuItem.Text = "Open New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(127, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewConfigToolStripMenuItem,
            this.addTextureFolderToolStripMenuItem,
            this.clearTexturesToolStripMenuItem,
            this.toolStripSeparator1,
            this.noCullingCToolStripMenuItem,
            this.wireframeFToolStripMenuItem,
            this.BackgroundColorToolStripMenuItem,
            this.toolStripSeparator2,
            this.aboutToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // viewConfigToolStripMenuItem
            // 
            this.viewConfigToolStripMenuItem.Name = "viewConfigToolStripMenuItem";
            this.viewConfigToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.viewConfigToolStripMenuItem.Text = "View Config (F1)";
            this.viewConfigToolStripMenuItem.Click += new System.EventHandler(this.viewConfigToolStripMenuItem_Click);
            // 
            // addTextureFolderToolStripMenuItem
            // 
            this.addTextureFolderToolStripMenuItem.Name = "addTextureFolderToolStripMenuItem";
            this.addTextureFolderToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.addTextureFolderToolStripMenuItem.Text = "Add Texture Folder";
            this.addTextureFolderToolStripMenuItem.Click += new System.EventHandler(this.addTextureFolderToolStripMenuItem_Click);
            // 
            // clearTexturesToolStripMenuItem
            // 
            this.clearTexturesToolStripMenuItem.Name = "clearTexturesToolStripMenuItem";
            this.clearTexturesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.clearTexturesToolStripMenuItem.Text = "Clear Textures";
            this.clearTexturesToolStripMenuItem.Click += new System.EventHandler(this.clearTexturesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(176, 6);
            // 
            // noCullingCToolStripMenuItem
            // 
            this.noCullingCToolStripMenuItem.Name = "noCullingCToolStripMenuItem";
            this.noCullingCToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.noCullingCToolStripMenuItem.Text = "No Culling (C)";
            this.noCullingCToolStripMenuItem.Click += new System.EventHandler(this.noCullingCToolStripMenuItem_Click);
            // 
            // wireframeFToolStripMenuItem
            // 
            this.wireframeFToolStripMenuItem.Name = "wireframeFToolStripMenuItem";
            this.wireframeFToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.wireframeFToolStripMenuItem.Text = "Wireframe (F)";
            this.wireframeFToolStripMenuItem.Click += new System.EventHandler(this.wireframeFToolStripMenuItem_Click);
            // 
            // BackgroundColorToolStripMenuItem
            // 
            this.BackgroundColorToolStripMenuItem.Name = "BackgroundColorToolStripMenuItem";
            this.BackgroundColorToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.BackgroundColorToolStripMenuItem.Text = "Background Color...";
            this.BackgroundColorToolStripMenuItem.Click += new System.EventHandler(this.BackgroundColorToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(176, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.levelModelToolStripMenuItem,
            this.bOULToolStripMenuItem,
            this.bUTNToolStripMenuItem,
            this.dSTRToolStripMenuItem,
            this.mRKRToolStripMenuItem,
            this.mVPTToolStripMenuItem,
            this.pKUPToolStripMenuItem,
            this.pLATToolStripMenuItem,
            this.pLYRToolStripMenuItem,
            this.sIMPToolStripMenuItem,
            this.tRIGToolStripMenuItem,
            this.vILToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.displayToolStripMenuItem.Text = "Display";
            // 
            // levelModelToolStripMenuItem
            // 
            this.levelModelToolStripMenuItem.Checked = true;
            this.levelModelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.levelModelToolStripMenuItem.Name = "levelModelToolStripMenuItem";
            this.levelModelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.levelModelToolStripMenuItem.Text = "BSP/JSP";
            this.levelModelToolStripMenuItem.Click += new System.EventHandler(this.levelModelToolStripMenuItem_Click);
            // 
            // bUTNToolStripMenuItem
            // 
            this.bUTNToolStripMenuItem.Checked = true;
            this.bUTNToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bUTNToolStripMenuItem.Name = "bUTNToolStripMenuItem";
            this.bUTNToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.bUTNToolStripMenuItem.Text = "BUTN";
            this.bUTNToolStripMenuItem.Click += new System.EventHandler(this.bUTNToolStripMenuItem_Click);
            // 
            // dSTRToolStripMenuItem
            // 
            this.dSTRToolStripMenuItem.Checked = true;
            this.dSTRToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dSTRToolStripMenuItem.Name = "dSTRToolStripMenuItem";
            this.dSTRToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.dSTRToolStripMenuItem.Text = "DSTR";
            this.dSTRToolStripMenuItem.Click += new System.EventHandler(this.dSTRToolStripMenuItem_Click);
            // 
            // mRKRToolStripMenuItem
            // 
            this.mRKRToolStripMenuItem.Checked = true;
            this.mRKRToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mRKRToolStripMenuItem.Name = "mRKRToolStripMenuItem";
            this.mRKRToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mRKRToolStripMenuItem.Text = "MRKR";
            this.mRKRToolStripMenuItem.Click += new System.EventHandler(this.mRKRToolStripMenuItem_Click);
            // 
            // mVPTToolStripMenuItem
            // 
            this.mVPTToolStripMenuItem.Checked = true;
            this.mVPTToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mVPTToolStripMenuItem.Name = "mVPTToolStripMenuItem";
            this.mVPTToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.mVPTToolStripMenuItem.Text = "MVPT";
            this.mVPTToolStripMenuItem.Click += new System.EventHandler(this.mVPTToolStripMenuItem_Click);
            // 
            // pKUPToolStripMenuItem
            // 
            this.pKUPToolStripMenuItem.Checked = true;
            this.pKUPToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pKUPToolStripMenuItem.Name = "pKUPToolStripMenuItem";
            this.pKUPToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pKUPToolStripMenuItem.Text = "PKUP";
            this.pKUPToolStripMenuItem.Click += new System.EventHandler(this.pKUPToolStripMenuItem_Click);
            // 
            // pLATToolStripMenuItem
            // 
            this.pLATToolStripMenuItem.Checked = true;
            this.pLATToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pLATToolStripMenuItem.Name = "pLATToolStripMenuItem";
            this.pLATToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pLATToolStripMenuItem.Text = "PLAT";
            this.pLATToolStripMenuItem.Click += new System.EventHandler(this.pLATToolStripMenuItem_Click);
            // 
            // pLYRToolStripMenuItem
            // 
            this.pLYRToolStripMenuItem.Checked = true;
            this.pLYRToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pLYRToolStripMenuItem.Name = "pLYRToolStripMenuItem";
            this.pLYRToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pLYRToolStripMenuItem.Text = "PLYR";
            this.pLYRToolStripMenuItem.Click += new System.EventHandler(this.pLYRToolStripMenuItem_Click);
            // 
            // sIMPToolStripMenuItem
            // 
            this.sIMPToolStripMenuItem.Checked = true;
            this.sIMPToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sIMPToolStripMenuItem.Name = "sIMPToolStripMenuItem";
            this.sIMPToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sIMPToolStripMenuItem.Text = "SIMP";
            this.sIMPToolStripMenuItem.Click += new System.EventHandler(this.sIMPToolStripMenuItem_Click);
            // 
            // tRIGToolStripMenuItem
            // 
            this.tRIGToolStripMenuItem.Checked = true;
            this.tRIGToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tRIGToolStripMenuItem.Name = "tRIGToolStripMenuItem";
            this.tRIGToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tRIGToolStripMenuItem.Text = "TRIG";
            this.tRIGToolStripMenuItem.Click += new System.EventHandler(this.tRIGToolStripMenuItem_Click);
            // 
            // vILToolStripMenuItem
            // 
            this.vILToolStripMenuItem.Checked = true;
            this.vILToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.vILToolStripMenuItem.Name = "vILToolStripMenuItem";
            this.vILToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.vILToolStripMenuItem.Text = "VIL";
            this.vILToolStripMenuItem.Click += new System.EventHandler(this.vILToolStripMenuItem_Click);
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
            this.renderPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseClick);
            this.renderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseDown);
            this.renderPanel.MouseLeave += new System.EventHandler(this.renderPanel_MouseLeave);
            this.renderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveControl);
            this.renderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseUp);
            this.renderPanel.Resize += new System.EventHandler(this.ResetMouseCenter);
            // 
            // bOULToolStripMenuItem
            // 
            this.bOULToolStripMenuItem.Checked = true;
            this.bOULToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bOULToolStripMenuItem.Name = "bOULToolStripMenuItem";
            this.bOULToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.bOULToolStripMenuItem.Text = "BOUL";
            this.bOULToolStripMenuItem.Click += new System.EventHandler(this.bOULToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 838);
            this.Controls.Add(this.renderPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private ToolStripMenuItem viewConfigToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem archiveEditorToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem addTextureFolderToolStripMenuItem;
        private ToolStripMenuItem clearTexturesToolStripMenuItem;
        private ToolStripMenuItem displayToolStripMenuItem;
        private ToolStripMenuItem bUTNToolStripMenuItem;
        private ToolStripMenuItem mVPTToolStripMenuItem;
        private ToolStripMenuItem tRIGToolStripMenuItem;
        private ToolStripMenuItem pKUPToolStripMenuItem;
        private ToolStripMenuItem levelModelToolStripMenuItem;
        private ToolStripMenuItem pLATToolStripMenuItem;
        private ToolStripMenuItem sIMPToolStripMenuItem;
        private ToolStripMenuItem vILToolStripMenuItem;
        private ToolStripMenuItem mRKRToolStripMenuItem;
        private ToolStripMenuItem pLYRToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem dSTRToolStripMenuItem;
        private ToolStripMenuItem bOULToolStripMenuItem;
    }
}

