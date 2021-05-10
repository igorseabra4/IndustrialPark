using System.Collections.Generic;
using HipHopFile;

namespace IndustrialPark
{
    public class AssetClipboard
    {
        public List<Game> games;
        public List<Endianness> endiannesses;
        public List<Section_AHDR> assets;

        public AssetClipboard(List<Game> games, List<Endianness> endiannesses, List<Section_AHDR> assets)
        {
            this.games = games;
            this.endiannesses = endiannesses;
            this.assets = assets;
        }
    }
}