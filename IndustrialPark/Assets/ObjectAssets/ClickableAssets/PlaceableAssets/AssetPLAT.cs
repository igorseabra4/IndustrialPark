using HipHopFile;

namespace IndustrialPark
{
    public class AssetPLAT : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        public AssetPLAT(Section_AHDR AHDR) : base(AHDR) { }
    }
}