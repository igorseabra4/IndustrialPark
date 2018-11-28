using HipHopFile;

namespace IndustrialPark
{
    public class AssetDPAT : ObjectAsset
    {
        public AssetDPAT(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x8;
    }
}