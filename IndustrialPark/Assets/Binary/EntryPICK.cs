using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryPICK
    {
        public AssetID ReferenceID { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown21 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown22 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown23 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown24 { get; set; }
        public uint Unknown3 { get; set; }
        public AssetID ModelAssetID { get; set; }
        public uint Unknown5 { get; set; }

        public override string ToString()
        {
            return $"[{ReferenceID.ToString()}] - [{ModelAssetID.ToString()}]";
        }
    }
}