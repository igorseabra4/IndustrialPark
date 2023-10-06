using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FSB3_File : GenericAssetDataContainer
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FSB3_Header Header { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FSB3_SampleHeader SampleHeader { get; set; }
        public GcWavInfo[] SoundEntries { get; set; }

        public FSB3_File()
        {
            Header = new FSB3_Header();
            SampleHeader = new FSB3_SampleHeader();
            SoundEntries = new GcWavInfo[0];
        }

        public FSB3_File(BinaryReader reader)
        {
            if ((reader.ReadChar() != 'F') ||
                (reader.ReadChar() != 'S') ||
                (reader.ReadChar() != 'B') ||
                (reader.ReadChar() != '3'))
                throw new Exception("Error reading FSB3 file");

            Header = new FSB3_Header(reader);

            SampleHeader = new FSB3_SampleHeader(reader);

            SoundEntries = new GcWavInfo[Header.NumSamples];
            for (int i = 0; i < SoundEntries.Length; i++)
            {
                SoundEntries[i] = new GcWavInfo();

                SoundEntries[i].BasicSampleHeader = i == 0 ?
                new FSB3_SampleHeaderBasic(SampleHeader.LengthSamples, SampleHeader.LengthCompressedBytes) :
                   new FSB3_SampleHeaderBasic(reader);

                SoundEntries[i].GcAdpcmInfos = new FMOD_GcADPCMInfo[SampleHeader.NumChannels];
                for (int j = 0; j < SoundEntries[i].GcAdpcmInfos.Length; j++)
                    SoundEntries[i].GcAdpcmInfos[j] = new FMOD_GcADPCMInfo(reader);
            }

            for (int i = 0; i < SoundEntries.Length; i++)
                SoundEntries[i].Data = reader.ReadBytes(SoundEntries[i].BasicSampleHeader.LengthCompressedBytes);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            using (var fsbWriter = new EndianBinaryWriter(Endianness.Little))
            {
                fsbWriter.Write((byte)'F');
                fsbWriter.Write((byte)'S');
                fsbWriter.Write((byte)'B');
                fsbWriter.Write((byte)'3');

                SampleHeader.LengthSamples = SoundEntries.Length > 0 ? SoundEntries[0].BasicSampleHeader.LengthSamples : 0;
                SampleHeader.LengthCompressedBytes = SoundEntries.Length > 0 ? SoundEntries[0].BasicSampleHeader.LengthCompressedBytes : 0;

                Header.NumSamples = SoundEntries.Length;
                Header.TotalDataSize = 0;
                foreach (var se in SoundEntries)
                    Header.TotalDataSize += se.BasicSampleHeader.LengthCompressedBytes;

                Header.Serialize(fsbWriter);

                int totalHeadersSize = (int)fsbWriter.BaseStream.Position;

                SampleHeader.Serialize(fsbWriter);

                for (int i = 0; i < SoundEntries.Length; i++)
                {
                    if (i != 0)
                        SoundEntries[i].BasicSampleHeader.Serialize(fsbWriter);

                    for (int j = 0; j < SoundEntries[i].GcAdpcmInfos.Length; j++)
                        SoundEntries[i].GcAdpcmInfos[j].Serialize(fsbWriter);
                }

                totalHeadersSize = (int)fsbWriter.BaseStream.Position - totalHeadersSize;
                Header.TotalHeadersSize = totalHeadersSize;

                for (int i = 0; i < SoundEntries.Length; i++)
                    fsbWriter.Write(SoundEntries[i].Data);

                fsbWriter.BaseStream.Position = 4;
                Header.Serialize(fsbWriter);

                writer.Write(fsbWriter.ToArray());
            }
        }

        [Category("Other"), ReadOnly(true)]
        public int offset { get; set; }
        [Category("Other")]
        public int UnknownEndValue1 { get; set; }
        [Category("Other")]
        public int UnknownEndValue2 { get; set; }

        public void Merge(GcWavInfo[] soundEntries)
        {
            List<GcWavInfo> list = SoundEntries.ToList();
            list.RemoveAll(entry => soundEntries.Select(entry2 => entry2._assetID).Contains(entry._assetID));
            list.AddRange(soundEntries);
            SoundEntries = list.ToArray();
        }
    }
}