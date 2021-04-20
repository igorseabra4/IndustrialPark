using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaLogicReference : AssetDYNA
    {
        private const string dynaCategoryName = "logic:reference";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Unknown { get; set; }

        public DynaLogicReference(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.logic__reference, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            Unknown = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Unknown);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Unknown == assetID || base.HasReference(assetID);
    }
}