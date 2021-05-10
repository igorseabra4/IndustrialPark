using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetLITE : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Light";

        [Category(categoryName)]
        public byte UnknownByte08 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte09 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte0A { get; set; }
        [Category(categoryName)]
        public byte UnknownByte0B { get; set; }
        [Category(categoryName)]
        public int UnknownInt0C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat10 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat14 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat18 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat1C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat20 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat24 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat28 { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat2C { get; set; }
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
        public AssetSingle UnknownFloat3C { get; set; }
        [Category(categoryName)]
        public AssetSingle UnknownFloat40 { get; set; }

        public AssetLITE(string assetName, Vector3 position) : base(assetName, AssetType.LITE, BaseAssetType.Light)
        {
            _position = position;

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetLITE(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            UnknownByte08 = reader.ReadByte();
            UnknownByte09 = reader.ReadByte();
            UnknownByte0A = reader.ReadByte();
            UnknownByte0B = reader.ReadByte();
            UnknownInt0C = reader.ReadInt32();
            UnknownFloat10 = reader.ReadSingle();
            UnknownFloat14 = reader.ReadSingle();
            UnknownFloat18 = reader.ReadSingle();   
            UnknownFloat1C = reader.ReadSingle();
            UnknownFloat20 = reader.ReadSingle();
            UnknownFloat24 = reader.ReadSingle();
            UnknownFloat28 = reader.ReadSingle();
            UnknownFloat2C = reader.ReadSingle();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            UnknownFloat3C = reader.ReadSingle();
            UnknownFloat40 = reader.ReadSingle();

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeBase(endianness));

            writer.Write(UnknownByte08);
            writer.Write(UnknownByte09);
            writer.Write(UnknownByte0A);
            writer.Write(UnknownByte0B);
            writer.Write(UnknownInt0C);
            writer.Write(UnknownFloat10);
            writer.Write(UnknownFloat14);
            writer.Write(UnknownFloat18);
            writer.Write(UnknownFloat1C);
            writer.Write(UnknownFloat20);
            writer.Write(UnknownFloat24);
            writer.Write(UnknownFloat28);
            writer.Write(UnknownFloat2C);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(UnknownFloat3C);
            writer.Write(UnknownFloat40);

            writer.Write(SerializeLinks(endianness));
            return writer.ToArray();
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

            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistanceFrom(renderer.Camera.Position) < SharpRenderer.DefaultLODTDistance)
                    return renderer.frustum.Intersects(ref boundingBox);
                return false;
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer) => renderer.DrawCube(world, isSelected);
        
        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position);
    }
}