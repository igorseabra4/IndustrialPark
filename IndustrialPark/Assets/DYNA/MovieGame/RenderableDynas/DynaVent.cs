using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaVent : DynaBase
    {
        public override string Note => "Version is always 1";
        
        public DynaVent() : base()
        {
            VentType_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            return VentType_AssetID == assetID;
        }
        
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(VentType_AssetID, ref result);
        }

        public DynaVent(IEnumerable<byte> enumerable) : base(enumerable)
        {
            VentType_AssetID = Switch(BitConverter.ToUInt32(Data, 0x00));
            _position.X = Switch(BitConverter.ToSingle(Data, 0x04));
            _position.Y = Switch(BitConverter.ToSingle(Data, 0x08));
            _position.Z = Switch(BitConverter.ToSingle(Data, 0x0C));
            _yaw = Switch(BitConverter.ToSingle(Data, 0x10));
            _pitch = Switch(BitConverter.ToSingle(Data, 0x14));
            _roll = Switch(BitConverter.ToSingle(Data, 0x18));
            UnknownFloat1C = Switch(BitConverter.ToSingle(Data, 0x1C));
            UnknownFloat20 = Switch(BitConverter.ToSingle(Data, 0x20));
            UnknownFloat24 = Switch(BitConverter.ToSingle(Data, 0x24));
            UnknownFloat28 = Switch(BitConverter.ToSingle(Data, 0x28));
            UnknownFloat2C = Switch(BitConverter.ToSingle(Data, 0x2C));
            UnknownFloat30 = Switch(BitConverter.ToSingle(Data, 0x30));
            UnknownFloat34 = Switch(BitConverter.ToSingle(Data, 0x34));
            UnknownInt38 = Switch(BitConverter.ToInt32(Data, 0x38));
            UnknownFloat3C = Switch(BitConverter.ToSingle(Data, 0x3C));
            UnknownFloat40 = Switch(BitConverter.ToSingle(Data, 0x40));
            UnknownFloat44 = Switch(BitConverter.ToSingle(Data, 0x44));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(VentType_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.X)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Y)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Z)));
            list.AddRange(BitConverter.GetBytes(Switch(_yaw)));
            list.AddRange(BitConverter.GetBytes(Switch(_pitch)));
            list.AddRange(BitConverter.GetBytes(Switch(_roll)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1C)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat20)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat24)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat28)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2C)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat30)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat34)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt38)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3C)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat40)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat44)));
            return list.ToArray();
        }

        [Category("Vent")]
        public AssetID VentType_AssetID { get; set; }

        private Vector3 _position;
        private float _yaw;
        private float _pitch;
        private float _roll;

        [Category("Vent"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => _position.X;
            set
            {
                _position.X = value;
                CreateTransformMatrix();
            }
        }
        [Category("Vent"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                CreateTransformMatrix();
            }
        }
        [Category("Vent"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                CreateTransformMatrix();
            }
        }
        [Category("Vent"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Yaw
        {
            get => MathUtil.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathUtil.DegreesToRadians(value);
                CreateTransformMatrix();
            }
        }
        [Category("Vent"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Pitch
        {
            get => MathUtil.RadiansToDegrees(_pitch);
            set
            {
                _pitch = MathUtil.DegreesToRadians(value);
                CreateTransformMatrix();
            }
        }
        [Category("Vent"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Roll
        {
            get => MathUtil.RadiansToDegrees(_roll);
            set
            {
                _roll = MathUtil.DegreesToRadians(value);
                CreateTransformMatrix();
            }
        }

        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1C { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat20 { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat24 { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat28 { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2C { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat30 { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat34 { get; set; }
        [Category("Vent")]
        public float UnknownInt38 { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3C { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat40 { get; set; }
        [Category("Vent"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat44 { get; set; }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) * Matrix.Translation(_position);

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
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        private float? TriangleIntersection(Ray r, float initialDistance)
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

        public override BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }
    }
}