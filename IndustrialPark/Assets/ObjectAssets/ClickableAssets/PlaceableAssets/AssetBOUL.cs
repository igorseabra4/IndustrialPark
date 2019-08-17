using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBOUL : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x9C + Offset + 2 * Offset2;

        public AssetBOUL(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) => Sound_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Sound_AssetID, ref result);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (!isSelected && (dontRender || isInvisible))
                return;

            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * Color : Color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Gravity
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Mass
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float BounceFactor
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Friction
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("BFBB Only")]
        public float StartFriction
        {
            get
            {
                if (game == Game.BFBB)
                    return ReadFloat(0x64 + Offset);
                return 0;
            }
            set
            {
                if (game == Game.BFBB)
                    Write(0x64 + Offset, value);
            }
        }

        private int Offset2 => game == Game.Incredibles ? -0x04 : 0x00;

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxLinearVelocity
        {
            get => ReadFloat(0x68 + Offset + Offset2);
            set => Write(0x68 + Offset + Offset2, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxAngularVelocity
        {
            get => ReadFloat(0x6C + Offset + Offset2);
            set => Write(0x6C + Offset + Offset2, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Stickiness
        {
            get => ReadFloat(0x70 + Offset + Offset2);
            set => Write(0x70 + Offset + Offset2, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float BounceDamp
        {
            get => ReadFloat(0x74 + Offset + Offset2);
            set => Write(0x74 + Offset + Offset2, value);
        }

        [Category("Boulder Flags")]
        public uint BoulderFlags
        {
            get => ReadUInt(0x78 + Offset + Offset2);
            set => Write(0x78 + Offset + Offset2, value);
        }

        [Category("Boulder Flags")]
        public bool CanHitWalls
        {
            get => (BoulderFlags & Mask(0)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(0)) : (BoulderFlags & InvMask(0));
        }

        [Category("Boulder Flags")]
        public bool DamagePlayer
        {
            get => (BoulderFlags & Mask(1)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(1)) : (BoulderFlags & InvMask(1));
        }

        [Category("Boulder Flags")]
        public bool SomethingRelatedToDestructibles
        {
            get => (BoulderFlags & Mask(2)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(2)) : (BoulderFlags & InvMask(2));
        }

        [Category("Boulder Flags")]
        public bool DamageNPCs
        {
            get => (BoulderFlags & Mask(3)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(3)) : (BoulderFlags & InvMask(3));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags4
        {
            get => (BoulderFlags & Mask(4)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(4)) : (BoulderFlags & InvMask(4));
        }

        [Category("Boulder Flags")]
        public bool DieOnOOBSurfaces
        {
            get => (BoulderFlags & Mask(5)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(5)) : (BoulderFlags & InvMask(5));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags6
        {
            get => (BoulderFlags & Mask(6)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(6)) : (BoulderFlags & InvMask(6));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags7
        {
            get => (BoulderFlags & Mask(7)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(7)) : (BoulderFlags & InvMask(7));
        }

        [Category("Boulder Flags")]
        public bool DieOnPlayerAttack
        {
            get => (BoulderFlags & Mask(8)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(8)) : (BoulderFlags & InvMask(8));
        }

        [Category("Boulder Flags")]
        public bool DieAfterKillTimer
        {
            get => (BoulderFlags & Mask(9)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(9)) : (BoulderFlags & InvMask(9));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags10
        {
            get => (BoulderFlags & Mask(10)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(10)) : (BoulderFlags & InvMask(10));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags11
        {
            get => (BoulderFlags & Mask(11)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(11)) : (BoulderFlags & InvMask(11));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags12
        {
            get => (BoulderFlags & Mask(12)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(12)) : (BoulderFlags & InvMask(12));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags13
        {
            get => (BoulderFlags & Mask(13)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(13)) : (BoulderFlags & InvMask(13));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags14
        {
            get => (BoulderFlags & Mask(14)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(14)) : (BoulderFlags & InvMask(14));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags15
        {
            get => (BoulderFlags & Mask(15)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(15)) : (BoulderFlags & InvMask(15));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags16
        {
            get => (BoulderFlags & Mask(16)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(16)) : (BoulderFlags & InvMask(16));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags17
        {
            get => (BoulderFlags & Mask(17)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(17)) : (BoulderFlags & InvMask(17));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags18
        {
            get => (BoulderFlags & Mask(18)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(18)) : (BoulderFlags & InvMask(18));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags19
        {
            get => (BoulderFlags & Mask(19)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(19)) : (BoulderFlags & InvMask(19));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags20
        {
            get => (BoulderFlags & Mask(20)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(20)) : (BoulderFlags & InvMask(20));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags21
        {
            get => (BoulderFlags & Mask(21)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(21)) : (BoulderFlags & InvMask(21));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags22
        {
            get => (BoulderFlags & Mask(22)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(22)) : (BoulderFlags & InvMask(22));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags23
        {
            get => (BoulderFlags & Mask(23)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(23)) : (BoulderFlags & InvMask(23));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags24
        {
            get => (BoulderFlags & Mask(24)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(24)) : (BoulderFlags & InvMask(24));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags25
        {
            get => (BoulderFlags & Mask(25)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(25)) : (BoulderFlags & InvMask(25));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags26
        {
            get => (BoulderFlags & Mask(26)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(26)) : (BoulderFlags & InvMask(26));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags27
        {
            get => (BoulderFlags & Mask(27)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(27)) : (BoulderFlags & InvMask(27));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags28
        {
            get => (BoulderFlags & Mask(28)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(28)) : (BoulderFlags & InvMask(28));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags29
        {
            get => (BoulderFlags & Mask(29)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(29)) : (BoulderFlags & InvMask(29));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags30
        {
            get => (BoulderFlags & Mask(30)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(30)) : (BoulderFlags & InvMask(30));
        }

        [Category("Boulder Flags")]
        public bool BoulderFlags31
        {
            get => (BoulderFlags & Mask(31)) != 0;
            set => BoulderFlags = value ? (BoulderFlags | Mask(31)) : (BoulderFlags & InvMask(31));
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float KillTimer
        {
            get => ReadFloat(0x7C + Offset + Offset2);
            set => Write(0x7C + Offset + Offset2, value);
        }

        [Category("Boulder")]
        public int Hitpoints
        {
            get => ReadInt(0x80 + Offset + Offset2);
            set => Write(0x80 + Offset, value);
        }

        [Category("Boulder")]
        public AssetID Sound_AssetID
        {
            get => ReadUInt(0x84 + Offset + Offset2);
            set => Write(0x84 + Offset + Offset2, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("BFBB Only")]
        public float Volume
        {
            get
            {
                if (game == Game.BFBB)
                    return ReadFloat(0x88 + Offset);
                return 0;
            }
            set
            {
                if (game == Game.BFBB)
                    Write(0x88 + Offset, value);
            }
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float MinSoundVel
        {
            get => ReadFloat(0x8C + Offset + 2 * Offset2);
            set => Write(0x8C + Offset + 2 * Offset2, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxSoundVel
        {
            get => ReadFloat(0x90 + Offset + 2 * Offset2);
            set => Write(0x90 + Offset + 2 * Offset2, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float InnerRadius
        {
            get => ReadFloat(0x94 + Offset + 2 * Offset2);
            set => Write(0x94 + Offset + 2 * Offset2, value);
        }

        [Category("Boulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float OuterRadius
        {
            get => ReadFloat(0x98 + Offset + 2 * Offset2);
            set => Write(0x98 + Offset + 2 * Offset2, value);
        }
    }
}