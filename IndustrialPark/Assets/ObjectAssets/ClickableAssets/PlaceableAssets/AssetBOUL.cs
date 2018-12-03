using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBOUL : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x9C + Offset;

        public AssetBOUL(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (SoundAssetID == assetID)
                return true;
            
            return base.HasReference(assetID);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (DontRender || isInvisible) return;

            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * Color : Color);
            else
                renderer.DrawCube(world, isSelected);
        }

        [Category("Boulder")]
        public float Gravity
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Boulder")]
        public float Mass
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Boulder")]
        public float BounceFactor
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Boulder")]
        public float Friction
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Boulder")]
        public float UnknownFloat64
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Boulder")]
        public float MaxLinearVelocity
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Boulder")]
        public float MaxAngularVelocity
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Boulder")]
        public float AngularFriction
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("Boulder")]
        public float MinBounceVelocity
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }

        [Category("Boulder")]
        public int Flags
        {
            get => ReadInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Boulder")]
        public float UnknownFloat7C
        {
            get => ReadFloat(0x7C);
            set => Write(0x7C, value);
        }

        [Category("Boulder")]
        public int UnknownInt80
        {
            get => ReadInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Boulder")]
        public AssetID SoundAssetID
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Boulder")]
        public float UnknownFloat88
        {
            get => ReadFloat(0x88);
            set => Write(0x88, value);
        }

        [Category("Boulder")]
        public float UnknownFloat8C
        {
            get => ReadFloat(0x8C);
            set => Write(0x8C, value);
        }

        [Category("Boulder")]
        public float UnknownFloat90
        {
            get => ReadFloat(0x90);
            set => Write(0x90, value);
        }

        [Category("Boulder")]
        public float UnknownFloat94
        {
            get => ReadFloat(0x94);
            set => Write(0x94, value);
        }

        [Category("Boulder")]
        public float UnknownFloat98
        {
            get => ReadFloat(0x98);
            set => Write(0x98, value);
        }
    }
}