using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalMultiAssetEditor : Form
    {
        public InternalMultiAssetEditor(Asset[] assets, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();
            TopMost = true;

            this.assets = assets; 
            this.archive = archive;
            this.updateListView = updateListView;

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
        private readonly Action<Asset> updateListView;
        private readonly Asset[] assets;
        public uint[] AssetIDs => (from Asset a in assets select a.assetID).ToArray();

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            foreach (var a in assets)
                updateListView(a);
            propertyGridAsset.Refresh();
        }
    }
}
