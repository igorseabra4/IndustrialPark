using HipHopFile;
using SharpDX;
using System.ComponentModel;
using IndustrialPark.AssetEditorColors;

namespace IndustrialPark
{
    public enum LightType : byte
    {
        Point = 0,
        Spot = 1,
        Point2 = 2,
        Point3 = 3
    }

    public enum LightEffect : byte
    {
        None = 0,
        None1 = 1,
        FlickerSlow = 2,
        Flicker = 3,
        FlickerErratic = 4,
        StrobeSlow = 5,
        Strobe = 6,
        StrobeFast = 7,
        DimSlow = 8,
        Dim = 9,
        DimFast = 10,
        HalfDimSlow = 11,
        HalfDim = 12,
        HalfDimFast = 13,
        RandomColSlow = 14,
        RandomCol = 15,
        RandomColFast = 16,
        Cauldron = 17,
    }

    public class AssetLITE : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Light";

        [Category(categoryName)]
        public LightType LightType { get; set; }
        [Category(categoryName)]
        public LightEffect LightEffect { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor(null, null, null, "Environment", null, "On");
        protected Vector4 _color;
        [Category(categoryName + " Color"), DisplayName("Red (0 - 1)")]
        public AssetSingle ColorRed
        {
            get => _color.X;
            set => _color.X = value;
        }
        [Category(categoryName + " Color"), DisplayName("Green (0 - 1)")]
        public AssetSingle ColorGreen
        {
            get => _color.Y;
            set => _color.Y = value;
        }
        [Category(categoryName + " Color"), DisplayName("Blue (0 - 1)")]
        public AssetSingle ColorBlue
        {
            get => _color.Z;
            set => _color.Z = value;
        }
        [Category(categoryName + " Color"), DisplayName("Alpha (0 - 1)")]
        public AssetSingle ColorAlpha
        {
            get => _color.W;
            set => _color.W = value;
        }
        [Category(categoryName + " Color")]
        public AssetColor Color_RBGA
        {
            get => AssetColor.FromVector4(ColorRed, ColorGreen, ColorBlue, ColorAlpha);
            set
            {
                ColorRed = value.R / 255f;
                ColorGreen = value.G / 255f;
                ColorBlue = value.B / 255f;
                ColorAlpha = value.A / 255f;
            }
        }
        [Category(categoryName)]
        public AssetSingle DirectionX { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionY { get; set; }
        [Category(categoryName)]
        public AssetSingle DirectionZ { get; set; }
        [Category(categoryName)]
        public AssetSingle LightConeAngle { get; set; }
        private Vector3 _position;
        [Category(categoryName)]
        public AssetSingle PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }
        [Category(categoryName)]
        public AssetSingle Radius { get; set; }
        [Category(categoryName)]
        public AssetID Attach { get; set; }

        public AssetLITE(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.Light, BaseAssetType.Light)
        {
            _position = position;
            _color = Vector4.One;
            Radius = 10f;

            if (template == AssetTemplate.Cauldron_Light)
            {
                LightType = LightType.Point3;
                LightEffect = LightEffect.Cauldron;
                Flags.FlagValueInt = 55;
                DirectionY = 1f;
                LightConeAngle = 45f;
                Radius = 5f;
                ColorRed = 0.250980f;
                ColorGreen = 0.917647f;
                ColorBlue = 0.082353f;
                ColorAlpha = 1f;
            }

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetLITE(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                LightType = (LightType)reader.ReadByte();
                LightEffect = (LightEffect)reader.ReadByte();
                reader.ReadInt16();
                Flags.FlagValueInt = reader.ReadUInt32();
                _color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                DirectionX = reader.ReadSingle();
                DirectionY = reader.ReadSingle();
                DirectionZ = reader.ReadSingle();
                LightConeAngle = reader.ReadSingle();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Radius = reader.ReadSingle();
                Attach = reader.ReadUInt32();

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)LightType);
            writer.Write((byte)LightEffect);
            writer.Write((short)0);
            writer.Write(Flags.FlagValueInt);
            writer.Write(_color.X);
            writer.Write(_color.Y);
            writer.Write(_color.Z);
            writer.Write(_color.W);
            writer.Write(DirectionX);
            writer.Write(DirectionY);
            writer.Write(DirectionZ);
            writer.Write(LightConeAngle);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(Radius);
            writer.Write(Attach);

            SerializeLinks(writer);
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);
            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            var vertices = new Vector3[SharpRenderer.cubeVertices.Count];

            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i] * 0.5f, world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices, world);
            return null;
        }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > SharpRenderer.DefaultLODTDistance)
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer) => renderer.DrawCube(world, isSelected);

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position);
    }
}