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
            labelAssetName.Text = asset.ToString();
        }
    }
}
