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

        public EntrySoundInfo_XBOX(byte[] Entry, uint assetID)
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

        public EntrySoundInfo_XBOX(EndianBinaryReader reader)
        {
            fmtId = reader.ReadInt16();
            fmtChannels = reader.ReadInt16();
            fmtSampleRate = reader.ReadInt32();
            fmtBytesPerSecond = reader.ReadInt32();
            fmtBlockAlignment = reader.ReadInt16();
            fmtBitsPerSample = reader.ReadInt16();
            fmtExtBytes = reader.ReadInt16();
            fmtExtData = reader.ReadInt16();
            dataSize = reader.ReadInt32();
            SoundAssetID = reader.ReadUInt32();

            reader.BaseStream.Position += 16;
        }

        public byte[] Serialize()
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

        public byte[] SoundHeader
        { 
            get => Serialize();
            set
            {
                fmtId = BitConverter.ToInt16(value, 0x00);
                fmtChannels = BitConverter.ToInt16(value, 0x02);
                fmtSampleRate = BitConverter.ToInt32(value, 0x04);
                fmtBytesPerSecond = BitConverter.ToInt32(value, 0x08);
                fmtBlockAlignment = BitConverter.ToInt16(value, 0x0C);
                fmtBitsPerSample = BitConverter.ToInt16(value, 0x0E);
                fmtExtBytes = BitConverter.ToInt16(value, 0x10);
                fmtExtData = BitConverter.ToInt16(value, 0x12);
                dataSize = BitConverter.ToInt32(value, 0x14);
            }
        }

        public override string ToString()
        {
            return Program.MainForm.GetAssetNameFromID(SoundAssetID);
        }
    }

    public class AssetSNDI_XBOX : Asset
    {
        private const string categoryName = "Sound Info: Xbox";

        [Category(categoryName)]
        public EntrySoundInfo_XBOX[] Entries_SND { get; set; }

        [Category(categoryName)]
        public EntrySoundInfo_XBOX[] Entries_SNDS { get; set; }

        [Category(categoryName)]
        public EntrySoundInfo_XBOX[] Entries_Sound_CIN { get; set; }

        public AssetSNDI_XBOX(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, Endianness.Little);

            int entriesSndAmount = reader.ReadInt32();
            int entriesSndsAmount = reader.ReadInt32();
            int entriesCinAmount = reader.ReadInt32();

            Entries_SND = new EntrySoundInfo_XBOX[entriesSndAmount];
            for (int i = 0; i < Entries_SND.Length; i++)
                Entries_SND[i] = new EntrySoundInfo_XBOX(reader);

            Entries_SNDS = new EntrySoundInfo_XBOX[entriesSndsAmount];
            for (int i = 0; i < Entries_SNDS.Length; i++)
                Entries_SNDS[i] = new EntrySoundInfo_XBOX(reader);

            Entries_Sound_CIN = new EntrySoundInfo_XBOX[entriesCinAmount];
            for (int i = 0; i < Entries_Sound_CIN.Length; i++)
                Entries_Sound_CIN[i] = new EntrySoundInfo_XBOX(reader);
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySoundInfo_XBOX a in Entries_SND)
                if (a.SoundAssetID == assetID)
                    return true;

            foreach (EntrySoundInfo_XBOX a in Entries_SNDS)
                if (a.SoundAssetID == assetID)
                    return true;

            foreach (EntrySoundInfo_XBOX a in Entries_Sound_CIN)
                if (a.SoundAssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntrySoundInfo_XBOX a in Entries_SND)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }

            foreach (EntrySoundInfo_XBOX a in Entries_SNDS)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }

            foreach (EntrySoundInfo_XBOX a in Entries_Sound_CIN)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
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

            entries.Add(new EntrySoundInfo_XBOX(soundData, assetID));

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
            {
                entries = Entries_Sound_CIN.ToList();

                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].SoundAssetID == assetID)
                        entry = entries[i];
            }

            if (entry == null)
                throw new Exception($"Error: SNDI asset does not contain {assetType.ToString()} sound header for asset [{assetID.ToString("X8")}]");

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

        public void Merge(AssetSNDI_XBOX assetSNDI)
        {
            {
                // SND
                List<EntrySoundInfo_XBOX> entriesSND = Entries_SND.ToList();
                List<uint> assetIDsAlreadyPresentSND = new List<uint>();
                foreach (EntrySoundInfo_XBOX entrySND in entriesSND)
                    assetIDsAlreadyPresentSND.Add(entrySND.SoundAssetID);
                foreach (EntrySoundInfo_XBOX entrySND in assetSNDI.Entries_SND)
                    if (!assetIDsAlreadyPresentSND.Contains(entrySND.SoundAssetID))
                        entriesSND.Add(entrySND);
                Entries_SND = entriesSND.ToArray();
            }
            {
                // SNDS
                List<EntrySoundInfo_XBOX> entriesSNDS = Entries_SNDS.ToList();
                List<uint> assetIDsAlreadyPresentSNDS = new List<uint>();
                foreach (EntrySoundInfo_XBOX entrySNDS in entriesSNDS)
                    assetIDsAlreadyPresentSNDS.Add(entrySNDS.SoundAssetID);
                foreach (EntrySoundInfo_XBOX entrySNDS in assetSNDI.Entries_SNDS)
                    if (!assetIDsAlreadyPresentSNDS.Contains(entrySNDS.SoundAssetID))
                        entriesSNDS.Add(entrySNDS);
                Entries_SNDS = entriesSNDS.ToArray();
            }
            {
                // Sound_CIN
                List<EntrySoundInfo_XBOX> entriesSound_CIN = Entries_Sound_CIN.ToList();
                List<uint> assetIDsAlreadyPresentSound_CIN = new List<uint>();
                foreach (EntrySoundInfo_XBOX entrySound_CIN in entriesSound_CIN)
                    assetIDsAlreadyPresentSound_CIN.Add(entrySound_CIN.SoundAssetID);
                foreach (EntrySoundInfo_XBOX entrySound_CIN in assetSNDI.Entries_Sound_CIN)
                    if (!assetIDsAlreadyPresentSound_CIN.Contains(entrySound_CIN.SoundAssetID))
                        entriesSound_CIN.Add(entrySound_CIN);
                Entries_Sound_CIN = entriesSound_CIN.ToArray();
            }
        }

        public void Clean(IEnumerable<uint> assetIDs)
        {
            {
                // SND
                var entriesSND = Entries_SND.ToList();
                for (int i = 0; i < entriesSND.Count; i++)
                    if (!assetIDs.Contains(entriesSND[i].SoundAssetID))
                        entriesSND.RemoveAt(i--);
                Entries_SND = entriesSND.ToArray();
            }
            {
                // SNDS
                var entriesSNDS = Entries_SNDS.ToList();
                for (int i = 0; i < entriesSNDS.Count; i++)
                    if (!assetIDs.Contains(entriesSNDS[i].SoundAssetID))
                        entriesSNDS.RemoveAt(i--);
                Entries_SNDS = entriesSNDS.ToArray();
            }
            {
                // Sound_CIN
                var entriesSNDS_CIN = Entries_Sound_CIN.ToList();
                for (int i = 0; i < entriesSNDS_CIN.Count; i++)
                    if (!assetIDs.Contains(entriesSNDS_CIN[i].SoundAssetID))
                        entriesSNDS_CIN.RemoveAt(i--);
                Entries_Sound_CIN = entriesSNDS_CIN.ToArray();
            }
        }
    }
}