using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTalkBox : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:talk_box";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 11;

        [Category(dynaCategoryName)]
        public AssetID Dialog_TextBoxID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Prompt_TextBoxID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Quit_TextBoxID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Trap { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Pause { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AllowQuit { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte TriggerPads { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Page { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Show { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Hide { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AudioEffect { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TeleportPointerID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AutoWaitTypeTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AutoWaitTypePrompt { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AutoWaitTypeSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AutoWaitTypeEvent { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AutoWaitDelay { get; set; }
        [Category(dynaCategoryName)]
        public int AutoWaitWhichEvent { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PromptSkip_TextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PromptNoSkip_TextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PromptQuitTextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PromptNoQuitTextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID PromptYesNoTextID { get; set; }

        public DynaGObjectTalkBox(string assetName, bool checkpointTalkbox) : base(assetName, DynaType.game_object__talk_box)
        {
            if (checkpointTalkbox)
            {
                Dialog_TextBoxID = 0x9BC49154;
                AutoWaitTypeTime = 1;
                AutoWaitDelay = 2f;
            }
        }

        public DynaGObjectTalkBox(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__talk_box, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Dialog_TextBoxID = reader.ReadUInt32();
                Prompt_TextBoxID = reader.ReadUInt32();
                Quit_TextBoxID = reader.ReadUInt32();
                Trap = reader.ReadByte();
                Pause = reader.ReadByte();
                AllowQuit = reader.ReadByte();
                TriggerPads = reader.ReadByte();
                Page = reader.ReadByte();
                Show = reader.ReadByte();
                Hide = reader.ReadByte();
                AudioEffect = reader.ReadByte();
                TeleportPointerID = reader.ReadUInt32();
                AutoWaitTypeTime = reader.ReadByte();
                AutoWaitTypePrompt = reader.ReadByte();
                AutoWaitTypeSound = reader.ReadByte();
                AutoWaitTypeEvent = reader.ReadByte();
                AutoWaitDelay = reader.ReadSingle();
                AutoWaitWhichEvent = reader.ReadInt32();
                PromptSkip_TextID = reader.ReadUInt32();
                PromptNoSkip_TextID = reader.ReadUInt32();
                PromptQuitTextID = reader.ReadUInt32();
                PromptNoQuitTextID = reader.ReadUInt32();
                PromptYesNoTextID = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Dialog_TextBoxID);
                writer.Write(Prompt_TextBoxID);
                writer.Write(Quit_TextBoxID);
                writer.Write(Trap);
                writer.Write(Pause);
                writer.Write(AllowQuit);
                writer.Write(TriggerPads);
                writer.Write(Page);
                writer.Write(Show);
                writer.Write(Hide);
                writer.Write(AudioEffect);
                writer.Write(TeleportPointerID);
                writer.Write(AutoWaitTypeTime);
                writer.Write(AutoWaitTypePrompt);
                writer.Write(AutoWaitTypeSound);
                writer.Write(AutoWaitTypeEvent);
                writer.Write(AutoWaitDelay);
                writer.Write(AutoWaitWhichEvent);
                writer.Write(PromptSkip_TextID);
                writer.Write(PromptNoSkip_TextID);
                writer.Write(PromptQuitTextID);
                writer.Write(PromptNoQuitTextID);
                writer.Write(PromptYesNoTextID);

                return writer.ToArray();
            }
        }
    }
}