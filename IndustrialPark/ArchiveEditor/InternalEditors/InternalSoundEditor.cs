using HipHopFile;
using System;
using System.IO;
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

            if (archive.platform == Platform.Xbox || (archive.platform == Platform.GameCube && archive.game >= Game.Incredibles && asset.assetType == AssetType.SoundStream))
                compressCheckBox.Enabled = true;
            if (archive.platform != Platform.PS2)
                checkBoxPS2Looping.Enabled = false;
            if (archive.platform == Platform.Xbox || (archive.platform == Platform.GameCube && archive.game >= Game.Incredibles))
                forceMonoCheckBox.Enabled = true;
        }

        public void RefreshPropertyGrid()
        {
            try
            {
                var sndi = archive.GetSNDI();
                soundSizeLabel.Text = ArchiveEditor.ConvertSize(asset.Data.Length);
                if (sndi != null)
                {
                    if (sndi is AssetSNDI_PS2 ps2)
                    {
                        var entry = ps2.GetEntry(asset.assetID, asset.assetType);
                        propertyGridSoundData.SelectedObject = new SoundInfoPs2Wrapper(entry);
                        CalculateSoundLength(entry);
                        return;
                    }
                    if (sndi is AssetSNDI_XBOX xbox)
                    {
                        var entry = xbox.GetEntry(asset.assetID, asset.assetType);
                        propertyGridSoundData.SelectedObject = new SoundInfoXboxWrapper(entry);
                        CalculateSoundLength(entry);
                        return;
                    }
                    if (sndi is AssetSNDI_GCN_V1 gcn1)
                    {
                        var entry = gcn1.GetEntry(asset.assetID, asset.assetType);
                        propertyGridSoundData.SelectedObject = new SoundInfoGcn1Wrapper(entry);
                        CalculateSoundLength(entry);
                        return;
                    }
                    if (sndi is AssetSNDI_GCN_V2 gcn2)
                    {
                        var entry = gcn2.GetEntry(asset.assetID);
                        soundSizeLabel.Text = ArchiveEditor.ConvertSize(entry.LengthCompressedBytes);
                        propertyGridSoundData.SelectedObject = entry.Clone();
                        CalculateSoundLength(entry);
                        return;

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
                    ps2.SetEntry(((SoundInfoPs2Wrapper)propertyGridSoundData.SelectedObject).Entry, asset.assetType);
                    archive.UnsavedChanges = true;
                }
                else if (sndi is AssetSNDI_XBOX xbox)
                {
                    xbox.SetEntry(((SoundInfoXboxWrapper)propertyGridSoundData.SelectedObject).Entry, asset.assetType);
                    archive.UnsavedChanges = true;
                }
                else if (sndi is AssetSNDI_GCN_V1 gcn1)
                {
                    gcn1.SetEntry(((SoundInfoGcn1Wrapper)propertyGridSoundData.SelectedObject).Entry, asset.assetType);
                    archive.UnsavedChanges = true;
                }
                else if (sndi is AssetSNDI_GCN_V2 gcn2)
                {
                    gcn2.SetEntry((FSB3_SampleHeader)propertyGridSoundData.SelectedObject);
                    archive.UnsavedChanges = true;
                }
            }
            SoundUtility_vgmstream.ClearSound();
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            SoundUtility_vgmstream.StopSound();
            archive.CloseInternalEditor(this);
        }

        private readonly AssetSound asset;
        private readonly ArchiveEditorFunctions archive;
        private readonly Action<Asset> updateListView;

        public uint GetAssetID()
        {
            return asset.assetID;
        }

        private void buttonImportRaw_Click(object sender, EventArgs e)
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
                SoundUtility_vgmstream.ClearSound();
                RefreshPropertyGrid();
            }
        }

        private void buttonExportRaw_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = asset.assetName + (
                (archive.platform == Platform.GameCube && asset.game < Game.Incredibles) ? ".DSP" :
                (archive.platform == Platform.Xbox) ? ".WAV" :
                (archive.platform == Platform.PS2) ? ".VAG" :
                ""),
                Filter = "All files|*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, archive.GetSoundData(asset.assetID, asset.Data));
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = asset.assetName + ".wav",
                Filter = "WAV files|*.wav"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                SoundUtility_vgmstream.ExportToFile(asset, archive, saveFileDialog.FileName);
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void buttonGenerateJawData_Click(object sender, EventArgs e)
        {
            ApplyJawData(SoundUtility_vgmstream.GenerateJawData(archive.GetSoundData(asset.assetID, asset.Data), (float)numericUpDownJawMultiplier.Value));
        }

        private void buttonImportJawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your JAW data file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
                ApplyJawData(File.ReadAllBytes(openFileDialog.FileName));
        }

        private void ApplyJawData(byte[] jawData)
        {
            try
            {
                var JAW = archive.GetJAW(true);
                JAW.AddEntry(jawData, asset.assetID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            archive.UnsavedChanges = true;
            updateListView(asset);
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            ArchiveEditorFunctions.OpenWikiPage(asset);
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            SoundUtility_vgmstream.PlaySound(asset, archive);
        }

        private void buttonImportSound_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your audio file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                importSoundStatusLabel.Text = "Import in progress...";
                soundLengthLabel.Visible = false;
                soundSizeLabel.Visible = false;
                Enabled = false;

                byte[] data = archive.CreateSoundFile(
                    openFileDialog.FileName, 
                    checkBoxPS2Looping.Checked,
                    compressCheckBox.Checked,
                    forceMonoCheckBox.Checked,
                    (int)samplerateNumeric.Value);

                if (data != null)
                {
                    try
                    {
                        archive.AddSoundToSNDI(data, asset.assetID, asset.assetType, out byte[] soundData);
                        asset.Data = soundData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                        
                    archive.UnsavedChanges = true;
                    updateListView(asset);
                    SoundUtility_vgmstream.ClearSound();
                }

                Enabled = true;
                RefreshPropertyGrid();
                importSoundStatusLabel.Text = "";
                soundLengthLabel.Visible = true;
                soundSizeLabel.Visible = true;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            SoundUtility_vgmstream.StopSound();
        }

        public static void VagHeaderToLittleEndian(ref byte[] data)
        {
            if (BitConverter.ToInt32(data, 4) > 32)
            {
                Array.Reverse(data, 4, 4);
                Array.Reverse(data, 12, 4);
                Array.Reverse(data, 16, 4);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            samplerateNumeric.Enabled = forceSampleRateCheckbox.Checked;
        }

        private void CalculateSoundLength(EntrySoundInfo_XBOX entry)
        {
            SetSoundLengthLabel((float)entry.dataSize / entry.nAvgBytesPerSec);
        }
        private void CalculateSoundLength(EntrySoundInfo_PS2 entry)
        {
            SetSoundLengthLabel((float)((entry.DataSize / 0x10) * 28) / entry.SampleRate);
        }

        private void CalculateSoundLength(EntrySoundInfo_GCN_V1 entry)
        {
            SetSoundLengthLabel((float)entry.num_samples / entry.sample_rate);
        }

        private void CalculateSoundLength(FSB3_SampleHeader entry)
        {
            SetSoundLengthLabel((float)entry.LengthSamples / entry.Frequency);
        }

        private void SetSoundLengthLabel(float duration)
        {
            TimeSpan ts = TimeSpan.FromSeconds(duration);
            soundLengthLabel.Text = $"Length: {ts:mm\\:ss\\.fff}";
        }

    }
}
