using System;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaHudBase : DynaBase
    {
        public DynaHudBase(Platform platform) : base(platform)
        {

        }

        public DynaHudBase(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            PositionX = Switch(BitConverter.ToSingle(Data, 0x0));
            PositionY = Switch(BitConverter.ToSingle(Data, 0x4));
            PositionZ = Switch(BitConverter.ToSingle(Data, 0x8));
            ScaleX = Switch(BitConverter.ToSingle(Data, 0xC));
            ScaleY = Switch(BitConverter.ToSingle(Data, 0x10));
            ScaleZ = Switch(BitConverter.ToSingle(Data, 0x14));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleX)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleY)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleZ)));

            return list.ToArray();
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleZ { get; set; }
    }
}