using IndustrialPark.Archive;
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

            Application.SetColorMode(SystemColorMode.System);

            menu = new RandomizerMenu();

            Application.Run(menu);
        }
    }
}
