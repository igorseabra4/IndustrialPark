using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public abstract class DynaEnemySB : DynaBase
    {
        public override bool IsRenderableClickable => true;

        public DynaEnemySB(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (Surface_AssetID == assetID)
                return true;
            if (Model_AssetID == assetID)
                return true;
            if (Unknown44 == assetID)
                return true;
            if (Unknown4C == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Surface_AssetID, ref result);
            Asset.Verify(Model_AssetID, ref result);
            Asset.Verify(Unknown44, ref result);
            Asset.Verify(Unknown4C, ref result);
        }

        [Browsable(false)]
        public Matrix world { get; private set; }
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
            if (renderingDictionary.ContainsKey(Model_AssetID) &&
                renderingDictionary[Model_AssetID].HasRenderWareModelFile() &&
                renderingDictionary[Model_AssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(renderingDictionary[Model_AssetID].GetRenderWareModelFile().vertexListG);
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

            if (renderingDictionary.ContainsKey(Model_AssetID))
            {
                if (renderingDictionary[Model_AssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        triangles = renderingDictionary[Model_AssetID].GetRenderWareModelFile().triangleList.ToArray();
                    else
                        triangles = null;
                }
                else
                    triangles = renderingDictionary[Model_AssetID].GetRenderWareModelFile().triangleList.ToArray();
            }
            else
                triangles = null;
        }

        public override bool ShouldDraw(SharpRenderer renderer)
        {
            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistance(renderer.Camera.Position) <
                    (AssetLODT.MaxDistances.ContainsKey(Model_AssetID) ?
                    AssetLODT.MaxDistances[Model_AssetID] : SharpRenderer.DefaultLODTDistance))
                    return renderer.frustum.Intersects(ref boundingBox);

                return false;
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            Vector4 Color = new Vector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            if (renderingDictionary.ContainsKey(Model_AssetID))
                renderingDictionary[Model_AssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * Color : Color, Vector3.Zero);
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

            float? smallestDistance = null;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                    if (smallestDistance == null || distance < smallestDistance)
                        smallestDistance = distance;
            
            return smallestDistance;
        }
        
        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }
        
        public int PaddingInt00
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte PaddingByte04
        {
            get => ReadByte(0x04);
            set => Write(0x04, value);
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte PaddingByte05
        {
            get => ReadByte(0x05);
            set => Write(0x05, value);
        }

        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort Flags06
        {
            get => ReadUShort(0x06);
            set => Write(0x06, value);
        }

        [Browsable(false)]
        public byte VisibilityFlag
        {
            get => ReadByte(0x08);
            set => Write(0x08, value);
        }
        public DynamicTypeDescriptor VisibilityFlags => ByteFlagsDescriptor(0x8, "Visible", "Stackable");

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte TypeFlag
        {
            get => ReadByte(0x9);
            set => Write(0x9, value);
        }

        public DynamicTypeDescriptor UnknownFlag0A => ByteFlagsDescriptor(0xA);

        [Browsable(false)]
        public byte SolidityFlag
        {
            get => ReadByte(0x0B);
            set => Write(0x0B, value);
        }
        public DynamicTypeDescriptor SolidityFlags => ByteFlagsDescriptor(0xB, "Unused", "Precise Collision");
        
        public AssetID Surface_AssetID
        {
            get => ReadUInt(0x0C);
            set => Write(0x0C, value);
        }
        
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX
        {
            get => ReadFloat(0x1C);
            set
            {
                Write(0x1C, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY
        {
            get => ReadFloat(0x20);
            set
            {
                Write(0x20, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ
        {
            get => ReadFloat(0x24);
            set
            {
                Write(0x24, value);
                CreateTransformMatrix();
            }
        }
                
        public override BoundingSphere GetObjectCenter()
        {
            BoundingSphere boundingSphere = new BoundingSphere(new Vector3(PositionX, PositionY, PositionZ), boundingBox.Size.Length());
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }
        
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Yaw
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x10));
            set
            {
                Write(0x10, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Pitch
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x14));
            set
            {
                Write(0x14, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Roll
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x18));
            set
            {
                Write(0x18, MathUtil.DegreesToRadians(value));
                CreateTransformMatrix();
            }
        }


        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleX
        {
            get => ReadFloat(0x28);
            set
            {
                Write(0x28, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleY
        {
            get => ReadFloat(0x2C);
            set
            {
                Write(0x2C, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleZ
        {
            get => ReadFloat(0x30);
            set
            {
                Write(0x30, value);
                CreateTransformMatrix();
            }
        }

        [DisplayName("Red (0 - 1)")]
        public float ColorRed
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        [DisplayName("Green (0 - 1)")]
        public float ColorGreen
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }
        [DisplayName("Blue (0 - 1)")]
        public float ColorBlue
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }
        [DisplayName("Alpha (0 - 1)")]
        public float ColorAlpha
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }

        [DisplayName("Color - (A,) R, G, B")]
        public System.Drawing.Color Color_ARGB
        {
            get => System.Drawing.Color.FromArgb(BitConverter.ToInt32(new byte[] { (byte)(ColorBlue * 255), (byte)(ColorGreen * 255), (byte)(ColorRed * 255), (byte)(ColorAlpha * 255) }, 0));
            set
            {
                ColorRed = value.R / 255f;
                ColorGreen = value.G / 255f;
                ColorBlue = value.B / 255f;
                ColorAlpha = value.A / 255f;
            }
        }

        public AssetID Unknown44
        {
            get => ReadUInt(0x44);
            set => Write(0x44, value);
        }

        public AssetID Model_AssetID
        {
            get => ReadUInt(0x48);
            set => Write(0x48, value);
        }

        public AssetID Unknown4C
        {
            get => ReadUInt(0x4C);
            set => Write(0x4C, value);
        }
    }
}