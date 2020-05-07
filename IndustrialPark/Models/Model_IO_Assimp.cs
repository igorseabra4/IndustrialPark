using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using RenderWareFile;
using RenderWareFile.Sections;
using Assimp;

namespace IndustrialPark.Models
{
    public static class Model_IO_Assimp
    {
        public static string GetImportFilter()
        {
            string[] formats = new AssimpContext().GetSupportedImportFormats();

            string filter = "All supported types|";

            foreach (string s in formats)
                filter += "*" + s + ";";

            filter += "*.dff|DFF Files|*.dff";

            foreach (string s in formats)
                filter += "|" + s.Substring(1).ToUpper() + " files|*" + s;

            filter += "|All files|*.*";

            return filter;
        }

        public static ModelConverterData ReadAssimp(string fileName)
        {
            Scene scene = new AssimpContext().ImportFile(fileName,
                PostProcessSteps.Debone | PostProcessSteps.FindInstances | PostProcessSteps.FindInvalidData |
                PostProcessSteps.OptimizeGraph | PostProcessSteps.OptimizeMeshes | PostProcessSteps.Triangulate |
                PostProcessSteps.PreTransformVertices);

            ModelConverterData data = new ModelConverterData()
            {
                MaterialList = new List<string>(),
                VertexList = new List<Vertex>(),
                UVList = new List<Vector2>(),
                ColorList = new List<SharpDX.Color>(),
                TriangleList = new List<Triangle>()
            };

            foreach (var mat in scene.Materials)
                if (mat.TextureDiffuse.FilePath == null)
                    data.MaterialList.Add(Path.GetFileNameWithoutExtension(""));
                else
                    data.MaterialList.Add(Path.GetFileNameWithoutExtension(mat.TextureDiffuse.FilePath));

            int totalVertices = 0;

            foreach (var m in scene.Meshes)
            {
                for (int i = 0; i < m.VertexCount; i++)
                {
                    Vertex v = new Vertex() { Position = new Vector3(m.Vertices[i].X, m.Vertices[i].Y, m.Vertices[i].Z) };

                    if (m.HasTextureCoords(0))
                        v.TexCoord = new Vector2(m.TextureCoordinateChannels[0][i].X, m.TextureCoordinateChannels[0][i].Y);
                    else
                        v.TexCoord = new Vector2();

                    if (m.HasVertexColors(0))
                        v.Color = new SharpDX.Color(m.VertexColorChannels[0][i].R, m.VertexColorChannels[0][i].G, m.VertexColorChannels[0][i].B, m.VertexColorChannels[0][i].A);
                    else
                        v.Color = SharpDX.Color.White;

                    data.VertexList.Add(v);
                }

                foreach (var t in m.Faces)
                    data.TriangleList.Add(new Triangle()
                    {
                        vertex1 = t.Indices[0] + totalVertices,
                        vertex2 = t.Indices[1] + totalVertices,
                        vertex3 = t.Indices[2] + totalVertices,
                        materialIndex = m.MaterialIndex
                    });
                totalVertices += m.VertexCount;
            }

            return data;
        }

        public static RWSection[] CreateDFFFromAssimp(string fileName, bool flipUVs, bool ignoreMeshColors)
        {
            PostProcessSteps pps =
                PostProcessSteps.Debone |
                PostProcessSteps.FindInstances |
                PostProcessSteps.FindInvalidData |
                PostProcessSteps.GenerateNormals |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.OptimizeGraph |
                PostProcessSteps.OptimizeMeshes |
                PostProcessSteps.PreTransformVertices |
                PostProcessSteps.Triangulate |
                (flipUVs ? PostProcessSteps.FlipUVs : 0);

            Scene scene = new AssimpContext().ImportFile(fileName, pps);

            int vertexCount = scene.Meshes.Sum(m => m.VertexCount);
            int triangleCount = scene.Meshes.Sum(m => m.FaceCount);

            if (vertexCount > 65535 || triangleCount > 65536)
                throw new ArgumentException("Model has too many vertices or triangles. Please import a simpler model.");

            var materials = new List<Material_0007>(scene.MaterialCount);

            foreach (var m in scene.Materials)
                materials.Add(new Material_0007()
                {
                    materialStruct = new MaterialStruct_0001()
                    {
                        unusedFlags = 0,
                        color = ignoreMeshColors ?
                        new RenderWareFile.Color(255, 255, 255, 255) :
                        new RenderWareFile.Color(
                            (byte)(m.ColorDiffuse.R * 255),
                            (byte)(m.ColorDiffuse.G * 255),
                            (byte)(m.ColorDiffuse.B * 255),
                            (byte)(m.ColorDiffuse.A * 255)),
                        unusedInt2 = 0x2DF53E84,
                        isTextured = m.HasTextureDiffuse ? 1 : 0,
                        ambient = 1f,
                        specular = 1f,
                        diffuse = 1f
                    },
                    texture = m.HasTextureDiffuse ? new Texture_0006()
                    {
                        textureStruct = new TextureStruct_0001() // use wrap as default
                        {
                            FilterMode = TextureFilterMode.FILTERLINEAR,
                            AddressModeU =
                            m.TextureDiffuse.WrapModeU == TextureWrapMode.Clamp ? TextureAddressMode.TEXTUREADDRESSCLAMP :
                            m.TextureDiffuse.WrapModeU == TextureWrapMode.Decal ? TextureAddressMode.TEXTUREADDRESSBORDER :
                            m.TextureDiffuse.WrapModeU == TextureWrapMode.Mirror ? TextureAddressMode.TEXTUREADDRESSMIRROR :
                            TextureAddressMode.TEXTUREADDRESSWRAP,

                            AddressModeV =
                            m.TextureDiffuse.WrapModeV == TextureWrapMode.Clamp ? TextureAddressMode.TEXTUREADDRESSCLAMP :
                            m.TextureDiffuse.WrapModeV == TextureWrapMode.Decal ? TextureAddressMode.TEXTUREADDRESSBORDER :
                            m.TextureDiffuse.WrapModeV == TextureWrapMode.Mirror ? TextureAddressMode.TEXTUREADDRESSMIRROR :
                            TextureAddressMode.TEXTUREADDRESSWRAP,
                            UseMipLevels = 1
                        },
                        diffuseTextureName = new String_0002(Path.GetFileNameWithoutExtension(m.TextureDiffuse.FilePath)),
                        alphaTextureName = new String_0002(""),
                        textureExtension = new Extension_0003()
                    } : null,
                    materialExtension = new Extension_0003(),
                });

            List<Vertex3> vertices = new List<Vertex3>();
            List<Vertex3> normals = new List<Vertex3>();
            List<Vertex2> textCoords = new List<Vertex2>();
            List<RenderWareFile.Color> vertexColors = new List<RenderWareFile.Color>();
            List<RenderWareFile.Triangle> triangles = new List<RenderWareFile.Triangle>();

            foreach (var m in scene.Meshes)
            {
                int totalVertices = vertices.Count;

                foreach (Vector3D v in m.Vertices)
                    vertices.Add(new Vertex3(v.X, v.Y, v.Z));

                foreach (Vector3D v in m.Normals)
                    normals.Add(new Vertex3(v.X, v.Y, v.Z));

                if (m.HasTextureCoords(0))
                    foreach (Vector3D v in m.TextureCoordinateChannels[0])
                        textCoords.Add(new Vertex2(v.X, v.Y));
                else
                    for (int i = 0; i < m.VertexCount; i++)
                        textCoords.Add(new Vertex2(0, 0));

                if (m.HasVertexColors(0))
                    foreach (Color4D c in m.VertexColorChannels[0])
                        vertexColors.Add(new RenderWareFile.Color(
                            (byte)(c.R * 255),
                            (byte)(c.G * 255),
                            (byte)(c.B * 255),
                            (byte)(c.A * 255)));
                else
                    for (int i = 0; i < m.VertexCount; i++)
                        vertexColors.Add(new RenderWareFile.Color(255, 255, 255, 255));

                foreach (var t in m.Faces)
                    if (t.IndexCount == 3)
                        triangles.Add(new RenderWareFile.Triangle()
                        {
                            vertex1 = (ushort)(t.Indices[0] + totalVertices),
                            vertex2 = (ushort)(t.Indices[1] + totalVertices),
                            vertex3 = (ushort)(t.Indices[2] + totalVertices),
                            materialIndex = (ushort)m.MaterialIndex
                        });
            }

            Vertex3 max = new Vertex3(vertices[0].X, vertices[0].Y, vertices[0].Z);
            Vertex3 min = new Vertex3(vertices[0].X, vertices[0].Y, vertices[0].Z);

            foreach (Vertex3 v in vertices)
            {
                if (v.X > max.X)
                    max.X = v.X;
                if (v.Y > max.Y)
                    max.Y = v.Y;
                if (v.Z > max.Z)
                    max.Z = v.Z;
                if (v.X < min.X)
                    min.X = v.X;
                if (v.Y < min.Y)
                    min.Y = v.Y;
                if (v.Z < min.Z)
                    min.Z = v.Z;
            }

            Vertex3 sphereCenter = new Vertex3((max.X + min.X) / 2f, (max.Y + min.Y) / 2f, (max.Z + min.Z) / 2f);
            float radius = Math.Max(max.X - min.X, Math.Max(max.Y - min.Y, max.Z - min.Z));

            var binMeshes = new List<BinMesh>(materials.Count);
            int k = 0;
            int totalIndexCount = 0;

            foreach (Material_0007 mat in materials)
            {
                List<int> indices = new List<int>(triangleCount * 3);

                foreach (var t in triangles)
                    if (t.materialIndex == k)
                    {
                        indices.Add(t.vertex1);
                        indices.Add(t.vertex2);
                        indices.Add(t.vertex3);
                    }

                if (indices.Count > 0)
                    binMeshes.Add(new BinMesh()
                    {
                        materialIndex = k,
                        indexCount = indices.Count(),
                        vertexIndices = indices.ToArray()
                    });

                k++;

                totalIndexCount += indices.Count;
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
                                rotationMatrix = new RenderWareFile.Sections.Matrix3x3()
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
                                rotationMatrix = new RenderWareFile.Sections.Matrix3x3()
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
                        numberOfGeometries = 1
                    },
                    geometryList = new List<Geometry_000F>()
                    {
                        new Geometry_000F()
                        {
                            materialList = new MaterialList_0008()
                            {
                                materialListStruct = new MaterialListStruct_0001()
                                {
                                    materialCount = materials.Count
                                },
                                materialList = materials.ToArray()
                            },
                            geometryStruct = new GeometryStruct_0001()
                            {
                                geometryFlags =
                                GeometryFlags.hasLights |
                                GeometryFlags.modeulateMaterialColor |
                                GeometryFlags.hasTextCoords |
                                GeometryFlags.hasVertexColors |
                                GeometryFlags.hasVertexPositions |
                                GeometryFlags.hasNormals,
                                geometryFlags2 = (GeometryFlags2)1,
                                numTriangles = triangles.Count(),
                                numVertices = vertices.Count(),
                                numMorphTargets = 1,
                                ambient = 1f,
                                specular = 1f,
                                diffuse = 1f,
                                vertexColors = vertexColors.ToArray(),
                                textCoords = textCoords.ToArray(),
                                triangles = triangles.ToArray(),
                                morphTargets = new MorphTarget[]
                                {
                                    new MorphTarget()
                                    {
                                        hasNormals = 1,
                                        hasVertices = 1,
                                        sphereCenter = sphereCenter,
                                        radius = radius,
                                        vertices = vertices.ToArray(),
                                        normals = normals.ToArray(),
                                    }
                                }
                            },
                            geometryExtension = new Extension_0003()
                            {
                                extensionSectionList = new List<RWSection>()
                                {
                                    new BinMeshPLG_050E()
                                    {
                                        binMeshHeaderFlags =  BinMeshHeaderFlags.TriangleList,
                                        numMeshes = binMeshes.Count,
                                        totalIndexCount = totalIndexCount,
                                        binMeshList = binMeshes.ToArray()
                                    }
                                }
                            }
                        }
                    }
                },
                atomicList = new List<Atomic_0014>() { new Atomic_0014()
                {
                    atomicStruct = new AtomicStruct_0001()
                    {
                        frameIndex = 1,
                        geometryIndex = 0,
                        flags = AtomicFlags.CollisionTestAndRender,
                        unused = 0
                    },
                    atomicExtension = new Extension_0003() // check this in case something fails
                    {
                        extensionSectionList = new List<RWSection>()
                    }
                }
                },

                clumpExtension = new Extension_0003()
            };

            return new RWSection[] { clump };
        }

        public static void ExportAssimp(string fileName, RWSection[] bspFile, bool flipUVs, ExportFormatDescription format, string textureExtension)
        {
            Scene scene = new Scene();

            foreach (RWSection rw in bspFile)
                if (rw is World_000B w)
                    WorldToScene(scene, w, textureExtension);
                else if (rw is Clump_0010 c)
                    ClumpToScene(scene, c, textureExtension);

            scene.RootNode = new Node() { Name = "root" };

            Node latest = scene.RootNode;

            for (int i = 0; i < scene.MeshCount; i++)
            {
                latest.Children.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Node>(
                    "{\"Name\":\"" + scene.Meshes[i].Name + "\", \"MeshIndices\": [" + i.ToString() + "]}"));

                //latest = latest.Children[0];
            }

            new AssimpContext().ExportFile(scene, fileName, format.FormatId,
                PostProcessSteps.Debone |
                PostProcessSteps.FindInstances |
                PostProcessSteps.GenerateNormals |
                PostProcessSteps.FindInvalidData |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.OptimizeGraph |
                PostProcessSteps.OptimizeMeshes |
                PostProcessSteps.PreTransformVertices |
                PostProcessSteps.RemoveRedundantMaterials |
                PostProcessSteps.Triangulate |
                PostProcessSteps.ValidateDataStructure |
                (flipUVs ? PostProcessSteps.FlipUVs : 0));
        }

        private static void WorldToScene(Scene scene, World_000B world, string textureExtension)
        {
            for (int i = 0; i < world.materialList.materialList.Length; i++)
            {
                var mat = world.materialList.materialList[i];

                scene.Materials.Add(new Material()
                {
                    ColorDiffuse = new Color4D(
                        mat.materialStruct.color.R / 255f,
                        mat.materialStruct.color.G / 255f,
                        mat.materialStruct.color.B / 255f,
                        mat.materialStruct.color.A / 255f),
                    TextureDiffuse = mat.materialStruct.isTextured != 0 ? new TextureSlot()
                    {
                        FilePath = mat.texture.diffuseTextureName.stringString + textureExtension,
                        TextureType = TextureType.Diffuse
                    } : default,
                    Name = mat.materialStruct.isTextured != 0 ? "mat_" + mat.texture.diffuseTextureName.stringString : default,
                });

                scene.Meshes.Add(new Mesh(PrimitiveType.Triangle)
                {
                    MaterialIndex = i,
                    Name = "mesh_" +
                    (mat.materialStruct.isTextured != 0 ? mat.texture.diffuseTextureName.stringString : ("default_" + i.ToString()))
                });
            }

            if (world.firstWorldChunk.sectionIdentifier == Section.AtomicSector)
                AtomicToScene(scene, (AtomicSector_0009)world.firstWorldChunk);
            else if (world.firstWorldChunk.sectionIdentifier == Section.PlaneSector)
                PlaneToScene(scene, (PlaneSector_000A)world.firstWorldChunk);
        }

        private static void PlaneToScene(Scene scene, PlaneSector_000A planeSection)
        {
            if (planeSection.leftSection is AtomicSector_0009 a1)
            {
                AtomicToScene(scene, a1);
            }
            else if (planeSection.leftSection is PlaneSector_000A p1)
            {
                PlaneToScene(scene, p1);
            }

            if (planeSection.rightSection is AtomicSector_0009 a2)
            {
                AtomicToScene(scene, a2);
            }
            else if (planeSection.rightSection is PlaneSector_000A p2)
            {
                PlaneToScene(scene, p2);
            }
        }

        private static void AtomicToScene(Scene scene, AtomicSector_0009 atomic)
        {
            int[] totalVertexIndices = new int[scene.MeshCount];

            for (int i = 0; i < scene.MeshCount; i++)
                totalVertexIndices[i] = scene.Meshes[i].VertexCount;

            foreach (RenderWareFile.Triangle t in atomic.atomicSectorStruct.triangleArray)
            {
                scene.Meshes[t.materialIndex].Faces.Add(new Face(new int[] {
                    t.vertex1 + totalVertexIndices[t.materialIndex],
                    t.vertex2 + totalVertexIndices[t.materialIndex],
                    t.vertex3 + totalVertexIndices[t.materialIndex]
                }));
            }

            foreach (Mesh mesh in scene.Meshes)
            {
                foreach (Vertex3 v in atomic.atomicSectorStruct.vertexArray)
                    mesh.Vertices.Add(new Vector3D(v.X, v.Y, v.Z));

                foreach (Vertex2 v in atomic.atomicSectorStruct.uvArray)
                    mesh.TextureCoordinateChannels[0].Add(new Vector3D(v.X, v.Y, 0f));

                foreach (RenderWareFile.Color c in atomic.atomicSectorStruct.colorArray)
                    mesh.VertexColorChannels[0].Add(new Color4D(
                        c.R / 255f,
                        c.G / 255f,
                        c.B / 255f,
                        c.A / 255f));
            }
        }

        private static void ClumpToScene(Scene scene, Clump_0010 clump, string textureExtension)
        {
            int totalMaterials = 0;

            for (int i = 0; i < clump.geometryList.geometryList.Count; i++)
            {
                Matrix transformMatrix = RenderWareModelFile.CreateMatrix(clump.frameList, clump.atomicList[i].atomicStruct.frameIndex);

                int triangleListOffset = 0;
                for (int j = 0; j < clump.geometryList.geometryList[i].materialList.materialList.Length; j++)
                {
                    var geo = clump.geometryList.geometryList[i].geometryStruct;
                    var mat = clump.geometryList.geometryList[i].materialList.materialList[j];

                    Material material = new Material()
                    {
                        ColorDiffuse = new Color4D(
                                mat.materialStruct.color.R / 255f,
                                mat.materialStruct.color.G / 255f,
                                mat.materialStruct.color.B / 255f,
                                mat.materialStruct.color.A / 255f),
                        Name = "default"
                    };

                    if (mat.materialStruct.isTextured != 0)
                    {
                        material.TextureDiffuse = new TextureSlot()
                        {
                            FilePath = mat.texture.diffuseTextureName.stringString + textureExtension,
                            TextureType = TextureType.Diffuse
                        };
                        material.Name = "mat_" + mat.texture.diffuseTextureName.stringString;
                    }

                    scene.Materials.Add(material);

                    Mesh mesh = new Mesh(PrimitiveType.Triangle)
                    {
                        MaterialIndex = j + totalMaterials,
                        Name = "mesh_" + material.Name.Replace("mat_", "")
                    };

                    if (geo.geometryFlags2 == (GeometryFlags2)0x0101)
                    {
                        NativeDataGC n = null;

                        foreach (RWSection rws in clump.geometryList.geometryList[i].geometryExtension.extensionSectionList)
                            if (rws is NativeDataPLG_0510 native)
                                n = native.nativeDataStruct.nativeData;

                        if (n == null)
                            throw new Exception("Unable to find native data section");

                        throw new NotImplementedException("Unable to convert native data to Assimp");
                    }
                    else
                    {
                        foreach (var v in geo.morphTargets[0].vertices)
                        {
                            var vt = Vector3.Transform(new Vector3(v.X, v.Y, v.Z), transformMatrix);
                            mesh.Vertices.Add(new Vector3D(vt.X, vt.Y, vt.Z));
                        }

                        if ((geo.geometryFlags & GeometryFlags.hasNormals) != 0)
                            foreach (var v in geo.morphTargets[0].normals)
                                mesh.Normals.Add(new Vector3D(v.X, v.Y, v.Z));

                        if ((geo.geometryFlags & GeometryFlags.hasTextCoords) != 0)
                            foreach (var v in geo.textCoords)
                                mesh.TextureCoordinateChannels[0].Add(new Vector3D(v.X, v.Y, 0));

                        if ((geo.geometryFlags & GeometryFlags.hasVertexColors) != 0)
                            foreach (var color in geo.vertexColors)
                                mesh.VertexColorChannels[0].Add(new Color4D(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));

                        foreach (var t in geo.triangles)
                            if (t.materialIndex == j)
                                mesh.Faces.Add(new Face(new int[] { t.vertex1, t.vertex2, t.vertex3 }));
                    }
                    scene.Meshes.Add(mesh);
                }
                totalMaterials = scene.Materials.Count;
            }
        }
    }
}
