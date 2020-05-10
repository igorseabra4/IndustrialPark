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
        public ushort vertex0 { get; set; }
        public ushort vertex1 { get; set; }

        public Line(ushort vertex0, ushort vertex1)
        {
            this.vertex0 = vertex0;
            this.vertex1 = vertex1;
        }

        public override string ToString()
        {
            return $"[{vertex0}, {vertex1}]";
        }
    }

    public struct WireVector
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public WireVector(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }
    }

    public class AssetWIRE : Asset, IRenderableAsset
    {
        public AssetWIRE(Section_AHDR AHDR, Game game, Platform platform, SharpRenderer renderer) : base(AHDR, game, platform)
        {
            Setup(renderer);
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssetSetCommon.Add(this);
        }

        public override bool HasReference(uint assetID) => hashID0 == assetID || hashID1 == assetID || base.HasReference(assetID);

        [Category("Wireframe Model"), ReadOnly(true)]
        public int totalFileSize
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }

        [Category("Wireframe Model"), ReadOnly(true)]
        public int vertexAmount
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }
        
        [Category("Wireframe Model"), ReadOnly(true)]
        public int lineAmount
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category("Wireframe Model")]
        public AssetID hashID0
        {
            get => ReadUInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Wireframe Model")]
        public AssetID hashID1
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }

        private const int vertexStart = 0x14;
        private const int vertexSize = 0xC;

        [Category("Wireframe Model")]
        public WireVector[] Points
        {
            get
            {
                var vertices = new WireVector[vertexAmount];
                for (int i = 0; i < vertexAmount; i++)
                    vertices[i] = new WireVector(
                        ReadFloat(vertexStart + i * vertexSize),
                        ReadFloat(vertexStart + i * vertexSize + 4),
                        ReadFloat(vertexStart + i * vertexSize + 8));
                
                return vertices;
            }
            set
            {
                List<byte> data = Data.Take(vertexStart).ToList();
                foreach (var v in value)
                {
                    data.AddRange(BitConverter.GetBytes(Switch(v.X)));
                    data.AddRange(BitConverter.GetBytes(Switch(v.Y)));
                    data.AddRange(BitConverter.GetBytes(Switch(v.Z)));
                }
                data.AddRange(Data.Skip(lineStart));
                Data = data.ToArray();
                vertexAmount = value.Length;
                totalFileSize = Data.Length;
            }
        }

        private int lineStart => vertexStart + vertexAmount * vertexSize;
        private const int lineSize = 0x4;

        [Category("Wireframe Model")]
        public Line[] Lines
        {
            get
            {
                var lines = new Line[lineAmount];
                for (int i = 0; i < lineAmount; i++)
                    lines[i] = new Line(ReadUShort(lineStart + i * lineSize), ReadUShort(lineStart + i * lineSize + 2));
                return lines;
            }
            set
            {
                List<byte> data = Data.Take(lineStart).ToList();
                foreach (var l in value)
                {
                    data.AddRange(BitConverter.GetBytes(Switch(l.vertex0)));
                    data.AddRange(BitConverter.GetBytes(Switch(l.vertex1)));
                }
                Data = data.ToArray();
                lineAmount = value.Length;
                totalFileSize = Data.Length;
            }
        }

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
                    objFile.WriteLine($"l {v.vertex0 + 1} {v.vertex1 + 1}");
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

        private BoundingBox boundingBox;

        public static bool dontRender = false;

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
                lineList.Add(Points[l.vertex0]);
                lineList.Add(Points[l.vertex1]);
            }
            vertexBuffer = SharpDX.Direct3D11.Buffer.Create(renderer.device.Device, BindFlags.VertexBuffer, lineList.ToArray());
            renderer.completeVertexBufferList.Add(vertexBuffer);
            vertexCount = lineList.Count;
        }

        public void Draw(SharpRenderer renderer)
        {
            if (isSelected)
                renderer.DrawSpline(vertexBuffer, vertexCount, Matrix.Identity, Color.YellowGreen.ToVector4(), true);
        }

        public void CreateTransformMatrix()
        {
            if (Points.Length == 0)
                boundingBox = new BoundingBox();
            else
                boundingBox = new BoundingBox(
                    new Vector3(Points[0].X, Points[0].Y, Points[0].Z),
                    new Vector3(Points[0].X, Points[0].Y, Points[0].Z));

            foreach (WireVector v in Points)
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

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, boundingBox.Center);
        }

        public float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return distance;
            return null;
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
        }
    }
}