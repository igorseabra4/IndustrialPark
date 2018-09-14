using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalDynaEditor : Form
    {
        public InternalDynaEditor(AssetDYNA asset)
        {
            InitializeComponent();
            TopMost = true;

            assetDYNA = asset;

            propertyGridAsset.SelectedObject = asset;
            propertyGridDynaType.SelectedObject = asset.DynaBase;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private AssetDYNA assetDYNA;

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGridDynaType.SelectedObject = assetDYNA.DynaBase;
        }

        private void propertyGridDynaType_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            assetDYNA.DynaBase = (DynaBase)propertyGridDynaType.SelectedObject;
        }
    }
}
