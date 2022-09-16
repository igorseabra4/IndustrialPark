using HipHopFile;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class SplineVector
    {
        public AssetSingle X { get; set; }
        public AssetSingle Y { get; set; }
        public AssetSingle Z { get; set; }
        public AssetSingle W { get; set; }

        public SplineVector()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 0;
        }

        public SplineVector(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public SplineVector(EndianBinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Z);
                return writer.ToArray();
            }
        }

        public static implicit operator Vector3(SplineVector vector) => new Vector3(vector.X, vector.Y, vector.Z);

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }
    }

    public class AssetSPLN : BaseAsset, IRenderableAsset
    {
        private const string categoryName = "Spline";

        [Category(categoryName), Description("Always 1 or 3, I don't recommend changing this on existing splines")]
        public int SplineType { get; set; }
        [Category(categoryName)]
        public AssetID UnknownHash_14 { get; set; }
        [Category(categoryName)]
        public AssetID UnknownHash_18 { get; set; }
        private SplineVector[] _points;
        [Category(categoryName)]
        public SplineVector[] Points
        {
            get => _points;
            set
            {
                _points = value;
                Setup(Program.Renderer);
            }
        }
        private AssetSingle[] _points2;
        [Category(categoryName)]
        public AssetSingle[] Points2
        {
            get => _points2;
            set
            {
                _points2 = value;
            }
        }

        public AssetSPLN(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game, endianness)
        {
            BaseFlags = BitConverter.ToUInt16(AHDR.data, 6);

            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                SplineType = reader.ReadInt32();
                int pointCount = reader.ReadInt32() - SplineType; // point count plus 3
                reader.ReadInt32(); // point count minus one
                UnknownHash_14 = reader.ReadUInt32();
                UnknownHash_18 = reader.ReadUInt32();

                _points = new SplineVector[pointCount];
                for (int i = 0; i < _points.Length; i++)
                    _points[i] = new SplineVector(reader);

                if (SplineType == 1)
                    reader.BaseStream.Position += 8;
                else if (SplineType == 3)
                    reader.BaseStream.Position += 16;

                for (int i = 0; i < _points.Length; i++)
                    _points[i].W = reader.ReadSingle();

                Setup(renderer);
                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(Endianness.Little));
                writer.Write(SplineType);
                writer.Write(_points.Length + SplineType); // point count plus 3
                writer.Write(_points.Length - 1); // point count minus one
                writer.Write(UnknownHash_14);
                writer.Write(UnknownHash_18);
                foreach (var v in _points)
                    writer.Write(v.Serialize(endianness));
                if (SplineType == 1)
                    writer.Write(new byte[8]);
                else if (SplineType == 3)
                    writer.Write(new byte[16]);
                foreach (var v in _points)
                    writer.Write(v.W);

                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(UnknownHash_14, ref result);
            Verify(UnknownHash_18, ref result);
        }

        private BoundingBox boundingBox;

        public static bool dontRender = false;

        private SharpDX.Direct3D11.Buffer vertexBuffer;
        private int vertexCount;

        public void Setup(SharpRenderer renderer)
        {
            if (renderer == null)
                return;

            renderer.completeVertexBufferList.Remove(vertexBuffer);
            if (vertexBuffer != null)
                vertexBuffer.Dispose();

            var vertices = new Vector3[_points.Length];
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = _points[i];

            vertexBuffer = SharpDX.Direct3D11.Buffer.Create(renderer.device.Device, BindFlags.VertexBuffer, vertices);
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

        public void Draw(SharpRenderer renderer) => renderer.DrawSpline(vertexBuffer, vertexCount, Matrix.Identity,
                isSelected ? renderer.selectedObjectColor : Color.YellowGreen.ToVector4(), false);

        public void CreateTransformMatrix()
        {
            if (_points.Length == 0)
                boundingBox = new BoundingBox();
            else
                boundingBox = new BoundingBox(_points[0], _points[0]);

            foreach (var v in _points)
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

        public float GetDistanceFrom(Vector3 cameraPosition) => Vector3.Distance(cameraPosition, boundingBox.Center);

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray) => null;

        public void Dispose() => vertexBuffer.Dispose();

        [Browsable(false)]
        public bool SpecialBlendMode => false;
    }
}