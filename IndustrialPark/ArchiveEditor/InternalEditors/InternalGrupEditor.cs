using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalGrupEditor : Form, IInternalEditor
    {
        public InternalGrupEditor(AssetGRUP asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalCamEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetGRUP asset;
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
            List<AssetID> items = new List<AssetID>();
            foreach (uint i in asset.GroupItems)
                items.Add(i);
            foreach (uint i in archive.GetCurrentlySelectedAssetIDs())
                if (!items.Contains(i))
                    items.Add(i);
            asset.GroupItems = items.ToArray();
        }

        private void buttonHelp_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }
    }
}
