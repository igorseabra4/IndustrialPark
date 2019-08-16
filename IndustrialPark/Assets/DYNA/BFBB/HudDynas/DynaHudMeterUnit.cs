using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaHudMeterUnit : DynaHudMeterBase
    {
        public DynaHudMeterUnit(Platform platform) : base(platform)
        {
            EmptyModel_AssetID = 0;
            FullModel_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (EmptyModel_AssetID == assetID)
                return true;
            if (FullModel_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(EmptyModel_AssetID, ref result);
            Asset.Verify(FullModel_AssetID, ref result);
        }

        public DynaHudMeterUnit(IEnumerable<byte> enumerable, Platform platform) : base(enumerable, platform)
        {
            EmptyModel_AssetID = Switch(BitConverter.ToUInt32(Data, 0x3C));
            EmptyOffset_X = Switch(BitConverter.ToSingle(Data, 0x40));
            EmptyOffset_Y = Switch(BitConverter.ToSingle(Data, 0x44));
            EmptyOffset_Z = Switch(BitConverter.ToSingle(Data, 0x48));
            EmptyScale_X = Switch(BitConverter.ToSingle(Data, 0x4C));
            EmptyScale_Y = Switch(BitConverter.ToSingle(Data, 0x50));
            EmptyScale_Z = Switch(BitConverter.ToSingle(Data, 0x54));
            FullModel_AssetID = Switch(BitConverter.ToUInt32(Data, 0x58));
            FullOffset_X = Switch(BitConverter.ToSingle(Data, 0x5C));
            FullOffset_Y = Switch(BitConverter.ToSingle(Data, 0x60));
            FullOffset_Z = Switch(BitConverter.ToSingle(Data, 0x64));
            FullScale_X = Switch(BitConverter.ToSingle(Data, 0x68));
            FullScale_Y = Switch(BitConverter.ToSingle(Data, 0x6C));
            FullScale_Z = Switch(BitConverter.ToSingle(Data, 0x70));
            Spacing_X = Switch(BitConverter.ToSingle(Data, 0x74));
            Spacing_Y = Switch(BitConverter.ToSingle(Data, 0x78));
            Spacing_Z = Switch(BitConverter.ToSingle(Data, 0x7C));
            MeterFillDirection = Switch(BitConverter.ToInt32(Data, 0x80));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();
            list.AddRange(BitConverter.GetBytes(Switch(EmptyModel_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyOffset_X)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyOffset_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyOffset_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyScale_X)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyScale_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(EmptyScale_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(FullModel_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(FullOffset_X)));
            list.AddRange(BitConverter.GetBytes(Switch(FullOffset_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(FullOffset_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(FullScale_X)));
            list.AddRange(BitConverter.GetBytes(Switch(FullScale_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(FullScale_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(Spacing_X)));
            list.AddRange(BitConverter.GetBytes(Switch(Spacing_Y)));
            list.AddRange(BitConverter.GetBytes(Switch(Spacing_Z)));
            list.AddRange(BitConverter.GetBytes(Switch(MeterFillDirection)));
            return list.ToArray();
        }

        public AssetID EmptyModel_AssetID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_X { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_Y { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyOffset_Z { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_X { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_Y { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float EmptyScale_Z { get; set; }
        public AssetID FullModel_AssetID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_X { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_Y { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullOffset_Z { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_X { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_Y { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FullScale_Z { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_X { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_Y { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Spacing_Z { get; set; }
        public int MeterFillDirection { get; set; }
    }
}