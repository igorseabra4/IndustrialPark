using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpDX;

namespace IndustrialPark
{
    public abstract class DynaPlaceableBase : DynaBase
    {
        public override bool IsRenderableClickable => true;

        protected DynaPlaceableBase() : base()
        {
            Surface_AssetID = 0;
            Model_AssetID = 0;
        }

        protected DynaPlaceableBase(IEnumerable<byte> enumerable) : base(enumerable) { }

        private Matrix world;
        private BoundingBox boundingBox;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Scaling(ScaleX, ScaleY, ScaleZ)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(PositionX, PositionY, PositionZ);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            List<Vector3> vertexList = new List<Vector3>();
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(Model_AssetID) &&
                ArchiveEditorFunctions.renderingDictionary[Model_AssetID].HasRenderWareModelFile() &&
                ArchiveEditorFunctions.renderingDictionary[Model_AssetID].GetRenderWareModelFile() != null)
            {
                CreateBoundingBox(ArchiveEditorFunctions.renderingDictionary[Model_AssetID].GetRenderWareModelFile().vertexListG);
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

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(Model_AssetID))
            {
                if (ArchiveEditorFunctions.renderingDictionary[Model_AssetID] is AssetMINF MINF)
                {
                    if (MINF.HasRenderWareModelFile())
                        triangles = ArchiveEditorFunctions.renderingDictionary[Model_AssetID].GetRenderWareModelFile().triangleList.ToArray();
                    else
                        triangles = null;
                }
                else
                    triangles = ArchiveEditorFunctions.renderingDictionary[Model_AssetID].GetRenderWareModelFile().triangleList.ToArray();
            }
            else
                triangles = null;
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            Vector4 Color = new Vector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(Model_AssetID))
                ArchiveEditorFunctions.renderingDictionary[Model_AssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * Color : Color);
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
        
        public override BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        protected static uint Mask(uint bit)
        {
            return (uint)Math.Pow(2, bit);
        }

        protected static uint InvMask(uint bit)
        {
            return uint.MaxValue - Mask(bit);
        }

        public int Unknown00 { get; set; }
        public byte Unknown04 { get; set; }
        public byte Unknown05 { get; set; }
        public short Flags06 { get; set; }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte VisibilityFlag { get; set; }

        [Category("Placement Flags")]
        public bool Visible
        {
            get => (VisibilityFlag & Mask(0)) != 0;
            set => VisibilityFlag = (byte)(value ? (VisibilityFlag | Mask(0)) : (VisibilityFlag & InvMask(0)));
        }

        [Category("Placement Flags")]
        public bool UseGravity
        {
            get => (VisibilityFlag & Mask(1)) != 0;
            set => VisibilityFlag = (byte)(value ? (VisibilityFlag | Mask(1)) : (VisibilityFlag & InvMask(1)));
        }

        [Category("Placement Flags"), ReadOnly(true), TypeConverter(typeof(HexByteTypeConverter))]
        public byte TypeFlag { get; set; }
        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownFlag0A { get; set; }
        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SolidityFlag { get; set; }

        [Category("Placement Flags")]
        public bool PreciseCollision
        {
            get => (SolidityFlag & Mask(1)) != 0;
            set => SolidityFlag = (byte)(value ? (VisibilityFlag | Mask(1)) : (VisibilityFlag & InvMask(1)));
        }
        [Category("Placement References")]
        public AssetID Surface_AssetID { get; set; }

        public Vector3 _position;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX
        {
            get => _position.X;
            set
            {
                _position.X = value;
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                CreateTransformMatrix();
            }
        }
                
        public override BoundingSphere GetObjectCenter()
        {
            BoundingSphere boundingSphere = new BoundingSphere(_position, boundingBox.Size.Length());
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }
        
        protected float _yaw;
        protected float _pitch;
        protected float _roll;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Yaw
        {
            get => MathUtil.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathUtil.DegreesToRadians(value);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Pitch
        {
            get => MathUtil.RadiansToDegrees(_pitch);
            set
            {
                _pitch = MathUtil.DegreesToRadians(value);
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float Roll
        {
            get => MathUtil.RadiansToDegrees(_roll);
            set
            {
                _roll = MathUtil.DegreesToRadians(value);
                CreateTransformMatrix();
            }
        }

        protected Vector3 _scale;

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleX
        {
            get => _scale.X;
            set
            {
                _scale.X = value;
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleY
        {
            get => _scale.Y;
            set
            {
                _scale.Y = value;
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float ScaleZ
        {
            get => _scale.Z;
            set
            {
                _scale.Z = value;
                CreateTransformMatrix();
            }
        }

        [Category("Placement Color"), DisplayName("Red (0 - 1)")]
        public float ColorRed { get; set; }
        [Category("Placement Color"), DisplayName("Green (0 - 1)")]
        public float ColorGreen { get; set; }
        [Category("Placement Color"), DisplayName("Blue (0 - 1)")]
        public float ColorBlue { get; set; }
        [Category("Placement Color"), DisplayName("Alpha (0 - 1)")]
        public float ColorAlpha { get; set; }
        
        [Category("Placement Color"), DisplayName("Color - (A,) R, G, B")]
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

        [Category("Placement References")]
        public AssetID Model_AssetID { get; set; }
    }
}