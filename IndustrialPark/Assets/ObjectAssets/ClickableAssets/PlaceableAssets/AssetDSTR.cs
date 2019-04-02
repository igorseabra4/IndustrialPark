using HipHopFile;
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