using Assimp;
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetANIM_KeyFrame : GenericAssetDataContainer
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

        public override void Serialize(EndianBinaryWriter writer)
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

    public class AssetANIM_V1 : Asset
    {
        public override string AssetInfo => $"{(float)Times.Last():F3} seconds";

        private const string categoryName = "Animation";

        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public ushort BoneCount { get; set; }
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

        public AssetANIM_V1()
        {
            KeyFrames = new AssetANIM_KeyFrame[0];
            Times = new AssetSingle[0];
            Offsets = new short[0][];
        }

        public AssetANIM_V1(EndianBinaryReader reader)
        {
            ReadAnim(reader);
        }

        public AssetANIM_V1(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
                ReadAnim(reader);
        }

        public void ReadAnim(EndianBinaryReader reader)
        {
            reader.ReadUInt32();
            Flags.FlagValueInt = reader.ReadUInt32();
            BoneCount = reader.ReadUInt16();
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
                for (int j = 0; j < BoneCount; j++)
                    offset.Add(reader.ReadInt16());
                offsets.Add(offset.ToArray());
            }
            Offsets = offsets.ToArray();
        }

        public override bool HasReference(uint assetID) => false;
        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.WriteMagic("SKB1");
            writer.Write(Flags.FlagValueInt);
            writer.Write(BoneCount);
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

            if (Pad)
                while (writer.BaseStream.Length % 4 != 0)
                    writer.Write((byte)0xCD);
        }

        public bool Pad = true;
    }
}