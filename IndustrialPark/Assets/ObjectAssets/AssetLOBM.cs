using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetLOBM : BaseAsset
    {
        public AssetLOBM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x80;

        private const string categoryName = "LobMaster";

        public override bool HasReference(uint assetID) =>
            Unknown_08 == assetID || PRJT_AssetID == assetID || Unknown_24 == assetID ||
            Unknown_48 == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (PRJT_AssetID == 0)
                result.Add("LOBM with PRJT_AssetID set to 0");
            Verify(PRJT_AssetID, ref result);
        }

        [Category(categoryName)]
        public AssetID Unknown_08
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }

        [Category(categoryName)]
        public AssetID PRJT_AssetID
        {
            get => ReadUInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        [Category(categoryName)]
        public AssetID Unknown_24
        {
            get => ReadUInt(0x24);
            set => Write(0x24, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_28
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_2C
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_30
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_34
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_38
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }

        [Category(categoryName)]
        public int UnknownInt_3C
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_40
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_44
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }

        [Category(categoryName)]
        public AssetID Unknown_48
        {
            get => ReadUInt(0x48);
            set => Write(0x48, value);
        }

        [Category(categoryName)]
        public int UnknownInt_4C
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category(categoryName)]
        public int UnknownInt_50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_54
        {
            get => ReadFloat(0x54);
            set => Write(0x54, value);
        }

        [Category(categoryName)]
        public int UnknownInt_58
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }

        [Category(categoryName)]
        public int UnknownInt_5C
        {
            get => ReadInt(0x5C);
            set => Write(0x5C, value);
        }

        [Category(categoryName)]
        public int UnknownInt_60
        {
            get => ReadInt(0x60);
            set => Write(0x60, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_64
        {
            get => ReadFloat(0x64);
            set => Write(0x64, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_68
        {
            get => ReadFloat(0x68);
            set => Write(0x68, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_6C
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_70
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_74
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_78
        {
            get => ReadFloat(0x78);
            set => Write(0x78, value);
        }

        [Category(categoryName)]
        public int UnknownInt_7C
        {
            get => ReadInt(0x7C);
            set => Write(0x7C, value);
        }
    }
}