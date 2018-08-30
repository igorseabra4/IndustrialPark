using HipHopFile;
using SharpDX;
using System;

namespace IndustrialPark
{
    public class AssetMINF : AssetWithModel
    {
        private uint _modelAssetID;
        public uint ModelAssetID
        {
            get { return _modelAssetID; }

            set
            {
                _modelAssetID = value;
                if (AHDR.containedFile.Length >= 0x18)
                    Write(0x14, value);
            }
        }

        public AssetMINF(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            _modelAssetID = ReadUInt(0x14);
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public override void Draw(SharpRenderer renderer, Matrix world, bool isSelected)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected);
            }
            else
            {
                renderer.DrawCube(world, isSelected);
                //throw new Exception("Error: MINF asset " + AHDR.ADBG.assetName + " could not find its MODL asset ID");
            }
        }

        public override RenderWareModelFile GetRenderWareModelFile()
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                return ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile();
            }
            else
            {
                throw new Exception("Error: MINF asset " + AHDR.ADBG.assetName + " could not find its MODL asset ID");
            }
        }

        public bool HasRenderWareModelFile()
        {
            return ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID);
        }
    }
}