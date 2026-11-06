using HipHopFile;
using System.Linq;

namespace IndustrialPark.Archive;

public class TableAssetManager
{
    private readonly ArchiveSession session;
    private readonly AssetRepository assetRepository;
    private readonly LayerRepository layerRepository;

    public TableAssetManager(
        ArchiveSession session,
        AssetRepository assetRepository,
        LayerRepository layerRepository)
    {
        this.session = session;
        this.assetRepository = assetRepository;
        this.layerRepository = layerRepository;
    }

    public AssetPIPT GetPIPT(bool create = false) => (AssetPIPT)GetAssetOfType(AssetType.PipeInfoTable, AssetTemplate.Pipe_Info_Table, create);

    public AssetCOLL GetCOLL(bool create = false) => (AssetCOLL)GetAssetOfType(AssetType.CollisionTable, AssetTemplate.Collision_Table, create);

    public AssetLODT GetLODT(bool create = false) => (AssetLODT)GetAssetOfType(AssetType.LevelOfDetailTable, AssetTemplate.Level_Of_Detail_Table, create);

    public AssetSHDW GetSHDW(bool create = false) => (AssetSHDW)GetAssetOfType(AssetType.ShadowTable, AssetTemplate.Shadow_Table, create);

    public AssetJAW GetJAW(bool create = false) => (AssetJAW)GetAssetOfType(AssetType.JawDataTable, AssetTemplate.Jaw_Data_Table, create);

    public Asset GetSNDI(bool create = false) => GetAssetOfType(AssetType.SoundInfo, AssetTemplate.Sound_Info, create);

    private Asset GetAssetOfType(AssetType assetType, AssetTemplate assetTemplate, bool create)
    {
        if (!assetRepository.ContainsType(assetType))
        {
            if (!create)
                return null;

            var layerType = assetTemplate == AssetTemplate.Sound_Info ? LayerType.SNDTOC : LayerType.DEFAULT;
            int layerIndex = layerRepository.GetOrCreateLayerOfType(layerType);

            var asset = AssetFactory.GenerateTemplate(session.Game, session.Platform, assetRepository.Contains, assetTemplate)[0];

            assetRepository.Add(asset);
            layerRepository.AddAsset(layerIndex, asset.assetID);
        }
        return assetRepository.Assets.FirstOrDefault(asset => asset.assetType == assetType);
    }

    public void MergeTableAsset<T, U>(T asset) where T : ITableAsset<T, U>
    {
        ITableAsset<T, U> current = assetRepository.Assets.OfType<ITableAsset<T, U>>().First();
        current.Merge(asset);
        ((Asset)current).InvokePropertyChanged(false);
    }

    public void MergeSoundInfo<T>(T asset) where T : ISoundInfoAsset<T>
    {
        var SNDI = assetRepository.Assets.First(a => a.assetType == AssetType.SoundInfo);
        ((ISoundInfoAsset<T>)SNDI).Merge(asset);
        SNDI.InvokePropertyChanged(false);
    }

    public void MergeTableAssets()
    {
        void MergeTableAssets<T, U>(AssetType assetType) where T : Asset, ITableAsset<T, U>
        {
            var assets = assetRepository.Assets.Where(asset => asset.assetType == assetType).Cast<T>().ToList();
            if (assets.Count < 2)
                return;
            for (int i = 1; i < assets.Count; i++)
            {
                assetRepository.Remove(assets[i].assetID);
                layerRepository.RemoveAsset(assets[i].assetID);
            }
            for (int i = 1; i < assets.Count; i++)
                MergeTableAsset<T, U>(assets[i]);
        }

        MergeTableAssets<AssetCOLL, EntryCOLL>(AssetType.CollisionTable);
        MergeTableAssets<AssetLODT, EntryLODT>(AssetType.LevelOfDetailTable);
        MergeTableAssets<AssetPIPT, PipeInfo>(AssetType.PipeInfoTable);
        MergeTableAssets<AssetSHDW, EntrySHDW>(AssetType.ShadowTable);
        MergeTableAssets<AssetJAW, EntryJAW>(AssetType.JawDataTable);

        void MergeSoundInfoAssets<T>() where T : Asset, ISoundInfoAsset<T>
        {
            var assets = assetRepository.Assets.Where(asset => asset.assetType == AssetType.SoundInfo).Cast<T>().ToList();
            if (assets.Count < 2)
                return;
            for (int i = 1; i < assets.Count; i++)
            {
                assetRepository.Remove(assets[i].assetID);
                layerRepository.RemoveAsset(assets[i].assetID);
            }
            for (int i = 1; i < assets.Count; i++)
                MergeSoundInfo<T>(assets[i]);
        }

        switch (session.Platform)
        {
            case Platform.GameCube when session.Game < Game.Incredibles:
                MergeSoundInfoAssets<AssetSNDI_GCN_V1>();
                break;
            case Platform.GameCube:
                MergeSoundInfoAssets<AssetSNDI_GCN_V2>();
                break;
            case Platform.Xbox:
                MergeSoundInfoAssets<AssetSNDI_XBOX>();
                break;
            case Platform.PS2:
                MergeSoundInfoAssets<AssetSNDI_PS2>();
                break;
        }
    }
}