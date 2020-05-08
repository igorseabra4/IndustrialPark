using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudMeterUnit : DynaHudMeter
    {
        public override int StructSize => 0x84;

        public DynaHudMeterUnit(AssetDYNA asset) : base(asset) { }
        
        public override bool HasReference(uint assetID)
        {
            if (EmptyModel_AssetID == assetID)
                return true;
            if (FullModel_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(EmptyModel_AssetID, ref result);
            Asset.Verify(FullModel_AssetID, ref result);
        }
        
        public AssetID EmptyModel_AssetID
        {
            get => ReadUInt(0x3C);
            set => Write(0x3C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_X
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_Y
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_Z
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_X
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_Y
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_Z
        {
            get => ReadFloat(0x54);
            set => Write(0x54, value);
        }
        public AssetID FullModel_AssetID
        {
            get => ReadUInt(0x58);
            set => Write(0x58, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_X
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_Y
        {
            get => ReadFloat(0x60);
            set => Write(0x60, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_Z
        {
            get => ReadFloat(0x64);
            set => Write(0x64, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_X
        {
            get => ReadFloat(0x68);
            set => Write(0x68, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_Y
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_Z
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_X
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_Y
        {
            get => ReadFloat(0x78);
            set => Write(0x78, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_Z
        {
            get => ReadFloat(0x7C);
            set => Write(0x7C, value);
        }
        public int MeterFillDirection
        {
            get => ReadInt(0x80);
            set => Write(0x80, value);
        }
    }
}