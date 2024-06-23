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
        public FSB3_SampleHeader[] SampleHeader { get; set; }

        public FSB3_File()
        {
            Header = new FSB3_Header();
            SampleHeader = new FSB3_SampleHeader[0];
        }

        public FSB3_File(BinaryReader reader)
        {
            if ((reader.ReadChar() != 'F') ||
                (reader.ReadChar() != 'S') ||
                (reader.ReadChar() != 'B') ||
                (reader.ReadChar() != '3'))
                throw new Exception("Error reading FSB3 file");

            Header = new FSB3_Header(reader);

            SampleHeader = new FSB3_SampleHeader[Header.NumSamples];
            if ((Header.Mode.FlagValueInt & 2) == 0)
            {
                for (int i = 0; i < Header.NumSamples; i++)
                    SampleHeader[i] = new FSB3_SampleHeader(reader);
            }
            else
            {
                SampleHeader[0] = new FSB3_SampleHeader(reader);
                for (int i = 1; i < Header.NumSamples; i++)
                {
                    SampleHeader[i] = SampleHeader[0].Clone();
                    SampleHeader[i].SampleName = "empty";
                    SampleHeader[i].LengthSamples = reader.ReadInt32();
                    SampleHeader[i].LoopEnd = (uint)(SampleHeader[i].LengthSamples - 1);
                    SampleHeader[i].LengthCompressedBytes = reader.ReadInt32();

                    if ((SampleHeader[0].SampleHeaderMode.FlagValueInt & 0x02000000) == 0)
                        continue;

                    SampleHeader[i].GCADPCM = new FMOD_GcADPCMInfo[SampleHeader[0].NumChannels];
                    for (int j = 0; j < SampleHeader[0].NumChannels; j++)
                        SampleHeader[i].GCADPCM[j] = new FMOD_GcADPCMInfo(reader);
                }

            }
            for (int i = 0; i < Header.NumSamples; i++)
                SampleHeader[i].Data = reader.ReadBytes(SampleHeader[i].LengthCompressedBytes);
        }

        public int TotalFSBSize => 0x18 + Header.TotalHeadersSize + Header.TotalDataSize;

        public override void Serialize(EndianBinaryWriter writer)
        {
            using (var fsbWriter = new EndianBinaryWriter(Endianness.Little))
            {
                fsbWriter.BaseStream.Position = 0x18;

                Header.NumSamples = SampleHeader.Length;
                Header.TotalDataSize = 0;
                foreach (var se in SampleHeader)
                    Header.TotalDataSize += se.LengthCompressedBytes;

                for (int i = 0; i < SampleHeader.Length; i++)
                {
                    if ((Header.Mode.FlagValueInt & 2) != 0)
                    {
                        if (i == 0)
                            SampleHeader[0].Serialize(fsbWriter);
                        else
                        {
                            fsbWriter.Write(SampleHeader[i].LengthSamples);
                            fsbWriter.Write(SampleHeader[i].LengthCompressedBytes);
                            foreach (FMOD_GcADPCMInfo gcadpcm in SampleHeader[i].GCADPCM)
                                gcadpcm.Serialize(fsbWriter);
                        }
                    }
                    else
                        SampleHeader[i].Serialize(fsbWriter);
                }

                int totalHeadersSize = (int)fsbWriter.BaseStream.Position - 0x18;
                Header.TotalHeadersSize = totalHeadersSize;

                for (int i = 0; i < SampleHeader.Length; i++)
                    fsbWriter.Write(SampleHeader[i].Data);

                fsbWriter.BaseStream.Position = 0;
                Header.Serialize(fsbWriter);

                writer.Write(fsbWriter.ToArray());
            }
        }

        // This seems to be a known issue?
        // https://github.com/vgmstream/vgmstream/blob/2efdcb651f472e757c44ac2715aaef753c5e8774/src/meta/fsb.c#L295
        [Category("Other")]
        public byte[] UnknownBytes;

        public void Merge(FSB3_SampleHeader[] entries)
        {
            List<FSB3_SampleHeader> list = SampleHeader.ToList();
            list.RemoveAll(entry => entries.Select(entry2 => entry2.Sound).Contains(entry.Sound));
            list.AddRange(entries);
            UnknownBytes = null;
            SampleHeader = list.ToArray();
        }
    }
}