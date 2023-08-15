namespace IndustrialPark{
    partial class ImageEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importJPEGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveImageToHOPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.closeImageEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentImagePropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllEditsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filtersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blueOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.greenOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90LeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorXAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorYAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.specialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpBoxImageViewContainer = new System.Windows.Forms.GroupBox();
            this.imageEditorViewerBox = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.grpBoxImageViewContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageEditorViewerBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1009, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "fileMenuTool";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importPNGToolStripMenuItem,
            this.importJPEGToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveImageToHOPToolStripMenuItem,
            this.toolStripSeparator4,
            this.closeImageEditorToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // importPNGToolStripMenuItem
            // 
            this.importPNGToolStripMenuItem.Name = "importPNGToolStripMenuItem";
            this.importPNGToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.importPNGToolStripMenuItem.Text = "Import PNG";
            this.importPNGToolStripMenuItem.Click += new System.EventHandler(this.importPNGToolStripMenuItem_Click);
            // 
            // importJPEGToolStripMenuItem
            // 
            this.importJPEGToolStripMenuItem.Name = "importJPEGToolStripMenuItem";
            this.importJPEGToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.importJPEGToolStripMenuItem.Text = "Import JPEG";
            this.importJPEGToolStripMenuItem.Click += new System.EventHandler(this.importJPEGToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
            // 
            // saveImageToHOPToolStripMenuItem
            // 
            this.saveImageToHOPToolStripMenuItem.Name = "saveImageToHOPToolStripMenuItem";
            this.saveImageToHOPToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.saveImageToHOPToolStripMenuItem.Text = "Save Image As";
            this.saveImageToHOPToolStripMenuItem.Click += new System.EventHandler(this.saveImageToHOPToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(221, 6);
            // 
            // closeImageEditorToolStripMenuItem
            // 
            this.closeImageEditorToolStripMenuItem.Name = "closeImageEditorToolStripMenuItem";
            this.closeImageEditorToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.closeImageEditorToolStripMenuItem.Text = "Close Image Editor";
            this.closeImageEditorToolStripMenuItem.Click += new System.EventHandler(this.closeImageEditorToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentImagePropertiesToolStripMenuItem,
            this.clearAllEditsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // currentImagePropertiesToolStripMenuItem
            // 
            this.currentImagePropertiesToolStripMenuItem.Name = "currentImagePropertiesToolStripMenuItem";
            this.currentImagePropertiesToolStripMenuItem.Size = new System.Drawing.Size(257, 26);
            this.currentImagePropertiesToolStripMenuItem.Text = "Current Image Properties";
            this.currentImagePropertiesToolStripMenuItem.Click += new System.EventHandler(this.currentImagePropertiesToolStripMenuItem_Click);
            // 
            // clearAllEditsToolStripMenuItem
            // 
            this.clearAllEditsToolStripMenuItem.Name = "clearAllEditsToolStripMenuItem";
            this.clearAllEditsToolStripMenuItem.Size = new System.Drawing.Size(257, 26);
            this.clearAllEditsToolStripMenuItem.Text = "Clear All Edits";
            this.clearAllEditsToolStripMenuItem.Click += new System.EventHandler(this.clearAllEditsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filtersToolStripMenuItem,
            this.changeScaleToolStripMenuItem,
            this.rotateToolStripMenuItem,
            this.mirrorToolStripMenuItem,
            this.toolStripSeparator2,
            this.specialToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(58, 24);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // filtersToolStripMenuItem
            // 
            this.filtersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blueOverlayToolStripMenuItem,
            this.redOverlayToolStripMenuItem,
            this.greenOverlayToolStripMenuItem,
            this.invertToolStripMenuItem});
            this.filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            this.filtersToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.filtersToolStripMenuItem.Text = "Filters";
            // 
            // blueOverlayToolStripMenuItem
            // 
            this.blueOverlayToolStripMenuItem.Name = "blueOverlayToolStripMenuItem";
            this.blueOverlayToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.blueOverlayToolStripMenuItem.Text = "Blue Overlay";
            this.blueOverlayToolStripMenuItem.Click += new System.EventHandler(this.blueOverlayToolStripMenuItem_Click);
            // 
            // redOverlayToolStripMenuItem
            // 
            this.redOverlayToolStripMenuItem.Name = "redOverlayToolStripMenuItem";
            this.redOverlayToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.redOverlayToolStripMenuItem.Text = "Red Overlay";
            this.redOverlayToolStripMenuItem.Click += new System.EventHandler(this.redOverlayToolStripMenuItem_Click);
            // 
            // greenOverlayToolStripMenuItem
            // 
            this.greenOverlayToolStripMenuItem.Name = "greenOverlayToolStripMenuItem";
            this.greenOverlayToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.greenOverlayToolStripMenuItem.Text = "Green Overlay";
            this.greenOverlayToolStripMenuItem.Click += new System.EventHandler(this.greenOverlayToolStripMenuItem_Click);
            // 
            // invertToolStripMenuItem
            // 
            this.invertToolStripMenuItem.Name = "invertToolStripMenuItem";
            this.invertToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.invertToolStripMenuItem.Text = "Invert";
            this.invertToolStripMenuItem.Click += new System.EventHandler(this.invertToolStripMenuItem_Click);
            // 
            // changeScaleToolStripMenuItem
            // 
            this.changeScaleToolStripMenuItem.Name = "changeScaleToolStripMenuItem";
            this.changeScaleToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.changeScaleToolStripMenuItem.Text = "Change Scale";
            this.changeScaleToolStripMenuItem.Click += new System.EventHandler(this.changeScaleToolStripMenuItem_Click);
            // 
            // rotateToolStripMenuItem
            // 
            this.rotateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rotate90ToolStripMenuItem,
            this.rotate90LeftToolStripMenuItem});
            this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
            this.rotateToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.rotateToolStripMenuItem.Text = "Rotate";
            // 
            // rotate90ToolStripMenuItem
            // 
            this.rotate90ToolStripMenuItem.Name = "rotate90ToolStripMenuItem";
            this.rotate90ToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.rotate90ToolStripMenuItem.Text = "Rotate 90° Right";
            this.rotate90ToolStripMenuItem.Click += new System.EventHandler(this.rotate90ToolStripMenuItem_Click);
            // 
            // rotate90LeftToolStripMenuItem
            // 
            this.rotate90LeftToolStripMenuItem.Name = "rotate90LeftToolStripMenuItem";
            this.rotate90LeftToolStripMenuItem.Size = new System.Drawing.Size(201, 26);
            this.rotate90LeftToolStripMenuItem.Text = "Rotate 90° Left";
            this.rotate90LeftToolStripMenuItem.Click += new System.EventHandler(this.rotate90LeftToolStripMenuItem_Click);
            // 
            // mirrorToolStripMenuItem
            // 
            this.mirrorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mirrorXAxisToolStripMenuItem,
            this.mirrorYAxisToolStripMenuItem});
            this.mirrorToolStripMenuItem.Name = "mirrorToolStripMenuItem";
            this.mirrorToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.mirrorToolStripMenuItem.Text = "Mirror";
            // 
            // mirrorXAxisToolStripMenuItem
            // 
            this.mirrorXAxisToolStripMenuItem.Name = "mirrorXAxisToolStripMenuItem";
            this.mirrorXAxisToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.mirrorXAxisToolStripMenuItem.Text = "Mirror X Axis";
            this.mirrorXAxisToolStripMenuItem.Click += new System.EventHandler(this.mirrorXAxisToolStripMenuItem_Click);
            // 
            // mirrorYAxisToolStripMenuItem
            // 
            this.mirrorYAxisToolStripMenuItem.Name = "mirrorYAxisToolStripMenuItem";
            this.mirrorYAxisToolStripMenuItem.Size = new System.Drawing.Size(177, 26);
            this.mirrorYAxisToolStripMenuItem.Text = "Mirror Y Axis";
            this.mirrorYAxisToolStripMenuItem.Click += new System.EventHandler(this.mirrorYAxisToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(221, 6);
            // 
            // specialToolStripMenuItem
            // 
            this.specialToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearImageToolStripMenuItem});
            this.specialToolStripMenuItem.Name = "specialToolStripMenuItem";
            this.specialToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.specialToolStripMenuItem.Text = "Special";
            // 
            // clearImageToolStripMenuItem
            // 
            this.clearImageToolStripMenuItem.Name = "clearImageToolStripMenuItem";
            this.clearImageToolStripMenuItem.Size = new System.Drawing.Size(172, 26);
            this.clearImageToolStripMenuItem.Text = "Clear Image";
            this.clearImageToolStripMenuItem.Click += new System.EventHandler(this.clearImageToolStripMenuItem_Click);
            // 
            // grpBoxImageViewContainer
            // 
            this.grpBoxImageViewContainer.Controls.Add(this.imageEditorViewerBox);
            this.grpBoxImageViewContainer.Location = new System.Drawing.Point(12, 31);
            this.grpBoxImageViewContainer.Name = "grpBoxImageViewContainer";
            this.grpBoxImageViewContainer.Size = new System.Drawing.Size(985, 632);
            this.grpBoxImageViewContainer.TabIndex = 1;
            this.grpBoxImageViewContainer.TabStop = false;
            this.grpBoxImageViewContainer.Text = "Image View";
            // 
            // imageEditorViewerBox
            // 
            this.imageEditorViewerBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageEditorViewerBox.Location = new System.Drawing.Point(3, 18);
            this.imageEditorViewerBox.Name = "imageEditorViewerBox";
            this.imageEditorViewerBox.Size = new System.Drawing.Size(979, 611);
            this.imageEditorViewerBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imageEditorViewerBox.TabIndex = 0;
            this.imageEditorViewerBox.TabStop = false;
            // 
            // ImageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 675);
            this.Controls.Add(this.grpBoxImageViewContainer);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ImageEditor";
            this.Text = "Image Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpBoxImageViewContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageEditorViewerBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importPNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importJPEGToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem closeImageEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageToHOPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filtersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeScaleToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpBoxImageViewContainer;
        private System.Windows.Forms.ToolStripMenuItem currentImagePropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mirrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blueOverlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redOverlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem greenOverlayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem specialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllEditsToolStripMenuItem;
        public System.Windows.Forms.PictureBox imageEditorViewerBox;
        private System.Windows.Forms.ToolStripMenuItem rotate90ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate90LeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mirrorXAxisToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mirrorYAxisToolStripMenuItem;
    }
}