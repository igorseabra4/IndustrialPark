using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPRJT : BaseAsset
    {
        private const string categoryName = "Projectile";

        [Category(categoryName)]
        public int UnknownInt08 { get; set; }
        [Category(categoryName)]
        public AssetID Model { get; set; }
        [Category(categoryName)]
        public int UnknownInt10 { get; set; }
        [Category(categoryName)]
        public int UnknownInt14 { get; set; }
        [Category(categoryName)]
        public int UnknownInt18 { get; set; }
        [Category(categoryName)]
        public int UnknownInt1C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat20 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat24 { get; set; }
        [Category(categoryName)]
        public int UnknownInt28 { get; set; }
        [Category(categoryName)]
        public int UnknownInt2C { get; set; }
        [Category(categoryName)]
        public int UnknownInt30 { get; set; }
        [Category(categoryName)]
        public int UnknownInt34 { get; set; }
        [Category(categoryName)]
        public int UnknownInt38 { get; set; }
        [Category(categoryName)]
        public int UnknownInt3C { get; set; }
        [Category(categoryName)]
        public int UnknownInt40 { get; set; }

        public AssetPRJT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                UnknownInt08 = reader.ReadInt32();
                Model = reader.ReadUInt32();
                UnknownInt10 = reader.ReadInt32();
                UnknownInt14 = reader.ReadInt32();
                UnknownInt18 = reader.ReadInt32();
                UnknownInt1C = reader.ReadInt32();
                UnknownFloat20 = reader.ReadSingle();
                UnknownFloat24 = reader.ReadSingle();
                UnknownInt28 = reader.ReadInt32();
                UnknownInt2C = reader.ReadInt32();
                UnknownInt30 = reader.ReadInt32();
                UnknownInt34 = reader.ReadInt32();
                UnknownInt38 = reader.ReadInt32();
                UnknownInt3C = reader.ReadInt32();
                UnknownInt40 = reader.ReadInt32();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(UnknownInt08);
                writer.Write(Model);
                writer.Write(UnknownInt10);
                writer.Write(UnknownInt14);
                writer.Write(UnknownInt18);
                writer.Write(UnknownInt1C);
                writer.Write(UnknownFloat20);
                writer.Write(UnknownFloat24);
                writer.Write(UnknownInt28);
                writer.Write(UnknownInt2C);
                writer.Write(UnknownInt30);
                writer.Write(UnknownInt34);
                writer.Write(UnknownInt38);
                writer.Write(UnknownInt3C);
                writer.Write(UnknownInt40);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Model == 0)
                result.Add("Projectile with Model set to 0");
            Verify(Model, ref result);
        }
    }
}