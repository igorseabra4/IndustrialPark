using System;
using System.Collections.Generic;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTeleport_BFBB : DynaBase
    {
        public override string Note => "Version is always 1 or 2. Version 2 doesn't use the Rotation2";

        public DynaTeleport_BFBB() : base()
        {
            MRKR_ID = 0;
            DYNA_Teleport_ID = 0;
        }

        private int version;

        public DynaTeleport_BFBB(IEnumerable<byte> enumerable, int version) : base (enumerable)
        {
            this.version = version;
            MRKR_ID = Switch(BitConverter.ToUInt32(Data, 0x0));
            UnknownInt = Switch(BitConverter.ToInt32(Data, 0x4));
            Rotation1 = Switch(BitConverter.ToInt32(Data, 0x8));
            if (version == 2)
            {
                Rotation2 = Switch(BitConverter.ToInt32(Data, 0xC));
                DYNA_Teleport_ID = Switch(BitConverter.ToUInt32(Data, 0x10));
            }
            else
            {
                DYNA_Teleport_ID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (DYNA_Teleport_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt)));
            list.AddRange(BitConverter.GetBytes(Switch(Rotation1)));
            if (version == 2)
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