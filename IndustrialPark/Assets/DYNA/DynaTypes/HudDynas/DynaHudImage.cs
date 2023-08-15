using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudImage : DynaHud
    {
        private const string dynaCategoryName = "hud:image";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Texture);

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Texture { get; set; }

        public DynaHudImage(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__image, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaHudEnd;
                Texture = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaHud(writer);
            writer.Write(Texture);
        }
    }
}