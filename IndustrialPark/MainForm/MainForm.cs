using HipHopFile;
using IndustrialPark.Archive;
using IndustrialPark.AssetEditorColors;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class MainForm : Form
    {
        public static string pathToSettings => Application.StartupPath + "/ip_settings.json";

        private readonly IPSettings settings;
        private ScrollableMessageBox undoBox;
        private string currentProjectPath;
        public SharpRenderer renderer => Program.Renderer;
        public readonly GizmoManager gizmoManager;
        private List<ArchiveEditor> ArchiveEditors => Program.ArchiveManager.ArchiveEditors;
        private List<ToolStripMenuItem> templateButtons;
        public AssetTemplate CurrentAssetTemplate = AssetTemplate.Null;
        public string CurrentUserTemplate = "";
        public ConnectMarkers markerConnector;
        private bool ConfirmBeforeDelete = true;

        public MainForm(IPSettings settings)
        {
            StartPosition = FormStartPosition.CenterScreen;
            this.settings = settings;
            InitializeComponent();
            InitAdvancedDocking();

#if !DEBUG
            openFolderToolStripMenuItem.Visible = false;
            dynaNameSearcherToolStripMenuItem.Visible = false;
#endif

            SetUiAssetsVisibility(false);

            templateButtons = AssetFactoryService.PopulateTemplateMenusAt(contextTemplatesToolStripMenuItem, TemplateToolStripItemClick);

            Program.Renderer = new SharpRenderer(renderPanel);

            gizmoManager = new GizmoManager(() => ArchiveEditors.SelectMany(ae => ae.SelectedAssets));
            markerConnector = new ConnectMarkers(() => ArchiveEditors.SelectMany(ae => ae.SelectedAssets).OfType<AssetMRKR>());

            SetupToolbar();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateUserTemplateComboBox();

            if (settings.FirstTime)
            {
                if (AutomaticUpdater.UpdateIndustrialPark(out _))
                {
                    Close();
                    System.Diagnostics.Process.Start(Application.StartupPath + "/IndustrialPark.exe");
                    return;
                }
                // todo: change this message
                MessageBox.Show(
                    "It appears this is your first time using Industrial Park.\nPlease consult the documentation and user guide on the Heavy Iron Modding Wiki to understand how to use the tool if you haven't already.",
                    "Welcome!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Program.AboutBox.Show(this);
            }

            ApplyIPSettings(settings);

            SetProjectToolStripStatusLabel();
            StartRenderer();
            UpdateTitleBar();
            ResetMouseCenter(null, null);
            UpdateInitialDockingArea();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UnsavedChanges())
            {
                DialogResult result = MessageBox.Show(
                    "You appear to have unsaved changes in one of your Archive Editors. Do you wish to save them before closing?",
                    "Warning",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    SaveAllChanges();
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            TextureIOHelper.closeConverter();

            if (projectAutoSaveOnClosingToolStripMenuItem.Checked)
                SaveProject(currentProjectPath);
            IPSettings settings = SettingsFromCurrentInstance();

            File.WriteAllText(pathToSettings, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
#if !DEBUG
                try
                {
#endif
                AddArchiveEditor(file);
#if !DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error opening file: " + ex.Message);
                }
#endif
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(CreateGameCubeBanner))
                {
                    form.BringToFront();
                    form.Focus();
                }
            }
        }

        private void MenuStrip_DropDownOpened(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.DropDown != null)
            {
                if (typeof(ToolStrip)
                    .GetProperty("ToolTip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                    .GetValue(menuItem.DropDown) is ToolTip dropDownToolTip)
                {
                    dropDownToolTip.AutoPopDelay = 20000;
                }
            }
        }

        private void newEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(KeyboardAction.NewArchiveEditor);
        }

        private void openLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(KeyboardAction.OpenLevel);
        }

        private void closeAllEditorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges())
            {
                DialogResult result = MessageBox.Show("You appear to have unsaved changes in one of your Archive Editors. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    SaveAllChanges();
                else if (result == DialogResult.Cancel)
                    return;
            }

            ArchiveEditor[] editorsToClose = ArchiveEditors.ToArray();

            // close all editors
            foreach (var editor in editorsToClose)
            {
                editor.CloseArchiveEditor();
            }

            closeAllEditorsToolStripMenuItem.Enabled = false;
            SetAllAssetTypesVisible();
        }

        private void projectNewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges())
            {
                DialogResult result = MessageBox.Show("You appear to have unsaved changes in one of your Archive Editors. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    SaveAllChanges();
                else if (result == DialogResult.Cancel)
                    return;
            }

            // updates presence so that it doesn't get stuck on the previously focused window name
            DiscordRPCController.SetPresence(idling: true);

            currentProjectPath = null;
            ApplyProject(new ProjectJson());
            SetProjectToolStripStatusLabel();
            SetAllAssetTypesVisible();
        }

        private void projectOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges())
            {
                DialogResult result = MessageBox.Show("You appear to have unsaved changes in one of your Archive Editors. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    SaveAllChanges();
                else if (result == DialogResult.Cancel)
                    return;
            }

            // updates presence so that it doesn't get stuck on the previously focused window name
            DiscordRPCController.SetPresence(idling: true);

            OpenFileDialog openFile = new OpenFileDialog()
            { Filter = "JSON files|*.json" };

            if (openFile.ShowDialog() == DialogResult.OK)
                ApplyProject(openFile.FileName, projectShowEditorsWhenLoadingToolStripMenuItem.Checked);
        }

        private void projectSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentProjectPath))
                SaveProject();
            else
                projectSaveAsToolStripMenuItem_Click(null, null);
        }

        private void projectSaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            { Filter = "JSON files|*.json" };

            if (saveFile.ShowDialog() == DialogResult.OK)
                SaveProject(saveFile.FileName);
        }

        private void updateReferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateReferencesToolStripMenuItem.Checked = !updateReferencesToolStripMenuItem.Checked;
            ArchiveEditor.UpdateReferencesOnCopy = updateReferencesToolStripMenuItem.Checked;
        }

        private void replaceAssetsOnPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            replaceAssetsOnPasteToolStripMenuItem.Checked = !replaceAssetsOnPasteToolStripMenuItem.Checked;
            ArchiveEditor.ReplaceAssetsOnPaste = replaceAssetsOnPasteToolStripMenuItem.Checked;
        }

        private void useLegacyAssetTypeFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useLegacyAssetTypeFormatToolStripMenuItem.Checked = !useLegacyAssetTypeFormatToolStripMenuItem.Checked;
            AssetTypeContainer.LegacyAssetNameFormat = useLegacyAssetTypeFormatToolStripMenuItem.Checked;
            SetupAssetVisibilityButtons();
            UpdateToolbarVisibilityButtons();
            foreach (var ae in ArchiveEditors)
                ae.PopulateAssetListAndComboBox();
        }

        private void useLegacyAssetIDFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useLegacyAssetIDFormatToolStripMenuItem.Checked = !useLegacyAssetIDFormatToolStripMenuItem.Checked;
            HexUIntTypeConverter.Legacy = useLegacyAssetIDFormatToolStripMenuItem.Checked;
        }

        private void defaultToAllAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            defaultToAllAssetsToolStripMenuItem.Checked = !defaultToAllAssetsToolStripMenuItem.Checked;
            ArchiveEditor.AlwaysListAllAssets = defaultToAllAssetsToolStripMenuItem.Checked;
        }

        private void dockTargetArchiveEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockTargetArchiveEditorToolStripMenuItem.Checked = !dockTargetArchiveEditorToolStripMenuItem.Checked;
            AutoDockArchiveEditors = dockTargetArchiveEditorToolStripMenuItem.Checked;

            RefreshAssetListDock();
        }

        private void dockNewAssetEditorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dockNewAssetEditorsToolStripMenuItem.Checked = !dockNewAssetEditorsToolStripMenuItem.Checked;
            Program.EditorManager.AutoDockAssetEditors = dockNewAssetEditorsToolStripMenuItem.Checked;
        }

        private void openDockedEditorOnSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openDockedEditorOnSelectionToolStripMenuItem.Checked = !openDockedEditorOnSelectionToolStripMenuItem.Checked;
            Program.EditorManager.AutoDockSelectedAssetEditors = openDockedEditorOnSelectionToolStripMenuItem.Checked;
        }

        private void enableAssetToolButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableAssetToolButtonsToolStripMenuItem.Checked = !enableAssetToolButtonsToolStripMenuItem.Checked;
            InternalEditor.EnableAssetToolButtons = enableAssetToolButtonsToolStripMenuItem.Checked;
        }

        private void translucentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            translucentToolStripMenuItem.Checked = !translucentToolStripMenuItem.Checked;
            TranslucentWhenOutOfFocus = translucentToolStripMenuItem.Checked;

            foreach (var ae in ArchiveEditors)
            {
                ae.Opacity = (TranslucentWhenOutOfFocus && !ae.OwnedForms.Any() && !ae.Focused) ? 0.5f : 1f;
            }
            Program.EditorManager.SetAllOpacity(TranslucentWhenOutOfFocus);
        }

        private void rotationGizmoLocalAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotationGizmoLocalAxisToolStripMenuItem.Checked = !rotationGizmoLocalAxisToolStripMenuItem.Checked;
            gizmoManager.RotationGizmoLocalAxis = rotationGizmoLocalAxisToolStripMenuItem.Checked;
        }

        private void templatesPersistentShiniesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templatesPersistentShiniesToolStripMenuItem.Checked = !templatesPersistentShiniesToolStripMenuItem.Checked;
            AssetPKUP.persistentShinies = templatesPersistentShiniesToolStripMenuItem.Checked;
        }

        private void templatesChainPointMVPTsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templatesChainPointMVPTsToolStripMenuItem.Checked = !templatesChainPointMVPTsToolStripMenuItem.Checked;
            AssetMVPT.chainPointMVPTs = templatesChainPointMVPTsToolStripMenuItem.Checked;
        }

        private void manageUserTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.UserTemplateManager.Visible)
                Program.UserTemplateManager.Show(this);
            Program.UserTemplateManager.Activate();
        }

        public void discordRichPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            discordRichPresenceToolStripMenuItem.Checked = !discordRichPresenceToolStripMenuItem.Checked;
            DiscordRPCController.ToggleDiscordRichPresence(discordRichPresenceToolStripMenuItem.Checked);
        }

        #region Color Mode
        private SystemColorMode SystemColorMode
        {
            get;
            set
            {
                field = value;
                systemDefaultToolStripMenuItem.Checked = value == SystemColorMode.System;
                darkModeToolStripMenuItem.Checked = value == SystemColorMode.Dark;
                lightModeToolStripMenuItem.Checked = value == SystemColorMode.Classic;
            }
        }

        private void darkModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemColorMode = SystemColorMode.Dark;
        }

        private void systemDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemColorMode = SystemColorMode.System;
        }

        private void lightModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemColorMode = SystemColorMode.Classic;
        }
        #endregion

        private void enableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoShowDropDowns = false;
            if (assetViewToolStripMenuItems != null)
                foreach (var item in assetViewToolStripMenuItems)
                    if (!item.Checked)
                        item.PerformClick();
            autoShowDropDowns = true;
            ShowDropDowns();
        }

        private void disableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoShowDropDowns = false;
            if (assetViewToolStripMenuItems != null)
                foreach (var item in assetViewToolStripMenuItems)
                    if (item.Checked)
                        item.PerformClick();
            autoShowDropDowns = true;
            ShowDropDowns();
        }

        private void viewConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ViewConfig.WindowState == FormWindowState.Minimized)
                Program.ViewConfig.WindowState = FormWindowState.Normal;

            if (!Program.ViewConfig.Visible)
                Program.ViewConfig.Show(this);
            Program.ViewConfig.Activate();
        }

        private void refreshTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshTexturesAndModels();
        }

        private void uIModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uIModeToolStripMenuItem.Checked = !uIModeToolStripMenuItem.Checked;
            renderer.isDrawingUI = uIModeToolStripMenuItem.Checked;
            SetUiAssetsVisibility(renderer.isDrawingUI);
            renderer.Camera.Reset();
            mouseMode = false;
        }

        private void toolStripMenuItem116_Click(object sender, EventArgs e)
        {
            SetViewAspect(656f / 565f);
        }

        private void toolStripMenuItem43_Click(object sender, EventArgs e)
        {
            SetViewAspect(4f / 3f);
        }

        private void toolStripMenuItem169_Click(object sender, EventArgs e)
        {
            SetViewAspect(16f / 9f);
        }

        private void changeFlyModeCursor(object sender, EventArgs e)
        {
            setFlyCursor(Convert.ToInt32(((ToolStripMenuItem)sender).Tag));
        }

        private void resetColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.ResetColors();
            if (PressedKeys.Contains(Keys.ShiftKey))
                renderer.colors = RendererColors.Classic;
        }

        private void backgroundColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var colorDialog = AssetColorEditor.GetColorFromDialog(renderer.backgroundColor.ToVector4());
            if (colorDialog.Result == DialogResult.OK)
                renderer.backgroundColor = new SharpDX.Color(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B, colorDialog.Color.A);
        }

        private void selectionColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorDialog = AssetColorEditor.GetColorFromDialog(renderer.selectedColor);
            if (colorDialog.Result == DialogResult.OK)
                renderer.SetSelectionColor(colorDialog.Color);
        }

        private void widgetColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorDialog = AssetColorEditor.GetColorFromDialog(renderer.normalColor);
            if (colorDialog.Result == DialogResult.OK)
                renderer.SetWidgetColor(colorDialog.Color);
        }

        private void mVPTColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorDialog = AssetColorEditor.GetColorFromDialog(renderer.mvptColor);
            if (colorDialog.Result == DialogResult.OK)
                renderer.SetMvptColor(colorDialog.Color);
        }

        private void tRIGColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorDialog = AssetColorEditor.GetColorFromDialog(renderer.trigColor);
            if (colorDialog.Result == DialogResult.OK)
                renderer.SetTrigColor(colorDialog.Color);
        }

        private void sFXColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorDialog = AssetColorEditor.GetColorFromDialog(renderer.sfxColor);
            if (colorDialog.Result == DialogResult.OK)
                renderer.SetSfxColor(colorDialog.Color);
        }

        private void backfaceCullingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            backfaceCullingToolStripMenuItem.Checked = !backfaceCullingToolStripMenuItem.Checked;
            renderer.device.SetNormalCullMode(backfaceCullingToolStripMenuItem.Checked ? SharpDX.Direct3D11.CullMode.None : SharpDX.Direct3D11.CullMode.Back);
        }

        private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wireframeToolStripMenuItem.Checked = !wireframeToolStripMenuItem.Checked;
            renderer.device.SetNormalFillMode(wireframeToolStripMenuItem.Checked ? SharpDX.Direct3D11.FillMode.Wireframe : SharpDX.Direct3D11.FillMode.Solid);
            toolStripButtonWireframe.Checked = wireframeToolStripMenuItem.Checked;
        }

        private void lowerQualityGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.device.SetGraphicsMode(!renderer.device.currentMode);
            lowerQualityGraphicsToolStripMenuItem.Checked = !renderer.device.currentMode;
        }

        private void vSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.device.VSync = !renderer.device.VSync;
            vSyncToolStripMenuItem.Checked = renderer.device.VSync;
        }

        private void texturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.device.RenderTextures = texturesToolStripMenuItem.Checked;
        }

        private void vertexColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.RenderVertexColors = vertexColorsToolStripMenuItem.Checked;
        }

        private void lightKitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetLKIT.DontRender = !lightKitToolStripMenuItem.Checked;
        }

        private void fogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetFOG.DontRender = !fogToolStripMenuItem.Checked;
            if (AssetFOG.DontRender)
                renderer.Camera.FarPlane = SharpCamera.DEFAULT_FAR_PLANE;
        }

        private void movementPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            movementPreviewToolStripMenuItem.Checked = !movementPreviewToolStripMenuItem.Checked;
            EntityAsset.movementPreview = movementPreviewToolStripMenuItem.Checked;
            Program.ArchiveManager.ResetMovementPreview();
        }

        private void hideInvisibleGeometryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideInvisibleGeometryToolStripMenuItem.Checked = !hideInvisibleGeometryToolStripMenuItem.Checked;
            RenderWareModelFile.dontDrawInvisible = hideInvisibleGeometryToolStripMenuItem.Checked;
        }

        private void useLODTForRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useLODTForRenderingToolStripMenuItem.Checked = !useLODTForRenderingToolStripMenuItem.Checked;
            AssetMODL.renderBasedOnLodt = useLODTForRenderingToolStripMenuItem.Checked;
        }

        private void UsePIPTForRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            usePIPTForRenderingToolStripMenuItem.Checked = !usePIPTForRenderingToolStripMenuItem.Checked;
            AssetMODL.renderBasedOnPipt = usePIPTForRenderingToolStripMenuItem.Checked;
        }

        private void drawOnlyFirstMINFReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawOnlyFirstMINFReferenceToolStripMenuItem.Checked = !drawOnlyFirstMINFReferenceToolStripMenuItem.Checked;
            AssetMINF.drawOnlyFirst = drawOnlyFirstMINFReferenceToolStripMenuItem.Checked;
        }

        private void allMovePointsArePointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allMovePointsArePointsToolStripMenuItem.Checked = !allMovePointsArePointsToolStripMenuItem.Checked;
            AssetMVPT.DrawAllAsPoints = allMovePointsArePointsToolStripMenuItem.Checked;
            Program.Renderer.Assets.RecalculateAllMatrices(renderer);
        }

        private void wireframeWidgetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.WireframeWidgets = wireframeWidgetsToolStripMenuItem.Checked;
        }

        private void gizmoConeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gizmoConesToolStripMenuItem.Checked = !gizmoConesToolStripMenuItem.Checked;
            GizmoBase.UseCones = gizmoConesToolStripMenuItem.Checked;
        }

        private void saveAllOpenHIPsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAllChanges();
        }

        private void launchGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ae = ArchiveEditors.FirstOrDefault(ae => ae.archive?.LevelName.Length == 4 && ae.archive?.LevelName is not ("boot" or "font" or "plat"));
            try
            {
                if (ae == null)
                    throw new Exception("No Archive Editor containing a stage found.");
                RemoteControl.RunGame(ae.archive.Game, ae.GetCurrentFilePath());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to run game:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buildAndRunPS2ISOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.BuildISO ??= new BuildISO();
            if (!Program.BuildISO.Visible)
                Program.BuildISO.Show(this);
        }

        private void createGameCubeBannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bannerCreator = new CreateGameCubeBanner();
            bannerCreator.Show(this);
        }

        private void stopSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SoundUtility_vgmstream.StopSound();
        }

        private void exportSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assimp.ExportFormatDescription format = null;

            string textureExtension = null;
            while (format == null)
            {
                bool ok;
                (ok, format, textureExtension) = Models.ChooseTarget.GetTarget();

                if (!ok)
                {
                    format = null;
                    break;
                }

                if (format == null || textureExtension == null)
                    MessageBox.Show("Unsupported format for exporting scene");
            }

            if (format != null)
                using (CommonOpenFileDialog a = new CommonOpenFileDialog() { IsFolderPicker = true })
                    if (a.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Program.Renderer.Assets.ExportScene(a.FileName, format, textureExtension, out string[] textureNames);

                        if (MessageBox.Show("Do you also wish to export textures?", "Export Textures", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

                            foreach (var ae in ArchiveEditors.Where(ae => ae.archive != null))
                            {
                                var d = TextureConverter.GetTexturesAsBitmaps(ae.archive, textureNames);
                                foreach (var key in d)
                                    bitmaps.Add(key.Key, key.Value);
                            }

                            RenderWareFile.ReadFileMethods.treatStuffAsByteArray = false;

                            foreach (string textureName in bitmaps.Keys)
                                bitmaps[textureName].Save(Path.Combine(a.FileName, textureName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
        }

        private void CheckForUpdatesNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AutomaticUpdater.UpdateIndustrialPark(out bool hasChecked, true))
            {
                Close();
                System.Diagnostics.Process.Start(Application.StartupPath + "/IndustrialPark.exe");
            }
            else if (hasChecked)
                MessageBox.Show("No update found.");
            else
                MessageBox.Show("Unable to check for updates.");
        }

        private void DownloadIndustrialParkEditorFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutomaticUpdater.DownloadEditorFiles();
        }

        private void CheckForUpdatesOnEditorFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!AutomaticUpdater.VerifyEditorFiles())
                MessageBox.Show("No update found.");
        }

        private void downloadVgmstreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SoundUtility_vgmstream.DownloadVgmstream();
        }

        private void ensureAssociationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show(
                "Will set Industrial Park as default application for HIP, HOP and BNR (GameCube banner) file formats on registry.",
                "Associate HIP/HOP files",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information))
                FileAssociations.FileAssociations.EnsureAssociationsSet();
        }

        private void setDolphinPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog()
            {
                Filter = "Executables|*.exe",
                Title = "Please select your Dolphin executable or a shortcut"
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                RemoteControl.DolphinPath = openFile.FileName;
                MessageBox.Show($"Dolphin path set successfully:\n{RemoteControl.DolphinPath}", "Dolphin path set", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void viewControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ViewControls.WindowState == FormWindowState.Minimized)
                Program.ViewControls.WindowState = FormWindowState.Normal;

            if (!Program.ViewControls.Visible)
                Program.ViewControls.Show(this);
            Program.ViewControls.Activate();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Program.AboutBox.Visible)
                Program.AboutBox.Show(this);
            Program.AboutBox.Activate();
        }

        private void viewUndoBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoBox == null)
            {
                undoBox = new ScrollableMessageBox("Undo Buffer", Program.UndoBuffer.ToString(), true);
                Program.UndoBuffer.Changed += () => undoBox.richTextBox1.Text = Program.UndoBuffer.ToString();
            }
            if (!undoBox.Visible)
                undoBox.Show(this);
            undoBox.Activate();
            undoBox.richTextBox1.Text = Program.UndoBuffer.ToString();
        }

        private void assetIDGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.AssetIDGenerator ??= new AssetIDGenerator();
            if (!Program.AssetIDGenerator.Visible)
                Program.AssetIDGenerator.Show(this);
        }

        private void hansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.HansMainForm ??= new IndustrialPark.SaveFile.HansMainForm();
            if (!Program.HansMainForm.Visible)
                Program.HansMainForm.Show(this);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var chooseFolder = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };
            if (chooseFolder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var dirList = new List<string> { chooseFolder.FileName };
                foreach (var s in Directory.GetDirectories(chooseFolder.FileName))
                    dirList.Add(s);

                var hipList = new List<string>();
                foreach (var s in dirList)
                    foreach (var ss in Directory.GetFiles(s))
                    {
                        var extensionToLower = Path.GetExtension(ss).ToLower();
                        if (extensionToLower.Equals(".hip") || extensionToLower.Equals(".hop"))
                            hipList.Add(ss);
                    }

                var scoobyPlat = Platform.Unknown;

                var p = new ProgressBar("Opening files...");
                p.SetProgressBar(0, hipList.Count, 1);
                p.Show(this);

                var layerLogs = new List<string>();
                var layerTypes = new Dictionary<LayerType, HashSet<AssetType>>();

                foreach (var hip in hipList)
                {
                    p.Text = Path.GetFileName(hip);
                    HipFile hipFile = HipFile.FromPath(hip);
                    if (hipFile.platform == Platform.Unknown && scoobyPlat != Platform.Unknown)
                        hipFile.platform = scoobyPlat;
                    var archive = new Archive.Archive(hipFile, hip, true, out _);
                    scoobyPlat = archive.Platform;

                    for (int i = 0; i < archive.LayerCount; i++)
                    {
                        var lType = archive.GetLayerTypeGeneric(i);
                        if (!layerTypes.ContainsKey(lType))
                            layerTypes[lType] = new HashSet<AssetType>();
                        foreach (var t in archive.GetLayerAssetIDs(i).Select(a => archive.GetAsset(a).assetType).ToHashSet())
                            layerTypes[lType].Add(t);
                    }

                    archive.Dispose();
                    p.PerformStep();
                }

                File.WriteAllText("layerslogs.txt", string.Join("\n", layerTypes.OrderBy(l => (int)l.Key).Select(k => $"{k.Key}: {string.Join(", ", k.Value.Select(v => v.ToString()).OrderBy(f => f))}")));

                p.Close();
            }
        }

        private void eventSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.EventSearch ??= new EventSearch();
            if (!Program.EventSearch.Visible)
                Program.EventSearch.Show(this);
        }

        private void dYNASearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.DynaSearch ??= new DynaSearch();
            if (!Program.DynaSearch.Visible)
                Program.DynaSearch.Show(this);
        }

        private void dynaNameSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var unkDtypes = new uint[] {
                    0xEBC04E7B,
                };

                string ReadZeroTerminatedString(EndianBinaryReader r)
                {
                    var bytes = new List<char>();
                    while (!r.EndOfStream)
                    {
                        var b = r.ReadByte();
                        if (b != 0)
                            bytes.Add((char)b);
                        else
                            break;
                    }

                    return new string(bytes.ToArray());
                }

                byte[] file = File.ReadAllBytes(openFileDialog.FileName);
                EndianBinaryReader reader = new EndianBinaryReader(file, Endianness.Little);
                while (!reader.EndOfStream)
                {
                    var str = ReadZeroTerminatedString(reader);
                    var hash = Functions.BKDRHash(str);
                    if (unkDtypes.Contains(hash))
                        MessageBox.Show("Found: [" + str + "] " + hash.ToString("X8"));
                }
                MessageBox.Show("Finished");
            }
        }

        private void pickupSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.PickupSearch ??= new PickupSearch();
            if (!Program.PickupSearch.Visible)
                Program.PickupSearch.Show(this);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            viewConfigToolStripMenuItem.PerformClick();
        }

        private void positionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGizmoMode(GizmoMode.Position);
        }

        private void rotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGizmoMode(GizmoMode.Rotation);
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGizmoMode(GizmoMode.Scale);
        }

        private void positionLocalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGizmoMode(GizmoMode.PositionLocal);
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGizmoMode(GizmoMode.None);
        }

        private void userTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUserTemplateComboBox();
        }

        private void toolStripComboBoxUserTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxUserTemplate.SelectedIndex != -1)
            {
                UnselectTemplateButtons();
                CurrentAssetTemplate = AssetTemplate.User_Template;
                CurrentUserTemplate = toolStripComboBoxUserTemplate.SelectedItem.ToString();
                toolStripStatusLabelTemplate.Text = $"Template: {toolStripComboBoxUserTemplate.SelectedItem} (User)";
            }
        }

        private void contextUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.UndoBuffer.Undo();
        }

        private void contextRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.UndoBuffer.Redo();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(KeyboardAction.HideSelectedAssets);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(KeyboardAction.Copy);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(KeyboardAction.Paste);
        }

        private void contextEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(KeyboardAction.OpenInternalEditors);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleAction(KeyboardAction.Delete);
        }

        private void connectMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (markerConnector.SetupConnectMarkers())
            {
                if (!markerConnector.Visible)
                    markerConnector.Show(this);
                markerConnector.Activate();
            }
            else
            {
                MessageBox.Show("You must have exactly 2 markers selected to use Connect Markers.");
                markerConnector.Hide();
            }
        }

        public void UpdateTitleBar()
        {
            if (IsDisposed)
                return;

            char startOfArchiveList = '[';
            char endOfArchiveList = ']';
            char archiveDelimiter = ',';
            char unsavedChanges = '*';

            StringBuilder builder = new StringBuilder();

            if (ArchiveEditors.Count > 0)
            {
                builder.Append(startOfArchiveList);
                for (int i = 0; i < ArchiveEditors.Count; i++)
                {
                    builder.Append(Path.GetFileName(ArchiveEditors[i].GetCurrentFilePath()));

                    // Unsaved changes
                    if (ArchiveEditors[i].UnsavedChanges)
                        builder.Append(unsavedChanges);

                    // Separator
                    if (i < ArchiveEditors.Count - 1)
                        builder.Append(archiveDelimiter + " ");
                }
                builder.Append(endOfArchiveList);
                builder.Append(" - ");
            }

            // Program name and version
            builder.Append("Industrial Park ");
            builder.Append(new IPversion().version);

#if DEBUG
            builder.Append(" (Debug)");
#endif

            // Prevents a crash if form is updated from a different thread.
            if (InvokeRequired)
            {
                Action<string> updateTitleSafe = (string s) => Text = s;
                Invoke(updateTitleSafe, builder.ToString());
            }
            else
            {
                Text = builder.ToString();
            }
        }

        public void SetToolStripStatusLabel(string Text)
        {
            toolStripStatusLabelMain.Text = Text;
        }

        private void StartRenderer()
        {
            new Thread(() =>
            {
                if (InvokeRequired)
                    Invoke(renderer.RunMainLoop, renderPanel);
                else
                    renderer.RunMainLoop(renderPanel);
            }).Start();
        }

        private void ApplyIPSettings(IPSettings settings)
        {
            SystemColorMode = settings.ColorMode;

            if (settings.CheckForUpdatesOnStartup && AutomaticUpdater.UpdateIndustrialPark(out _))
            {
                Close();
                System.Diagnostics.Process.Start(Application.StartupPath + "/IndustrialPark.exe");
            }

            projectAutoSaveOnClosingToolStripMenuItem.Checked = settings.AutosaveOnClose;
            projetcAutoLoadOnStartupToolStripMenuItem.Checked = settings.AutoloadOnStartup;

            checkForUpdatesDailyToolStripMenuItem.Checked = settings.CheckForUpdatesOnStartup;
            AutomaticUpdater.LastCheckedForUpdate = settings.LastCheckedForUpdate;

            drawOnlyFirstMINFReferenceToolStripMenuItem.Checked = settings.drawOnlyFirstMinf;
            AssetMINF.drawOnlyFirst = settings.drawOnlyFirstMinf;

            useLODTForRenderingToolStripMenuItem.Checked = settings.renderBasedOnLodt;
            AssetMODL.renderBasedOnLodt = settings.renderBasedOnLodt;

            usePIPTForRenderingToolStripMenuItem.Checked = settings.renderBasedOnPipt;
            AssetMODL.renderBasedOnPipt = settings.renderBasedOnPipt;

            allMovePointsArePointsToolStripMenuItem.Checked = settings.drawMvptsAsPoints;
            AssetMVPT.DrawAllAsPoints = settings.drawMvptsAsPoints;

            hideInvisibleGeometryToolStripMenuItem.Checked = settings.dontDrawInvisible;
            RenderWareModelFile.dontDrawInvisible = settings.dontDrawInvisible;

            SetToolbarVisible(settings.showToolbar);
            if (settings.toolbarAssetTypes != null)
                SetToolbarAssetTypes(ParseToolbarAssetTypes(settings.toolbarAssetTypes));

            templatesPersistentShiniesToolStripMenuItem.Checked = settings.persistentShinies;
            AssetPKUP.persistentShinies = settings.persistentShinies;

            useLegacyAssetIDFormatToolStripMenuItem.Checked = settings.LegacyAssetIDFormat;
            HexUIntTypeConverter.Legacy = settings.LegacyAssetIDFormat;

            useLegacyAssetTypeFormatToolStripMenuItem.Checked = settings.LegacyAssetTypeFormat;
            AssetTypeContainer.LegacyAssetNameFormat = settings.LegacyAssetTypeFormat;

            discordRichPresenceToolStripMenuItem.Checked = settings.discordRichPresence;
            DiscordRPCController.ToggleDiscordRichPresence(discordRichPresenceToolStripMenuItem.Checked);

            ArchiveEditor.AlwaysListAllAssets = settings.AlwaysListAllAssets;
            defaultToAllAssetsToolStripMenuItem.Checked = settings.AlwaysListAllAssets;

            gizmoManager.RotationGizmoLocalAxis = settings.RotationGizmoLocalAxis;
            rotationGizmoLocalAxisToolStripMenuItem.Checked = settings.RotationGizmoLocalAxis;

            enableAssetToolButtonsToolStripMenuItem.Checked = settings.EnableAssetToolButtons;
            InternalEditor.EnableAssetToolButtons = settings.EnableAssetToolButtons;

            AutoDockArchiveEditors = settings.AutoDockArchiveEditor;
            dockTargetArchiveEditorToolStripMenuItem.Checked = ShowToolbar && AutoDockArchiveEditors;

            dockNewAssetEditorsToolStripMenuItem.Checked = settings.AutoDockAssetEditors;
            Program.EditorManager.AutoDockAssetEditors = settings.AutoDockAssetEditors;

            openDockedEditorOnSelectionToolStripMenuItem.Checked = settings.AutoDockSelectedAssetEditors;
            Program.EditorManager.AutoDockSelectedAssetEditors = settings.AutoDockSelectedAssetEditors;

            gizmoConesToolStripMenuItem.Checked = settings.GizmoCones;
            GizmoBase.UseCones = settings.GizmoCones;

            wireframeWidgetsToolStripMenuItem.Checked = settings.WireframeWidgets;
            renderer.WireframeWidgets = settings.WireframeWidgets;

            fogToolStripMenuItem.Checked = settings.fog;
            AssetFOG.DontRender = !settings.fog;

            renderer.colors = settings.Colors;

            dockLeftSizeInit = settings.dockLeftSize;
            dockRightSizeInit = settings.dockRightSize;

            BuildISO.PCSX2Path = settings.pcsx2Path;
            BuildISO.recentGameDirPaths = settings.recentBuildIsoGamePaths;
            RemoteControl.DolphinPath = settings.DolphinPath;

            renderer.Camera.Speed = settings.Speed > 0 ? settings.Speed : SharpCamera.DEFAULT_SPEED;
            renderer.Camera.SpeedRot = settings.SpeedRot > 0 ? settings.SpeedRot : SharpCamera.DEFAULT_SPEED_ROT;

            bool showEditorsWhenLoadingProject = settings.showEditorsWhenLoadingProject;
            projectShowEditorsWhenLoadingToolStripMenuItem.Checked = showEditorsWhenLoadingProject;

            setFlyCursor(settings.flyModeCursor);
            TranslucentWhenOutOfFocus = settings.translucentEditor;
            translucentToolStripMenuItem.Checked = TranslucentWhenOutOfFocus;

            foreach (var kb in settings.UserKeyBindings ?? IpModernBindings)
                SetKeyBind(kb.Action, kb.Binding, false);

            if (settings.recentArchivePaths != null)
                foreach (string filepath in settings.recentArchivePaths)
                    SetRecentOpenedArchives(filepath);

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (Path.GetExtension(args[1]).ToLower() == ".hip" || Path.GetExtension(args[1]).ToLower() == ".hop")
                {
                    AddArchiveEditor(args[1]);
                    SetProjectToolStripStatusLabel();
                    return;
                }
                if (Path.GetExtension(args[1]).ToLower() == GameCubeBanner.DEFAULT_FILE_EXTENSION)
                {
                    CreateGameCubeBanner b = new CreateGameCubeBanner(args[1]);
                    b.Show(this);
                    return;
                }
            }

            if (settings.AutoloadOnStartup && !string.IsNullOrEmpty(settings.LastProjectPath) && File.Exists(settings.LastProjectPath))
                ApplyProject(settings.LastProjectPath, showEditorsWhenLoadingProject);
        }

        private IPSettings SettingsFromCurrentInstance() => new IPSettings
        {
            FirstTime = false,
            AutosaveOnClose = projectAutoSaveOnClosingToolStripMenuItem.Checked,
            AutoloadOnStartup = projetcAutoLoadOnStartupToolStripMenuItem.Checked,
            LastProjectPath = currentProjectPath,
            CheckForUpdatesOnStartup = checkForUpdatesDailyToolStripMenuItem.Checked,
            LastCheckedForUpdate = AutomaticUpdater.LastCheckedForUpdate,
            drawOnlyFirstMinf = AssetMINF.drawOnlyFirst,
            renderBasedOnLodt = AssetMODL.renderBasedOnLodt,
            renderBasedOnPipt = AssetMODL.renderBasedOnPipt,
            drawMvptsAsPoints = AssetMVPT.DrawAllAsPoints,
            discordRichPresence = discordRichPresenceToolStripMenuItem.Checked,
            dontDrawInvisible = RenderWareModelFile.dontDrawInvisible,
            showToolbar = showToolbarToolStripMenuItem.Checked,
            toolbarAssetTypes = toolbarAssetTypes.Select(t => t.ToString()).ToArray(),
            persistentShinies = AssetPKUP.persistentShinies,
            LegacyAssetIDFormat = HexUIntTypeConverter.Legacy,
            LegacyAssetTypeFormat = AssetTypeContainer.LegacyAssetNameFormat,
            pcsx2Path = BuildISO.PCSX2Path,
            recentBuildIsoGamePaths = BuildISO.recentGameDirPaths,
            recentArchivePaths = recentArchivesToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().Select(x => x.Text).ToArray(),
            flyModeCursor = (int)flyModeCursor,
            translucentEditor = TranslucentWhenOutOfFocus,
            showEditorsWhenLoadingProject = projectShowEditorsWhenLoadingToolStripMenuItem.Checked,
            ColorMode = SystemColorMode,
            AlwaysListAllAssets = ArchiveEditor.AlwaysListAllAssets,
            RotationGizmoLocalAxis = gizmoManager.RotationGizmoLocalAxis,
            EnableAssetToolButtons = InternalEditor.EnableAssetToolButtons,
            AutoDockArchiveEditor = AutoDockArchiveEditors,
            AutoDockAssetEditors = Program.EditorManager.AutoDockAssetEditors,
            AutoDockSelectedAssetEditors = Program.EditorManager.AutoDockSelectedAssetEditors,
            GizmoCones = GizmoBase.UseCones,
            WireframeWidgets = renderer.WireframeWidgets,
            dockLeftSize = dockLeftSizeSave,
            dockRightSize = dockRightSizeSave,
            Speed = renderer.Camera.Speed,
            SpeedRot = renderer.Camera.SpeedRot,
            fog = !AssetFOG.DontRender,
            DolphinPath = RemoteControl.DolphinPath,
            Colors = renderer.colors,
            UserKeyBindings = [.. UserKeyBindings],
        };

        private void SaveProject(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = Path.Combine(Application.StartupPath, "default_project.json");
            currentProjectPath = fileName;
            SaveProject();
        }

        private void SaveProject()
        {
            File.WriteAllText(currentProjectPath, JsonConvert.SerializeObject(FromCurrentInstance(), Formatting.Indented));
            SetProjectToolStripStatusLabel();
        }

        public ProjectJson FromCurrentInstance()
        {
            List<string> hips = new List<string>();
            List<Platform> platforms = new List<HipHopFile.Platform>();
            List<uint> hiddenAssets = new List<uint>();

            foreach (ArchiveEditor ae in ArchiveEditors)
                if (ae.archive != null && !string.IsNullOrEmpty(ae.GetCurrentFilePath()))
                {
                    hips.Add(ae.GetCurrentFilePath());
                    platforms.Add(ae.archive.Platform);
                    hiddenAssets.AddRange(ae.archive.Assets.Where(a => a.isInvisible).Select(a => a.assetID));
                }

            var dontRender = new Dictionary<string, bool>();
            foreach (var assetType in assetViewTypes.Keys)
            {
                var type = assetViewTypes[assetType];
                var prop = type.GetField("dontRender");
                dontRender[assetType.ToString()] = (bool)prop.GetValue(null);
            }

            return new ProjectJson()
            {
                hipPaths = hips,
                scoobyPlatforms = platforms,
                TextureFolderPaths = Program.TextureManager.OpenTextureFolders.ToList(),
                CamPos = renderer.Camera.Position,
                Yaw = renderer.Camera.Yaw,
                Pitch = renderer.Camera.Pitch,
                FieldOfView = renderer.Camera.FieldOfView,
                FarPlane = renderer.Camera.FarPlane,
                BackgroundColor = Program.Renderer.backgroundColor,
                Colors = renderer.colors,
                hiddenAssets = hiddenAssets,
                isDrawingUI = renderer.isDrawingUI,
                Grid = gizmoManager.Grid,
                dontRender = dontRender,
            };
        }

        private void ApplyProject(string projectPath, bool showEditors = true)
        {
            currentProjectPath = projectPath;
            SetProjectToolStripStatusLabel();
            ApplyProject(JsonConvert.DeserializeObject<ProjectJson>(File.ReadAllText(projectPath)), showEditors);
        }

        private void ApplyProject(ProjectJson project, bool showEditors = true)
        {
            if (project.version != ProjectJson.getCurrentVersion)
                MessageBox.Show("You are trying to open a project file made with a different version of Industrial Park. The program will attempt to load the project, but there is a chance it will not load properly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            Program.TextureManager.ClearTextures();

            List<ArchiveEditor> archiveEditors = [.. this.ArchiveEditors];
            foreach (ArchiveEditor ae in archiveEditors)
                ae.CloseArchiveEditor();

            Program.TextureManager.LoadTexturesFromFolder(project.TextureFolderPaths);

            for (int i = 0; i < project.hipPaths.Count; i++)
                if (project.hipPaths[i] == "Empty")
                    continue;
                else
                {
                    if (File.Exists(project.hipPaths[i]))
                        AddArchiveEditor(project.hipPaths[i], project.scoobyPlatforms[i], showEditors, false);
                    else
                    {
                        MessageBox.Show("Error opening " + project.hipPaths[i] + ": file not found",
                            "File not Found",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }

            RefreshTexturesAndModels();

            if (project.hiddenAssets.Count != 0)
                foreach (var ae in archiveEditors)
                    ae.HideAssets(project.hiddenAssets);

            renderer.Camera.SetPosition(project.CamPos);
            renderer.Camera.Yaw = project.Yaw;
            renderer.Camera.Pitch = project.Pitch;
            renderer.Camera.FieldOfView = project.FieldOfView;
            renderer.Camera.FarPlane = project.FarPlane;
            gizmoManager.Grid = project.Grid;

            renderer.backgroundColor = project.BackgroundColor;

            renderer.colors = project.Colors;

            uIModeToolStripMenuItem.Checked = project.isDrawingUI;
            renderer.isDrawingUI = project.isDrawingUI;

            if (assetViewToolStripMenuItems != null)
                for (int i = 0; i < assetViewToolStripMenuItems.Length; i++)
                {
                    AssetType type = (AssetType)assetViewToolStripMenuItems[i].Tag;
                    var code = type.ToString();
                    bool value = project.dontRender.ContainsKey(code) ? project.dontRender[code] : true;
                    assetViewToolStripMenuItems[i].Checked = !value;
                    assetViewTypes[type].GetField("dontRender").SetValue(null, value);
                }

            UpdateToolbarVisibilityButtons();
        }
        private void SetProjectToolStripStatusLabel()
        {
            toolStripStatusLabelProject.Text = "Project: " + (currentProjectPath ?? "None");
        }

        public ArchiveEditor AddArchiveEditor(string filePath = null, Platform scoobyPlatform = Platform.Unknown, bool show = true, bool refreshTexturesModels = true)
        {
            var ae = new ArchiveEditor(filePath, scoobyPlatform);
            ArchiveEditors.Add(ae);

            ToolStripMenuItem tempMenuItem = new ToolStripMenuItem(Path.GetFileName(ae.GetCurrentFilePath()));
            tempMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Program.ArchiveManager.ShowArchiveEditor(ae, this);
            });
            archiveEditorToolStripMenuItem.DropDownItems.Add(tempMenuItem);

            ae.ChangesMade += UpdateTitleBar;
            ae.SubscribeToSelectedAssetsCollectionChanged(UpdateSelectedAssetStatusBarItem);
            ae.EditorUpdate += EditorUpdate;
            ae.ToolbarStateChanged += ArchiveEditor_ToolbarStateChanged;
            if (filePath != null)
            {
                UpdateTitleBar();
                SetupAssetVisibilityButtons();
            }
            closeAllEditorsToolStripMenuItem.Enabled = true;
            UpdateToolbarArchives();

            ae.Show(this);

            // Cannot select assets if the editor isn't shown first?
            if (!show)
                ae.Hide();

            if (refreshTexturesModels)
                RefreshTexturesAndModels();

            return ae;
        }

        public void EditorUpdate()
        {
            closeAllEditorsToolStripMenuItem.Enabled = ArchiveEditors.Count > 0;
            RefreshTexturesAndModels();
            SetupAssetVisibilityButtons();
        }

        public void SetToolStripItemName(ArchiveEditor sender, string newName)
        {
            archiveEditorToolStripMenuItem.DropDownItems[ArchiveEditors.IndexOf(sender) + 5].Text = newName;
            UpdateToolbarArchives();
        }

        public void SetRecentOpenedArchives(string filepath)
        {
            if (!recentArchivesToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().Any(x => x.Text == filepath))
            {
                ToolStripMenuItem item = new ToolStripMenuItem(filepath);
                item.Click += (s, e) =>
                {
                    recentArchivesToolStripMenuItem.DropDownItems.Remove(item);
                    if (File.Exists(filepath))
                    {
                        recentArchivesToolStripMenuItem.DropDownItems.Add(item);
                        AddArchiveEditor(filepath);
                    }
                    else
                        MessageBox.Show("File does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
                recentArchivesToolStripMenuItem.DropDownItems.Add(item);
            }
            while (recentArchivesToolStripMenuItem.DropDownItems.Count > 15)
                recentArchivesToolStripMenuItem.DropDownItems.RemoveAt(0);
        }

        public void CloseArchiveEditor(ArchiveEditor sender)
        {
            int index = ArchiveEditors.IndexOf(sender);
            archiveEditorToolStripMenuItem.DropDownItems.RemoveAt(index + 5);
            ArchiveEditors.RemoveAt(index);
            Program.UndoBuffer.RemoveActionsOfArchive(sender);
            UpdateToolbarArchives();
        }

        public void SetCloseAllArchivesEnabled(bool enabled)
        {
            closeAllEditorsToolStripMenuItem.Enabled = enabled;
        }

        private bool UnsavedChanges()
        {
            foreach (ArchiveEditor ae in ArchiveEditors)
                if (ae.UnsavedChanges)
                    return true;

            return false;
        }

        private void SaveAllChanges()
        {
            foreach (var ae in ArchiveEditors)
                if (ae.UnsavedChanges)
                    ae.Save();
        }

        // Close all forms that are not Archive Editors
        private static void CloseMostForms()
        {
            var openForms = Application.OpenForms;
            var formsToClose = new List<Form>();

            for (int i = 0; i < openForms.Count; i++)
                if (openForms[i].GetType() != typeof(MainForm) && openForms[i].GetType() != typeof(ArchiveEditor))
                    formsToClose.Add(openForms[i]);

            foreach (Form form in formsToClose)
                form.Close();
        }

        private SharpDX.Rectangle ViewRectangle => new SharpDX.Rectangle(
            renderPanel.ClientRectangle.X,
            renderPanel.ClientRectangle.Y,
            renderPanel.ClientRectangle.Width,
            renderPanel.ClientRectangle.Height);

        /// <summary>
        /// Updates the status bar to show the number of selected assets.
        /// </summary>
        private void UpdateSelectedAssetStatusBarItem()
        {
            toolStripStatusLabelNumSelected.Text = $"{Program.ArchiveManager.GetNumberOfSelectedAssets()} selected";
        }

        private bool autoShowDropDowns = true;

        private void ShowDropDowns()
        {
            UpdateToolbarVisibilityButtons();

            if (autoShowDropDowns)
                BeginInvoke(() =>
                {
                    displayToolStripMenuItem.ShowDropDown();
                    assetTypesToolStripMenuItem.ShowDropDown();
                });
        }

        public void SetupAssetVisibilityButtons()
        {
            assetTypesToolStripMenuItem.DropDownItems.Clear();
            assetTypesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                enableAllToolStripMenuItem,
                disableAllToolStripMenuItem,
                toolStripSeparatorAssetTypes
            });

            var assetTypes = new HashSet<AssetType>();
            foreach (var ae in ArchiveEditors.Where(ae => ae.archive is not null))
                foreach (var assetType in ae.archive.AssetTypes)
                    assetTypes.Add(assetType);

            assetViewToolStripMenuItems = GetAssetViewToolStripMenuItems(assetTypes);
            assetTypesToolStripMenuItem.DropDownItems.AddRange(assetViewToolStripMenuItems);
        }

        private static Dictionary<AssetType, Type> assetViewTypes = new Dictionary<AssetType, Type>
        {
            { AssetType.JSP, typeof(AssetJSP) },
            { AssetType.BSP, typeof(AssetJSP) },
            { AssetType.Button, typeof(AssetBUTN) },
            { AssetType.Boulder, typeof(AssetBOUL) },
            { AssetType.Camera, typeof(AssetCAM) },
            { AssetType.DestructibleObject, typeof(AssetDSTR) },
            { AssetType.DashTrack, typeof(AssetDTRK) },
            { AssetType.ElectricArcGenerator, typeof(AssetEGEN) },
            { AssetType.GrassMesh, typeof(AssetGRSM) },
            { AssetType.Hangable, typeof(AssetHANG) },
            { AssetType.Light, typeof(AssetLITE) },
            { AssetType.Marker, typeof(AssetMRKR) },
            { AssetType.MovePoint, typeof(AssetMVPT) },
            { AssetType.Villain, typeof(AssetNPC) },
            { AssetType.Pendulum, typeof(AssetPEND) },
            { AssetType.Pickup, typeof(AssetPKUP) },
            { AssetType.Platform, typeof(AssetPLAT) },
            { AssetType.Player, typeof(AssetPLYR) },
            { AssetType.SFX, typeof(AssetSFX) },
            { AssetType.SDFX, typeof(AssetSDFX) },
            { AssetType.SimpleObject, typeof(AssetSIMP) },
            { AssetType.Track, typeof(AssetSIMP) },
            { AssetType.Spline, typeof(AssetSPLN) },
            { AssetType.Trigger, typeof(AssetTRIG) },
            { AssetType.UserInterface, typeof(AssetUI) },
            { AssetType.UserInterfaceFont, typeof(AssetUIFT) },
            { AssetType.NPC, typeof(AssetVIL) },
            { AssetType.Volume, typeof(AssetVOLU) },

            { AssetType.CameraPreset, typeof(DynaCameraPreset) },
            { AssetType.LightEffect, typeof(DynaEffectLight) },
            { AssetType.Springboard, typeof(DynaCObjectSpringBoard) },
            { AssetType.ScreenWarp, typeof(DynaEffectScreenWarp) },
            { AssetType.FlameEmitter, typeof(DynaGObjectFlameEmitter) },
            { AssetType.Flamethrower, typeof(DynaEffectFlamethrower) },
            { AssetType.IncrediblesIcon, typeof(DynaIncrediblesIcon) },
            { AssetType.IncrediblesPickup, typeof(DynaGObjectInPickup) },
            { AssetType.InterestPointer, typeof(DynaInterestPointer) },
            { AssetType.NPCCoverPoint, typeof(DynaNPCCoverpoint) },
            { AssetType.HUDCompassObject, typeof(DynaHudCompassObject) },
            { AssetType.Lightning, typeof(DynaEffectLightning) },
            { AssetType.ParticleGenerator, typeof(DynaEffectParticleGenerator) },
            { AssetType.Pointer, typeof(DynaPointer) },
            { AssetType.Ring, typeof(DynaGObjectRing) },
            { AssetType.RumbleSphericalEmitter, typeof(DynaEffectRumbleSphere) },
            { AssetType.SmokeEmitter, typeof(DynaEffectSmokeEmitter) },
            { AssetType.SparkEmitter, typeof(DynaEffectSparkEmitter) },
            { AssetType.TeleportBox, typeof(DynaGObjectTeleport) },
            { AssetType.Vent, typeof(DynaGObjectVent) },

            { AssetType.Spawner, typeof(DynaEnemyBucketOTron) },
            { AssetType.CastNCrew, typeof(DynaEnemyCastNCrew) },
            { AssetType.Critter, typeof(DynaEnemyCritter) },
            { AssetType.Dennis, typeof(DynaEnemyDennis) },
            { AssetType.FrogFish, typeof(DynaEnemyFrogFish) },
            { AssetType.Mindy, typeof(DynaEnemyMindy) },
            { AssetType.Neptune, typeof(DynaEnemyNeptune) },
            { AssetType.Enemy, typeof(DynaEnemyStandard) },
            { AssetType.Crate, typeof(DynaEnemySupplyCrate) },
            { AssetType.Turret, typeof(DynaEnemyTurret) },
            { AssetType.TrainCar, typeof(DynaGObjectTrainCar) },

            { AssetType.Bomber, typeof(DynaEnemyIN2Bomber) },
            { AssetType.BossUnderminerDrill, typeof(DynaEnemyIN2BossUnderminerDrill) },
            { AssetType.BossUnderminerUM, typeof(DynaEnemyIN2BossUnderminerUM) },
            { AssetType.Chicken, typeof(DynaEnemyIN2Chicken) },
            { AssetType.Driller, typeof(DynaEnemyIN2Driller) },
            { AssetType.Enforcer, typeof(DynaEnemyIN2Enforcer) },
            { AssetType.Humanoid, typeof(DynaEnemyIN2Humanoid) },
            { AssetType.Rat, typeof(DynaEnemyIN2Rat) },
            { AssetType.RobotTank, typeof(DynaEnemyIN2RobotTank) },
            { AssetType.Scientist, typeof(DynaEnemyIN2Scientist) },
            { AssetType.Shooter, typeof(DynaEnemyIN2Shooter) },

            { AssetType.EnemyLeftArm, typeof(DynaEnemyRATSLeftArm) },
            { AssetType.EnemyRightArm, typeof(DynaEnemyRATSRightArm) },
            { AssetType.EnemySwarmBug, typeof(DynaEnemyRATSSwarmBug) },
            { AssetType.EnemySwarmOwl, typeof(DynaEnemyRATSSwarmOwl) },
            { AssetType.EnemyThief, typeof(DynaEnemyRATSThief) },
            { AssetType.EnemyWaiter, typeof(DynaEnemyRATSWaiter) },
        };

        private ToolStripMenuItem[] assetViewToolStripMenuItems;

        private ToolStripMenuItem[] GetAssetViewToolStripMenuItems(IEnumerable<AssetType> assetTypes)
        {
            var items = new List<ToolStripMenuItem>();
            var names = new Dictionary<AssetType, string>();

            foreach (var assetType in assetTypes)
            {
                if (!assetViewTypes.ContainsKey(assetType))
                    continue;

                var text = AssetTypeContainer.AssetTypeToString(assetType);
                if (assetType == AssetType.NPC && assetTypes.Contains(AssetType.Duplicator))
                    text = $"{AssetTypeContainer.AssetTypeToString(AssetType.NPC)}/{AssetTypeContainer.AssetTypeToString(AssetType.Duplicator)}";

                names.Add(assetType, text);
            }

            foreach ((var key, var value) in names.OrderBy(f => f.Value))
            {
                var field = assetViewTypes[key].GetField("dontRender");
                var dontRender = (bool)field.GetValue(null);

                ToolStripMenuItem item = new ToolStripMenuItem(value)
                {
                    Checked = !dontRender,
                    CheckState = dontRender ? CheckState.Unchecked : CheckState.Checked,
                    Tag = key
                };
                item.Click += (object sender, EventArgs e) =>
                {
                    item.Checked = !item.Checked;
                    field.SetValue(null, !item.Checked);
                    ShowDropDowns();
                };
                items.Add(item);
            }

            return items.ToArray();
        }

        private void SetAllAssetTypesVisible()
        {
            foreach (var assetType in assetViewTypes.Values)
            {
                var field = assetType.GetField("dontRender");
                field.SetValue(null, false);
            }
            UpdateToolbarVisibilityButtons();
        }

        private void SetUiAssetsVisibility(bool value)
        {
            autoShowDropDowns = false;

            AssetUI.dontRender = !value;
            AssetUIFT.dontRender = !value;

            var clickThis = new AssetType[] { AssetType.UserInterface, AssetType.UserInterfaceFont };
            if (assetViewToolStripMenuItems != null)
                foreach (var item in assetViewToolStripMenuItems)
                    if (clickThis.Contains((AssetType)item.Tag))
                        item.Checked = !(bool)assetViewTypes[(AssetType)item.Tag].GetField("dontRender").GetValue(null);
                    else if (item.Checked == value)
                        item.PerformClick();
            autoShowDropDowns = true;
        }

        private void SetViewAspect(float aspect)
        {
            ClientSize = new Size((int)(renderPanel.Height * aspect), ClientSize.Height);
        }

        private void UnselectTemplateButtons()
        {
            foreach (var i in templateButtons)
                i.Checked = false;
        }

        private void SelectPreviousTemplate()
        {
            if (CurrentAssetTemplate == AssetTemplate.User_Template)
            {
                if (toolStripComboBoxUserTemplate.SelectedIndex == 0)
                    toolStripComboBoxUserTemplate.SelectedIndex = toolStripComboBoxUserTemplate.Items.Count - 1;
                else
                    toolStripComboBoxUserTemplate.SelectedIndex -= 1;
            }
            else
            {
                for (int i = 1; i < templateButtons.Count; i++)
                {
                    if (templateButtons[i].Checked)
                    {
                        templateButtons[i - 1].PerformClick();
                        return;
                    }
                }
                templateButtons.Last().PerformClick();
            }
        }

        private void SelectNextTemplate()
        {
            if (CurrentAssetTemplate == AssetTemplate.User_Template)
            {
                if (toolStripComboBoxUserTemplate.SelectedIndex == toolStripComboBoxUserTemplate.Items.Count - 1)
                    toolStripComboBoxUserTemplate.SelectedIndex = 0;
                else
                    toolStripComboBoxUserTemplate.SelectedIndex += 1;
            }
            else
            {
                for (int i = 0; i < templateButtons.Count - 1; i++)
                {
                    if (templateButtons[i].Checked)
                    {
                        templateButtons[i + 1].PerformClick();
                        return;
                    }
                }
                templateButtons[0].PerformClick();
            }
        }

        private void TemplateToolStripItemClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var template = (AssetTemplate)item.Tag;

            UnselectTemplateButtons();

            CurrentAssetTemplate = template;
            toolStripStatusLabelTemplate.Text = "Template: " + item.Text;
            toolStripComboBoxUserTemplate.SelectedItem = null;
            item.Checked = true;
        }

        public void UpdateUserTemplateComboBox()
        {
            toolStripComboBoxUserTemplate.Items.Clear();

            foreach (string s in Directory.GetFiles(UserTemplateManager.userTemplatesFolder))
            {
                toolStripComboBoxUserTemplate.Items.Add(Path.GetFileName(s));

                if (toolStripComboBoxUserTemplate.Size.Width < 8 * Path.GetFileName(s).Length)
                    toolStripComboBoxUserTemplate.Size = new System.Drawing.Size(8 * Path.GetFileName(s).Length, toolStripComboBoxUserTemplate.Size.Height);
            }
        }

        private void SetGizmoMode(GizmoMode mode)
        {
            gizmoManager.CurrentGizmoMode = mode;
            mode = gizmoManager.CurrentGizmoMode;

            positionToolStripMenuItem.Checked = mode == GizmoMode.Position;
            rotationToolStripMenuItem.Checked = mode == GizmoMode.Rotation;
            scaleToolStripMenuItem.Checked = mode == GizmoMode.Scale;
            positionLocalToolStripMenuItem.Checked = mode == GizmoMode.PositionLocal;
            noneToolStripMenuItem.Checked = mode == GizmoMode.None;
            UpdateToolbarGizmoButtons(mode);
        }

        public void RefreshTexturesAndModels()
        {
            var textures = ArchiveEditors.SelectMany(ae => ae.archive?.Assets.OfType<AssetRWTX>() ?? []);
            var progressBar = new ProgressBar("Setting up textures and models");
            progressBar.SetProgressBar(0, textures.Count() * 2 + Program.Renderer.Assets.ModelCount + Program.Renderer.Assets.MatrixCount, 1);
            progressBar.Show(this);

            try
            {
            Program.TextureManager.ClearTextures();
                TextureConverter.SetupTextureDisplay(textures, progressBar);
                Program.Renderer.Assets.ResetModels(Program.Renderer, progressBar);
                Program.Renderer.Assets.RecalculateAllMatrices(Program.Renderer, progressBar);
            }
            finally
            {
                progressBar.Close();
            }
        }

        public void UnhideAllAssets()
        {
            bool unhid = Program.ArchiveManager.UnhideAllAssets();
            if (!unhid)
            {
                autoShowDropDowns = false;
                if (assetViewToolStripMenuItems != null)
                    foreach (var item in assetViewToolStripMenuItems)
                        if (!item.Checked)
                            item.PerformClick();
                autoShowDropDowns = true;
            }
        }

        private void setFlyCursor(int flyModeCursor)
        {
            this.flyModeCursor = (FlyModeCursor)flyModeCursor;

            // Updaate the toolstrip dropdown items
            foreach (ToolStripMenuItem item in cursorInFlyModeToolStripMenuItem.DropDownItems)
                item.Checked = Convert.ToInt32(item.Tag) == flyModeCursor;
        }
    }
}