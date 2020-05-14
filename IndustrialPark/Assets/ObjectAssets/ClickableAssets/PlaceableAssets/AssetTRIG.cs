using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum TriggerShape
    {
        Box = 0,
        Sphere = 1,
        Cylinder = 2
    }

    public class AssetTRIG : EntityAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x94 + Offset;

        public BoundingSphere boundingSphere;
        private Matrix pivotWorld;

        public AssetTRIG(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            _shape = ReadByte(9);

            _trigPos0 = new Vector3(ReadFloat(0x54 + Offset), ReadFloat(0x58 + Offset), ReadFloat(0x5C + Offset));
            _trigPos1 = new Vector3(ReadFloat(0x60 + Offset), ReadFloat(0x64 + Offset), ReadFloat(0x68 + Offset));
            _trigPos2 = new Vector3(ReadFloat(0x6C + Offset), ReadFloat(0x70 + Offset), ReadFloat(0x74 + Offset));
            _trigPos3 = new Vector3(ReadFloat(0x78 + Offset), ReadFloat(0x7C + Offset), ReadFloat(0x80 + Offset));

            FixPosition();
        }

        public override void CreateTransformMatrix()
        {
            if (Shape == TriggerShape.Box)
            {
                Vector3 boxSize = _trigPos1 - _trigPos0;
                Vector3 midPos = (_trigPos0 + _trigPos1) / 2f;

                world = Matrix.Scaling(boxSize) *
                    Matrix.Translation(midPos) *
                    Matrix.Translation(-_position) *
                    Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) *
                    Matrix.Translation(_position);
            }
            else if (Shape == TriggerShape.Sphere)
            {
                world = Matrix.Scaling(Radius * 2f) *
                    Matrix.Translation(_trigPos0) *
                    Matrix.Translation(-_position) *
                    Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) *
                    Matrix.Translation(_position);
            }
            else
            {
                world = Matrix.Scaling(Radius * 2f, Height * 2f, Radius * 2f) *
                    Matrix.Translation(_trigPos0) *
                    Matrix.Translation(-_position) *
                    Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) *
                    Matrix.Translation(_position);
            }

            pivotWorld = Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            if (Shape == TriggerShape.Sphere)
            {
                boundingSphere = new BoundingSphere(_trigPos0, Radius);
                boundingBox = BoundingBox.FromSphere(boundingSphere);
            }
            else
            {
                List<Vector3> verticesF = Shape == TriggerShape.Box ?
                    SharpRenderer.cubeVertices : SharpRenderer.cylinderVertices;

                vertices = new Vector3[verticesF.Count];

                for (int i = 0; i < verticesF.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(verticesF[i], world);

                boundingBox = BoundingBox.FromPoints(vertices);
            }
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (Shape == TriggerShape.Box)
                renderer.DrawCube(world, isSelected, 1f);
            else if (Shape == TriggerShape.Sphere)
                renderer.DrawSphere(world, isSelected, renderer.trigColor);
            else
                renderer.DrawCylinder(world, isSelected, renderer.trigColor);

            if (isSelected)
                renderer.DrawCube(pivotWorld, isSelected);
        }

        public override float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (!ShouldDraw(renderer))
                return null;

            if (Shape == TriggerShape.Box)
            {
                if (ray.Intersects(ref boundingBox))
                    return TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices, world);
            }
            else if (Shape == TriggerShape.Sphere)
            {
                if (ray.Intersects(ref boundingSphere))
                    return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices, world);
            }
            else
            {
                if (ray.Intersects(ref boundingBox))
                    return TriangleIntersection(ray, SharpRenderer.cylinderTriangles, SharpRenderer.cylinderVertices, world);
            }

            return null;
        }
        
        public override float GetDistanceFrom(Vector3 cameraPosition)
        {
            if (Shape == TriggerShape.Sphere)
                return Vector3.Distance(cameraPosition, boundingSphere.Center) - boundingSphere.Radius;

            return base.GetDistanceFrom(cameraPosition);
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
                
        public void FixPosition()
        {
            if (Shape == TriggerShape.Box)
            {
                if (_trigPos0.X > _trigPos1.X)
                {
                    float temp = _trigPos1.X;
                    _trigPos1.X = _trigPos0.X;
                    _trigPos0.X = temp;
                }
                if (_trigPos0.Y > _trigPos1.Y)
                {
                    float temp = _trigPos1.Y;
                    _trigPos1.Y = _trigPos0.Y;
                    _trigPos0.Y = temp;
                }
                if (_trigPos0.Z > _trigPos1.Z)
                {
                    float temp = _trigPos1.Z;
                    _trigPos1.Z = _trigPos0.Z;
                    _trigPos0.Z = temp;
                }
            }

            Write(0x54 + Offset, _trigPos0.X);
            Write(0x58 + Offset, _trigPos0.Y);
            Write(0x5C + Offset, _trigPos0.Z);
            Write(0x60 + Offset, _trigPos1.X);
            Write(0x64 + Offset, _trigPos1.Y);
            Write(0x68 + Offset, _trigPos1.Z);

            CreateTransformMatrix();
        }
        
        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleX
        {
            get
            {
                if (Shape == TriggerShape.Sphere || Shape == TriggerShape.Cylinder)
                    return Radius;
                return base.ScaleX;
            }
            set
            {
                if (Shape == TriggerShape.Sphere || Shape == TriggerShape.Cylinder)
                    Radius = value;
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
                    return Radius;
                if (Shape == TriggerShape.Cylinder)
                    return Height;
                return base.ScaleY;
            }
            set
            {
                if (Shape == TriggerShape.Sphere)
                    Radius = value;
                if (Shape == TriggerShape.Cylinder)
                    Height = value;
                base.ScaleY = value;
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleZ
        {
            get
            {
                if (Shape == TriggerShape.Sphere || Shape == TriggerShape.Cylinder)
                    return Radius;
                return base.ScaleZ;
            }
            set
            {
                if (Shape == TriggerShape.Sphere || Shape == TriggerShape.Cylinder)
                    Radius = value;
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
                FixPosition();
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position0Y
        {
            get => _trigPos0.Y;
            set
            {
                _trigPos0.Y = value;
                FixPosition();
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        public float Position0Z
        {
            get => _trigPos0.Z;
            set
            {
                _trigPos0.Z = value;
                FixPosition();
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Used only for Sphere and Cylinder.")]
        public float Radius
        {
            get => Position1X;
            set => Position1X = value;
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Used only for Cylinder.")]
        public float Height
        {
            get => Position1Y;
            set => Position1Y = value;
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Used only for Box.")]
        public float Position1X
        {
            get => _trigPos1.X;
            set
            {
                _trigPos1.X = value;
                FixPosition();
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Used only for Box.")]
        public float Position1Y
        {
            get => _trigPos1.Y;
            set
            {
                _trigPos1.Y = value;
                FixPosition();
            }
        }
        [Category("Trigger"), TypeConverter(typeof(FloatTypeConverter))]
        [Description("Used only for Box.")]
        public float Position1Z
        {
            get => _trigPos1.Z;
            set
            {
                _trigPos1.Z = value;
                FixPosition();
            }
        }

        public void SetPositions(float x0, float y0, float z0, float x1, float y1, float z1)
        {
            _trigPos0.X = x0;
            _trigPos0.Y = y0;
            _trigPos0.Z = z0;
            _trigPos1.X = x1;
            _trigPos1.Y = y1;
            _trigPos1.Z = z1;
            FixPosition();
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
        public DynamicTypeDescriptor TriggerFlags => IntFlagsDescriptor(0x90 + Offset);
    }
}