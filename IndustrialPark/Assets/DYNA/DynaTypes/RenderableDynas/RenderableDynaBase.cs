using HipHopFile;
using IndustrialPark.ArchiveEditors;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public abstract class RenderableDynaBase : AssetDYNA, IRenderableAsset, IClickableAsset, IAssetCopyPasteTransformation
    {
        private const string dynaCategoryName = "Placement";

        protected Vector3 _position;
        [Category(dynaCategoryName)]
        public virtual AssetSingle PositionX
        {
            get => _position.X;
            set => _position.X = value;
        }
        [Category(dynaCategoryName)]
        public virtual AssetSingle PositionY
        {
            get => _position.Y;
            set => _position.Y = value;
        }
        [Category(dynaCategoryName)]
        public virtual AssetSingle PositionZ
        {
            get => _position.Z;
            set => _position.Z = value;
        }

        public RenderableDynaBase(string assetName, DynaType dynaType, Vector3 position) : base(assetName, dynaType)
        {
            _position = position;
        }

        public RenderableDynaBase(Section_AHDR AHDR, DynaType type, EndianBinaryReader reader) : base(AHDR, type, reader) { }

        [Browsable(false)]
        public Matrix world { get; protected set; }
        protected BoundingBox boundingBox;

        protected abstract List<Vector3> vertexSource(SharpRenderer renderer);
        protected abstract List<RenderWareFile.Triangle> triangleSource(SharpRenderer renderer);

        public virtual void CalculateTransformation(SharpRenderer renderer)
        {
            world = Matrix.Translation(_position);
            boundingBox = CreateBoundingBox(vertexSource(renderer), world);
        }

        public abstract void Draw(SharpRenderer renderer);

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistanceFrom(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, _position);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, triangleSource(renderer), vertexSource(renderer), world);
            return null;
        }

        [Browsable(false)]
        public bool SpecialBlendMode(SharpRenderer renderer) => true;

        protected abstract bool DontRender { get; }

        public virtual bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (DontRender)
                return false;
            if (isInvisible)
                return false;
            if (AssetMODL.renderBasedOnLodt && GetDistanceFrom(renderer.Camera.Position) > SharpRenderer.DefaultLODTDistance)
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public virtual AssetTransformation GetTransformation() => new AssetTransformation()
        {
            _positionX = _position.X,
            _positionY = _position.Y,
            _positionZ = _position.Z,
        };

        public virtual void SetTransformation(AssetTransformation transformation)
        {
            _position.X = transformation._positionX;
            _position.Y = transformation._positionY;
            _position.Z = transformation._positionZ;
        }
    }
}