using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.ComponentModel;

namespace IndustrialPark
{
    public class LightKitLight : GenericAssetDataContainer
    {
        public int Type { get; set; }
        public AssetSingle ColorRed { get; set; }
        public AssetSingle ColorGreen { get; set; }
        public AssetSingle ColorBlue { get; set; }
        public AssetSingle ColorAlpha { get; set; }
        public AssetColor ColorRGBA
        {
            get => AssetColor.FromVector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            set
            {
                var val = value.ToVector4();
                ColorRed = val.X;
                ColorGreen = val.Y;
                ColorBlue = val.Z;
                ColorAlpha = val.W;
            }
        }
        public AssetSingle Unknown02_X { get; set; }
        public AssetSingle Unknown02_Y { get; set; }
        public AssetSingle Unknown02_Z { get; set; }
        public AssetSingle Unknown02_W { get; set; }
        public AssetSingle Unknown03_X { get; set; }
        public AssetSingle Unknown03_Y { get; set; }
        public AssetSingle Unknown03_Z { get; set; }
        public AssetSingle Unknown03_W { get; set; }
        public AssetSingle Direction_X { get; set; }
        public AssetSingle Direction_Y { get; set; }
        public AssetSingle Direction_Z { get; set; }
        public AssetSingle Direction_W { get; set; }
        public AssetSingle Unknown05_X { get; set; }
        public AssetSingle Unknown05_Y { get; set; }
        public AssetSingle Unknown05_Z { get; set; }
        public AssetSingle Unknown05_W { get; set; }
        public AssetSingle Radius { get; set; }
        public AssetSingle Angle { get; set; }
        public AssetSingle PlatLight { get; set; }

        public LightKitLight() { }
        public LightKitLight(EndianBinaryReader reader)
        {
            Type = reader.ReadInt32();
            ColorRed = reader.ReadSingle();
            ColorGreen = reader.ReadSingle();
            ColorBlue = reader.ReadSingle();
            ColorAlpha = reader.ReadSingle();
            Unknown02_X = reader.ReadSingle();
            Unknown02_Y = reader.ReadSingle();
            Unknown02_Z = reader.ReadSingle();
            Unknown02_W = reader.ReadSingle();
            Unknown03_X = reader.ReadSingle();
            Unknown03_Y = reader.ReadSingle();
            Unknown03_Z = reader.ReadSingle();
            Unknown03_W = reader.ReadSingle();
            Direction_X = reader.ReadSingle();
            Direction_Y = reader.ReadSingle();
            Direction_Z = reader.ReadSingle();
            Direction_W = reader.ReadSingle();
            Unknown05_X = reader.ReadSingle();
            Unknown05_Y = reader.ReadSingle();
            Unknown05_Z = reader.ReadSingle();
            Unknown05_W = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Angle = reader.ReadSingle();
            PlatLight = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(ColorRed);
            writer.Write(ColorGreen);
            writer.Write(ColorBlue);
            writer.Write(ColorAlpha);
            writer.Write(Unknown02_X);
            writer.Write(Unknown02_Y);
            writer.Write(Unknown02_Z);
            writer.Write(Unknown02_W);
            writer.Write(Unknown03_X);
            writer.Write(Unknown03_Y);
            writer.Write(Unknown03_Z);
            writer.Write(Unknown03_W);
            writer.Write(Direction_X);
            writer.Write(Direction_Y);
            writer.Write(Direction_Z);
            writer.Write(Direction_W);
            writer.Write(Unknown05_X);
            writer.Write(Unknown05_Y);
            writer.Write(Unknown05_Z);
            writer.Write(Unknown05_W);
            writer.Write(Radius);
            writer.Write(Angle);
            writer.Write(PlatLight);
        }
    }

    public class AssetLKIT : Asset
    {
        public override string AssetInfo => $"{Lights.Length} entries";

        private const string categoryName = "Light Kit";
        [Category(categoryName)]
        public AssetID Group { get; set; }
        [Category(categoryName)]
        public LightKitLight[] Lights { get; set; }

        public AssetLKIT(string assetName, byte[] data, Endianness endianness) : base(assetName, AssetType.LightKit)
        {
            Read(data, endianness);
        }

        public AssetLKIT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            Read(AHDR.data, endianness);
        }

        private void Read(byte[] data, Endianness endianness)
        {
            using (var reader = new EndianBinaryReader(data, endianness))
            {
                reader.BaseStream.Position = 0x04;
                Group = reader.ReadUInt32();
                int lightCount = reader.ReadInt32();
                Lights = new LightKitLight[lightCount];

                reader.BaseStream.Position = 0x10;
                for (int i = 0; i < lightCount; i++)
                    Lights[i] = new LightKitLight(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            if (Lights == null)
                return;

            writer.WriteMagic("LKIT");

            writer.Write(Group);
            writer.Write(Lights.Length);
            writer.Write(0);

            foreach (var l in Lights)
                l.Serialize(writer);
        }
    }
}