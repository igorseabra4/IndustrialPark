using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTalkBox : DynaBase
    {
        public override string Note => "Version is always 11";

        public DynaTalkBox() : base()
        {
            Dialog_TextBoxID = 0;
            Prompt_TextBoxID = 0;
            Quit_TextBoxID = 0;
            TeleportPointerID = 0;
            PromptSkip_TextID = 0;
            PromptNoSkip_TextID = 0;
            PromptQuitTextID = 0;
            PromptNoQuitTextID = 0;
            PromptYesNoTextID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (Dialog_TextBoxID == assetID)
                return true;
            if (Prompt_TextBoxID == assetID)
                return true;
            if (Quit_TextBoxID == assetID)
                return true;
            if (TeleportPointerID == assetID)
                return true;
            if (PromptSkip_TextID == assetID)
                return true;
            if (PromptNoSkip_TextID == assetID)
                return true;
            if (PromptQuitTextID == assetID)
                return true;
            if (PromptNoQuitTextID == assetID)
                return true;
            if (PromptYesNoTextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Dialog_TextBoxID, ref result);
            Asset.Verify(Prompt_TextBoxID, ref result);
            Asset.Verify(Quit_TextBoxID, ref result);
            Asset.Verify(TeleportPointerID, ref result);
            Asset.Verify(PromptSkip_TextID, ref result);
            Asset.Verify(PromptNoSkip_TextID, ref result);
            Asset.Verify(PromptQuitTextID, ref result);
            Asset.Verify(PromptNoQuitTextID, ref result);
            Asset.Verify(PromptYesNoTextID, ref result);
        }

        public DynaTalkBox(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Dialog_TextBoxID = Switch(BitConverter.ToUInt32(Data, 0x0));
            Prompt_TextBoxID = Switch(BitConverter.ToUInt32(Data, 0x4));
            Quit_TextBoxID = Switch(BitConverter.ToUInt32(Data, 0x8));
            Trap = Data[0xC];
            Pause = Data[0xD];
            AllowQuit = Data[0xE];
            TriggerPads = Data[0xF];
            Page = Data[0x10];
            Show = Data[0x11];
            Hide = Data[0x12];
            AudioEffect = Data[0x13];
            TeleportPointerID = Switch(BitConverter.ToUInt32(Data, 0x14));
            AutoWaitTypeTime = Data[0x18];
            AutoWaitTypePrompt = Data[0x19];
            AutoWaitTypeSound = Data[0x1A];
            AutoWaitTypeEvent = Data[0x1B];
            AutoWaitDelay = Switch(BitConverter.ToSingle(Data, 0x1C));
            AutoWaitWhichEvent = Switch(BitConverter.ToInt32(Data, 0x20));
            PromptSkip_TextID = Switch(BitConverter.ToUInt32(Data, 0x24));
            PromptNoSkip_TextID = Switch(BitConverter.ToUInt32(Data, 0x28));
            PromptQuitTextID = Switch(BitConverter.ToUInt32(Data, 0x2C));
            PromptNoQuitTextID = Switch(BitConverter.ToUInt32(Data, 0x30));
            PromptYesNoTextID = Switch(BitConverter.ToUInt32(Data, 0x34));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Dialog_TextBoxID)));
            list.AddRange(BitConverter.GetBytes(Switch(Prompt_TextBoxID)));
            list.AddRange(BitConverter.GetBytes(Switch(Quit_TextBoxID)));
            list.Add(Trap);
            list.Add(Pause);
            list.Add(AllowQuit);
            list.Add(TriggerPads);
            list.Add(Page);
            list.Add(Show);
            list.Add(Hide);
            list.Add(AudioEffect);
            list.AddRange(BitConverter.GetBytes(Switch(TeleportPointerID)));
            list.Add(AutoWaitTypeTime);
            list.Add(AutoWaitTypePrompt);
            list.Add(AutoWaitTypeSound);
            list.Add(AutoWaitTypeEvent);
            list.AddRange(BitConverter.GetBytes(Switch(AutoWaitDelay)));
            list.AddRange(BitConverter.GetBytes(Switch(AutoWaitWhichEvent)));
            list.AddRange(BitConverter.GetBytes(Switch(PromptSkip_TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(PromptNoSkip_TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(PromptQuitTextID)));
            list.AddRange(BitConverter.GetBytes(Switch(PromptNoQuitTextID)));
            list.AddRange(BitConverter.GetBytes(Switch(PromptYesNoTextID)));
            return list.ToArray();
        }

        public AssetID Dialog_TextBoxID { get; set; }
        public AssetID Prompt_TextBoxID { get; set; }
        public AssetID Quit_TextBoxID { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Trap { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Pause { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowQuit { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte TriggerPads { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Page { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Show { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Hide { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AudioEffect { get; set; }
        public AssetID TeleportPointerID { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypeTime { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypePrompt { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypeSound { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypeEvent { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AutoWaitDelay { get; set; }
        public int AutoWaitWhichEvent { get; set; }
        public AssetID PromptSkip_TextID { get; set; }
        public AssetID PromptNoSkip_TextID { get; set; }
        public AssetID PromptQuitTextID { get; set; }
        public AssetID PromptNoQuitTextID { get; set; }
        public AssetID PromptYesNoTextID { get; set; }
    }
}