using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using IndustrialPark.SaveFile;

namespace IndustrialPark
{
    static class Program
    {
        public static MainForm MainForm;
        public static ViewConfig ViewConfig;
        public static AboutBox AboutBox;
        public static UserTemplateManager UserTemplateManager;
        public static HansMainForm HansMainForm;

        public static EventSearch EventSearch;
        public static DynaSearch DynaSearch;
        public static AssetIDGenerator AssetIDGenerator;
        public static PickupSearch PickupSearch;

        public static UndoBuffer UndoBuffer;
        public static BuildISO BuildISO;

        public static SharpRenderer Renderer => MainForm != null && MainForm.renderer != null ? MainForm.renderer : null;
        
        [STAThread]
        static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!SharpDevice.IsDirectX11Supported())
            {
                MessageBox.Show("DirectX11 feature level 11.0 is required to run Industrial Park. Maximum supported feature level is " + SharpDevice.GetSupportedFeatureLevel().ToString() + ". Please update your DirectX.");
                return;
            }

            Application.SetDefaultFont(new System.Drawing.Font(new FontFamily("Microsoft Sans Serif"), 8.25f));

            IPSettings ipSettings = null;
            try
            {
                ipSettings = File.Exists(MainForm.pathToSettings) ? JsonConvert.DeserializeObject<IPSettings>(File.ReadAllText(MainForm.pathToSettings)) : null;
            }
            catch { }

            UndoBuffer = new UndoBuffer();

            Application.SetColorMode(ipSettings != null ? ipSettings.ColorMode : SystemColorMode.System);

            MainForm = new MainForm(ipSettings);

            if (!Directory.Exists(MainForm.userTemplatesFolder))
                Directory.CreateDirectory(MainForm.userTemplatesFolder);

            ViewConfig = new ViewConfig();
            AboutBox = new AboutBox();
            UserTemplateManager = new UserTemplateManager();

            Application.Run(MainForm);
        }
    }
}
