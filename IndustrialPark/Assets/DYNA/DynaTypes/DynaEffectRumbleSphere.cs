using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectRumbleSphere : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x18;

        public DynaEffectRumbleSphere(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (Rumble_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Rumble_AssetID, ref result);
        }

        public AssetID Rumble_AssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_04
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_08
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_0C
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        public short UnknownShort_14
        {
            get => ReadShort(0x14);
            set => Write(0x14, value);
        }
        public short UnknownShort_16
        {
            get => ReadShort(0x16);
            set => Write(0x16, value);
        }
    }
}