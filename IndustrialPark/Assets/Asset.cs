using HipHopFile;
using SharpDX;
using System;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public abstract class Asset
    {
        public Section_AHDR AHDR;

        public Asset(Section_AHDR AHDR)
        {
            this.AHDR = AHDR;
        }

        public abstract void Setup(bool defaultMode);

        public override string ToString()
        {
            return AHDR.ADBG.assetName + " [" + AHDR.assetID.ToString("X8") + "]";
        }
    }

    public abstract class AssetWithModel : Asset
    {
        public AssetWithModel(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public abstract void Draw(Matrix worldViewProjection);
    }

    public class AssetGeneric : Asset
    {
        public AssetGeneric(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(bool defaultMode = true) { }
    }

    public class RenderableAsset : Asset
    {
        public RenderableAsset(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public Vector3 Position;
        public int RotationX;
        public int RotationY;
        public int RotationZ;
        public Vector3 Scale;

        public Matrix world;
        public int modelAssetID;

        public override void Setup(bool defaultMode = true)
        {
            if (defaultMode)
                modelAssetID = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x4C));

            RotationX = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x14));
            RotationY = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x18));
            RotationZ = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x1C));

            Position.X = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x20));
            Position.Y = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x24));
            Position.Z = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x28));

            Scale.X = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x2C));
            Scale.Y = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x30));
            Scale.Z = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x34));

            world = Matrix.Scaling(Scale)
            //* Matrix.CreateRotationY((float)(Switch(BitConverter.ToSingle(data, 0x14)) * Math.PI))
            //* Matrix.CreateRotationX((float)(Switch(BitConverter.ToSingle(data, 0x18)) * Math.PI))
            //* Matrix.CreateRotationZ((float)(Switch(BitConverter.ToSingle(data, 0x1C)) * Math.PI))
            * Matrix.Translation(Position);

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public void Draw()
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[modelAssetID].Draw(world);
            }
            else
            {
                SharpRenderer.DrawCube(world);
            }
        }
    }
}