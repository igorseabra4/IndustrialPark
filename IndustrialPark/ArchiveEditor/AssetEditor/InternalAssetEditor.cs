using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalAssetEditor : Form, IInternalEditor
    {
        public InternalAssetEditor(Asset asset, ArchiveEditor archiveEditor)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archiveEditor = archiveEditor;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archiveEditor.RemoveInternalEditor(this);
        }

        private Asset asset;
        private ArchiveEditor archiveEditor;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }
    }
}
