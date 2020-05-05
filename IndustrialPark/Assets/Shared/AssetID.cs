using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(AssetIDTypeConverter))]
    public class AssetID
    {
        public AssetID()
        {
            value = 0;
        }

        public AssetID(uint value)
        {
            this.value = value;
        }

        public AssetID(string value)
        {
            this.value = HipHopFile.Functions.BKDRHash(value);
        }

        private uint value;

        public override int GetHashCode()
        {
            return (int)value;
        }

        public override bool Equals(object obj)
        {
            if (obj is AssetID assetID)
                return assetID.value == value;
            if (obj is uint uinteger)
                return uinteger == value;
            if (obj is string str)
                return HipHopFile.Functions.BKDRHash(str) == value;
            return false;
        }

        public override string ToString()
        {
            return value.ToString("X8");
        }

        public string ToString(string v)
        {
            return value.ToString(v);
        }

        public static implicit operator uint(AssetID value)
        {
            return value.value;
        }

        public static implicit operator AssetID(uint value)
        {
            return new AssetID(value);
        }

        public static implicit operator AssetID(string value)
        {
            return HipHopFile.Functions.BKDRHash(value);
        }
    }
}