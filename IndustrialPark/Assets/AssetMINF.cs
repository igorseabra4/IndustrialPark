using HipHopFile;
using SharpDX;
using System;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class AssetMINF : AssetWithModel
    {
        public int modelAssetID;

        public AssetMINF(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(bool defaultMode = true)
        {
            if (AHDR.containedFile.Length >= 0x18)
                modelAssetID = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x14));
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public override void Draw(Matrix world)
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