using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaVentType : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x18;

        public DynaVentType(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (Constant_PARE == assetID)
                return true;
            if (Constant_SGRP == assetID)
                return true;
            if (Warning_PARE == assetID)
                return true;
            if (Warning_SGRP == assetID)
                return true;
            if (Emit_PARE == assetID)
                return true;
            if (Emit_SGRP == assetID)
                return true;

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Constant_PARE, ref result);
            Asset.Verify(Constant_SGRP, ref result);
            Asset.Verify(Warning_PARE, ref result);
            Asset.Verify(Warning_SGRP, ref result);
            Asset.Verify(Emit_PARE, ref result);
            Asset.Verify(Emit_SGRP, ref result);
        }
                
        public AssetID Constant_PARE
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public AssetID Constant_SGRP
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
        public AssetID Warning_PARE
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }
        public AssetID Warning_SGRP
        {
            get => ReadUInt(0x0C);
            set => Write(0x0C, value);
        }
        public AssetID Emit_PARE
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }
        public AssetID Emit_SGRP
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
    }
}