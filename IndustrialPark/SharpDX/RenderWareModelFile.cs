using RenderWareFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public class RenderWareModelFile
    {
        public string FileName;

        private const string DefaultTexture = "default";
        public RWSection[] rwChunkList;
        
        public List<string> MaterialList = new List<string>();
        public string ChunkName;
        public int ChunkNumber;
        public bool isNoCulling = false;
        public bool isCollision = false;

        public List<SharpMesh> meshList;
        public static List<SharpMesh> completeMeshList = new List<SharpMesh>();

        public uint vertexAmount;
        public uint triangleAmount;

        public RenderWareModelFile(string fileName)
        {
            FileName = fileName;
        }

        public void SetForRendering()
        {
            meshList = new List<SharpMesh>();

            foreach (RWSection rwSection in rwChunkList)
            {
                if (rwSection is World_000B w)
                {
                    vertexAmount = w.worldStruct.numVertices;
                    triangleAmount = w.worldStruct.numTriangles;

                    foreach (Material_0007 m in w.materialList.materialList)
                    {
                        if (isCollision)
                        {
                            MaterialList.Add(m.materialStruct.color.ToString());
                        }
                        else if (m.texture != null)
                        {
                            string textureName = m.texture.diffuseTextureName.stringString;
                            MaterialList.Add(textureName);
                        }
                        else
                        {
                            MaterialList.Add(DefaultTexture);
                        }
                    }
                    if (w.firstWorldChunk is AtomicSection_0009 a)
                    {
                        AddAtomic(a);
                    }
                    else if (w.firstWorldChunk is PlaneSection_000A p)
                    {
                        AddPlane(p);
                    }
                }
                else if (rwSection is Clump_0010 c)
                {
                    foreach (Geometry_000F g in c.geometryList.geometryList)
                    {
                        AddGeometry(g);                        
                    }
                }
            }
        }

        void AddPlane(PlaneSection_000A planeSection)
        {
            if (planeSection.leftSection is AtomicSection_0009 al)
            {
                AddAtomic(al);
            }
            else if (planeSection.leftSection is PlaneSection_000A pl)
            {
                AddPlane(pl);
            }
            else throw new Exception();

            if (planeSection.rightSection is AtomicSection_0009 ar)
            {
                AddAtomic(ar);
            }
            else if (planeSection.rightSection is PlaneSection_000A pr)
            {
                AddPlane(pr);
            }
            else throw new Exception();
        }

        void AddAtomic(AtomicSection_0009 atomicSection)
        {
            if (atomicSection.atomicStruct.isNativeData)
            {
                AddNativeData(atomicSection.atomicExtension, MaterialList);
                return;
            }
            
            List<VertexColoredTextured> vertexList = new List<VertexColoredTextured>();

            foreach (Vertex3 v in atomicSection.atomicStruct.vertexArray)
            {
                vertexList.Add(new VertexColoredTextured(new Vector3(v.X, v.Y, v.Z), new Vector2(), new SharpDX.Color()));
            }

            if (isCollision)
            {
            //    for (int i = 0; i < vertexList.Count; i++)
            //    {
            //        VertexColoredTextured v = vertexList[i];
            //        v.Color = SharpDX.Color.White.ToVector4();
            //        vertexList[i] = v;
            //    }
            }
            else
            {
                for (int i = 0; i < vertexList.Count; i++)
                {
                    RenderWareFile.Color c = atomicSection.atomicStruct.colorArray[i];

                    VertexColoredTextured v = vertexList[i];
                    v.Color = new SharpDX.Color(c.R, c.G, c.B, c.A);
                    vertexList[i] = v;
                }

                for (int i = 0; i < vertexList.Count; i++)
                {
                    TextCoord tc = atomicSection.atomicStruct.uvArray[i];

                    VertexColoredTextured v = vertexList[i];
                    v.TextureCoordinate = new Vector2(tc.X, tc.Y);
                    vertexList[i] = v;
                }
            }

            List<SharpSubSet> SubsetList = new List<SharpSubSet>();
            List<int> indexList = new List<int>();
            int previousIndexCount = 0;

            for (int i = 0; i < MaterialList.Count; i++)
            {
                foreach (Triangle t in atomicSection.atomicStruct.triangleArray)
                {
                    if (t.materialIndex == i)
                    {
                        indexList.Add(t.vertex1);
                        indexList.Add(t.vertex2);
                        indexList.Add(t.vertex3);
                        
                        if (isCollision)
                        {
                            RenderWareFile.Color c = RenderWareFile.Color.FromString(MaterialList[i]);
                            SharpDX.Color color = new SharpDX.Color(c.R, c.G, c.B, c.A);

                            VertexColoredTextured v1 = vertexList[t.vertex1];
                            v1.Color = color;
                            vertexList[t.vertex1] = v1;

                            VertexColoredTextured v2 = vertexList[t.vertex2];
                            v2.Color = color;
                            vertexList[t.vertex2] = v2;

                            VertexColoredTextured v3 = vertexList[t.vertex3];
                            v3.Color = color;
                            vertexList[t.vertex3] = v3;
                        }
                    }
                }

                if (indexList.Count - previousIndexCount > 0)
                {
                    if (SharpRenderer.TextureStream.ContainsKey(MaterialList[i]))
                        SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount, SharpRenderer.TextureStream[MaterialList[i]]));
                    else
                        SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount, SharpRenderer.whiteDefault));
                }

                previousIndexCount = indexList.Count();
            }

            if (SubsetList.Count > 0)
            {
                meshList.Add(SharpMesh.Create(SharpRenderer.device, vertexList.ToArray(), indexList.ToArray(), SubsetList));
                completeMeshList.Add(meshList.Last());
            }
        }

        void AddGeometry(Geometry_000F g)
        {
            List<string> MaterialList = new List<string>();
            foreach (Material_0007 m in g.materialList.materialList)
            {
                if (m.texture != null)
                {
                    string textureName = m.texture.diffuseTextureName.stringString;
                    //if (!MaterialList.Contains(textureName))
                        MaterialList.Add(textureName);
                }
                else
                    MaterialList.Add(DefaultTexture);
            }

            if (g.geometryStruct.geometryFlags2 == 0x0101)
            {
                AddNativeData(g.geometryExtension, MaterialList);
                return;
            }

            List<VertexColoredTextured> vertexList = new List<VertexColoredTextured>();

            if ((g.geometryStruct.geometryFlags & (int)GeometryFlags.hasVertexPositions) != 0)
            {
                foreach (Vertex3 v in g.geometryStruct.morphTargets[0].vertices)
                {
                    vertexList.Add(new VertexColoredTextured(new Vector3(v.X, v.Y, v.Z),
                        new Vector2(),
                        SharpDX.Color.White
                    ));
                }
            }

            if ((g.geometryStruct.geometryFlags & (int)GeometryFlags.hasVertexColors) != 0)
            {
                for (int i = 0; i < vertexList.Count; i++)
                {
                    RenderWareFile.Color c = g.geometryStruct.vertexColors[i];

                    VertexColoredTextured v = vertexList[i];
                    v.Color = new SharpDX.Color(c.R, c.G, c.B, c.A);
                    vertexList[i] = v;
                }
            }
            else
            {
                for (int i = 0; i < vertexList.Count; i++)
                {
                    VertexColoredTextured v = vertexList[i];
                    v.Color = SharpDX.Color.White;
                    vertexList[i] = v;
                }
            }

            if ((g.geometryStruct.geometryFlags & (int)GeometryFlags.hasTextCoords) != 0)
            {
                for (int i = 0; i < vertexList.Count; i++)
                {
                    TextCoord tc = g.geometryStruct.textCoords[i];

                    VertexColoredTextured v = vertexList[i];
                    v.TextureCoordinate = new Vector2(tc.X, tc.Y);
                    vertexList[i] = v;
                }
            }

            List<SharpSubSet> SubsetList = new List<SharpSubSet>();
            List<int> indexList = new List<int>();
            int previousIndexCount = 0;

            for (int i = 0; i < MaterialList.Count; i++)
            {
                foreach (Triangle t in g.geometryStruct.triangles)
                {
                    if (t.materialIndex == i)
                    {
                        indexList.Add(t.vertex1);
                        indexList.Add(t.vertex2);
                        indexList.Add(t.vertex3);
                    }
                }

                if (indexList.Count - previousIndexCount > 0)
                {
                    if (SharpRenderer.TextureStream.ContainsKey(MaterialList[i]))
                        SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount, SharpRenderer.TextureStream[MaterialList[i]]));
                    else
                        SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount, SharpRenderer.whiteDefault));
                }

                previousIndexCount = indexList.Count();
            }

            if (SubsetList.Count > 0)
            {
                meshList.Add(SharpMesh.Create(SharpRenderer.device, vertexList.ToArray(), indexList.ToArray(), SubsetList));
                completeMeshList.Add(meshList.Last());
            }
        }

        void AddNativeData(Extension_0003 extension, List<string> MaterialStream)
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
                    break;
                }
            }

            if (n == null) throw new Exception(ChunkName + ChunkNumber.ToString());

            List<Vertex3> vertexList1 = new List<Vertex3>();
            List<RenderWareFile.Color> colorList = new List<RenderWareFile.Color>();
            List<TextCoord> textCoordList = new List<TextCoord>();
            
            foreach (Declaration d in n.declarations)
            {
                foreach (object o in d.entryList)
                {
                    if (o is Vertex3 v)
                        vertexList1.Add(v);
                    else if (o is RenderWareFile.Color c)
                        colorList.Add(c);
                    else if (o is TextCoord t)
                        textCoordList.Add(t);
                    else throw new Exception();
                }
            }

            List<VertexColoredTextured> vertexList = new List<VertexColoredTextured>();
            List<int> indexList = new List<int>();
            int k = 0;
            int previousAmount = 0;
            List<SharpSubSet> subSetList = new List<SharpSubSet>();

            foreach (TriangleDeclaration td in n.triangleDeclarations)
            {
                foreach (TriangleList tl in td.TriangleListList)
                {
                    foreach (int[] objectList in tl.entries)
                    {
                        VertexColoredTextured v = new VertexColoredTextured();

                        for (int j = 0; j < objectList.Count(); j++)
                        {
                            if (n.declarations[j].declarationType == Declarations.Vertex)
                            {
                                v.Position.X = vertexList1[objectList[j]].X;
                                v.Position.Y = vertexList1[objectList[j]].Y;
                                v.Position.Z = vertexList1[objectList[j]].Z;
                            }
                            else if (n.declarations[j].declarationType == Declarations.Color)
                            {
                                v.Color = new SharpDX.Color(colorList[objectList[j]].R, colorList[objectList[j]].G, colorList[objectList[j]].B, colorList[objectList[j]].A);
                            }
                            else if (n.declarations[j].declarationType == Declarations.TextCoord)
                            {
                                v.TextureCoordinate.X = textCoordList[objectList[j]].X;
                                v.TextureCoordinate.Y = textCoordList[objectList[j]].Y;
                            }
                        }

                        vertexList.Add(v);
                        indexList.Add(k);
                        k++;
                    }

                    if (SharpRenderer.TextureStream.ContainsKey(MaterialStream[td.MaterialIndex]))
                        subSetList.Add(new SharpSubSet(previousAmount, vertexList.Count() - previousAmount, SharpRenderer.TextureStream[MaterialStream[td.MaterialIndex]]));
                    else
                        subSetList.Add(new SharpSubSet(previousAmount, vertexList.Count() - previousAmount, SharpRenderer.whiteDefault));

                    previousAmount = vertexList.Count();
                }
            }

            if (vertexList.Count > 0)
                meshList.Add(SharpMesh.Create(SharpRenderer.device, vertexList.ToArray(), indexList.ToArray(), subSetList, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip));
        }

        public void Render(SharpDevice device, SharpShader shader, SharpDX.Direct3D11.Buffer buffer, Matrix worldViewProjection)
        {
            device.UpdateData(buffer, worldViewProjection);
            device.DeviceContext.VertexShader.SetConstantBuffer(0, buffer);
            shader.Apply();

            foreach (SharpMesh mesh in meshList)
            {
                if (mesh == null) continue;

                mesh.Begin();
                for (int i = 0; i < mesh.SubSets.Count(); i++)
                {
                    SharpRenderer.device.DeviceContext.PixelShader.SetShaderResource(0, mesh.SubSets[i].DiffuseMap);
                    mesh.Draw(i);
                }
            }
        }
    }
}
