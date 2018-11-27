using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;
using static IndustrialPark.ConverterFunctions;

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

        public override string ToString()
        {
            return $"[{FrameNumer}] - [{CameraPosition.ToString()}]";
        }
    }

    public class AssetFLY : Asset
    {
        public AssetFLY (Section_AHDR AHDR) : base(AHDR)
        {
        }

        [Category("Flythrough")]
        public EntryFLY[] FLY_Entries
        {
            get
            {
                List<EntryFLY> entries = new List<EntryFLY>();
                BinaryReader binaryReader = new BinaryReader(new MemoryStream(Data));

                while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                {
                    EntryFLY entry = new EntryFLY
                    {
                        FrameNumer = binaryReader.ReadInt32(),
                        CameraNormalizedLeft = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                        CameraNormalizedUp = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                        CameraNormalizedBackward = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                        CameraPosition = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                        Unknown = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle())
                    };

                    entries.Add(entry);
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();

                foreach (EntryFLY i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(i.FrameNumer));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedLeft.X));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedLeft.Y));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedLeft.Z));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedUp.X));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedUp.Y));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedUp.Z));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedBackward.X));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedBackward.Y));
                    newData.AddRange(BitConverter.GetBytes(i.CameraNormalizedBackward.Z));
                    newData.AddRange(BitConverter.GetBytes(i.CameraPosition.X));
                    newData.AddRange(BitConverter.GetBytes(i.CameraPosition.Y));
                    newData.AddRange(BitConverter.GetBytes(i.CameraPosition.Z));
                    newData.AddRange(BitConverter.GetBytes(i.Unknown.X));
                    newData.AddRange(BitConverter.GetBytes(i.Unknown.Y));
                    newData.AddRange(BitConverter.GetBytes(i.Unknown.Z));
                }

                Data = newData.ToArray();
            }
        }
    }
}