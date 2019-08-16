using System;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaCamTweak : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaCamTweak(Platform platform) : base(platform) { }

        public DynaCamTweak(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            Priority = Switch(BitConverter.ToInt32(Data, 0x0));
            Time = Switch(BitConverter.ToSingle(Data, 0x04));
            PitchAdjust = Switch(BitConverter.ToSingle(Data, 0x08));
            DistAdjust = Switch(BitConverter.ToSingle(Data, 0x0C));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Priority)));
            list.AddRange(BitConverter.GetBytes(Switch(Time)));
            list.AddRange(BitConverter.GetBytes(Switch(PitchAdjust)));
            list.AddRange(BitConverter.GetBytes(Switch(DistAdjust)));
            return list.ToArray();
        }

        public int Priority { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Time { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PitchAdjust { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DistAdjust { get; set; }
    }
}