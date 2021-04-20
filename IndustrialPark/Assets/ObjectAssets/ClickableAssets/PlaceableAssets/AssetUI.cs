using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetUI : EntityAsset
    {
        private const string categoryName = "User Interface";

        [Category("Entity References")]
        public AssetID Sound_AssetID
        {
            get => Animation_AssetID;
            set => Animation_AssetID = value;
        }

        [Category(categoryName)]
        public FlagBitmask UIFlags { get; set; } = IntFlagsDescriptor(
            "Focus sets Select",
            null,
            "Screen Space",
            null,
            "Visible Allowed",
            "Invisible Allowed");

        private short _width;
        [Category(categoryName)]
        public short Width
        {
            get => _width;
            set
            {
                _width = value;
                CreateTransformMatrix();
            }
        }

        private short _height;
        [Category(categoryName)]
        public short Height
        {
            get => _height;
            set
            {
                _height = value;
                CreateTransformMatrix();
            }
        }

        [Category(categoryName)]
        public AssetID TextureAssetID { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopLeftX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopLeftY { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopRightX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordTopRightY { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomRightX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomRightY { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomLeftX { get; set; }
        [Category(categoryName)]
        public AssetSingle TextCoordBottomLeftY { get; set; }

        public AssetUI(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityHeaderEndPosition;

            UIFlags.FlagValueInt = reader.ReadUInt32();
            _width = reader.ReadInt16();
            Height = reader.ReadInt16();

            TextureAssetID = reader.ReadUInt32();
            TextCoordTopLeftX = reader.ReadSingle();
            TextCoordTopLeftY = reader.ReadSingle();
            TextCoordTopRightX = reader.ReadSingle();
            TextCoordTopRightY = reader.ReadSingle();
            TextCoordBottomRightX = reader.ReadSingle();
            TextCoordBottomRightY = reader.ReadSingle();
            TextCoordBottomLeftX = reader.ReadSingle();
            TextCoordBottomLeftY = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));
            writer.Write(SerializeUIData(platform));
            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        protected byte[] SerializeUIData(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(UIFlags.FlagValueInt);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(TextureAssetID);
            writer.Write(TextCoordTopLeftX);
            writer.Write(TextCoordTopLeftY);
            writer.Write(TextCoordTopRightX);
            writer.Write(TextCoordTopRightY);
            writer.Write(TextCoordBottomRightX);
            writer.Write(TextCoordBottomRightY);
            writer.Write(TextCoordBottomLeftX);
            writer.Write(TextCoordBottomLeftY);

            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) => TextureAssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(TextureAssetID, ref result);
        }

        public override void CreateTransformMatrix()
        {
            if (this is AssetUI && TextureAssetID == 0)
            {
                world = Matrix.Scaling(_scale) * Matrix.Scaling(Width, Height, 1f)
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(PositionX, -PositionY, -PositionZ);
            }
            else
            {
                world = Matrix.Scaling(Width, Height, 1f)
                    * Matrix.Translation(PositionX, -PositionY, -PositionZ);
            }

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            if (this is AssetUI && TextureAssetID == 0)
                base.CreateBoundingBox();
            else
            {
                var model = GetFromRenderingDictionary(_modelAssetID);
                CreateBoundingBox(model != null ? model.vertexListG : SharpRenderer.planeVertices);
            }
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (this is AssetUI && TextureAssetID == 0)
                base.Draw(renderer);
            else
                renderer.DrawPlane(world, isSelected, TextureAssetID, UvAnimOffset);
        }

        public override bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (DontRender)
                return false;
            if (isInvisible)
                return false;
            if (renderer.isDrawingUI)
                return true;
            
            return renderer.frustum.Intersects(ref boundingBox);
        }

        [Browsable(false)]
        public override bool SpecialBlendMode => true;

        protected override float? TriangleIntersection(Ray r)
        {
            if (TextureAssetID == 0)
                return base.TriangleIntersection(r);

            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (var t in SharpRenderer.planeTriangles)
            {
                var v1 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex1], world);
                var v2 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex2], world);
                var v3 = (Vector3)Vector3.Transform(SharpRenderer.planeVertices[t.vertex3], world);

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    hasIntersected = true;
                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }
            }

            if (hasIntersected)
                return smallestDistance;
            return null;
        }
    }
}