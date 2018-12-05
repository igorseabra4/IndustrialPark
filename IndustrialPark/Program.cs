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

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm = new MainForm();

            if (!Directory.Exists(MainForm.userTemplatesFolder))
                Directory.CreateDirectory(MainForm.userTemplatesFolder);

            ViewConfig = new ViewConfig();
            AboutBox = new AboutBox();
            UserTemplateManager = new UserTemplateManager();
            EventSearch = new EventSearch();

            Application.Run(MainForm);
        }
    }
}
