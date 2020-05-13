using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectFlameEmitter : DynaBase
    {
        public string Note => "Version is always 4";

        public override int StructSize => 0x38;

        public DynaGObjectFlameEmitter(AssetDYNA asset) : base(asset) { }
        
        public int UnknownInt_00
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => ReadFloat(0x04);
            set { Write(0x04, value); CreateTransformMatrix(); }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => ReadFloat(0x08);
            set { Write(0x08, value); CreateTransformMatrix(); }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => ReadFloat(0x0C);
            set { Write(0x0C, value); CreateTransformMatrix(); }
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
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
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_2C
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_30
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_34
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
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