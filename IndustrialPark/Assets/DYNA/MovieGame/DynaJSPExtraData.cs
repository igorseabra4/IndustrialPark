using System;
using System.Collections.Generic;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaJSPExtraData : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaJSPExtraData() : base()
        {
            JSPInfo_AssetID = 0;
            Group_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (JSPInfo_AssetID == assetID)
                return true;
            if (Group_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaJSPExtraData(IEnumerable<byte> enumerable) : base (enumerable)
        {
            JSPInfo_AssetID = Switch(BitConverter.ToUInt32(Data, 0x00));
            Group_AssetID = Switch(BitConverter.ToUInt32(Data, 0x04));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(JSPInfo_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Group_AssetID)));
            return list.ToArray();
        }

        public AssetID JSPInfo_AssetID { get; set; }
        public AssetID Group_AssetID { get; set; }
    }
}