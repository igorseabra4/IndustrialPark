using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaUITextUserString : DynaUIText
    {
        private const string dynaCategoryName = "ui:text:user string";
        public override string TypeString => dynaCategoryName;

        [Category(dynaCategoryName)]
        public byte hardMaxChars { get; set; }
        [Category(dynaCategoryName)]
        public byte softMaxChars { get; set; }

        public DynaUITextUserString(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__text__userstring, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaUITextEnd;

                hardMaxChars = reader.ReadByte();
                softMaxChars = reader.ReadByte();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            base.SerializeDyna(writer);
            writer.Write(hardMaxChars);
            writer.Write(softMaxChars);
            writer.Write((short)0);
        }
    }
}