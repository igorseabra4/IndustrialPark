using System;
using System.Collections.Generic;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTeleport : DynaBase
    {
        public override string Note => "Version is always 1 or 2";

        public DynaTeleport() : base()
        {
            MRKR_ID = 0;
            DYNA_Teleport_ID = 0;
        }

        public DynaTeleport(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MRKR_ID = Switch(BitConverter.ToUInt32(data, 0x0));
            UnknownInt = Switch(BitConverter.ToInt32(data, 0x4));
            Rotation1 = Switch(BitConverter.ToInt32(data, 0x8));
            Rotation2 = Switch(BitConverter.ToInt32(data, 0xC));
            DYNA_Teleport_ID = Switch(BitConverter.ToUInt32(data, 0x10));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt)));
            list.AddRange(BitConverter.GetBytes(Switch(Rotation1)));
            list.AddRange(BitConverter.GetBytes(Switch(Rotation2)));
            list.AddRange(BitConverter.GetBytes(Switch(DYNA_Teleport_ID)));
            return list.ToArray();
        }

        public AssetID MRKR_ID { get; set; }
        public int UnknownInt { get; set; }
        public int Rotation1 { get; set; }
        public int Rotation2 { get; set; }
        public AssetID DYNA_Teleport_ID { get; set; }
    }
}