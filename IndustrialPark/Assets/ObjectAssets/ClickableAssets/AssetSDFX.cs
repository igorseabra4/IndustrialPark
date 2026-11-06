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
            set => _soundGroup = value;
        }

        [Category(categoryName)]
        public AssetID Emitter { get; set; }
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

        [Category(categoryName)]
        public FlagBitmask SDFXFlags { get; set; } = IntFlagsDescriptor(
            null,
            null,
            "Play from Entity");

        public Func<uint, AssetSGRP> GetSGRP;

        public AssetSDFX(string assetName, Vector3 position) : base(assetName, AssetType.SDFX, BaseAssetType.SDFX)
        {
            _position = position;
        }

        public AssetSDFX(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            _soundGroup = reader.ReadUInt32();
            Emitter = reader.ReadUInt32();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            SDFXFlags.FlagValueInt = reader.ReadUInt32();
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
        public BoundingSphere boundingSphere;
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        public void CreateTransformMatrix(SharpRenderer renderer)
        {
            world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected void CreateBoundingBox()
        {
            boundingSphere = new BoundingSphere(_position, _radius);
            boundingBox = BoundingBox.FromSphere(boundingSphere);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingSphere))
                return TriangleIntersection(ray, renderer.sphereTriangles, renderer.sphereVertices, world);
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

            return renderer.frustum.Intersects(ref boundingSphere);
        }

        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawSphere(boundingSphere, isSelected, renderer.sfxColor);

            if (isSelected)
                renderer.DrawSphere(new BoundingSphere(_position, _radius2), false, renderer.sfxColor);
        }

        [Browsable(false)]
        public bool SpecialBlendMode(SharpRenderer renderer) => true;

        public BoundingBox GetBoundingBox() => boundingBox;

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, _position) - _radius;

        private float _radius
        {
            get
            {
                var soundGroup = GetSGRP?.Invoke(_soundGroup);
                return soundGroup == null ? 1f : (float)soundGroup.InnerRadius;
            }
        }

        private float _radius2
        {
            get
            {
                var soundGroup = GetSGRP?.Invoke(_soundGroup);
                return soundGroup == null ? 1f : (float)soundGroup.OuterRadius;
            }
        }

        public Transformation GetTransformation() => new Transformation()
        {
            _positionX = _position.X,
            _positionY = _position.Y,
            _positionZ = _position.Z,
        };

        public void SetTransformation(Transformation transformation)
        {
            _position.X = transformation._positionX;
            _position.Y = transformation._positionY;
            _position.Z = transformation._positionZ;
        }
    }
}