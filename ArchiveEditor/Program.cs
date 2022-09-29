using System;
using System.Windows.Forms;

namespace ArchiveEditor
{
    static class Program
    {
        public static IndustrialPark.ArchiveEditor archiveEditor;

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            archiveEditor = IndustrialPark.ArchiveEditor.Standalone;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(archiveEditor);
        }
    }
}
