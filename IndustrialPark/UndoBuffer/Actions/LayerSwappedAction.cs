using IndustrialPark.Archive;

namespace IndustrialPark;

public class LayerSwappedAction : IReversibleAction
{
    private LayerRepository layerRepository;
    private int index1;
    private int index2;

    public LayerSwappedAction(LayerRepository layerRepository, int index1, int index2)
    {
        this.layerRepository = layerRepository;
        this.index1 = index1;
        this.index2 = index2;
    }

    public void Undo()
    {
        layerRepository.SwapLayers(index1, index2);
    }

    public void Redo()
    {
        layerRepository.SwapLayers(index1, index2);
    }

    public override string ToString() => $"Layers with indices {index1} and {index2} swapped positions";
}