using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class FSB3_File : GenericAssetDataContainer
    {
        public override void Serialize(EndianBinaryWriter writer) { }

        // header
        // char4
        [Category("FSB3 Header")]
        public int numSamples => SoundEntries.Length;
        [Category("FSB3 Header")]
        public int totalHeadersSize => 72 + numSamples * 0x36;
        [Category("FSB3 Header")]
        public int totalDataSize
        {
            get
            {
                int acc = 0;

                for (int i = 0; i < numSamples; i++)
                    acc += SoundEntries[i].lengthcompressedbytes;

                return acc;
            }
        }
        [Category("FSB3 Header")]
        public int version { get; set; }
        [Category("FSB3 Header")]
        public int mode { get; set; }

        // sample header
        [Category("Sample Header")]
        public ushort size { get; set; }
        [Category("Sample Header")]
        public string name { get; set; }
        [Category("Sample Header")]
        public int lengthsamples
        {
            get
            {
                if (SoundEntries.Length > 0)
                    return SoundEntries[0].lengthsamples;
                return 0;
            }
        }
        [Category("Sample Header")]
        public int lengthcompressedbytes
        {
            get
            {
                if (SoundEntries.Length > 0)
                    return SoundEntries[0].lengthcompressedbytes;
                return 0;
            }
        }
        [Category("Sample Header")]
        public uint loopstart { get; set; }
        [Category("Sample Header")]
        public uint loopend
        {
            get
            {
                if (SoundEntries.Length > 0)
                    return (uint)(SoundEntries[0].lengthsamples - 1);
                return 0;
            }
        }
        [Category("Sample Header")]
        public FlagBitmask sampleHeaderMode { get; set; } = IntFlagsDescriptor(
            "FSOUND_LOOP_OFF",
            null,
            null,
            null,
            "FSOUND_16BITS",
            "FSOUND_MONO",
            "FSOUND_STEREO",
            null,
            "FSOUND_SIGNED",
            "FSOUND_MPEG",
            null,
            null,
            "FSOUND_HW3D",
            "FSOUND_2D",
            "FSOUND_SYNCPOINTS_NONAMES",
            "FSOUND_DUPLICATE",
            null,
            null,
            null,
            "FSOUND_HW2D",
            "FSOUND_3D",
            null,
            "FSOUND_IMAADPCM",
            null,
            "FSOUND_XMA",
            "FSOUND_GCADPCM",
            "FSOUND_MULTICHANNEL",
            null,
            "FSOUND_MPEG_LAYER3",
            "FSOUND_IMAADPCMSTEREO",
            null,
            "FSOUND_SYNCPOINTS");

        [Category("Sample Header")]
        public int Frequency { get; set; }
        [Category("Sample Header")]
        public ushort Volume { get; set; }
        [Category("Sample Header")]
        public short Pan { get; set; }
        [Category("Sample Header")]
        public ushort Priority { get; set; }
        [Category("Sample Header")]
        public ushort NumChannels { get; set; }
        [Category("Sample Header")]
        public float MinDistance { get; set; }
        [Category("Sample Header")]
        public float MaxDistance { get; set; }

        [Category("Sound Entries")]
        public GcWavInfo[] SoundEntries { get; set; }

        public FSB3_File()
        {
            mode = 2;
            version = 196609;

            Frequency = 32000;
            Pan = 128;
            Priority = 255;
            Volume = 255;
            MinDistance = 1f;
            MaxDistance = 1000000f;
            name = "empty";
            NumChannels = 1;
            sampleHeaderMode.FlagValueInt = 33558561;
            size = 126;

            SoundEntries = new GcWavInfo[0];
        }

        public FSB3_File(BinaryReader reader, bool condensed = false)
        {
            if ((reader.ReadChar() != 'F') ||
                (reader.ReadChar() != 'S') ||
                (reader.ReadChar() != 'B') ||
                (reader.ReadChar() != '3'))
                throw new Exception("Error reading FSB3 file");

            int numSamples = reader.ReadInt32();
            int totalHeadersSize = reader.ReadInt32();
            int totalDataSize = reader.ReadInt32();
            version = reader.ReadInt32();
            mode = reader.ReadInt32();

            size = reader.ReadUInt16();
            name = new string(reader.ReadChars(30));
            int lengthsamples = reader.ReadInt32();
            int templengthcompressedbytes = reader.ReadInt32();
            loopstart = reader.ReadUInt32();
            uint loopend = reader.ReadUInt32();
            sampleHeaderMode.FlagValueInt = reader.ReadUInt32();
            Frequency = reader.ReadInt32();
            Volume = reader.ReadUInt16();
            Pan = reader.ReadInt16();
            Priority = reader.ReadUInt16();
            NumChannels = reader.ReadUInt16();
            MinDistance = reader.ReadSingle();
            MaxDistance = reader.ReadSingle();

            SoundEntries = new GcWavInfo[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                SoundEntries[i] = new GcWavInfo();
                SoundEntries[i].SetEntryPartOne(reader);
            }

            if (numSamples > 0)
            {
                SoundEntries[0].lengthsamples = lengthsamples;
                SoundEntries[0].templengthcompressedbytes = templengthcompressedbytes;
            }

            for (int i = 0; i < numSamples; i++)
                SoundEntries[i].Data = reader.ReadBytes(SoundEntries[i].templengthcompressedbytes);

            if (condensed)
            {
                var assetID = reader.ReadUInt32();
                var uFlags = reader.ReadByte();
                var uAudioSampleIndex = reader.ReadByte();
                var uFSBIndex = reader.ReadByte();
                var uSoundInfoIndex = reader.ReadByte();
                SoundEntries[uAudioSampleIndex].SetEntryPartTwo(assetID, uFlags, uAudioSampleIndex, uFSBIndex, uSoundInfoIndex);
            }
        }

        public byte[] ToByteArray(int index, bool everyEntry = true, int entryIndex = -1)
        {
            using (var writer = new EndianBinaryWriter(Endianness.Little))
            {
                writer.Write((byte)'F');
                writer.Write((byte)'S');
                writer.Write((byte)'B');
                writer.Write((byte)'3');
                writer.Write(numSamples);
                writer.Write(totalHeadersSize);
                writer.Write(totalDataSize);
                writer.Write(version);
                writer.Write(mode);

                writer.Write(size);
                for (int i = 0; i < 30; i++)
                    if (i < name.Length)
                        writer.Write((byte)name[i]);
                    else
                        writer.Write((byte)0);
                writer.Write(lengthsamples);
                writer.Write(lengthcompressedbytes);
                writer.Write(loopstart);
                writer.Write(loopend);
                writer.Write(sampleHeaderMode.FlagValueInt);
                writer.Write(Frequency);
                writer.Write(Volume);
                writer.Write(Pan);
                writer.Write(Priority);
                writer.Write(NumChannels);
                writer.Write(MinDistance);
                writer.Write(MaxDistance);

                if (everyEntry)
                {
                    for (int i = 0; i < numSamples; i++)
                    {
                        SoundEntries[i].PartOneToByteArray(writer, i);
                        SoundEntries[i].uAudioSampleIndex = (byte)i;
                        SoundEntries[i].uFSBIndex = (byte)index;
                    }
                    for (int i = 0; i < SoundEntries.Length; i++)
                        writer.Write(SoundEntries[i].Data);
                }
                else
                {
                    SoundEntries[entryIndex].PartOneToByteArray(writer, -1);
                    SoundEntries[entryIndex].uAudioSampleIndex = 0;
                    SoundEntries[entryIndex].uFSBIndex = 0;
                    writer.Write(SoundEntries[entryIndex].Data);
                }

                if (!everyEntry)
                {
                    writer.Write(SoundEntries[entryIndex].Sound);
                    writer.Write(SoundEntries[entryIndex].uFlags);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write(SoundEntries[entryIndex].uSoundInfoIndex);
                }

                return writer.ToArray();
            }
        }

        [Category("Other"), ReadOnly(true)]
        public int offset { get; set; }
        [Category("Other")]
        public int UnknownEndValue1 { get; set; }
        [Category("Other")]
        public int UnknownEndValue2 { get; set; }

        public void Merge(FSB3_File file)
        {
            List<GcWavInfo> list = SoundEntries.ToList();

            List<uint> existingSounds = new List<uint>(SoundEntries.Length);
            foreach (GcWavInfo s in SoundEntries)
                existingSounds.Add(s.Sound);

            foreach (GcWavInfo s in file.SoundEntries)
                if (!existingSounds.Contains(s.Sound))
                    list.Add(s);

            SoundEntries = list.ToArray();
        }

        public byte[] SerializeSingleEntry(int index) => ToByteArray(0, false, index);
    }
}