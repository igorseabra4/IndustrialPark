using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntrySoundInfo_GCN_V1
    {
        [Editor(typeof(SoundHeaderEditor), typeof(UITypeEditor))]
        public byte[] SoundHeader { get; set; }
        public AssetID SoundAssetID { get; set; }

        public static int StructSize = 0x64;

        public EntrySoundInfo_GCN_V1()
        {
            SoundHeader = new byte[0x60];
            SoundAssetID = 0;
        }

        public override string ToString()
        {
            return Program.MainForm.GetAssetNameFromID(SoundAssetID);
        }
    }

    public class AssetSNDI_GCN_V1 : Asset
    {
        public AssetSNDI_GCN_V1(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySoundInfo_GCN_V1 a in Entries_SND)
                if (a.SoundAssetID == assetID)
                    return true;
            
            foreach (EntrySoundInfo_GCN_V1 a in Entries_SNDS)
                if (a.SoundAssetID == assetID)
                    return true;
            
            foreach (EntrySoundInfo_GCN_V1 a in Entries_Sound_CIN)
                if (a.SoundAssetID == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntrySoundInfo_GCN_V1 a in Entries_SND)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }

            foreach (EntrySoundInfo_GCN_V1 a in Entries_SNDS)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }

            foreach (EntrySoundInfo_GCN_V1 a in Entries_Sound_CIN)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }
        }

        private int Entries_SND_amount
        {
            get => ReadInt(0x0);
            set => Write(0x0, value);
        }
        [Browsable(false)]
        public uint Padding
        {
            get => ReadUInt(0x4);
            set => Write(0x4, value);
        }
        private int Entries_SNDS_amount
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }
        private int Entries_SND_CIN_amount
        {
            get
            {
                if (Functions.currentGame == Game.BFBB)
                    return ReadInt(0xC);

                return 0;
            }
            set
            {
                if (Functions.currentGame == Game.BFBB)
                    Write(0xC, value);
            }
        }

        private int Entries_SND_StartOffset
        {
            get
            {
                if (Functions.currentGame == Game.BFBB)
                    return 0x10;
                return 0xC;
            }
        }
        private int Entries_SNDS_StartOffset
        {
            get => Entries_SND_StartOffset + Entries_SND_amount * EntrySoundInfo_GCN_V1.StructSize;
        }
        private int Entries_SND_CIN_StartOffset
        {
            get => Entries_SNDS_StartOffset + Entries_SNDS_amount * EntrySoundInfo_GCN_V1.StructSize;
        }

        [Category("Sound Info")]
        public EntrySoundInfo_GCN_V1[] Entries_SND
        {
            get
            {
                List<EntrySoundInfo_GCN_V1> entries = new List<EntrySoundInfo_GCN_V1>();

                for (int i = 0; i < Entries_SND_amount; i++)
                {
                    entries.Add(new EntrySoundInfo_GCN_V1()
                    {
                        SoundHeader = AHDR.data.Skip(Entries_SND_StartOffset + EntrySoundInfo_GCN_V1.StructSize * i).Take(0x60).ToArray(),
                        SoundAssetID = ReadUInt(Entries_SND_StartOffset + EntrySoundInfo_GCN_V1.StructSize * i + 0x60)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo_GCN_V1> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SND_StartOffset).ToList();
                List<byte> restOfData = Data.Skip(Entries_SNDS_StartOffset).ToList();

                foreach (EntrySoundInfo_GCN_V1 i in newValues)
                {
                    newData.AddRange(i.SoundHeader);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SoundAssetID)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Entries_SND_amount = newValues.Count;
            }
        }

        [Category("Sound Info")]
        public EntrySoundInfo_GCN_V1[] Entries_SNDS
        {
            get
            {
                List<EntrySoundInfo_GCN_V1> entries = new List<EntrySoundInfo_GCN_V1>();

                for (int i = 0; i < Entries_SNDS_amount; i++)
                {
                    entries.Add(new EntrySoundInfo_GCN_V1()
                    {
                        SoundHeader = AHDR.data.Skip(Entries_SNDS_StartOffset + EntrySoundInfo_GCN_V1.StructSize * i).Take(0x60).ToArray(),
                        SoundAssetID = ReadUInt(Entries_SNDS_StartOffset + EntrySoundInfo_GCN_V1.StructSize * i + 0x60)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo_GCN_V1> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SNDS_StartOffset).ToList();
                List<byte> restOfData = Data.Skip(Entries_SND_CIN_StartOffset).ToList();

                foreach (EntrySoundInfo_GCN_V1 i in newValues)
                {
                    newData.AddRange(i.SoundHeader);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SoundAssetID)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Entries_SNDS_amount = newValues.Count;
            }
        }

        [Category("Sound Info")]
        public EntrySoundInfo_GCN_V1[] Entries_Sound_CIN
        {
            get
            {
                if (Functions.currentGame == Game.Scooby)
                    return new EntrySoundInfo_GCN_V1[0];

                List<EntrySoundInfo_GCN_V1> entries = new List<EntrySoundInfo_GCN_V1>();

                for (int i = 0; i < Entries_SND_CIN_amount; i++)
                {
                    entries.Add(new EntrySoundInfo_GCN_V1()
                    {
                        SoundHeader = AHDR.data.Skip(Entries_SND_CIN_StartOffset + EntrySoundInfo_GCN_V1.StructSize * i).Take(0x60).ToArray(),
                        SoundAssetID = ReadUInt(Entries_SND_CIN_StartOffset + EntrySoundInfo_GCN_V1.StructSize * i + 0x60)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                if (Functions.currentGame == Game.Scooby)
                    return;

                List<EntrySoundInfo_GCN_V1> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SND_CIN_StartOffset).ToList();

                foreach (EntrySoundInfo_GCN_V1 i in newValues)
                {
                    newData.AddRange(i.SoundHeader);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SoundAssetID)));
                }

                Data = newData.ToArray();
                Entries_SND_CIN_amount = newValues.Count;
            }
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            List<EntrySoundInfo_GCN_V1> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entries.Remove(entries[i]);

            entries.Add(new EntrySoundInfo_GCN_V1() { SoundAssetID = assetID, SoundHeader = soundData.Take(EntrySoundInfo_GCN_V1.StructSize - 4).ToArray() });

            finalData = soundData.Skip(EntrySoundInfo_GCN_V1.StructSize - 4).ToArray();

            if (assetType == AssetType.SND)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public void RemoveEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_GCN_V1> entries;
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
            List<EntrySoundInfo_GCN_V1> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    return entries[i].SoundHeader;

            entries = Entries_Sound_CIN.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    return entries[i].SoundHeader;

            throw new Exception($"Error: SNDI asset does not contain {assetType.ToString()} sound header for asset [{assetID.ToString("X8")}]");
        }

        public void Merge(AssetSNDI_GCN_V1 assetSNDI)
        {
            {
                // SND
                List<EntrySoundInfo_GCN_V1> entriesSND = Entries_SND.ToList();
                List<uint> assetIDsAlreadyPresentSND = new List<uint>();
                foreach (EntrySoundInfo_GCN_V1 entrySND in entriesSND)
                    assetIDsAlreadyPresentSND.Add(entrySND.SoundAssetID);
                foreach (EntrySoundInfo_GCN_V1 entrySND in assetSNDI.Entries_SND)
                    if (!assetIDsAlreadyPresentSND.Contains(entrySND.SoundAssetID))
                        entriesSND.Add(entrySND);
                Entries_SND = entriesSND.ToArray();
            }
            {
                // SNDS
                List<EntrySoundInfo_GCN_V1> entriesSNDS = Entries_SNDS.ToList();
                List<uint> assetIDsAlreadyPresentSNDS = new List<uint>();
                foreach (EntrySoundInfo_GCN_V1 entrySNDS in entriesSNDS)
                    assetIDsAlreadyPresentSNDS.Add(entrySNDS.SoundAssetID);
                foreach (EntrySoundInfo_GCN_V1 entrySNDS in assetSNDI.Entries_SNDS)
                    if (!assetIDsAlreadyPresentSNDS.Contains(entrySNDS.SoundAssetID))
                        entriesSNDS.Add(entrySNDS);
                Entries_SNDS = entriesSNDS.ToArray();
            }
            {
                // Sound_CIN
                List<EntrySoundInfo_GCN_V1> entriesSound_CIN = Entries_Sound_CIN.ToList();
                List<uint> assetIDsAlreadyPresentSound_CIN = new List<uint>();
                foreach (EntrySoundInfo_GCN_V1 entrySound_CIN in entriesSound_CIN)
                    assetIDsAlreadyPresentSound_CIN.Add(entrySound_CIN.SoundAssetID);
                foreach (EntrySoundInfo_GCN_V1 entrySound_CIN in assetSNDI.Entries_Sound_CIN)
                    if (!assetIDsAlreadyPresentSound_CIN.Contains(entrySound_CIN.SoundAssetID))
                        entriesSound_CIN.Add(entrySound_CIN);
                Entries_Sound_CIN = entriesSound_CIN.ToArray();
            }
        }
    }
}