using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;

namespace IndustrialPark.Models
{
    public class OBJFunctions
    {
        public static bool flipUVs = false;
                
        public static ModelConverterData ReadOBJFile(string InputFile, bool hasUVCoords = true)
        {
            ModelConverterData objData = new ModelConverterData()
            {
                MaterialList = new List<string>(),
                VertexList = new List<Vertex>(),
                UVList = new List<Vector2>(),
                ColorList = new List<SharpDX.Color>(),
                TriangleList = new List<Triangle>(),
                MTLLib = null
            };

            if (!hasUVCoords)
                objData.UVList = new List<Vector2>() { new Vector2() };

            string[] OBJFile = File.ReadAllLines(InputFile);

            int CurrentMaterial = -1;

            List<SharpDX.Color> ColorStream = new List<SharpDX.Color>();
            
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

                        objData.VertexList.Add(TempVertex);
                    }
                    else if (j.Substring(0, 3) == "vt ")
                    {
                        string[] SubStrings = j.Split(' ');
                        Vector2 TempUV = new Vector2
                        {
                            X = Convert.ToSingle(SubStrings[1]),
                            Y = Convert.ToSingle(SubStrings[2])
                        };
                        objData.UVList.Add(TempUV);
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
                            objData.UVList = new List<Vector2>() { new Vector2() };
                        }

                        Triangle TempTriangle = new Triangle
                        {
                            MaterialIndex = CurrentMaterial,
                            vertex1 = Convert.ToInt32(SubStrings[1].Split('/')[0]) - 1,
                            vertex2 = Convert.ToInt32(SubStrings[2].Split('/')[0]) - 1,
                            vertex3 = Convert.ToInt32(SubStrings[3].Split('/')[0]) - 1
                        };
                        if (hasUVCoords)
                        {
                            TempTriangle.UVCoord1 = Convert.ToInt32(SubStrings[1].Split('/')[1]) - 1;
                            TempTriangle.UVCoord2 = Convert.ToInt32(SubStrings[2].Split('/')[1]) - 1;
                            TempTriangle.UVCoord3 = Convert.ToInt32(SubStrings[3].Split('/')[1]) - 1;
                        }

                        objData.TriangleList.Add(TempTriangle);
                    }
                    else if (j.Length > 7)
                        if (j.Substring(0, 7) == "usemtl ")
                        {
                            objData.MaterialList.Add(Regex.Replace(j.Substring(7), @"\s+", ""));
                            CurrentMaterial += 1;
                        }
                        else if (j.Substring(0, 7) == "mtllib ")
                            objData.MTLLib = j.Substring(7).Split('\\').LastOrDefault();
                }
            }

            // Special code
            if (ColorStream.Count == objData.VertexList.Count)
                for (int i = 0; i < objData.VertexList.Count; i++)
                {
                    Vertex v = objData.VertexList[i];
                    v.Color = ColorStream[i];
                    objData.VertexList[i] = v;
                }

            try
            {
                objData.MaterialList = ReplaceMaterialNames(InputFile, objData.MTLLib, objData.MaterialList);
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
            for (int i = 0; i < data.TriangleList.Count; i++)
            {
                if (data.VertexList[data.TriangleList[i].vertex1].HasUV == false)
                {
                    Vertex TempVertex = data.VertexList[data.TriangleList[i].vertex1];

                    TempVertex.TexCoord.X = data.UVList[data.TriangleList[i].UVCoord1].X;
                    TempVertex.TexCoord.Y = data.UVList[data.TriangleList[i].UVCoord1].Y;
                    TempVertex.HasUV = true;
                    data.VertexList[data.TriangleList[i].vertex1] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = data.VertexList[data.TriangleList[i].vertex1];

                    if ((TempVertex.TexCoord.X != data.UVList[data.TriangleList[i].UVCoord1].X) | (TempVertex.TexCoord.Y != data.UVList[data.TriangleList[i].UVCoord1].Y))
                    {
                        TempVertex.TexCoord.X = data.UVList[data.TriangleList[i].UVCoord1].X;
                        TempVertex.TexCoord.Y = data.UVList[data.TriangleList[i].UVCoord1].Y;

                        Triangle TempTriangle = data.TriangleList[i];
                        TempTriangle.vertex1 = data.VertexList.Count;
                        data.TriangleList[i] = TempTriangle;
                        data.VertexList.Add(TempVertex);
                    }
                }
                if (data.VertexList[data.TriangleList[i].vertex2].HasUV == false)
                {
                    Vertex TempVertex = data.VertexList[data.TriangleList[i].vertex2];

                    TempVertex.TexCoord.X = data.UVList[data.TriangleList[i].UVCoord2].X;
                    TempVertex.TexCoord.Y = data.UVList[data.TriangleList[i].UVCoord2].Y;
                    TempVertex.HasUV = true;
                    data.VertexList[data.TriangleList[i].vertex2] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = data.VertexList[data.TriangleList[i].vertex2];

                    if ((TempVertex.TexCoord.X != data.UVList[data.TriangleList[i].UVCoord2].X) | (TempVertex.TexCoord.Y != data.UVList[data.TriangleList[i].UVCoord2].Y))
                    {
                        TempVertex.TexCoord.X = data.UVList[data.TriangleList[i].UVCoord2].X;
                        TempVertex.TexCoord.Y = data.UVList[data.TriangleList[i].UVCoord2].Y;

                        Triangle TempTriangle = data.TriangleList[i];
                        TempTriangle.vertex2 = data.VertexList.Count;
                        data.TriangleList[i] = TempTriangle;
                        data.VertexList.Add(TempVertex);
                    }
                }
                if (data.VertexList[data.TriangleList[i].vertex3].HasUV == false)
                {
                    Vertex TempVertex = data.VertexList[data.TriangleList[i].vertex3];

                    TempVertex.TexCoord.X = data.UVList[data.TriangleList[i].UVCoord3].X;
                    TempVertex.TexCoord.Y = data.UVList[data.TriangleList[i].UVCoord3].Y;
                    TempVertex.HasUV = true;
                    data.VertexList[data.TriangleList[i].vertex3] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = data.VertexList[data.TriangleList[i].vertex3];

                    if ((TempVertex.TexCoord.X != data.UVList[data.TriangleList[i].UVCoord3].X) | (TempVertex.TexCoord.Y != data.UVList[data.TriangleList[i].UVCoord3].Y))
                    {
                        TempVertex.TexCoord.X = data.UVList[data.TriangleList[i].UVCoord3].X;
                        TempVertex.TexCoord.Y = data.UVList[data.TriangleList[i].UVCoord3].Y;

                        Triangle TempTriangle = data.TriangleList[i];
                        TempTriangle.vertex3 = data.VertexList.Count;
                        data.TriangleList[i] = TempTriangle;
                        data.VertexList.Add(TempVertex);
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
            indexLists.Last().Add(triangleStream2[0].vertex1);
            indexLists.Last().Add(triangleStream2[0].vertex2);
            indexLists.Last().Add(triangleStream2[0].vertex3);
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
                        if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].vertex1 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].vertex2)
                        {
                            indexLists.Last().Add(triangleStream2[i].vertex3);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].vertex2 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].vertex3)
                        {
                            indexLists.Last().Add(triangleStream2[i].vertex1);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].vertex3 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].vertex1)
                        {
                            indexLists.Last().Add(triangleStream2[i].vertex2);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                    }
                    else
                    {
                        if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].vertex2 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].vertex1)
                        {
                            indexLists.Last().Add(triangleStream2[i].vertex3);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].vertex3 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].vertex2)
                        {
                            indexLists.Last().Add(triangleStream2[i].vertex1);
                            triangleStream2[i].MaterialIndex = -1;
                            inverted = !inverted;
                            i = 0;
                            continue;
                        }
                        else if (indexLists.Last()[indexLists.Last().Count - 2] == triangleStream2[i].vertex1 &
                            indexLists.Last()[indexLists.Last().Count - 1] == triangleStream2[i].vertex3)
                        {
                            indexLists.Last().Add(triangleStream2[i].vertex2);
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
                        indexLists.Last().Add(triangleStream2[i].vertex1);
                        indexLists.Last().Add(triangleStream2[i].vertex2);
                        indexLists.Last().Add(triangleStream2[i].vertex3);
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

            foreach (RWSection rw in bspFile.GetAsRWSectionArray())
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
                                + (j.vertex1 + 1).ToString() + " "
                                + (j.vertex2 + 1).ToString() + " "
                                + (j.vertex3 + 1).ToString());
                }
                else
                foreach (Triangle j in triangleList)
                    if (j.MaterialIndex == i)
                        OBJWriter.WriteLine("f "
                            + (j.vertex1 + 1).ToString() + "/" + (j.vertex1 + 1).ToString() + " "
                            + (j.vertex2 + 1).ToString() + "/" + (j.vertex2 + 1).ToString() + " "
                            + (j.vertex3 + 1).ToString() + "/" + (j.vertex3 + 1).ToString());

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
            if (atomicSection.atomicSectorStruct.vertexArray != null)
                foreach (Vertex3 i in atomicSection.atomicSectorStruct.vertexArray)
                    OBJWriter.WriteLine("v " + i.X.ToString() + " " + i.Y.ToString() + " " + i.Z.ToString());

            OBJWriter.WriteLine();

            //Write uv list to obj
            if (atomicSection.atomicSectorStruct.uvArray != null)
            {
                if (flipUVs)
                    foreach (Vertex2 i in atomicSection.atomicSectorStruct.uvArray)
                        OBJWriter.WriteLine("vt " + i.X.ToString() + " " + (-i.Y).ToString());
                else
                    foreach (Vertex2 i in atomicSection.atomicSectorStruct.uvArray)
                        OBJWriter.WriteLine("vt " + i.X.ToString() + " " + i.Y.ToString());
            }
            OBJWriter.WriteLine();

            // Write vcolors to obj
            if (atomicSection.atomicSectorStruct.colorArray != null)
                foreach (RenderWareFile.Color i in atomicSection.atomicSectorStruct.colorArray)
                OBJWriter.WriteLine("vc " + i.R.ToString() + " " + i.G.ToString() + " " + i.B.ToString() + " " + i.A.ToString());

            OBJWriter.WriteLine();

            if (atomicSection.atomicSectorStruct.triangleArray != null)
                foreach (RenderWareFile.Triangle i in atomicSection.atomicSectorStruct.triangleArray)
                {
                    triangleList.Add(new Triangle
                    {
                        MaterialIndex = i.materialIndex,
                        vertex1 = i.vertex1 + totalVertexIndices,
                        vertex2 = i.vertex2 + totalVertexIndices,
                        vertex3 = i.vertex3 + totalVertexIndices
                    });
                }

            if (atomicSection.atomicSectorStruct.vertexArray != null)
                totalVertexIndices += atomicSection.atomicSectorStruct.vertexArray.Count();
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