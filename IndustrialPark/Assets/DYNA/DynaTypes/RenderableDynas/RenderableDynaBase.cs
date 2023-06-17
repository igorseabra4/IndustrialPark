using HipHopFile;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public abstract class RenderableDynaBase : AssetDYNA, IRenderableAsset, IClickableAsset`, IAssetCopyPasteTransformation
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

        public RenderableDynaBase(string assetName, DynaType dynaType, Vector3 position) : base(assetName, dynaType)
        {
            _position = position;
        }

        public RenderableDynaBase(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness) { }

        [Browsable(false)]
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

        public virtual void CopyTransformation()
        {
            var transformation = new Transformation()
            {
                _positionX = _position.X,
                _positionY = _position.Y,
                _positionZ = _position.Z,
            };
            Clipboard.SetText(JsonConvert.SerializeObject(transformation));
        }

        public virtual void PasteTransformation()
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