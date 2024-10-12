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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archiveEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllEditorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.autoSaveOnClosingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoLoadOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showEditorsWhenLoadingProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.manageUserTemplatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.templatesPersistentShiniesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.templatesChainPointMVPTsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.updateReferencesOnCopyPasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceAssetsOnPasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLegacyAssetIDFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLegacyAssetTypeFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cursorInFlyModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crosshairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hiddenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllOpenHIPsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runGameF5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildAndRunPS2ISOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopSoundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createGameCubeBannerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatesOnStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadIndustrialParkEditorFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesOnEditorFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadVgmstreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ensureAssociationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discordRichPresenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assetTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorAssetTypes = new System.Windows.Forms.ToolStripSeparator();
            this.addTextureFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTXDArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshTexturesAndModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.colorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectionColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.widgetColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mVPTColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tRIGColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sFXInColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noCullingCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lowerQualityGraphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.uIModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uIModeAutoSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.drawOnlyFirstMINFReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showVertexColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLODTForRenderingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usePIPTForRenderingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideInvisibleMeshesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movementPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.researchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assetIDGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dYNASearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dynaNameSearcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pickupSearcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelProject = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTemplate = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelNumSelected = new System.Windows.Forms.ToolStripStatusLabel();
            this.renderPanel = new System.Windows.Forms.Panel();
            this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.gizmoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionLocalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Templates = new System.Windows.Forms.ToolStripMenuItem();
            this.userTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxUserTemplate = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.archiveEditorToolStripMenuItem, this.projectToolStripMenuItem, this.optionsToolStripMenuItem, this.toolsToolStripMenuItem, this.displayToolStripMenuItem, this.researchToolStripMenuItem, this.helpToolStripMenuItem });
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // archiveEditorToolStripMenuItem
            // 
            this.archiveEditorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.newToolStripMenuItem, this.importLevelToolStripMenuItem, this.openLastToolStripMenuItem, this.closeAllEditorsToolStripMenuItem, this.toolStripSeparator3 });
            this.archiveEditorToolStripMenuItem.Name = "archiveEditorToolStripMenuItem";
            resources.ApplyResources(this.archiveEditorToolStripMenuItem, "archiveEditorToolStripMenuItem");
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            resources.ApplyResources(this.newToolStripMenuItem, "newToolStripMenuItem");
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // importLevelToolStripMenuItem
            // 
            this.importLevelToolStripMenuItem.Name = "importLevelToolStripMenuItem";
            resources.ApplyResources(this.importLevelToolStripMenuItem, "importLevelToolStripMenuItem");
            this.importLevelToolStripMenuItem.Click += new System.EventHandler(this.openLevelToolStripMenuItem_Click);
            // 
            // openLastToolStripMenuItem
            // 
            this.openLastToolStripMenuItem.Name = "openLastToolStripMenuItem";
            resources.ApplyResources(this.openLastToolStripMenuItem, "openLastToolStripMenuItem");
            // 
            // closeAllEditorsToolStripMenuItem
            // 
            resources.ApplyResources(this.closeAllEditorsToolStripMenuItem, "closeAllEditorsToolStripMenuItem");
            this.closeAllEditorsToolStripMenuItem.Name = "closeAllEditorsToolStripMenuItem";
            this.closeAllEditorsToolStripMenuItem.Click += new System.EventHandler(this.closeAllEditorsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.newToolStripMenuItem1, this.openToolStripMenuItem, this.saveToolStripMenuItem, this.saveAsToolStripMenuItem, this.toolStripSeparator5, this.autoSaveOnClosingToolStripMenuItem, this.autoLoadOnStartupToolStripMenuItem, this.showEditorsWhenLoadingProjectToolStripMenuItem });
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            resources.ApplyResources(this.projectToolStripMenuItem, "projectToolStripMenuItem");
            // 
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            resources.ApplyResources(this.newToolStripMenuItem1, "newToolStripMenuItem1");
            this.newToolStripMenuItem1.Click += new System.EventHandler(this.newToolStripMenuItem1_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // autoSaveOnClosingToolStripMenuItem
            // 
            this.autoSaveOnClosingToolStripMenuItem.Checked = true;
            this.autoSaveOnClosingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSaveOnClosingToolStripMenuItem.Name = "autoSaveOnClosingToolStripMenuItem";
            resources.ApplyResources(this.autoSaveOnClosingToolStripMenuItem, "autoSaveOnClosingToolStripMenuItem");
            this.autoSaveOnClosingToolStripMenuItem.Click += new System.EventHandler(this.autoSaveOnClosingToolStripMenuItem_Click);
            // 
            // autoLoadOnStartupToolStripMenuItem
            // 
            this.autoLoadOnStartupToolStripMenuItem.Checked = true;
            this.autoLoadOnStartupToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoLoadOnStartupToolStripMenuItem.Name = "autoLoadOnStartupToolStripMenuItem";
            resources.ApplyResources(this.autoLoadOnStartupToolStripMenuItem, "autoLoadOnStartupToolStripMenuItem");
            this.autoLoadOnStartupToolStripMenuItem.Click += new System.EventHandler(this.autoLoadOnStartupToolStripMenuItem_Click);
            // 
            // showEditorsWhenLoadingProjectToolStripMenuItem
            // 
            this.showEditorsWhenLoadingProjectToolStripMenuItem.Checked = true;
            this.showEditorsWhenLoadingProjectToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showEditorsWhenLoadingProjectToolStripMenuItem.Name = "showEditorsWhenLoadingProjectToolStripMenuItem";
            resources.ApplyResources(this.showEditorsWhenLoadingProjectToolStripMenuItem, "showEditorsWhenLoadingProjectToolStripMenuItem");
            this.showEditorsWhenLoadingProjectToolStripMenuItem.Click += new System.EventHandler(this.showEditorsWhenLoadingProjectToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.viewConfigToolStripMenuItem, this.viewControlsToolStripMenuItem, this.toolStripSeparator4, this.manageUserTemplatesToolStripMenuItem, this.templatesPersistentShiniesToolStripMenuItem, this.templatesChainPointMVPTsToolStripMenuItem, this.toolStripSeparator7, this.updateReferencesOnCopyPasteToolStripMenuItem, this.replaceAssetsOnPasteToolStripMenuItem, this.useLegacyAssetIDFormatToolStripMenuItem, this.useLegacyAssetTypeFormatToolStripMenuItem, this.cursorInFlyModeToolStripMenuItem });
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            // 
            // viewConfigToolStripMenuItem
            // 
            this.viewConfigToolStripMenuItem.Name = "viewConfigToolStripMenuItem";
            resources.ApplyResources(this.viewConfigToolStripMenuItem, "viewConfigToolStripMenuItem");
            this.viewConfigToolStripMenuItem.Click += new System.EventHandler(this.viewConfigToolStripMenuItem_Click);
            // 
            // viewControlsToolStripMenuItem
            // 
            this.viewControlsToolStripMenuItem.Name = "viewControlsToolStripMenuItem";
            resources.ApplyResources(this.viewControlsToolStripMenuItem, "viewControlsToolStripMenuItem");
            this.viewControlsToolStripMenuItem.Click += new System.EventHandler(this.viewControlsToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // manageUserTemplatesToolStripMenuItem
            // 
            this.manageUserTemplatesToolStripMenuItem.Name = "manageUserTemplatesToolStripMenuItem";
            resources.ApplyResources(this.manageUserTemplatesToolStripMenuItem, "manageUserTemplatesToolStripMenuItem");
            this.manageUserTemplatesToolStripMenuItem.Click += new System.EventHandler(this.manageUserTemplatesToolStripMenuItem_Click);
            // 
            // templatesPersistentShiniesToolStripMenuItem
            // 
            this.templatesPersistentShiniesToolStripMenuItem.Checked = true;
            this.templatesPersistentShiniesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.templatesPersistentShiniesToolStripMenuItem.Name = "templatesPersistentShiniesToolStripMenuItem";
            resources.ApplyResources(this.templatesPersistentShiniesToolStripMenuItem, "templatesPersistentShiniesToolStripMenuItem");
            this.templatesPersistentShiniesToolStripMenuItem.Click += new System.EventHandler(this.templatesPersistentShiniesToolStripMenuItem_Click);
            // 
            // templatesChainPointMVPTsToolStripMenuItem
            // 
            this.templatesChainPointMVPTsToolStripMenuItem.Name = "templatesChainPointMVPTsToolStripMenuItem";
            resources.ApplyResources(this.templatesChainPointMVPTsToolStripMenuItem, "templatesChainPointMVPTsToolStripMenuItem");
            this.templatesChainPointMVPTsToolStripMenuItem.Click += new System.EventHandler(this.templatesChainPointMVPTsToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // updateReferencesOnCopyPasteToolStripMenuItem
            // 
            this.updateReferencesOnCopyPasteToolStripMenuItem.Checked = true;
            this.updateReferencesOnCopyPasteToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateReferencesOnCopyPasteToolStripMenuItem.Name = "updateReferencesOnCopyPasteToolStripMenuItem";
            resources.ApplyResources(this.updateReferencesOnCopyPasteToolStripMenuItem, "updateReferencesOnCopyPasteToolStripMenuItem");
            this.updateReferencesOnCopyPasteToolStripMenuItem.Click += new System.EventHandler(this.updateReferencesOnCopyPasteToolStripMenuItem_Click);
            // 
            // replaceAssetsOnPasteToolStripMenuItem
            // 
            this.replaceAssetsOnPasteToolStripMenuItem.Name = "replaceAssetsOnPasteToolStripMenuItem";
            resources.ApplyResources(this.replaceAssetsOnPasteToolStripMenuItem, "replaceAssetsOnPasteToolStripMenuItem");
            this.replaceAssetsOnPasteToolStripMenuItem.Click += new System.EventHandler(this.replaceAssetsOnPasteToolStripMenuItem_Click);
            // 
            // useLegacyAssetIDFormatToolStripMenuItem
            // 
            this.useLegacyAssetIDFormatToolStripMenuItem.Name = "useLegacyAssetIDFormatToolStripMenuItem";
            resources.ApplyResources(this.useLegacyAssetIDFormatToolStripMenuItem, "useLegacyAssetIDFormatToolStripMenuItem");
            this.useLegacyAssetIDFormatToolStripMenuItem.Click += new System.EventHandler(this.useLegacyAssetIDFormatToolStripMenuItem_Click);
            // 
            // useLegacyAssetTypeFormatToolStripMenuItem
            // 
            this.useLegacyAssetTypeFormatToolStripMenuItem.Name = "useLegacyAssetTypeFormatToolStripMenuItem";
            resources.ApplyResources(this.useLegacyAssetTypeFormatToolStripMenuItem, "useLegacyAssetTypeFormatToolStripMenuItem");
            this.useLegacyAssetTypeFormatToolStripMenuItem.Click += new System.EventHandler(this.useLegacyAssetTypeFormatToolStripMenuItem_Click);
            // 
            // cursorInFlyModeToolStripMenuItem
            // 
            this.cursorInFlyModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.defaultToolStripMenuItem, this.crosshairToolStripMenuItem, this.hiddenToolStripMenuItem });
            this.cursorInFlyModeToolStripMenuItem.Name = "cursorInFlyModeToolStripMenuItem";
            resources.ApplyResources(this.cursorInFlyModeToolStripMenuItem, "cursorInFlyModeToolStripMenuItem");
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            resources.ApplyResources(this.defaultToolStripMenuItem, "defaultToolStripMenuItem");
            this.defaultToolStripMenuItem.Tag = "0";
            this.defaultToolStripMenuItem.Click += new System.EventHandler(this.changeFlyModeCursor);
            // 
            // crosshairToolStripMenuItem
            // 
            this.crosshairToolStripMenuItem.Checked = true;
            this.crosshairToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.crosshairToolStripMenuItem.Name = "crosshairToolStripMenuItem";
            resources.ApplyResources(this.crosshairToolStripMenuItem, "crosshairToolStripMenuItem");
            this.crosshairToolStripMenuItem.Tag = "1";
            this.crosshairToolStripMenuItem.Click += new System.EventHandler(this.changeFlyModeCursor);
            // 
            // hiddenToolStripMenuItem
            // 
            this.hiddenToolStripMenuItem.Name = "hiddenToolStripMenuItem";
            resources.ApplyResources(this.hiddenToolStripMenuItem, "hiddenToolStripMenuItem");
            this.hiddenToolStripMenuItem.Tag = "2";
            this.hiddenToolStripMenuItem.Click += new System.EventHandler(this.changeFlyModeCursor);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.saveAllOpenHIPsToolStripMenuItem, this.runGameF5ToolStripMenuItem, this.buildAndRunPS2ISOToolStripMenuItem, this.stopSoundToolStripMenuItem, this.exportSceneToolStripMenuItem, this.createGameCubeBannerToolStripMenuItem, this.toolStripSeparator8, this.checkForUpdatesOnStartupToolStripMenuItem, this.checkForUpdatesNowToolStripMenuItem, this.downloadIndustrialParkEditorFilesToolStripMenuItem, this.checkForUpdatesOnEditorFilesToolStripMenuItem, this.downloadVgmstreamToolStripMenuItem, this.toolStripSeparator2, this.ensureAssociationsToolStripMenuItem, this.discordRichPresenceToolStripMenuItem });
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // saveAllOpenHIPsToolStripMenuItem
            // 
            this.saveAllOpenHIPsToolStripMenuItem.Name = "saveAllOpenHIPsToolStripMenuItem";
            resources.ApplyResources(this.saveAllOpenHIPsToolStripMenuItem, "saveAllOpenHIPsToolStripMenuItem");
            this.saveAllOpenHIPsToolStripMenuItem.Click += new System.EventHandler(this.saveAllOpenHIPsToolStripMenuItem_Click);
            // 
            // runGameF5ToolStripMenuItem
            // 
            this.runGameF5ToolStripMenuItem.Name = "runGameF5ToolStripMenuItem";
            resources.ApplyResources(this.runGameF5ToolStripMenuItem, "runGameF5ToolStripMenuItem");
            this.runGameF5ToolStripMenuItem.Click += new System.EventHandler(this.runGameF5ToolStripMenuItem_Click);
            // 
            // buildAndRunPS2ISOToolStripMenuItem
            // 
            this.buildAndRunPS2ISOToolStripMenuItem.Name = "buildAndRunPS2ISOToolStripMenuItem";
            resources.ApplyResources(this.buildAndRunPS2ISOToolStripMenuItem, "buildAndRunPS2ISOToolStripMenuItem");
            this.buildAndRunPS2ISOToolStripMenuItem.Click += new System.EventHandler(this.buildAndRunPS2ISOToolStripMenuItem_Click);
            // 
            // stopSoundToolStripMenuItem
            // 
            this.stopSoundToolStripMenuItem.Name = "stopSoundToolStripMenuItem";
            resources.ApplyResources(this.stopSoundToolStripMenuItem, "stopSoundToolStripMenuItem");
            this.stopSoundToolStripMenuItem.Click += new System.EventHandler(this.stopSoundToolStripMenuItem_Click);
            // 
            // exportSceneToolStripMenuItem
            // 
            this.exportSceneToolStripMenuItem.Name = "exportSceneToolStripMenuItem";
            resources.ApplyResources(this.exportSceneToolStripMenuItem, "exportSceneToolStripMenuItem");
            this.exportSceneToolStripMenuItem.Click += new System.EventHandler(this.exportSceneToolStripMenuItem_Click);
            // 
            // createGameCubeBannerToolStripMenuItem
            // 
            this.createGameCubeBannerToolStripMenuItem.Name = "createGameCubeBannerToolStripMenuItem";
            resources.ApplyResources(this.createGameCubeBannerToolStripMenuItem, "createGameCubeBannerToolStripMenuItem");
            this.createGameCubeBannerToolStripMenuItem.Click += new System.EventHandler(this.createGameCubeBannerToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // checkForUpdatesOnStartupToolStripMenuItem
            // 
            this.checkForUpdatesOnStartupToolStripMenuItem.Checked = true;
            this.checkForUpdatesOnStartupToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkForUpdatesOnStartupToolStripMenuItem.Name = "checkForUpdatesOnStartupToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesOnStartupToolStripMenuItem, "checkForUpdatesOnStartupToolStripMenuItem");
            this.checkForUpdatesOnStartupToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesOnStartupToolStripMenuItem_Click);
            // 
            // checkForUpdatesNowToolStripMenuItem
            // 
            this.checkForUpdatesNowToolStripMenuItem.Name = "checkForUpdatesNowToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesNowToolStripMenuItem, "checkForUpdatesNowToolStripMenuItem");
            this.checkForUpdatesNowToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesNowToolStripMenuItem_Click);
            // 
            // downloadIndustrialParkEditorFilesToolStripMenuItem
            // 
            this.downloadIndustrialParkEditorFilesToolStripMenuItem.Name = "downloadIndustrialParkEditorFilesToolStripMenuItem";
            resources.ApplyResources(this.downloadIndustrialParkEditorFilesToolStripMenuItem, "downloadIndustrialParkEditorFilesToolStripMenuItem");
            this.downloadIndustrialParkEditorFilesToolStripMenuItem.Click += new System.EventHandler(this.DownloadIndustrialParkEditorFilesToolStripMenuItem_Click);
            // 
            // checkForUpdatesOnEditorFilesToolStripMenuItem
            // 
            this.checkForUpdatesOnEditorFilesToolStripMenuItem.Name = "checkForUpdatesOnEditorFilesToolStripMenuItem";
            resources.ApplyResources(this.checkForUpdatesOnEditorFilesToolStripMenuItem, "checkForUpdatesOnEditorFilesToolStripMenuItem");
            this.checkForUpdatesOnEditorFilesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesOnEditorFilesToolStripMenuItem_Click);
            // 
            // downloadVgmstreamToolStripMenuItem
            // 
            this.downloadVgmstreamToolStripMenuItem.Name = "downloadVgmstreamToolStripMenuItem";
            resources.ApplyResources(this.downloadVgmstreamToolStripMenuItem, "downloadVgmstreamToolStripMenuItem");
            this.downloadVgmstreamToolStripMenuItem.Click += new System.EventHandler(this.downloadVgmstreamToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // ensureAssociationsToolStripMenuItem
            // 
            this.ensureAssociationsToolStripMenuItem.Name = "ensureAssociationsToolStripMenuItem";
            resources.ApplyResources(this.ensureAssociationsToolStripMenuItem, "ensureAssociationsToolStripMenuItem");
            this.ensureAssociationsToolStripMenuItem.Click += new System.EventHandler(this.ensureAssociationsToolStripMenuItem_Click);
            // 
            // discordRichPresenceToolStripMenuItem
            // 
            this.discordRichPresenceToolStripMenuItem.Checked = true;
            this.discordRichPresenceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.discordRichPresenceToolStripMenuItem.Name = "discordRichPresenceToolStripMenuItem";
            resources.ApplyResources(this.discordRichPresenceToolStripMenuItem, "discordRichPresenceToolStripMenuItem");
            this.discordRichPresenceToolStripMenuItem.Click += new System.EventHandler(this.discordRichPresenceToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.assetTypesToolStripMenuItem, this.addTextureFolderToolStripMenuItem, this.addTXDArchiveToolStripMenuItem, this.refreshTexturesAndModelsToolStripMenuItem, this.toolStripSeparator10, this.colorsToolStripMenuItem, this.noCullingCToolStripMenuItem, this.wireframeFToolStripMenuItem, this.vSyncToolStripMenuItem, this.lowerQualityGraphicsToolStripMenuItem, this.toolStripSeparator6, this.uIModeToolStripMenuItem, this.uIModeAutoSizeToolStripMenuItem, this.toolStripSeparator9, this.drawOnlyFirstMINFReferenceToolStripMenuItem, this.showVertexColorsToolStripMenuItem, this.useLODTForRenderingToolStripMenuItem, this.usePIPTForRenderingToolStripMenuItem, this.hideInvisibleMeshesToolStripMenuItem, this.movementPreviewToolStripMenuItem });
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            resources.ApplyResources(this.displayToolStripMenuItem, "displayToolStripMenuItem");
            // 
            // assetTypesToolStripMenuItem
            // 
            this.assetTypesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.enableAllToolStripMenuItem, this.disableAllToolStripMenuItem, this.toolStripSeparatorAssetTypes });
            this.assetTypesToolStripMenuItem.Name = "assetTypesToolStripMenuItem";
            resources.ApplyResources(this.assetTypesToolStripMenuItem, "assetTypesToolStripMenuItem");
            // 
            // enableAllToolStripMenuItem
            // 
            this.enableAllToolStripMenuItem.Name = "enableAllToolStripMenuItem";
            resources.ApplyResources(this.enableAllToolStripMenuItem, "enableAllToolStripMenuItem");
            this.enableAllToolStripMenuItem.Click += new System.EventHandler(this.enableAllToolStripMenuItem_Click);
            // 
            // disableAllToolStripMenuItem
            // 
            this.disableAllToolStripMenuItem.Name = "disableAllToolStripMenuItem";
            resources.ApplyResources(this.disableAllToolStripMenuItem, "disableAllToolStripMenuItem");
            this.disableAllToolStripMenuItem.Click += new System.EventHandler(this.disableAllToolStripMenuItem_Click);
            // 
            // toolStripSeparatorAssetTypes
            // 
            this.toolStripSeparatorAssetTypes.Name = "toolStripSeparatorAssetTypes";
            resources.ApplyResources(this.toolStripSeparatorAssetTypes, "toolStripSeparatorAssetTypes");
            // 
            // addTextureFolderToolStripMenuItem
            // 
            this.addTextureFolderToolStripMenuItem.Name = "addTextureFolderToolStripMenuItem";
            resources.ApplyResources(this.addTextureFolderToolStripMenuItem, "addTextureFolderToolStripMenuItem");
            this.addTextureFolderToolStripMenuItem.Click += new System.EventHandler(this.addTextureFolderToolStripMenuItem_Click);
            // 
            // addTXDArchiveToolStripMenuItem
            // 
            this.addTXDArchiveToolStripMenuItem.Name = "addTXDArchiveToolStripMenuItem";
            resources.ApplyResources(this.addTXDArchiveToolStripMenuItem, "addTXDArchiveToolStripMenuItem");
            this.addTXDArchiveToolStripMenuItem.Click += new System.EventHandler(this.addTXDArchiveToolStripMenuItem_Click);
            // 
            // refreshTexturesAndModelsToolStripMenuItem
            // 
            this.refreshTexturesAndModelsToolStripMenuItem.Name = "refreshTexturesAndModelsToolStripMenuItem";
            resources.ApplyResources(this.refreshTexturesAndModelsToolStripMenuItem, "refreshTexturesAndModelsToolStripMenuItem");
            this.refreshTexturesAndModelsToolStripMenuItem.Click += new System.EventHandler(this.refreshTexturesToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // colorsToolStripMenuItem
            // 
            this.colorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.resetColorsToolStripMenuItem, this.backgroundColorToolStripMenuItem1, this.selectionColorToolStripMenuItem1, this.toolStripSeparator11, this.widgetColorToolStripMenuItem, this.mVPTColorToolStripMenuItem, this.tRIGColorToolStripMenuItem, this.sFXInColorToolStripMenuItem });
            this.colorsToolStripMenuItem.Name = "colorsToolStripMenuItem";
            resources.ApplyResources(this.colorsToolStripMenuItem, "colorsToolStripMenuItem");
            // 
            // resetColorsToolStripMenuItem
            // 
            this.resetColorsToolStripMenuItem.Name = "resetColorsToolStripMenuItem";
            resources.ApplyResources(this.resetColorsToolStripMenuItem, "resetColorsToolStripMenuItem");
            this.resetColorsToolStripMenuItem.Click += new System.EventHandler(this.resetColorsToolStripMenuItem_Click);
            // 
            // backgroundColorToolStripMenuItem1
            // 
            this.backgroundColorToolStripMenuItem1.Name = "backgroundColorToolStripMenuItem1";
            resources.ApplyResources(this.backgroundColorToolStripMenuItem1, "backgroundColorToolStripMenuItem1");
            this.backgroundColorToolStripMenuItem1.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem1_Click);
            // 
            // selectionColorToolStripMenuItem1
            // 
            this.selectionColorToolStripMenuItem1.Name = "selectionColorToolStripMenuItem1";
            resources.ApplyResources(this.selectionColorToolStripMenuItem1, "selectionColorToolStripMenuItem1");
            this.selectionColorToolStripMenuItem1.Click += new System.EventHandler(this.selectionColorToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // widgetColorToolStripMenuItem
            // 
            this.widgetColorToolStripMenuItem.Name = "widgetColorToolStripMenuItem";
            resources.ApplyResources(this.widgetColorToolStripMenuItem, "widgetColorToolStripMenuItem");
            this.widgetColorToolStripMenuItem.Click += new System.EventHandler(this.widgetColorToolStripMenuItem_Click);
            // 
            // mVPTColorToolStripMenuItem
            // 
            this.mVPTColorToolStripMenuItem.Name = "mVPTColorToolStripMenuItem";
            resources.ApplyResources(this.mVPTColorToolStripMenuItem, "mVPTColorToolStripMenuItem");
            this.mVPTColorToolStripMenuItem.Click += new System.EventHandler(this.mVPTColorToolStripMenuItem_Click);
            // 
            // tRIGColorToolStripMenuItem
            // 
            this.tRIGColorToolStripMenuItem.Name = "tRIGColorToolStripMenuItem";
            resources.ApplyResources(this.tRIGColorToolStripMenuItem, "tRIGColorToolStripMenuItem");
            this.tRIGColorToolStripMenuItem.Click += new System.EventHandler(this.tRIGColorToolStripMenuItem_Click);
            // 
            // sFXInColorToolStripMenuItem
            // 
            this.sFXInColorToolStripMenuItem.Name = "sFXInColorToolStripMenuItem";
            resources.ApplyResources(this.sFXInColorToolStripMenuItem, "sFXInColorToolStripMenuItem");
            this.sFXInColorToolStripMenuItem.Click += new System.EventHandler(this.sFXInColorToolStripMenuItem_Click);
            // 
            // noCullingCToolStripMenuItem
            // 
            this.noCullingCToolStripMenuItem.Name = "noCullingCToolStripMenuItem";
            resources.ApplyResources(this.noCullingCToolStripMenuItem, "noCullingCToolStripMenuItem");
            this.noCullingCToolStripMenuItem.Click += new System.EventHandler(this.noCullingCToolStripMenuItem_Click);
            // 
            // wireframeFToolStripMenuItem
            // 
            this.wireframeFToolStripMenuItem.Name = "wireframeFToolStripMenuItem";
            resources.ApplyResources(this.wireframeFToolStripMenuItem, "wireframeFToolStripMenuItem");
            this.wireframeFToolStripMenuItem.Click += new System.EventHandler(this.wireframeFToolStripMenuItem_Click);
            // 
            // vSyncToolStripMenuItem
            // 
            this.vSyncToolStripMenuItem.Checked = true;
            this.vSyncToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.vSyncToolStripMenuItem.Name = "vSyncToolStripMenuItem";
            resources.ApplyResources(this.vSyncToolStripMenuItem, "vSyncToolStripMenuItem");
            this.vSyncToolStripMenuItem.Click += new System.EventHandler(this.vSyncToolStripMenuItem_Click);
            // 
            // lowerQualityGraphicsToolStripMenuItem
            // 
            this.lowerQualityGraphicsToolStripMenuItem.Name = "lowerQualityGraphicsToolStripMenuItem";
            resources.ApplyResources(this.lowerQualityGraphicsToolStripMenuItem, "lowerQualityGraphicsToolStripMenuItem");
            this.lowerQualityGraphicsToolStripMenuItem.Click += new System.EventHandler(this.lowerQualityGraphicsToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // uIModeToolStripMenuItem
            // 
            this.uIModeToolStripMenuItem.Name = "uIModeToolStripMenuItem";
            resources.ApplyResources(this.uIModeToolStripMenuItem, "uIModeToolStripMenuItem");
            this.uIModeToolStripMenuItem.Click += new System.EventHandler(this.uIModeToolStripMenuItem_Click);
            // 
            // uIModeAutoSizeToolStripMenuItem
            // 
            this.uIModeAutoSizeToolStripMenuItem.Name = "uIModeAutoSizeToolStripMenuItem";
            resources.ApplyResources(this.uIModeAutoSizeToolStripMenuItem, "uIModeAutoSizeToolStripMenuItem");
            this.uIModeAutoSizeToolStripMenuItem.Click += new System.EventHandler(this.uIModeAutoSizeToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // drawOnlyFirstMINFReferenceToolStripMenuItem
            // 
            this.drawOnlyFirstMINFReferenceToolStripMenuItem.Name = "drawOnlyFirstMINFReferenceToolStripMenuItem";
            resources.ApplyResources(this.drawOnlyFirstMINFReferenceToolStripMenuItem, "drawOnlyFirstMINFReferenceToolStripMenuItem");
            this.drawOnlyFirstMINFReferenceToolStripMenuItem.Click += new System.EventHandler(this.drawOnlyFirstMINFReferenceToolStripMenuItem_Click);
            // 
            // showVertexColorsToolStripMenuItem
            // 
            this.showVertexColorsToolStripMenuItem.Checked = true;
            this.showVertexColorsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showVertexColorsToolStripMenuItem.Name = "showVertexColorsToolStripMenuItem";
            resources.ApplyResources(this.showVertexColorsToolStripMenuItem, "showVertexColorsToolStripMenuItem");
            this.showVertexColorsToolStripMenuItem.Click += new System.EventHandler(this.showVertexColorsToolStripMenuItem_Click);
            // 
            // useLODTForRenderingToolStripMenuItem
            // 
            this.useLODTForRenderingToolStripMenuItem.Name = "useLODTForRenderingToolStripMenuItem";
            resources.ApplyResources(this.useLODTForRenderingToolStripMenuItem, "useLODTForRenderingToolStripMenuItem");
            this.useLODTForRenderingToolStripMenuItem.Click += new System.EventHandler(this.useMaxRenderDistanceToolStripMenuItem_Click);
            // 
            // usePIPTForRenderingToolStripMenuItem
            // 
            this.usePIPTForRenderingToolStripMenuItem.Checked = true;
            this.usePIPTForRenderingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.usePIPTForRenderingToolStripMenuItem.Name = "usePIPTForRenderingToolStripMenuItem";
            resources.ApplyResources(this.usePIPTForRenderingToolStripMenuItem, "usePIPTForRenderingToolStripMenuItem");
            this.usePIPTForRenderingToolStripMenuItem.Click += new System.EventHandler(this.UsePIPTForRenderingToolStripMenuItem_Click);
            // 
            // hideInvisibleMeshesToolStripMenuItem
            // 
            this.hideInvisibleMeshesToolStripMenuItem.Name = "hideInvisibleMeshesToolStripMenuItem";
            resources.ApplyResources(this.hideInvisibleMeshesToolStripMenuItem, "hideInvisibleMeshesToolStripMenuItem");
            this.hideInvisibleMeshesToolStripMenuItem.Click += new System.EventHandler(this.hideInvisibleMeshesToolStripMenuItem_Click);
            // 
            // movementPreviewToolStripMenuItem
            // 
            this.movementPreviewToolStripMenuItem.Name = "movementPreviewToolStripMenuItem";
            resources.ApplyResources(this.movementPreviewToolStripMenuItem, "movementPreviewToolStripMenuItem");
            this.movementPreviewToolStripMenuItem.Click += new System.EventHandler(this.pLATPreviewToolStripMenuItem_Click);
            // 
            // researchToolStripMenuItem
            // 
            this.researchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.assetIDGeneratorToolStripMenuItem, this.dYNASearchToolStripMenuItem, this.eventSearchToolStripMenuItem, this.openFolderToolStripMenuItem, this.dynaNameSearcherToolStripMenuItem, this.pickupSearcherToolStripMenuItem });
            this.researchToolStripMenuItem.Name = "researchToolStripMenuItem";
            resources.ApplyResources(this.researchToolStripMenuItem, "researchToolStripMenuItem");
            // 
            // assetIDGeneratorToolStripMenuItem
            // 
            this.assetIDGeneratorToolStripMenuItem.Name = "assetIDGeneratorToolStripMenuItem";
            resources.ApplyResources(this.assetIDGeneratorToolStripMenuItem, "assetIDGeneratorToolStripMenuItem");
            this.assetIDGeneratorToolStripMenuItem.Click += new System.EventHandler(this.assetIDGeneratorToolStripMenuItem_Click);
            // 
            // dYNASearchToolStripMenuItem
            // 
            this.dYNASearchToolStripMenuItem.Name = "dYNASearchToolStripMenuItem";
            resources.ApplyResources(this.dYNASearchToolStripMenuItem, "dYNASearchToolStripMenuItem");
            this.dYNASearchToolStripMenuItem.Click += new System.EventHandler(this.dYNASearchToolStripMenuItem_Click);
            // 
            // eventSearchToolStripMenuItem
            // 
            this.eventSearchToolStripMenuItem.Name = "eventSearchToolStripMenuItem";
            resources.ApplyResources(this.eventSearchToolStripMenuItem, "eventSearchToolStripMenuItem");
            this.eventSearchToolStripMenuItem.Click += new System.EventHandler(this.eventSearchToolStripMenuItem_Click);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            resources.ApplyResources(this.openFolderToolStripMenuItem, "openFolderToolStripMenuItem");
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // dynaNameSearcherToolStripMenuItem
            // 
            this.dynaNameSearcherToolStripMenuItem.Name = "dynaNameSearcherToolStripMenuItem";
            resources.ApplyResources(this.dynaNameSearcherToolStripMenuItem, "dynaNameSearcherToolStripMenuItem");
            this.dynaNameSearcherToolStripMenuItem.Click += new System.EventHandler(this.dynaNameSearcherToolStripMenuItem_Click);
            // 
            // pickupSearcherToolStripMenuItem
            // 
            this.pickupSearcherToolStripMenuItem.Name = "pickupSearcherToolStripMenuItem";
            resources.ApplyResources(this.pickupSearcherToolStripMenuItem, "pickupSearcherToolStripMenuItem");
            this.pickupSearcherToolStripMenuItem.Click += new System.EventHandler(this.pickupSearcherToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.aboutToolStripMenuItem });
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.toolStripStatusLabel1, this.toolStripStatusLabel2, this.toolStripStatusLabelProject, this.toolStripStatusLabel3, this.toolStripStatusLabelTemplate, this.toolStripStatusLabel4, this.toolStripStatusLabelNumSelected });
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // toolStripStatusLabelProject
            // 
            this.toolStripStatusLabelProject.Name = "toolStripStatusLabelProject";
            resources.ApplyResources(this.toolStripStatusLabelProject, "toolStripStatusLabelProject");
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            resources.ApplyResources(this.toolStripStatusLabel3, "toolStripStatusLabel3");
            // 
            // toolStripStatusLabelTemplate
            // 
            this.toolStripStatusLabelTemplate.Name = "toolStripStatusLabelTemplate";
            resources.ApplyResources(this.toolStripStatusLabelTemplate, "toolStripStatusLabelTemplate");
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            resources.ApplyResources(this.toolStripStatusLabel4, "toolStripStatusLabel4");
            // 
            // toolStripStatusLabelNumSelected
            // 
            this.toolStripStatusLabelNumSelected.Name = "toolStripStatusLabelNumSelected";
            resources.ApplyResources(this.toolStripStatusLabelNumSelected, "toolStripStatusLabelNumSelected");
            // 
            // renderPanel
            // 
            resources.ApplyResources(this.renderPanel, "renderPanel");
            this.renderPanel.Name = "renderPanel";
            this.renderPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseClick);
            this.renderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseDown);
            this.renderPanel.MouseLeave += new System.EventHandler(this.renderPanel_MouseLeave);
            this.renderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveControl);
            this.renderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseUp);
            this.renderPanel.Resize += new System.EventHandler(this.ResetMouseCenter);
            // 
            // contextMenuStripMain
            // 
            this.contextMenuStripMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.gizmoToolStripMenuItem, this.toolStripMenuItem_Templates, this.userTemplateToolStripMenuItem });
            this.contextMenuStripMain.Name = "contextMenuStripMain";
            resources.ApplyResources(this.contextMenuStripMain, "contextMenuStripMain");
            // 
            // gizmoToolStripMenuItem
            // 
            this.gizmoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.positionToolStripMenuItem, this.rotationToolStripMenuItem, this.scaleToolStripMenuItem, this.positionLocalToolStripMenuItem });
            this.gizmoToolStripMenuItem.Name = "gizmoToolStripMenuItem";
            resources.ApplyResources(this.gizmoToolStripMenuItem, "gizmoToolStripMenuItem");
            // 
            // positionToolStripMenuItem
            // 
            this.positionToolStripMenuItem.Checked = true;
            this.positionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.positionToolStripMenuItem.Name = "positionToolStripMenuItem";
            resources.ApplyResources(this.positionToolStripMenuItem, "positionToolStripMenuItem");
            this.positionToolStripMenuItem.Click += new System.EventHandler(this.positionToolStripMenuItem_Click);
            // 
            // rotationToolStripMenuItem
            // 
            this.rotationToolStripMenuItem.Name = "rotationToolStripMenuItem";
            resources.ApplyResources(this.rotationToolStripMenuItem, "rotationToolStripMenuItem");
            this.rotationToolStripMenuItem.Click += new System.EventHandler(this.rotationToolStripMenuItem_Click);
            // 
            // scaleToolStripMenuItem
            // 
            this.scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
            resources.ApplyResources(this.scaleToolStripMenuItem, "scaleToolStripMenuItem");
            this.scaleToolStripMenuItem.Click += new System.EventHandler(this.scaleToolStripMenuItem_Click);
            // 
            // positionLocalToolStripMenuItem
            // 
            this.positionLocalToolStripMenuItem.Name = "positionLocalToolStripMenuItem";
            resources.ApplyResources(this.positionLocalToolStripMenuItem, "positionLocalToolStripMenuItem");
            this.positionLocalToolStripMenuItem.Click += new System.EventHandler(this.positionLocalToolStripMenuItem_Click);
            // 
            // toolStripMenuItem_Templates
            // 
            this.toolStripMenuItem_Templates.Name = "toolStripMenuItem_Templates";
            resources.ApplyResources(this.toolStripMenuItem_Templates, "toolStripMenuItem_Templates");
            // 
            // userTemplateToolStripMenuItem
            // 
            this.userTemplateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.toolStripComboBoxUserTemplate });
            this.userTemplateToolStripMenuItem.Name = "userTemplateToolStripMenuItem";
            resources.ApplyResources(this.userTemplateToolStripMenuItem, "userTemplateToolStripMenuItem");
            this.userTemplateToolStripMenuItem.Click += new System.EventHandler(this.userTemplateToolStripMenuItem_Click);
            // 
            // toolStripComboBoxUserTemplate
            // 
            this.toolStripComboBoxUserTemplate.Name = "toolStripComboBoxUserTemplate";
            resources.ApplyResources(this.toolStripComboBoxUserTemplate, "toolStripComboBoxUserTemplate");
            this.toolStripComboBoxUserTemplate.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxUserTemplate_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.renderPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MouseMoveControl);
            this.Move += new System.EventHandler(this.ResetMouseCenter);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStripMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolStripMenuItem showEditorsWhenLoadingProjectToolStripMenuItem;

        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelNumSelected;

        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crosshairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hiddenToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem cursorInFlyModeToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem createGameCubeBannerToolStripMenuItem;

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel renderPanel;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noCullingCToolStripMenuItem;
        private ToolStripMenuItem wireframeFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewConfigToolStripMenuItem;
        private ToolStripMenuItem archiveEditorToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem displayToolStripMenuItem;

        private ToolStripMenuItem colorsToolStripMenuItem;
        private ToolStripMenuItem backgroundColorToolStripMenuItem1;
        private ToolStripMenuItem widgetColorToolStripMenuItem;
        private ToolStripMenuItem selectionColorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem useLegacyAssetIDFormatToolStripMenuItem;
        private ToolStripMenuItem resetColorsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem autoSaveOnClosingToolStripMenuItem;
        private ToolStripMenuItem autoLoadOnStartupToolStripMenuItem;
        private ToolStripMenuItem mVPTColorToolStripMenuItem;
        private ToolStripMenuItem tRIGColorToolStripMenuItem;
        private ToolStripMenuItem sFXInColorToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem1;
        private ToolStripMenuItem uIModeToolStripMenuItem;
        private ToolStripMenuItem uIModeAutoSizeToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabelProject;
        public ContextMenuStrip contextMenuStripMain;
        private ToolStripMenuItem toolStripMenuItem_Templates;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripStatusLabel toolStripStatusLabelTemplate;
        private ToolStripMenuItem userTemplateToolStripMenuItem;
        private ToolStripComboBox toolStripComboBoxUserTemplate;
        private ToolStripMenuItem gizmoToolStripMenuItem;
        private ToolStripMenuItem positionToolStripMenuItem;
        private ToolStripMenuItem rotationToolStripMenuItem;
        private ToolStripMenuItem scaleToolStripMenuItem;
        private ToolStripMenuItem positionLocalToolStripMenuItem;
        private ToolStripMenuItem researchToolStripMenuItem;
        private ToolStripMenuItem eventSearchToolStripMenuItem;
        private ToolStripMenuItem assetIDGeneratorToolStripMenuItem;
        private ToolStripMenuItem useLODTForRenderingToolStripMenuItem;
        private ToolStripMenuItem usePIPTForRenderingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem saveAllOpenHIPsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageUserTemplatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem templatesPersistentShiniesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runGameF5ToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem downloadIndustrialParkEditorFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesOnEditorFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesOnStartupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesNowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ensureAssociationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem hideInvisibleMeshesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadVgmstreamToolStripMenuItem;
        private ToolStripMenuItem stopSoundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem templatesChainPointMVPTsToolStripMenuItem;
        private ToolStripMenuItem dYNASearchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSceneToolStripMenuItem;
        private ToolStripMenuItem discordRichPresenceToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem vSyncToolStripMenuItem;
        private ToolStripMenuItem lowerQualityGraphicsToolStripMenuItem;
        private ToolStripMenuItem assetTypesToolStripMenuItem;
        private ToolStripMenuItem movementPreviewToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem viewControlsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem openFolderToolStripMenuItem;
        private ToolStripMenuItem dynaNameSearcherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importLevelToolStripMenuItem;
        private ToolStripMenuItem closeAllEditorsToolStripMenuItem;
        private ToolStripMenuItem enableAllToolStripMenuItem;
        private ToolStripMenuItem disableAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparatorAssetTypes;
        private ToolStripMenuItem pickupSearcherToolStripMenuItem;
        private ToolStripMenuItem drawOnlyFirstMINFReferenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useLegacyAssetTypeFormatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateReferencesOnCopyPasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceAssetsOnPasteToolStripMenuItem;
        private ToolStripMenuItem addTextureFolderToolStripMenuItem;
        private ToolStripMenuItem addTXDArchiveToolStripMenuItem;
        private ToolStripMenuItem refreshTexturesAndModelsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem showVertexColorsToolStripMenuItem;
        private ToolStripMenuItem buildAndRunPS2ISOToolStripMenuItem;
        private ToolStripMenuItem openLastToolStripMenuItem;
    }
}

