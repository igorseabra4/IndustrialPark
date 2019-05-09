﻿using HipHopFile;
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
            if (LightKit_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(LightKit_AssetID, ref result);

            if (EventStartOffset + AmountOfEvents * Link.sizeOfStruct + 4 < Data.Length)
                result.Add("Additional data found at the end of asset data");
            if (EventStartOffset + AmountOfEvents * Link.sizeOfStruct + 4 > Data.Length)
                result.Add("Asset expects mode data than present");
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

            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * Color : Color);
            else
                renderer.DrawPyramid(LocalWorld(), isSelected, 1f);
        }

        [Category("Player References")]
        public AssetID LightKit_AssetID
        {
            get => Functions.currentGame == Game.Scooby ? 0 : ReadUInt(EventStartOffset + AmountOfEvents * Link.sizeOfStruct);
            set { if (Functions.currentGame != Game.Scooby) Write(EventStartOffset + AmountOfEvents * Link.sizeOfStruct, value); }
        }
    }
}