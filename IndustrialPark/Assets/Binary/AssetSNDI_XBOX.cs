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
        public int unknown { get; set; }

        public static int StructSize = 0x2C;

        public EntrySoundInfo_XBOX() { }
        public EntrySoundInfo_XBOX(EndianBinaryReader reader)
        {
            Read(reader);
        }

        private void Read(EndianBinaryReader reader)
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
            unknown = reader.ReadInt32();

            reader.BaseStream.Position += 12;
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
            array.AddRange(BitConverter.GetBytes(unknown));
            array.AddRange(new byte[12]);

            return array.ToArray();
        }

        public byte[] SoundHeader
        {
            get => Serialize();            
            set => Read(new EndianBinaryReader(value, Endianness.Little));
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

        public AssetSNDI_XBOX(string assetName) : base(assetName, AssetType.SNDI)
        {
            Entries_SND = new EntrySoundInfo_XBOX[0];
            Entries_SNDS = new EntrySoundInfo_XBOX[0];
            Entries_Sound_CIN = new EntrySoundInfo_XBOX[0];
        }

        public AssetSNDI_XBOX(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Entries_SND.Length);
                writer.Write(Entries_SNDS.Length);
                writer.Write(Entries_Sound_CIN.Length);

                foreach (var e in Entries_SND)
                    writer.Write(e.Serialize());
                foreach (var e in Entries_SNDS)
                    writer.Write(e.Serialize());
                foreach (var e in Entries_Sound_CIN)
                    writer.Write(e.Serialize());

                return writer.ToArray();
            }
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

            var reader = new EndianBinaryReader(soundData, Endianness.Little);
            reader.BaseStream.Position = 0x14;

            entries.Add(new EntrySoundInfo_XBOX(reader) { SoundAssetID = assetID });

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
                throw new Exception($"Error: SNDI asset does not contain {assetType} sound header for asset [{assetID:X8}]");

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