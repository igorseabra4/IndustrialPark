using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;

namespace IndustrialPark
{
    public partial class OBJFunctions
    {
        public struct Vertex
        {
            public Vector3 Position;
            public SharpDX.Color Color;
            public Vector2 TexCoord;

            public bool HasUV;
            public bool HasColor;
        }

        public class Triangle
        {
            public int MaterialIndex;

            public int Vertex1;
            public int Vertex2;
            public int Vertex3;

            public int UVCoord1;
            public int UVCoord2;
            public int UVCoord3;

            public int Color1;
            public int Color2;
            public int Color3;
        }

        public static bool flipUVs = false;

        public struct ModelConverterData
        {
            public List<string> MaterialStream;
            public List<Vertex> VertexStream;
            public List<Vector2> UVStream;
            public List<SharpDX.Color> ColorStream;
            public List<Triangle> TriangleStream;
            public string MTLLib;
        }
        
        public static ModelConverterData ReadOBJFile(string InputFile)
        {
            ModelConverterData objData = new ModelConverterData()
            {
                MaterialStream = new List<string>(),
                VertexStream = new List<Vertex>(),
                UVStream = new List<Vector2>(),
                ColorStream = new List<SharpDX.Color>(),
                TriangleStream = new List<Triangle>(),
                MTLLib = null
            };

            string[] OBJFile = File.ReadAllLines(InputFile);

            int CurrentMaterial = -1;

            List<SharpDX.Color> ColorStream = new List<SharpDX.Color>();

            bool hasUVCoords = true;

            foreach (string j in OBJFile)
            {
                if (j.Length > 2)
                {
                    if (j.StartsWith("v "))
                    {
                        string a = Regex.Replace(j, @"\s+", " ");

                        string[] SubStrings = a.Split(' ');
                        Vertex TempVertex = new Vertex();
                        TempVertex.Position.X = Convert.ToSingle(SubStrings[1]);
                        TempVertex.Position.Y = Convert.ToSingle(SubStrings[2]);
                        TempVertex.Position.Z = Convert.ToSingle(SubStrings[3]);

                        TempVertex.Color = SharpDX.Color.White;

                        objData.VertexStream.Add(TempVertex);
                    }
                    else if (j.Substring(0, 3) == "vt ")
                    {
                        string[] SubStrings = j.Split(' ');
                        Vector2 TempUV = new Vector2
                        {
                            X = Convert.ToSingle(SubStrings[1]),
                            Y = Convert.ToSingle(SubStrings[2])
                        };
                        objData.UVStream.Add(TempUV);
                    }
                    else if (j.Substring(0, 3) == "vc ") // Special code
                    {
                        string[] SubStrings = j.Split(' ');
                        SharpDX.Color TempColor = new SharpDX.Color
                        {
                            R = Convert.ToByte(SubStrings[1]),
                            G = Convert.ToByte(SubStrings[2]),
                            B = Convert.ToByte(SubStrings[3]),
                            A = Convert.ToByte(SubStrings[4])
                        };

                        ColorStream.Add(TempColor);
                    }
                    else if (j.StartsWith("f "))
                    {
                        string[] SubStrings = j.Split(' ');

                        if (SubStrings[1].Split('/').Count() == 1 & hasUVCoords)
                        {
                            MessageBox.Show("Apparently you're trying to import an object which doesn't have texture coordinates.");
                            hasUVCoords = false;
                            objData.UVStream = new List<Vector2>() { new Vector2() };
                        }

                        Triangle TempTriangle = new Triangle
                        {
                            MaterialIndex = CurrentMaterial,
                            Vertex1 = Convert.ToInt32(SubStrings[1].Split('/')[0]) - 1,
                            Vertex2 = Convert.ToInt32(SubStrings[2].Split('/')[0]) - 1,
                            Vertex3 = Convert.ToInt32(SubStrings[3].Split('/')[0]) - 1
                        };
                        if (hasUVCoords)
                        {
                            TempTriangle.UVCoord1 = Convert.ToInt32(SubStrings[1].Split('/')[1]) - 1;
                            TempTriangle.UVCoord2 = Convert.ToInt32(SubStrings[2].Split('/')[1]) - 1;
                            TempTriangle.UVCoord3 = Convert.ToInt32(SubStrings[3].Split('/')[1]) - 1;
                        }

                        objData.TriangleStream.Add(TempTriangle);
                    }
                    else if (j.Length > 7)
                        if (j.Substring(0, 7) == "usemtl ")
                        {
                            objData.MaterialStream.Add(Regex.Replace(j.Substring(7), @"\s+", ""));
                            CurrentMaterial += 1;
                        }
                        else if (j.Substring(0, 7) == "mtllib ")
                            objData.MTLLib = j.Substring(7).Split('\\').LastOrDefault();
                }
            }

            // Special code
            if (ColorStream.Count == objData.VertexStream.Count)
                for (int i = 0; i < objData.VertexStream.Count; i++)
                {
                    Vertex v = objData.VertexStream[i];
                    v.Color = ColorStream[i];
                    objData.VertexStream[i] = v;
                }

            try
            {
                objData.MaterialStream = ReplaceMaterialNames(InputFile, objData.MTLLib, objData.MaterialStream);
            }
            catch
            {
                MessageBox.Show("Unable to load material lib. Will use material names as texture names.");
            }
            
            return FixUVCoords(objData);
        }

        public static List<string> ReplaceMaterialNames(string InputOBJFile, string MTLLib, List<string> MaterialList)
        {
            string[] MTLFile = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(InputOBJFile), MTLLib));

            string MaterialName = "";
            string TextureName = "";

            foreach (string j in MTLFile)
            {
                string a = Regex.Replace(j, @"\s+", "");

                if (a.StartsWith("newmtl"))
                {
                    MaterialName = a.Substring(6);
                }
                else if (a.StartsWith("map_Kd"))
                {
                    TextureName = Path.GetFileNameWithoutExtension(a.Substring(6));
                    for (int k = 0; k < MaterialList.Count; k++)
                    {
                        if (MaterialList[k] == MaterialName)
                        {
                            MaterialList[k] = TextureName;
                        }
                    }
                }
            }

            return MaterialList;
        }

        public static ModelConverterData FixUVCoords(ModelConverterData data)
        {
            for (int i = 0; i < data.TriangleStream.Count; i++)
            {
                if (data.VertexStream[data.TriangleStream[i].Vertex1].HasUV == false)
                {
                    Vertex TempVertex = data.VertexStream[data.TriangleStream[i].Vertex1];

                    TempVertex.TexCoord.X = data.UVStream[data.TriangleStream[i].UVCoord1].X;
                    TempVertex.TexCoord.Y = data.UVStream[data.TriangleStream[i].UVCoord1].Y;
                    TempVertex.HasUV = true;
                    data.VertexStream[data.TriangleStream[i].Vertex1] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = data.VertexStream[data.TriangleStream[i].Vertex1];

                    if ((TempVertex.TexCoord.X != data.UVStream[data.TriangleStream[i].UVCoord1].X) | (TempVertex.TexCoord.Y != data.UVStream[data.TriangleStream[i].UVCoord1].Y))
                    {
                        TempVertex.TexCoord.X = data.UVStream[data.TriangleStream[i].UVCoord1].X;
                        TempVertex.TexCoord.Y = data.UVStream[data.TriangleStream[i].UVCoord1].Y;

                        Triangle TempTriangle = data.TriangleStream[i];
                        TempTriangle.Vertex1 = data.VertexStream.Count;
                        data.TriangleStream[i] = TempTriangle;
                        data.VertexStream.Add(TempVertex);
                    }
                }
                if (data.VertexStream[data.TriangleStream[i].Vertex2].HasUV == false)
                {
                    Vertex TempVertex = data.VertexStream[data.TriangleStream[i].Vertex2];

                    TempVertex.TexCoord.X = data.UVStream[data.TriangleStream[i].UVCoord2].X;
                    TempVertex.TexCoord.Y = data.UVStream[data.TriangleStream[i].UVCoord2].Y;
                    TempVertex.HasUV = true;
                    data.VertexStream[data.TriangleStream[i].Vertex2] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = data.VertexStream[data.TriangleStream[i].Vertex2];

                    if ((TempVertex.TexCoord.X != data.UVStream[data.TriangleStream[i].UVCoord2].X) | (TempVertex.TexCoord.Y != data.UVStream[data.TriangleStream[i].UVCoord2].Y))
                    {
                        TempVertex.TexCoord.X = data.UVStream[data.TriangleStream[i].UVCoord2].X;
                        TempVertex.TexCoord.Y = data.UVStream[data.TriangleStream[i].UVCoord2].Y;

                        Triangle TempTriangle = data.TriangleStream[i];
                        TempTriangle.Vertex2 = data.VertexStream.Count;
                        data.TriangleStream[i] = TempTriangle;
                        data.VertexStream.Add(TempVertex);
                    }
                }
                if (data.VertexStream[data.TriangleStream[i].Vertex3].HasUV == false)
                {
                    Vertex TempVertex = data.VertexStream[data.TriangleStream[i].Vertex3];

                    TempVertex.TexCoord.X = data.UVStream[data.TriangleStream[i].UVCoord3].X;
                    TempVertex.TexCoord.Y = data.UVStream[data.TriangleStream[i].UVCoord3].Y;
                    TempVertex.HasUV = true;
                    data.VertexStream[data.TriangleStream[i].Vertex3] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = data.VertexStream[data.TriangleStream[i].Vertex3];

                    if ((TempVertex.TexCoord.X != data.UVStream[data.TriangleStream[i].UVCoord3].X) | (TempVertex.TexCoord.Y != data.UVStream[data.TriangleStream[i].UVCoord3].Y))
                    {
                        TempVertex.TexCoord.X = data.UVStream[data.TriangleStream[i].UVCoord3].X;
                        TempVertex.TexCoord.Y = data.UVStream[data.TriangleStream[i].UVCoord3].Y;

                        Triangle TempTriangle = data.TriangleStream[i];
                        TempTriangle.Vertex3 = data.VertexStream.Count;
                        data.TriangleStream[i] = TempTriangle;
                        data.VertexStream.Add(TempVertex);
                    }
                }
            }

            return data;
        }

        private static List<List<int>> GenerateTristrips(List<Triangle> triangleStream2)
        {
            List<List<int>> indexLists = new List<List<int>>
            {
                new List<int>()
            };
            indexLists.Last().Add(triangleStream2[0].Vertex1);
            indexLists.Last().Add(triangleStream2[0].Vertex2);
            indexLists.Last().Add(triangleStream2[0].Vertex3);
            triangleStream2[0].MaterialIndex = -1;

            bool allAreDone = false;

            while (!allAreDone)
            {
                bool inverted = false;

                for (int i = 0; i < triangleStream2.Count(); i++)
                {
                    if (triangleStream2[i].MaterialIndex == -1) continue;

                    if (!inverted)
                    {
                        if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].Vertex1 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].Vertex2)
                        {
                            indexLists.Last().Add(triangleStream2[i].Vertex3);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].Vertex2 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].Vertex3)
                        {
                            indexLists.Last().Add(triangleStream2[i].Vertex1);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].Vertex3 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].Vertex1)
                        {
                            indexLists.Last().Add(triangleStream2[i].Vertex2);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                    }
                    else
                    {
                        if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].Vertex2 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].Vertex1)
                        {
                            indexLists.Last().Add(triangleStream2[i].Vertex3);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].Vertex3 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].Vertex2)
                        {
                            indexLists.Last().Add(triangleStream2[i].Vertex1);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].Vertex1 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].Vertex3)
                        {
                            indexLists.Last().Add(triangleStream2[i].Vertex2);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                    }
                }

                allAreDone = true;

                for (int i = 0; i < triangleStream2.Count(); i++)
                {
                    if (triangleStream2[i].MaterialIndex == -1)
                        continue;
                    else
                    {
                        indexLists.Add(new List<int>());
                        indexLists.Last().Add(triangleStream2[i].Vertex1);
                        indexLists.Last().Add(triangleStream2[i].Vertex2);
                        indexLists.Last().Add(triangleStream2[i].Vertex3);
                        triangleStream2[i].MaterialIndex = -1;
                        allAreDone = false;
                        break;
                    }
                }
            }

            return indexLists;
        }
        
        public static void ConvertBSPtoOBJ(string fileName, RenderWareModelFile bspFile)
        {
            int totalVertexIndices = 0;

            string materialLibrary = Path.ChangeExtension(fileName, "MTL");
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            StreamWriter OBJWriter = new StreamWriter((Path.ChangeExtension(fileName, "OBJ")), false);

            List<Triangle> triangleList = new List<Triangle>();

            foreach (RWSection rw in bspFile.rwChunkList)
            {
                if (rw is World_000B w)
                {
                    OBJWriter.WriteLine("# Exported by Heroes Power Plant");
                    OBJWriter.WriteLine("mtllib " + Path.GetFileName(materialLibrary));
                    OBJWriter.WriteLine();
                    if (w.firstWorldChunk.sectionIdentifier == Section.AtomicSector)
                    {
                        GetAtomicTriangleList(OBJWriter, (AtomicSector_0009)w.firstWorldChunk, ref triangleList, ref totalVertexIndices);
                    }
                    else if (w.firstWorldChunk.sectionIdentifier == Section.PlaneSector)
                    {
                        GetPlaneTriangleList(OBJWriter, (PlaneSector_000A)w.firstWorldChunk, ref triangleList, ref totalVertexIndices);
                    }
                }
            }

            for (int i = 0; i < bspFile.MaterialList.Count; i++)
            {
                OBJWriter.WriteLine("g " + fileNameWithoutExtension + "_" + bspFile.MaterialList[i]);
                OBJWriter.WriteLine("usemtl " + bspFile.MaterialList[i] + "_m");

                if (bspFile.isCollision)
                {
                    foreach (Triangle j in triangleList)
                        if (j.MaterialIndex == i)
                            OBJWriter.WriteLine("f "
                                + (j.Vertex1 + 1).ToString() + " "
                                + (j.Vertex2 + 1).ToString() + " "
                                + (j.Vertex3 + 1).ToString());
                }
                else
                foreach (Triangle j in triangleList)
                    if (j.MaterialIndex == i)
                        OBJWriter.WriteLine("f "
                            + (j.Vertex1 + 1).ToString() + "/" + (j.Vertex1 + 1).ToString() + " "
                            + (j.Vertex2 + 1).ToString() + "/" + (j.Vertex2 + 1).ToString() + " "
                            + (j.Vertex3 + 1).ToString() + "/" + (j.Vertex3 + 1).ToString());

                OBJWriter.WriteLine();
            }

            OBJWriter.Close();
            WriteMaterialLib(bspFile.MaterialList.ToArray(), materialLibrary);
        }

        private static void GetPlaneTriangleList(StreamWriter OBJWriter, PlaneSector_000A PlaneSector, ref List<Triangle> triangleList, ref int totalVertexIndices)
        {
            if (PlaneSector.leftSection is AtomicSector_0009 a1)
            {
                GetAtomicTriangleList(OBJWriter, a1, ref triangleList, ref totalVertexIndices);
            }
            else if (PlaneSector.leftSection is PlaneSector_000A p1)
            {
                GetPlaneTriangleList(OBJWriter, p1, ref  triangleList, ref totalVertexIndices);
            }

            if (PlaneSector.rightSection is AtomicSector_0009 a2)
            {
                GetAtomicTriangleList(OBJWriter, a2, ref triangleList, ref totalVertexIndices);
            }
            else if (PlaneSector.rightSection is PlaneSector_000A p2)
            {
                GetPlaneTriangleList(OBJWriter, p2, ref triangleList, ref totalVertexIndices);
            }
        }

        private static void GetAtomicTriangleList(StreamWriter OBJWriter, AtomicSector_0009 atomicSection, ref List<Triangle> triangleList, ref int totalVertexIndices)
        {
            //Write vertex list to obj
            if (atomicSection.atomicStruct.vertexArray != null)
                foreach (Vertex3 i in atomicSection.atomicStruct.vertexArray)
                    OBJWriter.WriteLine("v " + i.X.ToString() + " " + i.Y.ToString() + " " + i.Z.ToString());

            OBJWriter.WriteLine();

            //Write uv list to obj
            if (atomicSection.atomicStruct.uvArray != null)
            {
                if (flipUVs)
                    foreach (TextCoord i in atomicSection.atomicStruct.uvArray)
                        OBJWriter.WriteLine("vt " + i.X.ToString() + " " + (-i.Y).ToString());
                else
                    foreach (TextCoord i in atomicSection.atomicStruct.uvArray)
                        OBJWriter.WriteLine("vt " + i.X.ToString() + " " + i.Y.ToString());
            }
            OBJWriter.WriteLine();

            // Write vcolors to obj
            if (atomicSection.atomicStruct.colorArray != null)
                foreach (RenderWareFile.Color i in atomicSection.atomicStruct.colorArray)
                OBJWriter.WriteLine("vc " + i.R.ToString() + " " + i.G.ToString() + " " + i.B.ToString() + " " + i.A.ToString());

            OBJWriter.WriteLine();

            if (atomicSection.atomicStruct.triangleArray != null)
                foreach (RenderWareFile.Triangle i in atomicSection.atomicStruct.triangleArray)
                {
                    triangleList.Add(new Triangle
                    {
                        MaterialIndex = i.materialIndex,
                        Vertex1 = i.vertex1 + totalVertexIndices,
                        Vertex2 = i.vertex2 + totalVertexIndices,
                        Vertex3 = i.vertex3 + totalVertexIndices
                    });
                }

            if (atomicSection.atomicStruct.vertexArray != null)
                totalVertexIndices += atomicSection.atomicStruct.vertexArray.Count();
        }

        private static void WriteMaterialLib(string[] MaterialStream, string materialLibrary)
        {
            StreamWriter MTLWriter = new StreamWriter(materialLibrary, false);
            MTLWriter.WriteLine("# Exported by Heroes Power Plant");

            for (int i = 0; i < MaterialStream.Length; i++)
            {
                MTLWriter.WriteLine("newmtl " + MaterialStream[i] + "_m");
                MTLWriter.WriteLine("Ka 0.2 0.2 0.2");
                MTLWriter.WriteLine("Kd 0.8 0.8 0.8");
                MTLWriter.WriteLine("Ks 0 0 0");
                MTLWriter.WriteLine("Ns 10");
                MTLWriter.WriteLine("d 1.0");
                MTLWriter.WriteLine("illum 4");
                MTLWriter.WriteLine("map_Kd " + MaterialStream[i] + ".png");
                MTLWriter.WriteLine();
            }

            MTLWriter.Close();            
        }
    }
}