using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(HexUIntTypeConverter))]
    public struct AssetID
    {
        [JsonConstructor]
        public AssetID(uint value = 0)
        {
            this.value = value;
        }

        [JsonRequired]
        private uint value;

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString("X8");

        public static implicit operator uint(AssetID value) => value.value;

        public static implicit operator AssetID(uint value) => new AssetID(value);

        public override bool Equals(object obj)
        {
            if (obj is AssetID assetID)
                return assetID.value == value;
            if (obj is uint uinteger)
                return uinteger == value;
            if (obj is int integer)
                return integer == value;
            if (obj is string str)
                return HipHopFile.Functions.BKDRHash(str) == value;
            if (obj is ushort usinteger)
                return usinteger == value;
            if (obj is short sinteger)
                return sinteger == value;
            if (obj is byte bv)
                return bv == value;
            return false;
        }

        public AssetID(string value)
        {
            this.value = HipHopFile.Functions.BKDRHash(value);
        }

        public string ToString(string v) => value.ToString(v);

        public static implicit operator AssetID(string value) => HipHopFile.Functions.BKDRHash(value);
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ValidReferenceRequiredAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class IgnoreVerificationAttribute : Attribute { }
}