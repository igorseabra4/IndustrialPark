using System;

namespace IndustrialPark
{
    public class IPSettings
    {
        public bool AutosaveOnClose;
        public bool AutoloadOnStartup;
        public string LastProjectPath;

        public bool CheckForUpdatesOnStartup;
        public DateTime LastCheckedForUpdate;

        public string[] recentArchivePaths;

        public bool drawOnlyFirstMinf = false;
        public bool renderBasedOnLodt = false;
        public bool renderBasedOnPipt = true;
        public bool discordRichPresence = true;
        public bool dontDrawInvisible = false;

        public bool persistentShinies = true;

        public bool LegacyAssetIDFormat = false;
        public bool LegacyAssetTypeFormat = false;

        public string pcsx2Path;
        public string[] recentBuildIsoGamePaths;
        public int flyModeCursor = 1;
        public bool translucentEditor = true;
        public bool showEditorsWhenLoadingProject = true;
    }
}
