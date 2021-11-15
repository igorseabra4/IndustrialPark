using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalMultiAssetEditor : Form
    {
        public InternalMultiAssetEditor(Asset[] assets, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            assetIDs = (from Asset a in assets select a.assetID).ToArray();
            this.archive = archive;

            var typeDescriptors = new List<DynamicTypeDescriptor>();

            labelAssetName.Text = "";

            foreach (var asset in assets)
            {
                DynamicTypeDescriptor dt = new DynamicTypeDescriptor(asset.GetType());
                asset.SetDynamicProperties(dt);
                typeDescriptors.Add(dt.FromComponent(asset));
            }

            propertyGridAsset.SelectedObjects = typeDescriptors.ToArray();
            labelAssetName.Text = string.Join(" | ", from Asset asset in assets select asset.assetName);
        }

        private ArchiveEditorFunctions archive;
        public uint[] assetIDs { get; private set; }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            propertyGridAsset.Refresh();
        }
    }
}
