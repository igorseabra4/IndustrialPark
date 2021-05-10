using HipHopFile;

namespace IndustrialPark
{
    public class DynaUIController : DynaUI
    {
        protected override short constVersion => 0;

        public DynaUIController(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__controller, game, endianness)
        {
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeDynaUI(endianness));
            return writer.ToArray();
        }
    }
}