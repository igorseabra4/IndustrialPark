using HipHopFile;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSPLN : BaseAsset, IRenderableAsset
    {
        private const string categoryName = "Spline";

        [Category(categoryName)]
        public AssetID UnknownHash_14 { get; set; }
        [Category(categoryName)]
        public AssetID UnknownHash_18 { get; set; }
        private WireVector[] _points;
        [Category(categoryName)]
        public WireVector[] Points
        {
            get => _points;
            set
            {
                _points = value;
                Setup(Program.MainForm.renderer);
            }
        }

        public AssetSPLN(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            reader.ReadInt32(); // unknown, always 3
            int pointCount = reader.ReadInt32() - 3; // point count plus 3
            reader.ReadInt32(); // point count minus one
            UnknownHash_14 = reader.ReadUInt32();
            UnknownHash_18 = reader.ReadUInt32();

            _points = new WireVector[pointCount];
            for (int i = 0; i < _points.Length; i++)
                _points[i] = new WireVector(reader);

            Setup(renderer);
            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeBase(endianness));

            writer.Write(3); // unknown, always 3
            writer.Write(_points.Length + 3); // point count plus 3
            writer.Write(_points.Length - 1); // point count minus one
            writer.Write(UnknownHash_14);
            writer.Write(UnknownHash_18);
            foreach (var v in _points)
                writer.Write(v.Serialize(endianness));
            writer.Write(new byte[16]);
            float acc = 0;
            for (int i = 0; i < _points.Length - 1; i++)
            {
                acc += Distance(_points[i], _points[i + 1]);
                writer.Write(acc);
            }
            writer.Write(acc);

            writer.Write(SerializeLinks(endianness));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => UnknownHash_14 == assetID || UnknownHash_18 == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(UnknownHash_14, ref result);
            Verify(UnknownHash_18, ref result);
        }

        private static float Distance(WireVector v1, WireVector v2) =>
            Vector3.Distance(new Vector3(v1.X, v1.Y, v1.Z), new Vector3(v2.X, v2.Y, v2.Z));

        private BoundingBox boundingBox;

        public static bool dontRender = false;

        private SharpDX.Direct3D11.Buffer vertexBuffer;
        private int vertexCount;

        public void Setup(SharpRenderer renderer)
        {
            renderer.completeVertexBufferList.Remove(vertexBuffer);
            if (vertexBuffer != null)
                vertexBuffer.Dispose();
            
            vertexBuffer = SharpDX.Direct3D11.Buffer.Create(renderer.device.Device, BindFlags.VertexBuffer, _points);
            renderer.completeVertexBufferList.Add(vertexBuffer);
            vertexCount = _points.Length;
        }

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;
            
            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawSpline(vertexBuffer, vertexCount, Matrix.Identity,
                isSelected ? renderer.selectedObjectColor : Color.YellowGreen.ToVector4(), false);
        }

        public void CreateTransformMatrix()
        {
            if (_points.Length == 0)
                boundingBox = new BoundingBox();
            else
                boundingBox = new BoundingBox(
                    new Vector3(_points[0].X, _points[0].Y, _points[0].Z),
                    new Vector3(_points[0].X, _points[0].Y, _points[0].Z));

            foreach (WireVector v in _points)
            {
                if (v.X > boundingBox.Maximum.X)
                    boundingBox.Maximum.X = v.X;
                if (v.Y > boundingBox.Maximum.Y)
                    boundingBox.Maximum.Y = v.Y;
                if (v.Z > boundingBox.Maximum.Z)
                    boundingBox.Maximum.Z = v.Z;
                if (v.X < boundingBox.Minimum.X)
                    boundingBox.Minimum.X = v.X;
                if (v.Y < boundingBox.Minimum.Y)
                    boundingBox.Minimum.Y = v.Y;
                if (v.Z < boundingBox.Minimum.Z)
                    boundingBox.Minimum.Z = v.Z;
            }
        }
        
        public float GetDistanceFrom(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, boundingBox.Center);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray) => null;

        public void Dispose()
        {
            vertexBuffer.Dispose();
        }

        [Browsable(false)]
        public bool SpecialBlendMode => false;
    }
}