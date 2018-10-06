using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSFX : ObjectAsset, IRenderableAsset, IClickableAsset
    {
        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        protected override int EventStartOffset { get => 0x30; }

        public AssetSFX(Section_AHDR AHDR) : base(AHDR) { }

        public void Setup()
        {
            _position = new Vector3(ReadFloat(0x1C), ReadFloat(0x20), ReadFloat(0x24));
            _radius = ReadFloat(0x28);
            _radius2 = ReadFloat(0x2C);

            CreateTransformMatrix();

            if (!ArchiveEditorFunctions.renderableAssetSetTrans.Contains(this))
                ArchiveEditorFunctions.renderableAssetSetTrans.Add(this);
        }

        public void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);
            world2 = Matrix.Scaling(_radius2 * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public BoundingSphere boundingSphere;

        protected void CreateBoundingBox()
        {
            boundingSphere = new BoundingSphere(_position, _radius);
            boundingBox = BoundingBox.FromSphere(boundingSphere);
        }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender)
                return null;

            if (ray.Intersects(ref boundingSphere))
                return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices);
            return null;
        }

        private float? TriangleIntersection(Ray r, List<Triangle> triangles, List<Vector3> vertices)
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
            if (dontRender) return;

            renderer.DrawSphere(world, isSelected, renderer.sfxColor);

            if (isSelected)
                renderer.DrawSphere(world2, false, renderer.sfxColor);
        }

        public virtual Vector3 GetGizmoCenter()
        {
            return boundingBox.Center;
        }

        public virtual float GetGizmoRadius()
        {
            return Math.Max(Math.Max(boundingBox.Size.X, boundingBox.Size.Y), boundingBox.Size.Z) * 0.9f;
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position) - _radius;
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag08
        {
            get => ReadByte(0x8);
            set => Write(0x8, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag09
        {
            get => ReadByte(0x9);
            set => Write(0x9, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag0A
        {
            get => ReadByte(0xA);
            set => Write(0xA, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag0B
        {
            get => ReadByte(0xB);
            set => Write(0xA, value);
        }

        public float UnknownFloat0C
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }

        public AssetID SoundAssetID
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }

        public float UnknownFloat14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        public int UnknownInt18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        private Vector3 _position;
        public float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x1C, _position.X);
                CreateTransformMatrix();
            }
        }

        public float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0x20, _position.Y);
                CreateTransformMatrix();
            }
        }

        public float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x24, _position.Z);
                CreateTransformMatrix();
            }
        }

        private float _radius;
        public float RadiusMin
        {
            get => _radius;
            set
            {
                _radius = value;
                Write(0x28, _radius);
                CreateTransformMatrix();
            }
        }

        private float _radius2;
        public float RadiusMax
        {
            get => _radius2;
            set
            {
                _radius2 = value;
                Write(0x2C, _radius2);
                CreateTransformMatrix();
            }
        }
    }
}