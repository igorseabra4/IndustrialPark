using HipHopFile;
using SharpDX;
using System;

namespace IndustrialPark
{
    public class AssetMINF : Asset, IAssetWithModel
    {
        private uint _modelAssetID;
        public AssetID ModelAssetID
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

        public void Setup()
        {
            _modelAssetID = ReadUInt(0x14);
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public void Draw(SharpRenderer renderer, Matrix world, bool isSelected)
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

        public RenderWareModelFile GetRenderWareModelFile()
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