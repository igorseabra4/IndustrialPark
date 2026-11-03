using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public class LayerRepository
{
    private List<Layer> Layers;

    public bool AutoLayers
    {
        get;
        set
        {
            if (field != value)
                AutoLayersChanged?.Invoke(value);
            field = value;
        }
    }

    public event Action<bool> AutoLayersChanged;
    public event Action<int> LayerAdded;
    public event Action<int> LayerRemoved;
    public event Action<int, int> LayerSwapped;
    public event Action<int, LayerType> LayerTypeChanged;
    public event Action<int, string> LayerRenamed;
    public event Action<int[]> LayerIndicesSet;
    public event Action<int, uint> LayerAssetAdded;
    public event Action<int, uint> LayerAssetRemoved;

    public LayerRepository(bool autoLayers = false)
    {
        this.AutoLayers = autoLayers;
        Layers = [];
    }

    public LayerRepository(List<Section_LHDR> LHDRList, Game game, Func<int, string> getLayerName) : this(false)
    {
        for (int i = 0; i < LHDRList.Count; i++)
            Layers.Add(LayerService.LHDRToLayer(LHDRList[i], game, getLayerName(i)));
    }

    private bool IsValidIndex(int index) => !AutoLayers && index >= 0 && index < Layers.Count;

    public int LayerCount => Layers.Count;

    public LayerType GetLayerType(int index) => Layers[index].Type;

    public void SetLayerType(int index, LayerType type)
    {
        if (!IsValidIndex(index) || Layers[index].Type == type)
            return;

        LayerTypeChanged?.Invoke(index, type);
        Layers[index].Type = type;
    }

    public string LayerToString(int index) => "Layer " + index.ToString("D2") + ": "
        + (string.IsNullOrWhiteSpace(Layers[index].LayerName) ? Layers[index].Type.ToString() : Layers[index].LayerName)
        + " [" + Layers[index].AssetIDs.Count() + "]";

    public List<uint> GetLayerAssetIDs(int index) => IsValidIndex(index) ? Layers[index].AssetIDs : [];

    public int GetFirstActiveLayerIndex()
    {
        if (AutoLayers)
            return -1;
        for (int i = 0; i < Layers.Count; i++)
            if (Layers[i].AssetIDs.Count > 0)
                return i;
        return -1;
    }

    public int Add(LayerType layerType = LayerType.DEFAULT, int index = -1)
    {
        if (AutoLayers)
            return -1;

        if (index == -1 || index >= Layers.Count)
            index = Layers.Count;

        Layers.Insert(index, new Layer(layerType));
        LayerAdded?.Invoke(index);

        return index;
    }

    public void Add(params IEnumerable<Layer> layers)
    {
        if (AutoLayers)
            return;

        foreach (var layer in layers)
        {
            Layers.Add(layer);
            LayerAdded?.Invoke(Layers.Count - 1);
        }
    }

    public void RemoveLayer(int index)
    {
        if (!IsValidIndex(index))
            return;

        LayerRemoved?.Invoke(index);
        Layers.RemoveAt(index);
    }

    public void RemoveLayersOfType(LayerType type)
    {
        if (AutoLayers)
            return;

        var layers = Layers.Where(l => l.Type == type).ToList();
        foreach (var layer in layers)
        {
            int i = Layers.IndexOf(layer);
            LayerRemoved?.Invoke(i);
            Layers.RemoveAt(i);
        }
    }

    public void ClearLayers()
    {
        for (int i = Layers.Count - 1; i >= 0; i--)
            LayerRemoved?.Invoke(i);
        
        Layers = [];
    }

    public bool MoveLayerUp(int index) => SwapLayers(index, index - 1);

    public bool MoveLayerDown(int index) => SwapLayers(index, index + 1);

    public bool SwapLayers(int index1, int index2)
    {
        if (IsValidIndex(index1) && IsValidIndex(index2))
        {
            (Layers[index1], Layers[index2]) = (Layers[index2], Layers[index1]);
            LayerSwapped?.Invoke(index1, index2);
            return true;
        }
        return false;
    }

    public int GetLayerFromAssetID(uint assetID)
    {
        if (AutoLayers)
            return -1;

        for (int i = 0; i < Layers.Count; i++)
            if (Layers[i].AssetIDs.Contains(assetID))
                return i;

        throw new Exception($"Asset ID {assetID:X8} is not present in any layer.");
    }

    public bool RenameLayer(int index, string name)
    {
        if (!IsValidIndex(index))
            return false;

        LayerRenamed?.Invoke(index, name);
        Layers[index].LayerName = name;
        return true;
    }

    public void SetLayerIndices(int[] indices)
    {
        if (LayerCount != indices.Length)
            return;

        var newLayers = new Layer[indices.Length];
        for (int i = 0; i < indices.Length; i++)
            newLayers[indices[i]] = Layers[i];

        Layers = newLayers.ToList();

        LayerIndicesSet?.Invoke(indices);
    }

    public void AddAsset(int layerIndex, uint assetID)
    {
        if (AutoLayers)
            return;

        if (IsValidIndex(layerIndex) && !Layers[layerIndex].AssetIDs.Contains(assetID))
        {
            Layers[layerIndex].AssetIDs.Add(assetID);

            LayerAssetAdded?.Invoke(layerIndex, assetID);
        }
    }

    public bool RemoveAsset(uint assetID)
    {
        if (AutoLayers)
            return false;

        for (int i = 0; i < Layers.Count; i++)
            if (Layers[i].AssetIDs.Remove(assetID))
            {
                LayerAssetRemoved?.Invoke(i, assetID);
                return true;
            }
        return false;
    }

    public int GetOrCreateLayerOfType(LayerType layerType)
    {
        if (AutoLayers)
            return -1;
        int layerIndex = Layers.FindIndex(l => l.Type == layerType);
        if (layerIndex == -1)
            layerIndex = Add(layerType);
        return layerIndex;
    }

    public IEnumerable<Section_LHDR> ToLayerHeaders(Game game) => Layers.Select(l => l.BuildLHDR(game));

    public void SetLayers(IEnumerable<Layer> layers)
    {
        for (int i = Layers.Count - 1; i >= 0; i--)
            LayerRemoved?.Invoke(i);

        Layers = [.. layers];

        for (int i = 0; i < Layers.Count; i++)
            LayerAdded?.Invoke(i);
    }

    public void SortLayers(Game game)
    {
        List<Layer> layersOld = [.. Layers];
        Layers = [.. Layers.OrderBy(f => LayerService.LayerTypeGenericToSpecific(f.Type, game), new LayerComparer(game))];

        var indices = new int[Layers.Count];
        for (int i = 0; i < Layers.Count; i++)
            indices[i] = Layers.IndexOf(layersOld[i]);
        LayerIndicesSet?.Invoke(indices);
    }

    public string GetLayerName(int index) => IsValidIndex(index) ? Layers[index].LayerName : null;

    public Dictionary<int, string> GetLayerNames() =>
        Layers.Select((l, i) => (l, i)).Where(x => !string.IsNullOrWhiteSpace(x.l.LayerName)).ToDictionary(x => x.i, x => x.l.LayerName);

    public int GetJSPInfoInsertIndex()
    {
        int lastJSPINFOIndex = Layers.FindLastIndex(l => l.Type == LayerType.JSPINFO);
        if (lastJSPINFOIndex == -1)
            lastJSPINFOIndex = Layers.FindLastIndex(l => l.Type == LayerType.TEXTURE_STRM);
        if (lastJSPINFOIndex == -1)
            lastJSPINFOIndex = Layers.FindLastIndex(l => l.Type == LayerType.TEXTURE);

        return lastJSPINFOIndex != -1 ? (lastJSPINFOIndex + 1) : Layers.Count;
    }
}
