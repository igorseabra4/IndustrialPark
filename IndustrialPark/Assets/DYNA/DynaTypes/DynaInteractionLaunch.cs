using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaInteractionLaunch : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x1C;

        public DynaInteractionLaunch(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID) =>
            SIMP_AssetID == assetID || Marker_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(SIMP_AssetID, ref result);
            Asset.Verify(Marker_AssetID, ref result);
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_00
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        public AssetID SIMP_AssetID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
        public AssetID Marker_AssetID
        {
            get => ReadUInt(0x08);
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
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
    }
}