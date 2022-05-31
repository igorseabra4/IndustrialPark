using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectUberLaser : AssetDYNA
    {
        private const string dynaCategoryName = "effect:uber_laser";

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

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Marker1);
                writer.Write(Marker2);
                writer.Write(UnknownFloat);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Marker1, ref result);
            Verify(Marker2, ref result);
            base.Verify(ref result);
        }
    }
}