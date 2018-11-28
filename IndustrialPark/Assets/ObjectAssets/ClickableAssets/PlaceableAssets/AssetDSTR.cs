using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x8C + Offset;

        public AssetDSTR(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (UnknownAssetID74 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Browsable(false)]
        public override AssetID ReferenceAssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("Destructable")]
        public AssetID AnimationAssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("Destructable")]
        public int UnknownInt54
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt58
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt5C
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt60
        {
            get => ReadInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt64
        {
            get => ReadInt(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt68
        {
            get => ReadInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Destructable")]
        public float UnknownFloat6C
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }

        [Category("Destructable")]
        public float UnknownFloat70
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }

        [Category("Destructable")]
        public AssetID UnknownAssetID74
        {
            get => ReadUInt(0x74);
            set => Write(0x74, value);
        }

        [Category("Destructable")]
        public int UnknownInt78
        {
            get => ReadInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt7C
        {
            get => ReadInt(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt80
        {
            get => ReadInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt84
        {
            get => ReadInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Destructable")]
        public int UnknownInt88
        {
            get => ReadInt(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }
    }
}