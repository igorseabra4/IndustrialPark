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

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            if (AHDR.containedFile.Length >= 0x18)
                modelAssetID = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x14));
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public override void Draw(SharpRenderer renderer, Matrix world, bool isSelected)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[modelAssetID].Draw(renderer, world, isSelected);
            }
            else
            {
                renderer.DrawCube(world, isSelected);
                //throw new Exception("Error: MINF asset " + AHDR.ADBG.assetName + " could not find its MODL asset ID");
            }
        }

        public override RenderWareModelFile GetRenderWareModelFile()
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID))
            {
                return ArchiveEditorFunctions.renderingDictionary[modelAssetID].GetRenderWareModelFile();
            }
            else
            {
                throw new Exception("Error: MINF asset " + AHDR.ADBG.assetName + " could not find its MODL asset ID");
            }
        }

        public bool HasRenderWareModelFile()
        {
            return ArchiveEditorFunctions.renderingDictionary.ContainsKey(modelAssetID);
        }
    }
}