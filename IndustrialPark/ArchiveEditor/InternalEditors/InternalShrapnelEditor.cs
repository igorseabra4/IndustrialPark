using HipHopFile;
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
            if (asset.game == Game.Incredibles)
                list.Add(new EntrySHRP_Type3_TSSM(asset.platform));
            else
                list.Add(new EntrySHRP_Type3_BFBB(asset.platform));
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd4_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            if (asset.game == Game.Incredibles)
                list.Add(new EntrySHRP_Type4_TSSM(asset.platform));
            else
                list.Add(new EntrySHRP_Type4_BFBB(asset.platform));
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd5_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            if (asset.game == Game.Incredibles)
                list.Add(new EntrySHRP_Type5_TSSM(asset.platform));
            else
                list.Add(new EntrySHRP_Type5_BFBB(asset.platform));
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd6_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            if (asset.game == Game.Incredibles)
                list.Add(new EntrySHRP_Type6_TSSM(asset.platform));
            else
                list.Add(new EntrySHRP_Type6_BFBB(asset.platform));
            asset.SHRPEntries = list.ToArray();


            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd8_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            list.Add(new EntrySHRP_Type8(asset.platform));
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
        }

        private void buttonAdd9_Click(object sender, System.EventArgs e)
        {
            List<EntrySHRP> list = asset.SHRPEntries.ToList();
            list.Add(new EntrySHRP_Type9(asset.platform));
            asset.SHRPEntries = list.ToArray();

            propertyGridAsset.SelectedObject = asset;
            archive.UnsavedChanges = true;
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
