using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaLogicReference : AssetDYNA
    {
        private const string dynaCategoryName = "logic:reference";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Initial);

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Initial { get; set; }

        public DynaLogicReference(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.logic__reference, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;
                Initial = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(Initial);
                
        }
    }
}