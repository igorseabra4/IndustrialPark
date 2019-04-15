using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntrySoundInfo_PS2
    {
        public byte[] SoundHeader { get; set; }

        public AssetID SoundAssetID
        {
            get => BitConverter.ToUInt32(SoundHeader, 0x8);
            set
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                SoundHeader[0x8] = byteArray[0];
                SoundHeader[0x9] = byteArray[1];
                SoundHeader[0xA] = byteArray[2];
                SoundHeader[0xB] = byteArray[3];
            }
        }

        public static int StructSize = 0x30;

        public EntrySoundInfo_PS2()
        {
            SoundHeader = new byte[0x30];
        }

        public EntrySoundInfo_PS2(byte[] Entry)
        {
            SoundHeader = Entry;
        }

        public override string ToString()
        {
            return Program.MainForm.GetAssetNameFromID(SoundAssetID);
        }
    }

    public class AssetSNDI_PS2 : Asset
    {
        public AssetSNDI_PS2(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySoundInfo_PS2 a in Entries_SND)
            {
                if (a.SoundAssetID == assetID)
                    return true;
            }
            foreach (EntrySoundInfo_PS2 a in Entries_SNDS)
            {
                if (a.SoundAssetID == assetID)
                    return true;
            }

            return base.HasReference(assetID);
        }

        private int Entries_SND_amount
        {
            get => ReadInt(0x0);
            set => Write(0x0, value);
        }
        private int Entries_SNDS_amount
        {
            get => ReadInt(0x4);
            set => Write(0x4, value);
        }

        private int Entries_SND_StartOffset
        {
            get => 0x8;
        }
        private int Entries_SNDS_StartOffset
        {
            get => Entries_SND_StartOffset + Entries_SND_amount * EntrySoundInfo_PS2.StructSize;
        }

        [Category("Sound Info")]
        public EntrySoundInfo_PS2[] Entries_SND
        {
            get
            {
                List<EntrySoundInfo_PS2> entries = new List<EntrySoundInfo_PS2>();

                for (int i = 0; i < Entries_SND_amount; i++)
                    entries.Add(new EntrySoundInfo_PS2(AHDR.data.Skip(Entries_SND_StartOffset + EntrySoundInfo_PS2.StructSize * i).Take(EntrySoundInfo_PS2.StructSize).ToArray()));
                
                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo_PS2> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SND_StartOffset).ToList();
                List<byte> restOfData = Data.Skip(Entries_SNDS_StartOffset).ToList();

                foreach (EntrySoundInfo_PS2 i in newValues)
                    newData.AddRange(i.SoundHeader);

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Entries_SND_amount = newValues.Count;
            }
        }

        [Category("Sound Info")]
        public EntrySoundInfo_PS2[] Entries_SNDS
        {
            get
            {
                List<EntrySoundInfo_PS2> entries = new List<EntrySoundInfo_PS2>();

                for (int i = 0; i < Entries_SNDS_amount; i++)
                    entries.Add(new EntrySoundInfo_PS2(AHDR.data.Skip(Entries_SNDS_StartOffset + EntrySoundInfo_PS2.StructSize * i).Take(EntrySoundInfo_PS2.StructSize).ToArray()));

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo_PS2> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SNDS_StartOffset).ToList();

                foreach (EntrySoundInfo_PS2 i in newValues)
                    newData.AddRange(i.SoundHeader);

                Data = newData.ToArray();
                Entries_SNDS_amount = newValues.Count;
            }
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            RemoveEntry(assetID, assetType);

            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            entries.Add(new EntrySoundInfo_PS2(soundData.Take(EntrySoundInfo_PS2.StructSize).ToArray()) { SoundAssetID = assetID });

            finalData = soundData.Skip(EntrySoundInfo_PS2.StructSize).ToArray();

            if (assetType == AssetType.SND)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public void RemoveEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entries.Remove(entries[i]);

            if (assetType == AssetType.SND)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public byte[] GetHeader(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    return entries[i].SoundHeader;

            throw new Exception($"Error: SNDI asset does not contain {assetType.ToString()} sound header for asset [{assetID.ToString("X8")}]");
        }

        public void Merge(AssetSNDI_PS2 assetSNDI)
        {
            List<EntrySoundInfo_PS2> entriesSND = Entries_SND.ToList();
            entriesSND.AddRange(assetSNDI.Entries_SND);
            Entries_SND = entriesSND.ToArray();
            List<EntrySoundInfo_PS2> entriesSNDS = Entries_SNDS.ToList();
            entriesSNDS.AddRange(assetSNDI.Entries_SNDS);
            Entries_SNDS = entriesSNDS.ToArray();
        }
    }
}