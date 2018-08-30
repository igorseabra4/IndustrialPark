using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    static class Program
    {
        public static MainForm MainForm;
        public static ViewConfig ViewConfig;
        public static InternalAssetEditor InternalEditor;

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
            ViewConfig = new ViewConfig();
            InternalEditor = new InternalAssetEditor();

            Application.Run(MainForm);
        }
    }
}
