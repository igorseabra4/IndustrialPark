using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalDynaEditor : Form, IInternalEditor
    {
        public InternalDynaEditor(AssetDYNA asset, ArchiveEditor archiveEditor)
        {
            InitializeComponent();
            TopMost = true;

            assetDYNA = asset;
            this.archiveEditor = archiveEditor;

            propertyGridAsset.SelectedObject = asset;
            propertyGridDynaType.SelectedObject = asset.DynaBase;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalDynaEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archiveEditor.RemoveInternalEditor(this);
        }

        private AssetDYNA assetDYNA;
        private ArchiveEditor archiveEditor;

        public uint GetAssetID()
        {
            return assetDYNA.AHDR.assetID;
        }

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
