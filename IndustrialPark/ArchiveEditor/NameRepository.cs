using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark.Archive;

public class NameRepository
{
    private readonly Dictionary<uint, string> nameDictionary;

    public NameRepository()
    {
        nameDictionary = new Dictionary<uint, string>();
    }

    public void Add(uint assetID, string name)
    {
        lock (nameDictionary)
            nameDictionary[assetID] = name;
    }

    public void Remove(uint assetID)
    {
        lock (nameDictionary)
            nameDictionary.Remove(assetID);
    }

    public string Get(uint assetID)
    {
        foreach (ArchiveEditor archiveEditor in Program.ArchiveEditors)
            if (archiveEditor.archive?.TryGetAsset(assetID, out Asset asset) ?? false)
                return asset.assetName;

        lock (nameDictionary)
            return nameDictionary.TryGetValue(assetID, out string value) ? value : $"0x{assetID:X8}";
    }

    public void Clear()
    {
        lock (nameDictionary)
            nameDictionary.Clear();
    }

    public bool AssetExists(uint assetID)
    {
        foreach (ArchiveEditor archiveEditor in Program.ArchiveEditors)
            if (archiveEditor.archive?.ContainsAsset(assetID) ?? false)
                return true;

        lock (nameDictionary)
            return nameDictionary.ContainsKey(assetID);
    }

    public void RegisterSubscriptions(Archive archive)
    {
        archive.Subscriber.SubscribeToAssetAdded((asset) => {
            if (asset is IAssetWithModel iawm && asset.game >= Game.Incredibles)
                Add(new AssetID(iawm.fileNameWithoutExtension), iawm.fileNameWithoutExtension);
        });

        archive.Subscriber.SubscribeToAssetRemoved((asset) => {
            if (asset is IAssetWithModel iawm)
                Remove(new AssetID(iawm.fileNameWithoutExtension));
        });
    }
}
