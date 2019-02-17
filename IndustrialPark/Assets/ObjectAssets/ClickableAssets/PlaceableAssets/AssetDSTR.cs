using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

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
        
        [Category("Destructable")]
        public int AnimationSpeed
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Destructable")]
        public int InitialAnimationState
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Destructable")]
        public int Health
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Destructable")]
        public AssetID SpawnItemAssetID
        {
            get => ReadUInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Destructable")]
        public int MaybeHitMask
        {
            get => ReadInt(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Destructable")]
        public byte CollType
        {
            get => ReadByte(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Destructable")]
        public byte FxType
        {
            get => ReadByte(0x69 + Offset);
            set => Write(0x69 + Offset, value);
        }

        [Category("Destructable")]
        public short Padding6A
        {
            get => ReadShort(0x6A + Offset);
            set => Write(0x6A + Offset, value);
        }
        
        [Category("Destructable"), TypeConverter(typeof(FloatTypeConverter))]
        public float BlastRadius
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }

        [Category("Destructable"), TypeConverter(typeof(FloatTypeConverter))]
        public float BlastStrength
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }

        [Category("Destructable")]
        public AssetID DestroyShrapnel_AssetID
        {
            get => ReadUInt(0x74);
            set => Write(0x74, value);
        }

        [Category("Destructable")]
        public AssetID HitShrapnel_AssetID
        {
            get => ReadUInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Destructable")]
        public AssetID DestroySFX_AssetID
        {
            get => ReadUInt(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Destructable")]
        public AssetID HitSFX_AssetID
        {
            get => ReadUInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Destructable")]
        public AssetID HitModel_AssetID
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Destructable")]
        public AssetID DestroyModel_AssetID
        {
            get => ReadUInt(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }
    }
}