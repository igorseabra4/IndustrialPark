using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using IndustrialPark.Models;

namespace IndustrialPark
{
    public class AssetCAM : ObjectAsset, IRenderableAsset, IClickableAsset
    {
        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        protected override int EventStartOffset
        {
            get => 0x88;
        }

        public AssetCAM(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup()
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));

            CreateTransformMatrix();

            if (!ArchiveEditorFunctions.renderableAssetSetTrans.Contains(this))
                ArchiveEditorFunctions.renderableAssetSetTrans.Add(this);
        }
                
        public void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender)
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
            if (dontRender) return;

            renderer.DrawCube(world, isSelected);
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
            return Vector3.Distance(cameraPosition, _position);
        }

        private Vector3 _position;
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x8, _position.X);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0xC, _position.Y);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x10, _position.Z);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardX
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardY
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardZ
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpX
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpY
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpZ
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float2C
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [TypeConverter(typeof(HexIntTypeConverter))]
        public int UnknownValue30
        {
            get => ReadInt(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(HexIntTypeConverter))]
        public int UnknownValue34
        {
            get => ReadInt(0x34);
            set => Write(0x34, value);
        }
        public int UnknownValue38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }
        public int UnknownValue3C
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }
        public int UnknownValue40
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }
        public short UnknownShort44
        {
            get => ReadShort(0x44);
            set => Write(0x44, value);
        }
        public short UnknownShort46
        {
            get => ReadShort(0x46);
            set => Write(0x46, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CameraSpeed
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Float4C
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
        public int UnknownValue50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat54
        {
            get => ReadFloat(0x54);
            set => Write(0x54, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat58
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5C
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat60
        {
            get => ReadFloat(0x60);
            set => Write(0x60, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat64
        {
            get => ReadFloat(0x64);
            set => Write(0x64, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat68
        {
            get => ReadFloat(0x68);
            set => Write(0x68, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6C
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat70
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat74
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags1
        {
            get => ReadByte(0x78);
            set => Write(0x78, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags2
        {
            get => ReadByte(0x79);
            set => Write(0x79, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags3
        {
            get => ReadByte(0x7A);
            set => Write(0x7A, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags4
        {
            get => ReadByte(0x7B);
            set => Write(0x7B, value);
        }
        [TypeConverter(typeof(HexIntTypeConverter))]
        public int UnknownValue7C
        {
            get => ReadInt(0x7C);
            set => Write(0x7C, value);
        }
        [TypeConverter(typeof(HexIntTypeConverter))]
        public int UnknownValue80
        {
            get => ReadInt(0x80);
            set => Write(0x80, value);
        }
        [TypeConverter(typeof(HexIntTypeConverter))]
        public int UnknownValue84
        {
            get => ReadInt(0x84);
            set => Write(0x84, value);
        }
    }
}