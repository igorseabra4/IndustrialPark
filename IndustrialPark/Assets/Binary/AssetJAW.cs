using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntryJAW
    {
        public AssetID SoundAssetID { get; set; }
        public byte[] JawData { get; set; }

        public EntryJAW()
        {
            SoundAssetID = 0;
            JawData = new byte[0];
        }

        public EntryJAW(AssetID soundAssetID, byte[] jawData)
        {
            SoundAssetID = soundAssetID;
            JawData = jawData;
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(SoundAssetID)}] - [{JawData.Length}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryJAW entryJAW)
                return SoundAssetID.Equals(entryJAW.SoundAssetID);
            return false;
        }

        public override int GetHashCode()
        {
            return SoundAssetID.GetHashCode();
        }
    }

    public class AssetJAW : Asset
    {
        [Category("Jaw Data")]
        public EntryJAW[] JAW_Entries { get; set; }

        public AssetJAW(Section_AHDR AHDR, Platform platform) : base(AHDR)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, platform))
            {
                JAW_Entries = new EntryJAW[reader.ReadInt32()];

                int startOfJawData = 4 + 12 * JAW_Entries.Length;

                for (int i = 0; i < JAW_Entries.Length; i++)
                {
                    uint soundAssetID = reader.ReadUInt32();
                    int offset = reader.ReadInt32();

                    int length = BitConverter.ToInt32(AHDR.data, startOfJawData + offset);
                    byte[] jawData = AHDR.data.Skip(startOfJawData + offset + 4).Take(length).ToArray();

                    JAW_Entries[i] = new EntryJAW(soundAssetID, jawData);
                }
            }
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            List<byte> newJawData = new List<byte>();

            writer.Write(JAW_Entries.Length);

            foreach (var i in JAW_Entries)
            {
                writer.Write(i.SoundAssetID);
                writer.Write(newJawData.Count);
                writer.Write(i.JawData.Length + 4);

                newJawData.AddRange(BitConverter.GetBytes(i.JawData.Length));
                newJawData.AddRange(i.JawData);

                while (newJawData.Count % 4 != 0)
                    newJawData.Add(0);
            }

            writer.Write(newJawData.ToArray());

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var a in JAW_Entries)
                if (a.SoundAssetID == assetID)
                    return true;

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryJAW a in JAW_Entries)
            {
                if (a.SoundAssetID == 0)
                    result.Add("JAW entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }
        }

        public void AddEntry(byte[] jawData, uint assetID)
        {
            List<EntryJAW> entries = JAW_Entries.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID.Equals(assetID))
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