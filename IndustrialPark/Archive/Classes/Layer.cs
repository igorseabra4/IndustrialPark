using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark.Archive;

public class Layer
{
    public LayerType Type;
    public List<uint> AssetIDs;
    public string LayerName;

    public Layer(LayerType type)
    {
        Type = type;
        AssetIDs = new List<uint>();
        LayerName = null;
    }

    public Layer(LayerType type, int assetCountBuffer, string layerName)
    {
        Type = type;
        AssetIDs = new List<uint>(assetCountBuffer);
        LayerName = layerName;
    }

    public Section_LHDR BuildLHDR(Game game) => new Section_LHDR()
    {
        layerType = LayerService.LayerTypeGenericToSpecific(Type, game),
        assetIDlist = AssetIDs,
        LDBG = new Section_LDBG(-1)
    };
}
