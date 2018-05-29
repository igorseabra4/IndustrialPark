using HipHopFile;

namespace IndustrialPark
{
    public class AssetRWTX : Asset
    {
        public AssetRWTX(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(bool defaultMode = true) { }
    }
}