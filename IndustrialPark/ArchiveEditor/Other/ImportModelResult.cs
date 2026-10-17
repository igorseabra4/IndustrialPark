using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    public sealed class ImportModelResult
    {
        public List<List<Section_AHDR>> AHDRCollections { get; init; }
        public AssetType AssetType { get; init; }
        public bool Overwrite { get; init; }
        public bool PiptVColors { get; init; }
        public bool GenerateSimps { get; init; }
        public bool LedgeGrabSimps { get; init; }
        public bool SolidSimps { get; init; }
    }
}
