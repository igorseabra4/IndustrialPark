using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetPLYR : EntityAsset
    {
        [Category("Player References")]
        public AssetID LightKit_AssetID { get; set; }

        public AssetPLYR(string assetName, Vector3 position, Game game) : base(assetName, AssetType.PLYR, BaseAssetType.Player, position)
        {
            BaseFlags = 0x0D;
            SolidityFlags.FlagValueByte = 0;

            ColorAlpha = 0;
            ColorAlphaSpeed = 0;
            if (game == Game.BFBB)
                Model_AssetID = 0x003FE4D5;
            else if (game == Game.Scooby)
                Model_AssetID = 0x96E7F1D5;
        }

        public AssetPLYR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            if (game != Game.Scooby)
                using (var reader = new EndianBinaryReader(AHDR.data, endianness))
                {
                    reader.BaseStream.Position = reader.BaseStream.Length - 4;
                    LightKit_AssetID = reader.ReadUInt32();
                }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(SerializeLinks(endianness));
                if (game != Game.Scooby)
                {
                    writer.Write(LightKit_AssetID);
                }
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) => LightKit_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(LightKit_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("LightKit_AssetID");

            base.SetDynamicProperties(dt);
        }

        protected override void CreateBoundingBox()
        {
            var model = GetFromRenderingDictionary(_modelAssetID);
            CreateBoundingBox(model == null ? SharpRenderer.pyramidVertices : model.vertexListG);
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
    }
}