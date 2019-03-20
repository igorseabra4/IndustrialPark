using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalCamEditor : Form, IInternalEditor
    {
        public InternalCamEditor(AssetCAM asset, ArchiveEditorFunctions archive)
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

        private AssetCAM asset;
        private ArchiveEditorFunctions archive;

        private void buttonGetPos_Click(object sender, System.EventArgs e)
        {
            asset.SetPosition(Program.MainForm.renderer.Camera.Position);
        }

        private void buttonGetDir_Click(object sender, System.EventArgs e)
        {
            asset.SetNormalizedForward(Program.MainForm.renderer.Camera.GetForward());
            asset.SetNormalizedUp(Program.MainForm.renderer.Camera.GetUp());
            asset.SetNormalizedLeft(Program.MainForm.renderer.Camera.GetRight());
        }

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

        private void buttonHelp_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }
    }
}
