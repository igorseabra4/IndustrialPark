using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudText : DynaHud
    {
        protected override short constVersion => 1;

        private const string dynaCategoryName = "hud:text";

        [Category(dynaCategoryName)]
        public AssetID TextBoxID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TextID { get; set; }

        public DynaHudText(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__text, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaHudEnd;

            TextBoxID = reader.ReadUInt32();
            TextID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(SerializeDynaHud(endianness));
            writer.Write(TextBoxID);
            writer.Write(TextID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (TextBoxID == assetID || TextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(TextBoxID, ref result);
            Verify(TextID, ref result);
        }
    }
}