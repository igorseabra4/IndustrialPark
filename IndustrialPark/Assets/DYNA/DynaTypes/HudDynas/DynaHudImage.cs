using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudImage : DynaHud
    {
        protected override short constVersion => 1;

        [Category("hud:image")]
        public AssetID Texture_AssetID { get; set; }

        public DynaHudImage(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__image, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaHudEnd;
                Texture_AssetID = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaHud(endianness));
                writer.Write(Texture_AssetID);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => Texture_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(Texture_AssetID, ref result);
            base.Verify(ref result);
        }
    }
}