using HipHopFile;
using IndustrialPark.Archives;
using IndustrialPark.Assets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.ArchiveEditors;

/// <summary>
/// This class provides a repository of asset names, which can be used to look up the name of an asset given its ID, application-wide.
/// </summary>
public class NameRepository
{
    private readonly Dictionary<uint, string> nameDictionary;

    public bool Persistent { get; set; } = true;

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
            Add(asset.assetID, asset.assetName);
            if (asset is IAssetWithModel iawm && asset.game >= Game.Incredibles)
                Add(new AssetID(iawm.fileNameWithoutExtension), iawm.fileNameWithoutExtension);
        });

        archive.Subscriber.SubscribeToAssetRemoved((asset, isDisposing) => {
            if (Persistent && isDisposing)
                return;
            Remove(asset.assetID);
            if (asset is IAssetWithModel iawm)
                Remove(new AssetID(iawm.fileNameWithoutExtension));
        });
    }

    public static string PathToNameRepository => Path.Combine(MainForm.PathToUserFolder, "name_repository.json");

    public void Save()
    {
        var serialized = JsonConvert.SerializeObject(nameDictionary, Formatting.Indented);
        File.WriteAllText(PathToNameRepository, serialized);
    }

    public void Load()
    {
        if (Persistent && File.Exists(PathToNameRepository))
        {
            var serialized = File.ReadAllText(PathToNameRepository);
            var deserialized = JsonConvert.DeserializeObject<Dictionary<uint, string>>(serialized);
            if (deserialized != null)
                lock (nameDictionary)
                    foreach (var kvp in deserialized.TakeLast(10000))
                        nameDictionary[kvp.Key] = kvp.Value;
        }
    }

    public void Reset()
    {
        nameDictionary.Clear();

        foreach (var asset in Program.ArchiveEditors.SelectMany(ae => ae.archive?.Assets ?? []))
            Add(asset.assetID, asset.assetName);
    }
}
