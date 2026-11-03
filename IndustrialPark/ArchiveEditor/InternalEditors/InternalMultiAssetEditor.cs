using IndustrialPark.Archive;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark;

public partial class InternalMultiAssetEditor : Form
{
    public InternalMultiAssetEditor(Asset[] assets)
    {
        InitializeComponent();

        this.assets = assets;

        var descriptors = assets.Select(DynamicTypeDescriptor.Create).ToArray();
        propertyGridAsset.SelectedObjects = descriptors;

        // Using PropertyGrid with multiple objects is broken with "Categorized" sorting after switching to .NET 10
        propertyGridAsset.PropertySort = PropertySort.CategorizedAlphabetical;

        labelAssetName.Text = string.Join(" | ", from Asset asset in assets select asset.assetName);

        InternalEditor.SelectFirstProperty(propertyGridAsset.SelectedGridItem);
    }

    private readonly Asset[] assets;
    public uint[] AssetIDs => (from Asset a in assets select a.assetID).ToArray();

    private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        foreach (var asset in assets)
            asset.InvokePropertyChanged(true);
        
        propertyGridAsset.Refresh();
    }

    public void RefreshPropertyGrid()
    {
        Invoke(propertyGridAsset.Refresh);
    }
}
