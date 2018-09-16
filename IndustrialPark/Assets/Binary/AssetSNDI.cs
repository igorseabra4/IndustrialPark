using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntrySoundInfo
    {
        [Editor(typeof(SoundHeaderEditor), typeof(UITypeEditor))]
        public byte[] SoundHeader { get; set; }
        public AssetID SoundAssetID { get; set; }

        public static int StructSize = 0x64;

        public EntrySoundInfo()
        {
            SoundHeader = new byte[0];
            SoundAssetID = 0;
        }

        public override string ToString()
        {
            return $"Sound: {Program.MainForm.GetAssetNameFromID(SoundAssetID)}";
        }
    }

    public class AssetSNDI : Asset
    {
        public AssetSNDI(Section_AHDR AHDR) : base(AHDR) { }

        private int Entries_SND_amount
        {
            get => ReadInt(0x0);
            set => Write(0x0, value);
        }
        private int Entries_SNDS_amount
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }
        private int Entries_SND_CIN_amount
        {
            get => ReadInt(0xC);
            set => Write(0xC, value);
        }

        private int Entries_SND_StartOffset
        {
            get => 0x10;
        }
        private int Entries_SNDS_StartOffset
        {
            get => Entries_SND_StartOffset + Entries_SND_amount * EntrySoundInfo.StructSize;
        }
        private int Entries_SND_CIN_StartOffset
        {
            get => Entries_SNDS_StartOffset + Entries_SNDS_amount * EntrySoundInfo.StructSize;
        }

        public EntrySoundInfo[] Entries_SND
        {
            get
            {
                List<EntrySoundInfo> entries = new List<EntrySoundInfo>();

                for (int i = 0; i < Entries_SND_amount; i++)
                {
                    entries.Add(new EntrySoundInfo()
                    {
                        SoundHeader = AHDR.data.Skip(Entries_SND_StartOffset + EntrySoundInfo.StructSize * i).Take(0x60).ToArray(),
                        SoundAssetID = ReadUInt(Entries_SND_StartOffset + EntrySoundInfo.StructSize * i + 0x60)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SND_StartOffset).ToList();
                List<byte> restOfData = Data.Skip(Entries_SNDS_StartOffset).ToList();

                foreach (EntrySoundInfo i in newValues)
                {
                    newData.AddRange(i.SoundHeader);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SoundAssetID)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Entries_SND_amount = newValues.Count;
            }
        }

        public EntrySoundInfo[] Entries_SNDS
        {
            get
            {
                List<EntrySoundInfo> entries = new List<EntrySoundInfo>();

                for (int i = 0; i < Entries_SNDS_amount; i++)
                {
                    entries.Add(new EntrySoundInfo()
                    {
                        SoundHeader = AHDR.data.Skip(Entries_SNDS_StartOffset + EntrySoundInfo.StructSize * i).Take(0x60).ToArray(),
                        SoundAssetID = ReadUInt(Entries_SNDS_StartOffset + EntrySoundInfo.StructSize * i + 0x60)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SNDS_StartOffset).ToList();
                List<byte> restOfData = Data.Skip(Entries_SND_CIN_StartOffset).ToList();

                foreach (EntrySoundInfo i in newValues)
                {
                    newData.AddRange(i.SoundHeader);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SoundAssetID)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Entries_SNDS_amount = newValues.Count;
            }
        }

        public EntrySoundInfo[] Entries_Sound_CIN
        {
            get
            {
                List<EntrySoundInfo> entries = new List<EntrySoundInfo>();

                for (int i = 0; i < Entries_SND_CIN_amount; i++)
                {
                    entries.Add(new EntrySoundInfo()
                    {
                        SoundHeader = AHDR.data.Skip(Entries_SND_CIN_StartOffset + EntrySoundInfo.StructSize * i).Take(0x60).ToArray(),
                        SoundAssetID = ReadUInt(Entries_SND_CIN_StartOffset + EntrySoundInfo.StructSize * i + 0x60)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SND_CIN_StartOffset).ToList();

                foreach (EntrySoundInfo i in newValues)
                {
                    newData.AddRange(i.SoundHeader);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SoundAssetID)));
                }

                Data = newData.ToArray();
                Entries_SND_CIN_amount = newValues.Count;
            }
        }

        public void AddEntry(byte[] headerData, uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entries.Remove(entries[i]);

            entries.Add(new EntrySoundInfo() { SoundAssetID = assetID, SoundHeader = headerData });

            if (assetType == AssetType.SND)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public byte[] GetHeader(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    return entries[i].SoundHeader;

            throw new Exception($"Error: SNDI asset does not contain {assetType.ToString()} sound header for asset [{assetID.ToString("X8")}]");
        }
    }
}