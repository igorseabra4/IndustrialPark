using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSURF : ObjectAsset
    {
        public AssetSURF(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x12C;

        public override bool HasReference(uint assetID)
        {
            if (UnknownAssetID1C == assetID)
                return true;
            if (GroupAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Surface")]
        public byte UnknownByte08
        {
            get => ReadByte(0x08);
            set => Write(0x08, value);
        }

        [Category("Surface")]
        public byte UnknownByte09
        {
            get => ReadByte(0x09);
            set => Write(0x09, value);
        }

        [Category("Surface")]
        public byte UnknownByte0A
        {
            get => ReadByte(0x0A);
            set => Write(0x0A, value);
        }

        [Category("Surface")]
        public byte UnknownByte0B
        {
            get => ReadByte(0x0B);
            set => Write(0x0B, value);
        }

        [Category("Surface")]
        public byte UnknownByte0C
        {
            get => ReadByte(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Surface")]
        public byte UnknownByte0D
        {
            get => ReadByte(0x0D);
            set => Write(0x0D, value);
        }

        [Category("Surface")]
        public byte UnknownByte0E
        {
            get => ReadByte(0x0E);
            set => Write(0x0E, value);
        }

        [Category("Surface")]
        public byte UnknownByte0F
        {
            get => ReadByte(0x0F);
            set => Write(0x0F, value);
        }

        [Category("Surface")]
        public float UnknownFloat10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Surface")]
        public int UnknownInt14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Surface")]
        public int UnknownInt18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category("Surface")]
        public AssetID UnknownAssetID1C
        {
            get => ReadUInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Surface")]
        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category("Surface")]
        public int UnknownInt24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }

        [Category("Surface")]
        public int UnknownInt28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }

        [Category("Surface")]
        public short UnknownShort2C
        {
            get => ReadShort(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Surface")]
        public short UnknownShort2E
        {
            get => ReadShort(0x2E);
            set => Write(0x2E, value);
        }

        [Category("Surface")]
        public float UnknownFloat30
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        [Category("Surface")]
        public int UnknownInt34
        {
            get => ReadInt(0x34);
            set => Write(0x34, value);
        }

        [Category("Surface")]
        public int UnknownInt38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }

        [Category("Surface")]
        public AssetID GroupAssetID
        {
            get => ReadUInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Surface")]
        public float UnknownFloat40
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }

        [Category("Surface")]
        public float UnknownFloat44
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }

        [Category("Surface")]
        public float UnknownFloat48
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }

        [Category("Surface")]
        public float UnknownFloat4C
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Surface")]
        public int UnknownInt50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category("Surface")]
        public int UnknownInt54
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }

        [Category("Surface")]
        public int UnknownInt58
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }

        [Category("Surface")]
        public int UnknownInt5C
        {
            get => ReadInt(0x5C);
            set => Write(0x5C, value);
        }

        [Category("Surface")]
        public int UnknownInt60
        {
            get => ReadInt(0x60);
            set => Write(0x60, value);
        }

        [Category("Surface")]
        public int UnknownInt64
        {
            get => ReadInt(0x64);
            set => Write(0x64, value);
        }

        [Category("Surface")]
        public int UnknownInt68
        {
            get => ReadInt(0x68);
            set => Write(0x68, value);
        }

        [Category("Surface")]
        public float UVAnimation_X
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }

        [Category("Surface")]
        public float UVAnimation_Y
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }

        [Category("Surface")]
        public float UnknownFloat74
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }

        [Category("Surface")]
        public float UnknownFloat78
        {
            get => ReadFloat(0x78);
            set => Write(0x78, value);
        }

        [Category("Surface")]
        public float UnknownFloat7C
        {
            get => ReadFloat(0x7C);
            set => Write(0x7C, value);
        }

        [Category("Surface")]
        public float UnknownFloat80
        {
            get => ReadFloat(0x80);
            set => Write(0x80, value);
        }

        [Category("Surface")]
        public float UnknownFloat84
        {
            get => ReadFloat(0x84);
            set => Write(0x84, value);
        }

        [Category("Surface")]
        public float UnknownFloat88
        {
            get => ReadFloat(0x88);
            set => Write(0x88, value);
        }

        [Category("Surface")]
        public float UnknownFloat8C
        {
            get => ReadFloat(0x8C);
            set => Write(0x8C, value);
        }

        [Category("Surface")]
        public float UnknownFloat90
        {
            get => ReadFloat(0x90);
            set => Write(0x90, value);
        }

        [Category("Surface")]
        public float UnknownFloat94
        {
            get => ReadFloat(0x94);
            set => Write(0x94, value);
        }

        [Category("Surface")]
        public float UnknownFloat98
        {
            get => ReadFloat(0x98);
            set => Write(0x98, value);
        }

        [Category("Surface")]
        public float UnknownFloat9C
        {
            get => ReadFloat(0x9C);
            set => Write(0x9C, value);
        }

        [Category("Surface")]
        public float UnknownFloatA0
        {
            get => ReadFloat(0xA0);
            set => Write(0xA0, value);
        }

        [Category("Surface")]
        public float UnknownFloatA4
        {
            get => ReadFloat(0xA4);
            set => Write(0xA4, value);
        }

        [Category("Surface")]
        public float UnknownFloatA8
        {
            get => ReadFloat(0xA8);
            set => Write(0xA8, value);
        }

        [Category("Surface")]
        public float UnknownFloatAC
        {
            get => ReadFloat(0xAC);
            set => Write(0xAC, value);
        }

        [Category("Surface")]
        public float UnknownFloatB0
        {
            get => ReadFloat(0xB0);
            set => Write(0xB0, value);
        }

        [Category("Surface")]
        public byte UnknownByteB4
        {
            get => ReadByte(0xB4);
            set => Write(0xB4, value);
        }

        [Category("Surface")]
        public byte UnknownByteB5
        {
            get => ReadByte(0xB5);
            set => Write(0xB5, value);
        }

        [Category("Surface")]
        public byte UnknownByteB6
        {
            get => ReadByte(0xB6);
            set => Write(0xB6, value);
        }

        [Category("Surface")]
        public byte UnknownByteB7
        {
            get => ReadByte(0xB7);
            set => Write(0xB7, value);
        }

        [Category("Surface")]
        public float UnknownFloatB8
        {
            get => ReadFloat(0xB8);
            set => Write(0xB8, value);
        }

        [Category("Surface")]
        public float UnknownFloatBC
        {
            get => ReadFloat(0xBC);
            set => Write(0xBC, value);
        }

        [Category("Surface")]
        public float UnknownFloatC0
        {
            get => ReadFloat(0xC0);
            set => Write(0xC0, value);
        }

        [Category("Surface")]
        public float UnknownFloatC4
        {
            get => ReadFloat(0xC4);
            set => Write(0xC4, value);
        }

        [Category("Surface")]
        public float UnknownFloatC8
        {
            get => ReadFloat(0xC8);
            set => Write(0xC8, value);
        }
    }
}