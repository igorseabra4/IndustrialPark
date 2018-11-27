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
        public List<byte> JawData { get; set; }

        public EntryJAW()
        {
            SoundAssetID = 0;
            JawData = new List<byte>();
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(SoundAssetID)}] - [{JawData.Count}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryJAW entryJAW)
                return SoundAssetID == entryJAW.SoundAssetID;
            return false;
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

                    entries.Add(new EntryJAW() { SoundAssetID = soundAssetID, JawData = jawData.ToList() });
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
                    newData.AddRange(BitConverter.GetBytes(Switch(i.JawData.Count + 4)));

                    newJawData.AddRange(BitConverter.GetBytes(i.JawData.Count));
                    newJawData.AddRange(i.JawData);

                    while (newJawData.Count % 4 != 0)
                        newJawData.Add(0);
                }

                newData.AddRange(newJawData);

                Data = newData.ToArray();
            }
        }

        public void Merge(AssetJAW assetJAW)
        {
            List<EntryJAW> entriesJAW = JAW_Entries.ToList();
            foreach (EntryJAW entryJAW in assetJAW.JAW_Entries)
                if (!entriesJAW.Contains(entryJAW))
                    entriesJAW.Add(entryJAW);
            JAW_Entries = entriesJAW.ToArray();
        }
    }
}