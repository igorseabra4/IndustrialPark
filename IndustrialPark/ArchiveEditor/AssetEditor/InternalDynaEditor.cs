using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalDynaEditor : Form
    {
        private AssetDYNA asset;

        public InternalDynaEditor(AssetDYNA asset)
        {
            InitializeComponent();
            TopMost = true;
            this.asset = asset;
            propertyGridAsset.SelectedObject = asset;
            propertyGridDynaType.SelectedObject = asset.DynaBase;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGridDynaType.SelectedObject = asset.DynaBase;
        }

        private void propertyGridDynaType_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            asset.DynaBase = (DynaBase)propertyGridDynaType.SelectedObject;
        }
    }
}
