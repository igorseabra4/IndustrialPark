using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.Models
{
    public static class BSP_IO
    {
        public static void ConvertBSPtoOBJ(string fileName, RWSection[] bspFile, bool flipUVs)
        {
            int totalVertexIndices = 1;

            string materialLibrary = Path.ChangeExtension(fileName, "MTL");
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            StreamWriter OBJWriter = new StreamWriter(Path.ChangeExtension(fileName, "OBJ"), false);

            List<Triangle> triangleList = new List<Triangle>();

            foreach (RWSection rw in bspFile)
            {
                if (rw is World_000B w)
                {
                    OBJWriter.WriteLine("# Exported by Heroes Power Plant");
                    OBJWriter.WriteLine("mtllib " + Path.GetFileName(materialLibrary));
                    OBJWriter.WriteLine();
                    if (w.firstWorldChunk.sectionIdentifier == Section.AtomicSector)
                    {
                        GetAtomicTriangleList(OBJWriter, (AtomicSector_0009)w.firstWorldChunk, ref triangleList, ref totalVertexIndices, flipUVs);
                    }
                    else if (w.firstWorldChunk.sectionIdentifier == Section.PlaneSector)
                    {
                        GetPlaneTriangleList(OBJWriter, (PlaneSector_000A)w.firstWorldChunk, ref triangleList, ref totalVertexIndices, flipUVs);
                    }

                    for (int i = 0; i < w.materialList.materialList.Length; i++)
                    {
                        string materialName = "default";
                        if (w.materialList.materialList[i].materialStruct.isTextured != 0)
                            materialName = w.materialList.materialList[i].texture.diffuseTextureName.stringString;

                        OBJWriter.WriteLine("g " + fileNameWithoutExtension + "_" + materialName);
                        OBJWriter.WriteLine("usemtl " + materialName + "_m");

                        foreach (Triangle j in triangleList)
                            if (j.materialIndex == i)
                                OBJWriter.WriteLine("f "
                                    + j.vertex1.ToString() + "/" + j.vertex1.ToString() + "/" + j.vertex1.ToString() + " "
                                    + j.vertex2.ToString() + "/" + j.vertex2.ToString() + "/" + j.vertex2.ToString() + " "
                                    + j.vertex3.ToString() + "/" + j.vertex3.ToString() + "/" + j.vertex3.ToString());

                        OBJWriter.WriteLine();
                    }

                    WriteMaterialLib(w.materialList, materialLibrary);
                }
            }


            OBJWriter.Close();
        }

        private static void GetPlaneTriangleList(StreamWriter OBJWriter, PlaneSector_000A planeSection, ref List<Triangle> triangleList, ref int totalVertexIndices, bool flipUVs)
        {
            if (planeSection.leftSection is AtomicSector_0009 a1)
            {
                GetAtomicTriangleList(OBJWriter, a1, ref triangleList, ref totalVertexIndices, flipUVs);
            }
            else if (planeSection.leftSection is PlaneSector_000A p1)
            {
                GetPlaneTriangleList(OBJWriter, p1, ref triangleList, ref totalVertexIndices, flipUVs);
            }

            if (planeSection.rightSection is AtomicSector_0009 a2)
            {
                GetAtomicTriangleList(OBJWriter, a2, ref triangleList, ref totalVertexIndices, flipUVs);
            }
            else if (planeSection.rightSection is PlaneSector_000A p2)
            {
                GetPlaneTriangleList(OBJWriter, p2, ref triangleList, ref totalVertexIndices, flipUVs);
            }
        }

        private static void GetAtomicTriangleList(StreamWriter OBJWriter, AtomicSector_0009 AtomicSector, ref List<Triangle> triangleList, ref int totalVertexIndices, bool flipUVs)
        {
            if (AtomicSector.atomicSectorStruct.isNativeData)
            {
                GetNativeTriangleList(OBJWriter, AtomicSector.atomicSectorExtension, null, ref triangleList, ref totalVertexIndices, flipUVs);
                return;
            }

            //Write vertex list to obj
            if (AtomicSector.atomicSectorStruct.vertexArray != null)
                foreach (Vertex3 i in AtomicSector.atomicSectorStruct.vertexArray)
                    OBJWriter.WriteLine("v " + i.X.ToString() + " " + i.Y.ToString() + " " + i.Z.ToString());

            OBJWriter.WriteLine();

            //Write uv list to obj
            if (AtomicSector.atomicSectorStruct.uvArray != null)
            {
                if (flipUVs)
                    foreach (Vertex2 i in AtomicSector.atomicSectorStruct.uvArray)
                        OBJWriter.WriteLine("vt " + i.X.ToString() + " " + (-i.Y).ToString());
                else
                    foreach (Vertex2 i in AtomicSector.atomicSectorStruct.uvArray)
                        OBJWriter.WriteLine("vt " + i.X.ToString() + " " + i.Y.ToString());
            }
            OBJWriter.WriteLine();

            // Write vcolors to obj
            if (AtomicSector.atomicSectorStruct.colorArray != null)
                foreach (Color i in AtomicSector.atomicSectorStruct.colorArray)
                    OBJWriter.WriteLine("vc " + i.R.ToString() + " " + i.G.ToString() + " " + i.B.ToString() + " " + i.A.ToString());

            OBJWriter.WriteLine();

            if (AtomicSector.atomicSectorStruct.triangleArray != null)
            {
                foreach (RenderWareFile.Triangle i in AtomicSector.atomicSectorStruct.triangleArray)
                {
                    triangleList.Add(new Triangle
                    {
                        materialIndex = i.materialIndex,
                        vertex1 = i.vertex1 + totalVertexIndices,
                        vertex2 = i.vertex2 + totalVertexIndices,
                        vertex3 = i.vertex3 + totalVertexIndices,
                    });
                }
            }

            if (AtomicSector.atomicSectorStruct.vertexArray != null)
                totalVertexIndices += AtomicSector.atomicSectorStruct.vertexArray.Count();
        }

        private static void GetNativeTriangleList(StreamWriter objWriter, Extension_0003 extension, List<string> materialList, ref List<Triangle> triangleList, ref int totalVertexIndices, bool flipUVs)
        {
            NativeDataGC n = null;

            foreach (RWSection rw in extension.extensionSectionList)
            {
                if (rw is BinMeshPLG_050E binmesh)
                {
                    if (binmesh.numMeshes == 0)
                        return;
                }
                if (rw is NativeDataPLG_0510 native)
                {
                    n = native.nativeDataStruct.nativeData;
                }
            }

            if (n == null)
                throw new Exception();

            List<Vertex3> vertexList_init = new List<Vertex3>();
            List<Vertex3> normalList_init = new List<Vertex3>();
            List<Color> colorList_init = new List<Color>();
            List<Vertex2> textCoordList_init = new List<Vertex2>();

            foreach (VertexAttribute d in n.attr)
            {
                if (d.attr == rwGCNVertexAttribute.rwGCNVA_POS)
                {
                    var dec = (VertexAttribute<Vertex3>)d;
                    foreach (var v in dec.entries)
                        vertexList_init.Add(v);
                }
                else if (d.attr == rwGCNVertexAttribute.rwGCNVA_NRM)
                {
                    var dec = (VertexAttribute<Vertex3>)d;
                    foreach (var v in dec.entries)
                        normalList_init.Add(v);
                }
                else if (d.attr == rwGCNVertexAttribute.rwGCNVA_CLR0)
                {
                    var dec = (VertexAttribute<RenderWareFile.Color>)d;
                    foreach (var c in dec.entries)
                        colorList_init.Add(c);
                }
                else if (d.attr == rwGCNVertexAttribute.rwGCNVA_TEX0)
                {
                    var dec = (VertexAttribute<Vertex2>)d;
                    foreach (var v in dec.entries)
                        textCoordList_init.Add(v);
                }
            }

            foreach (TriangleDeclaration td in n.displayList)
            {
                foreach (TriangleStrip tl in td.strips)
                {
                    List<Vertex3> vertexList_final = new List<Vertex3>();
                    List<Vertex3> normalList_final = new List<Vertex3>();
                    List<Color> colorList_final = new List<Color>();
                    List<Vertex2> textCoordList_final = new List<Vertex2>();

                    foreach (ushort[] objectList in tl.indices)
                    {
                        for (int j = 0; j < objectList.Count(); j++)
                        {
                            if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_POS)
                            {
                                vertexList_final.Add(vertexList_init[objectList[j]]);
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_NRM)
                            {
                                normalList_final.Add(normalList_init[objectList[j]]);
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_CLR0)
                            {
                                colorList_final.Add(colorList_init[objectList[j]]);
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_TEX0)
                            {
                                textCoordList_final.Add(textCoordList_init[objectList[j]]);
                            }
                        }
                    }

                    if (materialList != null)
                        triangleList.Clear();

                    bool control = true;

                    for (int i = 2; i < vertexList_final.Count(); i++)
                    {
                        if (control)
                        {
                            triangleList.Add(new Triangle
                            {
                                materialIndex = td.MaterialIndex,
                                vertex1 = i - 2 + totalVertexIndices,
                                vertex2 = i - 1 + totalVertexIndices,
                                vertex3 = i + totalVertexIndices
                            });
                        }
                        else
                        {
                            triangleList.Add(new Triangle
                            {
                                materialIndex = td.MaterialIndex,
                                vertex1 = i - 2 + totalVertexIndices,
                                vertex2 = i + totalVertexIndices,
                                vertex3 = i - 1 + totalVertexIndices
                            });
                        }
                        control = !control;
                    }

                    //Write vertex list to obj
                    foreach (Vertex3 i in vertexList_final)
                        objWriter.WriteLine("v " + i.X.ToString() + " " + i.Y.ToString() + " " + i.Z.ToString());

                    objWriter.WriteLine();

                    //Write normal list to obj
                    foreach (Vertex3 i in normalList_final)
                        objWriter.WriteLine("v " + i.X.ToString() + " " + i.Y.ToString() + " " + i.Z.ToString());

                    objWriter.WriteLine();

                    //Write uv list to obj
                    if (textCoordList_final.Count() > 0)
                    {
                        if (flipUVs)
                            foreach (Vertex2 i in textCoordList_final)
                                objWriter.WriteLine("vt " + i.X.ToString() + " " + (-i.Y).ToString());
                        else
                            foreach (Vertex2 i in textCoordList_final)
                                objWriter.WriteLine("vt " + i.X.ToString() + " " + i.Y.ToString());
                    }
                    objWriter.WriteLine();

                    // Write vcolors to obj
                    if (colorList_final.Count() > 0)
                        foreach (Color i in colorList_final)
                            objWriter.WriteLine("vc " + i.R.ToString() + " " + i.G.ToString() + " " + i.B.ToString() + " " + i.A.ToString());

                    objWriter.WriteLine();

                    totalVertexIndices += vertexList_final.Count();

                    if (materialList == null)
                        return;

                    for (int i = 0; i < materialList.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(lastMaterial) && lastMaterial != materialList[i])
                        {
                            objWriter.WriteLine("g obj_" + materialList[i]);
                            objWriter.WriteLine("usemtl " + materialList[i] + "_m");
                        }

                        foreach (Triangle j in triangleList)
                            if (j.materialIndex == i)
                                objWriter.WriteLine("f "
                                    + j.vertex1.ToString() + "/" + j.vertex1.ToString() + "/" + j.vertex1.ToString() + " "
                                    + j.vertex2.ToString() + "/" + j.vertex2.ToString() + "/" + j.vertex2.ToString() + " "
                                    + j.vertex3.ToString() + "/" + j.vertex3.ToString() + "/" + j.vertex3.ToString());

                        objWriter.WriteLine();

                        lastMaterial = materialList[i];
                    }
                }
            }
        }

        private static void WriteMaterialLib(MaterialList_0008 materialList, string materialLibrary)
        {
            StreamWriter MTLWriter = new StreamWriter(materialLibrary, false);
            MTLWriter.WriteLine("# Exported by Industrial Park");

            for (int i = 0; i < materialList.materialList.Length; i++)
            {
                string materialName = "default";
                if (materialList.materialList[i].materialStruct.isTextured != 0)
                    materialName = materialList.materialList[i].texture.diffuseTextureName.stringString;

                MTLWriter.WriteLine("newmtl " + materialName + "_m");
                MTLWriter.WriteLine("Ka 0.2 0.2 0.2");
                MTLWriter.WriteLine("Kd 0.8 0.8 0.8");
                MTLWriter.WriteLine("Ks 0 0 0");
                MTLWriter.WriteLine("Ns 10");
                MTLWriter.WriteLine("d 1.0");
                MTLWriter.WriteLine("illum 4");
                MTLWriter.WriteLine("map_Kd " + materialName + ".png");
                MTLWriter.WriteLine();
            }

            MTLWriter.Close();
        }

        private static string lastMaterial;

        public static void ConvertDFFtoOBJ(string fileName, RWSection[] dffFile, bool flipUVs)
        {
            lastMaterial = "";
            int totalVertexIndices = 1;

            string materialLibrary = Path.ChangeExtension(fileName, "MTL");
            int untexturedMaterials = 0;
            List<Triangle> triangleList = new List<Triangle>();

            StreamWriter writer = new StreamWriter((Path.ChangeExtension(fileName, "obj")), false);
            writer.WriteLine("# Exported by Industrial Park");
            writer.WriteLine("mtllib " + Path.GetFileName(materialLibrary));
            writer.WriteLine();

            foreach (RWSection rw in dffFile)
            {
                if (rw is Clump_0010 w)
                {
                    foreach (Geometry_000F rw2 in w.geometryList.geometryList)
                    {
                        ExportGeometryToOBJ(writer, rw2, ref triangleList, ref totalVertexIndices, ref untexturedMaterials, flipUVs);
                    }
                }
            }

            writer.Close();

            StreamWriter MTLWriter = new StreamWriter(materialLibrary, false);
            MTLWriter.WriteLine("# Exported by Industrial Park");
            MTLWriter.WriteLine();

            foreach (RWSection rw in dffFile)
            {
                if (rw is Clump_0010 w)
                {
                    foreach (Geometry_000F rw2 in w.geometryList.geometryList)
                    {
                        WriteMaterialLib(rw2, MTLWriter, ref untexturedMaterials);
                    }
                }
            }

            MTLWriter.Close();
        }

        private static void ExportGeometryToOBJ(StreamWriter writer, Geometry_000F g, ref List<Triangle> triangleList, ref int totalVertexIndices, ref int untexturedMaterials, bool flipUVs)
        {
            List<string> materialList = new List<string>();
            foreach (Material_0007 m in g.materialList.materialList)
            {
                if (m.texture != null)
                {
                    string textureName = m.texture.diffuseTextureName.stringString;
                    //if (!MaterialList.Contains(textureName))
                    materialList.Add(textureName);
                }
                else
                    materialList.Add("default");
            }

            GeometryStruct_0001 gs = g.geometryStruct;

            if ((gs.geometryFlags & GeometryFlags.rpGEOMETRYNATIVE) != 0)
            {
                GetNativeTriangleList(writer, g.geometryExtension, materialList, ref triangleList, ref totalVertexIndices, flipUVs);
                return;
            }

            if (g.materialList.materialList[0].materialStruct.isTextured != 0)
            {
                string mn = g.materialList.materialList[0].texture.diffuseTextureName.stringString;

                if (string.IsNullOrEmpty(lastMaterial) || lastMaterial != mn)
                {
                    writer.WriteLine("g obj_" + mn);
                    writer.WriteLine("usemtl " + mn + "_m");
                }

                lastMaterial = mn;
            }
            else
            {
                writer.WriteLine("g obj_default_" + untexturedMaterials.ToString());
                writer.WriteLine("usemtl default_" + untexturedMaterials.ToString() + "_m");
                untexturedMaterials++;
            }
            writer.WriteLine();

            foreach (MorphTarget m in gs.morphTargets)
            {
                if (m.hasVertices != 0)
                {
                    foreach (Vertex3 v in m.vertices)
                        writer.WriteLine("v " + v.X.ToString() + " " + v.Y.ToString() + " " + v.Z.ToString());
                    writer.WriteLine();
                }

                if (m.hasNormals != 0)
                {
                    foreach (Vertex3 vn in m.normals)
                        writer.WriteLine("vn " + vn.X.ToString() + " " + vn.Y.ToString() + " " + vn.Z.ToString());
                    writer.WriteLine();
                }

                if ((gs.geometryFlags & GeometryFlags.rpGEOMETRYPRELIT) != 0)
                {
                    foreach (Color c in gs.vertexColors)
                        writer.WriteLine("vc " + c.R.ToString() + " " + c.G.ToString() + " " + c.B.ToString() + " " + c.A.ToString());
                    writer.WriteLine();
                }

                if ((gs.geometryFlags & GeometryFlags.rpGEOMETRYTEXTURED) != 0)
                {
                    foreach (Vertex2 tc in gs.textCoords)
                        writer.WriteLine("vt " + tc.X.ToString() + " " + tc.Y.ToString());
                    writer.WriteLine();
                }

                foreach (RenderWareFile.Triangle t in gs.triangles)
                {
                    List<char> v1 = new List<char>(8);
                    List<char> v2 = new List<char>(8);
                    List<char> v3 = new List<char>(8);

                    int n1 = t.vertex1 + totalVertexIndices;
                    int n2 = t.vertex2 + totalVertexIndices;
                    int n3 = t.vertex3 + totalVertexIndices;

                    if (m.hasVertices != 0)
                    {
                        v1.AddRange(n1.ToString());
                        v2.AddRange(n2.ToString());
                        v3.AddRange(n3.ToString());
                    }
                    if (((gs.geometryFlags & GeometryFlags.rpGEOMETRYTEXTURED) != 0) & (m.hasNormals != 0))
                    {
                        v1.AddRange("/" + n1.ToString() + "/" + n1.ToString());
                        v2.AddRange("/" + n2.ToString() + "/" + n2.ToString());
                        v3.AddRange("/" + n3.ToString() + "/" + n3.ToString());
                    }
                    else if ((gs.geometryFlags & GeometryFlags.rpGEOMETRYTEXTURED) != 0)
                    {
                        v1.AddRange("/" + n1.ToString());
                        v2.AddRange("/" + n2.ToString());
                        v3.AddRange("/" + n3.ToString());
                    }
                    else if (m.hasNormals != 0)
                    {
                        v1.AddRange("//" + n1.ToString());
                        v2.AddRange("//" + n2.ToString());
                        v3.AddRange("//" + n3.ToString());
                    }
                    writer.WriteLine("f " + new string(v1.ToArray()) + " " + new string(v2.ToArray()) + " " + new string(v3.ToArray()));
                }

                totalVertexIndices += m.vertices.Count();
                writer.WriteLine();
            }
        }

        private static void WriteMaterialLib(Geometry_000F g, StreamWriter MTLWriter, ref int untexturedMaterials)
        {
            string textureName;
            if (g.materialList.materialList[0].materialStruct.isTextured != 0)
            {
                textureName = g.materialList.materialList[0].texture.diffuseTextureName.stringString;
            }
            else
            {
                textureName = "default_" + untexturedMaterials.ToString();
                untexturedMaterials++;
            }

            MTLWriter.WriteLine("newmtl " + textureName + "_m");
            MTLWriter.WriteLine("Ka 0.2 0.2 0.2");
            MTLWriter.WriteLine("Kd 0.8 0.8 0.8");
            MTLWriter.WriteLine("Ks 0 0 0");
            MTLWriter.WriteLine("Ns 10");
            MTLWriter.WriteLine("d 1.0");
            MTLWriter.WriteLine("illum 4");
            if (g.materialList.materialList[0].materialStruct.isTextured != 0)
                MTLWriter.WriteLine("map_Kd " + textureName + ".png");
            MTLWriter.WriteLine();
        }
    }
}