using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalShrapnelEditor : Form, IInternalEditor
    {
        public InternalShrapnelEditor(AssetSHRP asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetSHRP asset;
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

        private void buttonAdd3_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            list.Add(new EntrySHRP_Type3());
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd4_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            list.Add(new EntrySHRP_Type4());
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd5_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            list.Add(new EntrySHRP_Type5());
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd6_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            list.Add(new EntrySHRP_Type6());
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }
    }
}
