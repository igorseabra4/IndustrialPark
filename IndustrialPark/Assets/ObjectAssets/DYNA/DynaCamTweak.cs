using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaCamTweak : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaCamTweak() : base() { }

        public DynaCamTweak(IEnumerable<byte> enumerable) : base (enumerable)
        {
            UnknownInt = Switch(BitConverter.ToInt32(Data, 0x0));
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x04));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x08));
            UnknownFloat3 = Switch(BitConverter.ToSingle(Data, 0x0C));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            return list.ToArray();
        }

        public int UnknownInt { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3 { get; set; }
    }
}