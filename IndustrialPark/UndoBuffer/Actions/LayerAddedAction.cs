using IndustrialPark.Archive;

namespace IndustrialPark;

public class LayerAddedAction : IReversibleAction
{
    private LayerRepository layerRepository;
    private int index;
    private Layer snapshot;
    private bool layerRemoved;

    public LayerAddedAction(LayerRepository layerRepository, int index, bool layerRemoved = false)
    {
        this.layerRepository = layerRepository;
        this.index = index;
        snapshot = new Layer(layerRepository.GetLayerType(index))
        {
            AssetIDs = [.. layerRepository.GetLayerAssetIDs(index)],
            LayerName = layerRepository.GetLayerName(index),
        };
        this.layerRemoved = layerRemoved;
    }

    public void Undo()
    {
        if (layerRemoved)
            AddLayer();
        else
            RemoveLayer();
    }

    public void Redo()
    {
        if (layerRemoved)
            RemoveLayer();
        else
            AddLayer();
    }

    private void AddLayer()
    {
        layerRepository.Add(new Layer(snapshot.Type)
        {
            AssetIDs = [.. snapshot.AssetIDs],
            LayerName = snapshot.LayerName,
        });
    }

    private void RemoveLayer()
    {
        layerRepository.RemoveLayer(index);
    }

    public override string ToString() =>
        $"Layer of type {snapshot.Type} {(snapshot.AssetIDs.Count > 0 ? $"with {snapshot.AssetIDs.Count} assets " : "")}{(layerRemoved ? "removed" : "added")} at index {index}";
}