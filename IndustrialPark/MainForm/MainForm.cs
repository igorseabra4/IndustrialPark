using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public enum FlyModeCursor
        {
            Default = 0,
            Crosshair = 1,
            Hide = 2
        }

        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

#if !DEBUG
            addTextureFolderToolStripMenuItem.Visible = false;
            addTXDArchiveToolStripMenuItem.Visible = false;
            openFolderToolStripMenuItem.Visible = false;
            dynaNameSearcherToolStripMenuItem.Visible = false;
#endif

            SetUiAssetsVisibility(false);

            templateButtons = ArchiveEditorFunctions.PopulateTemplateMenusAt(toolStripMenuItem_Templates, TemplateToolStripItemClick);

            renderer = new SharpRenderer(renderPanel);
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

            if (archiveEditors.Count > 0)
            {
                builder.Append(startOfArchiveList);
                for (int i = 0; i < archiveEditors.Count; i++)
                {
                    builder.Append(Path.GetFileName(archiveEditors[i].GetCurrentlyOpenFileName()));

                    // Unsaved changes
                    if (archiveEditors[i].archive.UnsavedChanges)
                    {
                        builder.Append(unsavedChanges);
                    }

                    // Separator
                    if (i < archiveEditors.Count - 1)
                    {
                        builder.Append(archiveDelimiter + " ");
                    }
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

        private void StartRenderer()
        {
            new Thread(() =>
            {
                if (InvokeRequired)
                    Invoke(new StartLoop(renderer.RunMainLoop), renderPanel);
                else
                    renderer.RunMainLoop(renderPanel);
            }).Start();
        }

        public static string pathToSettings => Application.StartupPath + "/ip_settings.json";
        private string currentProjectPath;
        public string userTemplatesFolder => Application.StartupPath + "/Resources/UserTemplates/";

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateUserTemplateComboBox();

            if (File.Exists(pathToSettings))
                ApplyIPSettings(JsonConvert.DeserializeObject<IPSettings>(File.ReadAllText(pathToSettings)));
            else
            {
                if (AutomaticUpdater.UpdateIndustrialPark(out _))
                {
                    Close();
                    System.Diagnostics.Process.Start(Application.StartupPath + "/IndustrialPark.exe");
                    return;
                }
                MessageBox.Show(
                    "It appears this is your first time using Industrial Park.\nPlease consult the documentation and user guide on the Heavy Iron Modding Wiki to understand how to use the tool if you haven't already.",
                    "Welcome!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Program.AboutBox.Show();
            }

            SetProjectToolStripStatusLabel();
            StartRenderer();
            UpdateTitleBar();
            ResetMouseCenter(null, null);
        }

        private void ApplyIPSettings(IPSettings settings)
        {
            autoSaveOnClosingToolStripMenuItem.Checked = settings.AutosaveOnClose;
            autoLoadOnStartupToolStripMenuItem.Checked = settings.AutoloadOnStartup;
            checkForUpdatesOnStartupToolStripMenuItem.Checked = settings.CheckForUpdatesOnStartup;

            drawOnlyFirstMINFReferenceToolStripMenuItem.Checked = settings.drawOnlyFirstMinf;
            AssetMINF.drawOnlyFirst = settings.drawOnlyFirstMinf;

            useLODTForRenderingToolStripMenuItem.Checked = settings.renderBasedOnLodt;
            AssetMODL.renderBasedOnLodt = settings.renderBasedOnLodt;

            usePIPTForRenderingToolStripMenuItem.Checked = settings.renderBasedOnPipt;
            AssetMODL.renderBasedOnPipt = settings.renderBasedOnPipt;

            hideInvisibleMeshesToolStripMenuItem.Checked = settings.dontDrawInvisible;
            RenderWareModelFile.dontDrawInvisible = settings.dontDrawInvisible;

            templatesPersistentShiniesToolStripMenuItem.Checked = settings.persistentShinies;
            ArchiveEditorFunctions.persistentShinies = settings.persistentShinies;

            useLegacyAssetIDFormatToolStripMenuItem.Checked = settings.LegacyAssetIDFormat;
            HexUIntTypeConverter.Legacy = settings.LegacyAssetIDFormat;

            useLegacyAssetTypeFormatToolStripMenuItem.Checked = settings.LegacyAssetTypeFormat;
            AssetTypeContainer.LegacyAssetNameFormat = settings.LegacyAssetTypeFormat;

            discordRichPresenceToolStripMenuItem.Checked = settings.discordRichPresence;
            DiscordRPCController.ToggleDiscordRichPresence(discordRichPresenceToolStripMenuItem.Checked);

            BuildISO.PCSX2Path = settings.pcsx2Path;
            BuildISO.recentGameDirPaths = settings.recentBuildIsoGamePaths;

            bool showEditorsWhenLoadingProject = settings.showEditorsWhenLoadingProject;
            showEditorsWhenLoadingProjectToolStripMenuItem.Checked = showEditorsWhenLoadingProject;

            setFlyCursor(settings.flyModeCursor);

            if (settings.recentArchivePaths != null)
                foreach (string filepath in settings.recentArchivePaths)
                    SetRecentOpenedArchives(filepath);

            if (settings.CheckForUpdatesOnStartup && AutomaticUpdater.UpdateIndustrialPark(out _))
            {
                Close();
                System.Diagnostics.Process.Start(Application.StartupPath + "/IndustrialPark.exe");
            }

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
                    b.Show();
                    return;
                }
            }

            if (settings.AutoloadOnStartup && !string.IsNullOrEmpty(settings.LastProjectPath) && File.Exists(settings.LastProjectPath))
                ApplyProject(settings.LastProjectPath, showEditorsWhenLoadingProject);
        }

        private delegate void StartLoop(Panel renderPanel);

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

            if (autoSaveOnClosingToolStripMenuItem.Checked)
                SaveProject(currentProjectPath);

            IPSettings settings = new IPSettings
            {
                AutosaveOnClose = autoSaveOnClosingToolStripMenuItem.Checked,
                AutoloadOnStartup = autoLoadOnStartupToolStripMenuItem.Checked,
                LastProjectPath = currentProjectPath,
                CheckForUpdatesOnStartup = checkForUpdatesOnStartupToolStripMenuItem.Checked,
                drawOnlyFirstMinf = AssetMINF.drawOnlyFirst,
                renderBasedOnLodt = AssetMODL.renderBasedOnLodt,
                renderBasedOnPipt = AssetMODL.renderBasedOnPipt,
                discordRichPresence = discordRichPresenceToolStripMenuItem.Checked,
                dontDrawInvisible = RenderWareModelFile.dontDrawInvisible,
                persistentShinies = ArchiveEditorFunctions.persistentShinies,
                LegacyAssetIDFormat = HexUIntTypeConverter.Legacy,
                LegacyAssetTypeFormat = AssetTypeContainer.LegacyAssetNameFormat,
                pcsx2Path = BuildISO.PCSX2Path,
                recentBuildIsoGamePaths = BuildISO.recentGameDirPaths,
                recentArchivePaths = openLastToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().Select(x => x.Text).ToArray(),
                flyModeCursor = (int)flyModeCursor,
                showEditorsWhenLoadingProject = showEditorsWhenLoadingProjectToolStripMenuItem.Checked
            };

            File.WriteAllText(pathToSettings, JsonConvert.SerializeObject(settings, Formatting.Indented));
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

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
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
                ApplyProject(openFile.FileName, showEditorsWhenLoadingProjectToolStripMenuItem.Checked);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentProjectPath))
                SaveProject();
            else
                saveAsToolStripMenuItem_Click(null, null);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            { Filter = "JSON files|*.json" };

            if (saveFile.ShowDialog() == DialogResult.OK)
                SaveProject(saveFile.FileName);
        }

        private void SaveProject(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = Application.StartupPath + "\\default_project.json";
            currentProjectPath = fileName;
            SaveProject();
        }

        private void SaveProject()
        {
            File.WriteAllText(currentProjectPath, JsonConvert.SerializeObject(FromCurrentInstance(), Formatting.Indented));
            SetProjectToolStripStatusLabel();
        }

        private void autoLoadOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoLoadOnStartupToolStripMenuItem.Checked = !autoLoadOnStartupToolStripMenuItem.Checked;
        }

        private void autoSaveOnClosingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoSaveOnClosingToolStripMenuItem.Checked = !autoSaveOnClosingToolStripMenuItem.Checked;
        }

        private void CheckForUpdatesOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkForUpdatesOnStartupToolStripMenuItem.Checked = !checkForUpdatesOnStartupToolStripMenuItem.Checked;
        }

        private void CheckForUpdatesNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AutomaticUpdater.UpdateIndustrialPark(out bool hasChecked))
            {
                Close();
                System.Diagnostics.Process.Start(Application.StartupPath + "/IndustrialPark.exe");
            }
            else if (hasChecked)
                MessageBox.Show("No update found.");
        }

        private void CheckForUpdatesOnEditorFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!AutomaticUpdater.VerifyEditorFiles())
                MessageBox.Show("No update found.");
        }

        private void DownloadIndustrialParkEditorFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutomaticUpdater.DownloadEditorFiles();
        }

        private void downloadVgmstreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SoundUtility_vgmstream.DownloadVgmstream();
        }

        public ProjectJson FromCurrentInstance()
        {
            List<string> hips = new List<string>();
            List<Platform> platforms = new List<HipHopFile.Platform>();
            List<uint> hiddenAssets = new List<uint>();

            foreach (ArchiveEditor ae in archiveEditors)
                if (ae.archive != null && !string.IsNullOrEmpty(ae.GetCurrentlyOpenFileName()))
                {
                    hips.Add(ae.GetCurrentlyOpenFileName());
                    platforms.Add(ae.archive.platform);
                    hiddenAssets.AddRange(ae.archive.GetHiddenAssets());
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
                TextureFolderPaths = TextureManager.OpenTextureFolders.ToList(),
                CamPos = renderer.Camera.Position,
                Yaw = renderer.Camera.Yaw,
                Pitch = renderer.Camera.Pitch,
                Speed = renderer.Camera.Speed,
                SpeedRot = renderer.Camera.SpeedRot,
                FieldOfView = renderer.Camera.FieldOfView,
                FarPlane = renderer.Camera.FarPlane,
                NoCulling = noCullingCToolStripMenuItem.Checked,
                Wireframe = wireframeFToolStripMenuItem.Checked,
                BackgroundColor = renderer.backgroundColor,
                WidgetColor = renderer.normalColor,
                TrigColor = renderer.trigColor,
                MvptColor = renderer.mvptColor,
                SfxColor = renderer.sfxColor,
                hiddenAssets = hiddenAssets,
                isDrawingUI = renderer.isDrawingUI,
                Grid = ArchiveEditorFunctions.Grid,
                dontRender = dontRender,
            };
        }

        private void ApplyProject(string projectPath, bool showEditors = true)
        {
            currentProjectPath = projectPath;
            SetProjectToolStripStatusLabel();
            ApplyProject(JsonConvert.DeserializeObject<ProjectJson>(File.ReadAllText(projectPath)), showEditors);
        }

        private void ApplyProject(ProjectJson ipSettings, bool showEditors = true)
        {
            if (ipSettings.version != ProjectJson.getCurrentVersion)
                MessageBox.Show("You are trying to open a project file made with a different version of Industrial Park. The program will attempt to load the project, but there is a chance it will not load properly.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            TextureManager.ClearTextures();

            List<ArchiveEditor> archiveEditors = new List<ArchiveEditor>();
            archiveEditors.AddRange(this.archiveEditors);
            foreach (ArchiveEditor ae in archiveEditors)
                ae.CloseArchiveEditor();

            TextureManager.LoadTexturesFromFolder(ipSettings.TextureFolderPaths);

            ArchiveEditorFunctions.hiddenAssets = ipSettings.hiddenAssets;

            for (int i = 0; i < ipSettings.hipPaths.Count; i++)
                if (ipSettings.hipPaths[i] == "Empty")
                    AddArchiveEditor();
                else
                {
                    if (File.Exists(ipSettings.hipPaths[i]))
                        AddArchiveEditor(ipSettings.hipPaths[i], ipSettings.scoobyPlatforms[i], showEditors);
                    else
                    {
                        MessageBox.Show("Error opening " + ipSettings.hipPaths[i] + ": file not found",
                            "File not Found",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }

            ArchiveEditorFunctions.hiddenAssets.Clear();

            renderer.Camera.SetPosition(ipSettings.CamPos);
            renderer.Camera.Yaw = ipSettings.Yaw;
            renderer.Camera.Pitch = ipSettings.Pitch;
            renderer.Camera.Speed = ipSettings.Speed;
            renderer.Camera.SpeedRot = ipSettings.SpeedRot;
            renderer.Camera.FieldOfView = ipSettings.FieldOfView;
            renderer.Camera.FarPlane = ipSettings.FarPlane;
            ArchiveEditorFunctions.Grid = ipSettings.Grid;

            noCullingCToolStripMenuItem.Checked = ipSettings.NoCulling;
            if (noCullingCToolStripMenuItem.Checked)
                renderer.device.SetNormalCullMode(CullMode.None);
            else
                renderer.device.SetNormalCullMode(CullMode.Back);

            wireframeFToolStripMenuItem.Checked = ipSettings.Wireframe;
            if (wireframeFToolStripMenuItem.Checked)
                renderer.device.SetNormalFillMode(FillMode.Wireframe);
            else
                renderer.device.SetNormalFillMode(FillMode.Solid);

            renderer.backgroundColor = ipSettings.BackgroundColor;
            renderer.SetWidgetColor(ipSettings.WidgetColor);
            renderer.SetMvptColor(ipSettings.MvptColor);
            renderer.SetTrigColor(ipSettings.TrigColor);
            renderer.SetSfxColor(ipSettings.SfxColor);

            uIModeToolStripMenuItem.Checked = ipSettings.isDrawingUI;
            renderer.isDrawingUI = ipSettings.isDrawingUI;

            if (assetViewToolStripMenuItems != null)
                for (int i = 0; i < assetViewToolStripMenuItems.Length; i++)
                {
                    AssetType type = (AssetType)assetViewToolStripMenuItems[i].Tag;
                    var code = type.ToString();
                    bool value = ipSettings.dontRender.ContainsKey(code) ? ipSettings.dontRender[code] : true;
                    assetViewToolStripMenuItems[i].Checked = !value;
                    assetViewTypes[type].GetField("dontRender").SetValue(null, value);
                }
        }

        public void SetToolStripStatusLabel(string Text)
        {
            toolStripStatusLabel1.Text = Text;
        }

        private void SetProjectToolStripStatusLabel()
        {
            toolStripStatusLabelProject.Text = "Project: " + (currentProjectPath ?? "None");
        }

        public SharpRenderer renderer;

        private bool mouseMode = false;
        private bool cursorHidden = false;
        private FlyModeCursor flyModeCursor = FlyModeCursor.Crosshair;
        private System.Drawing.Point MouseCenter;
        private MouseEventArgs oldMousePosition = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);

        private void MouseMoveControl(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
                ArchiveEditorFunctions.ScreenUnclicked();

            if (renderer.isDrawingUI)
            {
                float x = ((e.X - renderPanel.ClientRectangle.X) * 640f / renderPanel.ClientRectangle.Width);
                float y = ((e.Y - renderPanel.ClientRectangle.Y) * 480f / renderPanel.ClientRectangle.Height);

                SetToolStripStatusLabel(string.Format("Position: [{0:0.0000}, {1:0.0000}]", x, y)
                    + " FPS: " + $"{renderer.sharpFPS.FPS:0.0000}");
            }
            else
            {
                if (mouseMode)
                {


                    switch (flyModeCursor)
                    {
                        case FlyModeCursor.Default:
                            if (Cursor.Current != Cursors.Default)
                                Cursor = Cursors.Default;
                            break;
                        case FlyModeCursor.Crosshair:
                            if (Cursor.Current != Cursors.Cross)
                                Cursor = Cursors.Cross;
                            break;
                        case FlyModeCursor.Hide:
                            if (!cursorHidden)
                            {
                                cursorHidden = true;
                                Cursor.Hide();
                            }
                            break;
                    }

                    renderer.Camera.AddYaw(MathUtil.DegreesToRadians(Cursor.Position.X - MouseCenter.X) / 4);
                    renderer.Camera.AddPitch(MathUtil.DegreesToRadians(Cursor.Position.Y - MouseCenter.Y) / 4);

                    Cursor.Position = MouseCenter;
                }
                else
                {
                    if (cursorHidden)
                    {
                        cursorHidden = false;
                        Cursor.Show();
                    }

                    if (Cursor.Current != Cursors.Default)
                        Cursor = Cursors.Default;

                    if (e.Button == MouseButtons.Middle)
                    {
                        renderer.Camera.AddYaw(MathUtil.DegreesToRadians(e.X - oldMousePosition.X));
                        renderer.Camera.AddPitch(MathUtil.DegreesToRadians(e.Y - oldMousePosition.Y));
                        // Clamp camera pitch so it doesn't loop around
                        float pitchPadding = 0.01f;
                        renderer.Camera.Pitch = MathUtil.Clamp(renderer.Camera.Pitch,
                            -90 + pitchPadding,
                            90 - pitchPadding);
                    }
                    if (e.Button == MouseButtons.Right && PressedKeys.Contains(Keys.ControlKey))
                    {
                        renderer.Camera.AddPositionSideways(e.X - oldMousePosition.X);
                        renderer.Camera.AddPositionUp(e.Y - oldMousePosition.Y);
                    }
                }

                if (e.Delta != 0)
                    renderer.Camera.AddPositionForward(e.Delta / 24);
            }

            MouseMoveForGizmos(e.X - oldMousePosition.X, e.Y - oldMousePosition.Y);

            oldMousePosition = e;
        }

        private void MouseModeToggle()
        {
            mouseMode = !mouseMode;
        }

        private void ResetMouseCenter(object sender, EventArgs e)
        {
            MouseCenter = renderPanel.PointToScreen(new System.Drawing.Point(renderPanel.Width / 2, renderPanel.Height / 2));
        }

        private void MouseMoveForGizmos(int deltaX, int deltaY)
        {
            foreach (ArchiveEditor ae in archiveEditors)
                ae.MouseMoveGeneric(renderer.viewProjection, deltaX, deltaY, PressedKeys.Contains(Keys.T));
        }

        private readonly HashSet<Keys> PressedKeys = new HashSet<Keys>();

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!PressedKeys.Contains(e.KeyCode))
                PressedKeys.Add(e.KeyCode);

            if (e.KeyCode == Keys.Z)
                MouseModeToggle();
            else if (e.KeyCode == Keys.Q)
                renderer.Camera.IncreaseCameraSpeed(-1);
            else if (e.KeyCode == Keys.E)
                renderer.Camera.IncreaseCameraSpeed(1);
            else if (e.KeyCode == Keys.D1)
                renderer.Camera.IncreaseCameraRotationSpeed(-1);
            else if (e.KeyCode == Keys.D3)
                renderer.Camera.IncreaseCameraRotationSpeed(1);
            else if (e.KeyCode == Keys.C)
                ToggleBackfaceCulling();
            else if (e.KeyCode == Keys.F)
                ToggleWireFrame();
            else if (e.KeyCode == Keys.H)
                DropSelectedAssets();
            else if (e.KeyCode == Keys.G)
                OpenInternalEditors();
            else if (e.KeyCode == Keys.V)
                ToggleGizmoType();
            else if (e.KeyCode == Keys.P)
                showVertexColorsToolStripMenuItem_Click(null, null);
            else if (e.KeyCode == Keys.Delete)
                DeleteSelectedAssets();
            else if (e.KeyCode == Keys.U)
                uIModeToolStripMenuItem_Click(null, null);
            else if (!PressedKeys.Contains(Keys.ControlKey) && e.KeyCode == Keys.B)
                SelectPreviousTemplate();
            else if (!PressedKeys.Contains(Keys.ControlKey) && e.KeyCode == Keys.N)
                SelectNextTemplate();

            if (e.KeyCode == Keys.F1)
                Program.ViewConfig.Show();
            else if (e.KeyCode == Keys.F4)
                saveAllOpenHIPsToolStripMenuItem_Click(sender, e);
            else if (e.KeyCode == Keys.F5)
                TryToRunGame();
            else if (e.KeyCode == Keys.F6)
                buildAndRunPS2ISOToolStripMenuItem_Click(sender, e);
            else if (e.KeyCode == Keys.F7)
                createGameCubeBannerToolStripMenuItem_Click(sender, e);

            if (PressedKeys.Contains(Keys.S)
                && PressedKeys.Contains(Keys.ControlKey)
                && PressedKeys.Contains(Keys.ShiftKey))
            {
                saveAllOpenHIPsToolStripMenuItem_Click(sender, e);
            }

            // Close all forms that are not Main Form/Archive Editors
            if (PressedKeys.Contains(Keys.ControlKey)
                && PressedKeys.Contains(Keys.ShiftKey)
                && PressedKeys.Contains(Keys.H))
            {
                var openForms = Application.OpenForms;
                var formsToClose = new List<Form>();

                for (int i = 0; i < openForms.Count; i++)
                {
                    if (openForms[i].GetType() != typeof(MainForm)
                        && openForms[i].GetType() != typeof(ArchiveEditor))
                    {
                        formsToClose.Add(openForms[i]);
                    }
                }

                foreach (Form form in formsToClose)
                {
                    form.Close();
                }
            }

            // in enum, Keys.D1 through Keys.D9 are numbers 49 to 57.
            int numberKeyOffset = 49;
            var editorsToShow = new List<Form>();

            // Open archive editors with ctrl and 1-9
            if (PressedKeys.Contains(Keys.ControlKey))
            {
                foreach (var item in PressedKeys)
                {
                    if ((int)item >= numberKeyOffset
                        && (int)item <= numberKeyOffset + 8
                        && (int)item - numberKeyOffset < archiveEditors.Count)
                    {
                        editorsToShow.Add(archiveEditors[(int)item - numberKeyOffset]);
                    }
                }

                foreach (var form in editorsToShow)
                {
                    form.Show();
                }
            }

            if (PressedKeys.Contains(Keys.ControlKey)
                && PressedKeys.Contains(Keys.N))
            {
                newToolStripMenuItem_Click(sender, e);
            }

            if (PressedKeys.Contains(Keys.ControlKey)
                && PressedKeys.Contains(Keys.O))
            {
                openLevelToolStripMenuItem_Click(sender, e);
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            PressedKeys.Remove(e.KeyCode);
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            PressedKeys.Clear();
        }

        public void KeyboardController()
        {
            if (renderer.isDrawingUI)
                return;

            if (PressedKeys.Contains(Keys.A) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddYaw(-0.05f * renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.A))
                renderer.Camera.AddPositionSideways(0.25f * renderer.TransformScaleFactor);

            if (PressedKeys.Contains(Keys.D) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddYaw(0.05f * renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.D))
                renderer.Camera.AddPositionSideways(-0.25f * renderer.TransformScaleFactor);

            if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPitch(-0.05f * renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ShiftKey))
                renderer.Camera.AddPositionUp(0.25f * renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.W))
                renderer.Camera.AddPositionForward(0.25f * renderer.TransformScaleFactor);

            // Added extra conditions to stop camera moving when saving all archive editors
            if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ControlKey) & !PressedKeys.Contains(Keys.ShiftKey))
                renderer.Camera.AddPitch(0.05f * renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ShiftKey) & !PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPositionUp(-0.25f * renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.S) & !PressedKeys.Contains(Keys.ShiftKey) & !PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPositionForward(-0.25f * renderer.TransformScaleFactor);

            if (PressedKeys.Contains(Keys.R))
                renderer.Camera.Reset();

            float pitchPadding = 0.01f;
            renderer.Camera.Pitch = MathUtil.Clamp(renderer.Camera.Pitch,
                -90 + pitchPadding,
                90 - pitchPadding);
        }

        public List<ArchiveEditor> archiveEditors = new List<ArchiveEditor>();

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddArchiveEditor();
            archiveEditors.Last().Show();
        }

        public void AddArchiveEditor(string filePath = null, Platform scoobyPlatform = Platform.Unknown, bool show = true)
        {
            ArchiveEditor ae = new ArchiveEditor();

            ae.Begin(filePath, scoobyPlatform);
            archiveEditors.Add(ae);

            ToolStripMenuItem tempMenuItem = new ToolStripMenuItem(Path.GetFileName(ae.GetCurrentlyOpenFileName()));
            tempMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ae.Show();
                ae.WindowState = FormWindowState.Normal;
            });
            archiveEditorToolStripMenuItem.DropDownItems.Add(tempMenuItem);

            ae.archive.ChangesMade += UpdateTitleBar;
            ae.archive.CurrentlySelectedAssets.CollectionChanged += UpdateSelectedAssetStatusBarItem;
            ae.EditorUpdate += EditorUpdate;
            UpdateTitleBar();
            SetupAssetVisibilityButtons();
            closeAllEditorsToolStripMenuItem.Enabled = true;

            ae.Show();

            // Cannot select assets if the editor isn't shown first?
            if (!show)
                ae.Hide();
        }

        public void EditorUpdate()
        {
            closeAllEditorsToolStripMenuItem.Enabled = archiveEditors.Count > 0;
            RefreshTexturesAndModels();
            SetupAssetVisibilityButtons();
        }

        public void SetToolStripItemName(ArchiveEditor sender, string newName)
        {
            archiveEditorToolStripMenuItem.DropDownItems[archiveEditors.IndexOf(sender) + 5].Text = newName;
        }

        public void SetRecentOpenedArchives(string filepath)
        {
            if (!openLastToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().Any(x => x.Text == filepath))
            {
                ToolStripMenuItem item = new ToolStripMenuItem(filepath);
                item.Click += (s, e) =>
                {
                    openLastToolStripMenuItem.DropDownItems.Remove(item);
                    if (File.Exists(filepath))
                    {
                        openLastToolStripMenuItem.DropDownItems.Add(item);
                        AddArchiveEditor(filepath);
                    }
                    else
                        MessageBox.Show("File does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
                openLastToolStripMenuItem.DropDownItems.Add(item);
            }
            while (openLastToolStripMenuItem.DropDownItems.Count > 15)
                openLastToolStripMenuItem.DropDownItems.RemoveAt(0);
        }

        public void CloseArchiveEditor(ArchiveEditor sender)
        {
            int index = archiveEditors.IndexOf(sender);
            archiveEditorToolStripMenuItem.DropDownItems.RemoveAt(index + 5);
            archiveEditors.RemoveAt(index);
        }

        public void SetCloseAllArchivesEnabled(bool enabled)
        {
            closeAllEditorsToolStripMenuItem.Enabled = enabled;
        }

        private bool UnsavedChanges()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                if (ae.archive.UnsavedChanges)
                    return true;

            return false;
        }

        private void SaveAllChanges()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                if (ae.archive.UnsavedChanges)
                    ae.Save();
        }

        private void noCullingCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleBackfaceCulling();
        }

        public void ToggleBackfaceCulling()
        {
            noCullingCToolStripMenuItem.Checked = !noCullingCToolStripMenuItem.Checked;
            renderer.device.SetNormalCullMode(noCullingCToolStripMenuItem.Checked ? CullMode.None : CullMode.Back);
        }

        private void wireframeFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleWireFrame();
        }

        public void ToggleWireFrame()
        {
            wireframeFToolStripMenuItem.Checked = !wireframeFToolStripMenuItem.Checked;
            renderer.device.SetNormalFillMode(wireframeFToolStripMenuItem.Checked ? FillMode.Wireframe : FillMode.Solid);
        }

        private void backgroundColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(BitConverter.ToInt32(BitConverter.GetBytes(renderer.backgroundColor.ToBgra()).Reverse().ToArray(), 0))
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
                renderer.backgroundColor = new SharpDX.Color(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B, colorDialog.Color.A);
        }

        private void widgetColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                renderer.SetWidgetColor(colorDialog.Color);
        }

        private void selectionColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                renderer.SetSelectionColor(colorDialog.Color);
        }

        private void mVPTColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                renderer.SetMvptColor(colorDialog.Color);
        }

        private void tRIGColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                renderer.SetTrigColor(colorDialog.Color);
        }

        private void sFXInColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                renderer.SetSfxColor(colorDialog.Color);
        }

        private void resetColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.ResetColors();
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            Program.ViewConfig.Show();
        }

        private void viewConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ViewConfig.Show();
        }

        private void viewControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Keyboard controls:\n" +
                "W, A, S, D: move view forward, left, backward, right\n" +
                "Shift + (W, S): move view up, down\n" +
                "Ctrl + (W, A, S, D): rotate view up, left, down, right\n" +
                "Q, E: decrease interval, increase interval (view move speed)\n" +
                "1, 3: decrease rotation interval, increase rotation interval (view rotation speed)\n" +
                "B and N: select previous/next template\n" +
                "C: toggles backface culling\n" +
                "F: toggles wireframe mode\n" +
                "G: open Asset Data Editor for selected assets\n" +
                "H: drop selected assets\n" +
                "P: Toggle vertex color display\n" +
                "R: reset view\n" +
                "T: snap gizmos to grid\n" +
                "U: toggle UI Mode\n" +
                "V: cycle between gizmos\n" +
                "Z: toggle mouse mode: similar to a first person camera. The view rotates automatically as you move the mouse. Use the keyboard to move around\n" +
                "F1: displays the View Config box\n" +
                "Ctrl + Shift + S or F4: save all open Archive Editors\n" +
                "F5: attempt to run game\n" +
                "F6: open Build/Run PS2 ISO\n" +
                "Delete: delete selected assets\n" +
                "Ctrl + Shift + H: closes all windows which are not Archive Editors\n" +
                "Ctrl + 1 to 9: Open the corresponding Archive Editor window\n" +
                "\n" +
                "Mouse controls:\n" +
                "Left click on an asset to select it\n" +
                "Ctrl + Left click to select multiple\n" +
                "Middle click and drag to rotate view\n" +
                "Mouse wheel to move forward/backward\n" +
                "Right click on screen to choose a gizmo or template\n" +
                "Shift + Right click to place a template\n" +
                "Ctrl + Right click and drag to pan (move view up, left, down, right)\n" +
                "\n" +
                "Please consult the Industrial Park user guide on Heavy Iron Modding for more information"
                );
        }

        private SharpDX.Rectangle ViewRectangle => new SharpDX.Rectangle(
                renderPanel.ClientRectangle.X,
                renderPanel.ClientRectangle.Y,
                renderPanel.ClientRectangle.Width,
                renderPanel.ClientRectangle.Height);

        private void renderPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ScreenClicked(ViewRectangle, e.X, e.Y, false);
            }
            else if (e.Button == MouseButtons.Right && PressedKeys.Contains(Keys.ShiftKey))
            {
                Vector3 Position = GetScreenClickedPosition(ViewRectangle, e.X, e.Y);

                bool placed = false;
                foreach (ArchiveEditor archiveEditor in archiveEditors)
                    if (archiveEditor.TemplateFocus)
                    {
                        archiveEditor.PlaceTemplate(Position);
                        placed = true;
                    }
                if (!placed)
                    MessageBox.Show($"Your template has not been placed as no Archive Editors have Template Focus on.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (e.Button == MouseButtons.Right)
            {
                contextMenuStripMain.Show(renderPanel.PointToScreen(e.Location));
            }
        }

        private void renderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ScreenClicked(ViewRectangle, e.X, e.Y, true);
        }

        public void ScreenClicked(SharpDX.Rectangle viewRectangle, int X, int Y, bool isMouseDown)
        {
            if (ArchiveEditorFunctions.FinishedMovingGizmo && !isMouseDown)
                ArchiveEditorFunctions.FinishedMovingGizmo = false;
            else
            {
                Ray ray = Ray.GetPickRay(X, Y, new Viewport(viewRectangle), renderer.viewProjection);

                if (isMouseDown)
                    ArchiveEditorFunctions.GizmoSelect(ray);
                else
                    SetSelectedIndex(renderer.isDrawingUI ?
                        ArchiveEditorFunctions.GetClickedAssetID2D(renderer, ray, renderer.Camera.FarPlane) :
                        ArchiveEditorFunctions.GetClickedAssetID(renderer, ray));
            }
        }

        public Vector3 GetScreenClickedPosition(SharpDX.Rectangle viewRectangle, int X, int Y) =>
            ArchiveEditorFunctions.GetRayIntersectionPosition(renderer,
                Ray.GetPickRay(X, Y, new Viewport(viewRectangle), renderer.viewProjection));

        private void renderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            ArchiveEditorFunctions.ScreenUnclicked();
        }

        private void renderPanel_MouseLeave(object sender, EventArgs e)
        {
            ArchiveEditorFunctions.ScreenUnclicked();
        }

        public void SetSelectedIndex(uint? assetID, bool add = false)
        {
            add |= PressedKeys.Contains(Keys.ControlKey) || PressedKeys.Contains(Keys.Control);
            if (add && assetID == null)
                return;
            foreach (ArchiveEditor ae in archiveEditors)
                ae.SetSelectedIndex(assetID ?? 0, false, add);
        }

        /// <summary>
        /// Gets the number of currently selected assets across all archive Editors.
        /// </summary>
        /// <returns>The number of selected assets</returns>
        private int GetNumberOfSelectedAssets()
        {
            return archiveEditors.Sum(ae => ae.archive.GetNumberOfSelectedAssets);
        }

        /// <summary>
        /// Updates the status bar to show the number of selected assets.
        /// </summary>
        private void UpdateSelectedAssetStatusBarItem(object sender, NotifyCollectionChangedEventArgs e)
        {
            toolStripStatusLabelNumSelected.Text = $"{GetNumberOfSelectedAssets()} selected";
        }

        private void OpenInternalEditors()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                ae.OpenInternalEditors();
        }

        private void DeleteSelectedAssets()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                ae.DeleteSelectedAssets();
        }

        private void addTextureFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addTXDArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pLATPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            movementPreviewToolStripMenuItem.Checked = !movementPreviewToolStripMenuItem.Checked;
            EntityAsset.movementPreview = movementPreviewToolStripMenuItem.Checked;

            ResetMovementPreview();
        }

        public void ResetMovementPreview()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                foreach (Asset a in ae.archive.GetAllAssets())
                    if (a is EntityAsset p)
                        p.Reset();
        }

        private void drawOnlyFirstMINFReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawOnlyFirstMINFReferenceToolStripMenuItem.Checked = !drawOnlyFirstMINFReferenceToolStripMenuItem.Checked;
            AssetMINF.drawOnlyFirst = drawOnlyFirstMINFReferenceToolStripMenuItem.Checked;
        }

        private void useMaxRenderDistanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useLODTForRenderingToolStripMenuItem.Checked = !useLODTForRenderingToolStripMenuItem.Checked;
            AssetMODL.renderBasedOnLodt = useLODTForRenderingToolStripMenuItem.Checked;
        }

        private void UsePIPTForRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            usePIPTForRenderingToolStripMenuItem.Checked = !usePIPTForRenderingToolStripMenuItem.Checked;
            AssetMODL.renderBasedOnPipt = usePIPTForRenderingToolStripMenuItem.Checked;
        }

        private void vSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.device.VSync = !renderer.device.VSync;
            vSyncToolStripMenuItem.Checked = renderer.device.VSync;
        }

        private void lowerQualityGraphicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renderer.device.SetGraphicsMode(!renderer.device.currentMode);
            lowerQualityGraphicsToolStripMenuItem.Checked = !renderer.device.currentMode;
        }

        private void useLegacyAssetIDFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useLegacyAssetIDFormatToolStripMenuItem.Checked = !useLegacyAssetIDFormatToolStripMenuItem.Checked;
            HexUIntTypeConverter.Legacy = useLegacyAssetIDFormatToolStripMenuItem.Checked;
        }

        private void useLegacyAssetTypeFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useLegacyAssetTypeFormatToolStripMenuItem.Checked = !useLegacyAssetTypeFormatToolStripMenuItem.Checked;
            AssetTypeContainer.LegacyAssetNameFormat = useLegacyAssetTypeFormatToolStripMenuItem.Checked;
            SetupAssetVisibilityButtons();
            foreach (var ae in archiveEditors)
                ae.PopulateAssetListAndComboBox();
        }

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

        private bool autoShowDropDowns = true;

        private void ShowDropDowns()
        {
            if (autoShowDropDowns)
            {
                displayToolStripMenuItem.ShowDropDown();
                assetTypesToolStripMenuItem.ShowDropDown();
            }
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
            foreach (var ae in archiveEditors)
                foreach (var assetType in ae.archive.AssetTypesOnArchive())
                    assetTypes.Add(assetType);

            SetAssetViewToolStripMenuItems(assetTypes);
            assetTypesToolStripMenuItem.DropDownItems.AddRange(assetViewToolStripMenuItems);
        }

        private Dictionary<AssetType, Type> assetViewTypes = new Dictionary<AssetType, Type>
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

        private void SetAssetViewToolStripMenuItems(IEnumerable<AssetType> assetTypes)
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

            foreach (var entry in names.OrderBy(f => f.Value))
            {
                var field = assetViewTypes[entry.Key].GetField("dontRender");
                var dontRender = (bool)field.GetValue(null);

                ToolStripMenuItem item = new ToolStripMenuItem(entry.Value)
                {
                    Checked = !dontRender,
                    CheckState = dontRender ? CheckState.Unchecked : CheckState.Checked,
                    Tag = entry.Key
                };
                item.Click += (object sender, EventArgs e) =>
                {
                    item.Checked = !item.Checked;
                    field.SetValue(null, !item.Checked);
                    ShowDropDowns();
                };
                items.Add(item);
            }

            assetViewToolStripMenuItems = items.ToArray();
        }

        private void SetAllAssetTypesVisible()
        {
            foreach (var assetType in assetViewTypes.Values)
            {
                var field = assetType.GetField("dontRender");
                field.SetValue(null, false);
            }
        }

        private void uIModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uIModeToolStripMenuItem.Checked = !uIModeToolStripMenuItem.Checked;
            renderer.isDrawingUI = uIModeToolStripMenuItem.Checked;
            SetUiAssetsVisibility(renderer.isDrawingUI);
            renderer.Camera.Reset();
            mouseMode = false;
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.AboutBox.Show();
        }

        public string GetAssetNameFromID(uint assetID)
        {
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                if (archiveEditor.HasAsset(assetID))
                    return archiveEditor.GetAssetNameFromID(assetID);

            return ArchiveEditorFunctions.GetFromNameDictionary(assetID) ?? "0x" + assetID.ToString("X8");
        }

        public bool AssetExists(uint assetID)
        {
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                if (archiveEditor.HasAsset(assetID))
                    return true;

            if (ArchiveEditorFunctions.GetFromNameDictionary(assetID) != null)
                return true;

            return false;
        }

        public void FindWhoTargets(uint assetID)
        {
            List<uint> whoTargets = WhoTargets(assetID);

            bool willOpen = true;
            if (whoTargets.Count > 15)
                willOpen = MessageBox.Show($"Warning: you're going to open {whoTargets.Count} Asset Data Editor windows. Are you sure you want to do that?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;

            if (willOpen)
                foreach (ArchiveEditor archiveEditor in archiveEditors)
                    archiveEditor.OpenInternalEditors(whoTargets, true);
        }

        public List<uint> WhoTargets(uint assetID)
        {
            List<uint> whoTargets = new List<uint>();
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                whoTargets.AddRange(archiveEditor.archive.FindWhoTargets(assetID));

            return whoTargets;
        }

        public void ClearTemplateFocus()
        {
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                archiveEditor.ClearTemplateFocus();
        }

        public void ClearModelTemplateFocus()
        {
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                archiveEditor.archive.ClearModelTemplateFocus();
        }

        private void uIModeAutoSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Width = (int)(Height * 656f / 565f);
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

        private void UnselectTemplateButtons()
        {
            foreach (var i in templateButtons)
                i.Checked = false;
        }

        private void SelectPreviousTemplate()
        {
            if (ArchiveEditorFunctions.CurrentAssetTemplate == AssetTemplate.User_Template)
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
            if (ArchiveEditorFunctions.CurrentAssetTemplate == AssetTemplate.User_Template)
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

        private List<ToolStripMenuItem> templateButtons;

        private void TemplateToolStripItemClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var template = (AssetTemplate)item.Tag;

            UnselectTemplateButtons();

            ArchiveEditorFunctions.CurrentAssetTemplate = template;
            toolStripStatusLabelTemplate.Text = "Template: " + item.Text;
            toolStripComboBoxUserTemplate.SelectedItem = null;
            item.Checked = true;
        }

        private void userTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUserTemplateComboBox();
        }

        public void UpdateUserTemplateComboBox()
        {
            toolStripComboBoxUserTemplate.Items.Clear();

            foreach (string s in Directory.GetFiles(userTemplatesFolder))
            {
                toolStripComboBoxUserTemplate.Items.Add(Path.GetFileName(s));

                if (toolStripComboBoxUserTemplate.Size.Width < 8 * Path.GetFileName(s).Length)
                    toolStripComboBoxUserTemplate.Size = new System.Drawing.Size(8 * Path.GetFileName(s).Length, toolStripComboBoxUserTemplate.Size.Height);
            }
        }

        private void toolStripComboBoxUserTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBoxUserTemplate.SelectedIndex != -1)
            {
                UnselectTemplateButtons();
                ArchiveEditorFunctions.CurrentAssetTemplate = AssetTemplate.User_Template;
                ArchiveEditorFunctions.CurrentUserTemplate = toolStripComboBoxUserTemplate.SelectedItem.ToString();
                toolStripStatusLabelTemplate.Text = $"Template: {toolStripComboBoxUserTemplate.SelectedItem} (User)";
            }
        }

        private void manageUserTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.UserTemplateManager.Show();
        }

        private void templatesPersistentShiniesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templatesPersistentShiniesToolStripMenuItem.Checked = !templatesPersistentShiniesToolStripMenuItem.Checked;
            ArchiveEditorFunctions.persistentShinies = templatesPersistentShiniesToolStripMenuItem.Checked;
        }

        private void templatesChainPointMVPTsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templatesChainPointMVPTsToolStripMenuItem.Checked = !templatesChainPointMVPTsToolStripMenuItem.Checked;
            ArchiveEditorFunctions.chainPointMVPTs = templatesChainPointMVPTsToolStripMenuItem.Checked;
        }

        private void ToggleGizmoType(GizmoMode mode = GizmoMode.Null)
        {
            GizmoMode outMode = ArchiveEditorFunctions.ToggleGizmoType(mode);

            positionToolStripMenuItem.Checked = outMode == GizmoMode.Position;
            rotationToolStripMenuItem.Checked = outMode == GizmoMode.Rotation;
            scaleToolStripMenuItem.Checked = outMode == GizmoMode.Scale;
            positionLocalToolStripMenuItem.Checked = outMode == GizmoMode.PositionLocal;
        }

        private void positionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleGizmoType(GizmoMode.Position);
        }

        private void rotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleGizmoType(GizmoMode.Rotation);
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleGizmoType(GizmoMode.Scale);
        }

        private void positionLocalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleGizmoType(GizmoMode.PositionLocal);
        }

        private void eventSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.EventSearch == null)
                Program.EventSearch = new EventSearch();
            Program.EventSearch.Show();
        }

        private void assetIDGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.AssetIDGenerator == null)
                Program.AssetIDGenerator = new AssetIDGenerator();
            Program.AssetIDGenerator.Show();
        }

        private void dYNASearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.DynaSearch == null)
                Program.DynaSearch = new DynaSearch();
            Program.DynaSearch.Show();
        }

        private void pickupSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.PickupSearch == null)
                Program.PickupSearch = new PickupSearch();
            Program.PickupSearch.Show();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (renderer == null)
                return;

            try
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    renderer.allowRender = false;
                    SetAllTopMost(false);
                }
                else
                {
                    renderer.allowRender = true;
                    SetAllTopMost(true);
                }
            }
            catch
            {
            }
        }

        private void SetAllTopMost(bool value)
        {
            Program.AboutBox.TopMost = value;
            Program.ViewConfig.TopMost = value;
            Program.UserTemplateManager.TopMost = value;

            if (Program.EventSearch != null)
                Program.EventSearch.TopMost = value;
            if (Program.AssetIDGenerator != null)
                Program.AssetIDGenerator.TopMost = value;
            if (Program.DynaSearch != null)
                Program.DynaSearch.TopMost = value;
            if (Program.PickupSearch != null)
                Program.PickupSearch.TopMost = value;

            foreach (ArchiveEditor ae in archiveEditors)
                ae.SetAllTopMost(value);
        }

        private void runGameF5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TryToRunGame();
        }

        private void TryToRunGame()
        {
            string dolPath = null;
            string filesPath = null;
            string hipName = null;
            foreach (var ae in archiveEditors)
            {
                hipName = ae.GetCurrentlyOpenFileName().ToLower();
                string rootFolderName = Path.GetDirectoryName(Path.GetDirectoryName(hipName));

                if (!(hipName.Contains("boot") || hipName.Contains("font") || hipName.Contains("plat")))
                    rootFolderName = Path.GetDirectoryName(rootFolderName);

                dolPath = Path.Combine(rootFolderName, "sys", "main.dol");
                filesPath = Path.Combine(rootFolderName, "files");

                if (File.Exists(dolPath))
                    break;
                dolPath = null;
            }

            var validIniNames = new string[] { "sb.ini", "sb04.ini", "sd2.ini", "in.ini", "in2.ini" };

            if (hipName != null && !(hipName.Contains("boot") || hipName.Contains("font") || hipName.Contains("plat")) && filesPath != null)
                foreach (string s in Directory.GetFiles(filesPath))
                    if (validIniNames.Contains(Path.GetFileName(s).ToLower()))
                    {
                        string[] ini = File.ReadAllLines(s);
                        for (int i = 0; i < ini.Length; i++)
                            if (ini[i].StartsWith("BOOT"))
                            {
                                ini[i] = "BOOT=" + Path.GetFileNameWithoutExtension(hipName).ToUpper();
                                break;
                            }
                        File.WriteAllLines(s, ini);
                        break;
                    }

            if (dolPath == null)
                MessageBox.Show("Unable to find DOL to launch.", "Unable to find DOL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                try
                {
                    RemoteControl.TryToRunGame(dolPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to open Dolphin: " + ex.Message, "Error opening Dolphin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveAllOpenHIPsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var ae in archiveEditors)
                if (ae.archive.UnsavedChanges)
                    ae.Save();
        }

        private void hideInvisibleMeshesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideInvisibleMeshesToolStripMenuItem.Checked = !hideInvisibleMeshesToolStripMenuItem.Checked;
            RenderWareModelFile.dontDrawInvisible = hideInvisibleMeshesToolStripMenuItem.Checked;
        }

        private void updateReferencesOnCopyPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateReferencesOnCopyPasteToolStripMenuItem.Checked = !updateReferencesOnCopyPasteToolStripMenuItem.Checked;
            ArchiveEditorFunctions.updateReferencesOnCopy = updateReferencesOnCopyPasteToolStripMenuItem.Checked;
        }

        private void replaceAssetsOnPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            replaceAssetsOnPasteToolStripMenuItem.Checked = !replaceAssetsOnPasteToolStripMenuItem.Checked;
            ArchiveEditorFunctions.replaceAssetsOnPaste = replaceAssetsOnPasteToolStripMenuItem.Checked;
        }

        private void refreshTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshTexturesAndModels();
        }

        public void RefreshTexturesAndModels()
        {
            TextureManager.ClearTextures();
            foreach (ArchiveEditor ae in archiveEditors)
                ae.RefreshHop(renderer);
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
                        ArchiveEditorFunctions.ExportScene(a.FileName, format, textureExtension, out string[] textureNames);

                        if (MessageBox.Show("Do you also wish to export textures?", "Export Textures", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

                            foreach (var ae in archiveEditors)
                            {
                                var d = ae.archive.GetTexturesAsBitmaps(textureNames);
                                foreach (var key in d)
                                    bitmaps.Add(key.Key, key.Value);
                            }

                            RenderWareFile.ReadFileMethods.treatStuffAsByteArray = false;

                            foreach (string textureName in bitmaps.Keys)
                                bitmaps[textureName].Save(Path.Combine(a.FileName, textureName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
        }

        public void discordRichPresenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            discordRichPresenceToolStripMenuItem.Checked = !discordRichPresenceToolStripMenuItem.Checked;
            DiscordRPCController.ToggleDiscordRichPresence(discordRichPresenceToolStripMenuItem.Checked);
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
                p.Show();

                var layerLogs = new List<string>();
                var layerTypes = new Dictionary<LayerType, HashSet<AssetType>>();

                foreach (var hip in hipList)
                {
                    p.Text = Path.GetFileName(hip);
                    ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                    archive.OpenFile(hip, false, scoobyPlat);
                    scoobyPlat = archive.platform;
                    foreach (var v in archive.AssetTypesPerLayer())
                    {
                        if (!layerTypes.ContainsKey(v.Key))
                            layerTypes[v.Key] = new HashSet<AssetType>();
                        foreach (var t in v.Value)
                        {
                            layerTypes[v.Key].Add(t);
                            if (v.Key == LayerType.SRAM && v.Value.Contains(AssetType.SoundInfo))
                                MessageBox.Show(archive.currentlyOpenFilePath);
                        }
                    }

                    //var sndis = archive.GetAllAssets().Where(a => a.assetType == AssetType.SoundInfo);
                    //if (sndis.Any())
                    //{
                    //    archive.SelectedLayerIndex = archive.GetLayerFromAssetID(sndis.FirstOrDefault().assetID);
                    //    if (archive.GetLayerType() == (int)LayerType_BFBB.SRAM)
                    //        MessageBox.Show(archive.currentlyOpenFilePath);
                    //}
                    archive.Dispose(false);
                    p.PerformStep();
                }

                File.WriteAllText("layerslogs.txt", string.Join("\n", layerTypes.OrderBy(l => (int)l.Key).Select(k => $"{k.Key}: {string.Join(", ", k.Value.Select(v => v.ToString()).OrderBy(f => f))}")));

                p.Close();
            }
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

        private void openLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OpenLevel().Show();
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

            ArchiveEditor[] editorsToClose = archiveEditors.ToArray();

            // close all editors
            foreach (var editor in editorsToClose)
            {
                editor.CloseArchiveEditor();
            }

            closeAllEditorsToolStripMenuItem.Enabled = false;
            SetAllAssetTypesVisible();
        }

        private void DropSelectedAssets()
        {
            foreach (var ae in archiveEditors)
                ae.archive.DropSelectedAssets(renderer);
        }

        private void showVertexColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showVertexColorsToolStripMenuItem.Checked = !showVertexColorsToolStripMenuItem.Checked;
            renderer.ToggleVertexColors(showVertexColorsToolStripMenuItem.Checked);
        }

        private void buildAndRunPS2ISOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.BuildISO == null)
                Program.BuildISO = new BuildISO();
            Program.BuildISO.Show();
        }

        private void createGameCubeBannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bannerCreator = new CreateGameCubeBanner();
            bannerCreator.Show();
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

        private void changeFlyModeCursor(object sender, EventArgs e)
        {
            setFlyCursor(Convert.ToInt32(((ToolStripMenuItem)sender).Tag));
        }

        private void setFlyCursor(int flyModeCursor)
        {
            this.flyModeCursor = (FlyModeCursor)flyModeCursor;

            // Updaate the toolstrip dropdown items
            foreach (ToolStripMenuItem item in cursorInFlyModeToolStripMenuItem.DropDownItems)
                item.Checked = Convert.ToInt32(item.Tag) == flyModeCursor;
        }

        private void showEditorsWhenLoadingProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showEditorsWhenLoadingProjectToolStripMenuItem.Checked = !showEditorsWhenLoadingProjectToolStripMenuItem.Checked;
        }
    }
}