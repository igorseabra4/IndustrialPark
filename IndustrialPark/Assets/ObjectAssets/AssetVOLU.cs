using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetVOLU : ObjectAsset
    {
        public AssetVOLU(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x50;
        
        [Category("Volume")]
        public int UnknownInt08
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Volume")]
        public byte UnknownByte0C
        {
            get => ReadByte(0xC);
            set => Write(0xC, value);
        }

        [Category("Volume")]
        public byte UnknownByte0D
        {
            get => ReadByte(0xD);
            set => Write(0xD, value);
        }

        [Category("Volume")]
        public byte UnknownByte0E
        {
            get => ReadByte(0xE);
            set => Write(0xE, value);
        }

        [Category("Volume")]
        public byte UnknownByte0F
        {
            get => ReadByte(0xF);
            set => Write(0xF, value);
        }
        
        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat28
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2C
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat30
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        [Category("Volume")]
        public int UnknownInt34
        {
            get => ReadInt(0x34);
            set => Write(0x34, value);
        }

        [Category("Volume")]
        public int UnknownInt38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }

        [Category("Volume")]
        public int UnknownInt3C
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Volume")]
        public int UnknownInt40
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }

        [Category("Volume")]
        public int UnknownInt44
        {
            get => ReadInt(0x44);
            set => Write(0x44, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat48
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }

        [Category("Volume"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat4C
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
    }
}