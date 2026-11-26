using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaHudModel : DynaHud
    {
        private const string dynaCategoryName = "hud:model";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Model);

        protected override short constVersion => 1;

        [Category(dynaCategoryName),
            Description("It needs to be a hash of the model's name without the .dff or else it won't play the spinning animation for some reason"),
            ValidReferenceRequired]
        public AssetID Model { get; set; }

        public DynaHudModel(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.hud__model, reader)
        {
            Model = reader.ReadUInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaHud(writer);
            writer.Write(Model);
        }
    }
}