using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class DynaSearch : Form
    {
        public DynaSearch()
        {
            InitializeComponent();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        private string rootDir;

        private void buttonChooseRoot_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
            {
                rootDir = openFile.FileName;
                labelRootDir.Text = "Root Directory: " + rootDir;
            }
        }
        
        private void buttonPerform_Click(object sender, EventArgs e)
        {
            List<string> fileList = new List<string>();
            AddFolder(rootDir, ref fileList);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = fileList.Count;
            progressBar1.Value = 0;
            progressBar1.Step = 1;

            Dictionary<(DynaType, int), int> dynas = new Dictionary<(DynaType, int), int>();

            foreach (string s in fileList)
            {
                progressBar1.PerformStep();

                ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                archive.OpenFile(s, false, Platform.Unknown, out _, true);
                Check(archive, ref dynas);
                archive.Dispose(false);
            }

            List<string> output = new List<string>();

            foreach (var v in dynas.Keys)
                output.Add($"{v.Item1} - v{v.Item2} - {dynas[v]} inst\n");

            richTextBox1.Clear();
            foreach (var s in output.OrderBy(f => f))
                richTextBox1.AppendText(s);
        }

        private void AddFolder(string folderPath, ref List<string> fileList)
        {
            foreach (string s in Directory.GetFiles(folderPath))
                if (Path.GetExtension(s).ToLower() == ".hip")
                    fileList.Add(s);
            foreach (string s in Directory.GetDirectories(folderPath))
                AddFolder(s, ref fileList);
        }
        
        private void Check(ArchiveEditorFunctions archive, ref Dictionary<(DynaType, int), int> dynas)
        {
            foreach (Asset asset in archive.GetAllAssets())
                if (asset is AssetDYNA dyna)
                    try
                    {
                        if (!dynas.ContainsKey((dyna.Type, dyna.Version)))
                            dynas.Add((dyna.Type, dyna.Version), 0);

                        dynas[(dyna.Type, dyna.Version)] = dynas[(dyna.Type, dyna.Version)] + 1;
                    }
                    catch
                    {

                    }
        }
    }
}
