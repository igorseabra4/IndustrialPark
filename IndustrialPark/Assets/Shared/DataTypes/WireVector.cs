using HipHopFile;

namespace IndustrialPark
{
    public struct WireVector
    {
        public AssetSingle X { get; set; }
        public AssetSingle Y { get; set; }
        public AssetSingle Z { get; set; }

        public WireVector(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public WireVector(EndianBinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public byte[] Serialize(Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            return writer.ToArray();
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }
    }
}