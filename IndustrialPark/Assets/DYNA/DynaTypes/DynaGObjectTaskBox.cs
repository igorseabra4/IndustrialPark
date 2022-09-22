using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTaskBox : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:task_box";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(TalkBox);

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetByte Persistent { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Loop { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Enable { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Retry { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID TalkBox { get; set; }
        [Category(dynaCategoryName)]
        public AssetID NextTaskBox { get; set; }
        [Category(dynaCategoryName)]
        public AssetID BeginText { get; set; }
        [Category(dynaCategoryName)]
        public AssetID DescriptionText { get; set; }
        [Category(dynaCategoryName)]
        public AssetID ReminderText { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SuccessText { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FailureText { get; set; }
        [Category(dynaCategoryName)]
        public AssetID End_TextID { get; set; }

        public DynaGObjectTaskBox(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__task_box, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Persistent = reader.ReadByte();
                Loop = reader.ReadByte();
                Enable = reader.ReadByte();
                Retry = reader.ReadByte();
                TalkBox = reader.ReadUInt32();
                NextTaskBox = reader.ReadUInt32();
                BeginText = reader.ReadUInt32();
                DescriptionText = reader.ReadUInt32();
                ReminderText = reader.ReadUInt32();
                SuccessText = reader.ReadUInt32();
                FailureText = reader.ReadUInt32();
                End_TextID = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(Persistent);
                writer.Write(Loop);
                writer.Write(Enable);
                writer.Write(Retry);
                writer.Write(TalkBox);
                writer.Write(NextTaskBox);
                writer.Write(BeginText);
                writer.Write(DescriptionText);
                writer.Write(ReminderText);
                writer.Write(SuccessText);
                writer.Write(FailureText);
                writer.Write(End_TextID);

                
        }
    }
}