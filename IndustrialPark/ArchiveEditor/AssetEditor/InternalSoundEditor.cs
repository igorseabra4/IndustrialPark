using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalSoundEditor : Form, IInternalEditor
    {
        public InternalSoundEditor(Asset asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private Asset asset;
        private ArchiveEditorFunctions archive;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your audio file (GameCube DSP, XBOX PCM, PS2 VAG)"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] file = File.ReadAllBytes(openFileDialog.FileName);
                if (checkBoxSendToSNDI.Checked)
                {
                    try
                    {
                        archive.AddSoundToSNDI(file, asset.AHDR.assetID, asset.AHDR.assetType, out byte[] soundData);
                        asset.AHDR.data = soundData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    asset.AHDR.data = file;
                }
                archive.UnsavedChanges = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = asset.AHDR.ADBG.assetName + (
                (HipHopFile.Functions.currentPlatform == HipHopFile.Platform.GameCube) ? ".DSP" :
                (HipHopFile.Functions.currentPlatform == HipHopFile.Platform.Xbox) ? ".WAV" :
                (HipHopFile.Functions.currentPlatform == HipHopFile.Platform.PS2) ? ".VAG" :
                ""),
                Filter = "All files|*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<byte> file = new List<byte>();
                file.AddRange(archive.GetHeaderFromSNDI(asset.AHDR.assetID));
                file.AddRange(asset.AHDR.data);

                if (new string(new char[] { (char)file[0], (char)file[1], (char)file[2], (char)file[3] }) == "RIFF")
                {
                    byte[] chunkSizeArr = BitConverter.GetBytes(file.Count - 8);

                    file[4] = chunkSizeArr[0];
                    file[5] = chunkSizeArr[1];
                    file[6] = chunkSizeArr[2];
                    file[7] = chunkSizeArr[3];
                }

                File.WriteAllBytes(saveFileDialog.FileName, file.ToArray());
            }
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }
    }
}
