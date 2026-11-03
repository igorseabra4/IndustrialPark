using IndustrialPark.Archive;

namespace IndustrialPark;

public class LayerRenamedAction : IReversibleAction
{
    private readonly LayerRepository layerRepository;
    private readonly int index;
    private readonly string oldName;
    private readonly string newName;

    public LayerRenamedAction(LayerRepository layerRepository, int index, string name)
    {
        this.layerRepository = layerRepository;
        this.index = index;
        this.oldName = layerRepository.GetLayerName(index);
        this.newName = name;
    }

    public void Undo()
    {
        layerRepository.RenameLayer(index, oldName);
    }

    public void Redo()
    {
        layerRepository.RenameLayer(index, newName);
    }

    public override string ToString() => $"Layer at index {index} renamed from {oldName} to {newName}";
}