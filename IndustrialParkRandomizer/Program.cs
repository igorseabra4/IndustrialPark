using System;
using System.Windows.Forms;

namespace IndustrialPark.Randomizer
{
    public static class Program
    {
        public static RandomizerMenu menu;

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new SharpRenderer();
            IndustrialPark.Program.UndoBuffer = new UndoBuffer();
            IndustrialPark.Program.MainForm = new MainForm(new IPSettings());
            ArchiveEditorFunctions.errorDisplay = false;
            menu = new RandomizerMenu();

            Application.Run(menu);
        }
    }
}
