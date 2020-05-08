using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaSceneProperties : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x34;

        public DynaSceneProperties(AssetDYNA asset) : base(asset) { }
        
        public override bool HasReference(uint assetID) => Sound_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            if (Sound_AssetID == 0)
                result.Add("Scene Properties with no song reference");
            Asset.Verify(Sound_AssetID, ref result);
        }
                
        public int UnknownInt00
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        public int UnknownInt04
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }
        public int UnknownInt08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }
        public int UnknownInt0C
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag10
        {
            get => ReadByte(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag11
        {
            get => ReadByte(0x11);
            set => Write(0x11, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag12
        {
            get => ReadByte(0x12);
            set => Write(0x12, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag13
        {
            get => ReadByte(0x13);
            set => Write(0x13, value);
        }
        public AssetID Sound_AssetID
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
        public int UnknownInt18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        public int UnknownInt24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }
        public int UnknownInt28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }
        public int UnknownInt2C
        {
            get => ReadInt(0x2C);
            set => Write(0x2C, value);
        }
        public int UnknownInt30
        {
            get => ReadInt(0x30);
            set => Write(0x30, value);
        }
    }
}