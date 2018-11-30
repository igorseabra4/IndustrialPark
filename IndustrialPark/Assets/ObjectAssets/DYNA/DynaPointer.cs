using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaPointer : DynaBase
    {
        public override string Note => "Version is always 1";
        
        public DynaPointer() : base()
        {
        }

        public DynaPointer(IEnumerable<byte> enumerable) : base(enumerable)
        {
            _position.X = Switch(BitConverter.ToSingle(Data, 0));
            _position.Y = Switch(BitConverter.ToSingle(Data, 4));
            _position.Z = Switch(BitConverter.ToSingle(Data, 8));
            _yaw = Switch(BitConverter.ToSingle(Data, 12));
            _pitch = Switch(BitConverter.ToSingle(Data, 16));
            _roll = Switch(BitConverter.ToSingle(Data, 20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(_position.X)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Y)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Z)));
            list.AddRange(BitConverter.GetBytes(Switch(_yaw)));
            list.AddRange(BitConverter.GetBytes(Switch(_pitch)));
            list.AddRange(BitConverter.GetBytes(Switch(_roll)));
            return list.ToArray();
        }

        private Vector3 _position;
        private float _yaw;
        private float _pitch;
        private float _roll;

        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => _position.X;
            set
            {
                _position.X = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float RotationYaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float RotationPitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float RotationRoll
        {
            get => _roll;
            set
            {
                _roll = value;
                CreateTransformMatrix();
            }
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) * Matrix.Translation(_position);

            Vector3[] vertices = new Vector3[SharpRenderer.pyramidVertices.Count];
            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);
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
                return TriangleIntersection(ray, distance, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices);
            return null;
        }

        private float? TriangleIntersection(Ray r, float initialDistance, List<Models.Triangle> triangles, List<Vector3> vertices)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (Models.Triangle t in triangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(vertices[t.vertex1], world);
                Vector3 v2 = (Vector3)Vector3.Transform(vertices[t.vertex2], world);
                Vector3 v3 = (Vector3)Vector3.Transform(vertices[t.vertex3], world);

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
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