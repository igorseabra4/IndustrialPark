using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaBusStop : DynaBase
    {
        public override string Note => "Version is always 2";

        public DynaBusStop() : base()
        {
            MRKR_ID = 0;
            CAM_ID = 0;
            SIMP_ID = 0;
        }

        public DynaBusStop(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MRKR_ID = Switch(BitConverter.ToUInt32(data, 0x0));
            Player = Switch(BitConverter.ToInt32(data, 0x4));
            CAM_ID = Switch(BitConverter.ToUInt32(data, 0x8));
            SIMP_ID = Switch(BitConverter.ToUInt32(data, 0xC));
            UnknownFloat = Switch(BitConverter.ToSingle(data, 0x10));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(Player)));
            list.AddRange(BitConverter.GetBytes(Switch(CAM_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(SIMP_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat)));
            return list.ToArray();
        }

        public AssetID MRKR_ID { get; set; }
        public int Player { get; set; }
        public AssetID CAM_ID { get; set; }
        public AssetID SIMP_ID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat { get; set; }
    }
}