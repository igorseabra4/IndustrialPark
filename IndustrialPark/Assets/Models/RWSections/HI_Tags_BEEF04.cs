using HipHopFile;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class HI_Tags_BEEF04 : GenericAssetDataContainer
    {
        public int RenderWareVersion;
        private Platform platform;

        private Vector3 BoundsUpper;
        private Vector3 BoundsLower;

        public AssetSingle Maximum_X
        {
            get => BoundsUpper.X;
            set { BoundsUpper.X = value; }
        }

        public AssetSingle Maximum_Y
        {
            get => BoundsUpper.Y;
            set { BoundsUpper.Y = value; }
        }

        public AssetSingle Maximum_Z
        {
            get => BoundsUpper.Z;
            set { BoundsUpper.Z = value; }
        }

        public AssetSingle Minimum_X
        {
            get => BoundsLower.X;
            set { BoundsLower.X = value; }
        }

        public AssetSingle Minimum_Y
        {
            get => BoundsLower.Y;
            set { BoundsLower.Y = value; }
        }

        public AssetSingle Minimum_Z
        {
            get => BoundsLower.Z;
            set { BoundsLower.Z = value; }
        }

        public HI_Tags_BEEF04(EndianBinaryReader reader, Endianness endianness)
        {
            reader.endianness = Endianness.Little;
            reader.BaseStream.Position += 8;
            RenderWareVersion = reader.ReadInt32();
            reader.endianness = endianness;

            reader.ReadChars(4);

            if (platform == Platform.GameCube)
                reader.endianness = Endianness.Big;

            BoundsUpper = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            BoundsLower = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public HI_Tags_BEEF04(Platform platform)
        {
            this.platform = platform;
            BoundsUpper = new Vector3(-431602080f);
            BoundsLower = new Vector3(-431602080f);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var filestart = writer.BaseStream.Position;
            writer.Write(new byte[12]);

            writer.Write("JSPX");
            writer.Write(BoundsUpper.X);
            writer.Write(BoundsUpper.Y);
            writer.Write(BoundsUpper.Z);
            writer.Write(BoundsLower.X);
            writer.Write(BoundsLower.Y);
            writer.Write(BoundsLower.Z);

            var fileEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = filestart;
            var endian = writer.endianness;
            writer.endianness = Endianness.Little;

            writer.Write((int)RenderWareFile.Section.HI_TAGS_BEEF04);
            writer.Write(0x1C);
            writer.Write(RenderWareVersion);

            writer.BaseStream.Position = fileEnd;
            writer.endianness = endian;
        }
    }
}
