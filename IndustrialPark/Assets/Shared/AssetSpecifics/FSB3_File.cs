using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static DiscordRPC.User;

namespace IndustrialPark
{
    public class FSB3_File : GenericAssetDataContainer
    {
        public override void Serialize(EndianBinaryWriter writer) { }

        // header
        // char4
        [Category("FSB3 Header")]
        public int numSamples => soundEntries.Length;
        [Category("FSB3 Header")]
        public int totalHeadersSize => 72 + numSamples * 0x36;
        [Category("FSB3 Header")]
        public int totalDataSize
        {
            get
            {
                int acc = 0;

                for (int i = 0; i < numSamples; i++)
                    acc += soundEntries[i].lengthcompressedbytes;

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
                if (soundEntries.Length > 0)
                    return soundEntries[0].lengthsamples;
                return 0;
            }
        }
        [Category("Sample Header")]
        public int lengthcompressedbytes
        {
            get
            {
                if (soundEntries.Length > 0)
                    return soundEntries[0].lengthcompressedbytes;
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
                if (soundEntries.Length > 0)
                    return (uint)(soundEntries[0].lengthsamples - 1);
                return 0;
            }
        }
        [Category("Sample Header")]
        public uint sampleHeaderMode { get; set; }
        [Category("Sample Header")]
        public int deffreq { get; set; }
        [Category("Sample Header")]
        public ushort defvol { get; set; }
        [Category("Sample Header")]
        public short defpan { get; set; }
        [Category("Sample Header")]
        public ushort defpri { get; set; }
        [Category("Sample Header")]
        public ushort numchannels { get; set; }
        [Category("Sample Header")]
        public float mindistance { get; set; }
        [Category("Sample Header")]
        public float maxdistance { get; set; }

        [Category("Other")]
        public GcWavInfo[] soundEntries { get; set; }

        public FSB3_File()
        {
            mode = 2;
            version = 196609;

            deffreq = 32000;
            defpan = 128;
            defpri = 255;
            defvol = 255;
            mindistance = 1f;
            maxdistance = 1000000f;
            name = "empty";
            numchannels = 1;
            sampleHeaderMode = 33558561;
            size = 126;

            soundEntries = new GcWavInfo[0];
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
            sampleHeaderMode = reader.ReadUInt32();
            deffreq = reader.ReadInt32();
            defvol = reader.ReadUInt16();
            defpan = reader.ReadInt16();
            defpri = reader.ReadUInt16();
            numchannels = reader.ReadUInt16();
            mindistance = reader.ReadSingle();
            maxdistance = reader.ReadSingle();

            soundEntries = new GcWavInfo[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                soundEntries[i] = new GcWavInfo();
                soundEntries[i].SetEntryPartOne(reader);
            }

            if (numSamples > 0)
            {
                soundEntries[0].lengthsamples = lengthsamples;
                soundEntries[0].templengthcompressedbytes = templengthcompressedbytes;
            }

            for (int i = 0; i < numSamples; i++)
                soundEntries[i].Data = reader.ReadBytes(soundEntries[i].templengthcompressedbytes);

            if (condensed)
            {
                var assetID = reader.ReadUInt32();
                var uFlags = reader.ReadByte();
                var uAudioSampleIndex = reader.ReadByte();
                var uFSBIndex = reader.ReadByte();
                var uSoundInfoIndex = reader.ReadByte();
                soundEntries[uAudioSampleIndex].SetEntryPartTwo(assetID, uFlags, uAudioSampleIndex, uFSBIndex, uSoundInfoIndex);
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
                writer.Write(sampleHeaderMode);
                writer.Write(deffreq);
                writer.Write(defvol);
                writer.Write(defpan);
                writer.Write(defpri);
                writer.Write(numchannels);
                writer.Write(mindistance);
                writer.Write(maxdistance);

                if (everyEntry)
                {
                    for (int i = 0; i < numSamples; i++)
                    {
                        soundEntries[i].PartOneToByteArray(writer, i);
                        soundEntries[i].uAudioSampleIndex = (byte)i;
                        soundEntries[i].uFSBIndex = (byte)index;
                    }
                    for (int i = 0; i < soundEntries.Length; i++)
                        writer.Write(soundEntries[i].Data);
                }
                else
                {
                    soundEntries[entryIndex].PartOneToByteArray(writer, -1);
                    soundEntries[entryIndex].uAudioSampleIndex = 0;
                    soundEntries[entryIndex].uFSBIndex = 0;
                    writer.Write(soundEntries[entryIndex].Data);
                }

                if (!everyEntry)
                {
                    writer.Write(soundEntries[entryIndex].Sound);
                    writer.Write(soundEntries[entryIndex].uFlags);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write(soundEntries[entryIndex].uSoundInfoIndex);
                }
                
                return writer.ToArray();
            }
        }

        [Category("Other"), ReadOnly(true)]
        public int offset { get; set; }

        public void Merge(FSB3_File file)
        {
            List<GcWavInfo> list = soundEntries.ToList();

            List<uint> existingSounds = new List<uint>(soundEntries.Length);
            foreach (GcWavInfo s in soundEntries)
                existingSounds.Add(s.Sound);

            foreach (GcWavInfo s in file.soundEntries)
                if (!existingSounds.Contains(s.Sound))
                    list.Add(s);

            soundEntries = list.ToArray();
        }

        public byte[] SerializeSingleEntry(int index) => ToByteArray(0, false, index);
    }
}