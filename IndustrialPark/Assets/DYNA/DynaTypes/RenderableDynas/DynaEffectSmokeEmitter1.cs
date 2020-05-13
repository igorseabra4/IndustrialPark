using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectSmokeEmitter : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x70;

        public DynaEffectSmokeEmitter(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (Texture_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Texture_AssetID, ref result);
        }
        
        public int UnknownInt_00
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        public int UnknownInt_04
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => ReadFloat(0x08);
            set { Write(0x08, value); CreateTransformMatrix(); }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => ReadFloat(0x0C);
            set { Write(0x0C, value); CreateTransformMatrix(); }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => ReadFloat(0x10);
            set { Write(0x10, value); CreateTransformMatrix(); }
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_28
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        public AssetID Texture_AssetID
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }
        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_30
        {
            get => ReadUShort(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_32
        {
            get => ReadUShort(0x32);
            set => Write(0x32, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_34
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_38
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_3C
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_40
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_44
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_48
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_4C
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_50
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_54
        {
            get => ReadFloat(0x54);
            set => Write(0x54, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_58
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_5C
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_60
        {
            get => ReadFloat(0x60);
            set => Write(0x60, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_64
        {
            get => ReadFloat(0x64);
            set => Write(0x64, value);
        }
        public short UnknownShort_68
        {
            get => ReadShort(0x68);
            set => Write(0x68, value);
        }
        public short UnknownShort_6A
        {
            get => ReadShort(0x6A);
            set => Write(0x6A, value);
        }
        public short UnknownShort_6C
        {
            get => ReadShort(0x6C);
            set => Write(0x6C, value);
        }
        public short UnknownShort_6E
        {
            get => ReadShort(0x6E);
            set => Write(0x6E, value);
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(PositionX, PositionY, PositionZ);

            vertices = new Vector3[SharpRenderer.cubeVertices.Count];
            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);

            triangles = new RenderWareFile.Triangle[SharpRenderer.cubeTriangles.Count];
            for (int i = 0; i < SharpRenderer.cubeTriangles.Count; i++)
            {
                triangles[i] = new RenderWareFile.Triangle((ushort)SharpRenderer.cubeTriangles[i].materialIndex,
                    (ushort)SharpRenderer.cubeTriangles[i].vertex1, (ushort)SharpRenderer.cubeTriangles[i].vertex2, (ushort)SharpRenderer.cubeTriangles[i].vertex3);
            }
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            renderer.DrawCube(world, isSelected);
        }

        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray);
            return null;
        }

        private float? TriangleIntersection(Ray r)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (RenderWareFile.Triangle t in triangles)
            {
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
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
    }
}