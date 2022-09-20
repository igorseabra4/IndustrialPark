using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectLensFlare : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Lens Flare Element";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Texture);

        protected override short constVersion => 3;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Texture { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown04 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown08 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown14 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown18 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown1C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown20 { get; set; }
        [Category(dynaCategoryName)]
        public int Unknown24 { get; set; }
        [Category(dynaCategoryName)]
        public int Unknown28 { get; set; }

        public DynaEffectLensFlare(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__LensFlareElement, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Texture = reader.ReadUInt32();
                Unknown04 = reader.ReadUInt32();
                Unknown08 = reader.ReadSingle();
                Unknown0C = reader.ReadSingle();
                Unknown10 = reader.ReadSingle();
                Unknown14 = reader.ReadSingle();
                Unknown18 = reader.ReadSingle();
                Unknown1C = reader.ReadSingle();
                Unknown20 = reader.ReadSingle();
                Unknown24 = reader.ReadInt32();
                Unknown28 = reader.ReadInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Texture);
                writer.Write(Unknown04);
                writer.Write(Unknown08);
                writer.Write(Unknown0C);
                writer.Write(Unknown10);
                writer.Write(Unknown14);
                writer.Write(Unknown18);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);

                return writer.ToArray();
            }
        }
    }
}