using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTaxi : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Taxi";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID MRKR_ID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID CAM_ID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PORT_ID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID DYNA_Talkbox_ID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TEXT_ID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SIMP_ID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InvisibleTimer { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TeleportTimer { get; set; }

        public DynaGObjectTaxi(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__Taxi, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            MRKR_ID = reader.ReadUInt32();
            CAM_ID = reader.ReadUInt32();
            PORT_ID = reader.ReadUInt32();
            DYNA_Talkbox_ID = reader.ReadUInt32();
            TEXT_ID = reader.ReadUInt32();
            SIMP_ID = reader.ReadUInt32();
            InvisibleTimer = reader.ReadSingle();
            TeleportTimer = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(MRKR_ID);
            writer.Write(CAM_ID);
            writer.Write(PORT_ID);
            writer.Write(DYNA_Talkbox_ID);
            writer.Write(TEXT_ID);
            writer.Write(SIMP_ID);
            writer.Write(InvisibleTimer);
            writer.Write(TeleportTimer);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (CAM_ID == assetID)
                return true;
            if (PORT_ID == assetID)
                return true;
            if (DYNA_Talkbox_ID == assetID)
                return true;
            if (TEXT_ID == assetID)
                return true;
            if (SIMP_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(MRKR_ID, ref result);
            Verify(CAM_ID, ref result);
            Verify(PORT_ID, ref result);
            Verify(DYNA_Talkbox_ID, ref result);
            Verify(TEXT_ID, ref result);
            Verify(SIMP_ID, ref result);
        }
    }
}