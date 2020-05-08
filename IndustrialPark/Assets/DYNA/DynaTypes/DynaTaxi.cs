using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaTaxi : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x20;

        public DynaTaxi(AssetDYNA asset) : base(asset) { }
        
        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (CAM_ID == assetID)
                return true;
            if (PORT_ID == assetID)
                return true;
            if (DYNA_Talkbox_ID == assetID)
                return true;
            if (TEXT_ID == assetID)
                return true;
            if (SIMP_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(MRKR_ID, ref result);
            Asset.Verify(CAM_ID, ref result);
            Asset.Verify(PORT_ID, ref result);
            Asset.Verify(DYNA_Talkbox_ID, ref result);
            Asset.Verify(TEXT_ID, ref result);
            Asset.Verify(SIMP_ID, ref result);
        }
        
        public AssetID MRKR_ID 
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public AssetID CAM_ID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
        public AssetID PORT_ID
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }
        public AssetID DYNA_Talkbox_ID
        {
            get => ReadUInt(0x0C);
            set => Write(0x0C, value);
        }
        public AssetID TEXT_ID
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }
        public AssetID SIMP_ID
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float InvisibleTimer
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TeleportTimer
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
    }
}