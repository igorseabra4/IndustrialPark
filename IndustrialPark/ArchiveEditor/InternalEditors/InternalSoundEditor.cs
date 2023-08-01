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
        }

        public void RefreshPropertyGrid()
        {
            try
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
            }
            SoundUtility.ClearSound();
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
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
                SoundUtility.ClearSound();
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
            byte[] file = SoundUtility.GenerateJawData(archive.GetSoundData(asset.assetID, asset.Data));

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

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            ArchiveEditorFunctions.OpenWikiPage(asset);
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            SoundUtility.PlaySound(asset, archive);
        }

        private void buttonImportSound_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select your audio file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] data = SoundUtility.ConvertSoundToDSP(openFileDialog.FileName);
                if (data == null)
                    return;

                try
                {
                    archive.AddSoundToSNDI(data, asset.assetID, asset.assetType, out byte[] soundData);
                    asset.Data = soundData;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                archive.UnsavedChanges = true;
                updateListView(asset);
                SoundUtility.ClearSound();
            }
        }
    }
}
