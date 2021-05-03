using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaUITextUserString : DynaUIText
    {
        private const string dynaCategoryName = "ui:text:user string";

        [Category(dynaCategoryName)]
        public byte hardMaxChars { get; set; }
        [Category(dynaCategoryName)]
        public byte softMaxChars { get; set; }

        public DynaUITextUserString(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.ui__text__userstring, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaUITextEnd;

            hardMaxChars = reader.ReadByte();
            softMaxChars = reader.ReadByte();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(base.SerializeDyna(game, platform));

            writer.Write(hardMaxChars);
            writer.Write(softMaxChars);
            writer.Write((short)0);

            return writer.ToArray();
        }
    }
}