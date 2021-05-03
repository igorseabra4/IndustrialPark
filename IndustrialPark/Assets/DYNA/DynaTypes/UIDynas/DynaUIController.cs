using HipHopFile;

namespace IndustrialPark
{
    public class DynaUIController : DynaUI
    {
        protected override int constVersion => 0;

        public DynaUIController(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.ui__controller, game, platform)
        {
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeDynaUI(platform));
            return writer.ToArray();
        }
    }
}