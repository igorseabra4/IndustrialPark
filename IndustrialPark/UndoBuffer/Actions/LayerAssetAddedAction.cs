using IndustrialPark.Archive;

namespace IndustrialPark;

public class LayerAssetAddedAction : IReversibleAction
{
    private LayerRepository layerRepository;
    private int index;
    private uint assetID;
    private bool assetRemoved;

    public LayerAssetAddedAction(LayerRepository layerRepository, int index, uint assetID, bool assetRemoved = false)
    {
        this.layerRepository = layerRepository;
        this.index = index;
        this.assetID = assetID;
        this.assetRemoved = assetRemoved;
    }

    public void Undo()
    {
        if (assetRemoved)
            layerRepository.AddAsset(index, assetID);
        else
            layerRepository.RemoveAsset(assetID);
    }

    public void Redo()
    {
        if (assetRemoved)
            layerRepository.RemoveAsset(assetID);
        else
            layerRepository.AddAsset(index, assetID);
    }

    public override string ToString() => $"Asset [{assetID:X8}] {(assetRemoved ? "removed from" : "added to")} layer at index {index}";
}