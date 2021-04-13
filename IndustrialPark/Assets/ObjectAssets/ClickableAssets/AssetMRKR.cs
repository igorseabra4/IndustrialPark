using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetMRKR : Asset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Marker";

        private Vector3 _position;
        [Category(categoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }

        public AssetMRKR(Section_AHDR AHDR, Platform platform) : base(AHDR)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            return writer.ToArray();
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public void CreateTransformMatrix()
        {
            world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f));
            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            var vertices = new Vector3[SharpRenderer.pyramidVertices.Count];

            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (!ShouldDraw(renderer))
                return null;

            if (ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices, world);
            return null;
        }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;

            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistanceFrom(renderer.Camera.Position) < SharpRenderer.DefaultLODTDistance)
                    return renderer.frustum.Intersects(ref boundingBox);
                return false;
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer) => renderer.DrawPyramid(world, isSelected, 1f);

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position);
    }
}