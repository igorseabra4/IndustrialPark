using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTalkBox : DynaBase
    {
        public  string Note => "Version is always 11";

        public override int StructSize => 0x38;

        public DynaGObjectTalkBox(AssetDYNA asset) : base(asset) { }

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
        
        public AssetID Dialog_TextBoxID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public AssetID Prompt_TextBoxID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
        public AssetID Quit_TextBoxID
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Trap
        {
            get => ReadByte(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Pause
        {
            get => ReadByte(0x0D);
            set => Write(0x0D, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowQuit
        {
            get => ReadByte(0x0E);
            set => Write(0x0E, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte TriggerPads
        {
            get => ReadByte(0x0F);
            set => Write(0x0F, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Page
        {
            get => ReadByte(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Show
        {
            get => ReadByte(0x11);
            set => Write(0x11, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Hide
        {
            get => ReadByte(0x12);
            set => Write(0x12, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AudioEffect
        {
            get => ReadByte(0x13);
            set => Write(0x13, value);
        }
        public AssetID TeleportPointerID
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypeTime
        {
            get => ReadByte(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypePrompt
        {
            get => ReadByte(0x19);
            set => Write(0x19, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypeSound
        {
            get => ReadByte(0x1A);
            set => Write(0x1A, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AutoWaitTypeEvent
        {
            get => ReadByte(0x1B);
            set => Write(0x1B, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AutoWaitDelay
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        public int AutoWaitWhichEvent
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
        public AssetID PromptSkip_TextID
        {
            get => ReadUInt(0x24);
            set => Write(0x24, value);
        }
        public AssetID PromptNoSkip_TextID
        {
            get => ReadUInt(0x28);
            set => Write(0x28, value);
        }
        public AssetID PromptQuitTextID
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }
        public AssetID PromptNoQuitTextID
        {
            get => ReadUInt(0x30);
            set => Write(0x30, value);
        }
        public AssetID PromptYesNoTextID
        {
            get => ReadUInt(0x34);
            set => Write(0x34, value);
        }
    }
}