using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaCObjectTightrope : AssetDYNA
    {
        private const string dynaCategoryName = "Context Object:Tightrope";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(CurveID);

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetID CurveID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID ObjectID { get; set; }
        [Category(dynaCategoryName)]
        public bool FallBelow { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = ByteFlagsDescriptor();

        public DynaCObjectTightrope(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ContextObject_Tightrope, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                CurveID = reader.ReadUInt32();
                ObjectID = reader.ReadUInt32();
                FallBelow = reader.ReadByteBool();
                Flags.FlagValueByte = reader.ReadByte();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(CurveID);
            writer.Write(ObjectID);
            writer.Write(FallBelow);
            writer.Write(Flags.FlagValueByte);
            writer.Write((short)0);
        }
    }
}