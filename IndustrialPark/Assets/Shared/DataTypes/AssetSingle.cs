using System.ComponentModel;
using Newtonsoft.Json;

namespace IndustrialPark
{
    [TypeConverter(typeof(AssetSingleTypeConverter))]
    public struct AssetSingle
    {
        public AssetSingle()
        {
            this.value = 0;
        }

        public AssetSingle(float value)
        {
            this.value = value;
        }

        [JsonProperty]
        private float value;

        [JsonIgnore]
        public AssetSingle Value
        {
            get => value;
            set => this.value = value;
        }

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => string.Format("{0:0.000000}", value);

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