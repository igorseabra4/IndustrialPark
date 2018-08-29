using HipHopFile;
using SharpDX;
using System;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class AssetMVPT : RenderableAsset
    {
        public AssetMVPT(Section_AHDR AHDR) : base(AHDR)
        {
        }

        int[] pairAssetIDs;

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            Position.X = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x8));
            Position.Y = Switch(BitConverter.ToSingle(AHDR.containedFile, 0xC));
            Position.Z = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x10));

            Scale.X = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x1C));
            Scale.Y = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x20));
            Scale.Z = Switch(BitConverter.ToSingle(AHDR.containedFile, 0x24));

            int amountOfPairs = Switch(BitConverter.ToInt16(AHDR.containedFile, 0x1A));

            pairAssetIDs = new int[amountOfPairs];
            for (int i = 0; i < amountOfPairs; i++)
                pairAssetIDs[i] = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x28 + 4 * i));

            world = Matrix.Translation(Position);

            boundingBox = BoundingBox.FromPoints(Program.MainForm.renderer.cubeVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[modelAssetID].Draw(renderer, world, isSelected);
            }
            else
            {
                renderer.DrawCube(world, isSelected);
            }
        }
    }
}