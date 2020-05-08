using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaTaskBox : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x24;

        public DynaTaskBox(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (TalkBox_AssetID == assetID)
                return true;
            if (NextTaskBox_AssetID == assetID)
                return true;
            if (Begin_TextID == assetID)
                return true;
            if (Description_TextID == assetID)
                return true;
            if (Reminder_TextID == assetID)
                return true;
            if (Success_TextID == assetID)
                return true;
            if (Failure_TextID == assetID)
                return true;
            if (End_TextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(TalkBox_AssetID, ref result);
            Asset.Verify(NextTaskBox_AssetID, ref result);
            Asset.Verify(Begin_TextID, ref result);
            Asset.Verify(Description_TextID, ref result);
            Asset.Verify(Reminder_TextID, ref result);
            Asset.Verify(Success_TextID, ref result);
            Asset.Verify(Failure_TextID, ref result);
            Asset.Verify(End_TextID, ref result);
        }
        
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Persistent
        {
            get => ReadByte(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Loop
        {
            get => ReadByte(0x01);
            set => Write(0x01, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Enable
        {
            get => ReadByte(0x02);
            set => Write(0x02, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Retry
        {
            get => ReadByte(0x03);
            set => Write(0x03, value);
        }
        public AssetID TalkBox_AssetID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
        public AssetID NextTaskBox_AssetID
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }
        public AssetID Begin_TextID
        {
            get => ReadUInt(0x0C);
            set => Write(0x0C, value);
        }
        public AssetID Description_TextID
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }
        public AssetID Reminder_TextID
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
        public AssetID Success_TextID
        {
            get => ReadUInt(0x18);
            set => Write(0x18, value);
        }
        public AssetID Failure_TextID
        {
            get => ReadUInt(0x1C);
            set => Write(0x1C, value);
        }
        public AssetID End_TextID
        {
            get => ReadUInt(0x20);
            set => Write(0x20, value);
        }
    }
}