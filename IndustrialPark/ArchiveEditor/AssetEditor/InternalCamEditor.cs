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
            asset.PositionX = Program.MainForm.renderer.Camera.Position.X;
            asset.PositionY = Program.MainForm.renderer.Camera.Position.Y;
            asset.PositionZ = Program.MainForm.renderer.Camera.Position.Z;
        }

        private void buttonGetDir_Click(object sender, System.EventArgs e)
        {
            asset.NormalizedForwardX = Program.MainForm.renderer.Camera.GetForward().X;
            asset.NormalizedForwardY = Program.MainForm.renderer.Camera.GetForward().Y;
            asset.NormalizedForwardZ = Program.MainForm.renderer.Camera.GetForward().Z;
            asset.NormalizedUpX = Program.MainForm.renderer.Camera.GetUp().X;
            asset.NormalizedUpY = Program.MainForm.renderer.Camera.GetUp().Y;
            asset.NormalizedUpZ = Program.MainForm.renderer.Camera.GetUp().Z;
        }

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }
    }
}
