using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudMeterUnit : DynaHudMeter
    {
        private const string dynaCategoryName = "hud:meter:unit";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetID EmptyModel { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EmptyOffset_X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EmptyOffset_Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EmptyOffset_Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EmptyScale_X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EmptyScale_Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle EmptyScale_Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FullModel { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FullOffset_X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FullOffset_Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FullOffset_Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FullScale_X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FullScale_Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FullScale_Z { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Spacing_X { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Spacing_Y { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Spacing_Z { get; set; }
        [Category(dynaCategoryName)]
        public int MeterFillDirection { get; set; }

        public DynaHudMeterUnit(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__meter__unit, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaHudMeterEnd;

                EmptyModel = reader.ReadUInt32();
                EmptyOffset_X = reader.ReadSingle();
                EmptyOffset_Y = reader.ReadSingle();
                EmptyOffset_Z = reader.ReadSingle();
                EmptyScale_X = reader.ReadSingle();
                EmptyScale_Y = reader.ReadSingle();
                EmptyScale_Z = reader.ReadSingle();
                FullModel = reader.ReadUInt32();
                FullOffset_X = reader.ReadSingle();
                FullOffset_Y = reader.ReadSingle();
                FullOffset_Z = reader.ReadSingle();
                FullScale_X = reader.ReadSingle();
                FullScale_Y = reader.ReadSingle();
                FullScale_Z = reader.ReadSingle();
                Spacing_X = reader.ReadSingle();
                Spacing_Y = reader.ReadSingle();
                Spacing_Z = reader.ReadSingle();
                MeterFillDirection = reader.ReadInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaHudMeter(writer);
            writer.Write(EmptyModel);
            writer.Write(EmptyOffset_X);
            writer.Write(EmptyOffset_Y);
            writer.Write(EmptyOffset_Z);
            writer.Write(EmptyScale_X);
            writer.Write(EmptyScale_Y);
            writer.Write(EmptyScale_Z);
            writer.Write(FullModel);
            writer.Write(FullOffset_X);
            writer.Write(FullOffset_Y);
            writer.Write(FullOffset_Z);
            writer.Write(FullScale_X);
            writer.Write(FullScale_Y);
            writer.Write(FullScale_Z);
            writer.Write(Spacing_X);
            writer.Write(Spacing_Y);
            writer.Write(Spacing_Z);
            writer.Write(MeterFillDirection);
        }
    }
}