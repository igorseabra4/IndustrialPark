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

        private readonly IPSettings settings;

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

            templateButtons = ArchiveEditorFunctions.PopulateTemplateMenusAt(toolStripMenuItem_Templates, TemplateToolStripItemClick);

            renderer = new SharpRenderer(renderPanel);

            gizmoManager = new GizmoManager(() => archiveEditors.SelectMany(ae => ae.archive.CurrentlySelectedAssets));

            SetupToolbar();
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
                    Invoke(renderer.RunMainLoop, renderPanel);
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

            if (settings.FirstTime)
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
                Program.AboutBox.Show(this);
            }

            ApplyIPSettings(settings);

            SetProjectToolStripStatusLabel();
            StartRenderer();
            UpdateTitleBar();
            ResetMouseCenter(null, null);
            UpdateInitialDockingArea();
        }

        private void ApplyIPSettings(IPSettings settings)
        {
            SystemColorMode = settings.ColorMode;

            if (settings.CheckForUpdatesOnStartup && AutomaticUpdater.UpdateIndustrialPark(out _))
            {
                Close();
                System.Diagnostics.Process.Start(Application.StartupPath + "/IndustrialPark.exe");
            }

            autoSaveOnClosingToolStripMenuItem.Checked = settings.AutosaveOnClose;
            autoLoadOnStartupToolStripMenuItem.Checked = settings.AutoloadOnStartup;

            checkForUpdatesOnStartupToolStripMenuItem.Checked = settings.CheckForUpdatesOnStartup;
            AutomaticUpdater.LastCheckedForUpdate = settings.LastCheckedForUpdate;

            drawOnlyFirstMINFReferenceToolStripMenuItem.Checked = settings.drawOnlyFirstMinf;
            AssetMINF.drawOnlyFirst = settings.drawOnlyFirstMinf;

            useLODTForRenderingToolStripMenuItem.Checked = settings.renderBasedOnLodt;
            AssetMODL.renderBasedOnLodt = settings.renderBasedOnLodt;

            usePIPTForRenderingToolStripMenuItem.Checked = settings.renderBasedOnPipt;
            AssetMODL.renderBasedOnPipt = settings.renderBasedOnPipt;

            allMovePointsArePointsToolStripMenuItem.Checked = settings.drawMvptsAsPoints;
            AssetMVPT.DrawAllAsPoints = settings.drawMvptsAsPoints;

            hideInvisibleMeshesToolStripMenuItem.Checked = settings.dontDrawInvisible;
            RenderWareModelFile.dontDrawInvisible = settings.dontDrawInvisible;

            SetToolbarVisible(settings.showToolbar);
            if (settings.toolbarAssetTypes != null)
                SetToolbarAssetTypes(ParseToolbarAssetTypes(settings.toolbarAssetTypes));

            templatesPersistentShiniesToolStripMenuItem.Checked = settings.persistentShinies;
            ArchiveEditorFunctions.persistentShinies = settings.persistentShinies;

            useLegacyAssetIDFormatToolStripMenuItem.Checked = settings.LegacyAssetIDFormat;
            HexUIntTypeConverter.Legacy = settings.LegacyAssetIDFormat;

            useLegacyAssetTypeFormatToolStripMenuItem.Checked = settings.LegacyAssetTypeFormat;
            AssetTypeContainer.LegacyAssetNameFormat = settings.LegacyAssetTypeFormat;

            discordRichPresenceToolStripMenuItem.Checked = settings.discordRichPresence;
            DiscordRPCController.ToggleDiscordRichPresence(discordRichPresenceToolStripMenuItem.Checked);

            ArchiveEditor.AlwaysListAllAssets = settings.AlwaysListAllAssets;
            listAllAssetsToolStripMenuItem.Checked = settings.AlwaysListAllAssets;

            gizmoManager.RotationGizmoLocalAxis = settings.RotationGizmoLocalAxis;
            rotationGizmoLocalAxisToolStripMenuItem.Checked = settings.RotationGizmoLocalAxis;

            enableAssetToolButtonsToolStripMenuItem.Checked = settings.EnableAssetToolButtons;
            InternalEditor.EnableAssetToolButtons = settings.EnableAssetToolButtons;

            AutoDockArchiveEditors = settings.AutoDockArchiveEditor;
            autoDockArchiveEditorToolStripMenuItem.Checked = ShowToolbar && AutoDockArchiveEditors;

            autoDockAssetEditorsToolStripMenuItem.Checked = settings.AutoDockAssetEditors;
            ArchiveEditorFunctions.AutoDockAssetEditors = settings.AutoDockAssetEditors;

            openDockedEditorOnSelectionToolStripMenuItem.Checked = settings.AutoDockSelectedAssetEditors;
            ArchiveEditorFunctions.AutoDockSelectedAssetEditors = settings.AutoDockSelectedAssetEditors;

            gizmoConesToolStripMenuItem.Checked = settings.GizmoCones;
            GizmoBase.UseCones = settings.GizmoCones;

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
            showEditorsWhenLoadingProjectToolStripMenuItem.Checked = showEditorsWhenLoadingProject;

            setFlyCursor(settings.flyModeCursor);
            TranslucentWhenOutOfFocus = settings.translucentEditor;
            translucentToolStripMenuItem.Checked = TranslucentWhenOutOfFocus;

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
                FirstTime = false,
                AutosaveOnClose = autoSaveOnClosingToolStripMenuItem.Checked,
                AutoloadOnStartup = autoLoadOnStartupToolStripMenuItem.Checked,
                LastProjectPath = currentProjectPath,
                CheckForUpdatesOnStartup = checkForUpdatesOnStartupToolStripMenuItem.Checked,
                LastCheckedForUpdate = AutomaticUpdater.LastCheckedForUpdate,
                drawOnlyFirstMinf = AssetMINF.drawOnlyFirst,
                renderBasedOnLodt = AssetMODL.renderBasedOnLodt,
                renderBasedOnPipt = AssetMODL.renderBasedOnPipt,
                drawMvptsAsPoints = AssetMVPT.DrawAllAsPoints,
                discordRichPresence = discordRichPresenceToolStripMenuItem.Checked,
                dontDrawInvisible = RenderWareModelFile.dontDrawInvisible,
                showToolbar = showToolbarToolStripMenuItem.Checked,
                toolbarAssetTypes = toolbarAssetTypes.Select(t => t.ToString()).ToArray(),
                persistentShinies = ArchiveEditorFunctions.persistentShinies,
                LegacyAssetIDFormat = HexUIntTypeConverter.Legacy,
                LegacyAssetTypeFormat = AssetTypeContainer.LegacyAssetNameFormat,
                pcsx2Path = BuildISO.PCSX2Path,
                recentBuildIsoGamePaths = BuildISO.recentGameDirPaths,
                recentArchivePaths = openLastToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().Select(x => x.Text).ToArray(),
                flyModeCursor = (int)flyModeCursor,
                translucentEditor = TranslucentWhenOutOfFocus,
                showEditorsWhenLoadingProject = showEditorsWhenLoadingProjectToolStripMenuItem.Checked,
                ColorMode = SystemColorMode,
                AlwaysListAllAssets = ArchiveEditor.AlwaysListAllAssets,
                RotationGizmoLocalAxis = gizmoManager.RotationGizmoLocalAxis,
                EnableAssetToolButtons = InternalEditor.EnableAssetToolButtons,
                AutoDockArchiveEditor = AutoDockArchiveEditors,
                AutoDockAssetEditors = ArchiveEditorFunctions.AutoDockAssetEditors,
                AutoDockSelectedAssetEditors = ArchiveEditorFunctions.AutoDockSelectedAssetEditors,
                GizmoCones = GizmoBase.UseCones,
                dockLeftSize = dockLeftSizeSave,
                dockRightSize = dockRightSizeSave,
                Speed = renderer.Camera.Speed,
                SpeedRot = renderer.Camera.SpeedRot,
                fog = !AssetFOG.DontRender,
                DolphinPath = RemoteControl.DolphinPath,
                Colors = renderer.colors,
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
                fileName = Path.Combine(Application.StartupPath, "default_project.json");
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
                FieldOfView = renderer.Camera.FieldOfView,
                FarPlane = renderer.Camera.FarPlane,
                BackgroundColor = SharpRenderer.backgroundColor,
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

            TextureManager.ClearTextures();

            List<ArchiveEditor> archiveEditors = [.. this.archiveEditors];
            foreach (ArchiveEditor ae in archiveEditors)
                ae.CloseArchiveEditor();

            TextureManager.LoadTexturesFromFolder(project.TextureFolderPaths);

            ArchiveEditorFunctions.hiddenAssets = project.hiddenAssets;

            for (int i = 0; i < project.hipPaths.Count; i++)
                if (project.hipPaths[i] == "Empty")
                    AddArchiveEditor();
                else
                {
                    if (File.Exists(project.hipPaths[i]))
                        AddArchiveEditor(project.hipPaths[i], project.scoobyPlatforms[i], showEditors);
                    else
                    {
                        MessageBox.Show("Error opening " + project.hipPaths[i] + ": file not found",
                            "File not Found",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }

            ArchiveEditorFunctions.hiddenAssets.Clear();

            renderer.Camera.SetPosition(project.CamPos);
            renderer.Camera.Yaw = project.Yaw;
            renderer.Camera.Pitch = project.Pitch;
            renderer.Camera.FieldOfView = project.FieldOfView;
            renderer.Camera.FarPlane = project.FarPlane;
            gizmoManager.Grid = project.Grid;

            SharpRenderer.backgroundColor = project.BackgroundColor;

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

        public void SetToolStripStatusLabel(string Text)
        {
            toolStripStatusLabelMain.Text = Text;
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
        public bool TranslucentWhenOutOfFocus;

        private void renderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
                ScreenUnclicked();

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

                    renderer.Camera.AddYaw((Cursor.Position.X - MouseCenter.X) * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                    renderer.Camera.AddPitch((Cursor.Position.Y - MouseCenter.Y) * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);

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
                        float deltaX = e.X - oldMousePosition.X;
                        float deltaY = e.Y - oldMousePosition.Y;
                        if (PressedKeys.Contains(Keys.ControlKey))
                        {
                            renderer.Camera.AddPositionForward(-deltaY * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                        }
                        else if (PressedKeys.Contains(Keys.ShiftKey) && !PressedKeys.Contains(Keys.W) && !PressedKeys.Contains(Keys.A) && !PressedKeys.Contains(Keys.S) && !PressedKeys.Contains(Keys.D))
                        {
                            renderer.Camera.AddPositionSideways(deltaX * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                            renderer.Camera.AddPositionUp(deltaY * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                        }
                        else
                        {
                            float rollRadians = MathUtil.DegreesToRadians(renderer.Camera.Roll);
                            float adjustedDeltaX = deltaX * (float)Math.Cos(rollRadians) - deltaY * (float)Math.Sin(rollRadians);
                            float adjustedDeltaY = deltaX * (float)Math.Sin(rollRadians) + deltaY * (float)Math.Cos(rollRadians);
                            renderer.Camera.AddYaw(adjustedDeltaX * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                            renderer.Camera.AddPitch(adjustedDeltaY * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);

                            // Clamp camera pitch so it doesn't loop around
                            float pitchPadding = 0.01f;
                            renderer.Camera.Pitch = MathUtil.Clamp(renderer.Camera.Pitch,
                                -90 + pitchPadding,
                                90 - pitchPadding);
                        }
                    }
                }

                if (e.Delta != 0)
                    renderer.Camera.AddPositionForward(e.Delta * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
            }

            gizmoManager.MouseMoveGizmos(renderer.viewProjection, e.X - oldMousePosition.X, e.Y - oldMousePosition.Y, PressedKeys.Contains(Keys.T));

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

        private readonly HashSet<Keys> PressedKeys = new HashSet<Keys>();

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            PressedKeys.Add(e.KeyCode);

            if ((PressedKeys.Contains(Keys.Y) && PressedKeys.Contains(Keys.ControlKey)) ||
                (PressedKeys.Contains(Keys.Z) && PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.ShiftKey)))
            {
                Program.UndoBuffer.Redo();
                return;
            }
            else if (PressedKeys.Contains(Keys.Z) && PressedKeys.Contains(Keys.ControlKey))
            {
                Program.UndoBuffer.Undo();
                return;
            }

            if (!renderPanel.Focused)
                return;

            switch (e.KeyCode)
            {
                case Keys.B:
                    SelectPreviousTemplate();
                    break;
                case Keys.C when PressedKeys.Contains(Keys.ControlKey):
                    CopySelectedAssets();
                    break;
                case Keys.C:
                    noCullingCToolStripMenuItem.PerformClick();
                    break;
                case Keys.F:
                    fogToolStripMenuItem.PerformClick();
                    break;
                case Keys.G when PressedKeys.Contains(Keys.ControlKey):
                    OpenSelectedAssetArchiveEditor();
                    break;
                case Keys.G:
                    OpenInternalEditors();
                    break;
                case Keys.H when PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.ShiftKey):
                    CloseMostForms();
                    break;
                case Keys.H when PressedKeys.Contains(Keys.ControlKey):
                    UnhideAllAssets();
                    break;
                case Keys.H:
                    HideSelectedAssets();
                    break;
                case Keys.J when PressedKeys.Contains(Keys.ControlKey):
                    DropSelectedAssets();
                    break;
                case Keys.J:
                    hideInvisibleMeshesToolStripMenuItem.PerformClick();
                    break;
                case Keys.L:
                    useLightKitsForRenderingToolStripMenuItem.PerformClick();
                    break;
                case Keys.M:
                    movementPreviewToolStripMenuItem.PerformClick();
                    break;
                case Keys.N when PressedKeys.Contains(Keys.ControlKey):
                    newToolStripMenuItem.PerformClick();
                    break;
                case Keys.N:
                    SelectNextTemplate();
                    break;
                case Keys.O when PressedKeys.Contains(Keys.ControlKey):
                    importLevelToolStripMenuItem.PerformClick();
                    break;
                case Keys.P:
                    showVertexColorsToolStripMenuItem.PerformClick();
                    break;
                case Keys.R when PressedKeys.Contains(Keys.ControlKey):
                    renderer.Camera.ResetRoll();
                    break;
                case Keys.R:
                    renderer.Camera.Reset();
                    break;
                case Keys.U:
                    uIModeToolStripMenuItem.PerformClick();
                    break;
                case Keys.V when PressedKeys.Contains(Keys.ControlKey):
                    PasteSelectedAssets();
                    break;
                case Keys.V:
                    SetGizmoMode(GizmoMode.Cycle);
                    break;
                case Keys.Y:
                    wireframeFToolStripMenuItem.PerformClick();
                    break;
                case Keys.Z:
                    MouseModeToggle();
                    break;
                case Keys.Delete:
                    DeleteSelectedAssets();
                    break;
                case Keys.F1:
                    viewControlsToolStripMenuItem.PerformClick();
                    break;
                case Keys.F2:
                    viewConfigToolStripMenuItem.PerformClick();
                    break;
                case Keys.F3:
                    aboutToolStripMenuItem.PerformClick();
                    break;
                case Keys.F4:
                case Keys.S when PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.ShiftKey):
                    saveAllOpenHIPsToolStripMenuItem.PerformClick();
                    break;
                case Keys.F5:
                    runGameF5ToolStripMenuItem.PerformClick();
                    break;
                case Keys.F6:
                    buildAndRunPS2ISOToolStripMenuItem.PerformClick();
                    break;
                case Keys.F7:
                    createGameCubeBannerToolStripMenuItem.PerformClick();
                    break;
                case Keys.D1:
                    renderer.Camera.IncreaseCameraSpeed(-SharpCamera.DEFAULT_SPEED_INCREMENT);
                    break;
                case Keys.D2:
                    renderer.Camera.IncreaseCameraSpeed(SharpCamera.DEFAULT_SPEED_INCREMENT);
                    break;
                case Keys.D3:
                    renderer.Camera.IncreaseCameraRotationSpeed(-SharpCamera.DEFAULT_SPEED_ROT_INCREMENT);
                    break;
                case Keys.D4:
                    renderer.Camera.IncreaseCameraRotationSpeed(SharpCamera.DEFAULT_SPEED_ROT_INCREMENT);
                    break;
            }

            // Open archive editors with ctrl and 1-9
            if (PressedKeys.Contains(Keys.ControlKey))
            {
                // in enum, Keys.D1 through Keys.D9 are numbers 49 to 57.
                int numberKeyOffset = 49;
                var editorsToShow = new List<ArchiveEditor>();

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
                    ShowArchiveEditor(form);
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
            gizmoManager.SkipGizmos = PressedKeys.Contains(Keys.ShiftKey) || PressedKeys.Contains(Keys.ControlKey);

            if (!renderPanel.Focused || renderer.isDrawingUI)
                return;

            if (PressedKeys.Contains(Keys.A) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddYaw(-renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.A))
                renderer.Camera.AddPositionSideways(renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.D) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddYaw(renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.D))
                renderer.Camera.AddPositionSideways(-renderer.TransformScaleFactor);

            if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPitch(-renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ShiftKey))
                renderer.Camera.AddPositionUp(renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.W))
                renderer.Camera.AddPositionForward(renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ControlKey) & !PressedKeys.Contains(Keys.ShiftKey))
                renderer.Camera.AddPitch(renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ShiftKey) & !PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPositionUp(-renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.S) & !PressedKeys.Contains(Keys.ShiftKey) & !PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPositionForward(-renderer.TransformScaleFactor);

            if (PressedKeys.Contains(Keys.Q) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddRoll(renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.Q))
                renderer.Camera.AddPositionUp(-renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.E) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddRoll(-renderer.TransformScaleFactor);
            else if (PressedKeys.Contains(Keys.E))
                renderer.Camera.AddPositionUp(renderer.TransformScaleFactor);

            float pitchPadding = 0.01f;
            renderer.Camera.Pitch = MathUtil.Clamp(renderer.Camera.Pitch,
                -90 + pitchPadding,
                90 - pitchPadding);
        }

        public readonly GizmoManager gizmoManager;

        public readonly List<ArchiveEditor> archiveEditors = new List<ArchiveEditor>();

        private ArchiveEditor GetTargetArchiveEditor() => archiveEditors.FirstOrDefault(ae => ae.Targeted);

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddArchiveEditor();
        }

        public ArchiveEditor AddArchiveEditor(string filePath = null, Platform scoobyPlatform = Platform.Unknown, bool show = true)
        {
            ArchiveEditor ae = new ArchiveEditor(filePath, scoobyPlatform);
            archiveEditors.Add(ae);

            ToolStripMenuItem tempMenuItem = new ToolStripMenuItem(Path.GetFileName(ae.GetCurrentlyOpenFileName()));
            tempMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ShowArchiveEditor(ae);
            });
            archiveEditorToolStripMenuItem.DropDownItems.Add(tempMenuItem);

            ae.archive.ChangesMade += UpdateTitleBar;
            ae.archive.CurrentlySelectedAssets.CollectionChanged += UpdateSelectedAssetStatusBarItem;
            ae.EditorUpdate += EditorUpdate;
            ae.ToolbarStateChanged += ArchiveEditor_ToolbarStateChanged;
            UpdateTitleBar();
            SetupAssetVisibilityButtons();
            closeAllEditorsToolStripMenuItem.Enabled = true;
            UpdateToolbarArchives();

            ae.Show(this);

            // Cannot select assets if the editor isn't shown first?
            if (!show)
                ae.Hide();

            return ae;
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
            UpdateToolbarArchives();
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
            Program.UndoBuffer.RemoveActionsOfArchive(sender.archive);
            UpdateToolbarArchives();
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

        private void noCullingCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            noCullingCToolStripMenuItem.Checked = !noCullingCToolStripMenuItem.Checked;
            renderer.device.SetNormalCullMode(noCullingCToolStripMenuItem.Checked ? CullMode.None : CullMode.Back);
        }

        private void wireframeFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wireframeFToolStripMenuItem.Checked = !wireframeFToolStripMenuItem.Checked;
            renderer.device.SetNormalFillMode(wireframeFToolStripMenuItem.Checked ? FillMode.Wireframe : FillMode.Solid);
            toolStripButtonWireframe.Checked = wireframeFToolStripMenuItem.Checked;
        }

        private void backgroundColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(BitConverter.ToInt32(BitConverter.GetBytes(SharpRenderer.backgroundColor.ToBgra()).Reverse().ToArray(), 0))
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
                SharpRenderer.backgroundColor = new SharpDX.Color(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B, colorDialog.Color.A);
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
            if (PressedKeys.Contains(Keys.ShiftKey))
                renderer.colors = RendererColors.Classic;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            viewConfigToolStripMenuItem.PerformClick();
        }

        private void viewConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.ViewConfig.WindowState == FormWindowState.Minimized)
                Program.ViewConfig.WindowState = FormWindowState.Normal;

            if (!Program.ViewConfig.Visible)
                Program.ViewConfig.Show(this);
            Program.ViewConfig.Activate();
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

                if (GetTargetArchiveEditor() is ArchiveEditor archiveEditor)
                    archiveEditor.PlaceTemplate(Position);
                else
                    MessageBox.Show($"The template could not be placed because no Archive Editor is selected as the target.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (gizmoManager.IsMovingGizmo)
                return;

            bool addToSelection = PressedKeys.Contains(Keys.ShiftKey);
            bool toggleSelection = PressedKeys.Contains(Keys.ControlKey);

            Ray ray = Ray.GetPickRay(X, Y, new Viewport(viewRectangle), renderer.viewProjection);

            if (isMouseDown)
            {
                if (!addToSelection && !toggleSelection)
                    gizmoManager.TrySelectGizmo(ray);
            }
            else
            {
                // both ctrl and shift
                var removeFromSelection = addToSelection && toggleSelection;

                // asset previous selection state must be equal to this, ignored if null
                bool? alreadySelectedFilter = removeFromSelection ? true : addToSelection ? false : null;

                (var archive, var assetID) = ArchiveEditorFunctions.GetClickedAsset(renderer, ray, alreadySelectedFilter);

                if (assetID == null && (addToSelection || toggleSelection))
                    return;

                foreach (ArchiveEditor ae in archiveEditors)
                    if (removeFromSelection)
                        ae.RemoveFromSelection(ae.archive == archive ? assetID : null);
                    else
                        ae.SetSelectedIndex(ae.archive == archive ? assetID : null, addToSelection, toggleSelection);
                if (ArchiveEditorFunctions.AutoDockSelectedAssetEditors)
                    renderPanel.Focus();
            }
        }

        public Vector3 GetScreenClickedPosition(SharpDX.Rectangle viewRectangle, int X, int Y) =>
            ArchiveEditorFunctions.GetRayIntersectionPosition(renderer,
                Ray.GetPickRay(X, Y, new Viewport(viewRectangle), renderer.viewProjection));

        private void renderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            ScreenUnclicked();
        }

        private void renderPanel_MouseLeave(object sender, EventArgs e)
        {
            ScreenUnclicked();
        }

        public void ScreenUnclicked()
        {
            gizmoManager.FinishGizmoMovement();
        }

        public void SetSelectedIndices(List<uint> assetIDs)
        {
            foreach (ArchiveEditor ae in archiveEditors)
                ae.SetSelectedIndices(assetIDs);
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

        private void OpenSelectedAssetArchiveEditor()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                if (ae.archive.CurrentlySelectedAssets.Count > 0)
                    ShowArchiveEditor(ae);
        }

        private void ShowArchiveEditor(ArchiveEditor ae)
        {
            if (ae.AssetListDocked)
                return;

            if (ae.WindowState == FormWindowState.Minimized)
                ae.WindowState = FormWindowState.Normal;

            if (!ae.Visible)
                ae.Show(this);
            ae.Activate();
            ae.ActiveControl = null;
        }

        private void DeleteSelectedAssets()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                ae.DeleteSelectedAssets();
        }

        private void movementPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            movementPreviewToolStripMenuItem.Checked = !movementPreviewToolStripMenuItem.Checked;
            EntityAsset.movementPreview = movementPreviewToolStripMenuItem.Checked;

            ResetMovementPreview();
        }

        public void ResetMovementPreview()
        {
            foreach (ArchiveEditor ae in archiveEditors)
                foreach (EntityAsset p in ae.archive.GetAllAssets().OfType<EntityAsset>())
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
            UpdateToolbarVisibilityButtons();
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
            foreach (var ae in archiveEditors)
                foreach (var asset in ae.archive.GetAllAssets())
                    assetTypes.Add(asset.assetType);


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
            if (!Program.AboutBox.Visible)
                Program.AboutBox.Show(this);
            Program.AboutBox.Activate();
        }

        public string GetAssetNameFromID(uint assetID)
        {
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                if (archiveEditor.archive.ContainsAsset(assetID))
                    return archiveEditor.GetAssetNameFromID(assetID);

            return ArchiveEditorFunctions.GetFromNameDictionary(assetID) ?? "0x" + assetID.ToString("X8");
        }

        public bool AssetExists(uint assetID)
        {
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                if (archiveEditor.archive.ContainsAsset(assetID))
                    return true;

            if (ArchiveEditorFunctions.GetFromNameDictionary(assetID) != null)
                return true;

            return false;
        }

        public void FindWhoTargets(uint assetID)
        {
            List<uint> whoTargets = WhoTargets(assetID);

            bool willOpen = true;
            if (whoTargets.Count > MaxDockedEditors)
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

        public void ClearModelTemplateFocus(InternalModelEditor except)
        {
            foreach (ArchiveEditor archiveEditor in archiveEditors)
                archiveEditor.archive.ClearModelTemplateFocus(except);
        }

        private void SetViewAspect(float aspect)
        {
            ClientSize = new Size((int)(renderPanel.Height * aspect), ClientSize.Height);
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
            if (!Program.UserTemplateManager.Visible)
                Program.UserTemplateManager.Show(this);
            Program.UserTemplateManager.Activate();
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

        private void eventSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.EventSearch ??= new EventSearch();
            if (!Program.EventSearch.Visible)
                Program.EventSearch.Show(this);
        }

        private void assetIDGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.AssetIDGenerator ??= new AssetIDGenerator();
            if (!Program.AssetIDGenerator.Visible)
                Program.AssetIDGenerator.Show(this);
        }

        private void dYNASearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.DynaSearch ??= new DynaSearch();
            if (!Program.DynaSearch.Visible)
                Program.DynaSearch.Show(this);
        }

        private void pickupSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.PickupSearch ??= new PickupSearch();
            if (!Program.PickupSearch.Visible)
                Program.PickupSearch.Show(this);
        }

        private void hansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.HansMainForm ??= new IndustrialPark.SaveFile.HansMainForm();
            if (!Program.HansMainForm.Visible)
                Program.HansMainForm.Show(this);
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
                }
                else
                {
                    renderer.allowRender = true;
                }
            }
            catch
            {
            }
        }

        private void runGameF5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ae = archiveEditors.FirstOrDefault(ae => ae.archive.LevelName.Length == 4 && ae.archive.LevelName is not ("boot" or "font" or "plat"));
            try
            {
                if (ae == null)
                    throw new Exception("No Archive Editor containing a stage found.");
                RemoteControl.RunGame(ae.archive.game, ae.GetCurrentlyOpenFileName());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to run game:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ArchiveEditorFunctions.RecalculateAllMatrices();
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
                p.Show(this);

                var layerLogs = new List<string>();
                var layerTypes = new Dictionary<LayerType, HashSet<AssetType>>();

                foreach (var hip in hipList)
                {
                    p.Text = Path.GetFileName(hip);
                    ArchiveEditorFunctions archive = new ArchiveEditorFunctions(null);
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
            new OpenLevel().Show(this);
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

        private void HideSelectedAssets()
        {
            foreach (var ae in archiveEditors)
                ae.HideSelectedAssets();
        }

        private void UnhideAllAssets()
        {
            bool unhid = false;
            foreach (var ae in archiveEditors)
                unhid |= ae.UnhideAllAssets();
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

        private void showVertexColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpRenderer.RenderVertexColors = showVertexColorsToolStripMenuItem.Checked;
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

        private void translucentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            translucentToolStripMenuItem.Checked = !translucentToolStripMenuItem.Checked;
            TranslucentWhenOutOfFocus = translucentToolStripMenuItem.Checked;

            foreach (var ae in archiveEditors)
            {
                ae.Opacity = (TranslucentWhenOutOfFocus && !ae.OwnedForms.Any() && !ae.Focused) ? 0.5f : 1f;
                ae.archive.SetAllOpacity();
            }
        }

        private void alwaysListAllAssetsOnArchiveEditorWhenSelectingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listAllAssetsToolStripMenuItem.Checked = !listAllAssetsToolStripMenuItem.Checked;
            ArchiveEditor.AlwaysListAllAssets = listAllAssetsToolStripMenuItem.Checked;
        }

        private void rotationGizmoOnSingleAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotationGizmoLocalAxisToolStripMenuItem.Checked = !rotationGizmoLocalAxisToolStripMenuItem.Checked;
            gizmoManager.RotationGizmoLocalAxis = rotationGizmoLocalAxisToolStripMenuItem.Checked;
        }

        private void enableAssetToolButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableAssetToolButtonsToolStripMenuItem.Checked = !enableAssetToolButtonsToolStripMenuItem.Checked;
            InternalEditor.EnableAssetToolButtons = enableAssetToolButtonsToolStripMenuItem.Checked;
        }

        private void fogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetFOG.DontRender = !fogToolStripMenuItem.Checked;
            if (AssetFOG.DontRender)
                renderer.Camera.FarPlane = SharpCamera.DEFAULT_FAR_PLANE;
        }

        private void useLightKitsForRenderingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssetLKIT.DontRender = !useLightKitsForRenderingToolStripMenuItem.Checked;
        }

        private void allMovePointsArePointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allMovePointsArePointsToolStripMenuItem.Checked = !allMovePointsArePointsToolStripMenuItem.Checked;
            AssetMVPT.DrawAllAsPoints = allMovePointsArePointsToolStripMenuItem.Checked;
            ArchiveEditorFunctions.RecalculateAllMatrices();
        }

        private void autoDockArchiveEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoDockArchiveEditorToolStripMenuItem.Checked = !autoDockArchiveEditorToolStripMenuItem.Checked;
            AutoDockArchiveEditors = autoDockArchiveEditorToolStripMenuItem.Checked;

            RefreshAssetListDock();
        }

        private void autoDockAssetEditorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoDockAssetEditorsToolStripMenuItem.Checked = !autoDockAssetEditorsToolStripMenuItem.Checked;
            ArchiveEditorFunctions.AutoDockAssetEditors = autoDockAssetEditorsToolStripMenuItem.Checked;
        }

        private void openDockedEditorOnSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openDockedEditorOnSelectionToolStripMenuItem.Checked = !openDockedEditorOnSelectionToolStripMenuItem.Checked;
            ArchiveEditorFunctions.AutoDockSelectedAssetEditors = openDockedEditorOnSelectionToolStripMenuItem.Checked;
        }

        private void gizmoConeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gizmoConesToolStripMenuItem.Checked = !gizmoConesToolStripMenuItem.Checked;
            GizmoBase.UseCones = gizmoConesToolStripMenuItem.Checked;
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

        private void viewUndoBufferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ScrollableMessageBox("Undo Buffer", Program.UndoBuffer.ToString()).Show(this);
        }

        public void RefreshAssetList(ArchiveEditorFunctions archive)
        {
            foreach (var ae in archiveEditors)
                if (ae.archive == archive)
                {
                    var layerIndex = ae.archive.NoLayers ? -1 : ae.SelectedLayerIndex;
                    ae.PopulateLayerComboBox();
                    ae.SelectedLayerIndex = layerIndex;
                }
        }

        internal void RefreshLayerList(ArchiveEditorFunctions archive, int index = -1)
        {
            var ae = archive.GetArchiveEditor();
            if (!ae.archive.NoLayers)
            {
                ae.PopulateLayerComboBox();
                if (index >= 0 && index < ae.archive.LayerCount)
                    ae.SelectedLayerIndex = index;
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

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.UndoBuffer.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.UndoBuffer.Redo();
        }

        private void CopySelectedAssets()
        {
            var clipboard = new List<AssetClipboard>();
            foreach (var ae in archiveEditors)
                clipboard.AddRange(ae.archive.CreateAssetClipboard());
            Clipboard.SetText(JsonConvert.SerializeObject(clipboard, Formatting.None));
        }

        private void PasteSelectedAssets()
        {
            if (GetTargetArchiveEditor() is ArchiveEditor archiveEditor)
            {
                if (!archiveEditor.archive.NoLayers && archiveEditor.SelectedLayerIndex == -1)
                {
                    MessageBox.Show("Please select a layer to paste your asset in!");
                }
                else
                {
                    (bool success, var finalIndices, _) = archiveEditor.archive.PasteAssetsFromClipboard(archiveEditor.SelectedLayerIndex);
                    if (success)
                        archiveEditor.SetSelectedIndices(finalIndices);
                }
            }
            else
                MessageBox.Show($"Nothing was pasted because no Archive Editor is selected as the target.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}