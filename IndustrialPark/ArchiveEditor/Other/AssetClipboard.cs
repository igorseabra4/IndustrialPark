using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class AssetClipboard
    {
        public List<Game> games;
        public List<Endianness> endiannesses;
        public List<Section_AHDR> assets;
        public List<AssetID[]> jspExtraInfo;

        public AssetClipboard()
        {
            games = new List<Game>();
            endiannesses = new List<Endianness>();
            assets = new List<Section_AHDR>();
            jspExtraInfo = new List<AssetID[]>();
        }
        
        public void Add(Game game, Endianness endianness, Section_AHDR asset, AssetID[] jspExtraInfo)
        {
            games.Add(game);
            endiannesses.Add(endianness);
            assets.Add(asset);
            this.jspExtraInfo.Add(jspExtraInfo);
        }
    }
}