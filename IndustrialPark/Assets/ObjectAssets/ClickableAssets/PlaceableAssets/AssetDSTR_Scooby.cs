using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetDSTR_Scooby : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x70;

        public AssetDSTR_Scooby(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID) => SpawnItemAssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(SpawnItemAssetID, ref result);
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
    }
}