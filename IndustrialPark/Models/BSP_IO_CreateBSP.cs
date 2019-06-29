using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RenderWareFile;
using RenderWareFile.Sections;

namespace IndustrialPark.Models
{
    public static class BSP_IO_CreateBSP
    {
        public static int scoobyRenderWareVersion => 0x00000310;
        public static int bfbbRenderWareVersion => 0x1003FFFF; // fix
        public static int tssmRenderWareVersion => 0x1400FFFF;

        public static int currentRenderWareVersion
        {
            get
            {
                if (HipHopFile.Functions.currentGame == HipHopFile.Game.Scooby)
                    return scoobyRenderWareVersion;
                if (HipHopFile.Functions.currentGame == HipHopFile.Game.BFBB)
                    return bfbbRenderWareVersion;
                if (HipHopFile.Functions.currentGame == HipHopFile.Game.Incredibles)
                    return tssmRenderWareVersion;
                return 0;
            }
        }
        
        public static RWSection[] CreateBSPFile(ModelConverterData data, bool flipUVs)
        {
            Vertex3 Max = new Vertex3(data.VertexList[0].Position.X, data.VertexList[0].Position.Y, data.VertexList[0].Position.Z);
            Vertex3 Min = new Vertex3(data.VertexList[0].Position.X, data.VertexList[0].Position.Y, data.VertexList[0].Position.Z);

            foreach (Vertex i in data.VertexList)
            {
                if (i.Position.X > Max.X)
                    Max.X = i.Position.X;
                if (i.Position.Y > Max.Y)
                    Max.Y = i.Position.Y;
                if (i.Position.Z > Max.Z)
                    Max.Z = i.Position.Z;
                if (i.Position.X < Min.X)
                    Min.X = i.Position.X;
                if (i.Position.Y < Min.Y)
                    Min.Y = i.Position.Y;
                if (i.Position.Z < Min.Z)
                    Min.Z = i.Position.Z;
            }

            List<Vertex3> vList = new List<Vertex3>(data.VertexList.Count);
            foreach (Vertex v in data.VertexList)
                vList.Add(new Vertex3(v.Position.X, v.Position.Y, v.Position.Z));

            List<Color> cList = new List<Color>(data.VertexList.Count);
            foreach (Vertex v in data.VertexList)
                cList.Add(new Color(v.Color.R, v.Color.G, v.Color.B, v.Color.A));

            List<Vertex2> uvList = new List<Vertex2>(data.VertexList.Count);
            if (flipUVs)
                foreach (Vertex v in data.VertexList)
                    uvList.Add(new Vertex2(v.TexCoord.X, v.TexCoord.Y));
            else
                foreach (Vertex v in data.VertexList)
                    uvList.Add(new Vertex2(v.TexCoord.X, -v.TexCoord.Y));

            List<RenderWareFile.Triangle> tList = new List<RenderWareFile.Triangle>(data.TriangleList.Count);
            foreach (Triangle t in data.TriangleList)
                tList.Add(new RenderWareFile.Triangle((ushort)t.materialIndex, (ushort)t.vertex1, (ushort)t.vertex2, (ushort)t.vertex3));

            List<BinMesh> binMeshList = new List<BinMesh>();
            int TotalNumberOfTristripIndicies = 0;

            for (int i = 0; i < data.MaterialList.Count; i++)
            {
                List<int> indices = new List<int>();
                foreach (Triangle f in data.TriangleList)
                {
                    if (f.materialIndex == i)
                    {
                        indices.Add(f.vertex1);
                        indices.Add(f.vertex2);
                        indices.Add(f.vertex3);
                    }
                }
                TotalNumberOfTristripIndicies += indices.Count();

                binMeshList.Add(new BinMesh
                {
                    materialIndex = i,
                    indexCount = indices.Count(),
                    vertexIndices = indices.ToArray()
                });
            }

            WorldFlags worldFlags = WorldFlags.HasOneSetOfTextCoords | WorldFlags.HasVertexColors | WorldFlags.WorldSectorsOverlap | (WorldFlags)0x00010000;
            
            World_000B world = new World_000B()
            {
                worldStruct = new WorldStruct_0001()
                {
                    rootIsWorldSector = 1,
                    inverseOrigin = new Vertex3(-0f, -0f, -0f),
                    numTriangles = (uint)data.TriangleList.Count(),
                    numVertices = (uint)data.VertexList.Count(),
                    numPlaneSectors = 0,
                    numAtomicSectors = 1,
                    colSectorSize = 0,
                    worldFlags = worldFlags,
                    boxMaximum = Max,
                    boxMinimum = Min,
                },

                materialList = new MaterialList_0008()
                {
                    materialListStruct = new MaterialListStruct_0001()
                    {
                        materialCount = data.MaterialList.Count()
                    },
                    materialList = new Material_0007[data.MaterialList.Count()]
                },

                firstWorldChunk = new AtomicSector_0009()
                {
                    atomicSectorStruct = new AtomicSectorStruct_0001()
                    {
                        matListWindowBase = 0,
                        numTriangles = data.TriangleList.Count(),
                        numVertices = data.VertexList.Count(),
                        boxMaximum = Max,
                        boxMinimum = Min,
                        collSectorPresent = 0x2F50D984,
                        unused = 0,
                        vertexArray = vList.ToArray(),
                        colorArray = cList.ToArray(),
                        uvArray = uvList.ToArray(),
                        triangleArray = tList.ToArray()
                    },
                    atomicSectorExtension = new Extension_0003()
                    {
                        extensionSectionList = new List<RWSection>() { new BinMeshPLG_050E()
                        {
                            binMeshHeaderFlags = BinMeshHeaderFlags.TriangleList,
                            numMeshes = binMeshList.Count(),
                            totalIndexCount = TotalNumberOfTristripIndicies,
                            binMeshList = binMeshList.ToArray()
                        }
                        }
                    }
                },

                worldExtension = new Extension_0003()
            };

            for (int i = 0; i < data.MaterialList.Count; i++)
            {
                world.materialList.materialList[i] = new Material_0007()
                {
                    materialStruct = new MaterialStruct_0001()
                    {
                        unusedFlags = 0,
                        color = new RenderWareFile.Color() { R = 0xFF, G = 0xFF, B = 0xFF, A = 0xFF },
                        unusedInt2 = 0x2DF53E84,
                        isTextured = 1,
                        ambient = 1f,
                        specular = 1f,
                        diffuse = 1f
                    },
                    texture = new Texture_0006()
                    {
                        textureStruct = new TextureStruct_0001()
                        {
                            filterMode = TextureFilterMode.FILTERLINEAR,
                            addressModeU = TextureAddressMode.TEXTUREADDRESSWRAP,
                            addressModeV = TextureAddressMode.TEXTUREADDRESSWRAP,
                            useMipLevels = 1
                        },
                        diffuseTextureName = new String_0002()
                        {
                            stringString = data.MaterialList[i]
                        },
                        alphaTextureName = new String_0002()
                        {
                            stringString = ""
                        },
                        textureExtension = new Extension_0003()
                    },
                    materialExtension = new Extension_0003(),
                };
            }

            return new RWSection[] { world };
        }

        public static RWSection[] CreateDFFFile(ModelConverterData data, bool flipUVs)
        {
            Vertex3 Max = new Vertex3(data.VertexList[0].Position.X, data.VertexList[0].Position.Y, data.VertexList[0].Position.Z);
            Vertex3 Min = new Vertex3(data.VertexList[0].Position.X, data.VertexList[0].Position.Y, data.VertexList[0].Position.Z);

            foreach (Vertex i in data.VertexList)
            {
                if (i.Position.X > Max.X)
                    Max.X = i.Position.X;
                if (i.Position.Y > Max.Y)
                    Max.Y = i.Position.Y;
                if (i.Position.Z > Max.Z)
                    Max.Z = i.Position.Z;
                if (i.Position.X < Min.X)
                    Min.X = i.Position.X;
                if (i.Position.Y < Min.Y)
                    Min.Y = i.Position.Y;
                if (i.Position.Z < Min.Z)
                    Min.Z = i.Position.Z;
            }

            Vertex3 sphereCenter;
            sphereCenter.X = (Max.X + Min.X) / 2f;
            sphereCenter.Y = (Max.Y + Min.Y) / 2f;
            sphereCenter.Z = (Max.Z + Min.Z) / 2f;

            float sphereRadius = Math.Max((Max.X - Min.X) / 2f, Math.Max((Max.Y - Min.Y) / 2f, (Max.Z - Min.Z) / 2f));

            List<Vertex3> vList = new List<Vertex3>(data.VertexList.Count);
            foreach (Vertex v in data.VertexList)
                vList.Add(new Vertex3(v.Position.X, v.Position.Y, v.Position.Z));

            List<Vertex3> nList = new List<Vertex3>(data.VertexList.Count);
            foreach (Vertex v in data.VertexList)
                nList.Add(new Vertex3(-v.Normal.X, -v.Normal.Y, -v.Normal.Z));

            List<Color> cList = new List<Color>(data.VertexList.Count);
            foreach (Vertex v in data.VertexList)
                cList.Add(new Color(v.Color.R, v.Color.G, v.Color.B, v.Color.A));

            List<Vertex2> uvList = new List<Vertex2>(data.VertexList.Count);
            if (flipUVs)
                foreach (Vertex v in data.VertexList)
                    uvList.Add(new Vertex2(v.TexCoord.X, v.TexCoord.Y));
            else
                foreach (Vertex v in data.VertexList)
                    uvList.Add(new Vertex2(v.TexCoord.X, -v.TexCoord.Y));

            List<RenderWareFile.Triangle> tList = new List<RenderWareFile.Triangle>(data.TriangleList.Count);
            foreach (Triangle t in data.TriangleList)
                tList.Add(new RenderWareFile.Triangle((ushort)t.materialIndex, (ushort)t.vertex1, (ushort)t.vertex2, (ushort)t.vertex3));

            List<Material_0007> materials = new List<Material_0007>(data.MaterialList.Count);

            for (int i = 0; i < data.MaterialList.Count; i++)
            {
                materials.Add(new Material_0007()
                {
                    materialStruct = new MaterialStruct_0001()
                    {
                        unusedFlags = 0,
                        color = new Color() { R = 0xFF, G = 0xFF, B = 0xFF, A = 0xFF },
                        unusedInt2 = currentRenderWareVersion == scoobyRenderWareVersion ? 0x27584014 : 0,
                        isTextured = 1,
                        ambient = 1f,
                        specular = 1f,
                        diffuse = 1f
                    },
                    texture = new Texture_0006()
                    {
                        textureStruct = new TextureStruct_0001()
                        {
                            filterMode = TextureFilterMode.FILTERLINEARMIPLINEAR,
                            addressModeU = TextureAddressMode.TEXTUREADDRESSWRAP,
                            addressModeV = TextureAddressMode.TEXTUREADDRESSWRAP,
                            useMipLevels = 1
                        },
                        diffuseTextureName = new String_0002()
                        {
                            stringString = data.MaterialList[i]
                        },
                        alphaTextureName = new String_0002()
                        {
                            stringString = ""
                        },
                        textureExtension = new Extension_0003()
                    },
                    materialExtension = new Extension_0003()
                });
            }
            
            List<BinMesh> binMeshList = new List<BinMesh>();
            int TotalNumberOfTristripIndicies = 0;

            for (int i = 0; i < data.MaterialList.Count; i++)
            {
                List<int> indices = new List<int>();
                foreach (Triangle f in data.TriangleList)
                {
                    if (f.materialIndex == i)
                    {
                        indices.Add(f.vertex1);
                        indices.Add(f.vertex2);
                        indices.Add(f.vertex3);
                    }
                }
                TotalNumberOfTristripIndicies += indices.Count();

                binMeshList.Add(new BinMesh
                {
                    materialIndex = i,
                    indexCount = indices.Count(),
                    vertexIndices = indices.ToArray()
                });
            }

            Clump_0010 clump = new Clump_0010()
            {
                clumpStruct = new ClumpStruct_0001()
                {
                    atomicCount = 1
                },
                frameList = new FrameList_000E()
                {
                    frameListStruct = new FrameListStruct_0001()
                    {
                        frames = new List<Frame>()
                        {
                            new Frame()
                            {
                                position = new Vertex3(),
                                rotationMatrix = new Matrix3x3()
                                {
                                    M11 = 1f,
                                    M12 = 0f,
                                    M13 = 0f,
                                    M21 = 0f,
                                    M22 = 1f,
                                    M23 = 0f,
                                    M31 = 0f,
                                    M32 = 0f,
                                    M33 = 1f,
                                },
                                parentFrame = -1,
                                unknown = 131075
                            },
                            new Frame()
                            {
                                position = new Vertex3(),
                                rotationMatrix = new Matrix3x3()
                                {
                                    M11 = 1f,
                                    M12 = 0f,
                                    M13 = 0f,
                                    M21 = 0f,
                                    M22 = 1f,
                                    M23 = 0f,
                                    M31 = 0f,
                                    M32 = 0f,
                                    M33 = 1f,
                                },
                                parentFrame = 0,
                                unknown = 0
                            }
                        }
                    },
                    extensionList = new List<Extension_0003>()
                    {
                        new Extension_0003(),
                        new Extension_0003()
                    }
                },
                geometryList = new GeometryList_001A()
                {
                    geometryListStruct = new GeometryListStruct_0001()
                    {
                        numberOfGeometries = 1,
                    },
                    geometryList = new List<Geometry_000F>()
                    {
                        new Geometry_000F()
                        {
                            geometryStruct = new GeometryStruct_0001()
                            {
                                geometryFlags = GeometryFlags.hasLights | GeometryFlags.modeulateMaterialColor | GeometryFlags.hasTextCoords | GeometryFlags.hasVertexColors | GeometryFlags.hasVertexPositions | (nList.Count > 0 ? GeometryFlags.hasNormals:0),
                                geometryFlags2 = (GeometryFlags2)1,
                                numTriangles = data.TriangleList.Count(),
                                numVertices = data.VertexList.Count(),
                                numMorphTargets = 1,
                                ambient = 1f,
                                specular = 1f,
                                diffuse = 1f,
                                vertexColors = cList.ToArray(),
                                textCoords = uvList.ToArray(),
                                triangles = tList.ToArray(),
                                morphTargets = new MorphTarget[]
                                {
                                    new MorphTarget()
                                    {
                                        hasNormals = nList.Count > 0 ? 1 : 0,
                                        hasVertices = 1,
                                        sphereCenter = sphereCenter,
                                        radius = sphereRadius,
                                        vertices = vList.ToArray(),
                                        normals = nList.ToArray(),
                                    }
                                }
                            },

                            materialList = new MaterialList_0008()
                            {
                                materialListStruct = new MaterialListStruct_0001()
                                {
                                    materialCount = materials.Count()
                                },

                                materialList = materials.ToArray()
                            },

                            geometryExtension = new Extension_0003()
                            {
                                extensionSectionList = new List<RWSection>() { new BinMeshPLG_050E()
                                {
                                    binMeshHeaderFlags =  BinMeshHeaderFlags.TriangleList,
                                    numMeshes = binMeshList.Count(),
                                    totalIndexCount = TotalNumberOfTristripIndicies,
                                    binMeshList = binMeshList.ToArray()
                                }
                                }
                            }
                        }
                    }
                },
                atomicList = new List<Atomic_0014>()
                {
                    new Atomic_0014()
                    {
                        atomicStruct = new AtomicStruct_0001()
                        {
                            frameIndex = 1,
                            geometryIndex = 0,
                            unknown1 = 5,
                            unknown2 = 0
                        },
                        atomicExtension = currentRenderWareVersion == scoobyRenderWareVersion ? new Extension_0003()
                        {
                            extensionSectionList =  new List<RWSection>()
                            {
                                new MaterialEffectsPLG_0120()
                                {
                                    value = 0
                                }
                            }
                        } : new Extension_0003()
                    }
                },

                clumpExtension = new Extension_0003()
            };

            return new RWSection[] { clump };
        }

        public static void ConvertBSPtoOBJ(string fileName, RenderWareModelFile bspFile, bool flipUVs)
        {
            int totalVertexIndices = 1;

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
                        GetAtomicTriangleList(OBJWriter, (AtomicSector_0009)w.firstWorldChunk, ref triangleList, ref totalVertexIndices, flipUVs);
                    }
                    else if (w.firstWorldChunk.sectionIdentifier == Section.PlaneSector)
                    {
                        GetPlaneTriangleList(OBJWriter, (PlaneSector_000A)w.firstWorldChunk, ref triangleList, ref totalVertexIndices, flipUVs);
                    }
                }
            }

            for (int i = 0; i < bspFile.MaterialList.Count; i++)
            {
                OBJWriter.WriteLine("g " + fileNameWithoutExtension + "_" + bspFile.MaterialList[i]);
                OBJWriter.WriteLine("usemtl " + bspFile.MaterialList[i] + "_m");

                foreach (Triangle j in triangleList)
                    if (j.materialIndex == i)
                        OBJWriter.WriteLine("f "
                            + j.vertex1.ToString() + "/" + j.vertex1.ToString() + "/" + j.vertex1.ToString() + " "
                            + j.vertex2.ToString() + "/" + j.vertex2.ToString() + "/" + j.vertex2.ToString() + " "
                            + j.vertex3.ToString() + "/" + j.vertex3.ToString() + "/" + j.vertex3.ToString());

                OBJWriter.WriteLine();
            }

            OBJWriter.Close();
            WriteMaterialLib(bspFile.MaterialList.ToArray(), materialLibrary);

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
                    if (binmesh.numMeshes == 0) return;
                }
                if (rw is NativeDataPLG_0510 native)
                {
                    n = native.nativeDataStruct.nativeData;
                }
            }

            if (n == null) throw new Exception();

            List<Vertex3> vertexList_init = new List<Vertex3>();
            List<Vertex3> normalList_init = new List<Vertex3>();
            List<RenderWareFile.Color> colorList_init = new List<RenderWareFile.Color>();
            List<Vertex2> textCoordList_init = new List<Vertex2>();

            foreach (Declaration d in n.declarations)
            {
                foreach (object o in d.entryList)
                {
                    if (d.declarationType == Declarations.Vertex)
                        vertexList_init.Add((Vertex3)o);
                    else if (d.declarationType == Declarations.Normal)
                        normalList_init.Add((Vertex3)o);
                    else if (d.declarationType == Declarations.Color)
                        colorList_init.Add((Color)o);
                    else if (d.declarationType == Declarations.TextCoord)
                        textCoordList_init.Add((Vertex2)o);
                    else throw new Exception();
                }
            }

            foreach (TriangleDeclaration td in n.triangleDeclarations)
            {
                foreach (TriangleList tl in td.TriangleListList)
                {
                    List<Vertex3> vertexList_final = new List<Vertex3>();
                    List<Vertex3> normalList_final = new List<Vertex3>();
                    List<Color> colorList_final = new List<Color>();
                    List<Vertex2> textCoordList_final = new List<Vertex2>();

                    foreach (int[] objectList in tl.entries)
                    {
                        for (int j = 0; j < objectList.Count(); j++)
                        {
                            if (n.declarations[j].declarationType == Declarations.Vertex)
                            {
                                vertexList_final.Add(vertexList_init[objectList[j]]);
                            }
                            else if (n.declarations[j].declarationType == Declarations.Normal)
                            {
                                normalList_final.Add(normalList_init[objectList[j]]);
                            }
                            else if (n.declarations[j].declarationType == Declarations.Color)
                            {
                                colorList_final.Add(colorList_init[objectList[j]]);
                            }
                            else if (n.declarations[j].declarationType == Declarations.TextCoord)
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

        private static void WriteMaterialLib(string[] MaterialStream, string materialLibrary)
        {
            StreamWriter MTLWriter = new StreamWriter(materialLibrary, false);
            MTLWriter.WriteLine("# Exported by Industrial Park");

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

        private static string lastMaterial;

        public static void ConvertDFFtoOBJ(string fileName, RenderWareModelFile renderWareModelFile, bool flipUVs)
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

            foreach (RWSection rw in renderWareModelFile.GetAsRWSectionArray())
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

            foreach (RWSection rw in renderWareModelFile.GetAsRWSectionArray())
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

            if (gs.geometryFlags2 == (GeometryFlags2)0x0101)
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

                if ((gs.geometryFlags & GeometryFlags.hasVertexColors) != 0)
                {
                    foreach (Color c in gs.vertexColors)
                        writer.WriteLine("vc " + c.R.ToString() + " " + c.G.ToString() + " " + c.B.ToString() + " " + c.A.ToString());
                    writer.WriteLine();
                }

                if ((gs.geometryFlags & GeometryFlags.hasTextCoords) != 0)
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
                    if (((gs.geometryFlags & GeometryFlags.hasTextCoords) != 0) & (m.hasNormals != 0))
                    {
                        v1.AddRange("/" + n1.ToString() + "/" + n1.ToString());
                        v2.AddRange("/" + n2.ToString() + "/" + n2.ToString());
                        v3.AddRange("/" + n3.ToString() + "/" + n3.ToString());
                    }
                    else if ((gs.geometryFlags & GeometryFlags.hasTextCoords) != 0)
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