using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR : EntityAsset
    {
        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        protected override int EventStartOffset => game == Game.Scooby ? 0x70 : 0x8C + Offset;

        public AssetDSTR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) 
        {
            if (SpawnItemAssetID == assetID)
                return true;

            if (game != Game.Scooby)
                if (DestroyShrapnel_AssetID == assetID || HitShrapnel_AssetID == assetID || DestroySFX_AssetID == assetID ||
                    HitSFX_AssetID == assetID || HitModel_AssetID == assetID || DestroyModel_AssetID == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            Verify(SpawnItemAssetID, ref result);

            if (game != Game.Scooby)
            {
                Verify(DestroyShrapnel_AssetID, ref result);
                Verify(HitShrapnel_AssetID, ref result);
                Verify(DestroySFX_AssetID, ref result);
                Verify(HitSFX_AssetID, ref result);
                Verify(HitModel_AssetID, ref result);
                Verify(DestroyModel_AssetID, ref result);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("DestroyShrapnel_AssetID");
                dt.RemoveProperty("HitShrapnel_AssetID");
                dt.RemoveProperty("DestroySFX_AssetID");
                dt.RemoveProperty("HitSFX_AssetID");
                dt.RemoveProperty("HitModel_AssetID");
                dt.RemoveProperty("DestroyModel_AssetID");
            }

            base.SetDynamicProperties(dt);
        }

        protected const string categoryName = "Destructable";

        [Category(categoryName)]
        public int AnimationSpeed
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category(categoryName)]
        public int InitialAnimationState
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category(categoryName)]
        public int Health
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category(categoryName)]
        public AssetID SpawnItemAssetID
        {
            get => ReadUInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category(categoryName)]
        public DynamicTypeDescriptor MaybeHitMask => IntFlagsDescriptor(0x64 + Offset);
        
        [Category(categoryName)]
        public byte CollType
        {
            get => ReadByte(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category(categoryName)]
        public byte FxType
        {
            get => ReadByte(0x69 + Offset);
            set => Write(0x69 + Offset, value);
        }

        [Category(categoryName)]
        public short Padding6A
        {
            get => ReadShort(0x6A + Offset);
            set => Write(0x6A + Offset, value);
        }
        
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float BlastRadius
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float BlastStrength
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category(categoryName)]
        public AssetID DestroyShrapnel_AssetID
        {
            get => ReadUInt(0x74 + Offset);
            set => Write(0x74 + Offset, value);
        }

        [Category(categoryName)]
        public AssetID HitShrapnel_AssetID
        {
            get => ReadUInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category(categoryName)]
        public AssetID DestroySFX_AssetID
        {
            get => ReadUInt(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category(categoryName)]
        public AssetID HitSFX_AssetID
        {
            get => ReadUInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category(categoryName)]
        public AssetID HitModel_AssetID
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category(categoryName)]
        public AssetID DestroyModel_AssetID
        {
            get => ReadUInt(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }
    }
}