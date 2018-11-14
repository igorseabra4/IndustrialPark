using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaNPCSettings : DynaBase
    {
        public override string Note => "Version is always 2";

        public DynaNPCSettings() : base() { }

        public DynaNPCSettings(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Unknown1 = Switch(BitConverter.ToInt32(Data, 0x0));
            Flags1 = Data[0x4];
            Flags2 = Data[0x5];
            Flags3 = Data[0x6];
            Flags4 = Data[0x7];
            Flags5 = Data[0x8];
            Flags6 = Data[0x9];
            Flags7 = Data[0xA];
            Flags8 = Data[0xB];
            Flags9 = Data[0xC];
            Flags10 = Data[0xD];
            Flags11 = Data[0xE];
            Flags12 = Data[0xF];
            Unknown2 = Switch(BitConverter.ToInt32(Data, 0x10));
            DuploSpawnRate = Switch(BitConverter.ToSingle(Data, 0x14));
            DuploEnemyLimit = Switch(BitConverter.ToInt32(Data, 0x18));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(Unknown1)));
            list.Add(Flags1);
            list.Add(Flags2);
            list.Add(Flags3);
            list.Add(Flags4);
            list.Add(Flags5);
            list.Add(Flags6);
            list.Add(Flags7);
            list.Add(Flags8);
            list.Add(Flags9);
            list.Add(Flags10);
            list.Add(Flags11);
            list.Add(Flags12);
            list.AddRange(BitConverter.GetBytes(Switch(Unknown2)));
            list.AddRange(BitConverter.GetBytes(Switch(DuploSpawnRate)));
            list.AddRange(BitConverter.GetBytes(Switch(DuploEnemyLimit)));
            return list.ToArray();
        }

        public int Unknown1 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags1 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags2 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags3 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags4 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags5 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags6 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags7 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags8 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags9 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags10 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags11 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags12 { get; set; }
        public int Unknown2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DuploSpawnRate { get; set; }
        public int DuploEnemyLimit { get; set; }
    }
}