using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialPark
{
    public class FlyFrame
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

        public byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(FrameNumer));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedRight.X));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedRight.Y));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedRight.Z));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedUp.X));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedUp.Y));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedUp.Z));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedBackward.X));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedBackward.Y));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedBackward.Z));
            data.AddRange(BitConverter.GetBytes(CameraPosition.X));
            data.AddRange(BitConverter.GetBytes(CameraPosition.Y));
            data.AddRange(BitConverter.GetBytes(CameraPosition.Z));
            data.AddRange(BitConverter.GetBytes(ApertureX));
            data.AddRange(BitConverter.GetBytes(ApertureY));
            data.AddRange(BitConverter.GetBytes(Focal));

            return data.ToArray();
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

        public AssetFLY(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            List<FlyFrame> entries = new List<FlyFrame>();

            using (var reader = new BinaryReader(new MemoryStream(AHDR.data)))
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                    entries.Add(new FlyFrame(reader));

            Frames = entries.ToArray();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            foreach (var i in Frames)
                writer.Write(i.Serialize());
        }
    }
}