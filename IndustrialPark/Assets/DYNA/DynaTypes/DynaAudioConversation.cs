using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaAudioConversation : AssetDYNA
    {
        private const string dynaCategoryName = "audio:conversation";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID SoundGroup { get; set; }

        public DynaAudioConversation(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.audio__conversation, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                SoundGroup = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(SoundGroup);
        }
    }
}