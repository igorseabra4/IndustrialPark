using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalAssetEditor : Form
    {
        public InternalAssetEditor(Asset asset)
        {
            InitializeComponent();
            TopMost = true;
            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
