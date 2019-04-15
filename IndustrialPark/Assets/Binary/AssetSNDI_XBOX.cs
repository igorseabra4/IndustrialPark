using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntrySoundInfo_XBOX
    {
        public short fmtId { get; set; }
        public short fmtChannels { get; set; }
        public int fmtSampleRate { get; set; }
        public int fmtBytesPerSecond { get; set; }
        public short fmtBlockAlignment { get; set; }
        public short fmtBitsPerSample { get; set; }
        public short fmtExtBytes { get; set; }
        public short fmtExtData { get; set; }
        public int dataSize { get; set; }
        public AssetID SoundAssetID { get; set; }

        public static int StructSize = 0x2C;

        public EntrySoundInfo_XBOX()
        {
            SoundAssetID = 0;
        }

        public EntrySoundInfo_XBOX(byte[] Entry, bool mustProcessFromWav, uint assetID = 0)
        {
            if (mustProcessFromWav)
            {
                fmtId = BitConverter.ToInt16(Entry, 0x00 + 0x14);
                fmtChannels = BitConverter.ToInt16(Entry, 0x02 + 0x14);
                fmtSampleRate = BitConverter.ToInt32(Entry, 0x04 + 0x14);
                fmtBytesPerSecond = BitConverter.ToInt32(Entry, 0x08 + 0x14);
                fmtBlockAlignment = BitConverter.ToInt16(Entry, 0x0C + 0x14);
                fmtBitsPerSample = BitConverter.ToInt16(Entry, 0x0E + 0x14);
                fmtExtBytes = BitConverter.ToInt16(Entry, 0x10 + 0x14);
                fmtExtData = BitConverter.ToInt16(Entry, 0x12 + 0x14);
                dataSize = BitConverter.ToInt32(Entry, 0x14 + 0x18);
                SoundAssetID = assetID;
            }
            else
            {
                fmtId = BitConverter.ToInt16(Entry, 0x00);
                fmtChannels = BitConverter.ToInt16(Entry, 0x02);
                fmtSampleRate = BitConverter.ToInt32(Entry, 0x04);
                fmtBytesPerSecond = BitConverter.ToInt32(Entry, 0x08);
                fmtBlockAlignment = BitConverter.ToInt16(Entry, 0x0C);
                fmtBitsPerSample = BitConverter.ToInt16(Entry, 0x0E);
                fmtExtBytes = BitConverter.ToInt16(Entry, 0x10);
                fmtExtData = BitConverter.ToInt16(Entry, 0x12);
                dataSize = BitConverter.ToInt32(Entry, 0x14);
                SoundAssetID = BitConverter.ToUInt32(Entry, 0x18);
            }
        }

        public byte[] ToByteArray()
        {
            List<byte> array = new List<byte>();

            array.AddRange(BitConverter.GetBytes(fmtId));
            array.AddRange(BitConverter.GetBytes(fmtChannels));
            array.AddRange(BitConverter.GetBytes(fmtSampleRate));
            array.AddRange(BitConverter.GetBytes(fmtBytesPerSecond));
            array.AddRange(BitConverter.GetBytes(fmtBlockAlignment));
            array.AddRange(BitConverter.GetBytes(fmtBitsPerSample));
            array.AddRange(BitConverter.GetBytes(fmtExtBytes));
            array.AddRange(BitConverter.GetBytes(fmtExtData));
            array.AddRange(BitConverter.GetBytes(dataSize));
            array.AddRange(BitConverter.GetBytes(SoundAssetID));
            array.AddRange(new byte[16]);

            return array.ToArray();
        }

        public override string ToString()
        {
            return Program.MainForm.GetAssetNameFromID(SoundAssetID);
        }
    }

    public class AssetSNDI_XBOX : Asset
    {
        public AssetSNDI_XBOX(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySoundInfo_XBOX a in Entries_SND)
            {
                if (a.SoundAssetID == assetID)
                    return true;
            }
            foreach (EntrySoundInfo_XBOX a in Entries_SNDS)
            {
                if (a.SoundAssetID == assetID)
                    return true;
            }
            foreach (EntrySoundInfo_XBOX a in Entries_Sound_CIN)
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
        private int Entries_SND_CIN_amount
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        private int Entries_SND_StartOffset
        {
            get => 0xC;
        }
        private int Entries_SNDS_StartOffset
        {
            get => Entries_SND_StartOffset + Entries_SND_amount * EntrySoundInfo_XBOX.StructSize;
        }
        private int Entries_SND_CIN_StartOffset
        {
            get => Entries_SNDS_StartOffset + Entries_SNDS_amount * EntrySoundInfo_XBOX.StructSize;
        }

        [Category("Sound Info")]
        public EntrySoundInfo_XBOX[] Entries_SND
        {
            get
            {
                List<EntrySoundInfo_XBOX> entries = new List<EntrySoundInfo_XBOX>();

                for (int i = 0; i < Entries_SND_amount; i++)
                    entries.Add(new EntrySoundInfo_XBOX(AHDR.data.Skip(Entries_SND_StartOffset + EntrySoundInfo_XBOX.StructSize * i).Take(EntrySoundInfo_XBOX.StructSize).ToArray(), false));
                
                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo_XBOX> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SND_StartOffset).ToList();
                List<byte> restOfData = Data.Skip(Entries_SNDS_StartOffset).ToList();

                foreach (EntrySoundInfo_XBOX i in newValues)
                    newData.AddRange(i.ToByteArray());

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Entries_SND_amount = newValues.Count;
            }
        }

        [Category("Sound Info")]
        public EntrySoundInfo_XBOX[] Entries_SNDS
        {
            get
            {
                List<EntrySoundInfo_XBOX> entries = new List<EntrySoundInfo_XBOX>();

                for (int i = 0; i < Entries_SNDS_amount; i++)
                    entries.Add(new EntrySoundInfo_XBOX(AHDR.data.Skip(Entries_SNDS_StartOffset + EntrySoundInfo_XBOX.StructSize * i).Take(EntrySoundInfo_XBOX.StructSize).ToArray(), false));

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo_XBOX> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SNDS_StartOffset).ToList();
                List<byte> restOfData = Data.Skip(Entries_SND_CIN_StartOffset).ToList();

                foreach (EntrySoundInfo_XBOX i in newValues)
                    newData.AddRange(i.ToByteArray());

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Entries_SNDS_amount = newValues.Count;
            }
        }

        [Category("Sound Info")]
        public EntrySoundInfo_XBOX[] Entries_Sound_CIN
        {
            get
            {
                List<EntrySoundInfo_XBOX> entries = new List<EntrySoundInfo_XBOX>();

                for (int i = 0; i < Entries_SND_CIN_amount; i++)
                    entries.Add(new EntrySoundInfo_XBOX(AHDR.data.Skip(Entries_SND_CIN_StartOffset + EntrySoundInfo_XBOX.StructSize * i).Take(EntrySoundInfo_XBOX.StructSize).ToArray(), false));

                return entries.ToArray();
            }
            set
            {
                List<EntrySoundInfo_XBOX> newValues = value.ToList();

                List<byte> newData = Data.Take(Entries_SND_CIN_StartOffset).ToList();

                foreach (EntrySoundInfo_XBOX i in newValues)
                    newData.AddRange(i.ToByteArray());

                Data = newData.ToArray();
                Entries_SND_CIN_amount = newValues.Count;
            }
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            RemoveEntry(assetID, assetType);

            List<EntrySoundInfo_XBOX> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            entries.Add(new EntrySoundInfo_XBOX(soundData, true, assetID));

            finalData = soundData.Skip(0x30).ToArray();

            if (assetType == AssetType.SND)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public void RemoveEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_XBOX> entries;
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
            List<EntrySoundInfo_XBOX> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            EntrySoundInfo_XBOX entry = null;

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entry = entries[i];

            if (entry == null)
                throw new Exception($"Error: SNDI asset does not contain {assetType.ToString()} sound header for asset [{assetID.ToString("X8")}]");
            else
            {
                List<byte> bytes = new List<byte>
                {
                    (byte)'R', (byte)'I', (byte)'F', (byte)'F',
                    0, 0, 0, 0,
                    (byte)'W', (byte)'A', (byte)'V', (byte)'E',
                    (byte)'f', (byte)'m', (byte)'t', (byte)' ',
                    0x14, 0, 0, 0
                };

                if (entry.fmtId == 0x0001)
                {
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtId));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtChannels));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtSampleRate));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtBytesPerSecond));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtBlockAlignment));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtBitsPerSample));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtExtBytes));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtExtData));
                }
                else if (entry.fmtId == 0x0069)
                {
                    bytes.AddRange(BitConverter.GetBytes((short)0x0011));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtChannels));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtSampleRate * 65 / 64));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtBytesPerSecond));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtBlockAlignment));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtBitsPerSample));
                    bytes.AddRange(BitConverter.GetBytes(entry.fmtExtBytes));
                    bytes.AddRange(BitConverter.GetBytes((short)0x0041));
                }

                bytes.Add((byte)'d');
                bytes.Add((byte)'a');
                bytes.Add((byte)'t');
                bytes.Add((byte)'a');
                bytes.AddRange(BitConverter.GetBytes(entry.dataSize));

                return bytes.ToArray();
            }
        }

        public void Merge(AssetSNDI_XBOX assetSNDI)
        {
            List<EntrySoundInfo_XBOX> entriesSND = Entries_SND.ToList();
            entriesSND.AddRange(assetSNDI.Entries_SND);
            Entries_SND = entriesSND.ToArray();
            List<EntrySoundInfo_XBOX> entriesSNDS = Entries_SNDS.ToList();
            entriesSNDS.AddRange(assetSNDI.Entries_SNDS);
            Entries_SNDS = entriesSNDS.ToArray();
            List<EntrySoundInfo_XBOX> entriesSoundCIN = Entries_Sound_CIN.ToList();
            entriesSoundCIN.AddRange(assetSNDI.Entries_Sound_CIN);
            Entries_Sound_CIN = entriesSoundCIN.ToArray();
        }
    }
}