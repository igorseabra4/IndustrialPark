using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AssetANIM_Header
    {
        private char[] _magic;
        public char[] Magic 
        {
            get => _magic;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("Value must be 4 characters long");
                _magic = value;
            }
        }
        public uint Flags { get; set; }
        public ushort BoneCount;
        public ushort TimeCount;
        public uint KeyCount;
        public AssetSingle ScaleX { get; set; }
        public AssetSingle ScaleY { get; set; }
        public AssetSingle ScaleZ { get; set; }

        public AssetANIM_Header()
        {
            Magic = new char[] { 'S', 'K', 'B', '1' };
        }

        public AssetANIM_Header(EndianBinaryReader reader)
        {
            var chars = reader.ReadChars(4);
            Magic = reader.endianness == Endianness.Big ? chars : chars.Reverse().ToArray();
            Flags = reader.ReadUInt32();
            BoneCount = reader.ReadUInt16();
            TimeCount = reader.ReadUInt16();
            KeyCount = reader.ReadUInt32();
            ScaleX = reader.ReadSingle();
            ScaleY = reader.ReadSingle();
            ScaleZ = reader.ReadSingle();
        }

        public byte[] Serialize(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(writer.endianness == Endianness.Big ? Magic : Magic.Reverse().ToArray());

            writer.Write(Flags);
            writer.Write(BoneCount);
            writer.Write(TimeCount);
            writer.Write(KeyCount);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
            writer.Write(ScaleZ);

            return writer.ToArray();
        }
    }

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

        public byte[] Serialize(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(TimeIndex);
            writer.Write(RotationX);
            writer.Write(RotationY);
            writer.Write(RotationZ);
            writer.Write(RotationW);

            return writer.ToArray();
        }
    }
    
    public class AssetANIM : Asset
    {
        private const string categoryName = "Animation";

        [Category(categoryName)]
        public AssetANIM_Header Header { get; set; }

        [Category(categoryName)]
        public AssetANIM_KeyFrame[] KeyFrames { get; set; }

        [Category(categoryName)]
        public AssetSingle[] Times { get; set; }

        [Category(categoryName)]
        public short[][] Offsets { get; set; }

        public AssetANIM(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            Header = new AssetANIM_Header(reader);

            var keyFrames = new List<AssetANIM_KeyFrame>();
            for (int i = 0; i < Header.KeyCount; i++)
                keyFrames.Add(new AssetANIM_KeyFrame(reader));
            KeyFrames = keyFrames.ToArray();

            var times = new List<AssetSingle>();
            for (int i = 0; i < Header.TimeCount; i++)
                times.Add(reader.ReadSingle());
            Times = times.ToArray();

            var offsets = new List<short[]>();
            for (int i = 0; i < Header.TimeCount - 1; i++)
            {
                var offset = new List<short>();
                for (int j = 0; j < Header.BoneCount; j++)
                    offset.Add(reader.ReadInt16());
                offsets.Add(offset.ToArray());
            }
            Offsets = offsets.ToArray();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            Header.KeyCount = (uint)KeyFrames.Length;
            Header.TimeCount = (ushort)Times.Length;

            if (Offsets.Length > 0)
                Header.BoneCount = (ushort)Offsets[0].Length;
            else
                Header.BoneCount = 0;

            var writer = new EndianBinaryWriter(platform);

            writer.Write(Header.Serialize(platform));
            foreach (var k in KeyFrames)
                writer.Write(k.Serialize(platform));
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