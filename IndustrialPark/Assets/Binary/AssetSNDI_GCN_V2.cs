using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSNDI_GCN_V2 : Asset
    {
        public override string AssetInfo => $"GameCube {game}, {Entries.Sum(e => e.SoundEntries.Length)} entries";

        private const string categoryName = "Sound Info";

        private uint _assetId; // hacky solution to weird issue

        [Category(categoryName), Description("Usually 0.")]
        public int pFMusicMod { get; set; }
        [Category(categoryName), Description("Usually 0.")]
        public int pFSBFileArray { get; set; }
        [Category(categoryName), Description("Usually 0.")]
        public int pWavInfoArray { get; set; }
        [Category(categoryName), Description("Usually 0.")]
        public int pCutsceneAudioHeaders { get; set; }

        [Category(categoryName)]
        public FSB3_File[] Entries { get; set; }

        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_Sound_CIN { get; set; }

        public AssetSNDI_GCN_V2(string assetName) : base(assetName, AssetType.SoundInfo)
        {
            Entries = new FSB3_File[0];
            Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[0];
        }

        public AssetSNDI_GCN_V2(Section_AHDR AHDR, Game game) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Big))
            {
                _assetId = reader.ReadUInt32();
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

                Entries = entries.ToArray();

                Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[nCutsceneAudioHeaders];
                for (int i = 0; i < Entries_Sound_CIN.Length; i++)
                    Entries_Sound_CIN[i] = new EntrySoundInfo_GCN_V1(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.BaseStream.Position = 0x20;
            for (int i = 0; i < Entries.Length; i++)
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

            for (int i = 0; i < Entries.Length; i++)
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
            ushort nSounds = (ushort)Entries.Sum(e => e.SoundEntries.Sum(se => ((se.uFlags & 2) == 0) ? 1 : 0));
            ushort nStreams = (ushort)(nWavFiles - nSounds);

            writer.BaseStream.Position = 0;
            writer.Write(_assetId);
            writer.Write(footerOffset);
            writer.Write(pFMusicMod);
            writer.Write(pFSBFileArray);
            writer.Write(pWavInfoArray);
            writer.Write(pCutsceneAudioHeaders);
            writer.Write(nWavFiles);
            writer.Write(nSounds);
            writer.Write(nStreams);
            writer.Write((byte)Entries.Length);
            writer.Write((byte)Entries_Sound_CIN.Length);
        }

        public byte[] GetHeader(uint assetID)
        {
            _assetId = assetID;

            foreach (FSB3_File f in Entries)
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

            List<FSB3_File> newEntries = Entries.ToList();

            if (temp.SoundEntry.StreamedSound)
            {
                newEntries.Add(temp.ToFSB3());
            }
            else if (newEntries.Count == 0 || (newEntries[0].SoundEntries.Length > 0 && newEntries[0].SoundEntries[0].StreamedSound))
            {
                var newNewEntries = new List<FSB3_File> { temp.ToFSB3() };
                newNewEntries.AddRange(newEntries);
                newEntries = newNewEntries;
            }
            else
            {
                newEntries[0].Merge(new GcWavInfo[] { temp.SoundEntry });
            }

            Entries = newEntries.ToArray();
        }

        public void Merge(AssetSNDI_GCN_V2 assetSNDI)
        {
            List<FSB3_File> newEntries = new List<FSB3_File>();

            FSB3_File first = Entries[0];
            first.Merge(assetSNDI.Entries[0].SoundEntries);

            newEntries.Add(first);

            for (int i = 1; i < Entries.Length; i++)
                newEntries.Add(Entries[i]);
            for (int i = 1; i < assetSNDI.Entries.Length; i++)
                newEntries.Add(assetSNDI.Entries[i]);

            Entries = newEntries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            List<FSB3_File> entries = Entries.ToList();

            for (int i = 0; i < entries.Count; i++)
            {
                List<GcWavInfo> soundEntries = entries[i].SoundEntries.ToList();

                for (int j = 0; j < soundEntries.Count; j++)
                    if (soundEntries[j].Sound == assetID)
                        soundEntries.RemoveAt(j--);

                entries[i].SoundEntries = soundEntries.ToArray();

                if (entries[i].SoundEntries.Length == 0)
                    entries.RemoveAt(i--);
            }

            Entries = entries.ToArray();
            _assetId = assetID;
        }

        public void Clean(IEnumerable<uint> assetIDs)
        {
            var fsb3s = Entries.ToList();

            for (int i = 0; i < fsb3s.Count; i++)
            {
                var entries = fsb3s[i].SoundEntries.ToList();
                for (int j = 0; i < entries.Count; j++)
                    if (!assetIDs.Contains(entries[j].Sound))
                        entries.RemoveAt(j--);
                if (entries.Count == 0)
                    fsb3s.RemoveAt(i--);
                else
                    fsb3s[i].SoundEntries = entries.ToArray();
            }

            Entries = fsb3s.ToArray();
            _assetId = assetID;
        }
    }
}