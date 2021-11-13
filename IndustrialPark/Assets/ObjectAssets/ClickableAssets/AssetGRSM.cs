using AssetEditorColors;
using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class GrassMeshVertex
    {
        public AssetSingle X { get; set; }
        public AssetSingle Y { get; set; }
        public AssetSingle Z { get; set; }
        public AssetSingle Height { get; set; }
        public AssetSingle NormalX { get; set; }
        public AssetSingle NormalY { get; set; }
        public AssetSingle NormalZ { get; set; }
        public AssetColor Color { get; set; }

        public GrassMeshVertex()
        {
        }

        public GrassMeshVertex(EndianBinaryReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            Height = reader.ReadSingle();
            NormalX = reader.ReadSingle();
            NormalY = reader.ReadSingle();
            NormalZ = reader.ReadSingle();
            Color = reader.ReadColor();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(X);
                writer.Write(Y);
                writer.Write(Z);
                writer.Write(Height);
                writer.Write(NormalX);
                writer.Write(NormalY);
                writer.Write(NormalZ);
                writer.Write(Color);
                return writer.ToArray();
            }
        }
    }

    public class AssetGRSM : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Grass Mesh";

        [Category(categoryName)]
        public AssetSingle MinX { get; set; }
        [Category(categoryName)]
        public AssetSingle MinY { get; set; }
        [Category(categoryName)]
        public AssetSingle MinZ { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxX { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxY { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxZ { get; set; }
        [Category(categoryName)]
        public GrassMeshVertex[] Vertices { get; set; }
        [Category(categoryName)]
        public DashTrackPortal[] Triangles { get; set; }

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public AssetGRSM(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                var numVertices = reader.ReadUInt32();
                var numTriangles = reader.ReadUInt32();

                MinX = reader.ReadSingle();
                MinY = reader.ReadSingle();
                MinZ = reader.ReadSingle();
                MaxX = reader.ReadSingle();
                MaxY = reader.ReadSingle();
                MaxZ = reader.ReadSingle();

                Vertices = new GrassMeshVertex[numVertices];
                for (int i = 0; i < Vertices.Length; i++)
                    Vertices[i] = new GrassMeshVertex(reader);

                Triangles = new DashTrackPortal[numTriangles];
                for (int i = 0; i < Triangles.Length; i++)
                    Triangles[i] = new DashTrackPortal(reader);

                CreateTransformMatrix();
                ArchiveEditorFunctions.AddToRenderableAssets(this);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(Vertices.Length);
                writer.Write(Triangles.Length);

                writer.Write(MinX);
                writer.Write(MinY);
                writer.Write(MinZ);
                writer.Write(MaxX);
                writer.Write(MaxY);
                writer.Write(MaxZ);

                foreach (var v in Vertices)
                    writer.Write(v.Serialize(endianness));

                foreach (var t in Triangles)
                    writer.Write(t.Serialize(endianness));

                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public void CreateTransformMatrix()
        {
            if (Vertices.Length == 0)
                boundingBox = new BoundingBox();
            else
                boundingBox = new BoundingBox(
                    new Vector3(Vertices[0].X, Vertices[0].Y, Vertices[0].Z),
                    new Vector3(Vertices[0].X, Vertices[0].Y, Vertices[0].Z));

            var verticesCT = new List<VertexColoredTextured>(Vertices.Length);

            foreach (var v in Vertices)
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

                verticesCT.Add(new VertexColoredTextured(
                    new Vector3(v.X, v.Y, v.Z),
                    new Vector2(0, 0),
                    //new Vector3(v.NormalX, v.NormalY, v.NormalZ),
                    new Color(v.Color.R, v.Color.G, v.Color.B, v.Color.A)));
            }

            triangles = new List<Models.Triangle>(Triangles.Length);

            foreach (var T in Triangles)
                triangles.Add(new Models.Triangle
                {
                    vertex1 = T.Vertex1,
                    vertex2 = T.Vertex2,
                    vertex3 = T.Vertex3,

                    normal1 = T.Vertex1,
                    normal2 = T.Vertex2,
                    normal3 = T.Vertex3,

                    Color1 = T.Vertex1,
                    Color2 = T.Vertex2,
                    Color3 = T.Vertex3
                });

            vertices = new List<Vector3>(Vertices.Length);
            foreach (var v in verticesCT)
                vertices.Add((Vector3)v.Position);

            Setup(Program.MainForm.renderer, verticesCT);
        }

        private void Setup(SharpRenderer renderer, List<VertexColoredTextured> vertices)
        {
            Dispose();

            var indices = new List<int>(triangles.Count * 3);
            foreach (var t in triangles)
            {
                indices.Add(t.vertex1);
                indices.Add(t.vertex2);
                indices.Add(t.vertex3);
            }

            mesh = SharpMesh.Create(renderer.device, vertices.ToArray(), indices.ToArray(),
                new List<SharpSubSet>() { new SharpSubSet(0, indices.Count, SharpRenderer.whiteDefault) }, SharpDX.Direct3D.PrimitiveTopology.TriangleList);
        }

        public float GetDistanceFrom(Vector3 position) => Vector3.Distance(position, boundingBox.Center);

        public static bool dontRender = false;

        private SharpMesh mesh;
        private BoundingBox boundingBox;

        private List<Vector3> vertices;
        private List<Models.Triangle> triangles;

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

        private UvAnimRenderData renderData;

        public void Draw(SharpRenderer renderer)
        {
            renderData.worldViewProjection = renderer.viewProjection;
            renderData.UvAnimOffset = Vector4.Zero;
            if (isSelected)
                renderData.Color = renderer.selectedObjectColor;
            else
                renderData.Color = Vector4.One;

            renderer.device.UpdateData(renderer.tintedBuffer, renderData);
            renderer.device.DeviceContext.VertexShader.SetConstantBuffer(0, renderer.tintedBuffer);
            renderer.tintedShader.Apply();

            renderer.device.SetBlendStateAlphaBlend();
            renderer.device.UpdateAllStates();

            mesh.Begin(renderer.device);
            mesh.Draw(renderer.device, 0);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (ShouldDraw(renderer) && ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray, triangles, vertices, Matrix.Identity);
            return null;
        }

        public void Dispose()
        {
            RenderWareModelFile.completeMeshList.Remove(mesh);
            if (mesh != null)
                mesh.Dispose();
        }

        public BoundingBox GetBoundingBox() => boundingBox;

        [Browsable(false)]
        public AssetSingle PositionX { get => 0; set { } }
        [Browsable(false)]
        public AssetSingle PositionY { get => 0; set { } }
        [Browsable(false)]
        public AssetSingle PositionZ { get => 0; set { } }
    }
}