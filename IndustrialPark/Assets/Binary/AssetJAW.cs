using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntryJAW : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Sound { get; set; }
        public int Flags { get; set; }
        public byte[] JawData { get; set; }

        public EntryJAW()
        {
            JawData = new byte[0];
        }
        public EntryJAW(AssetID soundAssetID, byte[] jawData)
        {
            Sound = soundAssetID;
            JawData = jawData;
        }

        public EntryJAW(AssetID soundAssetID, int flags, byte[] jawData) : this(soundAssetID, jawData)
        {
            Flags = flags;
        }

        public override string ToString()
        {
            return $"[{HexUIntTypeConverter.StringFromAssetID(Sound)}] - [{JawData.Length}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryJAW entryJAW)
                return Sound.Equals(entryJAW.Sound);
            return false;
        }

        public override int GetHashCode()
        {
            return Sound.GetHashCode();
        }

        public override void Serialize(EndianBinaryWriter writer) { }
    }

    public class AssetJAW : Asset
    {
        public override string AssetInfo => $"{JAW_Entries.Length} entries";

        [Category("Jaw Data")]
        public EntryJAW[] JAW_Entries { get; set; }

        public AssetJAW(string assetName) : base(assetName, AssetType.JawDataTable)
        {
            JAW_Entries = new EntryJAW[0];
        }

        public AssetJAW(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                JAW_Entries = new EntryJAW[reader.ReadInt32()];

                int startOfJawData = 4 + 12 * JAW_Entries.Length;

                for (int i = 0; i < JAW_Entries.Length; i++)
                {
                    uint soundAssetID = reader.ReadUInt32();
                    int offset = reader.ReadInt32();
                    reader.ReadInt32();

                    if (game >= Game.ROTU)
                    {
                        int length = BitConverter.ToInt32(AHDR.data.Skip(startOfJawData + offset).Take(4).Reverse().ToArray(), 0);
                        int flags = BitConverter.ToInt32(AHDR.data.Skip(startOfJawData + offset + 4).Take(4).Reverse().ToArray(), 0);
                        byte[] jawData = AHDR.data.Skip(startOfJawData + offset + 8).Take(length).ToArray();
                        JAW_Entries[i] = new EntryJAW(soundAssetID, flags, jawData);
                    }
                    else
                    {
                        int length = BitConverter.ToInt32(AHDR.data, startOfJawData + offset);
                        byte[] jawData = AHDR.data.Skip(startOfJawData + offset + 4).Take(length).ToArray();
                        JAW_Entries[i] = new EntryJAW(soundAssetID, jawData);
                    }
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            List<byte> newJawData = new List<byte>();

            writer.Write(JAW_Entries.Length);

            foreach (var i in JAW_Entries)
            {
                writer.Write(i.Sound);
                writer.Write(newJawData.Count);

                if (game >= Game.ROTU)
                {
                    writer.Write(i.JawData.Length + 8);
                    newJawData.AddRange(BitConverter.GetBytes(i.JawData.Length).Reverse());
                    newJawData.AddRange(BitConverter.GetBytes(i.Flags).Reverse());
                }
                else
                {
                    writer.Write(i.JawData.Length + 4);
                    newJawData.AddRange(BitConverter.GetBytes(i.JawData.Length));
                }
                newJawData.AddRange(i.JawData);

                while (newJawData.Count % 4 != 0)
                    newJawData.Add(0);
            }

            writer.Write(newJawData.ToArray());
        }

        public void AddEntry(byte[] jawData, uint assetID)
        {
            List<EntryJAW> entries = JAW_Entries.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound.Equals(assetID))
                    entries.RemoveAt(i--);

            entries.Add(new EntryJAW(assetID, jawData));

            JAW_Entries = entries.ToArray();
        }

        public void Merge(AssetJAW asset)
        {
            var entries = JAW_Entries.ToList();

            foreach (var entry in asset.JAW_Entries)
            {
                entries.Remove(entry);
                entries.Add(entry);
            }

            JAW_Entries = entries.ToArray();
        }
    }
}