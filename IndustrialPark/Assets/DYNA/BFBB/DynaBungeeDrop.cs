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

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(MRKR_ID, ref result);
        }

        public DynaBungeeDrop(IEnumerable<byte> enumerable) : base(enumerable)
        {
            MRKR_ID = Switch(BitConverter.ToUInt32(Data, 0x0));
            SetViewAngle = Switch(BitConverter.ToInt32(Data, 0x4));
            ViewAngle = Switch(BitConverter.ToSingle(Data, 0x8));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(MRKR_ID)));
            list.AddRange(BitConverter.GetBytes(Switch(SetViewAngle)));
            list.AddRange(BitConverter.GetBytes(Switch(ViewAngle)));
            return list.ToArray();
        }

        public AssetID MRKR_ID { get; set; }
        public int SetViewAngle { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ViewAngle { get; set; }
    }
}