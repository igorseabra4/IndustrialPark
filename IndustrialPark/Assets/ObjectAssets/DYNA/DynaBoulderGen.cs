using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaBoulderGen : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaBoulderGen() : base()
        {
            Unknown_ID = 0;
        }

        public DynaBoulderGen(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Unknown_ID = Switch(BitConverter.ToUInt32(Data, 0x0));
            Float1 = Switch(BitConverter.ToSingle(Data, 0x04));
            Float2 = Switch(BitConverter.ToSingle(Data, 0x08));
            Float3 = Switch(BitConverter.ToSingle(Data, 0x0C));
            Float4 = Switch(BitConverter.ToSingle(Data, 0x10));
            Float5 = Switch(BitConverter.ToSingle(Data, 0x14));
            Float6 = Switch(BitConverter.ToSingle(Data, 0x18));
            Float7 = Switch(BitConverter.ToSingle(Data, 0x1C));
            Float8 = Switch(BitConverter.ToSingle(Data, 0x20));
            Float9 = Switch(BitConverter.ToSingle(Data, 0x24));
            Float10 = Switch(BitConverter.ToSingle(Data, 0x28));
            Float11 = Switch(BitConverter.ToSingle(Data, 0x2C));
            Float12 = Switch(BitConverter.ToSingle(Data, 0x30));
            Float13 = Switch(BitConverter.ToSingle(Data, 0x34));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Unknown_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(Float1)));
            list.AddRange(BitConverter.GetBytes(Switch(Float2)));
            list.AddRange(BitConverter.GetBytes(Switch(Float3)));
            list.AddRange(BitConverter.GetBytes(Switch(Float4)));
            list.AddRange(BitConverter.GetBytes(Switch(Float5)));
            list.AddRange(BitConverter.GetBytes(Switch(Float6)));
            list.AddRange(BitConverter.GetBytes(Switch(Float7)));
            list.AddRange(BitConverter.GetBytes(Switch(Float8)));
            list.AddRange(BitConverter.GetBytes(Switch(Float9)));
            list.AddRange(BitConverter.GetBytes(Switch(Float10)));
            list.AddRange(BitConverter.GetBytes(Switch(Float11)));
            list.AddRange(BitConverter.GetBytes(Switch(Float12)));
            list.AddRange(BitConverter.GetBytes(Switch(Float13)));
            return list.ToArray();
        }

        public AssetID Unknown_ID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float3 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float4 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float5 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float6 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float7 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float8 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float9 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float10 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float11 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float12 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float13 { get; set; }
    }
}