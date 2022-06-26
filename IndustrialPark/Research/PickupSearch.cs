using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class PickupSearch : Form
    {
        public PickupSearch()
        {
            InitializeComponent();
            richTextBox1.WordWrap = false;
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

        public class PickupTypeResult
        {
            public HashSet<string> names;
            public HashSet<string> values;
            public HashSet<string> flags;
            public HashSet<string> files;
            public uint model;
            public string modelName;

            public PickupTypeResult()
            {
                names = new HashSet<string>();
                values = new HashSet<string>();
                flags = new HashSet<string>();
                files = new HashSet<string>();
                modelName = "";
            }

            public override string ToString()
            {
                return $"{modelName}, F: [{string.Join(", ", flags.OrderBy(f => f))}], V: [{string.Join(", ", values.OrderBy(f => f))}], N: [{string.Join(", ", names.OrderBy(f => f))}], FI: [{string.Join(", ", files.OrderBy(f => f))}]";
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

            var result = new Dictionary<uint, PickupTypeResult>();

            var scoobyPlatform = Platform.Unknown;

            foreach (string s in fileList)
            {
                progressBar1.PerformStep();

                ArchiveEditorFunctions archive = new ArchiveEditorFunctions();
                archive.OpenFile(s, false, scoobyPlatform, out _, true);
                scoobyPlatform = archive.platform;
                Check(archive, ref result);
                archive.Dispose(false);
            }

            List<string> output = new List<string>();

            foreach (var kvp in result.OrderBy(kvp => kvp.Value.names.FirstOrDefault()))
                output.Add($"0x{kvp.Key:X8} - v{kvp.Value}\n");

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

        private void Check(ArchiveEditorFunctions archive, ref Dictionary<uint, PickupTypeResult> result)
        {
            foreach (Asset asset in archive.GetAllAssets())
                if (asset is AssetPKUP pickup)
                    try
                    {
                        if (!result.ContainsKey(pickup.PickReferenceID))
                            result[pickup.PickReferenceID] = new PickupTypeResult();

                        var ptr = result[pickup.PickReferenceID];

                        ptr.names.Add(pickup.assetName);
                        ptr.values.Add(pickup.PickupValue.ToString());
                        ptr.flags.Add(pickup.PickupFlags.ToString());
                        ptr.files.Add(Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath));
                        ptr.model = AssetPICK.pickEntries[pickup.PickReferenceID];
                        var nmm = HexUIntTypeConverter.StringFromAssetID(AssetPICK.pickEntries[pickup.PickReferenceID]);
                        if (!string.IsNullOrEmpty(nmm))
                            ptr.modelName = nmm;
                    }
                    catch
                    {

                    }
        }
    }
}
