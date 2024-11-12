using HipHopFile;
using SharpDX;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetPLYR : EntityAsset
    {
        [Category("Player References")]
        public AssetID LightKit { get; set; }

        public AssetPLYR(string assetName, Vector3 position, Game game) : base(assetName, AssetType.Player, BaseAssetType.Player, position)
        {
            BaseFlags = 0x0D;
            SolidityFlags.FlagValueByte = 0;

            ColorAlpha = 0;
            ColorAlphaSpeed = 0;
            if (game == Game.BFBB)
                Model = 0x003FE4D5;
            else if (game == Game.Scooby)
                Model = 0x96E7F1D5;
        }

        public AssetPLYR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            if (game != Game.Scooby)
                using (var reader = new EndianBinaryReader(AHDR.data, endianness))
                {
                    reader.BaseStream.Position = reader.BaseStream.Length - 4;
                    LightKit = reader.ReadUInt32();
                }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            SerializeLinks(writer);
            if (game != Game.Scooby)
                writer.Write(LightKit);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("LightKit");

            base.SetDynamicProperties(dt);
        }

        protected override void CreateBoundingBox()
        {
            var model = GetFromRenderingDictionary(_model);
            CreateBoundingBox(model == null ? SharpRenderer.pyramidVertices : model.vertexListG);
        }

        public override void Draw(SharpRenderer renderer)
        {
            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (renderingDictionary.ContainsKey(_model))
                renderingDictionary[_model].Draw(renderer, LocalWorld(), Color, UvAnimOffset, isSelected);
            else
                renderer.DrawPyramid(LocalWorld(), isSelected, 1f);
        }
    }
}