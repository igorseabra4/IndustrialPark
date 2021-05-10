using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HipHopFile;
using SharpDX;
using SharpDX.Direct3D11;

namespace IndustrialPark
{
    public struct Line
    {
        public ushort Vertex0 { get; set; }
        public ushort Vertex1 { get; set; }

        public Line(ushort vertex0, ushort vertex1)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
        }

        public Line(EndianBinaryReader reader)
        {
            Vertex0 = reader.ReadUInt16();
            Vertex1 = reader.ReadUInt16();
        }

        public byte[] Serialize(Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(Vertex0);
            writer.Write(Vertex1);
            return writer.ToArray();
        }

        public override string ToString()
        {
            return $"[{Vertex0}, {Vertex1}]";
        }
    }

    public class AssetWIRE : Asset, IRenderableAsset
    {
        private const string categoryName = "Wireframe Model";

        [Category(categoryName)]
        public WireVector[] Points { get; set; }
        [Category(categoryName)]
        public Line[] Lines { get; set; }
        [Category(categoryName)]
        public AssetID hashID0 { get; set; }
        [Category(categoryName)]
        public AssetID hashID1 { get; set; }

        public AssetWIRE(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);

            reader.ReadInt32();
            int vertexAmount = reader.ReadInt32();
            int lineAmount = reader.ReadInt32();
            hashID0 = reader.ReadUInt32();
            hashID1 = reader.ReadUInt32();

            Points = new WireVector[vertexAmount];
            for (int i = 0; i < Points.Length; i++)
                Points[i] = new WireVector(reader);

            Lines = new Line[lineAmount];
            for (int i = 0; i < Lines.Length; i++)
                Lines[i] = new Line(reader);

            Setup(renderer);
            ArchiveEditorFunctions.AddToRenderableAssets(this);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(hashID0);
            writer.Write(hashID1);

            foreach (var p in Points)
                writer.Write(p.Serialize(endianness));

            foreach (var l in Lines)
                writer.Write(l.Serialize(endianness));

            writer.BaseStream.Position = 0;

            writer.Write((int)writer.BaseStream.Length);
            writer.Write(Points.Length);
            writer.Write(Lines.Length);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => hashID0 == assetID || hashID1 == assetID || base.HasReference(assetID);

        public void ToObj(string fileName)
        {
            using (var objFile = new StreamWriter(new FileStream(fileName, FileMode.Create)))
            {
                objFile.WriteLine("# Created by Industrial Park");
                objFile.WriteLine();
                foreach (var v in Points)
                    objFile.WriteLine($"v {v.X} {v.Y} {v.Z}");
                objFile.WriteLine();
                objFile.WriteLine("o wireframe_01");
                foreach (var v in Lines)
                    objFile.WriteLine($"l {v.Vertex0 + 1} {v.Vertex1 + 1}");
            }
        }

        public void FromObj(string fileName)
        {
            Line FromFace(IList<string> face, int v0, int v1) => new Line(
                (ushort)(Convert.ToUInt16(face[v0].Split('/')[0]) - 1),
                (ushort)(Convert.ToUInt16(face[v1].Split('/')[0]) - 1));

            Line FromSpline(IList<string> face, int v0, int v1) => new Line(
                (ushort)(Convert.ToUInt16(face[v0]) - 1),
                (ushort)(Convert.ToUInt16(face[v1]) - 1));

            var vertices = new List<WireVector>();
            var lines = new List<Line>();
            string[] objFile = File.ReadAllLines(fileName);
            foreach (var s in objFile)
                if (s.StartsWith("v "))
                {
                    string[] a = Regex.Replace(s, @"\s+", " ").Split(' ');
                    vertices.Add(new WireVector(Convert.ToSingle(a[1]), Convert.ToSingle(a[2]), Convert.ToSingle(a[3])));
                }
                else if (s.StartsWith("f "))
                {
                    var a = s.Split(' ').ToList();
                    if (a.Last() == "")
                        a.RemoveAt(a.Count - 1);

                    for (int i = 1; i < a.Count; i++)
                        for (int j = 1; j < a.Count; j++)
                            if (i < j)
                                lines.Add(FromFace(a, i, j));
                }
                else if (s.StartsWith("l "))
                {
                    var a = s.Split(' ').ToList();
                    if (a.Last() == "")
                        a.RemoveAt(a.Count - 1);

                    for (int i = 1; i < a.Count - 1; i++)
                        lines.Add(FromSpline(a, i, i + 1));
                }
            Points = vertices.ToArray();
            Lines = lines.ToArray();
        }

        private SharpDX.Direct3D11.Buffer vertexBuffer;
        private int vertexCount;

        public void Setup(SharpRenderer renderer)
        {
            renderer.completeVertexBufferList.Remove(vertexBuffer);
            if (vertexBuffer != null)
                vertexBuffer.Dispose();
            List<WireVector> lineList = new List<WireVector>();
            foreach (Line l in Lines)
            {
                lineList.Add(Points[l.Vertex0]);
                lineList.Add(Points[l.Vertex1]);
            }
            vertexBuffer = SharpDX.Direct3D11.Buffer.Create(renderer.device.Device, BindFlags.VertexBuffer, lineList.ToArray());
            renderer.completeVertexBufferList.Add(vertexBuffer);
            vertexCount = lineList.Count;
        }

        public bool ShouldDraw(SharpRenderer renderer) => isSelected;
        
        public void Draw(SharpRenderer renderer)
        {
            renderer.DrawSpline(vertexBuffer, vertexCount, Matrix.Identity, Color.YellowGreen.ToVector4(), true);
        }
        
        public float GetDistanceFrom(Vector3 cameraPosition) => cameraPosition.Length();
        
        public void Dispose()
        {
            vertexBuffer.Dispose();
        }

        public void CreateTransformMatrix()
        {
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray) => null;

        [Browsable(false)]
        public bool SpecialBlendMode => false;
    }
}