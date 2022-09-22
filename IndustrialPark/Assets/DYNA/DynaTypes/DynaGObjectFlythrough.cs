using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectFlythrough : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Flythrough";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Flythrough);

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Flythrough { get; set; }

        public DynaGObjectFlythrough(string assetName) : base(assetName, DynaType.game_object__Flythrough)
        {
            Flythrough = 0;
        }

        public DynaGObjectFlythrough(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Flythrough, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;
                Flythrough = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Flythrough);
        }
    }
}