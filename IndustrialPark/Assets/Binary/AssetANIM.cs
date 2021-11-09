using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetANIM_KeyFrame
    {
        public ushort TimeIndex { get; set; }
        public short RotationX { get; set; }
        public short RotationY { get; set; }
        public short RotationZ { get; set; }
        public short RotationW { get; set; }
        public short PositionX { get; set; }
        public short PositionY { get; set; }
        public short PositionZ { get; set; }

        public AssetANIM_KeyFrame(EndianBinaryReader reader)
        {
            TimeIndex = reader.ReadUInt16();
            RotationX = reader.ReadInt16();
            RotationY = reader.ReadInt16();
            RotationZ = reader.ReadInt16();
            RotationW = reader.ReadInt16();
            PositionX = reader.ReadInt16();
            PositionY = reader.ReadInt16();
            PositionZ = reader.ReadInt16();
        }

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(TimeIndex);
            writer.Write(RotationX);
            writer.Write(RotationY);
            writer.Write(RotationZ);
            writer.Write(RotationW);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
        }
    }

    public class AssetANIM : Asset
    {
        private const string categoryName = "Animation";

        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle ScaleX { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleY { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleZ { get; set; }
        [Category(categoryName)]
        public AssetANIM_KeyFrame[] KeyFrames { get; set; }
        [Category(categoryName)]
        public AssetSingle[] Times { get; set; }
        [Category(categoryName)]
        public short[][] Offsets { get; set; }

        public AssetANIM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
                var boneCount = reader.ReadUInt16();
                var timeCount = reader.ReadUInt16();
                var keyCount = reader.ReadUInt32();
                ScaleX = reader.ReadSingle();
                ScaleY = reader.ReadSingle();
                ScaleZ = reader.ReadSingle();

                var keyFrames = new List<AssetANIM_KeyFrame>();
                for (int i = 0; i < keyCount; i++)
                    keyFrames.Add(new AssetANIM_KeyFrame(reader));
                KeyFrames = keyFrames.ToArray();

                var times = new List<AssetSingle>();
                for (int i = 0; i < timeCount; i++)
                    times.Add(reader.ReadSingle());
                Times = times.ToArray();

                var offsets = new List<short[]>();
                for (int i = 0; i < timeCount - 1; i++)
                {
                    var offset = new List<short>();
                    for (int j = 0; j < boneCount; j++)
                        offset.Add(reader.ReadInt16());
                    offsets.Add(offset.ToArray());
                }
                Offsets = offsets.ToArray();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.WriteMagic("SKB1");
                writer.Write(Flags.FlagValueInt);

                if (Offsets.Length > 0)
                    writer.Write((ushort)Offsets[0].Length);
                else
                    writer.Write((ushort)0);

                writer.Write((ushort)Times.Length);
                writer.Write(KeyFrames.Length);
                writer.Write(ScaleX);
                writer.Write(ScaleY);
                writer.Write(ScaleZ);

                foreach (var k in KeyFrames)
                    k.Serialize(writer);
                foreach (var t in Times)
                    writer.Write(t);
                foreach (var o in Offsets)
                    foreach (var of in o)
                        writer.Write(of);

                while (writer.BaseStream.Length % 4 != 0)
                    writer.Write((byte)0xCD);

                return writer.ToArray();
            }
        }
    }
}