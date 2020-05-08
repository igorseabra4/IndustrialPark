using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaJSPExtraData : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x8;

        public DynaJSPExtraData(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID) => JSPInfo_AssetID == assetID || Group_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            if (JSPInfo_AssetID == 0)
                result.Add("JSP Extra Data with no JSPInfo reference");
            Asset.Verify(JSPInfo_AssetID, ref result);
            if (Group_AssetID == 0)
                result.Add("JSP Extra Data with no GRUP reference");
            Asset.Verify(Group_AssetID, ref result);
        }
        
        public AssetID JSPInfo_AssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public AssetID Group_AssetID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
    }
}