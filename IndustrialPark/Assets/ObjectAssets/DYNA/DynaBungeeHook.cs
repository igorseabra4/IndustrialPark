using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaBungeeHook : DynaBase
    {
        public override string Note => "Version is always 13";

        public DynaBungeeHook() : base()
        {
            Unknown_ID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (Unknown_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaBungeeHook(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Unknown_ID = Switch(BitConverter.ToUInt32(data, 0x0));
            Unknown1 = Switch(BitConverter.ToInt32(data, 0x4));
            Unknown2 = Switch(BitConverter.ToInt32(data, 0x8));
            Unknown3 = Switch(BitConverter.ToInt32(data, 0xC));
            UnknownFloat1 = Switch(BitConverter.ToSingle(data, 0x10));
            UnknownFloat2 = Switch(BitConverter.ToSingle(data, 0x14));
            UnknownFloat3 = Switch(BitConverter.ToSingle(data, 0x18));
            UnknownFloat4 = Switch(BitConverter.ToSingle(data, 0x1C));
            UnknownFloat5 = Switch(BitConverter.ToSingle(data, 0x20));
            UnknownFloat6 = Switch(BitConverter.ToSingle(data, 0x24));
            UnknownFloat7 = Switch(BitConverter.ToSingle(data, 0x28));
            UnknownFloat8 = Switch(BitConverter.ToSingle(data, 0x2C));
            UnknownFloat9 = Switch(BitConverter.ToSingle(data, 0x30));
            UnknownFloat10 = Switch(BitConverter.ToSingle(data, 0x34));
            UnknownFloat11 = Switch(BitConverter.ToSingle(data, 0x38));
            UnknownFloat12 = Switch(BitConverter.ToSingle(data, 0x3C));
            UnknownFloat13 = Switch(BitConverter.ToSingle(data, 0x40));
            UnknownFloat14 = Switch(BitConverter.ToSingle(data, 0x44));
            UnknownFloat15 = Switch(BitConverter.ToSingle(data, 0x48));
            UnknownFloat16 = Switch(BitConverter.ToSingle(data, 0x4C));
            UnknownFloat17 = Switch(BitConverter.ToSingle(data, 0x50));
            UnknownFloat18 = Switch(BitConverter.ToSingle(data, 0x54));
            UnknownFloat19 = Switch(BitConverter.ToSingle(data, 0x58));
            UnknownFloat20 = Switch(BitConverter.ToSingle(data, 0x5C));
            UnknownFloat21 = Switch(BitConverter.ToSingle(data, 0x60));
            UnknownFloat22 = Switch(BitConverter.ToSingle(data, 0x64));
            UnknownFloat23 = Switch(BitConverter.ToSingle(data, 0x68));
            UnknownFloat24 = Switch(BitConverter.ToSingle(data, 0x6C));
            UnknownFloat25 = Switch(BitConverter.ToSingle(data, 0x70));
            UnknownFloat26 = Switch(BitConverter.ToSingle(data, 0x74));
            UnknownFloat27 = Switch(BitConverter.ToSingle(data, 0x78));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Unknown_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown1)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown2)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat4)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat5)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat6)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat7)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat8)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat9)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat10)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat11)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat12)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat13)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat14)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat15)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat16)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat17)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat18)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat19)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat20)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat21)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat22)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat23)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat24)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat25)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat26)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat27)));
            return list.ToArray();
        }

        public AssetID Unknown_ID { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat4 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat7 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat8 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat9 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat10 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat11 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat12 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat13 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat14 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat15 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat16 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat17 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat18 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat19 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat21 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat22 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat23 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat24 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat25 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat26 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat27 { get; set; }
    }
}