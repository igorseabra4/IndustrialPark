using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetVOLU : BaseAsset
    {
        private const string categoryName = "Volume";

        [Category(categoryName)]
        public int UnknownInt08 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte0C { get; set; }
        [Category(categoryName)]
        public byte UnknownByte0D { get; set; }
        [Category(categoryName)]
        public byte UnknownByte0E { get; set; }
        [Category(categoryName)]
        public byte UnknownByte0F { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat10 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat14 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat18 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat1C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat20 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat24 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat28 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat2C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat30 { get; set; }
        [Category(categoryName)]
        public int UnknownInt34 { get; set; }
        [Category(categoryName)]
        public int UnknownInt38 { get; set; }
        [Category(categoryName)]
        public int UnknownInt3C { get; set; }
        [Category(categoryName)]
        public int UnknownInt40 { get; set; }
        [Category(categoryName)]
        public int UnknownInt44 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat48 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat4C { get; set; }

        public AssetVOLU(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseHeaderEndPosition;

            UnknownInt08 = reader.ReadInt32();
            UnknownByte0C = reader.ReadByte();
            UnknownByte0D = reader.ReadByte();
            UnknownByte0E = reader.ReadByte();
            UnknownByte0F = reader.ReadByte();
            UnknownFloat10 = reader.ReadSingle();
            UnknownFloat14 = reader.ReadSingle();
            UnknownFloat18 = reader.ReadSingle();
            UnknownFloat1C = reader.ReadSingle();
            UnknownFloat20 = reader.ReadSingle();
            UnknownFloat24 = reader.ReadSingle();
            UnknownFloat28 = reader.ReadSingle();
            UnknownFloat2C = reader.ReadSingle();
            UnknownFloat30 = reader.ReadSingle();
            UnknownInt34 = reader.ReadInt32();
            UnknownInt38 = reader.ReadInt32();
            UnknownInt3C = reader.ReadInt32();
            UnknownInt40 = reader.ReadInt32();
            UnknownInt44 = reader.ReadInt32();
            UnknownFloat48 = reader.ReadSingle();
            UnknownFloat4C = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(UnknownInt08);
            writer.Write(UnknownByte0C);
            writer.Write(UnknownByte0D);
            writer.Write(UnknownByte0E);
            writer.Write(UnknownByte0F);
            writer.Write(UnknownFloat10);
            writer.Write(UnknownFloat14);
            writer.Write(UnknownFloat18);
            writer.Write(UnknownFloat1C);
            writer.Write(UnknownFloat20);
            writer.Write(UnknownFloat24);
            writer.Write(UnknownFloat28);
            writer.Write(UnknownFloat2C);
            writer.Write(UnknownFloat30);
            writer.Write(UnknownInt34);
            writer.Write(UnknownInt38);
            writer.Write(UnknownInt3C);
            writer.Write(UnknownInt40);
            writer.Write(UnknownInt44);
            writer.Write(UnknownFloat48);
            writer.Write(UnknownFloat4C);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }
    }
}