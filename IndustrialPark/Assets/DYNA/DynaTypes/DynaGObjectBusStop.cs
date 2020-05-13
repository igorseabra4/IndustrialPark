using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBusStop : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x14;

        public DynaGObjectBusStop(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;
            if (CAM_ID == assetID)
                return true;
            if (SIMP_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (MRKR_ID == 0)
                result.Add("Bus stop with no MRKR reference");
            Asset.Verify(MRKR_ID, ref result);
            if (CAM_ID == 0)
                result.Add("Bus stop with no CAM reference");
            Asset.Verify(CAM_ID, ref result);
            if (SIMP_ID == 0)
                result.Add("Bus stop with no SIMP reference");
            Asset.Verify(SIMP_ID, ref result);
        }
                
        public AssetID MRKR_ID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public enum PlayerEnum
        {
            Patrick = 0,
            Sandy = 1
        }
        public PlayerEnum Player
        {
            get => (PlayerEnum)ReadInt(0x04);
            set => Write(0x04, (int)value);
        }
        public AssetID CAM_ID
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }
        public AssetID SIMP_ID
        {
            get => ReadUInt(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Delay
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
    }
}