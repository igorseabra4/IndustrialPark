using HipHopFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalSoundEditor : Form, IInternalEditor
    {
        public InternalSoundEditor(AssetSound asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;
            this.updateListView = updateListView;

            Text = $"[{asset.assetType}] {asset}";

            RefreshPropertyGrid();
        }

        public void RefreshPropertyGrid()
        {
            var sndi = archive.GetSNDI();
            if (sndi != null)
            {
                if (sndi is AssetSNDI_PS2 ps2)
                {
                    var header = ps2.GetHeader(asset.assetID, asset.assetType);
                    propertyGridSoundData.SelectedObject = new SoundInfoPs2Wrapper(header);
                    return;
                }
                if (sndi is AssetSNDI_XBOX xbox)
                {
                    var entry = xbox.GetEntry(asset.assetID, asset.assetType);
                    propertyGridSoundData.SelectedObject = new SoundInfoXboxWrapper(entry);
                    return;
                }
                if (sndi is AssetSNDI_GCN_V1 gcn1)
                {
                    var header = gcn1.GetHeader(asset.assetID, asset.assetType);
                    propertyGridSoundData.SelectedObject = new SoundInfoGcn1Wrapper(header);
                    return;
                }
            }
            propertyGridSoundData.SelectedObject = null;
        }

        private void propertyGridSoundData_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridSoundData.SelectedObject == null)
                return;
            var sndi = archive.GetSNDI();
            if (sndi != null)
            {
                if (sndi is AssetSNDI_PS2 ps2)
                {
                    ps2.AddEntry(((SoundInfoPs2Wrapper)propertyGridSoundData.SelectedObject).ToByteArray(), GetAssetID(), asset.assetType, out _);
                    archive.UnsavedChanges = true;
                }
                else if (sndi is AssetSNDI_XBOX xbox)
                {
                    xbox.SetEntry(((SoundInfoXboxWrapper)propertyGridSoundData.SelectedObject).Entry, asset.assetType);
                    archive.UnsavedChanges = true;
                }
                else if (sndi is AssetSNDI_GCN_V1 gcn1)
                {
                    gcn1.AddEntry(((SoundInfoGcn1Wrapper)propertyGridSoundData.SelectedObject).ToByteArray(), GetAssetID(), asset.assetType, out _);
                    archive.UnsavedChanges = true;
                }
                soundData = null;
                converterInitialized = false;
            }
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (player != null)
                player.Dispose();

            archive.CloseInternalEditor(this);
        }

        private readonly AssetSound asset;
        private readonly ArchiveEditorFunctions archive;
        private readonly Action<Asset> updateListView;

        public uint GetAssetID()
        {
            return asset.assetID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your audio file (GameCube DSP, GameCube FSB3, XBOX PCM, PS2 VAG)"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] file = File.ReadAllBytes(openFileDialog.FileName);
                if (checkBoxSendToSNDI.Checked)
                {
                    try
                    {
                        archive.AddSoundToSNDI(file, asset.assetID, asset.assetType, out byte[] soundData);
                        asset.Data = soundData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    asset.Data = file;
                }
                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = asset.assetName + (
                (archive.platform == Platform.GameCube && asset.game != Game.Incredibles) ? ".DSP" :
                (archive.platform == Platform.Xbox) ? ".WAV" :
                (archive.platform == Platform.PS2) ? ".VAG" :
                ""),
                Filter = "All files|*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, archive.GetSoundData(asset.assetID, asset.Data));
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void buttonImportJawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your raw data file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] file = File.ReadAllBytes(openFileDialog.FileName);

                try
                {
                    archive.AddJawDataToJAW(file, asset.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                archive.UnsavedChanges = true;
                updateListView(asset);
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            Process.Start(AboutBox.WikiLink + asset.assetType.ToString());
        }

        byte[] soundData;
        SoundPlayer player;

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            try
            {
                if (soundData == null)
                    if (!InitSound())
                    {
                        MessageBox.Show("Unable to play sound.");
                        return;
                    }
                player.Play();
            }
            catch (Exception ex)
            {
                soundData = null;
                converterInitialized = false;
                MessageBox.Show("Unable to play sound: " + ex);
            }
        }

        private static bool converterInitialized = false;
        private static string vgmstreamFolder => Path.Combine(Path.Combine(Application.StartupPath, "Resources"), "vgmstream");
        private static string vgmstreamPath => Path.Combine(vgmstreamFolder, "test.exe");
        private static string inPath => vgmstreamFolder + "/test_sound_in";
        private static string outPath => vgmstreamFolder + "/test_sound_out.wav";

        private bool InitSound()
        {
            if (!converterInitialized)
                if (!initConverter())
                    return false;

            soundData = archive.GetSoundData(asset.assetID, asset.Data);

            File.WriteAllBytes(inPath, soundData);
            Convert(inPath, outPath);

            player = new SoundPlayer(new MemoryStream(File.ReadAllBytes(outPath)));

            File.Delete(inPath);
            File.Delete(outPath);

            return true;
        }

        private static Process vgmsProcess;

        public static bool initConverter()
        {
            if (!File.Exists(vgmstreamPath))
            {
                var result = MessageBox.Show("vgmstream not found under /Resources/vgmstream. Do you wish to download it?", "vgmstream not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                    AutomaticUpdater.DownloadVgmstream();
                else
                    return false;
            }

            vgmsProcess = new Process();
            vgmsProcess.StartInfo.FileName = vgmstreamPath;
            vgmsProcess.StartInfo.CreateNoWindow = true;
            vgmsProcess.StartInfo.WorkingDirectory = vgmstreamFolder;
            vgmsProcess.StartInfo.RedirectStandardOutput = true;
            vgmsProcess.StartInfo.RedirectStandardError = true;
            vgmsProcess.StartInfo.UseShellExecute = false;
            vgmsProcess.EnableRaisingEvents = true;
            converterInitialized = true;
            return true;
        }

        public static void Convert(string inPath, string outPath,
                            double loopCount = -1, double fadeTime = -1,
                            double fadeDelay = -1, bool ignoreLooping = false)
        {
            List<string> args = new List<string>();

            if (loopCount >= 0)
                args.Add("-l " + loopCount);
            if (fadeTime >= 0)
                args.Add("-f " + fadeTime);
            if (fadeDelay >= 0)
                args.Add("-d " + fadeDelay);
            if (ignoreLooping)
                args.Add("-i");

            args.Add(string.Format("-o \"{0}\"", outPath));
            args.Add(string.Format("\"{0}\"", inPath));

            string argString = String.Join(" ", args.ToArray<string>());

            vgmsProcess.StartInfo.Arguments = argString;
            vgmsProcess.Start();
            vgmsProcess.WaitForExit();
        }
    }
}
