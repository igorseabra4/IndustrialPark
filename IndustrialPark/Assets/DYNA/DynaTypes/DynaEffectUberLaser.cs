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
        public AssetID Marker1_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Marker2_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat { get; set; }

        public DynaEffectUberLaser(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__uber_laser, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Marker1_AssetID = reader.ReadUInt32();
                Marker2_AssetID = reader.ReadUInt32();
                UnknownFloat = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Marker1_AssetID);
                writer.Write(Marker2_AssetID);
                writer.Write(UnknownFloat);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Marker1_AssetID, ref result);
            Verify(Marker2_AssetID, ref result);
            base.Verify(ref result);
        }

        public override bool HasReference(uint assetID) => Marker1_AssetID == assetID || Marker2_AssetID == assetID || base.HasReference(assetID);
    }
}