using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class AssetMRKR : Asset, IRenderableAsset, IClickableAsset, IAssetCopyPasteTransformation
    {
        private const string categoryName = "Marker";

        private Vector3 _position;
        [Category(categoryName)]
        public AssetSingle PositionX
        {
            get => _position.X;
            set => _position.X = value;
        }
        [Category(categoryName)]
        public AssetSingle PositionY
        {
            get => _position.Y;
            set => _position.Y = value;
        }
        [Category(categoryName)]
        public AssetSingle PositionZ
        {
            get => _position.Z;
            set => _position.Z = value;
        }

        public AssetMRKR(string assetName, Vector3 position) : base(assetName, AssetType.Marker)
        {
            _position = position;
        }

        public AssetMRKR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
        }

        private Matrix world;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        [Browsable(false)]
        public bool SpecialBlendMode(SharpRenderer renderer) => true;

        public void CreateTransformMatrix(SharpRenderer renderer)
        {
            world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f));
            CreateBoundingBox(renderer);
        }

        protected void CreateBoundingBox(SharpRenderer renderer)
        {
            var vertices = new Vector3[renderer.pyramidVertices.Count];

            for (int i = 0; i < renderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(renderer.pyramidVertices[i], world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, renderer.pyramidTriangles, renderer.pyramidVertices, world);
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

        public void Draw(SharpRenderer renderer) => renderer.DrawPyramid(world, isSelected, 1f);

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position);

        public void CopyTransformation()
        {
            var transformation = new Transformation()
            {
                _positionX = _position.X,
                _positionY = _position.Y,
                _positionZ = _position.Z,
            };
            Clipboard.SetText(JsonConvert.SerializeObject(transformation));
        }

        public void PasteTransformation()
        {
            try
            {
                var transformation = JsonConvert.DeserializeObject<Transformation>(Clipboard.GetText());
                _position.X = transformation._positionX;
                _position.Y = transformation._positionY;
                _position.Z = transformation._positionZ;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
            }
        }
    }
}