using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetPLYR : EntityAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x54 + Offset;

        public AssetPLYR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) => LightKit_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(LightKit_AssetID, ref result);

            if (EventStartOffset + LinkCount * Link.sizeOfStruct + 4 < Data.Length)
                result.Add("Additional data found at the end of asset data");
            if (EventStartOffset + LinkCount * Link.sizeOfStruct + 4 > Data.Length)
                result.Add("Asset expects mode data than present");
        }

        protected override void CreateBoundingBox()
        {
            if (renderingDictionary.ContainsKey(_modelAssetID) &&
                renderingDictionary[_modelAssetID].HasRenderWareModelFile() &&
                renderingDictionary[_modelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(renderingDictionary[_modelAssetID].GetRenderWareModelFile().vertexListG);
            }
            else
            {
                CreateBoundingBox(SharpRenderer.pyramidVertices);
            }
        }

        public override void Draw(SharpRenderer renderer)
        {
            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (renderingDictionary.ContainsKey(_modelAssetID))
                renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * Color : Color, UvAnimOffset);
            else
                renderer.DrawPyramid(LocalWorld(), isSelected, 1f);
        }

        [Category("Player References")]
        public AssetID LightKit_AssetID
        {
            get => game == Game.Scooby ? 0 : ReadUInt(EventStartOffset + LinkCount * Link.sizeOfStruct);
            set { if (game != Game.Scooby) Write(EventStartOffset + LinkCount * Link.sizeOfStruct, value); }
        }
    }
}