using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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

            var descriptors = assets.Select(DynamicTypeDescriptor.Create).ToArray();
            propertyValues.Clear();
            foreach (var asset in assets)
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(asset))
                    propertyValues[(asset, property.Name)] = property.GetValue(asset);
            propertyGridAsset.SelectedObjects = descriptors;

            // Using PropertyGrid with multiple objects is broken with "Categorized" sorting after switching to .NET 10
            propertyGridAsset.PropertySort = PropertySort.CategorizedAlphabetical;

            labelAssetName.Text = string.Join(" | ", from Asset asset in assets select asset.assetName);
        }

        private ArchiveEditorFunctions archive;
        private readonly Action<Asset> updateListView;
        private readonly Asset[] assets;
        public uint[] AssetIDs => (from Asset a in assets select a.assetID).ToArray();

        private readonly Dictionary<(object Asset, string Property), object> propertyValues = new();

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            var propertyName = e.ChangedItem.PropertyDescriptor.Name;
            var actions = new List<IReversibleAction>();
            foreach (var asset in assets)
            {
                var key = (asset, propertyName);
                var oldValue = propertyValues[key];
                var newValue = asset.GetType().GetProperty(propertyName).GetValue(asset);

                actions.Add(new AssetPropertyChangedAction(archive, asset, IInternalEditor.GetPropertyPath(e.ChangedItem), oldValue, newValue));
                propertyValues[key] = newValue;
                updateListView(asset);
            }
            propertyGridAsset.Refresh();
            Program.UndoBuffer.AddAction(new MultiAction(actions));
        }

        public void RefreshPropertyGrid()
        {
            Task.Run(() =>
            {
                Invoke(propertyGridAsset.Refresh);
            });
        }
    }
}
