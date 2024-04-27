using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;

namespace IndustrialPark
{
    public class EntrySoundInfo_PS2 : GenericAssetDataContainer
    {
        public byte[] magic;
        public uint Version { get; set; }
        public AssetID SoundAssetID { get; set; }
        public uint DataSize { get; set; }
        public uint SampleRate { get; set; }
        public uint StreamInterleaveSize { get; set; }
        public uint StreamInterleaveCount { get; set; }
        public uint reserved2 { get; set; }
        public string TrackName { get; set; }

        public static int StructSize = 0x30;

        public EntrySoundInfo_PS2() { }
        public EntrySoundInfo_PS2(EndianBinaryReader reader)
        {
            Read(reader);
        }

        private void Read(EndianBinaryReader reader)
        {
            magic = reader.ReadBytes(4);
            Version = reader.ReadUInt32();
            SoundAssetID = reader.ReadUInt32();
            DataSize = reader.ReadUInt32();
            SampleRate = reader.ReadUInt32();
            StreamInterleaveSize = reader.ReadUInt32();
            StreamInterleaveCount = reader.ReadUInt32();
            reserved2 = reader.ReadUInt32();
            TrackName = new string(reader.ReadChars(16));
        }

        public override void Serialize(EndianBinaryWriter writer) { }

        public byte[] Serialize()
        {
            List<byte> array = new List<byte>();

            array.AddRange(magic);
            array.AddRange(BitConverter.GetBytes(Version));
            array.AddRange(BitConverter.GetBytes(SoundAssetID));
            array.AddRange(BitConverter.GetBytes(DataSize));
            array.AddRange(BitConverter.GetBytes(SampleRate));
            array.AddRange(BitConverter.GetBytes(StreamInterleaveSize));
            array.AddRange(BitConverter.GetBytes(StreamInterleaveCount));
            array.AddRange(BitConverter.GetBytes(reserved2));
            array.AddRange(Encoding.ASCII.GetBytes(TrackName.PadRight(16, '\0')));

            return array.ToArray();
        }

        public byte[] SoundHeader
        {
            get => Serialize();
            set => Read(new EndianBinaryReader(value, Endianness.Little));
        }

        public override string ToString()
        {
            return HexUIntTypeConverter.StringFromAssetID(SoundAssetID);
        }
    }

    public class AssetSNDI_PS2 : Asset
    {
        public override string AssetInfo => $"PS2, {Entries_SND.Length + Entries_SNDS.Length} entries";

        private const string categoryName = "Sound Info: PS2";

        [Category(categoryName)]
        public EntrySoundInfo_PS2[] Entries_SND { get; set; }
        [Category(categoryName)]
        public EntrySoundInfo_PS2[] Entries_SNDS { get; set; }

        [Category(categoryName)]
        public EVersionIncrediblesROTUOthers AssetVersion { get; set; } = EVersionIncrediblesROTUOthers.ScoobyBfbbMovie;

        public AssetSNDI_PS2(string assetName) : base(assetName, AssetType.SoundInfo)
        {
            Entries_SND = new EntrySoundInfo_PS2[0];
            Entries_SNDS = new EntrySoundInfo_PS2[0];
        }

        public AssetSNDI_PS2(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Little))
            {
                var maybeAssetId = reader.ReadUInt32();
                if (maybeAssetId == assetID)
                {
                    AssetVersion = EVersionIncrediblesROTUOthers.IncrediblesRotu;
                }
                else
                {
                    AssetVersion = EVersionIncrediblesROTUOthers.ScoobyBfbbMovie;
                    reader.BaseStream.Position = 0;
                }

                int entriesSndAmount = reader.ReadInt32();
                int entriesSndsAmount = reader.ReadInt32();

                Entries_SND = new EntrySoundInfo_PS2[entriesSndAmount];
                for (int i = 0; i < Entries_SND.Length; i++)
                    Entries_SND[i] = new EntrySoundInfo_PS2(reader);

                Entries_SNDS = new EntrySoundInfo_PS2[entriesSndsAmount];
                for (int i = 0; i < Entries_SNDS.Length; i++)
                    Entries_SNDS[i] = new EntrySoundInfo_PS2(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            if (AssetVersion == EVersionIncrediblesROTUOthers.IncrediblesRotu)
                writer.Write(assetID);

            writer.Write(Entries_SND.Length);
            writer.Write(Entries_SNDS.Length);

            foreach (var e in Entries_SND)
                writer.Write(e.SoundHeader);
            foreach (var e in Entries_SNDS)
                writer.Write(e.SoundHeader);
        }

        private void OrderEntries()
        {
            Entries_SND = Entries_SND.OrderBy(i => i.SoundAssetID).ToArray();
            Entries_SNDS = Entries_SNDS.OrderBy(i => i.SoundAssetID).ToArray();
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            RemoveEntry(assetID, assetType);

            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            entries.Add(new EntrySoundInfo_PS2(new EndianBinaryReader(soundData, Endianness.Little)) { SoundAssetID = assetID });

            finalData = soundData.Skip(EntrySoundInfo_PS2.StructSize).ToArray();

            if (assetType == AssetType.Sound)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
            OrderEntries();
        }

        public void RemoveEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entries.Remove(entries[i]);

            if (assetType == AssetType.Sound)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();

        }

        public byte[] GetHeader(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    return entries[i].SoundHeader;

            throw new Exception($"Error: SNDI asset does not contain {assetType} sound header for asset [{assetID:X8}]");
        }

        public EntrySoundInfo_PS2 GetEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            EntrySoundInfo_PS2 entry = null;

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entry = entries[i];

            if (entry == null)
                throw new Exception($"Error: Sound Info asset does not contain {assetType} sound header for asset [{assetID:X8}]");

            return entry;
        }

        public void SetEntry(EntrySoundInfo_PS2 entry, AssetType assetType)
        {
            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == entry.SoundAssetID)
                {
                    entries[i] = entry;

                    if (assetType == AssetType.Sound)
                        Entries_SND = entries.ToArray();
                    else
                        Entries_SNDS = entries.ToArray();
                    return;
                }
        }

        public void Merge(AssetSNDI_PS2 assetSNDI)
        {
            {
                // SND
                List<EntrySoundInfo_PS2> entriesSND = Entries_SND.ToList();
                List<uint> assetIDsAlreadyPresentSND = new List<uint>();
                foreach (EntrySoundInfo_PS2 entrySND in entriesSND)
                    assetIDsAlreadyPresentSND.Add(entrySND.SoundAssetID);
                foreach (EntrySoundInfo_PS2 entrySND in assetSNDI.Entries_SND)
                    if (!assetIDsAlreadyPresentSND.Contains(entrySND.SoundAssetID))
                        entriesSND.Add(entrySND);
                Entries_SND = entriesSND.ToArray();
            }
            {
                // SNDS
                List<EntrySoundInfo_PS2> entriesSNDS = Entries_SNDS.ToList();
                List<uint> assetIDsAlreadyPresentSNDS = new List<uint>();
                foreach (EntrySoundInfo_PS2 entrySNDS in entriesSNDS)
                    assetIDsAlreadyPresentSNDS.Add(entrySNDS.SoundAssetID);
                foreach (EntrySoundInfo_PS2 entrySNDS in assetSNDI.Entries_SNDS)
                    if (!assetIDsAlreadyPresentSNDS.Contains(entrySNDS.SoundAssetID))
                        entriesSNDS.Add(entrySNDS);
                Entries_SNDS = entriesSNDS.ToArray();
            }
            OrderEntries();
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
        }
    }
}