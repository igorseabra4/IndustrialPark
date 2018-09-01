using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetMRKR : RenderableAssetWithPosition
    {
        public static bool dontRender = false;

        public AssetMRKR(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            _position = new Vector3(ReadFloat(0x0), ReadFloat(0x4), ReadFloat(0x8));
                        
            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(_position);

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            boundingBox =  BoundingBox.FromPoints(SharpRenderer.pyramidVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        protected override float? TriangleIntersection(Ray r, float distance)
        {
            return TriangleIntersection(r, distance, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices);
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;
            renderer.DrawCube(world, isSelected);
            base.Draw(renderer);
        }

        private Vector3 _position;
        public override float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x0, _position.X);
                CreateTransformMatrix();
            }
        }

        public override float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0x4, _position.Y);
                CreateTransformMatrix();
            }
        }

        public override float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x8, _position.Z);
                CreateTransformMatrix();
            }
        }

        [Browsable(false)]
        public override float ScaleX
        {
            get { return 0f; }
            set { }
        }

        [Browsable(false)]
        public override float ScaleY
        {
            get { return 0f; }
            set { }
        }

        [Browsable(false)]
        public override float ScaleZ
        {
            get { return 0f; }
            set { }
        }
    }
}