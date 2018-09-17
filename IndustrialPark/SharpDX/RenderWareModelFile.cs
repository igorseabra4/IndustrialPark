using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using RenderWareFile;
using RenderWareFile.Sections;

namespace IndustrialPark
{
    public class RenderWareModelFile
    {
        public string fileName;

        private const string DefaultTexture = "default";
        private byte[] rwSectionByteArray;
        private RWSection[] rwSectionArray;

        public List<string> MaterialList = new List<string>();
        public bool isNoCulling = false;
        public bool isCollision = false;
        public bool containsNormals = false;

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
        
        public RenderWareModelFile(string fileName)
        {
            this.fileName = fileName;
        }
        
        public List<Vector3> GetVertexList()
        {
            return vertexListG;
        }

        public byte[] GetAsByteArray()
        {
            return rwSectionByteArray;
        }

        public RWSection[] GetAsRWSectionArray()
        {
            return rwSectionArray;
        }
        
        public DefaultRenderData renderData;
        
        public void SetForRendering(SharpDevice device, RWSection[] rwChunkList, byte[] rwByteArray)
        {
            rwSectionArray = rwChunkList;

            if (rwByteArray == null)
            {
                rwSectionByteArray = ReadFileMethods.ExportRenderWareFile(rwSectionArray, 0x1400FFFF);

                if (rwSectionByteArray.Length > 450 * 1024)
                    System.Windows.Forms.MessageBox.Show(fileName + " is larger than 450 kb. I will still import it for you, but be warned that files too large might make the game crash.");
            }
            else
                rwSectionByteArray = rwByteArray;
            meshList = new List<SharpMesh>();

            vertexListG = new List<Vector3>();
            triangleList = new List<Triangle>();
            
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
                            MaterialList.Add(m.texture.diffuseTextureName.stringString);
                        }
                        else
                        {
                            MaterialList.Add(DefaultTexture);
                        }
                    }
                    if (w.firstWorldChunk is AtomicSector_0009 a)
                    {
                        AddAtomic(device, a);
                    }
                    else if (w.firstWorldChunk is PlaneSector_000A p)
                    {
                        AddPlane(device, p);
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

        private Matrix CreateMatrix(FrameList_000E frameList, int frameIndex)
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

        private void AddPlane(SharpDevice device, PlaneSector_000A planeSection)
        {
            if (planeSection.leftSection is AtomicSector_0009 al)
            {
                AddAtomic(device, al);
            }
            else if (planeSection.leftSection is PlaneSector_000A pl)
            {
                AddPlane(device, pl);
            }
            else throw new Exception();

            if (planeSection.rightSection is AtomicSector_0009 ar)
            {
                AddAtomic(device, ar);
            }
            else if (planeSection.rightSection is PlaneSector_000A pr)
            {
                AddPlane(device, pr);
            }
            else throw new Exception();
        }

        private void AddAtomic(SharpDevice device, AtomicSector_0009 AtomicSector)
        {
            if (AtomicSector.atomicSectorStruct.isNativeData)
            {
                AddNativeData(device, AtomicSector.atomicSectorExtension, MaterialList, Matrix.Identity);
                return;
            }

            List<VertexColoredTextured> vertexList = new List<VertexColoredTextured>();

            foreach (Vertex3 v in AtomicSector.atomicSectorStruct.vertexArray)
            {
                vertexList.Add(new VertexColoredTextured(new Vector3(v.X, v.Y, v.Z), new Vector2(), new SharpDX.Color()));
                this.vertexListG.Add(new Vector3(v.X, v.Y, v.Z));
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
                        
                        triangleList.Add(t);
                    }
                }

                if (indexList.Count - previousIndexCount > 0)
                {
                    SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount,
                        TextureManager.GetTextureFromDictionary(MaterialList[i]), MaterialList[i]));
                }

                previousIndexCount = indexList.Count();
            }

            if (SubsetList.Count > 0)
                AddToMeshList(SharpMesh.Create(device, vertexList.ToArray(), indexList.ToArray(), SubsetList));
        }

        private void AddGeometry(SharpDevice device, Geometry_000F g, Matrix transformMatrix)
        {
            List<string> MaterialList = new List<string>();
            foreach (Material_0007 m in g.materialList.materialList)
            {
                if (m.texture != null)
                {
                    string textureName = m.texture.diffuseTextureName.stringString;
                    MaterialList.Add(textureName);
                }
                else
                    MaterialList.Add(DefaultTexture);
            }

            if ((g.geometryStruct.geometryFlags2 & GeometryFlags2.isNativeGeometry) != 0)
            {
                AddNativeData(device, g.geometryExtension, MaterialList, transformMatrix);
                return;
            }

            List<Vector3> vertexList1 = new List<Vector3>();
            List<Vector3> normalList = new List<Vector3>();
            List<Vector2> textCoordList = new List<Vector2>();
            List<SharpDX.Color> colorList = new List<SharpDX.Color>();

            if ((g.geometryStruct.geometryFlags & GeometryFlags.hasVertexPositions) != 0)
            {
                MorphTarget m = g.geometryStruct.morphTargets[0];
                foreach (Vertex3 v in m.vertices)
                {
                    Vector3 pos = (Vector3)Vector3.Transform(new Vector3(v.X, v.Y, v.Z), transformMatrix);
                    vertexList1.Add(pos);
                    vertexListG.Add(pos);
                }
            }

            if ((g.geometryStruct.geometryFlags & GeometryFlags.hasNormals) != 0)
            {
                containsNormals = true;
                for (int i = 0; i < vertexList1.Count; i++)
                {
                    normalList.Add(new Vector3(g.geometryStruct.morphTargets[0].normals[i].X, g.geometryStruct.morphTargets[0].normals[i].Y, g.geometryStruct.morphTargets[0].normals[i].Z));
                }
            }

            if ((g.geometryStruct.geometryFlags & GeometryFlags.hasVertexColors) != 0)
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
                {
                    colorList.Add(new SharpDX.Color(1f, 1f, 1f, 1f));
                }
            }

            if ((g.geometryStruct.geometryFlags & GeometryFlags.hasTextCoords) != 0)
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
                {
                    textCoordList.Add(new Vector2());
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

                        triangleList.Add(t);
                    }
                }

                if (indexList.Count - previousIndexCount > 0)
                {
                    SubsetList.Add(new SharpSubSet(previousIndexCount, indexList.Count - previousIndexCount,
                        TextureManager.GetTextureFromDictionary(MaterialList[i]), MaterialList[i]));
                }

                previousIndexCount = indexList.Count();
            }

            if (SubsetList.Count > 0)
            {
                VertexColoredTextured[] vertices = new VertexColoredTextured[vertexList1.Count];
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i] = new VertexColoredTextured(vertexList1[i], textCoordList[i], colorList[i]);
                AddToMeshList(SharpMesh.Create(device, vertices, indexList.ToArray(), SubsetList));
            }
        }

        private void AddNativeData(SharpDevice device, Extension_0003 extension, List<string> MaterialStream, Matrix transformMatrix)
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

            if (n == null) throw new Exception();

            List<Vertex3> vertexList1 = new List<Vertex3>();
            List<Vertex3> normalList = new List<Vertex3>();
            List<RenderWareFile.Color> colorList = new List<RenderWareFile.Color>();
            List<Vertex2> textCoordList = new List<Vertex2>();

            foreach (Declaration d in n.declarations)
            {
                foreach (object o in d.entryList)
                {
                    if (d.declarationType == Declarations.Vertex)
                        vertexList1.Add((Vertex3)o);
                    else if (d.declarationType == Declarations.Color)
                        colorList.Add((RenderWareFile.Color)o);
                    else if (d.declarationType == Declarations.TextCoord)
                        textCoordList.Add((Vertex2)o);
                    else if (d.declarationType == Declarations.Normal)
                        normalList.Add((Vertex3)o);
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
                        Vector3 position = new Vector3();
                        SharpDX.Color color = new SharpDX.Color(255, 255, 255, 255);
                        Vector2 textureCoordinate = new Vector2();
                        Vector3 normal = new Vector3();

                        for (int j = 0; j < objectList.Count(); j++)
                        {
                            if (n.declarations[j].declarationType == Declarations.Vertex)
                            {
                                position = (Vector3)Vector3.Transform(
                                    new Vector3(
                                        vertexList1[objectList[j]].X,
                                        vertexList1[objectList[j]].Y,
                                        vertexList1[objectList[j]].Z),
                                    transformMatrix);
                            }
                            else if (n.declarations[j].declarationType == Declarations.Color)
                            {
                                color = new SharpDX.Color(colorList[objectList[j]].R, colorList[objectList[j]].G, colorList[objectList[j]].B, colorList[objectList[j]].A);
                                if (color.A == 0)
                                    color = new SharpDX.Color(255, 255, 255, 255);
                            }
                            else if (n.declarations[j].declarationType == Declarations.TextCoord)
                            {
                                textureCoordinate.X = textCoordList[objectList[j]].X;
                                textureCoordinate.Y = textCoordList[objectList[j]].Y;
                            }
                            else if (n.declarations[j].declarationType == Declarations.Normal)
                            {
                                normal = new Vector3(
                                        normalList[objectList[j]].X,
                                        normalList[objectList[j]].Y,
                                        normalList[objectList[j]].Z);
                                containsNormals = true;
                            }
                        }

                        vertexList.Add(new VertexColoredTextured(position, textureCoordinate, color));

                        indexList.Add(k);
                        k++;

                        vertexListG.Add(position);
                    }

                    subSetList.Add(new SharpSubSet(previousAmount, vertexList.Count() - previousAmount,
                        TextureManager.GetTextureFromDictionary(MaterialStream[td.MaterialIndex]), MaterialStream[td.MaterialIndex]));

                    previousAmount = vertexList.Count();
                }
            }

            if (vertexList.Count > 0)
            {
                for (int i = 2; i < indexList.Count; i++)
                    triangleList.Add(new Triangle(0, (ushort)(i - 2), (ushort)(i - 1), (ushort)i));

                VertexColoredTextured[] vertices = vertexList.ToArray();
                AddToMeshList(SharpMesh.Create(device, vertices, indexList.ToArray(), subSetList, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip));
            }
        }

        public void Render(SharpRenderer renderer, Matrix world, Vector4 color)
        { 
            renderData.worldViewProjection = world * renderer.viewProjection;
            renderData.Color = color;

            renderer.device.UpdateData(renderer.tintedBuffer, renderData);
            renderer.device.DeviceContext.VertexShader.SetConstantBuffer(0, renderer.tintedBuffer);
            renderer.tintedShader.Apply();
            
            foreach (SharpMesh mesh in meshList)
            {
                if (mesh == null) continue;

                mesh.Begin(renderer.device);
                for (int i = 0; i < mesh.SubSets.Count(); i++)
                {
                    renderer.device.DeviceContext.PixelShader.SetShaderResource(0, mesh.SubSets[i].DiffuseMap);
                    mesh.Draw(renderer.device, i);
                }
            }
        }

        public void Dispose()
        {
            foreach (SharpMesh m in meshList)
            {
                completeMeshList.Remove(m);
                m.Dispose();
            }
            meshList.Clear();
        }
    }
}
