using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaCObjectPoleSwing : AssetDYNA
    {
        private const string dynaCategoryName = "Context Object:Pole Swing";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(PoleID);
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID PoleID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Length { get; set; }

        public DynaCObjectPoleSwing(string assetName) : base(assetName, DynaType.ContextObject_PoleSwing) { }
        public DynaCObjectPoleSwing(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.ContextObject_PoleSwing, reader)
        {
            PoleID = reader.ReadUInt32();
            Length = reader.ReadSingle();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(PoleID);
            writer.Write(Length);
        }
    }
}