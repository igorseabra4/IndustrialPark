using IndustrialPark;
using IndustrialPark.Randomizer;
using System;
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

            IndustrialPark.Program.MainForm = new MainForm(new IPSettings());

            RandomizerMenu menu = new RandomizerMenu();

            Application.Run(menu);
        }
    }
}
