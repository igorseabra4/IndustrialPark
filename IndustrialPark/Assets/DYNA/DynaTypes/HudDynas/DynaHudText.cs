using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudText : DynaHud
    {
        protected override int constVersion => 1;

        private const string dynaCategoryName = "hud:text";

        [Category(dynaCategoryName)]
        public AssetID TextBoxID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TextID { get; set; }

        public DynaHudText(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.hud__text, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaHudEnd;

            TextBoxID = reader.ReadUInt32();
            TextID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(SerializeDynaHud(platform));
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