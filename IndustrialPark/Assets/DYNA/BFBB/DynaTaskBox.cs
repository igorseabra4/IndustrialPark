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
            TalkBox_AssetID = 0;
            NextTaskBox_AssetID = 0;
            Begin_TextID = 0;
            Description_TextID = 0;
            Reminder_TextID = 0;
            Success_TextID = 0;
            Failure_TextID = 0;
            End_TextID = 0;
        }

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

        public DynaTaskBox(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Persistent = Data[0x00];
            Loop = Data[0x01];
            Enable = Data[0x02];
            Retry = Data[0x03];
            TalkBox_AssetID = Switch(BitConverter.ToUInt32(Data, 0x04));
            NextTaskBox_AssetID = Switch(BitConverter.ToUInt32(Data, 0x08));
            Begin_TextID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            Description_TextID = Switch(BitConverter.ToUInt32(Data, 0x10));
            Reminder_TextID = Switch(BitConverter.ToUInt32(Data, 0x14));
            Success_TextID = Switch(BitConverter.ToUInt32(Data, 0x18));
            Failure_TextID = Switch(BitConverter.ToUInt32(Data, 0x1C));
            End_TextID = Switch(BitConverter.ToUInt32(Data, 0x20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>
            {
                Persistent,
                Loop,
                Enable,
                Retry
            };
            list.AddRange(BitConverter.GetBytes(Switch(TalkBox_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(NextTaskBox_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Begin_TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(Description_TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(Reminder_TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(Success_TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(Failure_TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(End_TextID)));
            return list.ToArray();
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Persistent { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Loop { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Enable { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Retry { get; set; }
        public AssetID TalkBox_AssetID { get; set; }
        public AssetID NextTaskBox_AssetID { get; set; }
        public AssetID Begin_TextID { get; set; }
        public AssetID Description_TextID { get; set; }
        public AssetID Reminder_TextID { get; set; }
        public AssetID Success_TextID { get; set; }
        public AssetID Failure_TextID { get; set; }
        public AssetID End_TextID { get; set; }
    }
}