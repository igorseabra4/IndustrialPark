using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBOUL : EntityAsset
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
            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * Color : Color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        private const string categoryName = "Boulder";

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Gravity
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Mass
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float BounceFactor
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Friction
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxLinearVelocity
        {
            get => ReadFloat(0x68 + Offset + Offset2);
            set => Write(0x68 + Offset + Offset2, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxAngularVelocity
        {
            get => ReadFloat(0x6C + Offset + Offset2);
            set => Write(0x6C + Offset + Offset2, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Stickiness
        {
            get => ReadFloat(0x70 + Offset + Offset2);
            set => Write(0x70 + Offset + Offset2, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float BounceDamp
        {
            get => ReadFloat(0x74 + Offset + Offset2);
            set => Write(0x74 + Offset + Offset2, value);
        }

        [Category(categoryName)]
        public DynamicTypeDescriptor BoulderFlags => IntFlagsDescriptor(0x78 + Offset + Offset2,
            "Can Hit Walls",
            "Damage Player",
            "Something related to destructibles",
            "Damage NPCs",
            null,
            "Die on OOB surfaces",
            null,
            null,
            "Die on player attack",
            "Die after kill timer");

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float KillTimer
        {
            get => ReadFloat(0x7C + Offset + Offset2);
            set => Write(0x7C + Offset + Offset2, value);
        }

        [Category(categoryName)]
        public int Hitpoints
        {
            get => ReadInt(0x80 + Offset + Offset2);
            set => Write(0x80 + Offset, value);
        }

        [Category(categoryName)]
        public AssetID Sound_AssetID
        {
            get => ReadUInt(0x84 + Offset + Offset2);
            set => Write(0x84 + Offset + Offset2, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MinSoundVel
        {
            get => ReadFloat(0x8C + Offset + 2 * Offset2);
            set => Write(0x8C + Offset + 2 * Offset2, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxSoundVel
        {
            get => ReadFloat(0x90 + Offset + 2 * Offset2);
            set => Write(0x90 + Offset + 2 * Offset2, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float InnerRadius
        {
            get => ReadFloat(0x94 + Offset + 2 * Offset2);
            set => Write(0x94 + Offset + 2 * Offset2, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float OuterRadius
        {
            get => ReadFloat(0x98 + Offset + 2 * Offset2);
            set => Write(0x98 + Offset + 2 * Offset2, value);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.BFBB)
            {
                dt.RemoveProperty("Volume");
                dt.RemoveProperty("StartFriction");
            }
            base.SetDynamicProperties(dt);
        }
    }

}