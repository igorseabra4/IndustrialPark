using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaJSPExtraData : AssetDYNA
    {
        private const string dynaCategoryName = "JSP Extra Data";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID JSPInfo_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Group_AssetID { get; set; }

        public DynaJSPExtraData(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.JSPExtraData, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            JSPInfo_AssetID = reader.ReadUInt32();
            Group_AssetID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(JSPInfo_AssetID);
            writer.Write(Group_AssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => JSPInfo_AssetID == assetID || Group_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            if (JSPInfo_AssetID == 0)
                result.Add("JSP Extra Data with no JSPInfo reference");
            Verify(JSPInfo_AssetID, ref result);
            if (Group_AssetID == 0)
                result.Add("JSP Extra Data with no GRUP reference");
            Verify(Group_AssetID, ref result);
        }
    }
}