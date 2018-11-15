using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSIMP : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender { get => dontRender; }

        protected override int EventStartOffset { get => 0x60 + Offset; }

        public AssetSIMP(Section_AHDR AHDR) : base(AHDR) { }

        [Browsable(false)]
        public override AssetID ReferenceAssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("Simple Object")]
        public AssetID AnimationAssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("Simple Object")]
        public float UnknownFloat_54
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Simple Object")]
        public int Unknown_58
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Simple Object"), TypeConverter(typeof(HexIntTypeConverter))]
        public int Unknown_5C
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }
    }
}