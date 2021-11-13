using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(AssetSingleTypeConverter))]
    public struct AssetSingle
    {
        public AssetSingle(float value)
        {
            this.value = value;
        }

        private float value;

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();

        public static implicit operator float(AssetSingle value) => value.value;

        public static implicit operator AssetSingle(float value) => new AssetSingle(value);

        public override bool Equals(object obj)
        {
            if (obj is AssetSingle asi)
                return asi.value == value;
            if (obj is float fv)
                return fv == value;
            if (obj is uint uinteger)
                return uinteger == value;
            if (obj is int integer)
                return integer == value;
            if (obj is ushort usinteger)
                return usinteger == value;
            if (obj is short sinteger)
                return sinteger == value;
            if (obj is byte bv)
                return bv == value;
            return false;
        }
    }
}