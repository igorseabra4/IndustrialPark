using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaSceneProperties : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaSceneProperties(Platform platform) : base(platform)
        {
            Sound_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            return Sound_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            if (Sound_AssetID == 0)
                result.Add("Scene Properties with no song reference");
            Asset.Verify(Sound_AssetID, ref result);
        }

        public DynaSceneProperties(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            UnknownInt1 = Switch(BitConverter.ToInt32(Data, 0x00));
            UnknownInt2 = Switch(BitConverter.ToInt32(Data, 0x04));
            UnknownInt3 = Switch(BitConverter.ToInt32(Data, 0x08));
            UnknownInt4 = Switch(BitConverter.ToInt32(Data, 0x0C));
            Flag1 = Data[0x10];
            Flag2 = Data[0x11];
            Flag3 = Data[0x12];
            Flag4 = Data[0x13];
            Sound_AssetID = Switch(BitConverter.ToUInt32(Data, 0x14));
            UnknownInt5 = Switch(BitConverter.ToInt32(Data, 0x18));
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x1C));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x20));
            UnknownInt6 = Switch(BitConverter.ToInt32(Data, 0x24));
            UnknownInt7 = Switch(BitConverter.ToInt32(Data, 0x28));
            UnknownInt8 = Switch(BitConverter.ToInt32(Data, 0x2C));
            UnknownInt9 = Switch(BitConverter.ToInt32(Data, 0x30));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt4)));
            list.Add(Flag1);
            list.Add(Flag2);
            list.Add(Flag3);
            list.Add(Flag4);
            list.AddRange(BitConverter.GetBytes(Switch(Sound_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt5)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt6)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt7)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt8)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt9)));

            return list.ToArray();
        }

        public int UnknownInt1 { get; set; }
        public int UnknownInt2 { get; set; }
        public int UnknownInt3 { get; set; }
        public int UnknownInt4 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag1 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag2 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag3 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag4 { get; set; }
        public AssetID Sound_AssetID { get; set; }
        public int UnknownInt5 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }
        public int UnknownInt6 { get; set; }
        public int UnknownInt7 { get; set; }
        public int UnknownInt8 { get; set; }
        public int UnknownInt9 { get; set; }
    }
}