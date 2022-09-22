using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectUberLaser : AssetDYNA
    {
        private const string dynaCategoryName = "effect:uber_laser";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Marker1);

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Marker1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Marker2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat { get; set; }

        public DynaEffectUberLaser(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__uber_laser, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Marker1 = reader.ReadUInt32();
                Marker2 = reader.ReadUInt32();
                UnknownFloat = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(Marker1);
                writer.Write(Marker2);
                writer.Write(UnknownFloat);

                
        }
    }
}