using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryPIPT
    {
        public AssetID ModelAssetID { get; set; }
        public int MaybeMeshIndex { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte RelatedToVisibility { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Culling { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte DestinationSourceBlend { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown34 { get; set; }

        public override string ToString()
        {
            return $"[{ModelAssetID.ToString()}] - {MaybeMeshIndex}";
        }
    }
}