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
        private const string categoryName = "Trigger";

        [Category(categoryName)]
        public TriggerShape Shape
        {
            get => (TriggerShape)(byte)TypeFlag;
            set => TypeFlag = (byte)value;
        }

        private Vector3 _trigPos0;
        private Vector3 _trigPos1;

        [Category(categoryName)]
        public AssetSingle Position0X
        {
            get => _trigPos0.X;
            set
            {
                _trigPos0.X = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        public AssetSingle Position0Y
        {
            get => _trigPos0.Y;
            set
            {
                _trigPos0.Y = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        public AssetSingle Position0Z
        {
            get => _trigPos0.Z;
            set
            {
                _trigPos0.Z = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        [Description("Used only for Sphere and Cylinder.")]
        public AssetSingle Radius
        {
            get => Position1X;
            set => Position1X = value;
        }
        [Category(categoryName)]
        [Description("Used only for Cylinder.")]
        public AssetSingle Height
        {
            get => Position1Y;
            set => Position1Y = value;
        }
        [Category(categoryName)]
        [Description("Used only for Box.")]
        public AssetSingle Position1X
        {
            get => _trigPos1.X;
            set
            {
                _trigPos1.X = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        [Description("Used only for Box.")]
        public AssetSingle Position1Y
        {
            get => _trigPos1.Y;
            set
            {
                _trigPos1.Y = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        [Description("Used only for Box.")]
        public AssetSingle Position1Z
        {
            get => _trigPos1.Z;
            set
            {
                _trigPos1.Z = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        public AssetSingle Position2X { get; set; }
        [Category(categoryName)]
        public AssetSingle Position2Y { get; set; }
        [Category(categoryName)]
        public AssetSingle Position2Z { get; set; }
        [Category(categoryName)]
        public AssetSingle Position3X { get; set; }
        [Category(categoryName)]
        public AssetSingle Position3Y { get; set; }
        [Category(categoryName)]
        public AssetSingle Position3Z { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionX { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionY { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionZ { get; set; }
        [Category(categoryName)]
        public FlagBitmask TriggerFlags { get; set; } = IntFlagsDescriptor();

        public AssetTRIG(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.TRIG, BaseAssetType.Trigger, position)
        {
            SolidityFlags.FlagValueByte = 0;
            ColorAlpha = 0;
            ColorAlphaSpeed = 0;
            DirectionY = -0;
            DirectionZ = 1f;

            switch (template)
            {
                case AssetTemplate.Box_Trigger:
                    Shape = TriggerShape.Box;
                    SetPositions(position.X + 5f, position.Y + 5f, position.Z + 5f, position.X - 5f, position.Y - 5f, position.Z - 5f);
                    break;
                case AssetTemplate.Sphere_Trigger:
                case AssetTemplate.Checkpoint:
                case AssetTemplate.Checkpoint_Invisible:
                case AssetTemplate.BusStop_Trigger:
                    Shape = TriggerShape.Sphere;
                    Position0X = position.X;
                    Position0Y = position.Y;
                    Position0Z = position.Z;
                    if (template == AssetTemplate.Sphere_Trigger)
                        Radius = 10f;
                    else if (template == AssetTemplate.BusStop_Trigger)
                        Radius = 2.5f;
                    else
                        Radius = 6f;
                    break;
                case AssetTemplate.Cylinder_Trigger:
                   Shape = TriggerShape.Cylinder;
                    Radius = 10f;
                    Height = 5f;
                    Position0X = position.X;
                    Position0Y = position.Y;
                    Position0Z = position.Z;
                    break;
            }
        }

        public AssetTRIG(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                _trigPos0 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _trigPos1 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Position2X = reader.ReadSingle();
                Position2Y = reader.ReadSingle();
                Position2Z = reader.ReadSingle();
                Position3X = reader.ReadSingle();
                Position3Y = reader.ReadSingle();
                Position3Z = reader.ReadSingle();
                DirectionX = reader.ReadSingle();
                DirectionY = reader.ReadSingle();
                DirectionZ = reader.ReadSingle();
                TriggerFlags.FlagValueInt = reader.ReadUInt32();

                FixPosition();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(Position0X);
                writer.Write(Position0Y);
                writer.Write(Position0Z);
                writer.Write(Position1X);
                writer.Write(Position1Y);
                writer.Write(Position1Z);
                writer.Write(Position2X);
                writer.Write(Position2Y);
                writer.Write(Position2Z);
                writer.Write(Position3X);
                writer.Write(Position3Y);
                writer.Write(Position3Z);
                writer.Write(DirectionX);
                writer.Write(DirectionY);
                writer.Write(DirectionZ);
                writer.Write(TriggerFlags.FlagValueInt);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public BoundingSphere boundingSphere;
        private Matrix pivotWorld;

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

        [Browsable(false)]
        public override bool SpecialBlendMode => true;

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

        public void FixPosition()
        {
            if (Shape == TriggerShape.Box)
            {
                if (_trigPos0.X > _trigPos1.X)
                {
                    AssetSingle temp = _trigPos1.X;
                    _trigPos1.X = _trigPos0.X;
                    _trigPos0.X = temp;
                }
                if (_trigPos0.Y > _trigPos1.Y)
                {
                    AssetSingle temp = _trigPos1.Y;
                    _trigPos1.Y = _trigPos0.Y;
                    _trigPos0.Y = temp;
                }
                if (_trigPos0.Z > _trigPos1.Z)
                {
                    AssetSingle temp = _trigPos1.Z;
                    _trigPos1.Z = _trigPos0.Z;
                    _trigPos0.Z = temp;
                }
            }

            CreateTransformMatrix();
        }
        
        [Category("Placement")]        
        public override AssetSingle ScaleX
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
        public override AssetSingle ScaleY
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
        public override AssetSingle ScaleZ
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
        
        public void SetPositions(float x0, AssetSingle y0, AssetSingle z0, AssetSingle x1, AssetSingle y1, AssetSingle z1)
        {
            _trigPos0.X = x0;
            _trigPos0.Y = y0;
            _trigPos0.Z = z0;
            _trigPos1.X = x1;
            _trigPos1.Y = y1;
            _trigPos1.Z = z1;
            FixPosition();
        }
    }
}