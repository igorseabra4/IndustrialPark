using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectFlythrough : AssetDYNA
    {
        protected override short constVersion => 1;

        [Category("game_object:Flythrough")]
        public AssetID FLY_ID { get; set; }

        public DynaGObjectFlythrough(string assetName) : base(assetName, DynaType.game_object__Flythrough, 1)
        {
            FLY_ID = 0;
        }

        public DynaGObjectFlythrough(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Flythrough, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;
                FLY_ID = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(FLY_ID);
                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => FLY_ID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            if (FLY_ID == 0)
                result.Add("Flythrough with no FLY reference");
            Verify(FLY_ID, ref result);

            base.Verify(ref result);
        }
    }
}