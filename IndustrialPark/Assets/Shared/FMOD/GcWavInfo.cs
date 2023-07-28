using Newtonsoft.Json;
using System.ComponentModel;

namespace IndustrialPark
{
    public class GcWavInfo : GenericAssetDataContainer
    {
        public FSB3_SampleHeaderBasic BasicSampleHeader;
        public FMOD_GcADPCMInfo[] GcAdpcmInfos { get; set; }

        [ReadOnly(true)]
        public byte[] Data { get; set; }

        public uint _assetID;
        [ValidReferenceRequired]
        public AssetID Sound { get => _assetID; set => _assetID = value; }
        public byte uFlags;
        public byte uAudioSampleIndex;
        public byte uFSBIndex;
        public byte uSoundInfoIndex;

        public bool Looped
        {
            get => (uFlags & 1) != 0;
            set
            {
                if (value)
                {
                    uFlags |= 1;
                }
                else
                {
                    uFlags = (byte)(uFlags & 254);
                }
            }
        }

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

        public void SetEntryData(uint assetID, byte uFlags, byte uAudioSampleIndex, byte uFSBIndex, byte uSoundInfoIndex)
        {
            _assetID = assetID;
            this.uFlags = uFlags;
            this.uAudioSampleIndex = uAudioSampleIndex;
            this.uFSBIndex = uFSBIndex;
            this.uSoundInfoIndex = uSoundInfoIndex;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Sound);
            writer.Write(uFlags);
            writer.Write(uAudioSampleIndex);
            writer.Write(uFSBIndex);
            writer.Write(uSoundInfoIndex);
        }

        public override string ToString()
        {
            return HexUIntTypeConverter.StringFromAssetID(Sound);
        }

        public GcWavInfo Clone()
        {
            var result = new GcWavInfo
            {
                BasicSampleHeader = BasicSampleHeader.Clone(),
                GcAdpcmInfos = new FMOD_GcADPCMInfo[GcAdpcmInfos.Length],
                Data = JsonConvert.DeserializeObject<byte[]>(JsonConvert.SerializeObject(Data)),
                _assetID = _assetID,
                uFlags = uFlags,
                uAudioSampleIndex = uAudioSampleIndex,
                uFSBIndex = uFSBIndex,
                uSoundInfoIndex = uSoundInfoIndex
            };

            for (int i = 0; i < result.GcAdpcmInfos.Length; i++)
                result.GcAdpcmInfos[i] = GcAdpcmInfos[i].Clone();

            return result;
        }
    }
}