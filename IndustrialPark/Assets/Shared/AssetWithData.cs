using HipHopFile;

namespace IndustrialPark
{
    public class AssetWithData : Asset
    {
        public byte[] Data { get; set; }

        public override string AssetInfo => Data.Length.ToString() + " bytes";

        public AssetWithData(string assetName, AssetType assetType, byte[] data) : base(assetName, assetType)
        {
            Data = data;
        }

        public AssetWithData(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            Data = AHDR.data;
        }

        public override byte[] Serialize(Game game, Platform platform) => Data;
    }
}