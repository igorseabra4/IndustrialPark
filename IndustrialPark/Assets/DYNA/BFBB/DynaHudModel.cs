using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaHudModel : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaHudModel() : base()
        {
            Model_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (Model_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Model_AssetID, ref result);
        }

        public DynaHudModel(IEnumerable<byte> enumerable) : base (enumerable)
        {
            PositionX = Switch(BitConverter.ToSingle(Data, 0x0));
            PositionY = Switch(BitConverter.ToSingle(Data, 0x4));
            PositionZ = Switch(BitConverter.ToSingle(Data, 0x8));
            SizeX = Switch(BitConverter.ToSingle(Data, 0xC));
            SizeY = Switch(BitConverter.ToSingle(Data, 0x10));
            SizeZ = Switch(BitConverter.ToSingle(Data, 0x14));
            Model_AssetID = Switch(BitConverter.ToUInt32(Data, 0x18));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(SizeX)));
            list.AddRange(BitConverter.GetBytes(Switch(SizeY)));
            list.AddRange(BitConverter.GetBytes(Switch(SizeZ)));
            list.AddRange(BitConverter.GetBytes(Switch(Model_AssetID)));
            return list.ToArray();
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeX{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeY{ get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeZ{ get; set; }
        public AssetID Model_AssetID { get; set; }
    }
}