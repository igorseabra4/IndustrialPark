using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetMVPT : RenderableAsset
    {
        public AssetMVPT(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override float PositionX
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                Write(0x8, _position.X);
                CreateTransformMatrix();
            }
        }

        public override float PositionY
        {
            get { return _position.Y; }
            set
            {
                _position.Y = value;
                Write(0xC, _position.Y);
                CreateTransformMatrix();
            }
        }

        public override float PositionZ
        {
            get { return _position.Z; }
            set
            {
                _position.Z = value;
                Write(0x10, _position.Z);
                CreateTransformMatrix();
            }
        }

        public override Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public override Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                Write(0x1C, value.X);
                Write(0x20, value.Y);
                Write(0x24, value.Z);
            }
        }
        
        public int[] PairAssetIDs { get; set; }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            _position = new Vector3(ReadFloat(0x8), ReadFloat(0xC), ReadFloat(0x10));
            _scale = new Vector3(ReadFloat(0x1C), ReadFloat(0x20), ReadFloat(0x24));

            int amountOfPairs = ReadShort(0x1A);

            PairAssetIDs = new int[amountOfPairs];
            for (int i = 0; i < amountOfPairs; i++)
                PairAssetIDs[i] = ReadInt(0x28 + 4 * i);

            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(_position);

            boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
        }
    }
}