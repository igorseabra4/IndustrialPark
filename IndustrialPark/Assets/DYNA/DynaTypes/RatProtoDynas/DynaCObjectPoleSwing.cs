using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaCObjectPoleSwing : AssetDYNA
    {
        private const string dynaCategoryName = "Context Object:Pole Swing";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID PoleID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Length { get; set; }

        public DynaCObjectPoleSwing(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ContextObject_PoleSwing, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                PoleID = reader.ReadUInt32();
                Length = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(PoleID);
            writer.Write(Length);
        }
    }
}