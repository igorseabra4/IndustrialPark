using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaLogicReference : AssetDYNA
    {
        private const string dynaCategoryName = "logic:reference";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Initial);

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Initial { get; set; }

        public DynaLogicReference(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.logic__reference, reader)
        {
            Initial = reader.ReadUInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Initial);
        }
    }
}