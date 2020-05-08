using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaRing : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x4C;

        public DynaRing(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            return DriverPLAT_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(DriverPLAT_AssetID, ref result);
        }
        
        public override bool IsRenderableClickable => true;
        
        private Matrix world;
        private BoundingBox boundingBox;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Scaling(ScaleX, ScaleY, ScaleZ)
                * Matrix.RotationYawPitchRoll(
                    MathUtil.DegreesToRadians(Yaw), 
                    MathUtil.DegreesToRadians(Pitch),
                    MathUtil.DegreesToRadians(Roll))
                * Matrix.Translation(PositionX, PositionY, PositionZ);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            if (renderingDictionary.ContainsKey(DynaRingControl.RingModelAssetID) &&
                renderingDictionary[DynaRingControl.RingModelAssetID].HasRenderWareModelFile() &&
                renderingDictionary[DynaRingControl.RingModelAssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(renderingDictionary[DynaRingControl.RingModelAssetID].GetRenderWareModelFile().vertexListG);
            }
            else
            {
                CreateBoundingBox(SharpRenderer.cubeVertices, 0.5f);
            }
        }

        protected Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        protected void CreateBoundingBox(List<Vector3> vertexList, float multiplier = 1f)
        {
            vertices = new Vector3[vertexList.Count];

            for (int i = 0; i < vertexList.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexList[i] * multiplier, world);

            boundingBox = BoundingBox.FromPoints(vertices);

            if (renderingDictionary.ContainsKey(DynaRingControl.RingModelAssetID))
            {
                if (renderingDictionary[DynaRingControl.RingModelAssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        triangles = renderingDictionary[DynaRingControl.RingModelAssetID].GetRenderWareModelFile().triangleList.ToArray();
                    else
                        triangles = null;
                }
                else
                    triangles = renderingDictionary[DynaRingControl.RingModelAssetID].GetRenderWareModelFile().triangleList.ToArray();
            }
            else
                triangles = null;
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            if (renderingDictionary.ContainsKey(DynaRingControl.RingModelAssetID))
                renderingDictionary[DynaRingControl.RingModelAssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor : Vector4.One, Vector3.Zero);
            else
                renderer.DrawCube(world, isSelected);
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        protected float? TriangleIntersection(Ray r, float initialDistance)
        {
            if (triangles == null)
                return initialDistance;

            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }

            if (hasIntersected)
                return smallestDistance;
            return null;
        }

        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        public override BoundingSphere GetObjectCenter()
        {
            BoundingSphere boundingSphere = new BoundingSphere(new Vector3(PositionX, PositionY, PositionZ), boundingBox.Size.Length());
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX
        {
            get => ReadFloat(0x00);
            set
            {
                Write(0x00, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY
        {
            get => ReadFloat(0x04);
            set
            {
                Write(0x04, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ
        {
            get => ReadFloat(0x08);
            set
            {
                Write(0x08, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Yaw
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0xC));
            set
            {
                Write(0x0C, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Pitch
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x10));
            set
            {
                Write(0x10, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Roll
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x14));
            set
            {
                Write(0x14, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        public int UnknownInt1
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }
        public int UnknownInt2
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }
        public int UnknownInt3
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleX
        {
            get => ReadFloat(0x24);
            set
            {
                Write(0x24, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleY
        {
            get => ReadFloat(0x28);
            set
            {
                Write(0x28, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleZ
        {
            get => ReadFloat(0x2C);
            set
            {
                Write(0x2C, value);
                CreateTransformMatrix();
            }
        }

        public int UnknownShadowFlag
        {
            get => ReadInt(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionRadius
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float NormalTimer
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float RedTimer
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }    
        public AssetID DriverPLAT_AssetID
        {
            get => ReadUInt(0x48);
            set => Write(0x48, value);
        }
    }
}