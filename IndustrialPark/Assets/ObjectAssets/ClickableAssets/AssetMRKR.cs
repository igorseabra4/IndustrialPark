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

        public AssetMRKR(Section_AHDR AHDR) : base(AHDR)
        {
            _position = new Vector3(ReadFloat(0x0), ReadFloat(0x4), ReadFloat(0x8));

            CreateTransformMatrix();

            if (!ArchiveEditorFunctions.renderableAssetSetTrans.Contains(this))
                ArchiveEditorFunctions.renderableAssetSetTrans.Add(this);
        }

        public void CreateTransformMatrix()
        {
            world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f));
            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            Vector3[] vertices = new Vector3[SharpRenderer.pyramidVertices.Count];

            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender || isInvisible)
                return null;

            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices);
            return null;
        }
        
        private float? TriangleIntersection(Ray r, float initialDistance, List<Triangle> triangles, List<Vector3> vertices)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (Triangle t in triangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(vertices[t.vertex1], world);
                Vector3 v2 = (Vector3)Vector3.Transform(vertices[t.vertex2], world);
                Vector3 v3 = (Vector3)Vector3.Transform(vertices[t.vertex3], world);

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }
            }

            if (hasIntersected)
                return smallestDistance;
            else return null;
        }

        public void Draw(SharpRenderer renderer)
        {
            if (dontRender || isInvisible) return;

            renderer.DrawPyramid(world, isSelected, 1f);
        }

        public BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position);
        }

        private Vector3 _position;

        [Category("Marker")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get { return _position.X; }
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
            get { return _position.Y; }
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
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x8, _position.Z);
                CreateTransformMatrix();
            }
        }
    }
}