using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class Layer
    {
        public LayerType Type;
        public List<uint> AssetIDs;

        public Layer(LayerType type)
        {
            Type = type;
            AssetIDs = new List<uint>();
        }
    }
}
