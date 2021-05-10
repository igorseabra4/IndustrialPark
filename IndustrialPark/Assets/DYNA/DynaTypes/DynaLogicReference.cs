using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaLogicReference : AssetDYNA
    {
        private const string dynaCategoryName = "logic:reference";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Unknown { get; set; }

        public DynaLogicReference(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.logic__reference, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            Unknown = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(Unknown);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Unknown == assetID || base.HasReference(assetID);
    }
}