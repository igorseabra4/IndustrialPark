using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaBoulderGen : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaBoulderGen() : base()
        {
            ObjectAssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (ObjectAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaBoulderGen(IEnumerable<byte> enumerable) : base (enumerable)
        {
            ObjectAssetID = Switch(BitConverter.ToUInt32(Data, 0x0));
            OffsetX = Switch(BitConverter.ToSingle(Data, 0x04));
            OffsetY = Switch(BitConverter.ToSingle(Data, 0x08));
            OffsetZ = Switch(BitConverter.ToSingle(Data, 0x0C));
            OffsetRand = Switch(BitConverter.ToSingle(Data, 0x10));
            InitVelX = Switch(BitConverter.ToSingle(Data, 0x14));
            InitVelY = Switch(BitConverter.ToSingle(Data, 0x18));
            InitVelZ = Switch(BitConverter.ToSingle(Data, 0x1C));
            VelAngleRand = Switch(BitConverter.ToSingle(Data, 0x20));
            VelMagRand = Switch(BitConverter.ToSingle(Data, 0x24));
            InitAxisX = Switch(BitConverter.ToSingle(Data, 0x28));
            InitAxisY = Switch(BitConverter.ToSingle(Data, 0x2C));
            InitAxisZ = Switch(BitConverter.ToSingle(Data, 0x30));
            AngVel = Switch(BitConverter.ToSingle(Data, 0x34));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(ObjectAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(OffsetX)));
            list.AddRange(BitConverter.GetBytes(Switch(OffsetY)));
            list.AddRange(BitConverter.GetBytes(Switch(OffsetZ)));
            list.AddRange(BitConverter.GetBytes(Switch(OffsetRand)));
            list.AddRange(BitConverter.GetBytes(Switch(InitVelX)));
            list.AddRange(BitConverter.GetBytes(Switch(InitVelY)));
            list.AddRange(BitConverter.GetBytes(Switch(InitVelZ)));
            list.AddRange(BitConverter.GetBytes(Switch(VelAngleRand)));
            list.AddRange(BitConverter.GetBytes(Switch(VelMagRand)));
            list.AddRange(BitConverter.GetBytes(Switch(InitAxisX)));
            list.AddRange(BitConverter.GetBytes(Switch(InitAxisY)));
            list.AddRange(BitConverter.GetBytes(Switch(InitAxisZ)));
            list.AddRange(BitConverter.GetBytes(Switch(AngVel)));
            return list.ToArray();
        }

        public AssetID ObjectAssetID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float OffsetRand { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitVelX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitVelY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitVelZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VelAngleRand { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VelMagRand { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitAxisX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitAxisY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InitAxisZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float AngVel { get; set; }
    }
}