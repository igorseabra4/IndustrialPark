using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaSceneProperties : AssetDYNA
    {
        private const string dynaCategoryName = "Scene Properties";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public int UnknownInt00 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt04 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt08 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Flag10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Flag11 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Flag12 { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Flag13 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Sound_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt18 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat1C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat20 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt24 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt28 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt2C { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt30 { get; set; }

        public DynaSceneProperties(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.SceneProperties, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;
            
            UnknownInt00 = reader.ReadInt32();
            UnknownInt04 = reader.ReadInt32();
            UnknownInt08 = reader.ReadInt32();
            UnknownInt0C = reader.ReadInt32();
            Flag10 = reader.ReadByte();
            Flag11 = reader.ReadByte();
            Flag12 = reader.ReadByte();
            Flag13 = reader.ReadByte();
            Sound_AssetID = reader.ReadUInt32();
            UnknownInt18 = reader.ReadInt32();
            UnknownFloat1C = reader.ReadSingle();
            UnknownFloat20 = reader.ReadSingle();
            UnknownInt24 = reader.ReadInt32();
            UnknownInt28 = reader.ReadInt32();
            UnknownInt2C = reader.ReadInt32();
            UnknownInt30 = reader.ReadInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(UnknownInt00);
            writer.Write(UnknownInt04);
            writer.Write(UnknownInt08);
            writer.Write(UnknownInt0C);
            writer.Write(Flag10);
            writer.Write(Flag11);
            writer.Write(Flag12);
            writer.Write(Flag13);
            writer.Write(Sound_AssetID);
            writer.Write(UnknownInt18);
            writer.Write(UnknownFloat1C);
            writer.Write(UnknownFloat20);
            writer.Write(UnknownInt24);
            writer.Write(UnknownInt28);
            writer.Write(UnknownInt2C);
            writer.Write(UnknownInt30);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Sound_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            if (Sound_AssetID == 0)
                result.Add("Scene Properties with no song reference");
            Verify(Sound_AssetID, ref result);
        }
    }
}