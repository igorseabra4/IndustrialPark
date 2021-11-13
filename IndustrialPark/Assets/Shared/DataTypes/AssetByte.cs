using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(HexByteTypeConverter))]
    public struct AssetByte
    {
        public AssetByte(byte value)
        {
            this.value = value;
        }

        private byte value;

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString("X2");

        public static implicit operator byte(AssetByte value) => value.value;

        public static implicit operator AssetByte(byte value) => new AssetByte(value);

        public override bool Equals(object obj)
        {
            if (obj is AssetByte ab)
                return ab.value == value;
            if (obj is byte bv)
                return bv == value;
            if (obj is uint uinteger)
                return uinteger == value;
            if (obj is int integer)
                return integer == value;
            if (obj is ushort usinteger)
                return usinteger == value;
            if (obj is short sinteger)
                return sinteger == value;
            if (obj is float fv)
                return fv == value;
            return false;
        }
    }
}