using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DashTrackVertex
    {
        public AssetSingle X { get; set; }
        public AssetSingle Y { get; set; }
        public AssetSingle Z { get; set; }

        public DashTrackVertex()
        {
        }

        public DashTrackVertex(EndianBinaryReader reader)
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
    }

    public class DashTrackTriangle
    {
        public ushort Vertex1 { get; set; }
        public ushort Vertex2 { get; set; }
        public ushort Vertex3 { get; set; }
        public ushort Flags { get; set; }
        public AssetSingle U1 { get; set; }
        public AssetSingle V1 { get; set; }
        public AssetSingle U2 { get; set; }
        public AssetSingle V2 { get; set; }
        public AssetSingle U3 { get; set; }
        public AssetSingle V3 { get; set; }

        public DashTrackTriangle()
        {
        }

        public DashTrackTriangle(EndianBinaryReader reader)
        {
            Vertex1 = reader.ReadUInt16();
            Vertex2 = reader.ReadUInt16();
            Vertex3 = reader.ReadUInt16();
            Flags = reader.ReadUInt16();
            U1 = reader.ReadSingle();
            U2 = reader.ReadSingle();
            U3 = reader.ReadSingle();
            V1 = reader.ReadSingle();
            V2 = reader.ReadSingle();
            V3 = reader.ReadSingle();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Vertex1);
                writer.Write(Vertex2);
                writer.Write(Vertex3);
                writer.Write(Flags);
                writer.Write(U1);
                writer.Write(U2);
                writer.Write(U3);
                writer.Write(V1);
                writer.Write(V2);
                writer.Write(V3);
                return writer.ToArray();
            }
        }
    }

    public class DashTrackPortal
    {
        public ushort Vertex1 { get; set; }
        public ushort Vertex2 { get; set; }
        public ushort Vertex3 { get; set; }

        public DashTrackPortal()
        {
        }

        public DashTrackPortal(EndianBinaryReader reader)
        {
            Vertex1 = reader.ReadUInt16();
            Vertex2 = reader.ReadUInt16();
            Vertex3 = reader.ReadUInt16();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Vertex1);
                writer.Write(Vertex2);
                writer.Write(Vertex3);
                return writer.ToArray();
            }
        }
    }

    public class AssetDTRK : BaseAsset, IRenderableAsset, IClickableAsset
    {
        private const string categoryName = "Dash Track";

        [Category(categoryName)]
        public uint LandableStart { get; set; }
        [Category(categoryName)]
        public uint LeavableStart { get; set; }
        [Category(categoryName)]
        public AssetID Unknown1 { get; set; }
        [Category(categoryName)]
        public AssetID Unknown2 { get; set; }
        [Category(categoryName)]
        public AssetID Unknown3 { get; set; }
        [Category(categoryName)]
        public DashTrackVertex[] Vertices { get; set; }
        [Category(categoryName)]
        public DashTrackTriangle[] Triangles { get; set; }
        [Category(categoryName)]
        public DashTrackPortal[] Portals { get; set; }
        [Category(categoryName)]
        public uint LastTriangle { get; set; }
        [Category(categoryName)]
        public AssetSingle LastPositionX { get; set; }
        [Category(categoryName)]
        public AssetSingle LastPositionY { get; set; }

        [Browsable(false)]
        public bool SpecialBlendMode => true;

        public AssetDTRK(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                var numVertices = reader.ReadUInt32();
                var numTriangles = reader.ReadUInt32();
                LandableStart = reader.ReadUInt32();
                LeavableStart = reader.ReadUInt32();
                Unknown1 = reader.ReadUInt32();
                Unknown2 = reader.ReadUInt32();
                Unknown3 = reader.ReadUInt32();

                Vertices = new DashTrackVertex[numVertices];
                for (int i = 0; i < Vertices.Length; i++)
                    Vertices[i] = new DashTrackVertex(reader);

                Triangles = new DashTrackTriangle[numTriangles];
                for (int i = 0; i < Triangles.Length; i++)
                    Triangles[i] = new DashTrackTriangle(reader);

                Portals = new DashTrackPortal[numTriangles];
                for (int i = 0; i < Portals.Length; i++)
                    Portals[i] = new DashTrackPortal(reader);

                LastTriangle = reader.ReadUInt32();
                LastPositionX = reader.ReadSingle();
                LastPositionY = reader.ReadSingle();

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
                writer.Write(LandableStart);
                writer.Write(LeavableStart);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);

                foreach (var v in Vertices)
                {
                    writer.Write(v.X);
                    writer.Write(v.Y);
                    writer.Write(v.Z);
                }

                foreach (var t in Triangles)
                    writer.Write(t.Serialize(endianness));

                foreach (var p in Portals)
                    writer.Write(p.Serialize(endianness));
                writer.Write(LastTriangle);
                writer.Write(LastPositionX);
                writer.Write(LastPositionY);
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

            var vertices = new List<VertexColoredTextured>(Vertices.Length);

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

                vertices.Add(new VertexColoredTextured(new Vector3(v.X, v.Y, v.Z), new Vector2(0, 0), new Color(255, 255, 255, 0)));
            }

            triangles = new List<Models.Triangle>(Triangles.Length);

            foreach (var T in Triangles)
            {
                var t = new Models.Triangle();

                t.vertex1 = T.Vertex1;
                if (vertices[T.Vertex1].Color.A == 0)
                {
                    var v = vertices[T.Vertex1];

                    v.TextureCoordinate.X = T.U1;
                    v.TextureCoordinate.Y = T.V1;
                    v.Color.A = 255;

                    vertices[T.Vertex1] = v;
                }
                else
                {
                    var v = vertices[T.Vertex1];

                    if ((v.TextureCoordinate.X != vertices[T.Vertex1].TextureCoordinate.X) || (v.TextureCoordinate.Y != vertices[T.Vertex1].TextureCoordinate.Y))
                    {
                        v.TextureCoordinate.X = T.U1;
                        v.TextureCoordinate.Y = T.V1;
                        t.vertex1 = vertices.Count;
                        vertices.Add(v);
                    }
                }

                t.vertex2 = T.Vertex2;
                if (vertices[T.Vertex2].Color.A == 0)
                {
                    var v = vertices[T.Vertex2];

                    v.TextureCoordinate.X = T.U2;
                    v.TextureCoordinate.Y = T.V2;
                    v.Color.A = 255;

                    vertices[T.Vertex2] = v;
                }
                else
                {
                    var v = vertices[T.Vertex2];

                    if ((v.TextureCoordinate.X != vertices[T.Vertex2].TextureCoordinate.X) || (v.TextureCoordinate.Y != vertices[T.Vertex2].TextureCoordinate.Y))
                    {
                        v.TextureCoordinate.X = T.U2;
                        v.TextureCoordinate.Y = T.V2;
                        t.vertex2 = vertices.Count;
                        vertices.Add(v);
                    }
                }

                t.vertex3 = T.Vertex3;
                if (vertices[T.Vertex3].Color.A == 0)
                {
                    var v = vertices[T.Vertex3];

                    v.TextureCoordinate.X = T.U3;
                    v.TextureCoordinate.Y = T.V3;
                    v.Color.A = 255;

                    vertices[T.Vertex3] = v;
                }
                else
                {
                    var v = vertices[T.Vertex3];

                    if ((v.TextureCoordinate.X != vertices[T.Vertex3].TextureCoordinate.X) || (v.TextureCoordinate.Y != vertices[T.Vertex3].TextureCoordinate.Y))
                    {
                        v.TextureCoordinate.X = T.U3;
                        v.TextureCoordinate.Y = T.V3;
                        t.vertex3 = vertices.Count;
                        vertices.Add(v);
                    }
                }

                triangles.Add(t);
            }

            this.vertices = new List<Vector3>(Vertices.Length);
            foreach (var v in vertices)
                this.vertices.Add((Vector3)v.Position);
            
            Setup(Program.MainForm.renderer, vertices);
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
                new List<SharpSubSet>() { new SharpSubSet(0, indices.Count, SharpRenderer.arrowDefault) }, SharpDX.Direct3D.PrimitiveTopology.TriangleList);
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