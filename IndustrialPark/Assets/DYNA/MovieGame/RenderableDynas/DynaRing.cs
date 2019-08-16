using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaRing : DynaBase
    {
        public override string Note => "Version is always 2";

        public DynaRing(Platform platform) : base(platform)
        {
            DriverPLAT_AssetID = 0;
        }

        public DynaRing(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            _position = new Vector3(
                Switch(BitConverter.ToSingle(Data, 0x00)),
                Switch(BitConverter.ToSingle(Data, 0x04)),
                Switch(BitConverter.ToSingle(Data, 0x08)));
            _yaw = Switch(BitConverter.ToSingle(Data, 0x0C));
            _pitch = Switch(BitConverter.ToSingle(Data, 0x10));
            _roll = Switch(BitConverter.ToSingle(Data, 0x14));
            UnknownInt1 = Switch(BitConverter.ToInt32(Data, 0x18));
            UnknownInt2 = Switch(BitConverter.ToInt32(Data, 0x1C));
            UnknownInt3 = Switch(BitConverter.ToInt32(Data, 0x20));
            _scale = new Vector3(
                Switch(BitConverter.ToSingle(Data, 0x24)),
                Switch(BitConverter.ToSingle(Data, 0x28)),
                Switch(BitConverter.ToSingle(Data, 0x2C)));
            UnknownShadowFlag = Switch(BitConverter.ToInt32(Data, 0x30));
            CollisionRadius = Switch(BitConverter.ToSingle(Data, 0x34));
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x38));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x3C));
            NormalTimer = Switch(BitConverter.ToSingle(Data, 0x40));
            RedTimer = Switch(BitConverter.ToSingle(Data, 0x44));
            DriverPLAT_AssetID = Switch(BitConverter.ToUInt32(Data, 0x48));

            CreateTransformMatrix();
        }
        
        public override bool HasReference(uint assetID)
        {
            return DriverPLAT_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(DriverPLAT_AssetID, ref result);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(_yaw)));
            list.AddRange(BitConverter.GetBytes(Switch(_pitch)));
            list.AddRange(BitConverter.GetBytes(Switch(_roll)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt3)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleX)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleY)));
            list.AddRange(BitConverter.GetBytes(Switch(ScaleZ)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownShadowFlag)));
            list.AddRange(BitConverter.GetBytes(Switch(CollisionRadius)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(NormalTimer)));
            list.AddRange(BitConverter.GetBytes(Switch(RedTimer)));
            list.AddRange(BitConverter.GetBytes(Switch(DriverPLAT_AssetID)));

            return list.ToArray();
        }

        public override bool IsRenderableClickable => true;
        
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

        public override BoundingSphere GetObjectCenter()
        {
            BoundingSphere boundingSphere = new BoundingSphere(_position, boundingBox.Size.Length());
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }

        private Vector3 _position;

        [Category("Ring")]
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

        [Category("Ring")]
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

        [Category("Ring")]
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

        private float _yaw;
        private float _pitch;
        private float _roll;

        [Category("Ring")]
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

        [Category("Ring")]
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

        [Category("Ring")]
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

        [Category("Ring")]
        public int UnknownInt1 { get; set; }
        [Category("Ring")]
        public int UnknownInt2 { get; set; }
        [Category("Ring")]
        public int UnknownInt3 { get; set; }

        private Vector3 _scale;

        [Category("Ring")]
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

        [Category("Ring")]
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

        [Category("Ring")]
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

        [Category("Ring")]
        public int UnknownShadowFlag { get; set; }

        [Category("Ring"), TypeConverter(typeof(FloatTypeConverter))]
        public float CollisionRadius { get; set; }

        [Category("Ring"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }

        [Category("Ring"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }

        [Category("Ring"), TypeConverter(typeof(FloatTypeConverter))]
        public float NormalTimer { get; set; }

        [Category("Ring"), TypeConverter(typeof(FloatTypeConverter))]
        public float RedTimer { get; set; }

        [Category("Ring")]
        public AssetID DriverPLAT_AssetID { get; set; }
    }
}