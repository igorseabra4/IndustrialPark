using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public enum TriggerShape
    {
        Box = 0,
        Sphere = 1
    }

    public class AssetTRIG : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        protected override int EventStartOffset
        {
            get => 0x94 + Offset;
        }

        public BoundingSphere boundingSphere;

        public AssetTRIG(Section_AHDR AHDR) : base(AHDR) { }

        public override void Setup(SharpRenderer renderer)
        {
            _shape = ReadByte(9);

            _trigPos0 = new Vector3(ReadFloat(0x54 + Offset), ReadFloat(0x58 + Offset), ReadFloat(0x5C + Offset));
            _trigPos1 = new Vector3(ReadFloat(0x60 + Offset), ReadFloat(0x64 + Offset), ReadFloat(0x68 + Offset));
            _trigPos2 = new Vector3(ReadFloat(0x6C + Offset), ReadFloat(0x70 + Offset), ReadFloat(0x74 + Offset));
            _trigPos3 = new Vector3(ReadFloat(0x78 + Offset), ReadFloat(0x7C + Offset), ReadFloat(0x80 + Offset));

            base.Setup(renderer);
        }

        public override void CreateTransformMatrix()
        {
            if (Shape == TriggerShape.Box)
            {
                Vector3 boxSize = _trigPos1 - _trigPos0;
                Vector3 midPos = (_trigPos0 + _trigPos1) / 2f;

                world = Matrix.Scaling(boxSize) *
                    Matrix.RotationY(_rotation.Y) *
                    Matrix.RotationX(_rotation.X) *
                    Matrix.RotationZ(_rotation.Z) *
                    Matrix.Translation(midPos);
            }
            else
            {
                world = Matrix.Scaling(Position1X_Radius * 2f) * Matrix.Translation(_trigPos0);
            }

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            if (Shape == TriggerShape.Box)
            {
                boundingBox = new BoundingBox(-Vector3.One, Vector3.One);
                boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
                boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
                boundingSphere = BoundingSphere.FromBox(boundingBox);
            }
            else
            {
                boundingSphere = new BoundingSphere(_trigPos0, Position1X_Radius);
                boundingBox = BoundingBox.FromSphere(boundingSphere);
            }
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;

            if (Shape == TriggerShape.Box)
                renderer.DrawCube(world, isSelected, 1f);
            else
                renderer.DrawSphere(world, isSelected);
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (dontRender)
                return null;

            if (Shape == TriggerShape.Box)
                if (ray.Intersects(ref boundingBox, out float distance))
                    return TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices);
            if (Shape == TriggerShape.Sphere)
                if (ray.Intersects(ref boundingSphere, out float distance2))
                    return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices);
            return null;
        }

        private float? TriangleIntersection(Ray r, List<Triangle> triangles, List<Vector3> vertices)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (Triangle t in triangles)
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

        protected override float? TriangleIntersection(Ray r, float distance)
        {
            return null;
        }

        public override Vector3 GetGizmoCenter()
        {
            if (Shape == TriggerShape.Box)
                return boundingBox.Center;
            else
                return boundingSphere.Center;
        }

        public override float GetGizmoRadius()
        {
            if (Shape == TriggerShape.Box)
                return base.GetGizmoRadius();
            else
                return boundingSphere.Radius;
        }

        private byte _shape;
        public TriggerShape Shape
        {
            get => (TriggerShape)_shape;
            set
            {
                _shape = (byte)value;
                Write(0x09, _shape);
            }
        }

        public override float PositionX
        {
            get => _position.X;
            set
            {
                if (Shape == TriggerShape.Sphere)
                {
                    Position0X = value;
                }
                else
                {
                    Position0X += value - _position.X;
                    Position1X_Radius += value - _position.X;
                }

                _position.X = value;
                Write(0x20 + Offset, _position.X);
                CreateTransformMatrix();
            }
        }

        public override float PositionY
        {
            get => _position.Y;
            set
            {
                if (Shape == TriggerShape.Sphere)
                {
                    Position0Y = value;
                }
                else
                {
                    Position0Y += value - _position.Y;
                    Position1Y += value - _position.Y;
                }

                _position.Y = value;
                Write(0x24 + Offset, _position.Y);
                CreateTransformMatrix();
            }
        }

        public override float PositionZ
        {
            get => _trigPos0.Z;
            set
            {
                if (Shape == TriggerShape.Sphere)
                {
                    Position0Z = value;
                }
                else
                {
                    Position0Z += value - _position.Z;
                    Position1Z += value - _position.Z;
                }

                _position.Z = value;
                Write(0x28 + Offset, _position.Z);
                Position0Z = value;
            }
        }

        private Vector3 _trigPos0;
        private Vector3 _trigPos1;
        private Vector3 _trigPos2;
        private Vector3 _trigPos3;

        public float Position0X
        {
            get => _trigPos0.X;
            set
            {
                _trigPos0.X = value;
                Write(0x54 + Offset, _trigPos0.X);
                CreateTransformMatrix();
            }
        }
        public float Position0Y
        {
            get => _trigPos0.Y;
            set
            {
                _trigPos0.Y = value;
                Write(0x58 + Offset, _trigPos0.Y);
                CreateTransformMatrix();
            }
        }
        public float Position0Z
        {
            get => _trigPos0.Z;
            set
            {
                _trigPos0.Z = value;
                Write(0x5C + Offset, _trigPos0.Z);
                CreateTransformMatrix();
            }
        }
        public float Position1X_Radius
        {
            get => _trigPos1.X;
            set
            {
                _trigPos1.X = value;
                Write(0x60 + Offset, _trigPos1.X);
                CreateTransformMatrix();
            }
        }
        public float Position1Y
        {
            get => _trigPos1.Y;
            set
            {
                _trigPos1.Y = value;
                Write(0x64, _trigPos1.Y);
                CreateTransformMatrix();
            }
        }
        public float Position1Z
        {
            get => _trigPos1.Z;
            set
            {
                _trigPos1.Z = value;
                Write(0x68 + Offset, _trigPos1.Z);
                CreateTransformMatrix();
            }
        }
        public float Position2X
        {
            get => _trigPos2.X;
            set
            {
                _trigPos2.X = value;
                Write(0x6C + Offset, _trigPos2.X);
            }
        }
        public float Position2Y
        {
            get => _trigPos2.Y;
            set
            {
                _trigPos2.Y = value;
                Write(0x70 + Offset, _trigPos2.Y);
            }
        }
        public float Position2Z
        {
            get => _trigPos2.Z;
            set
            {
                _trigPos2.Z = value;
                Write(0x74 + Offset, _trigPos2.Z);
            }
        }
        public float Position3X
        {
            get => _trigPos3.X;
            set
            {
                _trigPos3.X = value;
                Write(0x78 + Offset, _trigPos3.X);
            }
        }
        public float Position3Y
        {
            get => _trigPos3.Y;
            set
            {
                _trigPos3.Y = value;
                Write(0x7C + Offset, _trigPos3.Y);
            }
        }
        public float Position3Z
        {
            get => _trigPos3.Z;
            set
            {
                _trigPos3.Z = value;
                Write(0x80 + Offset, _trigPos3.Z);
            }
        }

        public float UnknownTriggerFloat1
        {
            get => ReadFloat(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        public float UnknownTriggerFloat2
        {
            get => ReadFloat(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        public float UnknownTriggerFloat3
        {
            get => ReadFloat(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        public float UnknownTriggerFloat4
        {
            get => ReadFloat(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }
    }
}