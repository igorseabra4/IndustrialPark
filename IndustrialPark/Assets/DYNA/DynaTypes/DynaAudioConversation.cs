using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaAudioConversation : AssetDYNA
    {
        private const string dynaCategoryName = "audio:conversation";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(SoundGroup);

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID SoundGroup { get; set; }

        public DynaAudioConversation(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.audio__conversation, reader)
        {
            SoundGroup = reader.ReadUInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(SoundGroup);
        }
    }
}