using HipHopFile;
using IndustrialPark.Archive;

namespace IndustrialPark;

public class LayerTypeChangedAction : IReversibleAction
{
    private readonly LayerRepository layerRepository;
    private readonly int index;
    private readonly LayerType oldValue;
    private readonly LayerType newValue;

    public LayerTypeChangedAction(LayerRepository layerRepository, int index, LayerType newValue)
    {
        this.layerRepository = layerRepository;
        this.index = index;
        this.oldValue = layerRepository.GetLayerType(index);
        this.newValue = newValue;
    }

    public void Undo()
    {
        layerRepository.SetLayerType(index, oldValue);
    }

    public void Redo()
    {
        layerRepository.SetLayerType(index, newValue);
    }

    public override string ToString() => $"Layer at {index} changed type from {oldValue} to {newValue}";
}