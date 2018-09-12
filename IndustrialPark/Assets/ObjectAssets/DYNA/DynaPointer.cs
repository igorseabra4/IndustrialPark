using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaPointer : DynaBase
    {
        public static int SectionSize => 24;
        public override string Note => "Version is always 1";
        
        public DynaPointer() : base()
        {
        }

        public DynaPointer(IEnumerable<byte> enumerable) : base(enumerable)
        {
            PositionX = Switch(BitConverter.ToInt32(data, 0));
            PositionY = Switch(BitConverter.ToInt32(data, 4));
            PositionZ = Switch(BitConverter.ToInt32(data, 8));
            RotationX = Switch(BitConverter.ToInt32(data, 12));
            RotationY = Switch(BitConverter.ToInt32(data, 16));
            RotationZ = Switch(BitConverter.ToInt32(data, 20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(RotationX)));
            list.AddRange(BitConverter.GetBytes(Switch(RotationY)));
            list.AddRange(BitConverter.GetBytes(Switch(RotationZ)));
            return list.ToArray();
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotationX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotationY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RotationZ { get; set; }
    }
}