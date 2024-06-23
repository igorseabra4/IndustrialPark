using HipHopFile;
using System;

namespace IndustrialPark
{
    public class GcnV2SoundWrapper
    {
        public FSB3_Header Header { get; private set; }
        public FSB3_SampleHeader SampleHeader { get; set; }

        public GcnV2SoundWrapper(FSB3_Header header, FSB3_SampleHeader sampleHeader)
        {
            Header = header.Clone();
            SampleHeader = sampleHeader.Clone();
        }

        public GcnV2SoundWrapper(byte[] data, AssetType assetType)
        {
            using (var reader = new EndianBinaryReader(data, Endianness.Little))
            {
                FSB3_File temp = new FSB3_File(reader);
                Header = temp.Header;
                SampleHeader = temp.SampleHeader[0];

                if (assetType == AssetType.SoundStream)
                    SampleHeader.SetEntryData(0, 2, 0, 0, 0);
            }
        }

        public byte[] Serialize()
        {
            using (var writer = new EndianBinaryWriter(Endianness.Little))
            {
                ToFSB3().Serialize(writer);
                return writer.ToArray();
            }

        }

        public FSB3_File ToFSB3()
        {
            return new FSB3_File()
            {
                Header = Header,
                SampleHeader = new FSB3_SampleHeader[] { SampleHeader },
            };
        }
    }
}