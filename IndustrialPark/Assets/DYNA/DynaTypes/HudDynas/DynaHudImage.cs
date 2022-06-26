using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudImage : DynaHud
    {
        private const string dynaCategoryName = "hud:image";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Texture);

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Texture { get; set; }

        public DynaHudImage(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__image, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaHudEnd;
                Texture = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaHud(endianness));
                writer.Write(Texture);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Texture, ref result);
            base.Verify(ref result);
        }
    }
}