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

        protected override int EventStartOffset => 0x88;

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
            Vector3[] vertices = new Vector3[SharpRenderer.cubeVertices.Count];

            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i] * 0.5f, world);

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

            renderer.DrawCube(world, isSelected);
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
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
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

        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardX
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardY
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedForwardZ
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpX
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpY
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedUpZ
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedLeftX
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedLeftY
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalizedLeftZ
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float ViewOffsetX
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public int ViewOffsetY
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public int ViewOffsetZ
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }
        [Category("Camera")]
        public short OffsetStartFrames
        {
            get => ReadShort(0x44);
            set => Write(0x44, value);
        }
        [Category("Camera")]
        public short OffsetEndFrames
        {
            get => ReadShort(0x46);
            set => Write(0x46, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float FieldOfView
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float TransitionTime
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
        [Category("Camera")]
        public int TransitionType
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }
        [Category("Camera")]
        public uint CamFlags
        {
            get => ReadUInt(0x54);
            set => Write(0x54, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float FadeUp
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float FadeDown
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat60
        {
            get => ReadFloat(0x60);
            set => Write(0x60, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat64
        {
            get => ReadFloat(0x64);
            set => Write(0x64, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat68
        {
            get => ReadFloat(0x68);
            set => Write(0x68, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6C
        {
            get => ReadFloat(0x6C);
            set => Write(0x6C, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat70
        {
            get => ReadFloat(0x70);
            set => Write(0x70, value);
        }
        [Category("Camera"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat74
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }
        [Category("Camera"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags1
        {
            get => ReadByte(0x78);
            set => Write(0x78, value);
        }
        [Category("Camera"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags2
        {
            get => ReadByte(0x79);
            set => Write(0x79, value);
        }
        [Category("Camera"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags3
        {
            get => ReadByte(0x7A);
            set => Write(0x7A, value);
        }
        [Category("Camera"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flags4
        {
            get => ReadByte(0x7B);
            set => Write(0x7B, value);
        }
        [Category("Camera")]
        public AssetID MarkerAssetId
        {
            get => ReadUInt(0x7C);
            set => Write(0x7C, value);
        }
        [Category("Camera"), TypeConverter(typeof(HexIntTypeConverter))]
        public int UnknownInt80
        {
            get => ReadInt(0x80);
            set => Write(0x80, value);
        }
        [Category("Camera")]
        public byte CamType
        {
            get => ReadByte(0x84);
            set => Write(0x84, value);
        }
        [Category("Camera")]
        public byte Padding85
        {
            get => ReadByte(0x85);
            set => Write(0x85, value);
        }
        [Category("Camera")]
        public byte Padding86
        {
            get => ReadByte(0x86);
            set => Write(0x86, value);
        }
        [Category("Camera")]
        public byte Padding87
        {
            get => ReadByte(0x87);
            set => Write(0x87, value);
        }

        public void SetPosition(Vector3 position)
        {
            PositionX = position.X;
            PositionY = position.Y;
            PositionZ = position.Z;
        }

        public void SetNormalizedForward(Vector3 forward)
        {
            NormalizedForwardX = forward.X;
            NormalizedForwardY = forward.Y;
            NormalizedForwardZ = forward.Z;
        }

        public void SetNormalizedUp(Vector3 up)
        {
            NormalizedUpX = up.X;
            NormalizedUpY = up.Y;
            NormalizedUpZ = up.Z;
        }

        public void SetNormalizedLeft(Vector3 right)
        {
            NormalizedLeftX = -right.X;
            NormalizedLeftY = -right.Y;
            NormalizedLeftZ = -right.Z;
        }
    }
}