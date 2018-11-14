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
            _rotation.X = Switch(BitConverter.ToSingle(Data, 12));
            _rotation.Y = Switch(BitConverter.ToSingle(Data, 16));
            _rotation.Z = Switch(BitConverter.ToSingle(Data, 20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(_position.X)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Y)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Z)));
            list.AddRange(BitConverter.GetBytes(Switch(_rotation.X)));
            list.AddRange(BitConverter.GetBytes(Switch(_rotation.Y)));
            list.AddRange(BitConverter.GetBytes(Switch(_rotation.Z)));
            return list.ToArray();
        }

        private Vector3 _position;
        private Vector3 _rotation;

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
        public float RotationX
        {
            get => _rotation.X;
            set
            {
                _rotation.X = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float RotationY
        {
            get => _rotation.Y;
            set
            {
                _rotation.Y = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public float RotationZ
        {
            get => _rotation.Z;
            set
            {
                _rotation.Z = value;
                CreateTransformMatrix();
            }
        }

        public override bool IsRenderableClickable { get => true; }

        private Matrix world;
        private BoundingBox boundingBox;

        public override void CreateTransformMatrix()
        {
            world =
                Matrix.RotationY(MathUtil.DegreesToRadians(_rotation.Y)) *
                Matrix.RotationX(MathUtil.DegreesToRadians(_rotation.X)) *
                Matrix.RotationZ(MathUtil.DegreesToRadians(_rotation.Z)) *
                Matrix.Translation(_position);

            CreateBoundingBox();
        }

        private void CreateBoundingBox()
        {
            boundingBox = BoundingBox.FromPoints(SharpRenderer.pyramidVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
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

        public override Vector3 GetGizmoCenter()
        {
            return boundingBox.Center;
        }

        public override float GetGizmoRadius()
        {
            return Math.Max(Math.Max(boundingBox.Size.X, boundingBox.Size.Y), boundingBox.Size.Z) * 0.9f;
        }
    }
}