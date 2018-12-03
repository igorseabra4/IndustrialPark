using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPLYR : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x54 + Offset;

        public AssetPLYR(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (LightKitID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        protected override void CreateBoundingBox()
        {
            List<Vector3> vertexList = new List<Vector3>();
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID) &&
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(ArchiveEditorFunctions.renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
            }
            else
            {
                CreateBoundingBox(SharpRenderer.pyramidVertices);
            }
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (DontRender || isInvisible) return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * _color : _color);
            else
                renderer.DrawPyramid(world, isSelected, 1f);
        }

        [Category("Player References")]
        public AssetID LightKitID
        {
            get => ReadUInt(EventStartOffset + AmountOfEvents * AssetEvent.sizeOfStruct);
            set => Write(EventStartOffset + AmountOfEvents * AssetEvent.sizeOfStruct, value);
        }
    }
}