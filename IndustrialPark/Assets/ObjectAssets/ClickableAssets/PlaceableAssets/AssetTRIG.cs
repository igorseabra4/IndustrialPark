using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

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

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x94 + Offset;

        public BoundingSphere boundingSphere;

        public AssetTRIG(Section_AHDR AHDR) : base(AHDR)
        {
            _shape = ReadByte(9);

            _trigPos0 = new Vector3(ReadFloat(0x54 + Offset), ReadFloat(0x58 + Offset), ReadFloat(0x5C + Offset));
            _trigPos1 = new Vector3(ReadFloat(0x60 + Offset), ReadFloat(0x64 + Offset), ReadFloat(0x68 + Offset));
            _trigPos2 = new Vector3(ReadFloat(0x6C + Offset), ReadFloat(0x70 + Offset), ReadFloat(0x74 + Offset));
            _trigPos3 = new Vector3(ReadFloat(0x78 + Offset), ReadFloat(0x7C + Offset), ReadFloat(0x80 + Offset));

            CreateTransformMatrix();

            if (!ArchiveEditorFunctions.renderableAssetSetTrans.Contains(this))
                ArchiveEditorFunctions.renderableAssetSetTrans.Add(this);
        }

        public override void CreateTransformMatrix()
        {
            if (Shape == TriggerShape.Box)
            {
                Vector3 boxSize = _trigPos1 - _trigPos0;
                Vector3 midPos = (_trigPos0 + _trigPos1) / 2f;

                world = Matrix.Scaling(boxSize) *
                    Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) *
                    Matrix.Translation(midPos);
            }
            else
            {
                world = Matrix.Scaling(Position1X_Radius * 2f) *
                    Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) *
                    Matrix.Translation(_trigPos0);
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
            if (dontRender || isInvisible) return;

            if (Shape == TriggerShape.Box)
                renderer.DrawCube(world, isSelected, 1f);
            else
                renderer.DrawSphere(world, isSelected, renderer.trigColor);
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (dontRender || isInvisible)
                return null;

            if (Shape == TriggerShape.Box)
                if (ray.Intersects(ref boundingBox))
                    return TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices);
            if (Shape == TriggerShape.Sphere)
                if (ray.Intersects(ref boundingSphere))
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

        public BoundingSphere GetBoundingSphere()
        {
            return boundingSphere;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            if (Shape == TriggerShape.Sphere)
                return Vector3.Distance(cameraPosition, boundingSphere.Center) - boundingSphere.Radius;
            return Vector3.Distance(cameraPosition, boundingBox.Center) - boundingSphere.Radius;
        }

        private byte _shape;
        [Category("Trigger")]
        public TriggerShape Shape
        {
            get => (TriggerShape)_shape;
            set
            {
                _shape = (byte)value;
                Write(0x09, _shape);
            }
        }

        [Category("Trigger"), ReadOnly(true)]
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

        [Category("Trigger"), ReadOnly(true)]
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

        [Category("Trigger"), ReadOnly(true)]
        public override float PositionZ
        {
            get => _position.Z;
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
                CreateTransformMatrix();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleX
        {
            get
            {
                if (Shape == TriggerShape.Sphere)
                    return Position1X_Radius;
                return base.ScaleX;
            }
            set
            {
                if (Shape == TriggerShape.Sphere)
                    Position1X_Radius = value;
                base.ScaleX = value;
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleY
        {
            get
            {
                if (Shape == TriggerShape.Sphere)
                    return Position1X_Radius;
                return base.ScaleY;
            }
            set
            {
                if (Shape == TriggerShape.Sphere)
                    Position1X_Radius = value;
                base.ScaleY = value;
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleZ
        {
            get
            {
                if (Shape == TriggerShape.Sphere)
                    return Position1X_Radius;
                return base.ScaleZ;
            }
            set
            {
                if (Shape == TriggerShape.Sphere)
                    Position1X_Radius = value;
                base.ScaleZ = value;
            }
        }

        private Vector3 _trigPos0;
        private Vector3 _trigPos1;
        private Vector3 _trigPos2;
        private Vector3 _trigPos3;

        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
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
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
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
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
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
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
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
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
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
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
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
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position2X
        {
            get => _trigPos2.X;
            set
            {
                _trigPos2.X = value;
                Write(0x6C + Offset, _trigPos2.X);
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position2Y
        {
            get => _trigPos2.Y;
            set
            {
                _trigPos2.Y = value;
                Write(0x70 + Offset, _trigPos2.Y);
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position2Z
        {
            get => _trigPos2.Z;
            set
            {
                _trigPos2.Z = value;
                Write(0x74 + Offset, _trigPos2.Z);
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position3X
        {
            get => _trigPos3.X;
            set
            {
                _trigPos3.X = value;
                Write(0x78 + Offset, _trigPos3.X);
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position3Y
        {
            get => _trigPos3.Y;
            set
            {
                _trigPos3.Y = value;
                Write(0x7C + Offset, _trigPos3.Y);
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position3Z
        {
            get => _trigPos3.Z;
            set
            {
                _trigPos3.Z = value;
                Write(0x80 + Offset, _trigPos3.Z);
            }
        }

        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float DirectionX
        {
            get => ReadFloat(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float DirectionY
        {
            get => ReadFloat(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float DirectionZ
        {
            get => ReadFloat(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        [Category("Trigger")]
        public int TriggerFlags
        {
            get => ReadInt(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }
    }
}