using System;
using System.Collections.Generic;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaFlythrough : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaFlythrough() : base()
        {
            FLY_ID = 0;
        }

        public DynaFlythrough(IEnumerable<byte> enumerable) : base (enumerable)
        {
            FLY_ID = Switch(BitConverter.ToUInt32(data, 0x0));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(FLY_ID)));
            return list.ToArray();
        }

        public AssetID FLY_ID { get; set; }
    }
}