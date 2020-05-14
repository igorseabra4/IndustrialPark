using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectVent : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x48;

        public DynaGObjectVent(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID) => VentType_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(VentType_AssetID, ref result);
        }
        
        public AssetID VentType_AssetID 
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => ReadFloat(0x04);
            set
            {
                Write(0x04, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => ReadFloat(0x08);
            set
            {
                Write(0x08, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => ReadFloat(0x0C);
            set
            {
                Write(0x0C, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Yaw
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x10));
            set
            {
                Write(0x10, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Pitch
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x14));
            set
            {
                Write(0x14, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Roll
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x18));
            set
            {
                Write(0x18, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat28
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float VentDistance
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat30
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float LaunchSpeed
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        public float UnknownInt38
        {
            get => ReadInt(0x38);
            set => Write(0x38, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3C
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat40
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat44
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.RotationYawPitchRoll(
                MathUtil.DegreesToRadians(Yaw),
                MathUtil.DegreesToRadians(Pitch),
                MathUtil.DegreesToRadians(Roll)) * Matrix.Translation(PositionX, PositionY, PositionZ);

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