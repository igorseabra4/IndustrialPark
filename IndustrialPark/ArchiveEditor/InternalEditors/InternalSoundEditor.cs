using HipHopFile;
using IndustrialPark.Archive;
using System;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalSoundEditor : InternalEditor
    {
        public InternalSoundEditor(AssetSound asset, ArchiveEditor ae) : base(asset, ae)
        {
            InitializeComponent();
            RefreshPropertyGrid();

            var archive = ae.archive;

            if (archive.Platform == Platform.Xbox || (archive.Platform == Platform.GameCube && archive.Game >= Game.Incredibles && asset.assetType == AssetType.SoundStream))
                compressCheckBox.Enabled = true;
            if (archive.Platform != Platform.PS2 || (archive.Platform == Platform.PS2 && asset.assetType == AssetType.SoundStream))
                checkBoxPS2Looping.Enabled = false;
            if (archive.Platform == Platform.Xbox || (archive.Platform == Platform.GameCube && archive.Game >= Game.Incredibles))
                forceMonoCheckBox.Enabled = true;
        }

        public override void RefreshPropertyGrid()
        {
            try
            {
                var sndi = archiveEditor.archive.TableManager.GetSNDI();
                soundSizeLabel.Text = ArchiveEditor.ConvertSize(asset.Data.Length);
                if (sndi == null)
                {
                    propertyGridSoundData.SelectedObject = null;
                    return;
                }
                switch (sndi)
                {
                    case AssetSNDI_PS2 ps2:
                        {
                            var entry = ps2.GetEntry(asset.assetID, asset.assetType);
                            propertyGridSoundData.SelectedObject = new SoundInfoPs2Wrapper(entry, asset.Data, asset.assetType == AssetType.SoundStream);
                            CalculateSoundLength(entry);
                        }
                        break;
                    case AssetSNDI_XBOX xbox:
                        {
                            var entry = xbox.GetEntry(asset.assetID, asset.assetType);
                            propertyGridSoundData.SelectedObject = new SoundInfoXboxWrapper(entry);
                            CalculateSoundLength(entry);
                        }
                        break;
                    case AssetSNDI_GCN_V1 gcn1:
                        {
                            var entry = gcn1.GetEntry(asset.assetID, asset.assetType);
                            propertyGridSoundData.SelectedObject = new SoundInfoGcn1Wrapper(entry);
                            CalculateSoundLength(entry);
                        }
                        break;
                    case AssetSNDI_GCN_V2 gcn2:
                        {
                            var entry = gcn2.GetEntry(asset.assetID);
                            soundSizeLabel.Text = ArchiveEditor.ConvertSize(entry.LengthCompressedBytes);
                            propertyGridSoundData.SelectedObject = entry.Clone();
                            CalculateSoundLength(entry);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            propertyGridSoundData.Refresh();
        }

        private void propertyGridSoundData_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGridSoundData.SelectedObject == null)
                return;
            var sndi = archiveEditor.archive.TableManager.GetSNDI();
            archiveEditor.BeginTransaction();
            if (sndi != null)
            {
                if (sndi is AssetSNDI_PS2 ps2)
                {
                    ps2.SetEntry(((SoundInfoPs2Wrapper)propertyGridSoundData.SelectedObject).Entry, asset.assetType);
                    asset.Data = ((SoundInfoPs2Wrapper)propertyGridSoundData.SelectedObject).SoundData;
                    sndi.InvokePropertyChanged(false);
                    asset.InvokePropertyChanged(false);
                }
                else if (sndi is AssetSNDI_XBOX xbox)
                {
                    xbox.SetEntry(((SoundInfoXboxWrapper)propertyGridSoundData.SelectedObject).Entry, asset.assetType);
                    xbox.InvokePropertyChanged(false);
                }
                else if (sndi is AssetSNDI_GCN_V1 gcn1)
                {
                    gcn1.SetEntry(((SoundInfoGcn1Wrapper)propertyGridSoundData.SelectedObject).Entry, asset.assetType);
                    gcn1.InvokePropertyChanged(false);
                }
                else if (sndi is AssetSNDI_GCN_V2 gcn2)
                {
                    gcn2.SetEntry((FSB3_SampleHeader)propertyGridSoundData.SelectedObject);
                    gcn2.InvokePropertyChanged(false);
                }
            }
            archiveEditor.EndTransaction();
            SoundUtility_vgmstream.ClearSound();
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            SoundUtility_vgmstream.StopSound();
        }

        private new AssetSound asset => (AssetSound)base.asset;

        private void buttonImportRaw_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your audio file (GameCube DSP, GameCube FSB3, XBOX PCM, PS2 VAG)"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] file = File.ReadAllBytes(openFileDialog.FileName);
                asset.Data = file;

                if (checkBoxSendToSNDI.Checked)
                {
                    try
                    {
                        archiveEditor.archive.SoundManager.AddToSoundInfo(asset);

                        var data = asset.Data;
                        if (checkBoxPS2Looping.Checked)
                            EntrySoundInfo_PS2.SetLoopingRange(ref data);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                archiveEditor.BeginTransaction();
                asset.InvokePropertyChanged(false);
                archiveEditor.archive.TableManager.GetSNDI().InvokePropertyChanged(false);
                archiveEditor.EndTransaction();

                SoundUtility_vgmstream.ClearSound();
                RefreshPropertyGrid();
            }
        }

        private void buttonExportRaw_Click(object sender, EventArgs e)
        {
            var platform = archiveEditor.archive.Platform;
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = asset.assetName + (
                (platform == Platform.GameCube && asset.game < Game.Incredibles) ? ".DSP" :
                (platform == Platform.Xbox) ? ".WAV" :
                (platform == Platform.PS2) ? ".VAG" :
                ""),
                Filter = "All files|*"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, archiveEditor.archive.GetSoundData(asset.assetID, asset.Data));
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = asset.assetName + ".wav",
                Filter = "WAV files|*.wav"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                SoundUtility_vgmstream.ExportToFile(asset, archiveEditor.archive, saveFileDialog.FileName);
        }

        private void buttonGenerateJawData_Click(object sender, EventArgs e)
        {
            ApplyJawData(SoundUtility_vgmstream.GenerateJawData(archiveEditor.archive.GetSoundData(asset.assetID, asset.Data), (float)numericUpDownJawMultiplier.Value));
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
            archiveEditor.BeginTransaction();
            AssetJAW JAW;
            try
            {
                JAW = archiveEditor.archive.TableManager.GetJAW(true);
                JAW.AddEntry(jawData, asset.assetID);

                asset.InvokePropertyChanged(false);
                JAW.InvokePropertyChanged(false);
                archiveEditor.EndTransaction();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            SoundUtility_vgmstream.PlaySound(asset, archiveEditor.archive);
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

                byte[] data = SoundService.CreateSoundFile(
                    archiveEditor.archive.Game,
                    archiveEditor.archive.Platform,
                    openFileDialog.FileName,
                    checkBoxPS2Looping.Checked,
                    compressCheckBox.Checked,
                    forceMonoCheckBox.Checked,
                    (int)samplerateNumeric.Value);

                if (data != null)
                {
                    try
                    {
                        archiveEditor.BeginTransaction();

                        asset.Data = data;
                        archiveEditor.archive.SoundManager.AddToSoundInfo(asset);

                        asset.InvokePropertyChanged(false);
                        archiveEditor.archive.TableManager.GetSNDI().InvokePropertyChanged(false);
                        archiveEditor.EndTransaction();
                        SoundUtility_vgmstream.ClearSound();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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
