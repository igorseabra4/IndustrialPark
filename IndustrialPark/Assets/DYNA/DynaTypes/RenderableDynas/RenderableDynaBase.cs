using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class RenderableDynaBase : AssetDYNA, IRenderableAsset, IClickableAsset
    {
        private const string dynaCategoryName = "DYNA Placement";

        protected Vector3 _position;
        [Category(dynaCategoryName)]
        public virtual AssetSingle PositionX
        {
            get => _position.X;
            set { _position.X = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public virtual AssetSingle PositionY
        {
            get => _position.Y;
            set { _position.Y = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public virtual AssetSingle PositionZ
        {
            get => _position.Z;
            set { _position.Z = value; CreateTransformMatrix(); }
        }

        public RenderableDynaBase(string assetName, DynaType dynaType, short version) : base(assetName, dynaType, version)
        {
        }

        public RenderableDynaBase(string assetName, DynaType dynaType, short version, Vector3 position) : base(assetName, dynaType, version)
        {
            _position = position;
        }

        public RenderableDynaBase(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness) { }

        public Matrix world { get; protected set; }
        protected BoundingBox boundingBox;
        protected Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        protected abstract List<Vector3> vertexSource { get; }
        protected abstract List<Models.Triangle> triangleSource { get; }

        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected virtual void CreateBoundingBox()
        {
            vertices = new Vector3[vertexSource.Count];
            for (int i = 0; i < vertexSource.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(vertexSource[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);

            triangles = new RenderWareFile.Triangle[triangleSource.Count];
            for (int i = 0; i < triangleSource.Count; i++)
                triangles[i] = new RenderWareFile.Triangle((ushort)triangleSource[i].materialIndex,
                    (ushort)triangleSource[i].vertex1, (ushort)triangleSource[i].vertex2, (ushort)triangleSource[i].vertex3);
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
                return triangles == null ? TriangleIntersection(ray, triangleSource, vertexSource, world) : TriangleIntersection(ray);
            return null;
        }

        private float? TriangleIntersection(Ray ray)
        {
            float? smallestDistance = null;

            foreach (RenderWareFile.Triangle t in triangles)
                if (ray.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                    if (smallestDistance == null || distance < smallestDistance)
                        smallestDistance = distance;

            return smallestDistance;
        }

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public virtual bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;

            BoundingBox bb = GetBoundingBox();

            if (AssetMODL.renderBasedOnLodt)
            {
                if (GetDistanceFrom(renderer.Camera.Position) < SharpRenderer.DefaultLODTDistance)
                    return renderer.frustum.Intersects(ref bb);

                return false;
            }

            return renderer.frustum.Intersects(ref bb);
        }
    }
}