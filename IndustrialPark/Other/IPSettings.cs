namespace IndustrialPark
{
    public class IPSettings
    {
        public bool AutosaveOnClose;
        public bool AutoloadOnStartup;
        public string LastProjectPath;
        public bool CheckForUpdatesOnStartup;

        public bool renderBasedOnLodt = false;
        public bool renderBasedOnPipt = true;
        public bool dontDrawInvisible = false;

        public bool updateReferencesOnCopy = true;
        public bool persistentShinies = true;
        public bool hideHelp = false;
    }
}
