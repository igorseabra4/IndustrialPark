using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialPark
{
    public class FlyFrame : GenericAssetDataContainer
    {
        public int FrameNumer { get; set; }
        public WireVector CameraNormalizedRight { get; set; }
        public WireVector CameraNormalizedUp { get; set; }
        public WireVector CameraNormalizedBackward { get; set; }
        public WireVector CameraPosition { get; set; }
        public AssetSingle ApertureX { get; set; }
        public AssetSingle ApertureY { get; set; }
        public AssetSingle Focal { get; set; }

        public FlyFrame()
        {
            CameraNormalizedRight = new WireVector();
            CameraNormalizedUp = new WireVector();
            CameraNormalizedBackward = new WireVector();
            CameraPosition = new WireVector();
        }

        public FlyFrame(BinaryReader binaryReader)
        {
            FrameNumer = binaryReader.ReadInt32();
            CameraNormalizedRight = new WireVector(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            CameraNormalizedUp = new WireVector(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            CameraNormalizedBackward = new WireVector(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            CameraPosition = new WireVector(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            ApertureX = binaryReader.ReadSingle();
            ApertureY = binaryReader.ReadSingle();
            Focal = binaryReader.ReadSingle();
        }

        public override string ToString()
        {
            return $"[{FrameNumer}] - [{CameraPosition}]";
        }

        public bool NearlySimilar(FlyFrame other)
        {
            return CameraPosition.NearEqual(other.CameraPosition) &&
                CameraNormalizedRight.NearEqual(other.CameraNormalizedRight) &&
                CameraNormalizedUp.NearEqual(other.CameraNormalizedUp) &&
                CameraNormalizedBackward.NearEqual(other.CameraNormalizedBackward);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(FrameNumer);
            writer.Write(CameraNormalizedRight.X);
            writer.Write(CameraNormalizedRight.Y);
            writer.Write(CameraNormalizedRight.Z);
            writer.Write(CameraNormalizedUp.X);
            writer.Write(CameraNormalizedUp.Y);
            writer.Write(CameraNormalizedUp.Z);
            writer.Write(CameraNormalizedBackward.X);
            writer.Write(CameraNormalizedBackward.Y);
            writer.Write(CameraNormalizedBackward.Z);
            writer.Write(CameraPosition.X);
            writer.Write(CameraPosition.Y);
            writer.Write(CameraPosition.Z);
            writer.Write(ApertureX);
            writer.Write(ApertureY);
            writer.Write(Focal);
        }
    }

    public class AssetFLY : Asset
    {
        public override string AssetInfo => $"{Frames.Length} frames";

        [Category("Flythrough")]
        public FlyFrame[] Frames { get; set; }

        public AssetFLY(string assetName) : base(assetName, AssetType.Flythrough)
        {
            Frames = new FlyFrame[0];
        }

        public AssetFLY(Section_AHDR AHDR, Game game) : base(AHDR, game)
        {
            List<FlyFrame> entries = new List<FlyFrame>();

            using (var reader = new BinaryReader(new MemoryStream(AHDR.data)))
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                    entries.Add(new FlyFrame(reader));

            Frames = entries.ToArray();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.endianness = Endianness.Little;

            foreach (var f in Frames)
                f.Serialize(writer);
        }
    }
}