using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IndustrialPark.Archive;

public class AssetSelectionManager
{
    public ObservableCollection<Asset> SelectedAssets { get; private set; }

    public IEnumerable<uint> SelectedAssetIDs => SelectedAssets.Select(a => a.assetID);

    public int SelectedAssetCount => SelectedAssets.Count;

    public AssetSelectionManager() 
    {
        SelectedAssets = new ObservableCollection<Asset>();
    }

    private void SetSelected(Asset asset)
    {
        asset.isSelected = true;
        if (!SelectedAssets.Contains(asset))
            SelectedAssets.Add(asset);
        if (asset is AssetRWTX)
            Program.TextureManager.SelectedTextures.Add(asset.assetName.Replace(".rw3", "", StringComparison.InvariantCultureIgnoreCase));
    }

    public void SetSelected(params IEnumerable<Asset> assets)
    {
        UnselectAll(except: assets.Select(a => a.assetID));
        foreach (var asset in assets)
            SetSelected(asset);
    }

    private void Unselect(Asset asset)
    {
        asset.isSelected = false;
        if (asset is AssetRWTX)
            Program.TextureManager.SelectedTextures.Remove(asset.assetName.Replace(".rw3", "", StringComparison.InvariantCultureIgnoreCase));
        SelectedAssets.Remove(asset);
    }

    public void Unselect(params IEnumerable<Asset> assets)
    {
        foreach (var asset in assets.Where(a => a.isSelected))
            Unselect(asset);
    }

    public void UnselectAll(IEnumerable<uint> except = null)
    {
        except = except?.ToHashSet();
        foreach (var asset in SelectedAssets.Where(a => except == null || !except.Contains(a.assetID)).ToList())
            Unselect(asset);
    }
}
