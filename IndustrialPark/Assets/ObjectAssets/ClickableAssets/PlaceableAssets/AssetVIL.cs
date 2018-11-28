using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetVIL : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x6C + Offset;

        public AssetVIL(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if ((uint)VilType == assetID)
                return true;
            if (AssetID_DYNA_NPCSettings == assetID)
                return true;
            if (AssetID_MVPT == assetID)
                return true;
            if (AssetID_DYNA_1 == assetID)
                return true;
            if (AssetID_DYNA_2 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("VIL")]
        public int Unknown54
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("VIL")]
        public VilType VilType
        {
            get => (VilType)ReadUInt(0x58 + Offset);
            set => Write(0x58 + Offset, (uint)value);
        }

        [Category("VIL")]
        public AssetID AssetID_DYNA_NPCSettings
        {
            get => ReadUInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("VIL")]
        public AssetID AssetID_MVPT
        {
            get => ReadUInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("VIL")]
        public AssetID AssetID_DYNA_1
        {
            get => ReadUInt(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("VIL")]
        public AssetID AssetID_DYNA_2
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }
    }
}