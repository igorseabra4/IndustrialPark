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
            return JSPInfo_AssetID == assetID || Group_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            if (JSPInfo_AssetID == 0)
                result.Add("JSP Extra Data with no JSPInfo reference");
            Asset.Verify(JSPInfo_AssetID, ref result);
            if (Group_AssetID == 0)
                result.Add("JSP Extra Data with no GRUP reference");
            Asset.Verify(Group_AssetID, ref result);
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