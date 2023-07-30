using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
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
    }
}
