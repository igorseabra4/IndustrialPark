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

        public DynaUITextUserString(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__text__userstring, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaUITextEnd;

            hardMaxChars = reader.ReadByte();
            softMaxChars = reader.ReadByte();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(base.SerializeDyna(game, endianness));

            writer.Write(hardMaxChars);
            writer.Write(softMaxChars);
            writer.Write((short)0);

            return writer.ToArray();
        }
    }
}