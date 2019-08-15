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

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (CAM_ID == assetID)
                return true;
            if (SIMP_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (MRKR_ID == 0)
                result.Add("Bus stop with no MRKR reference");
            Asset.Verify(MRKR_ID, ref result);
            if (CAM_ID == 0)
                result.Add("Bus stop with no CAM reference");
            Asset.Verify(CAM_ID, ref result);
            if (SIMP_ID == 0)
                result.Add("Bus stop with no SIMP reference");
            Asset.Verify(SIMP_ID, ref result);
        }

        public DynaBusStop(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MRKR_ID = Switch(BitConverter.ToUInt32(Data, 0x0));
            Player = (PlayerEnum)Switch(BitConverter.ToInt32(Data, 0x4));
            CAM_ID = Switch(BitConverter.ToUInt32(Data, 0x8));
            SIMP_ID = Switch(BitConverter.ToUInt32(Data, 0xC));
            Delay = Switch(BitConverter.ToSingle(Data, 0x10));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch((int)Player)));
            list.AddRange(BitConverter.GetBytes(Switch(CAM_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(SIMP_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(Delay)));
            return list.ToArray();
        }

        public AssetID MRKR_ID { get; set; }
        public enum PlayerEnum
        {
            Patrick = 0,
            Sandy = 1
        }
        public PlayerEnum Player { get; set; }
        public AssetID CAM_ID { get; set; }
        public AssetID SIMP_ID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Delay { get; set; }
    }
}