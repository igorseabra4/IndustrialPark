using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectSplash : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Splash";

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetSingle Unknown00 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown04 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown08 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown10 { get; set; }

        public DynaEffectSplash(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__Splash, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Unknown00 = reader.ReadSingle();
                Unknown04 = reader.ReadSingle();
                Unknown08 = reader.ReadSingle();
                Unknown0C = reader.ReadUInt32();
                Unknown10 = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Unknown00);
                writer.Write(Unknown04);
                writer.Write(Unknown08);
                writer.Write(Unknown0C);
                writer.Write(Unknown10);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Unknown0C, ref result);
            Verify(Unknown10, ref result);
            base.Verify(ref result);
        }
    }
}