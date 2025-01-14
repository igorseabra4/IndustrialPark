using IndustrialPark.RenderData;
using Assimp;
using Newtonsoft.Json;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using IndustrialPark.AssetEditorColors;

namespace IndustrialPark
{
    public class RenderWareModelFile
    {
        private const string DefaultTexture = "default";
        public bool isNativeData = false;

        public static bool dontDrawInvisible = false;

        public List<SharpMesh> meshList;
        private void AddToMeshList(SharpMesh mesh)
        {
            meshList.Add(mesh);
            completeMeshList.Add(mesh);
        }
        public static List<SharpMesh> completeMeshList = new List<SharpMesh>();

        public uint vertexAmount;
        public uint triangleAmount;

        public List<Vector3> vertexListG;
        public List<Triangle> triangleList;
        private int triangleListOffset;

        public FogLightRenderData renderData;
        public JspRenderData jspRenderData;

        public RenderWareModelFile(SharpDevice device, RWSection[] rwSectionArray)
        {
            meshList = new List<SharpMesh>();

            vertexListG = new List<Vector3>();
            triangleList = new List<Triangle>();
            triangleListOffset = 0;
            List<Material_0007> materialList = new();

            foreach (RWSection rwSection in rwSectionArray)
            {
                if (rwSection is World_000B w)
                {
                    vertexAmount = w.worldStruct.numVertices;
                    triangleAmount = w.worldStruct.numTriangles;

                    foreach (Material_0007 m in w.materialList.materialList)
                    {
                        if (m.texture != null)
                        {
                            materialList.Add(m);
                        }
                        else
                        {
                            materialList.Add(null);
                        }
                    }
                    if (w.firstWorldChunk is AtomicSector_0009 a)
                    {
                        AddAtomic(device, a, materialList, w);
                    }
                    else if (w.firstWorldChunk is PlaneSector_000A p)
                    {
                        AddPlane(device, p, materialList, w);
                    }
                }
                else if (rwSection is Clump_0010 c)
                {
                    for (int g = 0; g < c.geometryList.geometryList.Count; g++)
                    {
                        AddGeometry(device, c.geometryList.geometryList[g], CreateMatrix(c.frameList, c.atomicList[g].atomicStruct.frameIndex));
                    }
                }
            }
        }

        public static Matrix CreateMatrix(FrameList_000E frameList, int frameIndex)
        {
            Matrix transform = Matrix.Identity;

            for (int f = 0; f < frameList.frameListStruct.frames.Count; f++)
            {
                if (frameIndex == f)
                {
                    Frame cf = frameList.frameListStruct.frames[f];

                    transform.M11 = cf.rotationMatrix.M11;
                    transform.M12 = cf.rotationMatrix.M12;
                    transform.M13 = cf.rotationMatrix.M13;
                    transform.M21 = cf.rotationMatrix.M21;
                    transform.M22 = cf.rotationMatrix.M22;
                    transform.M23 = cf.rotationMatrix.M23;
                    transform.M31 = cf.rotationMatrix.M31;
                    transform.M32 = cf.rotationMatrix.M32;
                    transform.M33 = cf.rotationMatrix.M33;

                    transform *= Matrix.Translation(cf.position.X, cf.position.Y, cf.position.Z);
                    break;
                }
            }

            return transform;
        }

        private void AddPlane(SharpDevice device, PlaneSector_000A planeSection, List<Material_0007> materialList, World_000B w)
        {
            if (planeSection.leftSection is AtomicSector_0009 al)
            {
                AddAtomic(device, al, materialList, w);
            }
            else if (planeSection.leftSection is PlaneSector_000A pl)
            {
                AddPlane(device, pl, materialList, w);
            }
            else
                throw new Exception();

            if (planeSection.rightSection is AtomicSector_0009 ar)
            {
                AddAtomic(device, ar, materialList, w);
            }
            else if (planeSection.rightSection is PlaneSector_000A pr)
            {
                AddPlane(device, pr, materialList, w);
            }
            else
                throw new Exception();
        }

        private void AddAtomic(SharpDevice device, AtomicSector_0009 AtomicSector, List<Material_0007> MaterialList, World_000B world)
        {
            if (AtomicSector.atomicSectorStruct.isNativeData)
            {
                AddNativeData(device, AtomicSector.atomicSectorExtension, MaterialList, Matrix.Identity, world);
                return;
            }

            List<VertexColoredTextured> vertexList = new List<VertexColoredTextured>();

            foreach (Vertex3 v in AtomicSector.atomicSectorStruct.vertexArray)
            {
                vertexList.Add(new VertexColoredTextured(new Vector3(v.X, v.Y, v.Z), new Vector2(), new SharpDX.Color()));
                vertexListG.Add(new Vector3(v.X, v.Y, v.Z));
            }

            for (int i = 0; i < vertexList.Count; i++)
            {
                RenderWareFile.Color c = AtomicSector.atomicSectorStruct.colorArray[i];

                VertexColoredTextured v = vertexList[i];
                v.Color = new SharpDX.Color(c.R, c.G, c.B, c.A);
                vertexList[i] = v;
            }

            for (int i = 0; i < vertexList.Count; i++)
            {
                Vertex2 tc = AtomicSector.atomicSectorStruct.uvArray[i];

                VertexColoredTextured v = vertexList[i];
                v.TextureCoordinate = new Vector2(tc.X, tc.Y);
                vertexList[i] = v;
            }

            List<SharpSubSet> SubsetList = new List<SharpSubSet>();
            List<int> indexList = new List<int>();
            int previousIndexCount = 0;

            for (int i = 0; i < MaterialList.Count; i++)
            {
                for (int j = 0; j < AtomicSector.atomicSectorStruct.triangleArray.Length; j++) // each (Triangle t in AtomicSector.atomicStruct.triangleArray)
                {
                    Triangle t = AtomicSector.atomicSectorStruct.triangleArray[j];
                    if (t.materialIndex == i)
                    {
                        indexList.Add(t.vertex1);
                        indexList.Add(t.vertex2);
                        indexList.Add(t.vertex3);

                        triangleList.Add(new Triangle(t.materialIndex, (ushort)(t.vertex1 + triangleListOffset), (ushort)(t.vertex2 + triangleListOffset), (ushort)(t.vertex3 + triangleListOffset)));
                    }
                }

                if (indexList.Count - previousIndexCount > 0)
                {
                    SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount,
                        TextureManager.GetTextureFromDictionary(MaterialList[i]), MaterialList[i] != null ? ((AssetColor)MaterialList[i]?.materialStruct.color).ToVector4() : Vector4.One, 
                        MaterialList[i]?.texture?.DiffuseTextureName ?? DefaultTexture,
                        EnablePrelight: (world.worldStruct.worldFlags & WorldFlags.HasVertexColors) != 0, EnableLights: (world.worldStruct.worldFlags & WorldFlags.UseLighting) != 0));
                }

                previousIndexCount = indexList.Count();
            }

            triangleListOffset += AtomicSector.atomicSectorStruct.vertexArray.Length;

            if (SubsetList.Count > 0)
                AddToMeshList(SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), SubsetList));
        }

        private void AddGeometry(SharpDevice device, Geometry_000F g, Matrix transformMatrix)
        {
            List<Material_0007> materialList = g.materialList.materialList.ToList();

            if ((g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYNATIVE) != 0)
            {
                AddNativeData(device, g.geometryExtension, materialList, transformMatrix, g);
                return;
            }
            vertexAmount += (uint)g.geometryStruct.numVertices;
            triangleAmount += (uint)g.geometryStruct.numTriangles;

            List<Vector3> vertexList1 = new List<Vector3>();
            List<Vector3> normalList = new List<Vector3>();
            List<Vector2> textCoordList = new List<Vector2>();
            List<SharpDX.Color> colorList = new List<SharpDX.Color>();

            if ((g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYPOSITIONS) != 0)
            {
                MorphTarget m = g.geometryStruct.morphTargets[0];
                foreach (Vertex3 v in m.vertices)
                {
                    Vector3 pos = (Vector3)Vector3.Transform(new Vector3(v.X, v.Y, v.Z), transformMatrix);
                    vertexList1.Add(pos);
                    vertexListG.Add(pos);
                }
            }

            if ((g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYNORMALS) != 0)
            {
                for (int i = 0; i < vertexList1.Count; i++)
                    normalList.Add(new Vector3(g.geometryStruct.morphTargets[0].normals[i].X, g.geometryStruct.morphTargets[0].normals[i].Y, g.geometryStruct.morphTargets[0].normals[i].Z));
            }
            else
                for (int i = 0; i < vertexList1.Count; i++)
                    normalList.Add(new Vector3(0f, 0f, 0f));

            if ((g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYPRELIT) != 0)
            {
                for (int i = 0; i < vertexList1.Count; i++)
                {
                    RenderWareFile.Color c = g.geometryStruct.vertexColors[i];
                    colorList.Add(new SharpDX.Color(c.R, c.G, c.B, c.A));
                }
            }
            else
            {
                for (int i = 0; i < vertexList1.Count; i++)
                    colorList.Add(new SharpDX.Color(1f, 1f, 1f, 1f));
            }

            if ((g.geometryStruct.geometryFlags & (GeometryFlags.rpGEOMETRYTEXTURED | GeometryFlags.rpGEOMETRYTEXTURED2)) != 0)
            {
                for (int i = 0; i < vertexList1.Count; i++)
                {
                    Vertex2 tc = g.geometryStruct.textCoords[i];
                    textCoordList.Add(new Vector2(tc.X, tc.Y));
                }
            }
            else
            {
                for (int i = 0; i < vertexList1.Count; i++)
                    textCoordList.Add(new Vector2());
            }

            List<SharpSubSet> SubsetList = new List<SharpSubSet>();
            List<int> indexList = new List<int>();
            int previousIndexCount = 0;

            BinMesh[] binmeshes = g.geometryExtension.extensionSectionList.Where(b => b.sectionIdentifier == Section.BinMeshPLG)
                .OfType<BinMeshPLG_050E>()
                .SelectMany(b => b.binMeshList)
                .ToArray();

            for (int i = 0; i < materialList.Count; i++)
            {
                if (binmeshes.Any())
                    indexList.AddRange(binmeshes.Where(b => b.materialIndex == i).SelectMany(b => b.vertexIndices));
                else
                    indexList.AddRange(GetIndicesFromTriangles(g.geometryStruct.triangles.Where(t => t.materialIndex == i).ToList()));

                if (indexList.Count - previousIndexCount > 0)
                {
                    SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount,
                        TextureManager.GetTextureFromDictionary(materialList[i]), 
                        materialList[i] != null && (g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYMODULATEMATERIALCOLOR) != 0 ? ((AssetColor)materialList[i]?.materialStruct.color).ToVector4() : Vector4.One,
                        materialList[i]?.texture?.DiffuseTextureName ?? DefaultTexture, materialList[i]?.materialStruct.diffuse ?? 1f, materialList[i]?.materialStruct.ambient ?? 1f,
                        (g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYPRELIT) != 0, (g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYLIGHTS) != 0));
                }

                previousIndexCount = indexList.Count();
            }

            if ((g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYTRISTRIP) != 0 && binmeshes.Any())
                triangleList.AddRange(FilterTriangleStrip(indexList.ToArray(), offset: triangleListOffset));
            else
                triangleList.AddRange(FilterTriangleList(indexList.ToArray(), offset: triangleListOffset));


            triangleListOffset += vertexList1.Count;

            if (SubsetList.Count > 0)
            {
                VertexColoredTexturedNormalized[] vertices = new VertexColoredTexturedNormalized[vertexList1.Count];
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new VertexColoredTexturedNormalized(vertexList1[i], textCoordList[i], colorList[i], normalList[i]);
                AddToMeshList(SharpMesh.Create(device,
                    vertices,
                    indexList.ToArray(),
                    SubsetList,
                    (g.geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYTRISTRIP) != 0 ? SharpDX.Direct3D.PrimitiveTopology.TriangleStrip : SharpDX.Direct3D.PrimitiveTopology.TriangleList));
            }
            else
                AddToMeshList(null);
        }

        private void AddNativeData(SharpDevice device, Extension_0003 extension, List<Material_0007> MaterialStream, Matrix transformMatrix, RWSection geo)
        {
            isNativeData = true;
            NativeDataGC n = null;
            NativeDataPS2 nativeps2 = null;

            foreach (RWSection rw in extension.extensionSectionList)
            {
                if (rw is BinMeshPLG_050E binmesh)
                {
                    if (binmesh.numMeshes == 0)
                        return;
                }
                if (rw is NativeDataPLG_0510 native)
                {
                    if (native.nativeDataStruct.nativeDataType == NativeDataType.GameCube)
                        n = native.nativeDataStruct.nativeData;
                    else if (native.nativeDataStruct.nativeDataType == NativeDataType.PS2)
                        nativeps2 = native.nativeDataStruct.nativeDataPs2;
                    break;
                }
            }

            if (n != null)
                AddGameCubeNativeData(device, extension, MaterialStream, transformMatrix, n, geo);
            else if (nativeps2 != null)
                AddPS2NativeData(device, extension, MaterialStream, transformMatrix, nativeps2, geo);
            else
                throw new Exception();
        }

        public void AddPS2NativeData(SharpDevice device, Extension_0003 extension, List<Material_0007> MaterialStream, Matrix transformMatrix, NativeDataPS2 nativeps2, RWSection geo)
        {
            List<Vertex3> vertexList1 = nativeps2.GetLinearVerticesList();
            List<Vertex3> normalList = nativeps2.GetLinearNormalsList();
            List<RenderWareFile.Color> colorList = nativeps2.GetLinearColorList();
            List<Vertex2> textCoordList = nativeps2.GetLinearTexCoordsList();
            List<Vertex4> vec4vertices = nativeps2.GetLinearVerticesFlagList();

            bool useLights = geo is World_000B ? (((World_000B)geo).worldStruct.worldFlags & WorldFlags.UseLighting) != 0 : (((Geometry_000F)geo).geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYLIGHTS) != 0;
            bool usePrelight = geo is World_000B ? (((World_000B)geo).worldStruct.worldFlags & WorldFlags.HasVertexColors) != 0 : (((Geometry_000F)geo).geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYPRELIT) != 0;
            bool modulateMaterialColor = geo is World_000B ? (((World_000B)geo).worldStruct.worldFlags & WorldFlags.ModulateMaterialColors) != 0 : (((Geometry_000F)geo).geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYMODULATEMATERIALCOLOR) != 0;

            List<VertexColoredTextured> vertexList = new List<VertexColoredTextured>();
            int previousAmount = 0;
            List<SharpSubSet> subSetList = new List<SharpSubSet>();


            if (vec4vertices.Count == 0)
            {
                for (int i = 0; i < vertexList1.Count; i++)
                {
                    Vector3 position = (Vector3)Vector3.Transform(new Vector3(vertexList1[i].X, vertexList1[i].Y, vertexList1[i].Z), transformMatrix);
                    SharpDX.Color color = new SharpDX.Color(colorList[i].R, colorList[i].G, colorList[i].B, colorList[i].A);
                    Vector2 texcoord = new Vector2(textCoordList[i].X, textCoordList[i].Y);

                    vertexList.Add(new VertexColoredTextured(position, texcoord, color));
                    vertexListG.Add(position);
                }
            }
            else
            {
                for (int i = 0; i < vec4vertices.Count; i++)
                {
                    Vector3 position = (Vector3)Vector3.Transform(new Vector3(vec4vertices[i].X, vec4vertices[i].Y, vec4vertices[i].Z), transformMatrix);
                    SharpDX.Color color = (colorList.Count != 0) ? new SharpDX.Color(colorList[i].R * 2, colorList[i].G * 2, colorList[i].B * 2, colorList[i].A * 2) : new SharpDX.Color(255, 255, 255, 255);
                    Vector2 texcoord = new Vector2(textCoordList[i].X, textCoordList[i].Y);

                    if ((vec4vertices[i].W & 0xffff) == 0x8000 && vertexList.Count != 0)
                    {
                        vertexList.Add(vertexList.Last());
                        vertexList.Add(vertexList.Last());
                    }

                    vertexList.Add(new VertexColoredTextured(position, texcoord, color));
                    vertexListG.Add(position);
                }
            }

            subSetList.Add(new SharpSubSet(previousAmount, vertexList.Count() - previousAmount, TextureManager.GetTextureFromDictionary(MaterialStream[0]),
                modulateMaterialColor ? ((AssetColor)MaterialStream[0]?.materialStruct.color).ToVector4() : Vector4.One,
                MaterialStream[0]?.texture?.DiffuseTextureName ?? DefaultTexture,
                MaterialStream[0]?.materialStruct.diffuse ?? 1f, MaterialStream[0]?.materialStruct.ambient ?? 1f, usePrelight, useLights));
            previousAmount = vertexList.Count();

            if (vertexList.Count > 0)
            {
                for (int i = 2; i < vertexList.Count; i++)
                    triangleList.Add(new Triangle(0, (ushort)(i + triangleListOffset - 2), (ushort)(i + triangleListOffset - 1), (ushort)(i + triangleListOffset)));

                triangleListOffset += vertexList.Count;

                AddToMeshList(SharpMesh.Create(device, vertexList.ToArray(), subSetList));
            }
            else
                AddToMeshList(null);

        }

        public void AddGameCubeNativeData(SharpDevice device, Extension_0003 extension, List<Material_0007> MaterialStream, Matrix transformMatrix, NativeDataGC n, RWSection geo)
        {
            List<Vertex3> vertexList1 = new List<Vertex3>();
            List<Vertex3> normalList = new List<Vertex3>();
            List<RenderWareFile.Color> colorList = new List<RenderWareFile.Color>();
            List<Vertex2> textCoordList = new List<Vertex2>();

            bool useLights = geo is World_000B ? (((World_000B)geo).worldStruct.worldFlags & WorldFlags.UseLighting) != 0 : (((Geometry_000F)geo).geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYLIGHTS) != 0;
            bool usePrelight = geo is World_000B ? (((World_000B)geo).worldStruct.worldFlags & WorldFlags.HasVertexColors) != 0 : (((Geometry_000F)geo).geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYPRELIT) != 0;
            bool modulateMaterialColor = geo is World_000B ? (((World_000B)geo).worldStruct.worldFlags & WorldFlags.ModulateMaterialColors) != 0 : (((Geometry_000F)geo).geometryStruct.geometryFlags & GeometryFlags.rpGEOMETRYMODULATEMATERIALCOLOR) != 0;

            foreach (VertexAttribute d in n.attr)
            {
                if (d.attr == rwGCNVertexAttribute.rwGCNVA_POS)
                {
                    var dec = (VertexAttribute<Vertex3>)d;
                    foreach (var v in dec.entries)
                        vertexList1.Add(v);
                }
                else if (d.attr == rwGCNVertexAttribute.rwGCNVA_NRM)
                {
                    var dec = (VertexAttribute<Vertex3>)d;
                    foreach (var v in dec.entries)
                        normalList.Add(v);
                }
                else if (d.attr == rwGCNVertexAttribute.rwGCNVA_CLR0)
                {
                    var dec = (VertexAttribute<RenderWareFile.Color>)d;
                    foreach (var c in dec.entries)
                        colorList.Add(c);
                }
                else if (d.attr == rwGCNVertexAttribute.rwGCNVA_TEX0)
                {
                    var dec = (VertexAttribute<Vertex2>)d;
                    foreach (var v in dec.entries)
                        textCoordList.Add(v);
                }
            }

            List<VertexColoredTexturedNormalized> vertexList = new();
            List<int> indexList = new List<int>();
            int k = 0;
            int previousAmount = 0;
            List<SharpSubSet> subSetList = new List<SharpSubSet>();

            foreach (TriangleDeclaration td in n.displayList)
            {
                foreach (TriangleStrip tl in td.strips)
                {
                    foreach (ushort[] objectList in tl.indices)
                    {
                        Vector3 position = new Vector3();
                        SharpDX.Color color = new SharpDX.Color(255, 255, 255, 255);
                        Vector2 textureCoordinate = new Vector2();
                        Vector3 normal = new Vector3();

                        for (int j = 0; j < objectList.Count(); j++)
                        {
                            if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_POS)
                            {
                                position = (Vector3)Vector3.Transform(
                                    new Vector3(
                                        vertexList1[objectList[j]].X,
                                        vertexList1[objectList[j]].Y,
                                        vertexList1[objectList[j]].Z),
                                    transformMatrix);
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_CLR0)
                            {
                                color = new SharpDX.Color(colorList[objectList[j]].R, colorList[objectList[j]].G, colorList[objectList[j]].B, colorList[objectList[j]].A);
                                if (color.A == 0)
                                    color = new SharpDX.Color(255, 255, 255, 255);
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_TEX0)
                            {
                                textureCoordinate.X = textCoordList[objectList[j]].X;
                                textureCoordinate.Y = textCoordList[objectList[j]].Y;
                            }
                            else if (n.attr[j].attr == rwGCNVertexAttribute.rwGCNVA_NRM)
                            {
                                normal = new Vector3(
                                        normalList[objectList[j]].X,
                                        normalList[objectList[j]].Y,
                                        normalList[objectList[j]].Z);
                            }
                        }

                        vertexList.Add(new VertexColoredTexturedNormalized(position, textureCoordinate, color, normal));

                        indexList.Add(k);
                        k++;

                        vertexListG.Add(position);
                    }

                    subSetList.Add(new SharpSubSet(previousAmount, vertexList.Count() - previousAmount,
                        TextureManager.GetTextureFromDictionary(MaterialStream[td.MaterialIndex]),
                        MaterialStream[td.MaterialIndex] != null && modulateMaterialColor ? ((AssetColor)MaterialStream[td.MaterialIndex]?.materialStruct.color).ToVector4() : Vector4.One,
                        MaterialStream[td.MaterialIndex]?.texture?.DiffuseTextureName ?? DefaultTexture,
                        MaterialStream[td.MaterialIndex]?.materialStruct.diffuse ?? 1f, MaterialStream[td.MaterialIndex]?.materialStruct.ambient ?? 1f,
                        usePrelight, useLights));

                    previousAmount = vertexList.Count();
                }
            }

            if (vertexList.Count > 0)
            {
                for (int i = 2; i < indexList.Count; i++)
                    triangleList.Add(new Triangle(0, (ushort)(i + triangleListOffset - 2), (ushort)(i + triangleListOffset - 1), (ushort)(i + triangleListOffset)));

                triangleListOffset += vertexList.Count;

                VertexColoredTexturedNormalized[] vertices = vertexList.ToArray();
                AddToMeshList(SharpMesh.Create(device, vertices, subSetList));
            }
            else
                AddToMeshList(null);
        }

        public void Render(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset, bool isSelected, bool[] atomicFlags)
        {
            renderData.worldViewProjection = world * renderer.viewProjection;
            renderData.world = world;
            renderData.ColorMultiplier = color;
            renderData.SelectedObjectColor = isSelected ? renderer.selectedObjectColor : Vector4.Zero;
            renderData.UvAnimOffset = (Vector4)uvAnimOffset;
            renderData.FogColor = SharpRenderer.Fog?.FogColor.ToVector4() ?? Vector4.Zero;
            renderData.FogStart = SharpRenderer.Fog?.StartDistance ?? 0f;
            renderData.FogEnd = SharpRenderer.Fog?.EndDistance ?? 0f;
            renderData.FogEnable = SharpRenderer.Fog != null && !AssetFOG.DontRender;
            renderData.VertexColorEnable = false;
            renderData.AlphaDiscard = 0;
            renderData.DirectionalLights = new DirectionalLight[8];

            Vector4 ambientColor = new Vector4(0f, 0f, 0f, 1f);
            int directionalLightCount = 0;
            if (AssetLKIT.SceneLightKit?.Lights != null)
            {
                foreach (var light in AssetLKIT.SceneLightKit.Lights)
                {
                    if (light.Type == 2)
                    {
                        if (directionalLightCount < AssetLKIT.MAX_DIRECTIONAL_LIGHTS)
                        {
                            renderData.DirectionalLights[directionalLightCount] = new DirectionalLight() { Direction = (Vector4)(light.DirectionVector), Color = light.ColorRGBA.ToVector4() };
                        }
                        directionalLightCount++;
                    }
                    else
                    {
                        ambientColor.X += light.ColorRed * light.ColorAlpha;
                        ambientColor.Y += light.ColorGreen * light.ColorAlpha;
                        ambientColor.Z += light.ColorBlue * light.ColorAlpha;
                    }
                }
            }
            renderData.AmbientColor = ambientColor;

            renderer.device.SetBlendStateAlphaBlend();
            renderer.device.SetCullModeNone();
            renderer.device.SetDefaultDepthState();
            renderer.device.ApplyRasterState();
            renderer.device.UpdateAllStates();

            renderer.device.DeviceContext.VertexShader.SetConstantBuffer(0, renderer.fogLightBuffer.Buffer);
            renderer.device.DeviceContext.PixelShader.SetConstantBuffer(0, renderer.fogLightBuffer.Buffer);
#if DEBUG
            SharpRenderer.TotalObjectsDrawn++;
#endif

            for (int i = meshList.Count - 1; i >= 0; i--)
            {
                if (meshList[i] == null || (dontDrawInvisible && atomicFlags[i]))
                    continue;

                meshList[i].Begin(renderer.device);
                for (int j = 0; j < meshList[i].SubSets.Count; j++)
                {
                    renderData.LightingEnable = !AssetLKIT.DontRender && meshList[i].SubSets[j].EnableLights;
                    renderData.DiffuseMult = meshList[i].SubSets[j].DiffuseMult;
                    renderData.AmbientMult = meshList[i].SubSets[j].AmbientMult;

                    renderData.MaterialColor = meshList[i].SubSets[j].DiffuseColor;
                    renderData.MaterialColor.X *= color.Y;
                    renderData.MaterialColor.Y *= color.Y;
                    renderData.MaterialColor.Z *= color.Z;
                    renderData.MaterialColor.W = color.W;

                    renderer.fogLightBuffer.UpdateValue(renderData);
                    renderer.fogLightShader.Apply();
                    meshList[i].Draw(renderer.device, j);
                }
#if DEBUG
                SharpRenderer.TotalVerticesDrawn += meshList[i].VerticesAmount;
                SharpRenderer.TotalAtomicsDrawn++;
#endif
            }
        }

        public void RenderJsp(SharpRenderer renderer, Matrix world, bool isSelected, bool[] atomicFlags, xJSPNodeInfo[] nodeinfos)
        {
            jspRenderData.worldViewProjection = world * renderer.viewProjection;
            jspRenderData.FogColor = SharpRenderer.Fog?.FogColor.ToVector4() ?? Vector4.Zero;
            jspRenderData.FogStart = SharpRenderer.Fog?.StartDistance ?? 0f;
            jspRenderData.FogEnd = SharpRenderer.Fog?.EndDistance ?? 0f;
            jspRenderData.FogEnable = SharpRenderer.Fog != null && !AssetFOG.DontRender;
            jspRenderData.SelectedObjectColor = isSelected ? renderer.selectedObjectColor : Vector4.Zero;

            renderer.device.SetBlendStateAlphaBlend();

            renderer.device.DeviceContext.VertexShader.SetConstantBuffer(0, renderer.jspBuffer.Buffer);
            renderer.device.DeviceContext.PixelShader.SetConstantBuffer(0, renderer.jspBuffer.Buffer);

#if DEBUG
            SharpRenderer.TotalObjectsDrawn++;
#endif

            for (int i = meshList.Count - 1; i >= 0; i--)
            {
                if (meshList[i] == null || (dontDrawInvisible && atomicFlags[i]))
                    continue;

                renderer.device.SetDefaultDepthState();
                renderer.device.SetCullModeDefault();

                if (nodeinfos != null && nodeinfos.Length == meshList.Count)
                {
                    if ((nodeinfos?[i].nodeFlags & 2) != 0)
                        renderer.device.DisableDepthBufferWrite();
                    if ((nodeinfos?[i].nodeFlags & 4) != 0)
                        renderer.device.SetCullModeNone();
                }
                renderer.device.UpdateAllStates();
                renderer.device.ApplyRasterState();

                meshList[i].Begin(renderer.device);
                for (int j = 0; j < meshList[i].SubSets.Count; j++)
                {
                    jspRenderData.VertexColorEnable = SharpRenderer.RenderVertexColors && meshList[i].SubSets[j].EnablePrelight;
                    jspRenderData.MaterialColor = meshList[i].SubSets[j].DiffuseColor;

                    renderer.jspBuffer.UpdateValue(jspRenderData);
                    renderer.jspShader.Apply();

                    meshList[i].Draw(renderer.device, j);
                }
#if DEBUG
                SharpRenderer.TotalVerticesDrawn += meshList[i].VerticesAmount;
                SharpRenderer.TotalAtomicsDrawn++;
#endif
            }
        }

        public void RenderPipt(SharpRenderer renderer, Matrix world, Vector4 color, Vector3 uvAnimOffset, bool isSelected, bool[] atomicFlags, Dictionary<uint, PipeInfo> pipeEntries)
        {
            renderData.worldViewProjection = world * renderer.viewProjection;
            renderData.world = world;
            renderData.ColorMultiplier = color;
            renderData.SelectedObjectColor = isSelected ? renderer.selectedColor : Vector4.Zero;
            renderData.UvAnimOffset = (Vector4)uvAnimOffset;
            renderData.FogColor = SharpRenderer.Fog?.FogColor.ToVector4() ?? Vector4.Zero;
            renderData.FogStart = SharpRenderer.Fog?.StartDistance ?? 0f;
            renderData.FogEnd = SharpRenderer.Fog?.EndDistance ?? 0f;
            renderData.FogEnable = SharpRenderer.Fog != null && !AssetFOG.DontRender;

            renderData.DirectionalLights = new DirectionalLight[8];
            Vector4 ambientColor = new Vector4(0f, 0f, 0f, 1f);
            int directionalLightCount = 0;
            if (AssetLKIT.SceneLightKit?.Lights != null)
            {
                foreach (var light in AssetLKIT.SceneLightKit.Lights)
                {
                    if (light.Type == 2)
                    {
                        if (directionalLightCount < AssetLKIT.MAX_DIRECTIONAL_LIGHTS)
                        {
                            renderData.DirectionalLights[directionalLightCount] = new DirectionalLight() { Direction = (Vector4)(light.DirectionVector), Color = light.ColorRGBA.ToVector4() };
                        }
                        directionalLightCount++;
                    }
                    else
                    {
                        ambientColor.X += light.ColorRed * light.ColorAlpha;
                        ambientColor.Y += light.ColorGreen * light.ColorAlpha;
                        ambientColor.Z += light.ColorBlue * light.ColorAlpha;
                    }
                }
            }
            renderData.AmbientColor = ambientColor;

#if DEBUG
            SharpRenderer.TotalObjectsDrawn++;
#endif

            for (int i = meshList.Count - 1; i >= 0; i--)
            {
                if (meshList[i] == null || (dontDrawInvisible && atomicFlags[i]))
                    continue;

                renderer.device.DeviceContext.VertexShader.SetConstantBuffer(0, renderer.fogLightBuffer.Buffer);
                renderer.device.DeviceContext.PixelShader.SetConstantBuffer(0, renderer.fogLightBuffer.Buffer);

                renderData.AlphaDiscard = 0;
                renderer.device.SetDefaultDepthState();
                renderer.device.SetBlendStateAlphaBlend();
                renderer.device.SetCullModeNone();

                PipeInfo pipe;
                bool drawTwice = false;
                if (pipeEntries.TryGetValue((uint)i, out pipe) || pipeEntries.TryGetValue(uint.MaxValue, out pipe))
                {
                    renderData.AlphaDiscard = pipe.AlphaDiscard / 255f;
                    renderer.device.SetBlend(BlendOperation.Add, 
                        pipe.SourceBlend != BlendFactorType.None ? pipe.GetSharpBlendMode(true) : BlendOption.SourceAlpha, 
                        pipe.DestinationBlend != BlendFactorType.None ? pipe.GetSharpBlendMode(false) : BlendOption.InverseSourceAlpha);

                    if (pipe.ZWriteMode != ZWriteMode.Enabled)
                    {
                        renderer.device.DisableDepthBufferWrite();
                        if (pipe.ZWriteMode == ZWriteMode.ZFirst)
                            drawTwice = true;
                    }
                    if (pipe.IgnoreFog)
                        renderData.FogEnable = false;
                    if (pipe.LightingMode != LightingMode.LightKit)
                        renderData.VertexColorEnable = true;
                    if (pipe.CullMode >= PiptCullMode.Back)
                    {
                        renderer.device.SetCullModeNormal();
                        if (pipe.CullMode == PiptCullMode.BackThenFront)
                            drawTwice = true;
                    }

                }
                renderer.device.ApplyRasterState();
                renderer.device.UpdateAllStates();

            Draw:
                meshList[i].Begin(renderer.device);
                for (int j = 0; j < meshList[i].SubSets.Count; j++)
                {
                    renderData.VertexColorEnable = SharpRenderer.RenderVertexColors &&
                        meshList[i].SubSets[j].EnablePrelight &&
                        ((pipe?.LightingMode == LightingMode.Prelight || pipe?.LightingMode == LightingMode.Both) |
                        (pipe?.LightingMode == LightingMode.LightKit && AssetLKIT.SceneLightKit == null));

                    renderData.LightingEnable = !AssetLKIT.DontRender && meshList[i].SubSets[j].EnableLights && pipe?.LightingMode != LightingMode.Prelight;
                    renderData.DiffuseMult = meshList[i].SubSets[j].DiffuseMult;
                    renderData.AmbientMult = meshList[i].SubSets[j].AmbientMult;

                    renderData.MaterialColor = meshList[i].SubSets[j].DiffuseColor;
                    renderData.MaterialColor.X *= color.X;
                    renderData.MaterialColor.Y *= color.Y;
                    renderData.MaterialColor.Z *= color.Z;
                    renderData.MaterialColor.W = color.W;

                    renderer.fogLightBuffer.UpdateValue(renderData);
                    renderer.fogLightShader.Apply();

                    meshList[i].Draw(renderer.device, j);
                }
#if DEBUG
                SharpRenderer.TotalVerticesDrawn += meshList[i].VerticesAmount;
                SharpRenderer.TotalAtomicsDrawn++;
#endif

                if (drawTwice)
                {
                    if (pipe.CullMode == PiptCullMode.BackThenFront)
                        renderer.device.SetCullModeReverse();
                    if (pipe.ZWriteMode == ZWriteMode.ZFirst)
                        renderer.device.SetDefaultDepthState();
                    renderer.device.ApplyRasterState();
                    drawTwice = false;
                    goto Draw;
                }
            }
        }

        public void Dispose()
        {
            if (meshList == null)
                return;

            foreach (SharpMesh m in meshList)
            {
                completeMeshList.Remove(m);
                if (m != null)
                    m.Dispose();
            }
            meshList.Clear();
        }

        public static bool IsDegenerate(int i1, int i2, int i3)
        {
            return i1 == i2 || i2 == i3 || i1 == i3;
        }

        /// <summary>
        /// Create a list of RenderWareFile.Triangles from a raw triangle list indices array (like used in BinMesh PLG).
        /// </summary>
        /// <param name="indices">Raw indices array</param>
        /// <param name="materialIndex">Material index, default is 0</param>
        /// <param name="offset">Offset to apply to every index</param>
        /// <returns></returns>
        public static List<Triangle> FilterTriangleList(int[] indices, int materialIndex = 0, int offset = 0)
        {
            List<Triangle> triangles = new();

            for (int i = 0; i < indices.Length; i += 3)
            {
                int i1 = indices[i] + offset;
                int i2 = indices[i + 1] + offset;
                int i3 = indices[i + 2] + offset;

                triangles.Add(new Triangle((ushort)materialIndex, (ushort)i1, (ushort)i2, (ushort)i3));
            }

            return triangles;
        }

        /// <summary>
        /// Create a list of RenderWareFile.Triangles from a raw triangle strip indices array (like used in BinMesh PLG).
        /// </summary>
        /// <param name="indices">Raw indices array</param>
        /// <param name="materialIndex">Material index, default is 0</param>
        /// <param name="offset">Offset to apply to every index</param>
        /// <returns>List of RenderWareFile.Triangles excluding degenerated triangles</returns>
        public static List<Triangle> FilterTriangleStrip(int[] indices, int materialIndex = 0, int offset = 0)
        {
            List<Triangle> triangles = new();

            for (int i = 2; i < indices.Length; i++)
            {
                if (IsDegenerate(indices[i - 2], indices[i - 1], indices[i]))
                    continue;

                if (i % 2 == 0)
                    triangles.Add(new Triangle((ushort)materialIndex, (ushort)(indices[i - 2] + offset), (ushort)(indices[i - 1] + offset), (ushort)(indices[i] + offset)));
                else
                    triangles.Add(new Triangle((ushort)materialIndex, (ushort)(indices[i - 1] + offset), (ushort)(indices[i - 2] + offset), (ushort)(indices[i] + offset)));
            }

            return triangles;
        }

        public static int[] GetIndicesFromTriangles(List<Triangle> triangles)
        {
            if (triangles == null || triangles.Count == 0)
                return Array.Empty<int>();

            return triangles.SelectMany(tri => new int[] { tri.vertex1, tri.vertex2, tri.vertex3 }).ToArray();
        }
    }
}
