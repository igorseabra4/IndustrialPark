using System.Collections.Generic;
using HipHopFile;

namespace IndustrialPark
{
    public class AssetClipboard
    {
        public Game game;
        public Endianness endianness;
        public List<Section_AHDR> assets;

        public AssetClipboard(Game game, Endianness endianness, List<Section_AHDR> assets)
        {
            this.game = game;
            this.endianness = endianness;
            this.assets = assets;
        }
    }
}