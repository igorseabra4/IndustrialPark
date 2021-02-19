using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaPointer : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x18;

        public DynaPointer(AssetDYNA asset) : base(asset) { }

        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => ReadFloat(0x00);
            set
            {
                Write(0x00, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => ReadFloat(0x04);
            set
            {
                Write(0x04, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => ReadFloat(0x08);
            set
            {
                Write(0x08, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Yaw
        {
            get => ReadFloat(0x0C);
            set
            {
                Write(0x0C, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Pitch
        {
            get => ReadFloat(0x10);
            set
            {
                Write(0x10, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Roll
        {
            get => ReadFloat(0x14);
            set
            {
                Write(0x14, value);
                CreateTransformMatrix();
            }
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(Yaw), MathUtil.DegreesToRadians(Pitch), MathUtil.DegreesToRadians(Roll)) * Matrix.Translation(PositionX, PositionY, PositionZ);

            vertices = new Vector3[SharpRenderer.pyramidVertices.Count];
            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);

            triangles = new RenderWareFile.Triangle[SharpRenderer.pyramidTriangles.Count];
            for (int i = 0; i < SharpRenderer.pyramidTriangles.Count; i++)
            {
                triangles[i] = new RenderWareFile.Triangle((ushort)SharpRenderer.pyramidTriangles[i].materialIndex,
                    (ushort)SharpRenderer.pyramidTriangles[i].vertex1, (ushort)SharpRenderer.pyramidTriangles[i].vertex2, (ushort)SharpRenderer.pyramidTriangles[i].vertex3);
            }
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
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