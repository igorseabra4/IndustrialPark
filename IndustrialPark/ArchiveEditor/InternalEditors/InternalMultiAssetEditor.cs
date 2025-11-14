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

            propertyGridAsset.SelectedObjects = assets.Select(a => DynamicTypeDescriptor.Create(a)).ToArray();

            // Using PropertyGrid with multiple objects is broken with "Categorized" sorting after switching to .NET 10
            propertyGridAsset.PropertySort = PropertySort.CategorizedAlphabetical;

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
