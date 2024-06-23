using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSNDI_GCN_V2 : Asset
    {
        public override string AssetInfo => $"GameCube {game}, {(Entry_Sounds == null ? 0 : Entry_Sounds.SampleHeader.Length) + Entries_StreamingSounds.Length} entries";

        private const string categoryName = "Sound Info";

        [Category(categoryName)]
        public FSB3_File Entry_Sounds { get; set; }

        [Category(categoryName)]
        public FSB3_File[] Entries_StreamingSounds { get; set; }

        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_Sound_CIN { get; set; }

        public AssetSNDI_GCN_V2(string assetName) : base(assetName, AssetType.SoundInfo)
        {
            Entry_Sounds = null;
            Entries_StreamingSounds = new FSB3_File[0];
            Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[0];
        }

        public AssetSNDI_GCN_V2(Section_AHDR AHDR, Game game) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Big))
            {
                reader.ReadUInt32();
                var totalSize = reader.ReadUInt32();
                // Internal Pointers
                reader.BaseStream.Position += 0x10;
                ushort nWavFiles = reader.ReadUInt16();
                ushort nSounds = reader.ReadUInt16();
                ushort nStreams = reader.ReadUInt16();
                byte nFSBFiles = reader.ReadByte();
                byte nCutsceneAudioHeaders = reader.ReadByte();

                reader.BaseStream.Position = totalSize + 0x20;

                var offsets = new uint[nFSBFiles];
                for (int i = 0; i < nFSBFiles; i++)
                    offsets[i] = reader.ReadUInt32() + 0x20;

                List<FSB3_File> entries = new List<FSB3_File>();
                for (int i = 0; i < offsets.Length; i++)
                    using (var fsbReader = new EndianBinaryReader(AHDR.data, Endianness.Little))
                    {
                        fsbReader.BaseStream.Position = offsets[i];
                        var fsb3file = new FSB3_File(fsbReader);
                        fsb3file.UnknownBytes = fsbReader.ReadBytes((32 - (fsb3file.TotalFSBSize % 32)) % 32);
                        entries.Add(fsb3file);
                    }

                for (int i = 0; i < nWavFiles; i++)
                {
                    var assetID = reader.ReadUInt32();
                    var uFlags = reader.ReadByte();
                    var uAudioSampleIndex = reader.ReadByte();
                    var uFSBIndex = reader.ReadByte();
                    var uSoundInfoIndex = reader.ReadByte();
                    entries[uFSBIndex].SampleHeader[uAudioSampleIndex].SetEntryData(assetID, uFlags, uAudioSampleIndex, uFSBIndex, uSoundInfoIndex);
                }

                if (entries.Count == 0 || (entries.Count == 1 && entries[0].SampleHeader.Length == 0))
                {
                    Entry_Sounds = null;
                    Entries_StreamingSounds = new FSB3_File[0];
                }
                else if (entries.Count == 1)
                {
                    if (entries[0].SampleHeader[0].StreamedSound)
                    {
                        Entry_Sounds = null;
                        Entries_StreamingSounds = entries.ToArray();
                    }
                    else
                    {
                        Entry_Sounds = entries[0];
                        Entries_StreamingSounds = new FSB3_File[0];
                    }
                }
                else
                {
                    Entry_Sounds = entries[0];
                    Entries_StreamingSounds = entries.Skip(1).ToArray();
                }

                Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[nCutsceneAudioHeaders];
                for (int i = 0; i < Entries_Sound_CIN.Length; i++)
                    Entries_Sound_CIN[i] = new EntrySoundInfo_GCN_V1(reader);
            }
        }

        public List<FSB3_File> GetAllEntries()
        {
            var result = new List<FSB3_File>();
            if (Entry_Sounds != null && Entry_Sounds.SampleHeader.Length > 0)
                result.Add(Entry_Sounds);
            result.AddRange(Entries_StreamingSounds);
            return result;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var Entries = GetAllEntries();

            List<int> offsets = new List<int>();
            writer.BaseStream.Position = 0x20;
            for (int i = 0; i < Entries.Count; i++)
            {
                offsets.Add((int)writer.BaseStream.Position - 0x20);
                Entries[i].Serialize(writer);

                if (Entries[i].UnknownBytes == null)
                    while (writer.BaseStream.Position % 0x20 != 0)
                        writer.BaseStream.Position++;
                else
                    writer.Write(Entries[i].UnknownBytes);
            }

            int footerOffset = (int)(writer.BaseStream.Position - 0x20);
            List<FSB3_SampleHeader> gcWavInfos = new List<FSB3_SampleHeader>();

            for (int i = 0; i < Entries.Count; i++)
            {
                writer.Write(offsets[i]);
                for (int j = 0; j < Entries[i].Header.NumSamples; j++)
                {
                    Entries[i].SampleHeader[j].uAudioSampleIndex = (byte)j;
                    Entries[i].SampleHeader[j].uFSBIndex = (byte)i;
                    gcWavInfos.Add(Entries[i].SampleHeader[j]);
                }
            }

            gcWavInfos = gcWavInfos.OrderBy(f => f.Sound).ToList();
            foreach (FSB3_SampleHeader entry in gcWavInfos)
                entry.SerializeWavInfo(writer);

            foreach (var entry in Entries_Sound_CIN)
                entry.Serialize(writer);

            ushort nWavFiles = (ushort)Entries.Sum(e => e.SampleHeader.Length);
            ushort nSounds = (ushort)Entries.Sum(e => e.SampleHeader.Count(se => !se.StreamedSound));
            ushort nStreams = (ushort)Entries.Sum(e => e.SampleHeader.Count(se => se.StreamedSound));

            writer.BaseStream.Position = 0;
            writer.Write(assetID);
            writer.Write(footerOffset);
            writer.Write(new byte[0x10]);
            writer.Write(nWavFiles);
            writer.Write(nSounds);
            writer.Write(nStreams);
            writer.Write((byte)Entries.Count);
            writer.Write((byte)Entries_Sound_CIN.Length);
        }

        public byte[] GetHeader(uint assetID)
        {
            foreach (FSB3_File f in GetAllEntries())
                for (int i = 0; i < f.SampleHeader.Length; i++)
                    if (f.SampleHeader[i].Sound == assetID)
                        return new GcnV2SoundWrapper(f.Header, f.SampleHeader[i]).Serialize();

            throw new Exception($"Error: SNDI asset does not contain sound header for asset [{assetID:X8}]");
        }

        public FSB3_SampleHeader GetEntry(uint assetID)
        {
            FSB3_SampleHeader entry = null;
            foreach (FSB3_File fsb3 in GetAllEntries())
                foreach (FSB3_SampleHeader header in fsb3.SampleHeader)
                    if (header.Sound == assetID)
                        entry = header;

            if (entry == null)
                throw new Exception($"Error: Sound Info asset does not contain sound header for asset [{assetID:X8}]");
            return entry;
        }

        public void SetEntry(FSB3_SampleHeader entry)
        {
            foreach (FSB3_File fsb3 in GetAllEntries())
            {
                List<FSB3_SampleHeader> entries = fsb3.SampleHeader.ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].Sound == entry.Sound)
                    {
                        entries[i] = entry;
                        fsb3.SampleHeader = entries.ToArray();
                        return;
                    }
            }
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType)
        {
            RemoveEntry(assetID);

            var temp = new GcnV2SoundWrapper(soundData, assetType);
            temp.SampleHeader.Sound = assetID;

            if (assetType == AssetType.Sound)
            {
                temp.SampleHeader.StreamedSound = false;
                if (Entry_Sounds == null)
                    Entry_Sounds = new FSB3_File();
                Entry_Sounds.Header.Mode.FlagValueInt &= 253;
                Entry_Sounds.Merge(new FSB3_SampleHeader[] { temp.SampleHeader});
            }
            else
            {
                temp.SampleHeader.StreamedSound = true;
                List<FSB3_File> newEntries = Entries_StreamingSounds.ToList();
                newEntries.Add(temp.ToFSB3());
                Entries_StreamingSounds = newEntries.ToArray();
            }
        }

        public void Merge(AssetSNDI_GCN_V2 assetSNDI)
        {
            if (assetSNDI.Entry_Sounds != null && assetSNDI.Entry_Sounds.SampleHeader.Length > 0)
            {
                if (Entry_Sounds == null)
                    Entry_Sounds = assetSNDI.Entry_Sounds;
                else
                    Entry_Sounds.Merge(assetSNDI.Entry_Sounds.SampleHeader);
            }

            List<FSB3_File> newEntries = Entries_StreamingSounds.ToList();
            newEntries.RemoveAll(entry => assetSNDI.Entries_StreamingSounds.Select(es => es.SampleHeader[0].Sound).Contains(entry.SampleHeader[0].Sound));
            newEntries.AddRange(assetSNDI.Entries_StreamingSounds);
            Entries_StreamingSounds = newEntries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            if (Entry_Sounds != null)
            {
                List<FSB3_SampleHeader> list = Entry_Sounds.SampleHeader.ToList();
                list.RemoveAll(entry => entry.Sound == assetID);
                Entry_Sounds.SampleHeader = list.ToArray();
            }

            List<FSB3_File> newEntries = Entries_StreamingSounds.ToList();
            newEntries.RemoveAll(entry => entry.SampleHeader[0].Sound == assetID);
            Entries_StreamingSounds = newEntries.ToArray();
        }
    }
}