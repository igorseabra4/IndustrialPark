using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectFlythrough : AssetDYNA
    {
        protected override int constVersion => 1;

        [Category("game_object:Flythrough")]
        public AssetID FLY_ID { get; set; }

        public DynaGObjectFlythrough(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__Flythrough, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;
            FLY_ID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(FLY_ID);
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => FLY_ID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            if (FLY_ID == 0)
                result.Add("Flythrough with no FLY reference");
            Verify(FLY_ID, ref result);
        }
    }
}