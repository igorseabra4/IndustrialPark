using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBoulderGen : DynaBase
    {
        public string Note => "Version is always 1";
        public override int StructSize => 0x38;

        public DynaGObjectBoulderGen(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (ObjectAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(ObjectAssetID, ref result);
        }

        public AssetID ObjectAssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetX
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetY
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetZ
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetRand
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitVelX
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitVelY
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitVelZ
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VelAngleRand
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VelMagRand
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitAxisX
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitAxisY
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitAxisZ
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AngVel
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
    }
}