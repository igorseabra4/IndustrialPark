using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public enum TriggerShape
    {
        Box = 0,
        Sphere = 1
    }

    public class AssetTRIG : PlaceableAsset
    {
        public static new bool dontRender = false;

        public AssetTRIG(Section_AHDR AHDR) : base(AHDR)
        {
        }

        protected byte _shape;
        public TriggerShape Shape
        {
            get { return (TriggerShape)_shape; }
            set
            {
                _shape = (byte)value;
                Write(0x09, _shape);
            }
        }

        public override float PositionX
        {
            get { return _position.X; }
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
                Write(0x20, _position.X);
                CreateTransformMatrix();
            }
        }

        public override float PositionY
        {
            get { return _position.Y; }
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
                Write(0x24, _position.Y);
                CreateTransformMatrix();
            }
        }

        public override float PositionZ
        {
            get { return _trigPos0.Z; }
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
                Write(0x28, _position.Z);
                Position0Z = value;
            }
        }
        
        protected Vector3 _trigPos0;
        protected Vector3 _trigPos1;
        protected Vector3 _trigPos2;
        protected Vector3 _trigPos3;

        public float Position0X
        {
            get { return _trigPos0.X; }
            set
            {
                _trigPos0.X = value;
                Write(0x54, _trigPos0.X);
                CreateTransformMatrix();
            }
        }
        public float Position0Y
        {
            get { return _trigPos0.Y; }
            set
            {
                _trigPos0.Y = value;
                Write(0x58, _trigPos0.Y);
                CreateTransformMatrix();
            }
        }
        public float Position0Z
        {
            get { return _trigPos0.Z; }
            set
            {
                _trigPos0.Z = value;
                Write(0x5C, _trigPos0.Z);
                CreateTransformMatrix();
            }
        }
        public float Position1X_Radius
        {
            get { return _trigPos1.X; }
            set
            {
                _trigPos1.X = value;
                Write(0x60, _trigPos1.X);
                CreateTransformMatrix();
            }
        }
        public float Position1Y
        {
            get { return _trigPos1.Y; }
            set
            {
                _trigPos1.Y = value;
                Write(0x64, _trigPos1.Y);
                CreateTransformMatrix();
            }
        }
        public float Position1Z
        {
            get { return _trigPos1.Z; }
            set
            {
                _trigPos1.Z = value;
                Write(0x68, _trigPos1.Z);
                CreateTransformMatrix();
            }
        }
        public float Position2X
        {
            get { return _trigPos2.X; }
            set
            {
                _trigPos2.X = value;
                Write(0x6C, _trigPos2.X);
            }
        }
        public float Position2Y
        {
            get { return _trigPos2.Y; }
            set
            {
                _trigPos2.Y = value;
                Write(0x70, _trigPos2.Y);
            }
        }
        public float Position2Z
        {
            get { return _trigPos2.Z; }
            set
            {
                _trigPos2.Z = value;
                Write(0x74, _trigPos2.Z);
            }
        }
        public float Position3X
        {
            get { return _trigPos3.X; }
            set
            {
                _trigPos3.X = value;
                Write(0x78, _trigPos3.X);
            }
        }
        public float Position3Y
        {
            get { return _trigPos3.Y; }
            set
            {
                _trigPos3.Y = value;
                Write(0x7C, _trigPos3.Y);
            }
        }
        public float Position3Z
        {
            get { return _trigPos3.Z; }
            set
            {
                _trigPos3.Z = value;
                Write(0x80, _trigPos3.Z);
            }
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            _assetID = ReadUInt(0);
            _shape = ReadByte(9);
            _position = new Vector3(ReadFloat(0x20), ReadFloat(0x24), ReadFloat(0x28));
            _rotation = new Vector3(ReadFloat(0x18), ReadFloat(0x14), ReadFloat(0x1C));
            _scale = new Vector3(ReadFloat(0x2C), ReadFloat(0x30), ReadFloat(0x34));
            _trigPos0 = new Vector3(ReadFloat(0x54), ReadFloat(0x58), ReadFloat(0x5C));
            _trigPos1 = new Vector3(ReadFloat(0x60), ReadFloat(0x64), ReadFloat(0x68));
            _trigPos2 = new Vector3(ReadFloat(0x6C), ReadFloat(0x70), ReadFloat(0x74));
            _trigPos3 = new Vector3(ReadFloat(0x78), ReadFloat(0x7C), ReadFloat(0x80));

            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
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
                
                boundingBox = new BoundingBox(-Vector3.One, Vector3.One);
                boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
                boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
            }
            else
            {
                world = Matrix.Scaling(Position1X_Radius) * Matrix.Translation(_trigPos0);

                boundingSphere = new BoundingSphere(_trigPos0, Position1X_Radius / 2f);
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

        public BoundingSphere boundingSphere;

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

        public override float? IntersectsWith(Ray ray)
        {
            if (dontRender)
                return null;

            if (Shape == TriggerShape.Box)
                if (ray.Intersects(ref boundingBox, out float distance))
                    return TriangleIntersection(ray, distance, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices);
            if (Shape == TriggerShape.Sphere)
                if (ray.Intersects(ref boundingSphere, out float distance2))
                    return TriangleIntersection(ray, distance2, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices);
            return null;
        }
    }
}