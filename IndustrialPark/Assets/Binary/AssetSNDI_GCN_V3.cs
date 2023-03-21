using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSNDI_GCN_V2 : Asset
    {
        public override string AssetInfo => $"GameCube {game}, {Entries.Sum(e => e.soundEntries.Length)} entries";

        public int pFMusicMod { get; set; }
        public int pFSBFileArray { get; set; }
        public int pWavInfoArray { get; set; }
        public int pCutsceneAudioHeaders { get; set; }
        public ushort nWavFiles { get; set; }
        public ushort nSounds { get; set; }
        public ushort nStreams { get; set; }
        public byte nFSBFiles { get; set; }
        public byte nCutsceneAudioHeaders { get; set; }
        public FSB3_File[] Entries { get; set; }
        public EntrySoundInfo_GCN_V1[] Entries_Sound_CIN { get; set; }

        public AssetSNDI_GCN_V2(string assetName) : base(assetName, AssetType.SoundInfo)
        {
            Entries = new FSB3_File[0];
            Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[0];
        }

        public AssetSNDI_GCN_V2(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, Endianness.Big))
            {
                reader.ReadUInt32();
                var totalSize = reader.ReadUInt32();
                pFMusicMod = reader.ReadInt32();
                pFSBFileArray = reader.ReadInt32();
                pWavInfoArray = reader.ReadInt32();
                pCutsceneAudioHeaders = reader.ReadInt32();
                nWavFiles = reader.ReadUInt16();
                nSounds = reader.ReadUInt16();
                nStreams = reader.ReadUInt16();
                nFSBFiles = reader.ReadByte();
                nCutsceneAudioHeaders = reader.ReadByte();

                reader.BaseStream.Position = totalSize + 0x20;

                List<FSB3_File> entries = new List<FSB3_File>();
                for (int i = 0; i < nFSBFiles; i++)
                    using (var reader2 = new EndianBinaryReader(AHDR.data, Endianness.Little))
                    {
                        reader2.BaseStream.Position = reader.ReadUInt32() + 0x20;
                        entries.Add(new FSB3_File(reader2));
                    }

                for (int i = 0; i < nWavFiles; i++)
                {
                    var assetID = reader.ReadUInt32();
                    var uFlags = reader.ReadByte();
                    var uAudioSampleIndex = reader.ReadByte();
                    var uFSBIndex = reader.ReadByte();
                    var uSoundInfoIndex = reader.ReadByte();
                    entries[uFSBIndex].soundEntries[uAudioSampleIndex].SetEntryPartTwo(assetID, uFlags, uAudioSampleIndex, uFSBIndex, uSoundInfoIndex);
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
                writer.Write(Entries[i].ToByteArray(i));
                while (writer.BaseStream.Position % 0x20 != 0)
                    writer.BaseStream.Position++;
                writer.BaseStream.Position -= 0x04;
            }

            int footerOffset = (int)(writer.BaseStream.Position - 0x20);
            List<GcWavInfo> listForPart2 = new List<GcWavInfo>();

            for (int i = 0; i < Entries.Length; i++)
            {
                writer.Write(Entries[i].offset);
                for (int j = 0; j < Entries[i].numSamples; j++)
                {
                    Entries[i].soundEntries[j].uAudioSampleIndex = (byte)j;
                    Entries[i].soundEntries[j].uFSBIndex = (byte)i;
                    listForPart2.Add(Entries[i].soundEntries[j]);
                }
            }

            listForPart2 = listForPart2.OrderBy(f => f._assetID).ToList();
            foreach (GcWavInfo entry in listForPart2)
                entry.PartTwoToByteArray(writer);

            foreach (var entry in Entries_Sound_CIN)
                entry.Serialize(writer);

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
            writer.Write(nFSBFiles);
            writer.Write(nCutsceneAudioHeaders);
        }

        public byte[] GetHeader(uint assetID)
        {
            foreach (FSB3_File f in Entries)
                for (int i = 0; i < f.numSamples; i++)
                    if (f.soundEntries[i].Sound == assetID)
                        return f.SerializeSingleEntry(i);

            throw new Exception($"Error: SNDI asset does not contain sound header for asset [{assetID:X8}]");
        }

        public void AddEntry(byte[] soundData, uint assetID)
        {
            RemoveEntry(assetID);

            using (var reader2 = new EndianBinaryReader(soundData, Endianness.Little))
            {
                FSB3_File temp = new FSB3_File(reader2, true);
                List<FSB3_File> newEntries = Entries.ToList();

                if (temp.soundEntries[0].uFlags == 2)
                {
                    newEntries.Add(temp);
                }
                else
                {
                    if (newEntries.Count == 0)
                        newEntries.Add(new FSB3_File());
                    temp.soundEntries[0].Sound = assetID;
                    newEntries[0].Merge(temp);
                }

                Entries = newEntries.ToArray();
            }
        }

        public void Merge(AssetSNDI_GCN_V2 assetSNDI)
        {
            List<FSB3_File> newEntries = new List<FSB3_File>();

            FSB3_File first = Entries[0];
            first.Merge(assetSNDI.Entries[0]);

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
                List<GcWavInfo> soundEntries = entries[i].soundEntries.ToList();

                for (int j = 0; j < soundEntries.Count; j++)
                    if (soundEntries[j].Sound == assetID)
                        soundEntries.RemoveAt(j--);

                entries[i].soundEntries = soundEntries.ToArray();

                if (entries[i].numSamples == 0)
                    entries.RemoveAt(i--);
            }

            Entries = entries.ToArray();
        }

        public void Clean(IEnumerable<uint> assetIDs)
        {
            var fsb3s = Entries.ToList();

            for (int i = 0; i < fsb3s.Count; i++)
            {
                var entries = fsb3s[i].soundEntries.ToList();
                for (int j = 0; i < entries.Count; j++)
                    if (!assetIDs.Contains(entries[j].Sound))
                        entries.RemoveAt(j--);
                if (entries.Count == 0)
                    fsb3s.RemoveAt(i--);
                else
                    fsb3s[i].soundEntries = entries.ToArray();
            }

            Entries = fsb3s.ToArray();
        }
    }
}