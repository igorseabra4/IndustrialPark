using HipHopFile;
using IndustrialPark.ArchiveEditors;
using IndustrialPark.Archives;
using IndustrialPark.Assets;
using System;
using System.Collections.Generic;

namespace IndustrialPark.Scripting;

public class ArchiveInterface
{
    private Archive Archive { get; set; }

    public List<AssetError> AssetErrors { get; set; }

    public ArchiveInterface(Game game, Platform platform)
    {
        Archive = new Archive(platform, game, DateTime.Now, DateTime.Now.ToString(), true, false, false);
    }

    public ArchiveInterface(string fileName, Platform platform = Platform.Unknown)
    {
        HipFile hipFile = HipFile.FromPath(fileName);

        while (hipFile.game == Game.Unknown)
            hipFile.game = ChooseGame.GetGame();

        if (hipFile.platform == Platform.Unknown && platform != Platform.Unknown)
            hipFile.platform = platform;
        while (hipFile.platform == Platform.Unknown)
            hipFile.platform = ChoosePlatformDialog.GetPlatform();

        Archive = new Archive(hipFile, fileName, false, out List<AssetError> errors);
        this.AssetErrors = errors;
    }

    public ArchiveInterface(Archive archive)
    {
        Archive = archive;
    }

    public void Save(string fileName = null, bool createBackup = false) => Archive.Save(fileName, createBackup);

    public Game Game => Archive.Game;
    public Platform Platform => Archive.Platform;
    public string CurrentFilePath => Archive.CurrentFilePath;
    public string LevelName => Archive.LevelName;
    public DateTime FileDate => Archive.FileDate;
    public string DateString => Archive.DateString;
    public bool ContainsAsset(uint assetID) => Archive.ContainsAsset(assetID);
    public bool ContainsAsset(Asset asset) => Archive.ContainsAsset(asset);
    public bool ContainsAssetWithType(AssetType assetType) => Archive.ContainsAssetWithType(assetType);
    public Asset GetAsset(uint key) => Archive.GetAsset(key);
    public T GetAsset<T>(uint key) where T : Asset => Archive.GetAsset<T>(key);
    public bool TryGetAsset<T>(uint key, out T asset) where T : Asset => Archive.TryGetAsset(key, out asset);
    public IEnumerable<Asset> Assets => Archive.Assets;
    public int AssetCount => Archive.AssetCount;
    public IEnumerable<AssetType> AssetTypes => Archive.AssetTypes;
    public IEnumerable<Asset> FindWhoTargets(uint assetID) => Archive.FindWhoTargets(assetID);
    public Asset AddAsset(Asset asset, int layerIndex = -1) => Archive.AddNewAsset(asset, layerIndex);
    public void RemoveAssets(IEnumerable<uint> assetIDs, bool removeSound = true) => Archive.RemoveAssets(assetIDs, removeSound);
    public void RemoveAsset(uint assetID, bool removeSound = true) => Archive.RemoveAsset(assetID, removeSound);
    public int LayerCount => Archive.LayerCount;
    public bool AutoLayers => Archive.AutoLayers;
    public string GetLayerName(int index) => Archive.GetLayerName(index);
    public LayerType GetLayerType(int index) => Archive.GetLayerTypeGeneric(index);
    public void SetLayerType(int index, LayerType type) => Archive.SetLayerType(index, type);
    public IEnumerable<uint> GetLayerAssetIDs(int layer) => Archive.GetLayerAssetIDs(layer);
    public int AddLayer(LayerType layerType = LayerType.DEFAULT, int index = -1) => Archive.AddLayer(layerType, index);
    public void RemoveLayer(int index) => Archive.RemoveLayer(index);
    public bool MoveLayerUp(int index) => Archive.MoveLayerUp(index);
    public bool MoveLayerDown(int index) => Archive.MoveLayerDown(index);
    public int GetLayerFromAssetID(uint assetID) => Archive.GetLayerFromAssetID(assetID);
    public bool RenameLayer(int index, string name) => Archive.RenameLayer(index, name);
    public IEnumerable<AssetType> AssetTypesOnLayer(int index) => Archive.AssetTypesOnLayer(index);
    public int GetOrCreateLayerOfType(LayerType layerType) => Archive.GetOrCreateLayerOfType(layerType);
}