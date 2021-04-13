using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class AssetDEST : Asset
    {
        private const string categoryName = "Destructible";

        [Category(categoryName)]
        public AssetID MINF_AssetID { get; set; }
        [Category(categoryName)]
        public int UnknownInt_04 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_08 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_0C { get; set; }
        [Category(categoryName)]
        public int UnknownInt_10 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_14 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_18 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_1C { get; set; }
        [Category(categoryName)]
        public int UnknownInt_20 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte_24 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte_25 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte_26 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte_27 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_28 { get; set; }
        [Category(categoryName)]
        public AssetID Model_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID SHRP_AssetID { get; set; }
        [Category(categoryName)]
        public int UnknownInt_34 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_38 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_3C { get; set; }
        [Category(categoryName)]
        public int UnknownInt_40 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_44 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_48 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_4C { get; set; }
        [Category(categoryName)]
        public int UnknownInt_50 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_54 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_58 { get; set; }
        [Category(categoryName)]
        public AssetID UnknownInt_5C { get; set; }

        public AssetDEST(Section_AHDR AHDR, Platform platform) : base(AHDR)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            MINF_AssetID = reader.ReadUInt32();
            UnknownInt_04 = reader.ReadInt32();
            UnknownInt_08 = reader.ReadInt32();
            UnknownInt_0C = reader.ReadInt32();
            UnknownInt_10 = reader.ReadInt32();
            UnknownInt_14 = reader.ReadInt32();
            UnknownInt_18 = reader.ReadInt32();
            UnknownInt_1C = reader.ReadInt32();
            UnknownInt_20 = reader.ReadInt32();
            UnknownByte_24 = reader.ReadByte();
            UnknownByte_25 = reader.ReadByte();
            UnknownByte_26 = reader.ReadByte();
            UnknownByte_27 = reader.ReadByte();
            UnknownInt_28 = reader.ReadInt32();
            Model_AssetID = reader.ReadUInt32();
            SHRP_AssetID = reader.ReadUInt32();
            UnknownInt_34 = reader.ReadInt32();
            UnknownInt_38 = reader.ReadInt32();
            UnknownInt_3C = reader.ReadInt32();
            UnknownInt_40 = reader.ReadInt32();
            UnknownInt_44 = reader.ReadInt32();
            UnknownInt_48 = reader.ReadInt32();
            UnknownInt_4C = reader.ReadInt32();
            UnknownInt_50 = reader.ReadInt32();
            UnknownInt_54 = reader.ReadInt32();
            UnknownInt_58 = reader.ReadInt32();
            UnknownInt_5C = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(MINF_AssetID);
            writer.Write(UnknownInt_04);
            writer.Write(UnknownInt_08);
            writer.Write(UnknownInt_0C);
            writer.Write(UnknownInt_10);
            writer.Write(UnknownInt_14);
            writer.Write(UnknownInt_18);
            writer.Write(UnknownInt_1C);
            writer.Write(UnknownInt_20);
            writer.Write(UnknownByte_24);
            writer.Write(UnknownByte_25);
            writer.Write(UnknownByte_26);
            writer.Write(UnknownByte_27);
            writer.Write(UnknownInt_28);
            writer.Write(Model_AssetID);
            writer.Write(SHRP_AssetID);
            writer.Write(UnknownInt_34);
            writer.Write(UnknownInt_38);
            writer.Write(UnknownInt_3C);
            writer.Write(UnknownInt_40);
            writer.Write(UnknownInt_44);
            writer.Write(UnknownInt_48);
            writer.Write(UnknownInt_4C);
            writer.Write(UnknownInt_50);
            writer.Write(UnknownInt_54);
            writer.Write(UnknownInt_58);
            writer.Write(UnknownInt_5C);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            MINF_AssetID == assetID || Model_AssetID == assetID || SHRP_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Verify(MINF_AssetID, ref result);

            if (MINF_AssetID == 0)
                result.Add("DEST with MINF_AssetID set to 0");
            if (Model_AssetID == 0)
                result.Add("DEST with Model_AssetID set to 0");
            if (SHRP_AssetID == 0)
                result.Add("DEST with SHRP_AssetID set to 0");
        }
    }
}