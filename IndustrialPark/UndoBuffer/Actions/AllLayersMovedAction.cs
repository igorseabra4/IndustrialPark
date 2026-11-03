using IndustrialPark.Archive;
using System;

namespace IndustrialPark;

public class AllLayersMovedAction : IReversibleAction
{
    private readonly LayerRepository layerRepository;
    private readonly int[] indices;

    public AllLayersMovedAction(LayerRepository layerRepository, int[] indices)
    {
        if (indices.Length != layerRepository.LayerCount)
            throw new ArgumentException("Index count must be the same as layer count");

        this.layerRepository = layerRepository;
        this.indices = indices;
    }

    private int[] ReverseDict()
    {
        var result = new int[indices.Length];
        for (int i = 0; i < indices.Length; i++)
            result[indices[i]] = i;
        return result;
    }

    public void Undo()
    {
        layerRepository.SetLayerIndices(ReverseDict());
    }

    public void Redo()
    {
        layerRepository.SetLayerIndices(indices);
    }

    public override string ToString() => $"All layers reordered";
}