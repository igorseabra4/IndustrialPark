using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalCamEditor : Form, IInternalEditor
    {
        public InternalCamEditor(AssetCAM asset, ArchiveEditor archiveEditor)
        {
            InitializeComponent();
            TopMost = true;

            assetCAM = asset;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalCamEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archiveEditor.RemoveInternalEditor(this);
        }

        private AssetCAM assetCAM;
        private ArchiveEditor archiveEditor;

        private void buttonGetPos_Click(object sender, System.EventArgs e)
        {
            assetCAM.PositionX = Program.MainForm.renderer.Camera.Position.X;
            assetCAM.PositionY = Program.MainForm.renderer.Camera.Position.Y;
            assetCAM.PositionZ = Program.MainForm.renderer.Camera.Position.Z;
        }

        private void buttonGetDir_Click(object sender, System.EventArgs e)
        {
            assetCAM.NormalizedForwardX = Program.MainForm.renderer.Camera.GetForward().X;
            assetCAM.NormalizedForwardY = Program.MainForm.renderer.Camera.GetForward().Y;
            assetCAM.NormalizedForwardZ = Program.MainForm.renderer.Camera.GetForward().Z;
            assetCAM.NormalizedUpX = Program.MainForm.renderer.Camera.GetUp().X;
            assetCAM.NormalizedUpY = Program.MainForm.renderer.Camera.GetUp().Y;
            assetCAM.NormalizedUpZ = Program.MainForm.renderer.Camera.GetUp().Z;
        }

        public uint GetAssetID()
        {
            return assetCAM.AHDR.assetID;
        }
    }
}
