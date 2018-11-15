using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEGEN : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender { get => dontRender; }
        
        public AssetEGEN(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset { get => 0x6C + Offset; }

        public override bool HasReference(uint assetID)
        {
            if (UnknownAssetID_68 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Electric Arc")]
        public int UnknownInt54
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Electric Arc")]
        public int UnknownInt58
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Electric Arc")]
        public int UnknownInt5C
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Electric Arc")]
        public byte UnknownByte60
        {
            get => ReadByte(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Electric Arc")]
        public byte UnknownByte61
        {
            get => ReadByte(0x61 + Offset);
            set => Write(0x61 + Offset, value);
        }

        [Category("Electric Arc")]
        public byte UnknownByte62
        {
            get => ReadByte(0x62 + Offset);
            set => Write(0x62 + Offset, value);
        }

        [Category("Electric Arc")]
        public byte UnknownByte63
        {
            get => ReadByte(0x63 + Offset);
            set => Write(0x63 + Offset, value);
        }

        [Category("Electric Arc")]
        public float TimeActiveSeconds
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Electric Arc")]
        public AssetID UnknownAssetID_68
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }
    }
}