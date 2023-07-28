using System.IO;

namespace IndustrialPark
{
    public class FSB3_SampleHeaderBasic : GenericAssetDataContainer
    {
        public int LengthSamples { get; set; }
        public int LengthCompressedBytes { get; set; }

        public FSB3_SampleHeaderBasic()
        {
        }

        public FSB3_SampleHeaderBasic(BinaryReader reader)
        {
            LengthSamples = reader.ReadInt32();
            LengthCompressedBytes = reader.ReadInt32();
        }

        public FSB3_SampleHeaderBasic(int lengthSamples, int lengthCompressedBytes)
        {
            LengthSamples = lengthSamples;
            LengthCompressedBytes = lengthCompressedBytes;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(LengthSamples);
            writer.Write(LengthCompressedBytes);
        }

        public FSB3_SampleHeaderBasic Clone()
        {
            return new FSB3_SampleHeaderBasic(LengthSamples, LengthCompressedBytes);
        }
    }
}