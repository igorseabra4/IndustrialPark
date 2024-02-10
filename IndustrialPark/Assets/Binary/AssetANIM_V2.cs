using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class AssetANIM_KeyFrame_V2 : GenericAssetDataContainer
    {
        public ushort Frame { get; set; }
        public ushort TranIndex { get; set; }
        public short QuatX { get; set; }
        public short QuatY { get; set; }
        public short QuatZ { get; set; }

        public AssetANIM_KeyFrame_V2(EndianBinaryReader reader)
        {
            Frame = reader.ReadUInt16();
            TranIndex = reader.ReadUInt16();
            QuatX = reader.ReadInt16();
            QuatY = reader.ReadInt16();
            QuatZ = reader.ReadInt16();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Frame);
            writer.Write(TranIndex);
            writer.Write(QuatX);
            writer.Write(QuatY);
            writer.Write(QuatZ);
        }
    }
    public class AssetANIM_V2 : Asset
    {
        public override string AssetInfo => $"{(float)Times.Last() / 30:F3} seconds";

        private const string categoryName = "Animation V2";

        [Category(categoryName), ReadOnly(true)]
        public ushort BoneCount { get; set; }
        [Category(categoryName), ReadOnly(true)]
        public ushort TimeCount { get; set; }
        [Category(categoryName), ReadOnly(true)]
        public ushort KeyCount { get; set; }
        [Category(categoryName), ReadOnly(true)]
        public ushort TranCount { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleX { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleY { get; set; }
        [Category(categoryName)]
        public AssetSingle ScaleZ { get; set; }
        [Category(categoryName)]
        public AssetANIM_KeyFrame_V2[] KeyFrames { get; set; }
        [Category(categoryName)]
        public short[] Times { get; set; }
        [Category(categoryName)]
        public short[][] TranslateTable { get; set; }
        [Category(categoryName)]
        public short[][] Offsets { get; set; }

        public AssetANIM_V2(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadUInt64();
                BoneCount = reader.ReadUInt16();
                TimeCount = reader.ReadUInt16();
                KeyCount = reader.ReadUInt16();
                TranCount = reader.ReadUInt16();
                ScaleX = reader.ReadSingle();
                ScaleY = reader.ReadSingle();
                ScaleZ = reader.ReadSingle();

                var keyframes = new List<AssetANIM_KeyFrame_V2>();
                for (int i = 0; i < KeyCount; i++)
                    keyframes.Add(new AssetANIM_KeyFrame_V2(reader));
                KeyFrames = keyframes.ToArray();

                var times = new List<short>();
                for (int i = 0; i < TimeCount; i++)
                    times.Add(reader.ReadInt16());
                Times = times.ToArray();

                if (TimeCount % 2 != 0)
                    reader.ReadUInt16();

                var trantable = new List<short[]>();
                for (int i = 0; i < TranCount; i++)
                {
                    var translate = new List<short>();
                    for (int j = 0; j < 3; j++)
                        translate.Add(reader.ReadInt16());
                    trantable.Add(translate.ToArray());
                }
                TranslateTable = trantable.ToArray();

                if (TranCount % 2 != 0)
                    reader.ReadUInt16();

                var offsets = new List<short[]>();
                for (int i = 0; i < TimeCount - 1; i++)
                {
                    var offset = new List<short>();
                    for (int j = 0; j < BoneCount; j++)
                        offset.Add(reader.ReadInt16());
                    offsets.Add(offset.ToArray());
                }
                Offsets = offsets.ToArray();

                if ((BoneCount * (TimeCount - 1)) % 2 != 0)
                    reader.ReadUInt16();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.WriteMagic("SKB1");
            writer.Write(0);

            if (Offsets.Length > 0)
                writer.Write((ushort)Offsets[0].Length);
            else
                writer.Write((ushort)0);

            writer.Write((ushort)Times.Length);
            writer.Write((ushort)KeyFrames.Length);
            writer.Write((ushort)TranslateTable.Length);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
            writer.Write(ScaleZ);

            foreach (var k in KeyFrames)
                k.Serialize(writer);
            foreach (var t in Times)
                writer.Write(t);

            if (Times.Length % 2 != 0)
                writer.Write((ushort)0xCDCD);

            foreach (var tt in TranslateTable)
                foreach (var t in tt)
                    writer.Write(t);

            if (TranslateTable.Length % 2 != 0)
                writer.Write((ushort)0xCDCD);

            foreach (var o in Offsets)
                foreach (var of in o)
                    writer.Write(of);

            if (((Times.Length - 1) * BoneCount) % 2 != 0)
                writer.Write((ushort)0xCDCD);
        }
    }
}