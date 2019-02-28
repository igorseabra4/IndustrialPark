using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaHudMeterUnit : DynaBase
    {
        public override string Note => "Version is always 3";

        public DynaHudMeterUnit() : base()
        {
            Unknown_AssetID = 0;
            EmptyModel_AssetID = 0;
            FullModel_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (Unknown_AssetID == assetID)
                return true;
            if (EmptyModel_AssetID == assetID)
                return true;
            if (FullModel_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaHudMeterUnit(IEnumerable<byte> enumerable) : base (enumerable)
        {
            PositionX = Switch(BitConverter.ToSingle(Data, 0x0));
            PositionY = Switch(BitConverter.ToSingle(Data, 0x4));
            PositionZ = Switch(BitConverter.ToSingle(Data, 0x8));
            SizeX = Switch(BitConverter.ToSingle(Data, 0xC));
            SizeY = Switch(BitConverter.ToSingle(Data, 0x10));
            SizeZ = Switch(BitConverter.ToSingle(Data, 0x14));
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x18));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x1C));
            UnknownFloat3 = Switch(BitConverter.ToSingle(Data, 0x20));
            UnknownFloat4 = Switch(BitConverter.ToSingle(Data, 0x24));
            UnknownFloat5 = Switch(BitConverter.ToSingle(Data, 0x28));
            Unknown_AssetID = Switch(BitConverter.ToUInt32(Data, 0x2C));
            UnknownFloat6_X = Switch(BitConverter.ToSingle(Data, 0x30));
            UnknownFloat7_Y = Switch(BitConverter.ToSingle(Data, 0x34));
            UnknownFloat8_Z = Switch(BitConverter.ToSingle(Data, 0x38));
            EmptyModel_AssetID = Switch(BitConverter.ToUInt32(Data, 0x3C));
            EmptyOffset_X = Switch(BitConverter.ToSingle(Data, 0x40));
            EmptyOffset_Y = Switch(BitConverter.ToSingle(Data, 0x44));
            EmptyOffset_Z = Switch(BitConverter.ToSingle(Data, 0x48));
            EmptyScale_X = Switch(BitConverter.ToSingle(Data, 0x4C));
            EmptyScale_Y = Switch(BitConverter.ToSingle(Data, 0x50));
            EmptyScale_Z = Switch(BitConverter.ToSingle(Data, 0x54));
            FullModel_AssetID = Switch(BitConverter.ToUInt32(Data, 0x58));
            FullOffset_X = Switch(BitConverter.ToSingle(Data, 0x5C));
            FullOffset_Y = Switch(BitConverter.ToSingle(Data, 0x60));
            FullOffset_Z = Switch(BitConverter.ToSingle(Data, 0x64));
            FullScale_X = Switch(BitConverter.ToSingle(Data, 0x68));
            FullScale_Y = Switch(BitConverter.ToSingle(Data, 0x6C));
            FullScale_Z = Switch(BitConverter.ToSingle(Data, 0x70));
            Spacing_X = Switch(BitConverter.ToSingle(Data, 0x74));
            Spacing_Y = Switch(BitConverter.ToSingle(Data, 0x78));
            Spacing_Z = Switch(BitConverter.ToSingle(Data, 0x7C));
            MeterFillDirection = Switch(BitConverter.ToInt32(Data, 0x80));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(SizeX)));
            list.AddRange(BitConverter.GetBytes(Switch(SizeY)));
            list.AddRange(BitConverter.GetBytes(Switch(SizeZ)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat4)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat5)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown_AssetID)));            
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat6_X)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat7_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat8_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyModel_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyOffset_X)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyOffset_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyOffset_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyScale_X)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyScale_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyScale_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(FullModel_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(FullOffset_X)));
            list.AddRange(BitConverter.GetBytes(Switch(FullOffset_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(FullOffset_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(FullScale_X)));
            list.AddRange(BitConverter.GetBytes(Switch(FullScale_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(FullScale_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(Spacing_X)));
            list.AddRange(BitConverter.GetBytes(Switch(Spacing_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(Spacing_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(MeterFillDirection)));
            return list.ToArray();
        }

        public AssetID Unknown_AssetID { get; set; }
        public AssetID EmptyModel_AssetID { get; set; }
        public AssetID FullModel_AssetID { get; set; }
        public int MeterFillDirection { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeX{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeY{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeZ{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat4{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6_X{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat7_Y{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat8_Z{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_X{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_Y{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_Z{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_X{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_Y{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_Z{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_X{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_Y{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_Z{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_X{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_Y{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_Z{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_X{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_Y{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_Z{ get; set; }
    }
}