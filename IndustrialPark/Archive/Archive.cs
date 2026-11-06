using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.Archive;

public class Archive
{
    protected readonly ArchiveSession session;
    protected readonly AssetRepository assetRepository;
    protected readonly LayerRepository layerRepository;

    public ArchiveSubscriber Subscriber => new ArchiveSubscriber(this, session, assetRepository, layerRepository);
    public ArchiveTransformer Transformer => new ArchiveTransformer(session, assetRepository, layerRepository);
    public SoundManager SoundManager => new SoundManager(session, assetRepository, layerRepository);
    public TableAssetManager TableManager => new TableAssetManager(session, assetRepository, layerRepository);

    public Game Game => session.Game;

    public Platform Platform => session.Platform;

    public string CurrentFilePath => session.CurrentFilePath;

    public string LevelName => session.LevelName;

    // New archive
    public Archive(
        Platform platform,
        Game game,
        DateTime fileDate,
        string dateString,
        bool autoLayers,
        bool addDefaultAssets)
    {
        session = new ArchiveSession(platform, game, fileDate, dateString);
        assetRepository = new AssetRepository();
        layerRepository = new LayerRepository(autoLayers);

        Subscriber.RegisterPreAssetsSubscriptions();

        if (addDefaultAssets)
        {
            (var layers, var assets) = AssetFactory.GenerateDefaultAssets(session.Game, session.Platform);
            layerRepository.Add(layers);
            assetRepository.Add(assets);
        }
    }

    // Open from path
    public Archive(
        HipFile hipFile,
        string fileName,
        bool subscriptions,
        out List<AssetError> errors,
        IProgressBar progressBar = null)
    {
        session = new ArchiveSession(hipFile.platform, hipFile.game, hipFile.PACK, fileName);
        layerRepository = new LayerRepository(hipFile.DICT.LTOC.LHDRList, hipFile.game, hipFile.HIPB.GetLayerName);
        assetRepository = new AssetRepository();

        if (subscriptions)
            Subscriber.RegisterPreAssetsSubscriptions();
       
        errors = [];

        for (int i = 0; i < layerRepository.LayerCount; i++)
            foreach (var u in layerRepository.GetLayerAssetIDs(i))
            {
                var AHDR = hipFile.DICT.ATOC.AHDRList.First(ahdr => ahdr.assetID == u);
                var asset = AssetService.ConvertHeaderToAsset(AHDR, Game, Platform, true, out AssetError error);
                assetRepository.Add(asset);

                if (error.reason != ErrorReason.NoError)
                    errors.Add(error);
                progressBar?.PerformStep();
            }

        foreach (var jspinfo in assetRepository.Assets.OfType<AssetJSP_INFO>())
        {
            var ni = jspinfo.SetJspAssets(GetJspAssets(jspinfo));
            foreach (var (jspAssetID, nodeInfo) in ni)
                Program.Renderer.Assets.AddToJspNodeInfos(jspAssetID, nodeInfo);
        }

        if ((hipFile.HIPB?.HasNoLayers ?? 0) != 0)
        {
            layerRepository.AutoLayers = true;
            layerRepository.ClearLayers();
        }
    }

    public void Dispose()
    {
        assetRepository.Dispose();
    }

    public bool LegacySave = false;

    public void Save(string path = null)
    {
        var hipFile = ArchiveService.BuildHipFile(session, assetRepository.Assets, layerRepository);
        if (LegacySave)
            hipFile.HIPB = null;
        hipFile.ToFile(path ?? session.CurrentFilePath);

        session.CurrentFilePath = path ?? session.CurrentFilePath;
    }

    public void ExportToIni(string path)
    {
        try
        {
            var hipFile = ArchiveService.BuildHipFile(session, assetRepository.Assets, layerRepository);
            hipFile.ToIni(path, true, true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public bool ContainsAsset(uint assetID) => assetRepository.Contains(assetID);

    public bool ContainsAsset(Asset asset) => assetRepository.Contains(asset);

    public bool ContainsAssetWithType(AssetType assetType) => assetRepository.ContainsType(assetType);

    public bool ContainsNewlyAddedAssetType(IEnumerable<uint> assetIDs)
    {
        var countPerType = new Dictionary<AssetType, int>(assetIDs.Count());
        foreach (var u in assetIDs)
        {
            var type = GetAsset(u).assetType;
            countPerType[type] = (countPerType.TryGetValue(type, out var c) ? c : 0) + 1;
        }
        foreach (var kvp in countPerType)
            if (AmountOfAssetWithType(kvp.Key) == kvp.Value)
                return true;
        return false;
    }

    public int AmountOfAssetWithType(AssetType assetType) => assetRepository.Assets.Count(a => a.assetType.Equals(assetType));

    public Asset GetAsset(uint key) => assetRepository.Get(key);

    public bool TryGetAsset(uint key, out Asset asset) => assetRepository.TryGet(key, out asset);

    public IEnumerable<Asset> Assets => assetRepository.Assets;

    public int AssetCount => assetRepository.Assets.Count();

    public IEnumerable<AssetType> AssetTypes => assetRepository.AssetTypes;

    public IEnumerable<Asset> FindWhoTargets(uint assetID) => assetRepository.FindWhoTargets(assetID);

    public Asset AddNewAsset(Asset asset, int layerIndex)
    {
        layerRepository.AddAsset(layerIndex, asset.assetID);
        assetRepository.Add(asset);
        return asset;
    }

    public Asset AddNewAsset(Section_AHDR AHDR, Game game, Platform platform, int layerIndex)
    {
        var asset = AssetService.ConvertHeaderToAsset(AHDR, game, platform, false, out _);
        return AddNewAsset(asset, layerIndex);
    }

    public List<Asset> ImportAssets(IEnumerable<Section_AHDR> AHDRs, int layerIndex, bool overwrite)
    {
        var result = new List<Asset>();

        foreach (Section_AHDR AHDR in AHDRs)
        {
            try
            {
                if (ContainsAsset(AHDR.assetID))
                {
                    if (overwrite)
                    {
                        RemoveAsset(AHDR.assetID);
                    }
                    else
                    {
                        var assetName = AssetService.GetUniqueAssetName(AHDR.ADBG.assetName, AHDR.assetID, false, false, ContainsAsset);
                        AHDR.ADBG.assetName = assetName;
                        AHDR.assetID = new AssetID(AHDR.ADBG.assetName);
                    }
                }

                var asset = AddNewAsset(AHDR, Game, Platform, layerIndex);

                if (asset is AssetSound sound)
                    SoundManager.AddToSoundInfo(sound);

                result.Add(asset);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to import asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName}: {ex.Message}");
            }
        }

        return result;
    }

    public List<Asset> AddMultipleAssets(IEnumerable<Asset> assets, int layerIndex, bool overwrite)
    {
        var result = new List<Asset>();

        foreach (Asset asset in assets)
        {
            try
            {
                if (ContainsAsset(asset.assetID))
                {
                    if (overwrite)
                    {
                        RemoveAsset(asset.assetID);
                    }
                    else
                    {
                        var assetName = AssetService.GetUniqueAssetName(asset.assetName, asset.assetID, false, false, ContainsAsset);
                        asset.assetName = assetName;
                        asset.assetID = new AssetID(asset.assetName);
                    }
                }

                AddNewAsset(asset, layerIndex);

                if (asset is AssetSound sound)
                    SoundManager.AddToSoundInfo(sound);

                result.Add(asset);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add asset [{asset.assetID:X8}] {asset.assetName}: {ex.Message}");
            }
        }

        foreach (Asset asset in result)
        {
            // process jsp info and set game later, to make sure any jsps are in archive already and we detect the game conversion
            if (asset is AssetJSP_INFO jspInfo)
            {
                if ((asset.game >= Game.Incredibles && Game == Game.BFBB) || (asset.game == Game.BFBB && Game >= Game.Incredibles))
                {
                    if (MessageBox.Show("The JSPINFO asset you are about to paste might not be compatible with the destination game if using native data, but can be converted. Convert?",
                        $"JSPINFO: Convert to {(Game == Game.BFBB ? "Legacy" : "Newer")} Format", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (jspInfo.JSP_AssetIDs.Length != jspInfo.JSP_AssetIDs.Sum(u => ContainsAsset(u) ? 1 : 0))
                        {
                            MessageBox.Show("One or more of the JSP assets referenced by this JSPINFO asset is missing from the archive. " +
                                "In order to convert the JSPINFO asset, please make sure all of the specified JSP AssetIDs exists in the target archive.", "JSPINFO: Cannot find JSP assets",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        try
                        {
                            if (Game == Game.BFBB)
                                JSPService.ConvertJSPINFOToLegacyFormat(jspInfo, GetAsset);
                            else
                                JSPService.ConvertJSPINFOToNewerFormat(jspInfo, GetAsset, Platform);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"There was an error converting the JSPINFO asset [{jspInfo.assetID:X8}] {jspInfo.assetName}: {ex.Message}", "JSPINFO: Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            asset.SetGame(Game);
        }

        return result;
    }

    public void RemoveAssets(IEnumerable<uint> assetIDs, bool removeSound = true)
    {
        foreach (var u in assetIDs.ToList())
            RemoveAsset(u, removeSound);
    }

    public void RemoveAsset(uint assetID, bool removeSound = true)
    {
        layerRepository.RemoveAsset(assetID);
        RemoveAssetFromRepository(assetID, removeSound);
    }

    private void RemoveAssetFromRepository(uint assetID, bool removeSound)
    {
        var asset = assetRepository.Get(assetID);

        if (removeSound)
            SoundManager.RemoveFromSoundInfo(assetID, GetAsset(assetID).assetType);

        if (asset is AssetMODL)
            foreach (var table in assetRepository.Assets.OfType<ITableAsset>().Where(t => ((Asset)t).HasReference(asset.assetID)))
                table.RemoveEntry(assetID);

        assetRepository.Remove(assetID);
    }

    public void SetAssetData(uint assetID, byte[] data, Platform platform)
    {
        if (!TryGetAsset(assetID, out Asset asset))
            return;

        var AHDR = new Section_AHDR(asset.assetID, asset.assetType, asset.flags,
            new Section_ADBG(0, asset.assetName, asset.assetFileName, asset.checksum),
            data);

        try
        {
            var newAsset = AssetService.ConvertHeaderToAsset(AHDR, Game, platform, false, out _);
            assetRepository.Add(newAsset);
        }
        catch (Exception e)
        {
            MessageBox.Show($"There was an error performing the asset property change operation: {e.Message}");
        }
    }

    public int LayerCount => layerRepository.LayerCount;

    public bool AutoLayers => layerRepository.AutoLayers;

    public int GetLayerType(int index) => LayerService.LayerTypeGenericToSpecific(GetLayerTypeGeneric(index), Game);

    public string GetLayerName(int index) => layerRepository.GetLayerName(index);

    public LayerType GetLayerTypeGeneric(int index) => layerRepository.GetLayerType(index);

    public void SetGameSpecificLayerType(int index, int type) => SetLayerType(index, LayerService.LayerTypeSpecificToGeneric(type, Game));

    public void SetLayerType(int index, LayerType type) => layerRepository.SetLayerType(index, type);

    public string LayerToString(int index) => layerRepository.LayerToString(index);

    public IEnumerable<uint> GetLayerAssetIDs(int layer) => layerRepository.AutoLayers ? assetRepository.Assets.Select(a => a.assetID) : layerRepository.GetLayerAssetIDs(layer);

    public int GetFirstActiveLayerIndex() => layerRepository.GetFirstActiveLayerIndex();

    public int AddLayer(LayerType layerType = LayerType.DEFAULT, int index = -1) => layerRepository.Add(layerType, index);

    public void RemoveLayer(int index)
    {
        var assets = GetLayerAssetIDs(index).Reverse();
        foreach (var asset in assets)
            RemoveAssetFromRepository(asset, true);
        layerRepository.RemoveLayer(index);
    }

    public void RemoveLayerOfType(LayerType type) => layerRepository.RemoveLayersOfType(type);

    public bool MoveLayerUp(int index) => layerRepository.MoveLayerUp(index);

    public bool MoveLayerDown(int index) => layerRepository.MoveLayerDown(index);

    public int GetLayerFromAssetID(uint assetID) => layerRepository.GetLayerFromAssetID(assetID);

    public bool RenameLayer(int index, string name) => layerRepository.RenameLayer(index, name);

    public IEnumerable<AssetType> AssetTypesOnLayer(int index) => layerRepository.AutoLayers ?
        assetRepository.Assets.Select(a => a.assetType).Distinct() :
        layerRepository.GetLayerAssetIDs(index).Select(u => assetRepository.Get(u).assetType).Distinct();

    public int GetOrCreateLayerOfType(LayerType layerType) => layerRepository.GetOrCreateLayerOfType(layerType);

    public byte[] GetSoundData(uint assetID, byte[] previousData) => SoundManager.GetCompleteSoundData(assetID, GetAsset(assetID).assetType, previousData);

    public IEnumerable<Asset> ImportSounds(bool raw, string[] fileNames, AssetType assetType, bool forceOverwrite, Action<string> step = null)
    {
        var sounds = SoundService.CreateSounds(Game, Platform, assetType, raw, fileNames, step);

        if (!sounds.Any())
            return [];

        var sramLayer = GetOrCreateLayerOfType(LayerType.SRAM);
        TableManager.GetSNDI(true);

        var assets = new List<Asset>();

        foreach (var AHDR in sounds)
        {
            if (ContainsAsset(AHDR.assetID))
            {
                var result = forceOverwrite ? DialogResult.Yes :
                MessageBox.Show($"Asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                    RemoveAsset(AHDR.assetID, true);
                else
                    continue;
            }

            try
            {
                var sound = (AssetSound)AddNewAsset(AHDR, Game, Platform, sramLayer);
                SoundManager.AddToSoundInfo(sound);
                assets.Add(sound);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        return assets;
    }

    public AssetJSP[] GetJspAssets(AssetJSP_INFO jspInfo)
    {
        var result = new List<AssetJSP>();

        if (jspInfo.JSP_AssetIDs != null && jspInfo.JSP_AssetIDs.Length > 0)
        {
            foreach (var u in jspInfo.JSP_AssetIDs)
                if (GetAsset(u) is AssetJSP jsp)
                    result.Add(jsp);
            return result.ToArray();
        }

        var layerIndex = GetLayerFromAssetID(jspInfo.assetID);
        for (int i = layerIndex - 3; i < layerIndex; i++)
            if (i > 0 && i < LayerCount)
            {
                if (GetLayerTypeGeneric(i) == LayerType.BSP)
                {
                    foreach (var u in GetLayerAssetIDs(i))
                        if (GetAsset(u) is AssetJSP jsp)
                            result.Add(jsp);
                }
                else if (GetLayerTypeGeneric(i) == LayerType.JSPINFO)
                    result.Clear();
            }
        return result.ToArray();
    }
}
