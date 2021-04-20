using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBungeeDrop : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:bungee_drop";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID MRKR_ID { get; set; }
        [Category(dynaCategoryName)]
        public int SetViewAngle { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ViewAngle { get; set; }

        public DynaGObjectBungeeDrop(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__bungee_drop, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            MRKR_ID = reader.ReadUInt32();
            SetViewAngle = reader.ReadInt32();
            ViewAngle = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(MRKR_ID);
            writer.Write(SetViewAngle);
            writer.Write(ViewAngle);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => MRKR_ID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(MRKR_ID, ref result);
        }
    }
}