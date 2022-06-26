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

    public class AssetTRIG : EntityAsset, IVolumeAsset
    {
        private const string categoryName = "Trigger";
        public override string AssetInfo => Shape.ToString();

        [Category(categoryName)]
        public TriggerShape Shape
        {
            get => (TriggerShape)(byte)TypeFlag;
            set => TypeFlag = (byte)value;
        }

        private Vector3 _minimum;
        private Vector3 _maximum;

        [Category(categoryName), Description("Center position for Sphere and Cylinder")]
        public AssetSingle MinimumX
        {
            get => _minimum.X;
            set
            {
                _minimum.X = value;
                FixPosition();
            }
        }
        [Category(categoryName), Description("Center position for Sphere and Cylinder")]
        public AssetSingle MinimumY
        {
            get => _minimum.Y;
            set
            {
                _minimum.Y = value;
                FixPosition();
            }
        }
        [Category(categoryName), Description("Center position for Sphere and Cylinder")]
        public AssetSingle MinimumZ
        {
            get => _minimum.Z;
            set
            {
                _minimum.Z = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        [Description("Used only for Sphere and Cylinder.")]
        public AssetSingle Radius
        {
            get => MaximumX;
            set => MaximumX = value;
        }
        [Category(categoryName)]
        [Description("Used only for Cylinder.")]
        public AssetSingle Height
        {
            get => MaximumY;
            set => MaximumY = value;
        }
        [Category(categoryName)]
        [Description("Used only for Box.")]
        public AssetSingle MaximumX
        {
            get => _maximum.X;
            set
            {
                _maximum.X = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        [Description("Used only for Box.")]
        public AssetSingle MaximumY
        {
            get => _maximum.Y;
            set
            {
                _maximum.Y = value;
                FixPosition();
            }
        }
        [Category(categoryName)]
        [Description("Used only for Box.")]
        public AssetSingle MaximumZ
        {
            get => _maximum.Z;
            set
            {
                _maximum.Z = value;
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

        public AssetTRIG(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.Trigger, BaseAssetType.Trigger, position)
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
                case AssetTemplate.Bus_Stop_Trigger:
                    Shape = TriggerShape.Sphere;
                    MinimumX = position.X;
                    MinimumY = position.Y;
                    MinimumZ = position.Z;
                    if (template == AssetTemplate.Sphere_Trigger)
                        Radius = 10f;
                    else if (template == AssetTemplate.Bus_Stop_Trigger)
                        Radius = 2.5f;
                    else
                        Radius = 6f;
                    break;
                case AssetTemplate.Cylinder_Trigger:
                    Shape = TriggerShape.Cylinder;
                    Radius = 10f;
                    Height = 5f;
                    MinimumX = position.X;
                    MinimumY = position.Y;
                    MinimumZ = position.Z;
                    break;
            }
        }

        public AssetTRIG(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                _minimum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _maximum = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
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
                writer.Write(MinimumX);
                writer.Write(MinimumY);
                writer.Write(MinimumZ);
                writer.Write(MaximumX);
                writer.Write(MaximumY);
                writer.Write(MaximumZ);
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
                Vector3 boxSize = _maximum - _minimum;
                Vector3 midPos = (_minimum + _maximum) / 2f;

                world = Matrix.Scaling(boxSize) *
                    Matrix.Translation(midPos) *
                    Matrix.Translation(-_position) *
                    Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) *
                    Matrix.Translation(_position);
            }
            else if (Shape == TriggerShape.Sphere)
            {
                world = Matrix.Scaling(Radius * 2f) *
                    Matrix.Translation(_minimum) *
                    Matrix.Translation(-_position) *
                    Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) *
                    Matrix.Translation(_position);
            }
            else
            {
                world = Matrix.Scaling(Radius * 2f, Height * 2f, Radius * 2f) *
                    Matrix.Translation(_minimum) *
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
                boundingSphere = new BoundingSphere(_minimum, Radius);
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
                if (_minimum.X > _maximum.X)
                {
                    var temp = _maximum.X;
                    _maximum.X = _minimum.X;
                    _minimum.X = temp;
                }
                if (_minimum.Y > _maximum.Y)
                {
                    var temp = _maximum.Y;
                    _maximum.Y = _minimum.Y;
                    _minimum.Y = temp;
                }
                if (_minimum.Z > _maximum.Z)
                {
                    var temp = _maximum.Z;
                    _maximum.Z = _minimum.Z;
                    _minimum.Z = temp;
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
            _minimum.X = x0;
            _minimum.Y = y0;
            _minimum.Z = z0;
            _maximum.X = x1;
            _maximum.Y = y1;
            _maximum.Z = z1;
            FixPosition();
        }
    }
}