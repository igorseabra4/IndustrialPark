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
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

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
            public int count;
            public HashSet<string> names;
            public HashSet<string> values;
            public HashSet<string> flags;
            public HashSet<string> files;
            public uint model;
            public PickupReference pickRefId { get; private set; }
            public string modelName;

            public PickupTypeResult(uint pickRefId, uint model)
            {
                this.pickRefId = (PickupReference)pickRefId;
                this.model = model;

                count = 0;
                names = new HashSet<string>();
                values = new HashSet<string>();
                flags = new HashSet<string>();
                files = new HashSet<string>();
                modelName = "";
            }

            public override string ToString()
            {
                var pick = pickRefId.ToString();
                if (!Enum.IsDefined(pickRefId))
                    pick = ((uint)pickRefId).ToString("X8");
                return $"[{pick}], M: [{modelName}], C: {count}, F: [{string.Join(", ", flags.OrderBy(f => f))}], V: [{string.Join(", ", values.OrderBy(f => f))}], FI: [{string.Join(", ", files.OrderBy(f => f))}], N: [{string.Join(", ", names.OrderBy(f => f))}]";
            }
        }

        private Dictionary<uint, uint> pickEntries;

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

            ArchiveEditorFunctions.errorDisplay = false;

            pickEntries = [];
            foreach ((uint a, uint b) in AssetPICK.pickEntries)
                pickEntries[a] = b;

            foreach (string s in fileList)
            {
                progressBar1.PerformStep();

                ArchiveEditorFunctions archive = new ArchiveEditorFunctions(null) { SkipTextureDisplay = true };
                archive.OpenFile(s, false, scoobyPlatform);
                scoobyPlatform = archive.platform;
                Check(archive, result, pickEntries);
                archive.Dispose(false);
            }

            ArchiveEditorFunctions.errorDisplay = true;

            List<string> output = new List<string>();

            foreach (var kvp in result)
                output.Add(kvp.Value.ToString() + "\n");

            richTextBox1.Clear();
            foreach (var s in output.OrderBy(f => f))
                richTextBox1.AppendText(s);
            foreach (var kvp in result)
                richTextBox1.AppendText($"\"{kvp.Value.modelName}\" = 0x{(uint)kvp.Value.pickRefId:X8},\n");
        }

        private void AddFolder(string folderPath, ref List<string> fileList)
        {
            foreach (string s in Directory.GetFiles(folderPath))
                if (Path.GetExtension(s).ToLower() == ".hip")
                    fileList.Add(s);
            foreach (string s in Directory.GetDirectories(folderPath))
                AddFolder(s, ref fileList);
        }

        private void Check(ArchiveEditorFunctions archive, Dictionary<uint, PickupTypeResult> result, Dictionary<uint, uint> pickEntries)
        {
            foreach (AssetPKUP pickup in archive.GetAllAssets().OfType<AssetPKUP>())
            {
                if (!result.ContainsKey(pickup.PickReferenceID))
                    result[pickup.PickReferenceID] = new PickupTypeResult(pickup.PickReferenceID, pickEntries[pickup.PickReferenceID]);

                PickupTypeResult ptr = result[pickup.PickReferenceID];

                ptr.count++;
                ptr.names.Add(pickup.assetName);
                ptr.values.Add(pickup.PickupValue.ToString());
                ptr.flags.Add(pickup.PickupFlags.ToString());
                ptr.files.Add(Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath));

                if (archive.TryGetAsset(ptr.model, out Asset asset) && asset is AssetMODL model)
                    ptr.modelName = model.assetName;
            }
        }
    }
}
