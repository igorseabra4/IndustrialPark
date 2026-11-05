using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public class AssetRepository
{
    private readonly Dictionary<uint, Asset> assetDictionary;

    public AssetRepository()
    {
        assetDictionary = new Dictionary<uint, Asset>();
    }

    public event Action<Asset> AssetAdded;

    public event Action<Asset> AssetRemoved;

    public IEnumerable<Asset> Assets => assetDictionary.Values;

    public IEnumerable<AssetType> AssetTypes => assetDictionary.Values.Select(a => a.assetType).ToHashSet();

    public bool Contains(uint assetID) => assetDictionary.ContainsKey(assetID);

    public bool Contains(Asset asset) => assetDictionary.ContainsValue(asset);

    public Asset Get(uint assetID)
    {
        if (assetDictionary.TryGetValue(assetID, out var asset))
            return asset;
        throw new KeyNotFoundException($"Asset [{assetID:X8}] not present in dictionary.");
    }

    public bool TryGet(uint key, out Asset asset) => assetDictionary.TryGetValue(key, out asset);

    public void Add(params IEnumerable<Asset> assets)
    {
        Remove(assets.Select(a => a.assetID));

        foreach (var asset in assets)
        {
            assetDictionary[asset.assetID] = asset;
            AssetAdded?.Invoke(asset);
        }
    }

    public void Remove(params IEnumerable<uint> assetIDs)
    {
        foreach (uint u in assetIDs)
            if (assetDictionary.Remove(u, out Asset asset))
                AssetRemoved?.Invoke(asset);
    }
    
    public bool ContainsType(AssetType assetType) => assetDictionary.Values.Any(a => a.assetType.Equals(assetType));

    public IEnumerable<Asset> FindWhoTargets(uint assetID) => Assets.Where(asset => asset.HasReference(assetID)).ToList();

    public void Dispose()
    {
        if (AssetRemoved != null)
            foreach (var asset in Assets)
                AssetRemoved.Invoke(asset);
    }
}
