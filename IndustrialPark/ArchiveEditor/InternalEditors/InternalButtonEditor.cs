using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalButtonEditor : Form, IInternalEditor
    {
        public InternalButtonEditor(AssetBUTN asset, ArchiveEditorFunctions archive, bool hideHelp)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";

            propertyGrid_Motion.SelectedObject = asset.Motion;

            propertyGridAsset.HelpVisible = !hideHelp;
            propertyGrid_Motion.HelpVisible = !hideHelp;
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetBUTN asset;
        private ArchiveEditorFunctions archive;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            ArchiveEditorFunctions.UpdateGizmoPosition();
        }

        private void propertyGrid_Motion_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            asset.Motion = (Motion)propertyGrid_Motion.SelectedObject;
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }

        public void SetHideHelp(bool hideHelp)
        {
            propertyGridAsset.HelpVisible = !hideHelp;
            propertyGrid_Motion.HelpVisible = !hideHelp;
        }
    }
}
