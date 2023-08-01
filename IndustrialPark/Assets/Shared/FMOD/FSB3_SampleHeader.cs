using System.ComponentModel;
using System.IO;

namespace IndustrialPark
{
    public class FSB3_SampleHeader : GenericAssetDataContainer
    {
        public string Name { get; set; }
        [Browsable(false)]
        public int LengthSamples { get; set; }
        [Browsable(false)]
        public int LengthCompressedBytes { get; set; }
        public uint LoopStart { get; set; }
        public uint LoopEnd { get; set; }

        [ReadOnly(true)]
        public FlagBitmask SampleHeaderMode { get; set; } = IntFlagsDescriptor(
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

        public int Frequency { get; set; }
        public ushort Volume { get; set; }
        public short Pan { get; set; }
        public ushort Priority { get; set; }
        [ReadOnly(true)]
        public ushort NumChannels { get; set; }
        public AssetSingle MinDistance { get; set; }
        public AssetSingle MaxDistance { get; set; }
        public int VariableFrequency { get; set; }
        public ushort VariableVolume { get; set; }
        public short VariablePan { get; set; }

        public FSB3_SampleHeader()
        {
            Name = "empty";
            SampleHeaderMode.FlagValueInt = 33558561;
            Frequency = 32000;
            Volume = 255;
            Pan = 128;
            Priority = 255;
            NumChannels = 1;
            MinDistance = 1f;
            MaxDistance = 1000000f;
        }

        public FSB3_SampleHeader(BinaryReader reader)
        {
            reader.ReadUInt16();
            Name = new string(reader.ReadChars(30));
            LengthSamples = reader.ReadInt32();
            LengthCompressedBytes = reader.ReadInt32();
            LoopStart = reader.ReadUInt32();
            LoopEnd = reader.ReadUInt32();
            SampleHeaderMode.FlagValueInt = reader.ReadUInt32();
            Frequency = reader.ReadInt32();
            Volume = reader.ReadUInt16();
            Pan = reader.ReadInt16();
            Priority = reader.ReadUInt16();
            NumChannels = reader.ReadUInt16();
            MinDistance = reader.ReadSingle();
            MaxDistance = reader.ReadSingle();
            VariableFrequency = reader.ReadInt32();
            VariableVolume = reader.ReadUInt16();
            VariablePan = reader.ReadInt16();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((ushort)(0x50 + NumChannels * 0x2E));
            writer.WritePaddedString(Name, 30);
            writer.Write(LengthSamples);
            writer.Write(LengthCompressedBytes);
            writer.Write(LoopStart);
            writer.Write(LoopEnd);
            writer.Write(SampleHeaderMode.FlagValueInt);
            writer.Write(Frequency);
            writer.Write(Volume);
            writer.Write(Pan);
            writer.Write(Priority);
            writer.Write(NumChannels);
            writer.Write(MinDistance);
            writer.Write(MaxDistance);
            writer.Write(VariableFrequency);
            writer.Write(VariableVolume);
            writer.Write(VariablePan);
        }

        public FSB3_SampleHeader Clone()
        {
            var result = new FSB3_SampleHeader
            {
                Name = Name,
                LengthSamples = LengthSamples,
                LengthCompressedBytes = LengthCompressedBytes,
                LoopStart = LoopStart,
                LoopEnd = LoopEnd,
                Frequency = Frequency,
                Volume = Volume,
                Pan = Pan,
                Priority = Priority,
                NumChannels = NumChannels,
                MinDistance = MinDistance,
                MaxDistance = MaxDistance,
                VariableFrequency = VariableFrequency,
                VariableVolume = VariableVolume,
                VariablePan = VariablePan
            };
            result.SampleHeaderMode.FlagValueInt = SampleHeaderMode.FlagValueInt;

            return result;
        }
    }
}