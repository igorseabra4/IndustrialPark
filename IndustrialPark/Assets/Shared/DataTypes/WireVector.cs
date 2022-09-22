using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WireVector
    {
        public AssetSingle X { get; set; }
        public AssetSingle Y { get; set; }
        public AssetSingle Z { get; set; }

        public WireVector()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

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

        public void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        public static implicit operator WireVector(Vector3 vector)
        {
            return new WireVector(vector.X, vector.Y, vector.Z);
        }

        public static implicit operator Vector3(WireVector vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static WireVector operator -(WireVector vector)
        {
            return new WireVector(-vector.X, -vector.Y, -vector.Z);
        }

        public bool NearEqual(WireVector other)
        {
            return MathUtil.NearEqual(other.X, X) && MathUtil.NearEqual(other.Y, Y) && MathUtil.NearEqual(other.Z, Z);
        }
    }
}