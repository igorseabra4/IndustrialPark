using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaBungeeDrop : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaBungeeDrop() : base()
        {
            MRKR_ID = 0;
        }

        public DynaBungeeDrop(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MRKR_ID = Switch(BitConverter.ToUInt32(data, 0x0));
            Unknown = Switch(BitConverter.ToInt32(data, 0x4));
            MaybeRotation = Switch(BitConverter.ToSingle(data, 0x8));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown)));
            list.AddRange(BitConverter.GetBytes(Switch(MaybeRotation)));
            return list.ToArray();
        }

        public AssetID MRKR_ID { get; set; }
        public int Unknown { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float MaybeRotation { get; set; }
    }
}