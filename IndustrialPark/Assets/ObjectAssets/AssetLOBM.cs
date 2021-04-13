using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetLOBM : BaseAsset
    {
        private const string categoryName = "LobMaster";

        [Category(categoryName)]
        public AssetID Unknown_08 { get; set; }
        [Category(categoryName)]
        public AssetID PRJT_AssetID { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_1C { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_20 { get; set; }
        [Category(categoryName)]
        public AssetID Unknown_24 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_28 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_2C { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_30 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_34 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_38 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_3C { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_40 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_44 { get; set; }
        [Category(categoryName)]
        public AssetID Unknown_48 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_4C { get; set; }
        [Category(categoryName)]
        public int UnknownInt_50 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_54 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_58 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_5C { get; set; }
        [Category(categoryName)]
        public int UnknownInt_60 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_64 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_68 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_6C { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_70 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_74 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_78 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_7C { get; set; }

        public AssetLOBM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            Unknown_08 = reader.ReadUInt32();
            PRJT_AssetID = reader.ReadUInt32();
            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            PositionZ = reader.ReadSingle();
            UnknownFloat_1C = reader.ReadSingle();
            UnknownFloat_20 = reader.ReadSingle();
            Unknown_24 = reader.ReadUInt32();
            UnknownFloat_28 = reader.ReadSingle();
            UnknownFloat_2C = reader.ReadSingle();
            UnknownFloat_30 = reader.ReadSingle();
            UnknownFloat_34 = reader.ReadSingle();
            UnknownFloat_38 = reader.ReadSingle();
            UnknownInt_3C = reader.ReadInt32();
            UnknownFloat_40 = reader.ReadSingle();
            UnknownFloat_44 = reader.ReadSingle();
            Unknown_48 = reader.ReadUInt32();
            UnknownInt_4C = reader.ReadInt32();
            UnknownInt_50 = reader.ReadInt32();
            UnknownFloat_54 = reader.ReadSingle();
            UnknownInt_58 = reader.ReadInt32();
            UnknownInt_5C = reader.ReadInt32();
            UnknownInt_60 = reader.ReadInt32();
            UnknownFloat_64 = reader.ReadSingle();
            UnknownFloat_68 = reader.ReadSingle();
            UnknownFloat_6C = reader.ReadSingle();
            UnknownFloat_70 = reader.ReadSingle();
            UnknownFloat_74 = reader.ReadSingle();
            UnknownFloat_78 = reader.ReadSingle();
            UnknownInt_7C = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(SerializeBase(platform));
            writer.Write(Unknown_08);
            writer.Write(PRJT_AssetID);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
            writer.Write(UnknownFloat_1C);
            writer.Write(UnknownFloat_20);
            writer.Write(Unknown_24);
            writer.Write(UnknownFloat_28);
            writer.Write(UnknownFloat_2C);
            writer.Write(UnknownFloat_30);
            writer.Write(UnknownFloat_34);
            writer.Write(UnknownFloat_38);
            writer.Write(UnknownInt_3C);
            writer.Write(UnknownFloat_40);
            writer.Write(UnknownFloat_44);
            writer.Write(Unknown_48);
            writer.Write(UnknownInt_4C);
            writer.Write(UnknownInt_50);
            writer.Write(UnknownFloat_54);
            writer.Write(UnknownInt_58);
            writer.Write(UnknownInt_5C);
            writer.Write(UnknownInt_60);
            writer.Write(UnknownFloat_64);
            writer.Write(UnknownFloat_68);
            writer.Write(UnknownFloat_6C);
            writer.Write(UnknownFloat_70);
            writer.Write(UnknownFloat_74);
            writer.Write(UnknownFloat_78);
            writer.Write(UnknownInt_7C);

            writer.Write(SerializeLinks(platform));

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) =>
            Unknown_08 == assetID || PRJT_AssetID == assetID || Unknown_24 == assetID ||
            Unknown_48 == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (PRJT_AssetID == 0)
                result.Add("LOBM with PRJT_AssetID set to 0");
            Verify(PRJT_AssetID, ref result);
        }
    }
}