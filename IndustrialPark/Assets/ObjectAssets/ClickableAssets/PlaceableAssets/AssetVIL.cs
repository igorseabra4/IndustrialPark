using HipHopFile;

namespace IndustrialPark
{
    public class AssetVIL : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        public AssetVIL(Section_AHDR AHDR) : base(AHDR) { }
        
        public AssetID Unknown
        {
            get { return ReadUInt(0x58); }
            set { Write(0x58, value); }
        }

        public AssetID MVPTAssetID
        {
            get { return ReadUInt(0x60); }
            set { Write(0x60, value); }
        }

        public AssetID DYNAAssetID_0
        {
            get { return ReadUInt(0x64); }
            set { Write(0x64, value); }
        }

        public AssetID DYNAAssetID_1
        {
            get { return ReadUInt(0x68); }
            set { Write(0x68, value); }
        }

        public AssetEvent[] Events
        {
            get { return ReadEvents(0x6C); }
            set { WriteEvents(0x6C, value); }
        }
    }
}