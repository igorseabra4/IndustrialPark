using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudMeterUnit : DynaHudMeter
    {
        private const string dynaCategoryName = "hud:meter:unit";

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetID EmptyModel_AssetID { get; set; }
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
        public AssetID FullModel_AssetID { get; set; }
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

                EmptyModel_AssetID = reader.ReadUInt32();
                EmptyOffset_X = reader.ReadSingle();
                EmptyOffset_Y = reader.ReadSingle();
                EmptyOffset_Z = reader.ReadSingle();
                EmptyScale_X = reader.ReadSingle();
                EmptyScale_Y = reader.ReadSingle();
                EmptyScale_Z = reader.ReadSingle();
                FullModel_AssetID = reader.ReadUInt32();
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

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaHudMeter(endianness));
                writer.Write(EmptyModel_AssetID);
                writer.Write(EmptyOffset_X);
                writer.Write(EmptyOffset_Y);
                writer.Write(EmptyOffset_Z);
                writer.Write(EmptyScale_X);
                writer.Write(EmptyScale_Y);
                writer.Write(EmptyScale_Z);
                writer.Write(FullModel_AssetID);
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

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (EmptyModel_AssetID == assetID)
                return true;
            if (FullModel_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(EmptyModel_AssetID, ref result);
            Verify(FullModel_AssetID, ref result);
            base.Verify(ref result);
        }
    }
}