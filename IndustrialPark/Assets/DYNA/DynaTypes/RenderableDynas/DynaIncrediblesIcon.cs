using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaIncrediblesIcon : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x20;

        public DynaIncrediblesIcon(AssetDYNA asset) : base(asset) { }
        
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => ReadFloat(0x00);
            set { Write(0x00, value); CreateTransformMatrix(); }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => ReadFloat(0x04);
            set { Write(0x04, value); CreateTransformMatrix(); }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => ReadFloat(0x08);
            set { Write(0x08, value); CreateTransformMatrix(); }
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_0C
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
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
        public int UnknownInt_1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
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
            float? smallestDistance = null;

            if (ray.Intersects(ref boundingBox))
                foreach (RenderWareFile.Triangle t in triangles)
                    if (ray.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                        if (smallestDistance == null || distance < smallestDistance)
                            smallestDistance = distance;

            return smallestDistance;
        }
    }
}