using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR : AssetDSTR_Scooby
    {
        protected override int EventStartOffset => 0x8C + Offset;

        public AssetDSTR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) => DestroyShrapnel_AssetID == assetID || HitShrapnel_AssetID == assetID || DestroySFX_AssetID == assetID ||
            HitSFX_AssetID == assetID || HitModel_AssetID == assetID || DestroyModel_AssetID == assetID || base.HasReference(assetID);
        
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
        
        [Category("Destructable"), Description("Not present in Scooby")]
        public AssetID DestroyShrapnel_AssetID
        {
            get => ReadUInt(0x74);
            set => Write(0x74, value);
        }

        [Category("Destructable"), Description("Not present in Scooby")]
        public AssetID HitShrapnel_AssetID
        {
            get => ReadUInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Destructable"), Description("Not present in Scooby")]
        public AssetID DestroySFX_AssetID
        {
            get => ReadUInt(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Destructable"), Description("Not present in Scooby")]
        public AssetID HitSFX_AssetID
        {
            get => ReadUInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Destructable"), Description("Not present in Scooby")]
        public AssetID HitModel_AssetID
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Destructable"), Description("Not present in Scooby")]
        public AssetID DestroyModel_AssetID
        {
            get => ReadUInt(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }
    }
}