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
            BasisType = Switch(BitConverter.ToInt32(Data, 0x0));
            AllowDetect = Data[0x4];
            AllowPatrol = Data[0x5];
            AllowWander = Data[0x6];
            ReduceCollide = Data[0x7];
            UseNavSplines = Data[0x8];
            Padding09 = Data[0x9];
            Padding0A = Data[0xA];
            Padding0B = Data[0xB];
            AllowChase = Data[0xC];
            AllowAttack = Data[0xD];
            AssumeLOS = Data[0xE];
            AssumeFOV = Data[0xF];
            DuploWaveMode = Switch(BitConverter.ToInt32(Data, 0x10));
            DuploSpawnDelay = Switch(BitConverter.ToSingle(Data, 0x14));
            DuploSpawnLifeMax = Switch(BitConverter.ToInt32(Data, 0x18));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(BasisType)));
            list.Add(AllowDetect);
            list.Add(AllowPatrol);
            list.Add(AllowWander);
            list.Add(ReduceCollide);
            list.Add(UseNavSplines);
            list.Add(Padding09);
            list.Add(Padding0A);
            list.Add(Padding0B);
            list.Add(AllowChase);
            list.Add(AllowAttack);
            list.Add(AssumeLOS);
            list.Add(AssumeFOV);
            list.AddRange(BitConverter.GetBytes(Switch(DuploWaveMode)));
            list.AddRange(BitConverter.GetBytes(Switch(DuploSpawnDelay)));
            list.AddRange(BitConverter.GetBytes(Switch(DuploSpawnLifeMax)));
            return list.ToArray();
        }

        public int BasisType { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowDetect { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowPatrol { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowWander { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte ReduceCollide { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UseNavSplines { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding09 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding0A { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding0B { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowChase { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AllowAttack { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AssumeLOS { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte AssumeFOV { get; set; }
        public int DuploWaveMode { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DuploSpawnDelay { get; set; }
        public int DuploSpawnLifeMax { get; set; }
    }
}