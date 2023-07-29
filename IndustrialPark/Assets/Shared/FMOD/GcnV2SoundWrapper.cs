using HipHopFile;
using System;

namespace IndustrialPark
{
    public class GcnV2SoundWrapper
    {
        public FSB3_Header Header { get; private set; }
        public FSB3_SampleHeader SampleHeader { get; set; }
        public GcWavInfo SoundEntry { get; set; }

        public GcnV2SoundWrapper(FSB3_Header header, FSB3_SampleHeader sampleHeader, GcWavInfo soundEntry)
        {
            Header = header.Clone();
            SampleHeader = sampleHeader.Clone();
            SoundEntry = soundEntry.Clone();
        }

        public GcnV2SoundWrapper(byte[] data, AssetType assetType)
        {
            using (var reader = new EndianBinaryReader(data, Endianness.Little))
            {
                if ((reader.ReadChar() != 'F') ||
                (reader.ReadChar() != 'S') ||
                (reader.ReadChar() != 'B') ||
                (reader.ReadChar() != '3'))
                    throw new Exception("Error reading FSB3 file");

                Header = new FSB3_Header(reader);

                SampleHeader = new FSB3_SampleHeader(reader);

                SoundEntry = new GcWavInfo();

                SoundEntry.BasicSampleHeader = new FSB3_SampleHeaderBasic(SampleHeader.LengthSamples, SampleHeader.LengthCompressedBytes);

                SoundEntry.GcAdpcmInfos = new FMOD_GcADPCMInfo[SampleHeader.NumChannels];
                for (int j = 0; j < SoundEntry.GcAdpcmInfos.Length; j++)
                    SoundEntry.GcAdpcmInfos[j] = new FMOD_GcADPCMInfo(reader);

                SoundEntry.Data = reader.ReadBytes(SoundEntry.BasicSampleHeader.LengthCompressedBytes);

                try
                {
                    var assetID = reader.ReadUInt32();
                    var uFlags = reader.ReadByte();
                    var uAudioSampleIndex = reader.ReadByte();
                    var uFSBIndex = reader.ReadByte();
                    var uSoundInfoIndex = reader.ReadByte();

                    SoundEntry.SetEntryData(assetID, uFlags, uAudioSampleIndex, uFSBIndex, uSoundInfoIndex);
                }
                catch
                {
                    if (assetType == AssetType.SoundStream)
                        SoundEntry.SetEntryData(0, 2, 0, 0, 0);
                }
            }
        }

        public byte[] Serialize()
        {
            using (var writer = new EndianBinaryWriter(Endianness.Little))
            {
                writer.Write((byte)'F');
                writer.Write((byte)'S');
                writer.Write((byte)'B');
                writer.Write((byte)'3');

                SampleHeader.LengthSamples = SoundEntry.BasicSampleHeader.LengthSamples;
                SampleHeader.LengthCompressedBytes = SoundEntry.BasicSampleHeader.LengthCompressedBytes;

                Header.NumSamples = 1;
                Header.TotalDataSize = SoundEntry.BasicSampleHeader.LengthCompressedBytes;

                Header.Serialize(writer);

                int totalHeadersSize = (int)writer.BaseStream.Position;

                SampleHeader.Serialize(writer);

                for (int i = 0; i < SoundEntry.GcAdpcmInfos.Length; i++)
                    SoundEntry.GcAdpcmInfos[i].Serialize(writer);

                totalHeadersSize = (int)writer.BaseStream.Position - totalHeadersSize;
                Header.TotalHeadersSize = totalHeadersSize;

                writer.Write(SoundEntry.Data);

                SoundEntry.Serialize(writer);

                writer.BaseStream.Position = 4;
                Header.Serialize(writer);

                return writer.ToArray();
            }
        }

        public FSB3_File ToFSB3()
        {
            return new FSB3_File()
            {
                Header = Header,
                SampleHeader = SampleHeader,
                SoundEntries = new GcWavInfo[] { SoundEntry }
            };
        }
    }
}