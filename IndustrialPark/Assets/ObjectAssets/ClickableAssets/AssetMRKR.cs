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
        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        public AssetMRKR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _position = new Vector3(ReadFloat(0x0), ReadFloat(0x4), ReadFloat(0x8));
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssets.Add(this);
        }

        public override void Verify(ref List<string> result)
        {
            if (Data.Length != 12)
                result.Add("MRKR data length is invalid");
        }

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

        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
        }
        
        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistanceFrom(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        private Vector3 _position;

        [Category("Marker")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get => _position.X;
            set
            {
                _position.X = value;
                Write(0x0, _position.X);
                CreateTransformMatrix();
            }
        }

        [Category("Marker")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                Write(0x4, _position.Y);
                CreateTransformMatrix();
            }
        }

        [Category("Marker")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                Write(0x8, _position.Z);
                CreateTransformMatrix();
            }
        }
    }
}