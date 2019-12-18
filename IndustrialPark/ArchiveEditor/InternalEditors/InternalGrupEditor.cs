using IndustrialPark.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalGrupEditor : Form, IInternalEditor
    {
        public InternalGrupEditor(Asset asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
            if (asset is AssetANIM)
                buttonAddSelected.Text = "Import";
        }

        private void InternalCamEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private Asset asset;
        private ArchiveEditorFunctions archive;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
        }

        private void buttonAddSelected_Click(object sender, System.EventArgs e)
        {
            if (asset is AssetGRUP assetGRUP)
            {
                List<AssetID> items = new List<AssetID>();
                foreach (uint i in assetGRUP.GroupItems)
                    items.Add(i);
                foreach (uint i in archive.GetCurrentlySelectedAssetIDs())
                    if (!items.Contains(i))
                        items.Add(i);
                assetGRUP.GroupItems = items.ToArray();

                archive.UnsavedChanges = true;
            }
            else
            {
                using (OpenFileDialog openFile = new OpenFileDialog()
                {
                    Filter = Model_IO_Assimp.GetImportFilter()
                })
                    if (openFile.ShowDialog() == DialogResult.OK)
                    {
                        asset.Data = new Animation_IO_Assimp(EndianConverter.PlatformEndianness(asset.platform)).CreateANIMFromAssimp(openFile.FileName);
                        archive.UnsavedChanges = true;
                    }
            }
        }

        private void buttonHelp_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }

        public void SetHideHelp(bool _)
        {
        }
    }
}
