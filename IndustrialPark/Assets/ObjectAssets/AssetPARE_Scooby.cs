using AssetEditorColors;
using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPARE_Scooby : BaseAsset
    {
        private const string categoryName = "Particle Emitter";

        [Category(categoryName)]
        public AssetByte UnknownByte01 { get; set; }
        [Category(categoryName)]
        public AssetByte UnknownByte02 { get; set; }
        [Category(categoryName)]
        public AssetByte UnknownByte03 { get; set; }
        [Category(categoryName)]
        public AssetByte UnknownByte04 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat03 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat04 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat05 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat06 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat07 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat08 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat09 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat10 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat11 { get; set; }
        [Category(categoryName)]
        public AssetID ParticleSystem { get; set; }
        [Category(categoryName)]
        public AssetSingle PositionX { get; set; }
        [Category(categoryName)]
        public AssetSingle PositionY { get; set; }
        [Category(categoryName)]
        public AssetSingle PositionZ { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionX { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionY { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionZ { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat19 { get; set; }
        [Category(categoryName)]
        public AssetColor StartColor { get; set; }
        [Category(categoryName)]
        public AssetColor EndColor { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat22 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat23 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat24 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat25 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat26 { get; set; }
        [Category(categoryName)]
        public AssetByte UnknownByte05 { get; set; }
        [Category(categoryName)]
        public AssetByte UnknownByte06 { get; set; }
        [Category(categoryName)]
        public AssetByte UnknownByte07 { get; set; }
        [Category(categoryName)]
        public AssetByte UnknownByte08 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat28 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat29 { get; set; }

        public AssetPARE_Scooby(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.ParticleEmitter, BaseAssetType.ParticleEmitter)
        {
            PositionX = position.X;
            PositionY = position.Y;
            PositionZ = position.Z;

            if (template == AssetTemplate.Cauldron_Emitter)
            {
                UnknownByte01 = 0x01;
                UnknownByte02 = 0x02;
                UnknownByte03 = 0x03;
                UnknownByte04 = 0x01;
                UnknownFloat03 = 0.1f;
                UnknownFloat04 = 0.5f;
                ParticleSystem = "CAULDRON SYSTEM";
                DirectionY = 1f;
                UnknownFloat19 = 5.2359879e-002f;
                StartColor = new AssetColor(0x20, 0xC8, 0x40, 0xFF);
                EndColor = new AssetColor(0x00, 0x96, 0x40, 0x00);
                UnknownFloat22 = 0.15f;
                UnknownFloat23 = 0.05f;
                UnknownFloat24 = 0.05f;
                UnknownFloat25 = 1.5f;
                UnknownFloat26 = 0.5f;
                UnknownByte07 = 0x03;
                UnknownFloat28 = 100f;
            }
        }

        public AssetPARE_Scooby(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                UnknownByte01 = reader.ReadByte();
                UnknownByte02 = reader.ReadByte();
                UnknownByte03 = reader.ReadByte();
                UnknownByte04 = reader.ReadByte();
                UnknownFloat03 = reader.ReadSingle();
                UnknownFloat04 = reader.ReadSingle();
                UnknownFloat05 = reader.ReadSingle();
                UnknownFloat06 = reader.ReadSingle();
                UnknownFloat07 = reader.ReadSingle();
                UnknownFloat08 = reader.ReadSingle();
                UnknownFloat09 = reader.ReadSingle();
                UnknownFloat10 = reader.ReadSingle();
                UnknownFloat11 = reader.ReadSingle();
                ParticleSystem = reader.ReadUInt32();
                PositionX = reader.ReadSingle();
                PositionY = reader.ReadSingle();
                PositionZ = reader.ReadSingle();
                DirectionX = reader.ReadSingle();
                DirectionY = reader.ReadSingle();
                DirectionZ = reader.ReadSingle();
                UnknownFloat19 = reader.ReadSingle();
                StartColor = reader.ReadColor();
                EndColor = reader.ReadColor();
                UnknownFloat22 = reader.ReadSingle();
                UnknownFloat23 = reader.ReadSingle();
                UnknownFloat24 = reader.ReadSingle();
                UnknownFloat25 = reader.ReadSingle();
                UnknownFloat26 = reader.ReadSingle();
                UnknownByte05 = reader.ReadByte();
                UnknownByte06 = reader.ReadByte();
                UnknownByte07 = reader.ReadByte();
                UnknownByte08 = reader.ReadByte();
                UnknownFloat28 = reader.ReadSingle();
                UnknownFloat29 = reader.ReadSingle();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(UnknownByte01);
                writer.Write(UnknownByte02);
                writer.Write(UnknownByte03);
                writer.Write(UnknownByte04);
                writer.Write(UnknownFloat03);
                writer.Write(UnknownFloat04);
                writer.Write(UnknownFloat05);
                writer.Write(UnknownFloat06);
                writer.Write(UnknownFloat07);
                writer.Write(UnknownFloat08);
                writer.Write(UnknownFloat09);
                writer.Write(UnknownFloat10);
                writer.Write(UnknownFloat11);
                writer.Write(ParticleSystem);
                writer.Write(PositionX);
                writer.Write(PositionY);
                writer.Write(PositionZ);
                writer.Write(DirectionX);
                writer.Write(DirectionY);
                writer.Write(DirectionZ);
                writer.Write(UnknownFloat19);
                writer.Write(StartColor);
                writer.Write(EndColor);
                writer.Write(UnknownFloat22);
                writer.Write(UnknownFloat23);
                writer.Write(UnknownFloat24);
                writer.Write(UnknownFloat25);
                writer.Write(UnknownFloat26);
                writer.Write(UnknownByte05);
                writer.Write(UnknownByte06);
                writer.Write(UnknownByte07);
                writer.Write(UnknownByte08);
                writer.Write(UnknownFloat28);
                writer.Write(UnknownFloat29);
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (ParticleSystem == 0)
                result.Add("Particle Emitter with ParticleSystem set to 0");
            Verify(ParticleSystem, ref result);
        }
    }
}