using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

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

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(SoundAssetID)}] - [{JawData.Length}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryJAW entryJAW)
                return SoundAssetID == entryJAW.SoundAssetID;
            return false;
        }

        public override int GetHashCode()
        {
            return SoundAssetID.GetHashCode();
        }
    }

    public class AssetJAW : Asset
    {
        public AssetJAW(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryJAW a in JAW_Entries)
                if (a.SoundAssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        private int JawDataCount
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        private int StartOfJawData => 4 + 12 * JawDataCount;

        [Category("Jaw Data")]
        public EntryJAW[] JAW_Entries
        {
            get
            {
                List<EntryJAW> entries = new List<EntryJAW>();

                for (int i = 0; i < JawDataCount; i++)
                {
                    uint soundAssetID = ReadUInt(4 + i * 0xC);
                    int offset = ReadInt(8 + i * 0xC);
                    int size = ReadInt(12 + i * 0xC);

                    int length = BitConverter.ToInt32(Data, StartOfJawData + offset);
                    byte[] jawData = Data.Skip(StartOfJawData + offset + 4).Take(length).ToArray();

                    entries.Add(new EntryJAW() { SoundAssetID = soundAssetID, JawData = jawData });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                List<byte> newJawData = new List<byte>();

                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (EntryJAW i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SoundAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(newJawData.Count)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.JawData.Length + 4)));

                    newJawData.AddRange(BitConverter.GetBytes(i.JawData.Length));
                    newJawData.AddRange(i.JawData);

                    while (newJawData.Count % 4 != 0)
                        newJawData.Add(0);
                }

                newData.AddRange(newJawData);

                Data = newData.ToArray();
            }
        }

        public void AddEntry(byte[] jawData, uint assetID)
        {
            RemoveEntry(assetID);

            List<EntryJAW> entries = JAW_Entries.ToList();
            entries.Add(new EntryJAW() { SoundAssetID = assetID, JawData = jawData });

            JAW_Entries = entries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            List<EntryJAW> entries = JAW_Entries.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entries.Remove(entries[i]);

            JAW_Entries = entries.ToArray();
        }

        public void Merge(AssetJAW assetJAW)
        {
            List<EntryJAW> entriesJAW = JAW_Entries.ToList();
            List<uint> assetIDsAlreadyPresent = new List<uint>();

            foreach (EntryJAW entryJAW in entriesJAW)
                assetIDsAlreadyPresent.Add(entryJAW.SoundAssetID);

            foreach (EntryJAW entryJAW in assetJAW.JAW_Entries)
                if (!assetIDsAlreadyPresent.Contains(entryJAW.SoundAssetID))
                    entriesJAW.Add(entryJAW);

            JAW_Entries = entriesJAW.ToArray();
        }
    }
}