using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntrySoundInfo_XBOX : GenericAssetDataContainer
    {
        public short wFormatTag { get; set; }
        public short nChannels { get; set; }
        public int nSamplesPerSec { get; set; }
        public int nAvgBytesPerSec { get; set; }
        public short nBlockAlign { get; set; }
        public short wBitsPerSample { get; set; }
        public short cbSize { get; set; }
        public short NibblesPerBlock { get; set; }
        public int dataSize { get; set; }
        [ValidReferenceRequired]
        public AssetID Sound { get; set; }
        public int flag_loop { get; set; }

        public static int StructSize = 0x2C;

        public EntrySoundInfo_XBOX() { }
        public EntrySoundInfo_XBOX(EndianBinaryReader reader)
        {
            Read(reader);
        }

        private void Read(EndianBinaryReader reader)
        {
            wFormatTag = reader.ReadInt16();
            nChannels = reader.ReadInt16();
            nSamplesPerSec = reader.ReadInt32();
            nAvgBytesPerSec = reader.ReadInt32();
            nBlockAlign = reader.ReadInt16();
            wBitsPerSample = reader.ReadInt16();
            cbSize = reader.ReadInt16();
            NibblesPerBlock = reader.ReadInt16();
            dataSize = reader.ReadInt32();
            Sound = reader.ReadUInt32();
            flag_loop = reader.ReadInt32();

            reader.BaseStream.Position += 12;
        }

        public override void Serialize(EndianBinaryWriter writer) { }

        public byte[] Serialize()
        {
            List<byte> array = new List<byte>();

            array.AddRange(BitConverter.GetBytes(wFormatTag));
            array.AddRange(BitConverter.GetBytes(nChannels));
            array.AddRange(BitConverter.GetBytes(nSamplesPerSec));
            array.AddRange(BitConverter.GetBytes(nAvgBytesPerSec));
            array.AddRange(BitConverter.GetBytes(nBlockAlign));
            array.AddRange(BitConverter.GetBytes(wBitsPerSample));
            array.AddRange(BitConverter.GetBytes(cbSize));
            array.AddRange(BitConverter.GetBytes(NibblesPerBlock));
            array.AddRange(BitConverter.GetBytes(dataSize));
            array.AddRange(BitConverter.GetBytes(Sound));
            array.AddRange(BitConverter.GetBytes(flag_loop));
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
            return HexUIntTypeConverter.StringFromAssetID(Sound);
        }
    }

    public class AssetSNDI_XBOX : Asset
    {
        public override string AssetInfo => $"Xbox, {Entries_SND.Length + Entries_SNDS.Length + Entries_Sound_CIN.Length} entries";

        private const string categoryName = "Sound Info: Xbox";

        [Category(categoryName)]
        public EntrySoundInfo_XBOX[] Entries_SND { get; set; }

        [Category(categoryName)]
        public EntrySoundInfo_XBOX[] Entries_SNDS { get; set; }

        [Category(categoryName)]
        public EntrySoundInfo_XBOX[] Entries_Sound_CIN { get; set; }

        public AssetSNDI_XBOX(string assetName) : base(assetName, AssetType.SoundInfo)
        {
            Entries_SND = new EntrySoundInfo_XBOX[0];
            Entries_SNDS = new EntrySoundInfo_XBOX[0];
            Entries_Sound_CIN = new EntrySoundInfo_XBOX[0];
        }

        public AssetSNDI_XBOX(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Little))
            {
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
        }

        public override void Serialize(EndianBinaryWriter writer)
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
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            RemoveEntry(assetID, assetType);

            List<EntrySoundInfo_XBOX> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            var reader = new EndianBinaryReader(soundData, Endianness.Little);
            reader.BaseStream.Position = 0x14;

            entries.Add(new EntrySoundInfo_XBOX(reader) { Sound = assetID });

            finalData = soundData.Skip(0x30).ToArray();

            if (assetType == AssetType.Sound)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public void RemoveEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_XBOX> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                    entries.Remove(entries[i]);

            if (assetType == AssetType.Sound)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public byte[] GetHeader(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_XBOX> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            EntrySoundInfo_XBOX entry = null;

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                    entry = entries[i];

            if (entry == null)
            {
                entries = Entries_Sound_CIN.ToList();

                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].Sound == assetID)
                        entry = entries[i];
            }

            if (entry == null)
                throw new Exception($"Error: Sound Info asset does not contain {assetType} sound header for asset [{assetID:X8}]");

            List<byte> bytes = new List<byte>
                {
                    (byte)'R', (byte)'I', (byte)'F', (byte)'F',
                    0, 0, 0, 0,
                    (byte)'W', (byte)'A', (byte)'V', (byte)'E',
                    (byte)'f', (byte)'m', (byte)'t', (byte)' ',
                    0x14, 0, 0, 0
                };

            if (entry.wFormatTag == 0x0001)
            {
                bytes.AddRange(BitConverter.GetBytes(entry.wFormatTag));
                bytes.AddRange(BitConverter.GetBytes(entry.nChannels));
                bytes.AddRange(BitConverter.GetBytes(entry.nSamplesPerSec));
                bytes.AddRange(BitConverter.GetBytes(entry.nAvgBytesPerSec));
                bytes.AddRange(BitConverter.GetBytes(entry.nBlockAlign));
                bytes.AddRange(BitConverter.GetBytes(entry.wBitsPerSample));
                bytes.AddRange(BitConverter.GetBytes(entry.cbSize));
                bytes.AddRange(BitConverter.GetBytes(entry.NibblesPerBlock));
            }
            else if (entry.wFormatTag == 0x0069 || entry.wFormatTag == 0x006A)
            {
                bytes.AddRange(BitConverter.GetBytes((short)0x0011));
                bytes.AddRange(BitConverter.GetBytes(entry.nChannels));
                bytes.AddRange(BitConverter.GetBytes(entry.nSamplesPerSec * 65 / 64));
                bytes.AddRange(BitConverter.GetBytes(entry.nAvgBytesPerSec));
                bytes.AddRange(BitConverter.GetBytes(entry.nBlockAlign));
                bytes.AddRange(BitConverter.GetBytes(entry.wBitsPerSample));
                bytes.AddRange(BitConverter.GetBytes(entry.cbSize));
                bytes.AddRange(BitConverter.GetBytes((short)0x0041));
            }

            bytes.Add((byte)'d');
            bytes.Add((byte)'a');
            bytes.Add((byte)'t');
            bytes.Add((byte)'a');
            bytes.AddRange(BitConverter.GetBytes(entry.dataSize));

            return bytes.ToArray();
        }

        public EntrySoundInfo_XBOX GetEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_XBOX> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            EntrySoundInfo_XBOX entry = null;

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                    entry = entries[i];

            if (entry == null)
            {
                entries = Entries_Sound_CIN.ToList();

                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].Sound == assetID)
                        entry = entries[i];
            }

            if (entry == null)
                throw new Exception($"Error: Sound Info asset does not contain {assetType} sound header for asset [{assetID:X8}]");

            return entry;
        }

        public void SetEntry(EntrySoundInfo_XBOX entry, AssetType assetType)
        {
            List<EntrySoundInfo_XBOX> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == entry.Sound)
                {
                    entries[i] = entry;

                    if (assetType == AssetType.Sound)
                        Entries_SND = entries.ToArray();
                    else
                        Entries_SNDS = entries.ToArray();
                    return;
                }

            entries = Entries_Sound_CIN.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                {
                    entries[i] = entry;
                    Entries_Sound_CIN = entries.ToArray();
                    return;
                }
        }

        public void Merge(AssetSNDI_XBOX assetSNDI)
        {
            {
                // SND
                List<EntrySoundInfo_XBOX> entriesSND = Entries_SND.ToList();
                List<uint> assetIDsAlreadyPresentSND = new List<uint>();
                foreach (EntrySoundInfo_XBOX entrySND in entriesSND)
                    assetIDsAlreadyPresentSND.Add(entrySND.Sound);
                foreach (EntrySoundInfo_XBOX entrySND in assetSNDI.Entries_SND)
                    if (!assetIDsAlreadyPresentSND.Contains(entrySND.Sound))
                        entriesSND.Add(entrySND);
                Entries_SND = entriesSND.ToArray();
            }
            {
                // SNDS
                List<EntrySoundInfo_XBOX> entriesSNDS = Entries_SNDS.ToList();
                List<uint> assetIDsAlreadyPresentSNDS = new List<uint>();
                foreach (EntrySoundInfo_XBOX entrySNDS in entriesSNDS)
                    assetIDsAlreadyPresentSNDS.Add(entrySNDS.Sound);
                foreach (EntrySoundInfo_XBOX entrySNDS in assetSNDI.Entries_SNDS)
                    if (!assetIDsAlreadyPresentSNDS.Contains(entrySNDS.Sound))
                        entriesSNDS.Add(entrySNDS);
                Entries_SNDS = entriesSNDS.ToArray();
            }
            {
                // Sound_CIN
                List<EntrySoundInfo_XBOX> entriesSound_CIN = Entries_Sound_CIN.ToList();
                List<uint> assetIDsAlreadyPresentSound_CIN = new List<uint>();
                foreach (EntrySoundInfo_XBOX entrySound_CIN in entriesSound_CIN)
                    assetIDsAlreadyPresentSound_CIN.Add(entrySound_CIN.Sound);
                foreach (EntrySoundInfo_XBOX entrySound_CIN in assetSNDI.Entries_Sound_CIN)
                    if (!assetIDsAlreadyPresentSound_CIN.Contains(entrySound_CIN.Sound))
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
                    if (!assetIDs.Contains(entriesSND[i].Sound))
                        entriesSND.RemoveAt(i--);
                Entries_SND = entriesSND.ToArray();
            }
            {
                // SNDS
                var entriesSNDS = Entries_SNDS.ToList();
                for (int i = 0; i < entriesSNDS.Count; i++)
                    if (!assetIDs.Contains(entriesSNDS[i].Sound))
                        entriesSNDS.RemoveAt(i--);
                Entries_SNDS = entriesSNDS.ToArray();
            }
            {
                // Sound_CIN
                var entriesSNDS_CIN = Entries_Sound_CIN.ToList();
                for (int i = 0; i < entriesSNDS_CIN.Count; i++)
                    if (!assetIDs.Contains(entriesSNDS_CIN[i].Sound))
                        entriesSNDS_CIN.RemoveAt(i--);
                Entries_Sound_CIN = entriesSNDS_CIN.ToArray();
            }
        }
    }
}