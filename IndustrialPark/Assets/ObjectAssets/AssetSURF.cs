using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSURF : ObjectAsset
    {
        public AssetSURF(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x12C;

        [Category("Surface")]
        public int UnknownInt08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
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
        public int UnknownInt1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Surface")]
        public int UnknownInt20
        {
            get => ReadInt(0x20);
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
        public float UnknownFloat34
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }

        [Category("Surface")]
        public float UnknownFloat38
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }

        [Category("Surface")]
        public float UnknownFloat3C
        {
            get => ReadFloat(0x3C);
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
        public float UnknownFloat100
        {
            get => ReadFloat(0x100);
            set => Write(0x100, value);
        }

        [Category("Surface")]
        public float UnknownFloat104
        {
            get => ReadFloat(0x104);
            set => Write(0x104, value);
        }

        [Category("Surface")]
        public float UnknownFloat108
        {
            get => ReadFloat(0x108);
            set => Write(0x108, value);
        }

        [Category("Surface")]
        public float UnknownFloat10C
        {
            get => ReadFloat(0x10C);
            set => Write(0x10C, value);
        }

        [Category("Surface")]
        public float UnknownFloat110
        {
            get => ReadFloat(0x110);
            set => Write(0x110, value);
        }

        [Category("Surface")]
        public byte UnknownByte114
        {
            get => ReadByte(0x114);
            set => Write(0x114, value);
        }

        [Category("Surface")]
        public byte UnknownByte115
        {
            get => ReadByte(0x115);
            set => Write(0x115, value);
        }

        [Category("Surface")]
        public byte UnknownByte116
        {
            get => ReadByte(0x116);
            set => Write(0x116, value);
        }

        [Category("Surface")]
        public byte UnknownByte117
        {
            get => ReadByte(0x117);
            set => Write(0x117, value);
        }

        [Category("Surface")]
        public float UnknownFloat118
        {
            get => ReadFloat(0x118);
            set => Write(0x118, value);
        }

        [Category("Surface")]
        public float UnknownFloat11C
        {
            get => ReadFloat(0x11C);
            set => Write(0x11C, value);
        }

        [Category("Surface")]
        public float UnknownFloat120
        {
            get => ReadFloat(0x120);
            set => Write(0x120, value);
        }

        [Category("Surface")]
        public float UnknownFloat124
        {
            get => ReadFloat(0x124);
            set => Write(0x124, value);
        }

        [Category("Surface")]
        public float UnknownFloat128
        {
            get => ReadFloat(0x128);
            set => Write(0x128, value);
        }
    }
}