using HipHopFile;
using SharpDX;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetMINF : Asset, IAssetWithModel
    {
        public AssetMINF(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup()
        {
            try
            {
                _modelAssetID = ReadUInt(0x14);
            }
            catch
            {
                _modelAssetID = 0;
            }
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public override bool HasReference(uint assetID)
        {
            if (_modelAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color)
        {
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
            {
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * color : color);
            }
            else
            {
                renderer.DrawCube(world, isSelected |isSelected);
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
                throw new Exception("Error: MINF asset " + AHDR.ADBG.assetName + " could not find its RenderWareModelFile");
            }
        }

        public bool HasRenderWareModelFile()
        {
            return ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID) && ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null;
        }

        private uint _modelAssetID;
        [Category("Model Info")]
        public AssetID ModelAssetID
        {
            get { return _modelAssetID; }

            set
            {
                _modelAssetID = value;
                if (AHDR.data.Length >= 0x18)
                    Write(0x14, value);
            }
        }
    }
}