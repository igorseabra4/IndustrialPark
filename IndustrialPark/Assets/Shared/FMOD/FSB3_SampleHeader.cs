using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class FSB3_SampleHeader : GenericAssetDataContainer
    {
        public string SampleName { get; set; }
        [ReadOnly(true)]
        public int LengthSamples { get; set; }
        [ReadOnly(true)]
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
        public FMOD_GcADPCMInfo[] GCADPCM { get; set; }

        private uint _assetid;
        [ValidReferenceRequired]
        public AssetID Sound { get => _assetid; set => _assetid = value; }
        public byte uFlags;
        public byte uAudioSampleIndex;
        public byte uFSBIndex;
        public byte uSoundInfoIndex;
        public byte[] Data { get; set; }
        public bool Looped
        {
            get => (uFlags & 1) != 0;
            set
            {
                if (value)
                {
                    uFlags |= 1;
                    SampleHeaderMode.FlagValueInt &= 0xFFFFFFFE;
                }
                else
                {
                    uFlags &= 254;
                    SampleHeaderMode.FlagValueInt |= 1;
                }
            }
        }

        [Browsable(false)]
        public bool StreamedSound
        {
            get => (uFlags & 2) != 0;
            set
            {
                if (value)
                {
                    uFlags |= 2;
                }
                else
                {
                    uFlags = (byte)(uFlags & 253);
                }
            }
        }

        public FSB3_SampleHeader()
        {
            SampleName = "empty";
            SampleHeaderMode.FlagValueInt = 33558561;
            Frequency = 32000;
            Volume = 255;
            Pan = 128;
            Priority = 255;
            NumChannels = 1;
            MinDistance = 1f;
            MaxDistance = 1000000f;
            GCADPCM = new FMOD_GcADPCMInfo[0];
        }

        public FSB3_SampleHeader(BinaryReader reader)
        {
            reader.ReadUInt16();
            SampleName = new string(reader.ReadChars(30));
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

            if ((SampleHeaderMode.FlagValueInt & 0x02000000) != 0)
            {
                GCADPCM = new FMOD_GcADPCMInfo[NumChannels];
                for (int i = 0; i < NumChannels; i++)
                    GCADPCM[i] = new FMOD_GcADPCMInfo(reader);
            }
        }

        public void SetEntryData(uint assetID, byte uFlags, byte uAudioSampleIndex, byte uFSBIndex, byte uSoundInfoIndex)
        {
            _assetid = assetID;
            this.uFlags = uFlags;
            this.uAudioSampleIndex = uAudioSampleIndex;
            this.uFSBIndex = uFSBIndex;
            this.uSoundInfoIndex = uSoundInfoIndex;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((ushort)(0x50 + (GCADPCM is null ? 0 : NumChannels * 0x2E)));
            writer.WritePaddedString(SampleName, 30);
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

            GCADPCM?.ToList().ForEach(a => a.Serialize(writer));

        }

        public void SerializeWavInfo(EndianBinaryWriter writer)
        {
            writer.Write(Sound);
            writer.Write(uFlags);
            writer.Write(uAudioSampleIndex);
            writer.Write(uFSBIndex);
            writer.Write(uSoundInfoIndex);
        }

        public FSB3_SampleHeader Clone()
        {
            var result = new FSB3_SampleHeader
            {
                SampleName = SampleName,
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
                VariablePan = VariablePan,
                GCADPCM = GCADPCM,
                uFlags = uFlags,
                uAudioSampleIndex = uAudioSampleIndex,
                uFSBIndex = uFSBIndex,
                uSoundInfoIndex = uSoundInfoIndex,
                _assetid = _assetid,
                Data = Data,
            };
            result.SampleHeaderMode.FlagValueInt = SampleHeaderMode.FlagValueInt;

            return result;
        }

        public override string ToString()
        {
            return HexUIntTypeConverter.StringFromAssetID(Sound);
        }
    }
}