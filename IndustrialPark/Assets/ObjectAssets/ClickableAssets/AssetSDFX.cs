using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class AssetSDFX : BaseAsset, IRenderableAsset, IClickableAsset, IAssetCopyPasteTransformation
    {
        private const string categoryName = "SDFX";
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(SoundGroup);

        private uint _soundGroup;
        [Category(categoryName), ValidReferenceRequired]
        public AssetID SoundGroup
        {
            get => _soundGroup;
            set { _soundGroup = value; CreateTransformMatrix(); }
        }

        [Category(categoryName)]
        public AssetID Emitter { get; set; }
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
        public FlagBitmask SDFXFlags { get; set; } = IntFlagsDescriptor(
            null,
            null,
            "Play from Entity");

        private Func<uint, AssetSGRP> GetSGRP;

        public AssetSDFX(string assetName, Vector3 position, Func<uint, AssetSGRP> getSGRP) : base(assetName, AssetType.SDFX, BaseAssetType.SDFX)
        {
            GetSGRP = getSGRP;

            _position = position;

            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public AssetSDFX(Section_AHDR AHDR, Game game, Endianness endianness, Func<uint, AssetSGRP> getSGRP) : base(AHDR, game, endianness)
        {
            GetSGRP = getSGRP;

            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                _soundGroup = reader.ReadUInt32();
                Emitter = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                SDFXFlags.FlagValueInt = reader.ReadUInt32();

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(_soundGroup);
            writer.Write(Emitter);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(SDFXFlags.FlagValueInt);
            SerializeLinks(writer);
        }

        private Matrix world;
        private Matrix world2;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        public void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);
            world2 = Matrix.Scaling(_radius2 * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        public BoundingSphere boundingSphere;

        protected void CreateBoundingBox()
        {
            boundingSphere = new BoundingSphere(_position, _radius);
            boundingBox = BoundingBox.FromSphere(boundingSphere);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingSphere))
                return TriangleIntersection(ray, SharpRenderer.sphereTriangles, SharpRenderer.sphereVertices, world);
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

        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawSphere(world, isSelected, renderer.sfxColor);

            if (isSelected)
                renderer.DrawSphere(world2, false, renderer.sfxColor);
        }

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - _radius;

        private float _radius
        {
            get
            {
                var soundGroup = GetSGRP(_soundGroup);
                return soundGroup == null ? 1f : (float)soundGroup.InnerRadius;
            }
        }

        private float _radius2
        {
            get
            {
                var soundGroup = GetSGRP(_soundGroup);
                return soundGroup == null ? 1f : (float)soundGroup.OuterRadius;
            }
        }

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

                CreateTransformMatrix();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
            }
        }
    }
}