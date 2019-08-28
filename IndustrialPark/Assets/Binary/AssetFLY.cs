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

        public byte[] ToByteArray()
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
            return $"[{FrameNumer}] - [{CameraPosition.ToString()}]";
        }
    }

    public class AssetFLY : Asset
    {
        public AssetFLY(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override void Verify(ref List<string> result)
        {
            EntryFLY[] entries = FLY_Entries;
        }

        [Category("Flythrough")]
        public EntryFLY[] FLY_Entries
        {
            get
            {
                List<EntryFLY> entries = new List<EntryFLY>();
                using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(Data)))
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                        entries.Add(new EntryFLY
                        {
                            FrameNumer = binaryReader.ReadInt32(),
                            CameraNormalizedLeft = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                            CameraNormalizedUp = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                            CameraNormalizedBackward = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                            CameraPosition = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle()),
                            Unknown = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle())
                        });

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();

                foreach (EntryFLY i in value)
                    newData.AddRange(i.ToByteArray());
                
                Data = newData.ToArray();
            }
        }
    }
}