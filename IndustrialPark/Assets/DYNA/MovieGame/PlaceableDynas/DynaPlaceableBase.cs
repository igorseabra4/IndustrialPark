using System;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;
using SharpDX;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public abstract class DynaPlaceableBase : DynaBase
    {
        public override bool IsRenderableClickable => true;

        protected DynaPlaceableBase(Platform platform) : base(platform)
        {
            Surface_AssetID = 0;
            Model_AssetID = 0;
            Unknown44 = 0;
            Unknown4C = 0;
        }

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

        protected DynaPlaceableBase(IEnumerable<byte> enumerable, Platform platform) : base(enumerable, platform)
        {
            PaddingInt00 = Switch(BitConverter.ToInt32(Data, 0x0));
            PaddingByte04 = Data[0x04];
            PaddingByte05 = Data[0x05];
            Flags06 = Switch(BitConverter.ToInt16(Data, 0x06));
            VisibilityFlag = Data[0x08];
            TypeFlag = Data[0x08];
            UnknownFlag0A = Data[0x08];
            SolidityFlag = Data[0x08];
            Surface_AssetID = Switch(BitConverter.ToUInt32(Data, 0x0C));
            _yaw = Switch(BitConverter.ToSingle(Data, 0x10));
            _pitch = Switch(BitConverter.ToSingle(Data, 0x14));
            _roll = Switch(BitConverter.ToSingle(Data, 0x18));
            _position.X = Switch(BitConverter.ToSingle(Data, 0x1C));
            _position.Y = Switch(BitConverter.ToSingle(Data, 0x20));
            _position.Z = Switch(BitConverter.ToSingle(Data, 0x24));
            _scale.X = Switch(BitConverter.ToSingle(Data, 0x28));
            _scale.Y = Switch(BitConverter.ToSingle(Data, 0x2C));
            _scale.Z = Switch(BitConverter.ToSingle(Data, 0x30));
            ColorRed = Switch(BitConverter.ToSingle(Data, 0x34));
            ColorGreen = Switch(BitConverter.ToSingle(Data, 0x38));
            ColorBlue = Switch(BitConverter.ToSingle(Data, 0x3C));
            ColorAlpha = Switch(BitConverter.ToSingle(Data, 0x40));
            Unknown44 = Switch(BitConverter.ToUInt32(Data, 0x44));
            Model_AssetID = Switch(BitConverter.ToUInt32(Data, 0x48));
            Unknown4C = Switch(BitConverter.ToUInt32(Data, 0x4C));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(PaddingInt00)));
            list.Add(PaddingByte04);
            list.Add(PaddingByte05);
            list.AddRange(BitConverter.GetBytes(Switch(Flags06)));
            list.Add(VisibilityFlag);
            list.Add(TypeFlag);
            list.Add(UnknownFlag0A);
            list.Add(SolidityFlag);
            list.AddRange(BitConverter.GetBytes(Switch(Surface_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(_yaw)));
            list.AddRange(BitConverter.GetBytes(Switch(_pitch)));
            list.AddRange(BitConverter.GetBytes(Switch(_roll)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleX)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleY)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleZ)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorRed)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorGreen)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorBlue)));
            list.AddRange(BitConverter.GetBytes(Switch(ColorAlpha)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
            list.AddRange(BitConverter.GetBytes(Switch(Model_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
            return list.ToArray();
        }

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

        protected static uint Mask(uint bit)
        {
            return (uint)Math.Pow(2, bit);
        }

        protected static uint InvMask(uint bit)
        {
            return 0xFFFFFFFF ^ Mask(bit);
        }

        [Category("Placement Flags")]
        public int PaddingInt00 { get; set; }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte PaddingByte04 { get; set; }

        [Category("Placement Flags"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte PaddingByte05 { get; set; }

        [Category("Placement Flags"), TypeConverter(typeof(HexShortTypeConverter))]
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
            set => SolidityFlag = (byte)(value ? (SolidityFlag | Mask(1)) : (SolidityFlag & InvMask(1)));
        }
        [Category("Placement Flags")]
        public bool LedgeGrab
        {
            get => (SolidityFlag & Mask(7)) != 0;
            set => SolidityFlag = (byte)(value ? (SolidityFlag | Mask(7)) : (SolidityFlag & InvMask(7)));
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
                dynaSpecificPropertyChanged?.Invoke(this);
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
        public AssetID Unknown44 { get; set; }

        [Category("Placement References")]
        public AssetID Model_AssetID { get; set; }

        [Category("Placement References")]
        public AssetID Unknown4C { get; set; }
    }
}