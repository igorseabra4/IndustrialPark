using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Windows.Forms;

namespace IndustrialPark.Randomizer
{
    public static class Program
    {
        public static RandomizerMenu menu;

        [ModuleInitializer]
        internal static void InitializeAssemblyResolver()
        {
            AssemblyLoadContext.Default.Resolving += ResolveDependencyAssembly;
        }

        private static Assembly ResolveDependencyAssembly(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            string assemblyPath = Path.Combine(AppContext.BaseDirectory, "Library", assemblyName.Name + ".dll");
            return File.Exists(assemblyPath) ? context.LoadFromAssemblyPath(assemblyPath) : null;
        }

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
