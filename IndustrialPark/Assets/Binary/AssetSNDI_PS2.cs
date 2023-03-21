using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntrySoundInfo_PS2 : GenericAssetDataContainer
    {
        public override void Serialize(EndianBinaryWriter writer) { }

        public byte[] SoundHeader { get; set; }

        [ValidReferenceRequired]
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
            return HexUIntTypeConverter.StringFromAssetID(SoundAssetID);
        }
    }

    public class AssetSNDI_PS2 : Asset
    {
        public override string AssetInfo => $"PS2, {Entries_SND.Length + Entries_SNDS.Length} entries";

        private const string categoryName = "Sound Info: GCN V1";

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
                    Entries_SND[i] = new EntrySoundInfo_PS2(reader.ReadBytes(EntrySoundInfo_PS2.StructSize));

                Entries_SNDS = new EntrySoundInfo_PS2[entriesSndsAmount];
                for (int i = 0; i < Entries_SNDS.Length; i++)
                    Entries_SNDS[i] = new EntrySoundInfo_PS2(reader.ReadBytes(EntrySoundInfo_PS2.StructSize));
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

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            RemoveEntry(assetID, assetType);

            List<EntrySoundInfo_PS2> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            entries.Add(new EntrySoundInfo_PS2(soundData.Take(EntrySoundInfo_PS2.StructSize).ToArray()) { SoundAssetID = assetID });

            finalData = soundData.Skip(EntrySoundInfo_PS2.StructSize).ToArray();

            if (assetType == AssetType.Sound)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
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