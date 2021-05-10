using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudModel : DynaHud
    {
        protected override short constVersion => 1;

        [Category("hud:model"),
            Description("It needs to be a hash of the model's name without the .dff or else it won't play the spinning animation for some reason")]
        public AssetID Model_AssetID { get; set; }

        public DynaHudModel(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__model, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaHudEnd;

            Model_AssetID = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(SerializeDynaHud(endianness));
            writer.Write(Model_AssetID);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Model_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Verify(Model_AssetID, ref result);
        }
    }
}