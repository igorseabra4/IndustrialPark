using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaLogicReference : AssetDYNA
    {
        private const string dynaCategoryName = "logic:reference";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Initial { get; set; }

        public DynaLogicReference(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.logic__reference, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Initial = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Initial);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => Initial == assetID || base.HasReference(assetID);
    }
}