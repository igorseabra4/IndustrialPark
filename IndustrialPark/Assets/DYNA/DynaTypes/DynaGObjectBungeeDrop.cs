using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBungeeDrop : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:bungee_drop";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID MRKR_ID { get; set; }
        [Category(dynaCategoryName)]
        public int SetViewAngle { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ViewAngle { get; set; }

        public DynaGObjectBungeeDrop(string assetName, uint mrkrAssetId) : base(assetName, DynaType.game_object__bungee_drop, 1)
        {
            MRKR_ID = mrkrAssetId;
            SetViewAngle = 1;
        }

        public DynaGObjectBungeeDrop(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__bungee_drop, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                MRKR_ID = reader.ReadUInt32();
                SetViewAngle = reader.ReadInt32();
                ViewAngle = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(MRKR_ID);
                writer.Write(SetViewAngle);
                writer.Write(ViewAngle);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => MRKR_ID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(MRKR_ID, ref result);
        }
    }
}