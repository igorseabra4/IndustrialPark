using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            TextID1 = 0;
            TextID2 = 0;
            ReminderTextID = 0;
            SuccessTextID = 0;
            FailureTextID = 0;
            EndTextID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (TalkBoxID == assetID)
                return true;
            if (TaskBoxID == assetID)
                return true;
            if (TextID1 == assetID)
                return true;
            if (TextID2 == assetID)
                return true;
            if (ReminderTextID == assetID)
                return true;
            if (SuccessTextID == assetID)
                return true;
            if (FailureTextID == assetID)
                return true;
            if (EndTextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaTaskBox(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Flags1 = Data[0x0];
            Flags2 = Data[0x1];
            Flags3 = Data[0x2];
            Flags4 = Data[0x3];
            TalkBoxID = Switch(BitConverter.ToUInt32(Data, 0x4));
            TaskBoxID = Switch(BitConverter.ToUInt32(Data, 0x8));
            TextID1 = Switch(BitConverter.ToUInt32(Data, 0xC));
            TextID2 = Switch(BitConverter.ToUInt32(Data, 0x10));
            ReminderTextID = Switch(BitConverter.ToUInt32(Data, 0x14));
            SuccessTextID = Switch(BitConverter.ToUInt32(Data, 0x18));
            FailureTextID = Switch(BitConverter.ToUInt32(Data, 0x1C));
            EndTextID = Switch(BitConverter.ToUInt32(Data, 0x20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.Add(Flags1);
            list.Add(Flags2);
            list.Add(Flags3);
            list.Add(Flags4);
            list.AddRange(BitConverter.GetBytes(Switch(TalkBoxID)));
            list.AddRange(BitConverter.GetBytes(Switch(TaskBoxID)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID1)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID2)));
            list.AddRange(BitConverter.GetBytes(Switch(ReminderTextID)));
            list.AddRange(BitConverter.GetBytes(Switch(SuccessTextID)));
            list.AddRange(BitConverter.GetBytes(Switch(FailureTextID)));
            list.AddRange(BitConverter.GetBytes(Switch(EndTextID)));
            return list.ToArray();
        }
        
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags1 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags2 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags3 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags4 { get; set; }
        public AssetID TalkBoxID { get; set; }
        public AssetID TaskBoxID { get; set; }
        public AssetID TextID1 { get; set; }
        public AssetID TextID2 { get; set; }
        public AssetID ReminderTextID { get; set; }
        public AssetID SuccessTextID { get; set; }
        public AssetID FailureTextID { get; set; }
        public AssetID EndTextID { get; set; }
    }
}