using HipHopFile;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntryLKIT
    {
        public int Type { get; set; }
        public AssetSingle ColorR { get; set; }
        public AssetSingle ColorG { get; set; }
        public AssetSingle ColorB { get; set; }
        public AssetSingle Unknown04 { get; set; }
        public AssetSingle Unknown05_X { get; set; }
        public AssetSingle Unknown06_Y { get; set; }
        public AssetSingle Unknown07_Z { get; set; }
        public AssetSingle Unknown08 { get; set; }
        public AssetSingle Unknown09_X { get; set; }
        public AssetSingle Unknown10_Y { get; set; }
        public AssetSingle Unknown11_Z { get; set; }
        public AssetSingle Unknown12 { get; set; }
        public AssetSingle Direction_X { get; set; }
        public AssetSingle Direction_Y { get; set; }
        public AssetSingle Direction_Z { get; set; }
        public AssetSingle Unknown16 { get; set; }
        public AssetSingle Unknown17_X { get; set; }
        public AssetSingle Unknown18_Y { get; set; }
        public AssetSingle Unknown19_Z { get; set; }
        public AssetSingle Unknown20 { get; set; }
        public AssetSingle Unknown21_X { get; set; }
        public AssetSingle Unknown22_Y { get; set; }
        public AssetSingle Unknown23_Z { get; set; }

        public EntryLKIT(EndianBinaryReader reader)
        {
            Type = reader.ReadInt32();
            ColorR = reader.ReadSingle();
            ColorG = reader.ReadSingle();
            ColorB = reader.ReadSingle();
            Unknown04 = reader.ReadSingle();
            Unknown05_X = reader.ReadSingle();
            Unknown06_Y = reader.ReadSingle();
            Unknown07_Z = reader.ReadSingle();
            Unknown08 = reader.ReadSingle();
            Unknown09_X = reader.ReadSingle();
            Unknown10_Y = reader.ReadSingle();
            Unknown11_Z = reader.ReadSingle();
            Unknown12 = reader.ReadSingle();
            Direction_X = reader.ReadSingle();
            Direction_Y = reader.ReadSingle();
            Direction_Z = reader.ReadSingle();
            Unknown16 = reader.ReadSingle();
            Unknown17_X = reader.ReadSingle();
            Unknown18_Y = reader.ReadSingle();
            Unknown19_Z = reader.ReadSingle();
            Unknown20 = reader.ReadSingle();
            Unknown21_X = reader.ReadSingle();
            Unknown22_Y = reader.ReadSingle();
            Unknown23_Z = reader.ReadSingle();
        }

        public byte[] Serialize(Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(Type);
            writer.Write(ColorR);
            writer.Write(ColorG);
            writer.Write(ColorB);
            writer.Write(Unknown04);
            writer.Write(Unknown05_X);
            writer.Write(Unknown06_Y);
            writer.Write(Unknown07_Z);
            writer.Write(Unknown08);
            writer.Write(Unknown09_X);
            writer.Write(Unknown10_Y);
            writer.Write(Unknown11_Z);
            writer.Write(Unknown12);
            writer.Write(Direction_X);
            writer.Write(Direction_Y);
            writer.Write(Direction_Z);
            writer.Write(Unknown16);
            writer.Write(Unknown17_X);
            writer.Write(Unknown18_Y);
            writer.Write(Unknown19_Z);
            writer.Write(Unknown20);
            writer.Write(Unknown21_X);
            writer.Write(Unknown22_Y);
            writer.Write(Unknown23_Z);

            return writer.ToArray();
        }
    }

    public class AssetLKIT : Asset
    {
        [Category("Light Kit")]
        public EntryLKIT[] Lights { get; set; }

        public AssetLKIT(string assetName, byte[] data, Platform platform) : base(assetName, AssetType.LKIT)
        {
            var reader = new EndianBinaryReader(data, platform);

            reader.BaseStream.Position = 0x08;
            int lightCount = reader.ReadInt32();
            Lights = new EntryLKIT[lightCount];

            reader.BaseStream.Position = 0x10;
            for (int i = 0; i < lightCount; i++)
                Lights[i] = new EntryLKIT(reader);
        }

        public AssetLKIT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);

            reader.BaseStream.Position = 0x08;
            int lightCount = reader.ReadInt32();
            Lights = new EntryLKIT[lightCount];

            reader.BaseStream.Position = 0x10;
            for (int i = 0; i < lightCount; i++)
                Lights[i] = new EntryLKIT(reader);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.WriteMagic("LKIT");

            writer.Write(0);
            writer.Write(Lights.Length);
            writer.Write(0);

            foreach (var l in Lights)
                writer.Write(l.Serialize(endianness));

            return writer.ToArray();
        }
    }
}