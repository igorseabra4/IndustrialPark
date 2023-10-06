using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSNDI_GCN_V2 : Asset
    {
        public override string AssetInfo => $"GameCube {game}, {(Entry_Sounds == null ? 0 : Entry_Sounds.SoundEntries.Length) + Entries_StreamingSounds.Length} entries";

        private const string categoryName = "Sound Info";

        [Category(categoryName), Description("Usually 0.")]
        public int pFMusicMod { get; set; }
        [Category(categoryName), Description("Usually 0.")]
        public int pFSBFileArray { get; set; }
        [Category(categoryName), Description("Usually 0.")]
        public int pWavInfoArray { get; set; }
        [Category(categoryName), Description("Usually 0.")]
        public int pCutsceneAudioHeaders { get; set; }

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
                pFMusicMod = reader.ReadInt32();
                pFSBFileArray = reader.ReadInt32();
                pWavInfoArray = reader.ReadInt32();
                pCutsceneAudioHeaders = reader.ReadInt32();
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
                        fsbReader.BaseStream.Position = (i + 1 < offsets.Length) ? (offsets[i + 1] - 0x08) : (totalSize + 0x18);
                        fsb3file.UnknownEndValue1 = fsbReader.ReadInt32();
                        fsb3file.UnknownEndValue2 = fsbReader.ReadInt32();
                        entries.Add(fsb3file);
                    }

                for (int i = 0; i < nWavFiles; i++)
                {
                    var assetID = reader.ReadUInt32();
                    var uFlags = reader.ReadByte();
                    var uAudioSampleIndex = reader.ReadByte();
                    var uFSBIndex = reader.ReadByte();
                    var uSoundInfoIndex = reader.ReadByte();
                    entries[uFSBIndex].SoundEntries[uAudioSampleIndex].SetEntryData(assetID, uFlags, uAudioSampleIndex, uFSBIndex, uSoundInfoIndex);
                }

                if (entries.Count == 0 || (entries.Count == 1 && entries[0].SoundEntries.Length == 0))
                {
                    Entry_Sounds = null;
                    Entries_StreamingSounds = new FSB3_File[0];
                }
                else if (entries.Count == 1)
                {
                    if (entries[0].SoundEntries[0].StreamedSound)
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
            if (Entry_Sounds != null && Entry_Sounds.SoundEntries.Length > 0)
                result.Add(Entry_Sounds);
            result.AddRange(Entries_StreamingSounds);
            return result;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var Entries = GetAllEntries();

            writer.BaseStream.Position = 0x20;
            for (int i = 0; i < Entries.Count; i++)
            {
                Entries[i].offset = (int)(writer.BaseStream.Position - 0x20);
                Entries[i].Serialize(writer);

                while (writer.BaseStream.Position % 0x20 != 0)
                    writer.BaseStream.Position++;

                if (Entries[i].UnknownEndValue1 != 0 || Entries[i].UnknownEndValue2 != 0)
                {
                    writer.BaseStream.Position -= 8;
                    var ukbs = BitConverter.GetBytes(Entries[i].UnknownEndValue1);
                    writer.Write(ukbs[0]);
                    writer.Write(ukbs[1]);
                    writer.Write(ukbs[2]);
                    writer.Write(ukbs[3]);

                    ukbs = BitConverter.GetBytes(Entries[i].UnknownEndValue2);
                    writer.Write(ukbs[0]);
                    writer.Write(ukbs[1]);
                    writer.Write(ukbs[2]);
                    writer.Write(ukbs[3]);
                }
            }

            int footerOffset = (int)(writer.BaseStream.Position - 0x20);
            List<GcWavInfo> gcWavInfos = new List<GcWavInfo>();

            for (int i = 0; i < Entries.Count; i++)
            {
                writer.Write(Entries[i].offset);
                for (int j = 0; j < Entries[i].Header.NumSamples; j++)
                {
                    Entries[i].SoundEntries[j].uAudioSampleIndex = (byte)j;
                    Entries[i].SoundEntries[j].uFSBIndex = (byte)i;
                    gcWavInfos.Add(Entries[i].SoundEntries[j]);
                }
            }

            gcWavInfos = gcWavInfos.OrderBy(f => f._assetID).ToList();
            foreach (GcWavInfo entry in gcWavInfos)
                entry.Serialize(writer);

            foreach (var entry in Entries_Sound_CIN)
                entry.Serialize(writer);

            ushort nWavFiles = (ushort)Entries.Sum(e => e.SoundEntries.Length);
            ushort nSounds = (ushort)Entries.Sum(e => e.SoundEntries.Count(se => !se.StreamedSound));
            ushort nStreams = (ushort)Entries.Sum(e => e.SoundEntries.Count(se => se.StreamedSound));

            writer.BaseStream.Position = 0;
            writer.Write(assetID);
            writer.Write(footerOffset);
            writer.Write(pFMusicMod);
            writer.Write(pFSBFileArray);
            writer.Write(pWavInfoArray);
            writer.Write(pCutsceneAudioHeaders);
            writer.Write(nWavFiles);
            writer.Write(nSounds);
            writer.Write(nStreams);
            writer.Write((byte)Entries.Count);
            writer.Write((byte)Entries_Sound_CIN.Length);
        }

        public byte[] GetHeader(uint assetID)
        {
            foreach (FSB3_File f in GetAllEntries())
                foreach (var se in f.SoundEntries)
                    if (se.Sound == assetID)
                        return new GcnV2SoundWrapper(f.Header, f.SampleHeader, se).Serialize();

            throw new Exception($"Error: SNDI asset does not contain sound header for asset [{assetID:X8}]");
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType)
        {
            RemoveEntry(assetID);

            var temp = new GcnV2SoundWrapper(soundData, assetType);
            temp.SoundEntry.Sound = assetID;

            if (assetType == AssetType.Sound)
            {
                temp.SoundEntry.StreamedSound = false;
                if (Entry_Sounds == null)
                    Entry_Sounds = new FSB3_File();
                Entry_Sounds.Merge(new GcWavInfo[] { temp.SoundEntry });
            }
            else
            {
                temp.SoundEntry.StreamedSound = true;
                List<FSB3_File> newEntries = Entries_StreamingSounds.ToList();
                newEntries.Add(temp.ToFSB3());
                Entries_StreamingSounds = newEntries.ToArray();
            }
        }

        public void Merge(AssetSNDI_GCN_V2 assetSNDI)
        {
            if (assetSNDI.Entry_Sounds != null && assetSNDI.Entry_Sounds.SoundEntries.Length > 0)
            {
                if (Entry_Sounds == null)
                    Entry_Sounds = assetSNDI.Entry_Sounds;
                else
                    Entry_Sounds.Merge(assetSNDI.Entry_Sounds.SoundEntries);
            }

            List<FSB3_File> newEntries = Entries_StreamingSounds.ToList();
            newEntries.RemoveAll(entry => assetSNDI.Entries_StreamingSounds.Select(es => es.SoundEntries[0]._assetID).Contains(entry.SoundEntries[0]._assetID));
            newEntries.AddRange(assetSNDI.Entries_StreamingSounds);
            Entries_StreamingSounds = newEntries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            if (Entry_Sounds != null)
            {
                List<GcWavInfo> list = Entry_Sounds.SoundEntries.ToList();
                list.RemoveAll(entry => entry._assetID == assetID);
                Entry_Sounds.SoundEntries = list.ToArray();
            }

            List<FSB3_File> newEntries = Entries_StreamingSounds.ToList();
            newEntries.RemoveAll(entry => entry.SoundEntries[0]._assetID == assetID);
            Entries_StreamingSounds = newEntries.ToArray();
        }
    }
}