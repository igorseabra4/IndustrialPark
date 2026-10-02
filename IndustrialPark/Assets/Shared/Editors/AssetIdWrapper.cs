using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetIdWrapper : GenericAssetDataContainer
    {
        private const string categoryName = "Asset ID";

        private uint value;

        [Category(categoryName)]
        public AssetID Value
        {
            get => value;
            set => this.value = value;
        }

        [Category(categoryName), DisplayName("Value (Hex)")]
        public string ValueHex
        {
            get => "0x" + value.ToString("X8");
            set => this.value = uint.Parse(value, System.Globalization.NumberStyles.HexNumber);
        }

        public AssetIdWrapper()
        {
            value = 0;
        }

        public AssetIdWrapper(uint id)
        {
            value = id;
        }

        public AssetIdWrapper(EndianBinaryReader reader)
        {
            value = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(value);
        }

        public override string ToString()
        {
            return HexUIntTypeConverter.StringFromAssetID(value);
        }
    }
}