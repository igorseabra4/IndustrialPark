using System.ComponentModel;
using System.IO;

namespace IndustrialPark
{
    public class FSB3_Header : GenericAssetDataContainer
    {
        [Browsable(false)]
        public int NumSamples { get; set; }

        [Browsable(false)]
        public int TotalHeadersSize { get; set; }

        [Browsable(false)]
        public int TotalDataSize { get; set; }

        [ReadOnly(true)]
        public int Version { get; set; }

        [ReadOnly(true)]
        public FlagBitmask Mode { get; set; } = IntFlagsDescriptor(
            null,
            "FMOD_FSB_SOURCE_BASICHEADERS",
            null,
            null,
            "FMOD_FSB_SOURCE_NOTINTERLEAVED",
            "FMOD_FSB_SOURCE_MPEG_PADDED",
            "FMOD_FSB_SOURCE_MPEG_PADDED4");

        public FSB3_Header()
        {
            NumSamples = 0;
            TotalHeadersSize = 0;
            TotalDataSize = 0;
            Version = 196609;
            Mode.FlagValueInt = 2;
        }

        public FSB3_Header(BinaryReader reader)
        {
            NumSamples = reader.ReadInt32();
            TotalHeadersSize = reader.ReadInt32();
            TotalDataSize = reader.ReadInt32();
            Version = reader.ReadInt32();
            Mode.FlagValueInt = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(NumSamples);
            writer.Write(TotalHeadersSize);
            writer.Write(TotalDataSize);
            writer.Write(Version);
            writer.Write(Mode.FlagValueInt);
        }

        public FSB3_Header Clone()
        {
            var result = new FSB3_Header()
            {
                NumSamples = NumSamples,
                TotalHeadersSize = TotalHeadersSize,
                TotalDataSize = TotalDataSize,
                Version = Version
            };
            result.Mode.FlagValueInt = Mode.FlagValueInt;
            return result;
        }
    }
}