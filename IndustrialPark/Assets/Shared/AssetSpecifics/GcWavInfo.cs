using System.ComponentModel;
using System.IO;

namespace IndustrialPark
{
    public class GcWavInfo : GenericAssetDataContainer
    {
        public override void Serialize(EndianBinaryWriter writer) { }

        public int templengthcompressedbytes;

        public int lengthsamples { get; set; }
        public int lengthcompressedbytes => Data.Length;

        public short[] coef { get; set; }

        public ushort gain { get; set; }
        public ushort pred_scale { get; set; }
        public ushort yn1 { get; set; }
        public ushort yn2 { get; set; }
        public ushort loop_pred_scale { get; set; }
        public short loop_yn1 { get; set; }
        public short loop_yn2 { get; set; }

        public byte[] Data { get; set; }

        public GcWavInfo()
        {
            Data = new byte[0];
        }

        public void SetEntryPartOne(BinaryReader binaryReader)
        {
            lengthsamples = binaryReader.ReadInt32();
            templengthcompressedbytes = binaryReader.ReadInt32();

            coef = new short[16];
            for (int i = 0; i < coef.Length; i++)
                coef[i] = binaryReader.ReadInt16();

            gain = binaryReader.ReadUInt16();
            pred_scale = binaryReader.ReadUInt16();
            yn1 = binaryReader.ReadUInt16();
            yn2 = binaryReader.ReadUInt16();
            loop_pred_scale = binaryReader.ReadUInt16();
            loop_yn1 = binaryReader.ReadInt16();
            loop_yn2 = binaryReader.ReadInt16();
        }

        public void PartOneToByteArray(EndianBinaryWriter writer, int index)
        {
            if (index == 0)
            {
                writer.Write(0);
                writer.Write(0);
            }
            else
            {
                writer.Write(lengthsamples);
                writer.Write(lengthcompressedbytes);
            }

            for (int i = 0; i < 16; i++)
                if (i < coef.Length)
                    writer.Write(coef[i]);
                else
                    writer.Write(new byte[2]);

            writer.Write(gain);
            writer.Write(pred_scale);
            writer.Write(yn1);
            writer.Write(yn2);
            writer.Write(loop_pred_scale);
            writer.Write(loop_yn1);
            writer.Write(loop_yn2);
        }

        public uint _assetID;
        [ValidReferenceRequired]
        public AssetID Sound { get => _assetID; set => _assetID = value; }
        public byte uFlags;
        public byte uAudioSampleIndex;
        public byte uFSBIndex;
        public byte uSoundInfoIndex;

        [Category("Flags")]
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

        [Category("Flags")]
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

        public void SetEntryPartTwo(uint assetID, byte uFlags, byte uAudioSampleIndex, byte uFSBIndex, byte uSoundInfoIndex)
        {
            _assetID = assetID;
            this.uFlags = uFlags;
            this.uAudioSampleIndex = uAudioSampleIndex;
            this.uFSBIndex = uFSBIndex;
            this.uSoundInfoIndex = uSoundInfoIndex;
        }

        public void PartTwoToByteArray(EndianBinaryWriter writer)
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
    }
}