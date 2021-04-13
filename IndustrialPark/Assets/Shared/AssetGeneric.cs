using HipHopFile;

namespace IndustrialPark
{
    public class AssetGeneric : Asset
    {
        public byte[] Data { get; set; }

        public AssetGeneric(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            Data = AHDR.data;
        }

        public override byte[] Serialize() => Data;
    }
}