using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IndustrialPark.Models
{
    public static class BSP_IO_ReadOBJ
    {
        public static ModelConverterData ReadOBJFile(string InputFile, bool hasUVCoords = true)
        {
            ModelConverterData objData = new ModelConverterData()
            {
                MaterialList = new List<string>(),
                VertexList = new List<Vertex>(),
                NormalList = new List<Vector3>(),
                UVList = new List<Vector2>(),
                ColorList = new List<Color>(),
                TriangleList = new List<Triangle>(),
                MTLLib = null
            };

            if (!hasUVCoords)
                objData.UVList = new List<Vector2>() { new Vector2() };

            string[] OBJFile = File.ReadAllLines(InputFile);

            int CurrentMaterial = -1;
            
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
                    else if (j.Substring(0, 3) == "vn ")
                    {
                        string[] SubStrings = j.Split(' ');
                        Vector3 TempNormal = new Vector3
                        {
                            X = Convert.ToSingle(SubStrings[1]),
                            Y = Convert.ToSingle(SubStrings[2]),
                            Z = Convert.ToSingle(SubStrings[3])
                        };
                        objData.NormalList.Add(TempNormal);
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

                        objData.ColorList.Add(TempColor);
                    }
                    else if (j.StartsWith("f "))
                    {
                        string[] SubStrings = j.Split(' ');

                        Triangle TempTriangle = new Triangle
                        {
                            materialIndex = CurrentMaterial,
                            vertex1 = Convert.ToInt32(SubStrings[1].Split('/')[0]) - 1,
                            vertex2 = Convert.ToInt32(SubStrings[2].Split('/')[0]) - 1,
                            vertex3 = Convert.ToInt32(SubStrings[3].Split('/')[0]) - 1
                        };

                        if (hasUVCoords)
                        {
                            try
                            {
                                TempTriangle.UVCoord1 = Convert.ToInt32(SubStrings[1].Split('/')[1]) - 1;
                                TempTriangle.UVCoord2 = Convert.ToInt32(SubStrings[2].Split('/')[1]) - 1;
                                TempTriangle.UVCoord3 = Convert.ToInt32(SubStrings[3].Split('/')[1]) - 1;
                            }
                            catch
                            {
                                MessageBox.Show("Error parsing texture coordinates. The model will be imported without them.");
                                hasUVCoords = false;
                                objData.UVList = new List<Vector2>() { new Vector2() };
                            }
                        }

                        objData.TriangleList.Add(TempTriangle);
                    }
                    else if (j.StartsWith("usemtl "))
                    {
                        objData.MaterialList.Add(Regex.Replace(j.Substring(7), @"\s+", ""));
                        CurrentMaterial += 1;
                    }
                    else if (j.StartsWith("mtllib "))
                        objData.MTLLib = j.Substring(7).Split('\\').LastOrDefault();
                }
            }
            
            // Special code
            if (objData.ColorList.Count == objData.VertexList.Count)
                for (int i = 0; i < objData.VertexList.Count; i++)
                {
                    Vertex v = objData.VertexList[i];
                    v.Color = objData.ColorList[i];
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

            if (hasUVCoords)
            {
                FixNormals(ref objData);
                FixUVCoords(ref objData);
            }

            return objData;
        }

        public static List<string> ReplaceMaterialNames(string InputOBJFile, string MTLLib, List<string> MaterialList)
        {
            string MTLPath = Path.Combine(Path.GetDirectoryName(InputOBJFile), MTLLib);
            string[] MTLFile = File.ReadAllLines(MTLPath);

            Dictionary<string, string> MaterialLibrary = new Dictionary<string, string>();

            string MaterialName = "";

            foreach (string j in MTLFile)
            {
                string a = Regex.Replace(j, @"\s+", "");

                if (a.StartsWith("newmtl"))
                    MaterialName = a.Substring(6);
                else if (a.StartsWith("map_Kd"))
                    MaterialLibrary[MaterialName] = Path.GetFileNameWithoutExtension(a.Substring(6));
            }

            for (int k = 0; k < MaterialList.Count; k++)
            {
                if (MaterialLibrary.ContainsKey(MaterialList[k]))
                    MaterialList[k] = MaterialLibrary[MaterialList[k]];
                //else
                //    MessageBox.Show("Texture name for material " + MaterialList[k] + " was not found in the " + MTLPath + " file.");
            }

            return MaterialList;
        }

        public static void FixUVCoords(ref ModelConverterData data)
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

                        data.TriangleList[i].vertex1 = data.VertexList.Count;
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

                        data.TriangleList[i].vertex2 = data.VertexList.Count;
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

                        data.TriangleList[i].vertex3 = data.VertexList.Count;
                        data.VertexList.Add(TempVertex);
                    }
                }
            }
        }

        public static void FixNormals(ref ModelConverterData data)
        {
            List<Vector3>[] normalListList = new List<Vector3>[data.VertexList.Count];
            for (int i = 0; i < normalListList.Count(); i++)
                normalListList[i] = new List<Vector3>();

            foreach (Triangle t in data.TriangleList)
            {
                normalListList[t.vertex1].Add(data.NormalList[t.normal1]);
                normalListList[t.vertex2].Add(data.NormalList[t.normal2]);
                normalListList[t.vertex3].Add(data.NormalList[t.normal3]);
            }

            for (int i = 0; i < data.VertexList.Count; i++)
            {
                Vector3 acc = new Vector3();
                foreach (Vector3 v in normalListList[i])
                    acc += v;
                acc.Normalize();

                Vertex TempVertex = data.VertexList[i];
                TempVertex.Normal = acc;
                data.VertexList[i] = TempVertex;
            }
        }

        public static void FixColors(ref ModelConverterData d)
        {
            for (int i = 0; i < d.TriangleList.Count; i++)
            {
                if (d.VertexList[d.TriangleList[i].vertex1].HasColor == false)
                {
                    Vertex TempVertex = d.VertexList[d.TriangleList[i].vertex1];

                    TempVertex.Color = d.ColorList[d.TriangleList[i].Color1];
                    TempVertex.HasColor = true;
                    d.VertexList[d.TriangleList[i].vertex1] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = d.VertexList[d.TriangleList[i].vertex1];

                    if (!TempVertex.Color.Equals(d.ColorList[d.TriangleList[i].Color1]))
                    {
                        TempVertex.Color.R = d.ColorList[d.TriangleList[i].Color1].R;
                        TempVertex.Color.G = d.ColorList[d.TriangleList[i].Color1].G;
                        TempVertex.Color.B = d.ColorList[d.TriangleList[i].Color1].B;
                        TempVertex.Color.A = d.ColorList[d.TriangleList[i].Color1].A;

                        d.TriangleList[i].vertex1 = d.VertexList.Count;
                        d.VertexList.Add(TempVertex);
                    }
                }
                if (d.VertexList[d.TriangleList[i].vertex2].HasColor == false)
                {
                    Vertex TempVertex = d.VertexList[d.TriangleList[i].vertex2];

                    TempVertex.Color = d.ColorList[d.TriangleList[i].Color2];
                    TempVertex.HasColor = true;
                    d.VertexList[d.TriangleList[i].vertex2] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = d.VertexList[d.TriangleList[i].vertex2];

                    if (!TempVertex.Color.Equals(d.ColorList[d.TriangleList[i].Color2]))
                    {
                        TempVertex.Color.R = d.ColorList[d.TriangleList[i].Color2].R;
                        TempVertex.Color.G = d.ColorList[d.TriangleList[i].Color2].G;
                        TempVertex.Color.B = d.ColorList[d.TriangleList[i].Color2].B;
                        TempVertex.Color.A = d.ColorList[d.TriangleList[i].Color2].A;

                        d.TriangleList[i].vertex2 = d.VertexList.Count;
                        d.VertexList.Add(TempVertex);
                    }
                }
                if (d.VertexList[d.TriangleList[i].vertex3].HasColor == false)
                {
                    Vertex TempVertex = d.VertexList[d.TriangleList[i].vertex3];

                    TempVertex.Color = d.ColorList[d.TriangleList[i].Color3];
                    TempVertex.HasColor = true;
                    d.VertexList[d.TriangleList[i].vertex3] = TempVertex;
                }
                else
                {
                    Vertex TempVertex = d.VertexList[d.TriangleList[i].vertex3];

                    if (!TempVertex.Color.Equals(d.ColorList[d.TriangleList[i].Color3]))
                    {
                        TempVertex.Color.R = d.ColorList[d.TriangleList[i].Color3].R;
                        TempVertex.Color.G = d.ColorList[d.TriangleList[i].Color3].G;
                        TempVertex.Color.B = d.ColorList[d.TriangleList[i].Color3].B;
                        TempVertex.Color.A = d.ColorList[d.TriangleList[i].Color3].A;

                        d.TriangleList[i].vertex3 = d.VertexList.Count;
                        d.VertexList.Add(TempVertex);
                    }
                }
            }
        }
    }
}
