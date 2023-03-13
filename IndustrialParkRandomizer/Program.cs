using IndustrialPark.Randomizer;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace IndustrialParkRandomizer
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new IndustrialPark.SharpRenderer();

            RandomizerMenu menu = new RandomizerMenu();

            Application.Run(menu);
        }
    }
}
