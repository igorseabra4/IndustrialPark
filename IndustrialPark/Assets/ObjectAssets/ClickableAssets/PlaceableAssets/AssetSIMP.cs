using HipHopFile;

namespace IndustrialPark
{
    public class AssetSIMP : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        public AssetSIMP(Section_AHDR AHDR) : base(AHDR) { }
        
        public float UnknownFloat_54
        {
            get { return ReadFloat(0x54); }
            set { Write(0x54, value); }
        }

        public int Unknown_58
        {
            get { return ReadInt(0x58); }
            set { Write(0x58, value); }
        }

        public int Unknown_5C
        {
            get { return ReadInt(0x5C); }
            set { Write(0x5C, value); }
        }

        public AssetEvent[] Events
        {
            get { return ReadEvents(0x60); }
            set { WriteEvents(0x60, value); }
        }
    }
}