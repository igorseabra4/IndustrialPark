using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEGEN : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender { get => dontRender; }
        
        public AssetEGEN(Section_AHDR AHDR) : base(AHDR) { }

        [Category("EGEN")]
        public string Note => "This asset type is not entirely supported yet...";
    }
}