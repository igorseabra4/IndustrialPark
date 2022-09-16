using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class Layer
    {
        public int Type;
        public List<uint> AssetIDs;

        public Layer(int type)
        {
            Type = type;
            AssetIDs = new List<uint>();
        }
    }
}
