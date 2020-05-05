using HipHopFile;

namespace IndustrialPark
{
    public class AssetDPAT : BaseAsset
    {
        public AssetDPAT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x8;
    }
}