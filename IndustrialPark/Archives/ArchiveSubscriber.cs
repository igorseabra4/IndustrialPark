using HipHopFile;
using IndustrialPark.ArchiveEditors;
using IndustrialPark.Assets;
using IndustrialPark.Undo;
using System;

namespace IndustrialPark.Archives;

public class ArchiveSubscriber
{
    private readonly Archive archive;
    private readonly ArchiveSession session;
    private readonly AssetRepository assetRepository;
    private readonly LayerRepository layerRepository;

    public ArchiveSubscriber(Archive archive, ArchiveSession session, AssetRepository assetRepository, LayerRepository layerRepository)
    {
        this.archive = archive;
        this.session = session;
        this.assetRepository = assetRepository;
        this.layerRepository = layerRepository;
    }

    public void SubscribeToArchiveHeaderSet(Action<Platform, Game, DateTime, string> action) => session.ArchiveHeaderSet += action;
    public void SubscribeToModDateSet(Action<int> action) => session.ModDateSet += action;
    public void SubscribeToAssetAdded(Action<Asset> action) => assetRepository.AssetAdded += action;
    public void UnsubscribeFromAssetAdded(Action<Asset> action) => assetRepository.AssetAdded -= action;
    public void SubscribeToAssetRemoved(AssetRepository.AssetRemovedHandler action) => assetRepository.AssetRemoved += action;
    public void UnsubscribeFromAssetRemoved(AssetRepository.AssetRemovedHandler action) => assetRepository.AssetRemoved -= action;
    public void SubscribeToAutoLayersChanged(Action<bool> action) => layerRepository.AutoLayersChanged += action;
    public void SubscribeToLayerAdded(Action<int> action) => layerRepository.LayerAdded += action;
    public void SubscribeToLayerRemoved(Action<int> action) => layerRepository.LayerRemoved += action;
    public void SubscribeToLayerSwapped(Action<int, int> action) => layerRepository.LayerSwapped += action;
    public void SubscribeToLayerTypeChanged(Action<int, LayerType> action) => layerRepository.LayerTypeChanged += action;
    public void SubscribeToLayerRenamed(Action<int, string> action) => layerRepository.LayerRenamed += action;
    public void SubscribeToLayerIndicesSet(Action<int[]> action) => layerRepository.LayerIndicesSet += action;
    public void SubscribeToLayerAssetAdded(Action<int, uint> action) => layerRepository.LayerAssetAdded += action;
    public void SubscribeToLayerAssetRemoved(Action<int, uint> action) => layerRepository.LayerAssetRemoved += action;

    public void RegisterPreAssetsSubscriptions()
    {
        SubscribeToAssetAdded((asset) =>
        {
            switch (asset)
            {
                case AssetRWTX rwtx:
                    TextureConverter.EnableTextureForDisplay(rwtx);
                    break;
                case AssetSFX sfx:
                    sfx.GetAttach = attachID => archive.TryGetAsset(attachID, out Asset asset) ? asset : null;
                    break;
                case AssetSDFX sdfx:
                    sdfx.GetSGRP = sgrp => archive.TryGetAsset(sgrp, out AssetSGRP SGRP) ? SGRP : null;
                    break;
                case DynaGObjectTeleport teleport:
                    teleport.GetMRKR = mrkr => archive.TryGetAsset(mrkr, out AssetMRKR MRKR) ? MRKR : null;
                    break;
            }
        });

        SubscribeToAssetRemoved((asset, _) =>
        {
            if (asset is IDisposableAsset ida)
                ida.Dispose();
        });

        Program.Renderer.Assets.RegisterSubscriptions(archive);
        Program.NameRepository.RegisterSubscriptions(archive);
    }

    public void SetupUndoBuffer(ArchiveEditor ae, UndoBuffer undo)
    {
        // TODO: SubscribeToArchiveHeaderSet
        // TODO: SubscribeToModDateSet

        SubscribeToAssetAdded((asset) => undo.AddActionToTransaction(ae, new AssetAddedAction(session, assetRepository, asset)));
        SubscribeToAssetRemoved((asset, isDisposing) => {
            if (!isDisposing)
                undo.AddActionToTransaction(ae, new AssetAddedAction(session, assetRepository, asset, true));
        });

        SubscribeToAutoLayersChanged((value) => undo.AddActionToTransaction(ae, new AutoLayersChangedAction(layerRepository, value)));
        SubscribeToLayerTypeChanged((index, type) => undo.AddActionToTransaction(ae, new LayerTypeChangedAction(layerRepository, index, type)));
        SubscribeToLayerAdded((index) => undo.AddActionToTransaction(ae, new LayerAddedAction(layerRepository, index)));
        SubscribeToLayerRemoved((index) => undo.AddActionToTransaction(ae, new LayerAddedAction(layerRepository, index, true)));
        SubscribeToLayerSwapped((index1, index2) => undo.AddActionToTransaction(ae, new LayerSwappedAction(layerRepository, index1, index2)));
        SubscribeToLayerRenamed((index, name) => undo.AddActionToTransaction(ae, new LayerRenamedAction(layerRepository, index, name)));
        SubscribeToLayerIndicesSet((indices) => undo.AddActionToTransaction(ae, new AllLayersMovedAction(layerRepository, indices)));
        SubscribeToLayerAssetAdded((index, assetID) => undo.AddActionToTransaction(ae, new LayerAssetAddedAction(layerRepository, index, assetID)));
        SubscribeToLayerAssetRemoved((index, assetID) => undo.AddActionToTransaction(ae, new LayerAssetAddedAction(layerRepository, index, assetID, true)));
    }
}