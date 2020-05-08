using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaBungeeDrop : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0xC;

        public DynaBungeeDrop(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (MRKR_ID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(MRKR_ID, ref result);
        }
        
        public AssetID MRKR_ID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public int SetViewAngle
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ViewAngle
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
    }
}