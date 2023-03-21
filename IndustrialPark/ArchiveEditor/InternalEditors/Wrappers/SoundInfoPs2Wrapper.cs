using System;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class SoundInfoPs2Wrapper
    {
        private byte[] magic;
        private int formatVersion;
        private int sourceStartOffset;
        private int waveformDataSize;
        public int sampleRate { get; set; }
        public short baseVolumeLeft { get; set; }
        public short baseVolumeRight { get; set; }
        public short basePitch { get; set; }
        public short baseADSR1 { get; set; }
        public short baseADSR2 { get; set; }
        private short reserved;
        private byte[] trackName;

        public SoundInfoPs2Wrapper(byte[] header)
        {
            using (var reader = new BinaryReader(new MemoryStream(header)))
            {
                magic = reader.ReadBytes(4);
                formatVersion = reader.ReadInt32();
                sourceStartOffset = reader.ReadInt32();
                waveformDataSize = reader.ReadInt32();
                sampleRate = reader.ReadInt32();
                baseVolumeLeft = reader.ReadInt16();
                baseVolumeRight = reader.ReadInt16();
                basePitch = reader.ReadInt16();
                baseADSR1 = reader.ReadInt16();
                baseADSR2 = reader.ReadInt16();
                reserved = reader.ReadInt16();
                trackName = reader.ReadBytes(16);
            }
            if (!Enumerable.SequenceEqual(header, ToByteArray()))
                throw new Exception("Unable to open sound editor");
        }

        public byte[] ToByteArray()
        {
            using (var writer = new BinaryWriter(new MemoryStream()))
            {
                writer.Write(magic);
                writer.Write(formatVersion);
                writer.Write(sourceStartOffset);
                writer.Write(waveformDataSize);
                writer.Write(sampleRate);
                writer.Write(baseVolumeLeft);
                writer.Write(baseVolumeRight);
                writer.Write(basePitch);
                writer.Write(baseADSR1);
                writer.Write(baseADSR2);
                writer.Write(reserved);
                writer.Write(trackName);
                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }
    }
}