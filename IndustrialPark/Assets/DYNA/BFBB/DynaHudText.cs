using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaHudText : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaHudText() : base()
        {
            TextboxID = 0;
            TextID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (TextboxID == assetID)
                return true;
            if (TextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(TextboxID, ref result);
            Asset.Verify(TextID, ref result);
        }

        public DynaHudText(IEnumerable<byte> enumerable) : base (enumerable)
        {
            PositionX = Switch(BitConverter.ToSingle(Data, 0x0));
            PositionY = Switch(BitConverter.ToSingle(Data, 0x4));
            PositionZ = Switch(BitConverter.ToSingle(Data, 0x8));
            SizeX = Switch(BitConverter.ToSingle(Data, 0xC));
            SizeY = Switch(BitConverter.ToSingle(Data, 0x10));
            SizeZ = Switch(BitConverter.ToSingle(Data, 0x14));
            TextboxID = Switch(BitConverter.ToUInt32(Data, 0x18));
            TextID = Switch(BitConverter.ToUInt32(Data, 0x1C));
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
            list.AddRange(BitConverter.GetBytes(Switch(TextboxID)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID)));

            return list.ToArray();
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float SizeZ { get; set; }
        public AssetID TextboxID { get; set; }
        public AssetID TextID { get; set; }
    }
}