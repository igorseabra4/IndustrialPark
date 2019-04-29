using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTaskBox : DynaBase
    {
        public override string Note => "Version is always 2";

        public DynaTaskBox() : base()
        {
            TalkBoxID = 0;
            TaskBoxID = 0;
            TextID = 0;
            TextID_Description = 0;
            TextOrGrupID_Reminder = 0;
            TextID_Success = 0;
            TextID_Failure = 0;
            TextID_End = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (TalkBoxID == assetID)
                return true;
            if (TaskBoxID == assetID)
                return true;
            if (TextID == assetID)
                return true;
            if (TextID_Description == assetID)
                return true;
            if (TextOrGrupID_Reminder == assetID)
                return true;
            if (TextID_Success == assetID)
                return true;
            if (TextID_Failure == assetID)
                return true;
            if (TextID_End == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(TalkBoxID, ref result);
            Asset.Verify(TaskBoxID, ref result);
            Asset.Verify(TextID, ref result);
            Asset.Verify(TextID_Description, ref result);
            Asset.Verify(TextOrGrupID_Reminder, ref result);
            Asset.Verify(TextID_Success, ref result);
            Asset.Verify(TextID_Failure, ref result);
            Asset.Verify(TextID_End, ref result);
        }

        public DynaTaskBox(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Flag1 = Data[0x00];
            Flag2 = Data[0x01];
            Flag3 = Data[0x02];
            Flag4 = Data[0x03];
            TalkBoxID = Switch(BitConverter.ToUInt32(Data, 0x04));
            TaskBoxID = Switch(BitConverter.ToUInt32(Data, 0x08));
            TextID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            TextID_Description = Switch(BitConverter.ToUInt32(Data, 0x10));
            TextOrGrupID_Reminder = Switch(BitConverter.ToUInt32(Data, 0x14));
            TextID_Success = Switch(BitConverter.ToUInt32(Data, 0x18));
            TextID_Failure = Switch(BitConverter.ToUInt32(Data, 0x1C));
            TextID_End = Switch(BitConverter.ToUInt32(Data, 0x20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>
            {
                Flag1,
                Flag2,
                Flag3,
                Flag4
            };
            list.AddRange(BitConverter.GetBytes(Switch(TalkBoxID)));
            list.AddRange(BitConverter.GetBytes(Switch(TaskBoxID)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID_Description)));
            list.AddRange(BitConverter.GetBytes(Switch(TextOrGrupID_Reminder)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID_Success)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID_Failure)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID_End)));
            return list.ToArray();
        }

        public byte Flag1 { get; set; }
        public byte Flag2 { get; set; }
        public byte Flag3 { get; set; }
        public byte Flag4 { get; set; }
        public AssetID TalkBoxID { get; set; }
        public AssetID TaskBoxID { get; set; }
        public AssetID TextID { get; set; }
        public AssetID TextID_Description { get; set; }
        public AssetID TextOrGrupID_Reminder { get; set; }
        public AssetID TextID_Success { get; set; }
        public AssetID TextID_Failure { get; set; }
        public AssetID TextID_End { get; set; }
    }
}