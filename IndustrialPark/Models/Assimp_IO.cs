using Assimp;
using Assimp.Configs;
using IndustrialPark.Models.CollisionTree;
using IndustrialPark.RenderWare;
using Newtonsoft.Json;
using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace IndustrialPark.Models
{
    public static class Assimp_IO
    {
        public static Vertex3 ToVertex3(this System.Numerics.Vector3 v) => new Vertex3(v.X, v.Y, v.Z);
        public static Vertex2 ToVertex2(this System.Numerics.Vector3 v) => new Vertex2(v.X, v.Y);

        public const int TRI_AND_VERTEX_LIMIT = 65535;

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

        private static Texture_0006 RWTextureFromAssimpMaterial(TextureSlot texture) =>
            new Texture_0006()
            {
                textureStruct = new TextureStruct_0001()
                {
                    FilterMode = TextureFilterMode.FILTERLINEARMIPLINEAR,
                    AddressModeU = RWTextureAddressModeFromAssimp(texture.WrapModeU),
                    AddressModeV = RWTextureAddressModeFromAssimp(texture.WrapModeV),
                    UseMipLevels = 1
                },
                diffuseTextureName = new String_0002(Path.GetFileNameWithoutExtension(texture.FilePath)),
                alphaTextureName = new String_0002(""),
                textureExtension = new Extension_0003()
            };

        // use wrap as default
        public static TextureAddressMode RWTextureAddressModeFromAssimp(TextureWrapMode mode) =>
            mode == TextureWrapMode.Clamp ? TextureAddressMode.TEXTUREADDRESSCLAMP :
            mode == TextureWrapMode.Decal ? TextureAddressMode.TEXTUREADDRESSBORDER :
            mode == TextureWrapMode.Mirror ? TextureAddressMode.TEXTUREADDRESSMIRROR :
            TextureAddressMode.TEXTUREADDRESSWRAP;

        /// <summary>
        /// Creates a bare minimum model used for collision only
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static RWSection CreateCollDFFFromAssimp(string fileName)
        {
            PostProcessSteps pps =
                PostProcessSteps.Debone |
                PostProcessSteps.FindInstances |
                PostProcessSteps.FindInvalidData |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.OptimizeMeshes |
                PostProcessSteps.PreTransformVertices |
                PostProcessSteps.RemoveComponent |
                PostProcessSteps.Triangulate;

            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new RemoveComponentConfig(ExcludeComponent.Normals | ExcludeComponent.Colors | ExcludeComponent.TexCoords));
            importer.SetConfig(new ColladaUseColladaNamesConfig(true));
            importer.SetConfig(new RootTransformationConfig(Matrix4x4.Identity));
            Scene scene = importer.ImportFile(fileName, pps);

            if (scene.Meshes.Sum(m => m.VertexCount) > TRI_AND_VERTEX_LIMIT || scene.Meshes.Sum(m => m.FaceCount) > TRI_AND_VERTEX_LIMIT)
                throw new ArgumentException("Model has too many vertices or triangles. Please import a simpler model.");

            List<Vertex3> Vertices = new();
            List<RenderWareFile.Triangle> Faces = new();
            int meshIndex = -1;
            foreach (var m in scene.Meshes)
            {
                int vertexCount = Vertices.Count;
                meshIndex++;

                if (scene.RootNode.ChildCount > 0)
                {
                    var assimpMat = scene.RootNode.Children[meshIndex].Transform;
                    var transform = new Matrix4x4(
                        assimpMat.M11, assimpMat.M12, assimpMat.M13, assimpMat.M14,
                        assimpMat.M21, assimpMat.M22, assimpMat.M23, assimpMat.M24,
                        assimpMat.M31, assimpMat.M32, assimpMat.M33, assimpMat.M34,
                        assimpMat.M41, assimpMat.M42, assimpMat.M43, assimpMat.M44);

                    var transformedVertices = m.Vertices.Select(v => Vector3.Transform(new Vector3(v.X, v.Y, v.Z), transform));
                    Vertices.AddRange(transformedVertices.Select(v => new Vertex3(v.X, v.Y, v.Z)).ToList());
                }
                else
                    Vertices.AddRange(m.Vertices.Select(v => new Vertex3(v.X, v.Y, v.Z)).ToList());

                foreach (var f in m.Faces)
                {
                    if (f.IndexCount != 3)
                        continue;
                    Faces.Add(new RenderWareFile.Triangle(0, (ushort)(f.Indices[0] + vertexCount), (ushort)(f.Indices[1] + vertexCount), (ushort)(f.Indices[2] + vertexCount)));
                }
            }

            SharpDX.BoundingSphere boundingSphere = SharpDX.BoundingSphere.FromPoints(Vertices.Select(v => new SharpDX.Vector3(v.X, v.Y, v.Z)).ToArray());

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
                                rotationMatrix = RenderWareFile.Sections.Matrix3x3.Identity,
                                parentFrame = -1,
                                unknown = 0
                            }
                        }
                    },
                    extensionList = new List<Extension_0003>()
                    {
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
                                    materialCount = 1
                                },
                                materialList = new Material_0007[]
                                {
                                    new Material_0007()
                                    {
                                        materialStruct = new MaterialStruct_0001()
                                        {
                                            unusedFlags = 0,
                                            color = new RenderWareFile.Color(255, 255, 255, 255),
                                            unusedInt2 = 0,
                                            isTextured = 0,
                                            ambient = 1f,
                                            specular = 1f,
                                            diffuse = 1f
                                        },
                                        texture = null,
                                        materialExtension = new Extension_0003()
                                        {
                                            extensionSectionList = new List<RWSection>()
                                        }
                                    }
                                }
                            },
                            geometryStruct = new GeometryStruct_0001()
                            {
                                geometryFlags = GeometryFlags.rpGEOMETRYPOSITIONS,
                                numTriangles = Faces.Count,
                                numVertices = Vertices.Count,
                                numMorphTargets = 1,
                                ambient = 1f,
                                specular = 1f,
                                diffuse = 1f,
                                vertexColors = null,
                                textCoords = null,
                                triangles = Faces.ToArray(),
                                morphTargets = new MorphTarget[]
                                {
                                    new MorphTarget()
                                    {
                                        hasNormals = 0,
                                        hasVertices = 1,
                                        sphereCenter = new Vertex3(
                                            boundingSphere.Center.X,
                                            boundingSphere.Center.Y,
                                            boundingSphere.Center.Z),
                                        radius = boundingSphere.Radius,
                                        vertices = Vertices.ToArray(),
                                        normals = null,
                                    }
                                }
                            },
                            geometryExtension = new Extension_0003()
                            {
                                extensionSectionList = new List<RWSection>()
                            }
                        }
                    }
                },
                atomicList = new List<Atomic_0014>() { new Atomic_0014()
                {
                    atomicStruct = new AtomicStruct_0001()
                    {
                        frameIndex = 0,
                        geometryIndex = 0,
                        flags = AtomicFlags.CollisionTest,
                        unused = 0
                    },
                    atomicExtension = new Extension_0003()
                    {
                        extensionSectionList = new List<RWSection>() 
                    }
                }
                },

                clumpExtension = new Extension_0003()
                {
                    extensionSectionList = new List<RWSection>()
                    {
                        new String_0002("COLL") // Custom implementation used to check if model asset is a collision model
                    }
                }
            };

            clump.geometryList.geometryList[0].geometryExtension.extensionSectionList.Add(new Collis_31().RpCollisionGeometryBuildData(clump.geometryList.geometryList[0]));

            return clump;

        }

        /// <summary>
        /// Create a renderware model
        /// </summary>
        /// <param name="fileName">Filepath to model</param>
        /// <param name="tristrip">Import as triangle strips if set to true, triangle list otherwise (Only has an effect when importing a Bin Mesh PLG)</param>
        /// <param name="flipUVs">Should UV coords be flipped?</param>
        /// <param name="ignoreMeshColors">Ignore mesh color</param>
        /// <param name="vertexcolors">Set to true if model should include vertex color</param>
        /// <param name="texcoords">Set to true if model should include uv coords</param>
        /// <param name="normals">Set to true if model should include normals (will be auto-generated if your model doesn't have any)</param>
        /// <param name="geoTriangles">Set to true if model should create triangles in geometry struct. Required for collision and will be used for rendering too if no Bin Mesh PLG is present</param>
        /// <param name="multiAtomic">Create multiple atomics per sub-mesh. Breaks collision and is only really useful for JSP</param>
        /// <param name="nativeData">Model will be in native data format (GameCube only)</param>
        /// <param name="collTree">If set to true, will create a collision tree (pre 3.6 format), used for fast collision checking</param>
        /// <param name="binMesh">If set to true, will create an optimized model topology used to draw in-game. Can be in triangle list or strip format. Geometry triangles are used for collision only then</param>
        /// <returns>A renderware model file</returns>
        /// <exception cref="ArgumentException"></exception>
        public static RWSection CreateDFFFromAssimp(string fileName, bool tristrip, bool flipUVs = false, bool ignoreMeshColors = true, bool vertexcolors = false, 
            bool texcoords = true, bool normals = true, bool geoTriangles = true, bool multiAtomic = false, bool nativeData = false, bool collTree = false, bool binMesh = false)
        {
            PostProcessSteps pps =
                PostProcessSteps.Debone |
                PostProcessSteps.FindInstances |
                PostProcessSteps.FindInvalidData |
                (normals ? PostProcessSteps.GenerateNormals : 0) |
                PostProcessSteps.JoinIdenticalVertices |
                PostProcessSteps.FindDegenerates |
                PostProcessSteps.ValidateDataStructure |
                PostProcessSteps.ImproveCacheLocality |
                PostProcessSteps.OptimizeMeshes |
                PostProcessSteps.RemoveComponent |
                PostProcessSteps.RemoveRedundantMaterials |
                PostProcessSteps.SortByPrimitiveType |
                (multiAtomic ? 0 : PostProcessSteps.PreTransformVertices) |
                PostProcessSteps.Triangulate |
                (multiAtomic ? PostProcessSteps.SplitLargeMeshes : 0) |
                (flipUVs ? 0 : PostProcessSteps.FlipUVs);

            AssimpContext importer = new AssimpContext();
            importer.SetConfig(new RemoveComponentConfig((normals ? 0 : ExcludeComponent.Normals) | (texcoords ? 0 : ExcludeComponent.TexCoords) | (vertexcolors ? 0 :ExcludeComponent.Colors)));
            importer.SetConfig(new ColladaUseColladaNamesConfig(true));
            importer.SetConfig(new RootTransformationConfig(System.Numerics.Matrix4x4.Identity));
            importer.SetConfig(new MeshVertexLimitConfig(300));
            importer.SetConfig(new SortByPrimitiveTypeConfig(PrimitiveType.Point | PrimitiveType.Line | PrimitiveType.Polygon));
            Scene scene = importer.ImportFile(fileName, pps);

            if (multiAtomic ? scene.Meshes.Any(m => m.VertexCount > TRI_AND_VERTEX_LIMIT || m.FaceCount > TRI_AND_VERTEX_LIMIT) :
                scene.Meshes.Sum(m => m.VertexCount) > TRI_AND_VERTEX_LIMIT || scene.Meshes.Sum(m => m.FaceCount) > TRI_AND_VERTEX_LIMIT)
                throw new ArgumentException("Model has too many vertices or triangles. Please import a simpler model.");

            //if (nativeData) // Force tristrip and multi-atomic when importing as native data
                //tristrip = multiAtomic = true;
            if (geoTriangles && !binMesh) // Force trilist when bin mesh is disabled
                tristrip = false;

            var materials = new List<Material_0007>();

            bool atomicNeedsMaterialEffects = false;

                foreach (var m in scene.Materials)
                {
                    materials.Add(new Material_0007()
                    {
                        materialStruct = new MaterialStruct_0001()
                        {
                            unusedFlags = 0,
                            color = ignoreMeshColors ?
                            new RenderWareFile.Color(255, 255, 255, 255) :
                            new RenderWareFile.Color(m.ColorDiffuse.X, m.ColorDiffuse.Y, m.ColorDiffuse.Z, m.ColorDiffuse.W),
                            unusedInt2 = 0,
                            isTextured = m.HasTextureDiffuse ? 1 : 0,
                            ambient = ignoreMeshColors ? 1f : m.ColorAmbient.W,
                            specular = ignoreMeshColors ? 1f : m.ColorSpecular.W,
                            diffuse = ignoreMeshColors ? 1f : m.ColorDiffuse.W
                        },
                        texture = m.HasTextureDiffuse ? RWTextureFromAssimpMaterial(m.TextureDiffuse) : null,
                        materialExtension = new Extension_0003()
                        {
                            extensionSectionList = m.HasTextureReflection ? new List<RWSection>()
                            {
                                new MaterialEffectsPLG_0120()
                                {
                                    isAtomicExtension = false,
                                    value = MaterialEffectType.EnvironmentMap,
                                    materialEffect1 = new MaterialEffectEnvironmentMap()
                                    {   
                                        EnvironmentMapTexture = RWTextureFromAssimpMaterial(m.TextureReflection),
                                        ReflectionCoefficient = m.Reflectivity,
                                        UseFrameBufferAlphaChannel = false
                                    }
                                }
                            } : new List<RWSection>()
                        },
                    });

                    atomicNeedsMaterialEffects |= m.HasTextureReflection;
                }

            List<Geometry_000F> geometries = new List<Geometry_000F>();

            List<Vertex3> vertices = new();
            List<Vertex3> Normals = new();
            List<Vertex2> textCoords = new();
            List<Vertex2> textCoords2 = new();
            List<RenderWareFile.Color> vertexColors = new();

            List<int> indices = new();
            List<RenderWareFile.Triangle> triangles = new();
            List<BinMesh> binMeshes = new();

            //Dictionary<System.Numerics.Vector3, ushort> normIndices = new();
            //Dictionary<System.Numerics.Vector3, ushort> uvIndices = new();
            //Dictionary<System.Numerics.Vector3, ushort> uvIndices2 = new();
            //Dictionary<System.Numerics.Vector4, ushort> colorIndices = new();

            Dictionary<int, Matrix> meshTransforms = new();
            void CollectMeshTransforms(Node node, Matrix parentTransform)
            {
                var nodeTransform = new Matrix((float)node.Transform.A1, (float)node.Transform.B1, (float)node.Transform.C1, (float)node.Transform.D1,
                        (float)node.Transform.A2, (float)node.Transform.B2, (float)node.Transform.C2, (float)node.Transform.D2,
                        (float)node.Transform.A3, (float)node.Transform.B3, (float)node.Transform.C3, (float)node.Transform.D3,
                        (float)node.Transform.A4, (float)node.Transform.B4, (float)node.Transform.C4, (float)node.Transform.D4);
                var globalTransform = parentTransform * nodeTransform;

                foreach (int meshIndex in node.MeshIndices)
                {
                    meshTransforms[meshIndex] = globalTransform;
                }

                foreach (var child in node.Children)
                {
                    CollectMeshTransforms(child, globalTransform);
                }
            }
            CollectMeshTransforms(scene.RootNode, Matrix.Identity);


            int meshIndex = -1;
            foreach (var m in scene.Meshes)
            {
                int totalVertices = vertices.Count;
                int materialIndex = multiAtomic ? 0 : m.MaterialIndex;
                meshIndex++;

                var transform = meshTransforms.ContainsKey(meshIndex) ? meshTransforms[meshIndex] : Matrix.Identity;

                var transformedVertices = m.Vertices.Select(v =>
                {
                    var vec = new Vector3((float)v.X, (float)v.Y, (float)v.Z);
                    return Vector3.Transform(vec, transform);
                });
                vertices.AddRange(transformedVertices.Select(v => new Vertex3(v.X, v.Y, v.Z)).ToList());
           
                //if (nativeData)
                //{
                //    for (int i = 0; i < m.VertexCount; i++)
                //    {
                //        Vertex3 norm = new Vertex3(m.Normals[i].X, m.Normals[i].Y, m.Normals[i].Z);
                //        if (!Normals.Contains(norm))
                //        {
                //            normIndices[m.Normals[i]] = (ushort)Normals.Count;
                //            Normals.Add(norm);
                //        }

                //        if (m.HasTextureCoords(0))
                //        {
                //            Vertex2 coord = new Vertex2(m.TextureCoordinateChannels[0][i].X, m.TextureCoordinateChannels[0][i].Y);
                //            if (!textCoords.Contains(coord))
                //            {
                //                uvIndices[m.TextureCoordinateChannels[0][i]] = (ushort)textCoords.Count;
                //                textCoords.Add(coord);
                //            }
                //        }

                //if (m.HasTextureCoords(1))
                //{
                //    Vertex2 coord = new Vertex2(m.TextureCoordinateChannels[1][i].X, m.TextureCoordinateChannels[1][i].Y);
                //    if (!textCoords2.Contains(coord))
                //    {
                //        uvIndices2[m.TextureCoordinateChannels[1][i]] = (ushort)textCoords2.Count;
                //        textCoords2.Add(coord);
                //    }
                //}

                //        if (m.HasVertexColors(0))
                //        {
                //            RenderWareFile.Color color = new RenderWareFile.Color(m.VertexColorChannels[0][i].R, m.VertexColorChannels[0][i].G, m.VertexColorChannels[0][i].B, m.VertexColorChannels[0][i].A);
                //            if (!colorIndices.ContainsKey(m.VertexColorChannels[0][i]))
                //            {
                //                colorIndices[m.VertexColorChannels[0][i]] = (ushort)vertexColors.Count;
                //                vertexColors.Add(color);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                Normals.AddRange(m.Normals.Select(n => new Vertex3(n.X, n.Y, n.Z)).ToList());
                    //textCoords2.AddRange(m.TextureCoordinateChannels[1].Select(t => new Vertex2(t.X, t.Y)).ToList());

                    if (m.HasTextureCoords(0))
                        textCoords.AddRange(m.TextureCoordinateChannels[0].Select(t => new Vertex2(t.X, t.Y)).ToList());
                    else if (texcoords)
                        for (int i = 0; i < m.VertexCount; i++)
                            textCoords.Add(new Vertex2(1f, 1f));

                    if (m.HasVertexColors(0))
                        vertexColors.AddRange(m.VertexColorChannels[0].Select(c => new RenderWareFile.Color(c.X, c.Y, c.Z, c.W)).ToList());
                    else if (vertexcolors)
                        for (int i = 0; i < m.VertexCount; i++)
                            vertexColors.Add(new RenderWareFile.Color(1f, 1f, 1f, 1f));
                //}

                foreach (var t in m.Faces)
                {
                    if (t.IndexCount != 3)
                        continue;
                    indices.AddRange([t.Indices[0] + totalVertices, t.Indices[1] + totalVertices, t.Indices[2] + totalVertices]);
                }

                NvTriStripDotNet.PrimitiveGroup[] primitives = [];
                var stripifier = new NvTriStripDotNet.NvStripifier()
                {
                    StitchStrips = nativeData ? false : true,
                    CacheSize = 16,
                    ListsOnly = tristrip ? false : true,
                    UseRestart = false,
                };

                if (stripifier.GenerateStrips(indices.ConvertAll(i => (ushort)i).ToArray(), out primitives, true))
                {
                    binMeshes.Add(new BinMesh()
                    {
                        materialIndex = materialIndex,
                        indexCount = primitives.Sum(p => p.IndexCount),
                        vertexIndices = primitives.SelectMany(p => p.Indices.Select(i => (int)i)).ToArray(),
                    });
                }
                else
                {
                    binMeshes.Add(new BinMesh()
                    {
                        materialIndex = materialIndex,
                        indexCount = indices.Count, 
                        vertexIndices = indices.ToArray(),
                    });
                    tristrip = false;
                }

                if (tristrip)
                    triangles.AddRange(RenderWareModelFile.FilterTriangleStrip(binMeshes[^1].vertexIndices, materialIndex));
                else
                    triangles.AddRange(RenderWareModelFile.FilterTriangleList(binMeshes[^1].vertexIndices, materialIndex));

                indices.Clear();
                if (!multiAtomic && meshIndex < (scene.MeshCount - 1))
                    continue;

                if (!geoTriangles)
                    triangles.Clear();
                if (!binMesh)
                    binMeshes.Clear();

                //if (nativeData)
                //{
                //    geometries.Add(ToNativeGeometry(multiAtomic ? [materials[m.MaterialIndex]] : materials.ToArray(), m,
                //        vertices.ToArray(), Normals.ToArray(), textCoords.ToArray(), textCoords2.ToArray(), vertexColors.ToArray(),
                //        normIndices, uvIndices, uvIndices2, colorIndices,
                //        triangles.ToArray(), primitives, ignoreMeshColors));
                //}
                //else
                //{
                    var geometry = ToGeometry(multiAtomic ? [materials[m.MaterialIndex]] : materials.ToArray(),
                        vertices.ToArray(), Normals.ToArray(), textCoords.ToArray(), textCoords2.ToArray(), vertexColors.ToArray(),
                        triangles.ToArray(), binMeshes.ToArray(), tristrip, ignoreMeshColors, collTree);

                    if (geoTriangles && collTree)
                        geometry.geometryExtension.extensionSectionList.Add(new Collis_31().RpCollisionGeometryBuildData(geometry));
                    geometries.Add(geometry);
                //}

                triangles.Clear();
                vertices.Clear();
                textCoords.Clear();
                textCoords2.Clear();
                vertexColors.Clear();
                Normals.Clear();
                //normIndices.Clear();
                //uvIndices.Clear();
                //uvIndices2.Clear();
                //colorIndices.Clear();
                binMeshes.Clear();
            }

            return ToClump(geometries, atomicNeedsMaterialEffects);
        }

        //private static Geometry_000F ToNativeGeometry(Material_0007[] materials, Mesh mesh,
        //    Vertex3[] vertices, Vertex3[] normals, Vertex2[] textCoords, Vertex2[] textCoords2, RenderWareFile.Color[] vertexColors,
        //    Dictionary<Vector3D, ushort> normalIndices, Dictionary<Vector3D, ushort> uvIndices, Dictionary<Vector3D, ushort> uvIndices2, Dictionary<Color4D, ushort> colorIndices,
        //    RenderWareFile.Triangle[] triangles, NvTriStripDotNet.PrimitiveGroup[] indices, bool ignoreMeshColor)
        //{
        //    BoundingSphere boundingSphere = BoundingSphere.FromPoints(vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray());
        //    TriangleDeclaration declaration = new() { strips = [] };

        //    foreach (NvTriStripDotNet.PrimitiveGroup primgroup in indices)
        //    {
        //        List<ushort[]> triEntries = new();

        //        foreach (var i in primgroup.Indices)
        //        {
        //            List<ushort> ind = new List<ushort> { i };

        //            if (normals.Any())
        //                ind.Add(normalIndices[mesh.Normals[i]]);
        //            if (textCoords.Any())
        //                ind.Add(uvIndices[mesh.TextureCoordinateChannels[0][i]]);
        //            if (textCoords2.Any())
        //                ind.Add(uvIndices2[mesh.TextureCoordinateChannels[1][i]]);
        //            if (vertexColors.Any())
        //                ind.Add(colorIndices[mesh.VertexColorChannels[0][i]]);
        //            triEntries.Add(ind.ToArray());
        //        }

        //        declaration.strips.Add(new TriangleStrip()
        //        {
        //            primitive = GXPrimitive.GX_TRIANGLESTRIP,
        //            numIndices = (byte)primgroup.IndexCount,
        //            indices = triEntries,
        //        });
        //    }

        //    List<VertexAttribute> declarations = new List<VertexAttribute>() { new VertexAttribute<Vertex3>(vertices, rwGCNVertexAttribute.rwGCNVA_POS) };
        //    if (normals.Any())
        //        declarations.Add(new VertexAttribute<Vertex3>(normals, rwGCNVertexAttribute.rwGCNVA_NRM));
        //    if (textCoords.Any())
        //        declarations.Add(new VertexAttribute<Vertex2>(textCoords, rwGCNVertexAttribute.rwGCNVA_TEX0));
        //    if (textCoords2.Any())
        //        declarations.Add(new VertexAttribute<Vertex2>(textCoords2, rwGCNVertexAttribute.rwGCNVA_TEX1));
        //    if (vertexColors.Any())
        //        declarations.Add(new VertexAttribute<RenderWareFile.Color>(vertexColors, rwGCNVertexAttribute.rwGCNVA_CLR0));

        //    return new Geometry_000F()
        //    {
        //        materialList = new MaterialList_0008()
        //        {
        //            materialListStruct = new MaterialListStruct_0001()
        //            {
        //                materialCount = materials.Length
        //            },
        //            materialList = materials
        //        },
        //        geometryStruct = new GeometryStruct_0001()
        //        {
        //            geometryFlags = GeometryFlags.rpGEOMETRYNATIVE |
        //                GeometryFlags.rpGEOMETRYLIGHTS |
        //                (textCoords2.Any() ? GeometryFlags.rpGEOMETRYTEXTURED2 : (textCoords.Any() ? GeometryFlags.rpGEOMETRYTEXTURED : 0)) |
        //                (vertexColors.Any() ? GeometryFlags.rpGEOMETRYPRELIT : 0) |
        //                GeometryFlags.rpGEOMETRYPOSITIONS |
        //                GeometryFlags.rpGEOMETRYTRISTRIP |
        //                (normals.Any() ? GeometryFlags.rpGEOMETRYNORMALS : 0) |
        //                (ignoreMeshColor ? 0 : GeometryFlags.rpGEOMETRYMODULATEMATERIALCOLOR),
        //            numTriangles = triangles.Length,
        //            numVertices = vertices.Length,
        //            numMorphTargets = 1,
        //            ambient = 1f,
        //            specular = 1f,
        //            diffuse = 1f,
        //            vertexColors = [],
        //            textCoords = [],
        //            triangles = [],
        //            sphereCenterX = boundingSphere.Center.X,
        //            sphereCenterY = boundingSphere.Center.Y,
        //            sphereCenterZ = boundingSphere.Center.Z,
        //            sphereRadius = boundingSphere.Radius,
        //            hasVertices = 0,
        //            hasNormals = 0,
        //        },
        //        geometryExtension = new Extension_0003()
        //        {
        //            extensionSectionList = new List<RWSection>()
        //            {
        //                new BinMeshPLG_050E()
        //                {
        //                     binMeshHeaderFlags = BinMeshHeaderFlags.TriangleStrip,
        //                     numMeshes = 1,
        //                     totalIndexCount = indices.Sum(i => i.IndexCount),
        //                     binMeshList = [new BinMesh() { indexCount = indices.Sum(i => i.IndexCount), materialIndex = 0, vertexIndices = [] }]
        //                },
        //                new NativeDataPLG_0510()
        //                {
        //                    nativeDataStruct = new NativeDataStruct_0001()
        //                    {
        //                        nativeDataType = NativeDataType.GameCube,
        //                        nativeData = new NativeDataGC()
        //                        {
        //                            token = 1,
        //                            serialNumber = 1,
        //                            flags = 0,
        //                            attr = declarations.ToArray(),
        //                            displayList = [declaration],
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    };
        //}

        private static Geometry_000F ToGeometry(Material_0007[] materials,
            Vertex3[] vertices, Vertex3[] normals, Vertex2[] textCoords, Vertex2[] textCoords2, RenderWareFile.Color[] vertexColors,
            RenderWareFile.Triangle[] triangles, BinMesh[] binMeshes, bool isTristrip, bool ignoreMeshColor, bool collTree)
        {
            SharpDX.BoundingSphere boundingSphere = SharpDX.BoundingSphere.FromPoints(vertices.Select(v => new SharpDX.Vector3(v.X, v.Y, v.Z)).ToArray());

            return new Geometry_000F()
            {
                materialList = new MaterialList_0008()
                {
                    materialListStruct = new MaterialListStruct_0001()
                    {
                        materialCount = materials.Length
                    },
                    materialList = materials
                },
                geometryStruct = new GeometryStruct_0001()
                {
                    geometryFlags =
                                GeometryFlags.rpGEOMETRYLIGHTS |
                                (textCoords2.Any() ? GeometryFlags.rpGEOMETRYTEXTURED2 : (textCoords.Any() ? GeometryFlags.rpGEOMETRYTEXTURED : 0)) |
                                (vertexColors.Any() ? GeometryFlags.rpGEOMETRYPRELIT : 0) |
                                GeometryFlags.rpGEOMETRYPOSITIONS |
                                (isTristrip ? GeometryFlags.rpGEOMETRYTRISTRIP : 0) |
                                (normals.Any() ? GeometryFlags.rpGEOMETRYNORMALS : 0) |
                                (ignoreMeshColor ? 0 : GeometryFlags.rpGEOMETRYMODULATEMATERIALCOLOR),
                    numTriangles = triangles.Length,
                    numVertices = vertices.Length,
                    numMorphTargets = 1,
                    ambient = 1f,
                    specular = 1f,
                    diffuse = 1f,
                    vertexColors = vertexColors,
                    textCoords = textCoords,
                    textCoords2 = textCoords2,
                    triangles = triangles,
                    morphTargets = new MorphTarget[]
                    {
                        new MorphTarget()
                        {
                            hasNormals = Convert.ToInt32(normals.Any()),
                            hasVertices = 1,
                            sphereCenter = new Vertex3(
                                boundingSphere.Center.X,
                                boundingSphere.Center.Y,
                                boundingSphere.Center.Z),
                            radius = boundingSphere.Radius,
                            vertices = vertices,
                            normals = normals,
                        }
                    }
                },
                geometryExtension = new Extension_0003()
                {
                    extensionSectionList = binMeshes.Any() ? new()
                    {
                        new BinMeshPLG_050E()
                        {
                             binMeshHeaderFlags = isTristrip ? BinMeshHeaderFlags.TriangleStrip : BinMeshHeaderFlags.TriangleList,
                             numMeshes = binMeshes.Length,
                             totalIndexCount = binMeshes.Sum(b => b.indexCount),
                             binMeshList = binMeshes
                        }
                    } : new()
                }
            };
        }

        private static RWSection ToClump(List<Geometry_000F> geometries, bool atomicNeedsMaterialEffects)
        {
            Clump_0010 clump = new Clump_0010()
            {
                clumpStruct = new ClumpStruct_0001()
                {
                    atomicCount = geometries.Count
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
                                rotationMatrix = RenderWareFile.Sections.Matrix3x3.Identity,
                                parentFrame = -1,
                                unknown = 0
                            }
                        }
                    },
                    extensionList = Enumerable.Range(0, 1).Select(_ => new Extension_0003()).ToList()
                },
                geometryList = new GeometryList_001A()
                {
                    geometryListStruct = new GeometryListStruct_0001()
                    {
                        numberOfGeometries = geometries.Count
                    },
                    geometryList = geometries
                },
                atomicList = Enumerable.Range(0, geometries.Count).Select(i => new Atomic_0014()
                {
                    atomicStruct = new AtomicStruct_0001()
                    {
                        frameIndex = 0,
                        geometryIndex = i,
                        flags = AtomicFlags.CollisionTestAndRender,
                        unused = 0
                    },
                    atomicExtension = new Extension_0003()
                    {
                        extensionSectionList = atomicNeedsMaterialEffects ? new List<RWSection>()
                        {
                            new MaterialEffectsPLG_0120()
                            {
                                isAtomicExtension = true,
                                value = (MaterialEffectType)1
                            }
                        }
                        : new List<RWSection>()
                    }
                }).ToList(),

                clumpExtension = new Extension_0003()
            };

            return clump;
        }

        public static void ExportAssimp(string fileName, RWSection[] bspFile, bool flipUVs, ExportFormatDescription format, string textureExtension, Matrix worldTransform)
        {
            Scene scene = new Scene();

            foreach (RWSection rw in bspFile)
                if (rw is World_000B w)
                    WorldToScene(scene, w, textureExtension);
                else if (rw is Clump_0010 c)
                    ClumpToScene(scene, c, textureExtension, worldTransform);

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
                //PostProcessSteps.GenerateNormals |
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

                var matdest = new Material()
                {
                    ColorDiffuse = new System.Numerics.Vector4(
                        mat.materialStruct.color.R / 255f,
                        mat.materialStruct.color.G / 255f,
                        mat.materialStruct.color.B / 255f,
                        mat.materialStruct.color.A / 255f),
                };

                if (mat.materialStruct.isTextured != 0)
                    matdest.TextureDiffuse = new TextureSlot()
                    {
                        FilePath = mat.texture.diffuseTextureName.stringString + textureExtension,
                        TextureType = TextureType.Diffuse
                    };

                matdest.Name = "mat_";

                if (mat.materialStruct.isTextured != 0)
                    matdest.Name += mat.texture.diffuseTextureName.stringString;
                else
                    matdest.Name += i.ToString();

                scene.Materials.Add(matdest);

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
            if (atomic.atomicSectorStruct.isNativeData)
            {
                NativeDataGC n = null;

                foreach (RWSection rws in atomic.atomicSectorExtension.extensionSectionList)
                    if (rws is NativeDataPLG_0510 native)
                        n = native.nativeDataStruct.nativeData;

                if (n == null)
                    throw new Exception("Unable to find native data section");

                GetNativeTriangleList(scene, n);
                return;
            }

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
                    mesh.Vertices.Add(new System.Numerics.Vector3(v.X, v.Y, v.Z));

                foreach (Vertex2 v in atomic.atomicSectorStruct.uvArray)
                    mesh.TextureCoordinateChannels[0].Add(new System.Numerics.Vector3(v.X, v.Y, 0f));

                foreach (RenderWareFile.Color c in atomic.atomicSectorStruct.colorArray)
                    mesh.VertexColorChannels[0].Add(new System.Numerics.Vector4(
                        c.R / 255f,
                        c.G / 255f,
                        c.B / 255f,
                        c.A / 255f));
            }
        }

        private static void GetNativeTriangleList(Scene scene, NativeDataGC n, int totalMaterials = 0)
        {
            List<Vertex3> vertexList_init = new List<Vertex3>();
            List<Vertex3> normalList_init = new List<Vertex3>();
            List<RenderWareFile.Color> colorList_init = new List<RenderWareFile.Color>();
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
                Mesh mesh = new Mesh(PrimitiveType.Triangle)
                {
                    MaterialIndex = td.MaterialIndex + totalMaterials,
                    Name = scene.Materials[td.MaterialIndex + totalMaterials].Name.Replace("mat_", "mesh_") + "_" + (scene.MeshCount + 1).ToString()
                };

                foreach (TriangleStrip tl in td.strips)
                {
                    int totalVertexIndices = mesh.VertexCount;
                    int vcount = 0;

                    foreach (ushort[] objectList in tl.indices)
                    {
                        for (int j = 0; j < objectList.Count(); j++)
                        {
                            if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_POS)
                            {
                                var v = vertexList_init[objectList[j]];
                                mesh.Vertices.Add(new System.Numerics.Vector3(v.X, v.Y, v.Z));
                                vcount++;
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_NRM)
                            {
                                var v = normalList_init[objectList[j]];
                                mesh.Normals.Add(new System.Numerics.Vector3(v.X, v.Y, v.Z));
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_CLR0)
                            {
                                var c = colorList_init[objectList[j]];
                                mesh.VertexColorChannels[0].Add(new System.Numerics.Vector4(
                                        c.R / 255f,
                                        c.G / 255f,
                                        c.B / 255f,
                                        c.A / 255f));
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_TEX0)
                            {
                                var v = textCoordList_init[objectList[j]];
                                mesh.TextureCoordinateChannels[0].Add(new System.Numerics.Vector3(v.X, v.Y, 0f));
                            }
                        }
                    }

                    bool control = true;
                    for (int i = 2; i < vcount; i++)
                    {
                        if (control)
                        {
                            mesh.Faces.Add(new Face(new int[] {
                                i - 2 + totalVertexIndices,
                                i - 1 + totalVertexIndices,
                                i + totalVertexIndices
                            }));

                            control = false;
                        }
                        else
                        {
                            mesh.Faces.Add(new Face(new int[] {
                                i - 2 + totalVertexIndices,
                                i + totalVertexIndices,
                                i - 1 + totalVertexIndices
                            }));

                            control = true;
                        }
                    }
                }

                scene.Meshes.Add(mesh);
            }
        }

        private static void ClumpToScene(Scene scene, Clump_0010 clump, string textureExtension, SharpDX.Matrix worldTransform)
        {
            int totalMaterials = 0;

            for (int i = 0; i < clump.geometryList.geometryList.Count; i++)
            {
                SharpDX.Matrix transformMatrix = RenderWareModelFile.CreateMatrix(clump.frameList, clump.atomicList[i].atomicStruct.frameIndex);

                for (int j = 0; j < clump.geometryList.geometryList[i].materialList.materialList.Length; j++)
                {
                    var geo = clump.geometryList.geometryList[i].geometryStruct;
                    var ext = clump.geometryList.geometryList[i].geometryExtension;
                    var mat = clump.geometryList.geometryList[i].materialList.materialList[j];

                    Material material = new Material()
                    {
                        ColorDiffuse = new System.Numerics.Vector4(
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

                    if ((geo.geometryFlags & GeometryFlags.rpGEOMETRYNATIVE) != 0)
                    {
                        foreach (RWSection rws in clump.geometryList.geometryList[i].geometryExtension.extensionSectionList)
                            if (rws is NativeDataPLG_0510 native)
                                if (native.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                                    GetNativeTriangleList(scene, native.nativeDataStruct.nativeData, totalMaterials);
                                else
                                    throw new Exception("Unable to find native data section");
                    }
                    else
                    {
                        foreach (var v in geo.morphTargets[0].vertices)
                        {
                            var vt = Vector3.Transform((Vector3)Vector3.Transform(new Vector3(v.X, v.Y, v.Z), transformMatrix), worldTransform);
                            mesh.Vertices.Add(new System.Numerics.Vector3(vt.X, vt.Y, vt.Z));
                        }

                        if ((geo.geometryFlags & GeometryFlags.rpGEOMETRYNORMALS) != 0)
                            foreach (var v in geo.morphTargets[0].normals)
                                mesh.Normals.Add(new System.Numerics.Vector3(v.X, v.Y, v.Z));

                        if ((geo.geometryFlags & (GeometryFlags.rpGEOMETRYTEXTURED | GeometryFlags.rpGEOMETRYTEXTURED2)) != 0)
                            foreach (var v in geo.textCoords)
                                mesh.TextureCoordinateChannels[0].Add(new System.Numerics.Vector3(v.X, v.Y, 0));

                        if ((geo.geometryFlags & GeometryFlags.rpGEOMETRYTEXTURED2) != 0)
                            foreach (var v in geo.textCoords2)
                                mesh.TextureCoordinateChannels[1].Add(new Vector3(v.X, v.Y, 0));

                        if ((geo.geometryFlags & GeometryFlags.rpGEOMETRYPRELIT) != 0)
                            foreach (var color in geo.vertexColors)
                                mesh.VertexColorChannels[0].Add(new System.Numerics.Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));

                        foreach (var t in geo.triangles)
                            if (t.materialIndex == j)
                                mesh.Faces.Add(new Face(new int[] { t.vertex1, t.vertex2, t.vertex3 }));

                        if (geo.triangles.Length == 0)
                            foreach (var ex in ext.extensionSectionList)
                                if (ex is BinMeshPLG_050E binmesh)
                                    if ((binmesh.binMeshHeaderFlags & BinMeshHeaderFlags.TriangleStrip) != 0)
                                        mesh.Faces.AddRange(RenderWareModelFile.FilterTriangleStrip(binmesh.binMeshList
                                            .Where(bin => bin.materialIndex == j)
                                            .SelectMany(list => list.vertexIndices).ToArray())
                                            .Select(tri => new Face([tri.vertex1, tri.vertex2, tri.vertex3])));
                                    else if ((binmesh.binMeshHeaderFlags & BinMeshHeaderFlags.TriangleList) != 0)
                                        mesh.Faces.AddRange(RenderWareModelFile.FilterTriangleList(binmesh.binMeshList
                                            .Where(bin => bin.materialIndex == j)
                                            .SelectMany(list => list.vertexIndices).ToArray())
                                            .Select(tri => new Face([tri.vertex1, tri.vertex2, tri.vertex3])));
                                    else
                                        throw new Exception($"Unsupported BinMeshHeaderFlags");

                        scene.Meshes.Add(mesh);
                    }
                }
                totalMaterials = scene.Materials.Count;
            }
        }
    }
}
