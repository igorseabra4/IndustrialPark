using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR : AssetDSTR_Scooby
    {
        protected override int EventStartOffset => 0x8C + Offset;

        public AssetDSTR(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (DestroyShrapnel_AssetID == assetID)
                return true;
            if (HitShrapnel_AssetID == assetID)
                return true;
            if (DestroySFX_AssetID == assetID)
                return true;
            if (HitSFX_AssetID == assetID)
                return true;
            if (HitModel_AssetID == assetID)
                return true;
            if (DestroyModel_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(DestroyShrapnel_AssetID, ref result);
            Verify(HitShrapnel_AssetID, ref result);
            Verify(DestroySFX_AssetID, ref result);
            Verify(HitSFX_AssetID, ref result);
            Verify(HitModel_AssetID, ref result);
            Verify(DestroyModel_AssetID, ref result);
        }

        [Category("Destructable Ext.")]
        public AssetID DestroyShrapnel_AssetID
        {
            get => ReadUInt(0x74);
            set => Write(0x74, value);
        }

        [Category("Destructable Ext.")]
        public AssetID HitShrapnel_AssetID
        {
            get => ReadUInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Destructable Ext.")]
        public AssetID DestroySFX_AssetID
        {
            get => ReadUInt(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Destructable Ext.")]
        public AssetID HitSFX_AssetID
        {
            get => ReadUInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Destructable Ext.")]
        public AssetID HitModel_AssetID
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Destructable Ext.")]
        public AssetID DestroyModel_AssetID
        {
            get => ReadUInt(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }
    }
}