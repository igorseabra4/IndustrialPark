using System;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    static class Program
    {
        public static MainForm MainForm;
        public static ViewConfig ViewConfig;
        public static AboutBox AboutBox;
        public static UserTemplateManager UserTemplateManager;

        public static EventSearch EventSearch;
        public static AssetIDGenerator AssetIDGenerator;

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

            MainForm = new MainForm();

            if (!Directory.Exists(MainForm.userTemplatesFolder))
                Directory.CreateDirectory(MainForm.userTemplatesFolder);

            ViewConfig = new ViewConfig();
            AboutBox = new AboutBox();
            UserTemplateManager = new UserTemplateManager();

            EventSearch = new EventSearch();
            AssetIDGenerator = new AssetIDGenerator();

            Application.Run(MainForm);
        }
    }
}
