using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;

namespace IndustrialPark
{
    public class EntryFLY
    {
        public int FrameNumer { get; set; }
        public Vector3 CameraNormalizedLeft { get; set; }
        public Vector3 CameraNormalizedUp { get; set; }
        public Vector3 CameraNormalizedBackward { get; set; }
        public Vector3 CameraPosition { get; set; }
        public Vector3 Unknown { get; set; }

        public EntryFLY() { }
        public EntryFLY(BinaryReader binaryReader)
        {
            FrameNumer = binaryReader.ReadInt32();
            CameraNormalizedLeft = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            CameraNormalizedUp = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            CameraNormalizedBackward = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            CameraPosition = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
            Unknown = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        }

        public byte[] Serialize()
        {
            List<byte> data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(FrameNumer));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedLeft.X));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedLeft.Y));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedLeft.Z));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedUp.X));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedUp.Y));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedUp.Z));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedBackward.X));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedBackward.Y));
            data.AddRange(BitConverter.GetBytes(CameraNormalizedBackward.Z));
            data.AddRange(BitConverter.GetBytes(CameraPosition.X));
            data.AddRange(BitConverter.GetBytes(CameraPosition.Y));
            data.AddRange(BitConverter.GetBytes(CameraPosition.Z));
            data.AddRange(BitConverter.GetBytes(Unknown.X));
            data.AddRange(BitConverter.GetBytes(Unknown.Y));
            data.AddRange(BitConverter.GetBytes(Unknown.Z));

            return data.ToArray();
        }

        public override string ToString()
        {
            return $"[{FrameNumer}] - [{CameraPosition}]";
        }
    }

    public class AssetFLY : Asset
    {
        [Category("Flythrough")]
        public EntryFLY[] FLY_Entries { get; set; }

        public AssetFLY(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            List<EntryFLY> entries = new List<EntryFLY>();

            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(AHDR.data)))
                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    entries.Add(new EntryFLY(binaryReader));

            FLY_Entries = entries.ToArray();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            List<byte> newData = new List<byte>();

            foreach (EntryFLY i in FLY_Entries)
                newData.AddRange(i.Serialize());

            return newData.ToArray();
        }
    }
}