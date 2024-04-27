using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;

namespace IndustrialPark
{
    public class EntrySoundInfo_GCN_V1 : GenericAssetDataContainer
    {
        public uint num_samples { get; set; }
        public uint num_adpcm_nibbles { get; set; }
        public uint sample_rate { get; set; }
        public bool Loop { get; set; }
        public ushort format { get; set; }
        public uint loop_start_offset { get; set; }
        public uint loop_end_offset { get; set; }
        public uint initial_offset_value { get; set; }
        public short[] coefs { get; set; }
        public ushort gain_factor { get; set; }
        public ushort pred_scale { get; set; }
        public ushort yn1 { get; set; }
        public ushort yn2 { get; set; }
        public ushort loop_pred_scale { get; set; }
        public ushort loop_yn1 { get; set; }
        public ushort loop_yn2 { get; set; }
        public byte[] pad { get; set; }
        [ValidReferenceRequired]
        public AssetID Sound { get; set; }

        public EntrySoundInfo_GCN_V1() { }

        public EntrySoundInfo_GCN_V1(EndianBinaryReader reader)
        {
            Read(reader);
        }

        private void Read(EndianBinaryReader reader)
        {
            num_samples = reader.ReadUInt32();
            num_adpcm_nibbles = reader.ReadUInt32();
            sample_rate = reader.ReadUInt32();
            Loop = Convert.ToBoolean(reader.ReadUInt16());
            format = reader.ReadUInt16();
            loop_start_offset = reader.ReadUInt32();
            loop_end_offset = reader.ReadUInt32();
            initial_offset_value = reader.ReadUInt32();
            coefs = new short[16];
            for (int i = 0; i < coefs.Length; i++)
                coefs[i] = reader.ReadInt16();
            gain_factor = reader.ReadUInt16();
            pred_scale = reader.ReadUInt16();
            yn1 = reader.ReadUInt16();
            yn2 = reader.ReadUInt16();
            loop_pred_scale = reader.ReadUInt16();
            loop_yn1 = reader.ReadUInt16();
            loop_yn2 = reader.ReadUInt16();
            pad = new byte[22];
            for (int i = 0; i < pad.Length; i++)
                pad[i] = reader.ReadByte();
            Sound = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(SoundHeader);
            writer.Write(Sound);
        }

        public byte[] Serialize()
        {
            List<byte> array = new List<byte>();

            array.AddRange(BitConverter.GetBytes(num_samples).Reverse());
            array.AddRange(BitConverter.GetBytes(num_adpcm_nibbles).Reverse());
            array.AddRange(BitConverter.GetBytes(sample_rate).Reverse());
            array.AddRange(BitConverter.GetBytes(Convert.ToUInt16(Loop)).Reverse());
            array.AddRange(BitConverter.GetBytes(format).Reverse());
            array.AddRange(BitConverter.GetBytes(loop_start_offset).Reverse());
            array.AddRange(BitConverter.GetBytes(loop_end_offset).Reverse());
            array.AddRange(BitConverter.GetBytes(initial_offset_value).Reverse());
            array.AddRange(coefs.SelectMany(i => BitConverter.GetBytes(i).Reverse()).ToArray());
            array.AddRange(BitConverter.GetBytes(gain_factor).Reverse());
            array.AddRange(BitConverter.GetBytes(pred_scale).Reverse());
            array.AddRange(BitConverter.GetBytes(yn1).Reverse());
            array.AddRange(BitConverter.GetBytes(yn2).Reverse());
            array.AddRange(BitConverter.GetBytes(loop_pred_scale).Reverse());
            array.AddRange(BitConverter.GetBytes(loop_yn1).Reverse());
            array.AddRange(BitConverter.GetBytes(loop_yn2).Reverse());
            array.AddRange(pad);
            return array.ToArray();
        }

        public byte[] SoundHeader
        {
            get => Serialize();
            set => Read(new EndianBinaryReader(value, Endianness.Big));
        }

        public override string ToString()
        {
            return HexUIntTypeConverter.StringFromAssetID(Sound);
        }
    }

    public class AssetSNDI_GCN_V1 : Asset
    {
        public override string AssetInfo => $"GameCube {game}, {Entries_SND.Length + Entries_SNDS.Length + Entries_Sound_CIN.Length} entries";

        private const string categoryName = "Sound Info: GCN V1";

        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_SND { get; set; }
        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_SNDS { get; set; }
        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_Sound_CIN { get; set; }

        public AssetSNDI_GCN_V1(string assetName) : base(assetName, AssetType.SoundInfo)
        {
            Entries_SND = new EntrySoundInfo_GCN_V1[0];
            Entries_SNDS = new EntrySoundInfo_GCN_V1[0];
            Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[0];
        }

        public AssetSNDI_GCN_V1(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Big))
            {
                int entriesSndAmount = reader.ReadInt32();
                reader.ReadInt32();
                int entriesSndsAmount = reader.ReadInt32();
                int entriesCinAmount = game == Game.BFBB ? reader.ReadInt32() : 0;

                Entries_SND = new EntrySoundInfo_GCN_V1[entriesSndAmount];
                for (int i = 0; i < Entries_SND.Length; i++)
                    Entries_SND[i] = new EntrySoundInfo_GCN_V1(reader);

                Entries_SNDS = new EntrySoundInfo_GCN_V1[entriesSndsAmount];
                for (int i = 0; i < Entries_SNDS.Length; i++)
                    Entries_SNDS[i] = new EntrySoundInfo_GCN_V1(reader);

                if (game == Game.BFBB)
                {
                    Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[entriesCinAmount];
                    for (int i = 0; i < Entries_Sound_CIN.Length; i++)
                        Entries_Sound_CIN[i] = new EntrySoundInfo_GCN_V1(reader);
                }
                else
                    Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[0];
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Entries_SND.Length);
            writer.Write(0xCDCDCDCD);
            writer.Write(Entries_SNDS.Length);
            if (game == Game.BFBB)
                writer.Write(Entries_Sound_CIN.Length);

            foreach (var e in Entries_SND)
                e.Serialize(writer);
            foreach (var e in Entries_SNDS)
                e.Serialize(writer);
            if (game == Game.BFBB)
                foreach (var e in Entries_Sound_CIN)
                    e.Serialize(writer);
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            List<EntrySoundInfo_GCN_V1> entries;

            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                    entries.Remove(entries[i--]);

            entries.Add(new EntrySoundInfo_GCN_V1(new EndianBinaryReader(soundData, Endianness.Big)) { Sound = assetID });

            finalData = soundData.Skip(0x60).ToArray();

            if (assetType == AssetType.Sound)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public void RemoveEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_GCN_V1> entries;

            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                    entries.Remove(entries[i--]);

            if (assetType == AssetType.Sound)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public void SetEntry(EntrySoundInfo_GCN_V1 entry, AssetType assetType)
        {
            List<EntrySoundInfo_GCN_V1> entries;
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

        public EntrySoundInfo_GCN_V1 GetEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_GCN_V1> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            EntrySoundInfo_GCN_V1 entry = null;

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

        public byte[] GetHeader(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_GCN_V1> entries;
            if (assetType == AssetType.Sound)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                    return entries[i].SoundHeader;

            entries = Entries_Sound_CIN.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Sound == assetID)
                    return entries[i].SoundHeader;

            throw new Exception($"Error: Sound Info does not contain {assetType} sound header for asset [{assetID:X8}]");
        }

        public void Merge(AssetSNDI_GCN_V1 assetSNDI)
        {
            {
                // SND
                var entries = Entries_SND.ToList();
                var assetIDsAlreadyPresent = (from entry in entries select (uint)entry.Sound).ToList();

                foreach (var entry in assetSNDI.Entries_SND)
                    if (!assetIDsAlreadyPresent.Contains(entry.Sound))
                        entries.Add(entry);

                Entries_SND = entries.ToArray();
            }
            {
                // SNDS
                var entries = Entries_SNDS.ToList();
                var assetIDsAlreadyPresent = (from entry in entries select (uint)entry.Sound).ToList();

                foreach (var entry in assetSNDI.Entries_SNDS)
                    if (!assetIDsAlreadyPresent.Contains(entry.Sound))
                        entries.Add(entry);

                Entries_SNDS = entries.ToArray();
            }
            {
                // Sound_CIN
                var entries = Entries_Sound_CIN.ToList();
                var assetIDsAlreadyPresent = (from entry in entries select (uint)entry.Sound).ToList();

                foreach (var entry in assetSNDI.Entries_Sound_CIN)
                    if (!assetIDsAlreadyPresent.Contains(entry.Sound))
                        entries.Add(entry);

                Entries_Sound_CIN = entries.ToArray();
            }
        }

        public void Clean(IEnumerable<uint> assetIDs)
        {
            {
                // SND
                var entries = Entries_SND.ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (!assetIDs.Contains(entries[i].Sound))
                        entries.RemoveAt(i--);
                Entries_SND = entries.ToArray();
            }
            {
                // SNDS
                var entries = Entries_SNDS.ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (!assetIDs.Contains(entries[i].Sound))
                        entries.RemoveAt(i--);
                Entries_SNDS = entries.ToArray();
            }
            {
                // Sound_CIN
                var entries = Entries_Sound_CIN.ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (!assetIDs.Contains(entries[i].Sound))
                        entries.RemoveAt(i--);
                Entries_Sound_CIN = entries.ToArray();
            }
        }
    }
}