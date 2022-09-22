using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaUIModel : DynaUI
    {
        private const string dynaCategoryName = "ui:model";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Model);

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetID Model { get; set; }
        [Category(dynaCategoryName)]
        public AssetID AnimationList { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Surface { get; set; }

        public DynaUIModel(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__model, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaUIEnd;

                Model = reader.ReadUInt32();
                AnimationList = reader.ReadUInt32();
                Surface = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaUI(writer);
            writer.Write(Model);
            writer.Write(AnimationList);
            writer.Write(Surface);
        }
    }
}