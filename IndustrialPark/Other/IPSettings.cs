using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class IPSettings
    {
        public bool FirstTime = true;
        public bool AutosaveOnClose = true;
        public bool AutoloadOnStartup = true;
        public string LastProjectPath = null;

        public bool CheckForUpdatesOnStartup = true;
        public DateTime LastCheckedForUpdate = DateTime.MinValue;

        public string[] recentArchivePaths;

        public float Speed = SharpCamera.DEFAULT_SPEED;
        public float SpeedRot = SharpCamera.DEFAULT_SPEED_ROT;

        public bool drawOnlyFirstMinf = false;
        public bool renderBasedOnLodt = false;
        public bool renderBasedOnPipt = true;
        public bool discordRichPresence = true;
        public bool dontDrawInvisible = false;
        public bool drawMvptsAsPoints = false;
        public bool showToolbar = true;
        public string[] toolbarAssetTypes = null; // AssetType names; null uses the default set
        public bool persistentShinies = true;
        public bool LegacyAssetIDFormat = false;
        public bool LegacyAssetTypeFormat = false;
        public bool AlwaysListAllAssets = true;
        public bool RotationGizmoLocalAxis = true;
        public bool EnableAssetToolButtons = true;
        public bool AutoDockArchiveEditor = false;
        public bool AutoDockAssetEditors = false;
        public bool AutoDockSelectedAssetEditors = false;
        public bool GizmoCones = true;

        public RendererColors Colors = RendererColors.Default;
        public bool fog = true;

        public int dockLeftSize = -1;
        public int dockRightSize = -1;

        public string DolphinPath;
        public string pcsx2Path;
        public string[] recentBuildIsoGamePaths;
        public int flyModeCursor = 1;
        public bool translucentEditor = true;
        public bool showEditorsWhenLoadingProject = true;
        public SystemColorMode ColorMode = SystemColorMode.System;
    }
}
