using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaInPickup : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x30;

        public DynaInPickup(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID) =>
            PickupHash == assetID || 
            SCRP_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(PickupHash, ref result);
            Asset.Verify(SCRP_AssetID, ref result);
        }

        public AssetID PickupHash
        {
            get => ReadUInt(0x00);
            set
            {
                Write(0x00, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => ReadFloat(0x04);
            set
            {
                Write(0x04, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => ReadFloat(0x08);
            set
            {
                Write(0x08, value);
                CreateTransformMatrix();
            }
        }
        [Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => ReadFloat(0x0C);
            set
            {
                Write(0x0C, value);
                CreateTransformMatrix();
            }
        }

        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort10
        {
            get => ReadUShort(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort12
        {
            get => ReadUShort(0x12);
            set => Write(0x12, value);
        }

        public AssetID SCRP_AssetID
        {
            get { try { return ReadUInt(0x14); } catch { return 0; } }
            set { try { Write(0x14, value); ; } catch { } }    
        }
        public int Unknown18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }
        public int Unknown1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }
        public int Unknown20
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
        public int Unknown24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }
        public int Unknown28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }
        public int Unknown2C
        {
            get => ReadInt(0x2C);
            set => Write(0x2C, value);
        }
        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(PositionX, PositionY, PositionZ);

            vertices = new Vector3[SharpRenderer.cubeVertices.Count];

            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i], world);

            if (AssetTPIK.tpikEntries.ContainsKey(PickupHash))
            {
                var u = AssetTPIK.tpikEntries[PickupHash].Model_AssetID;
                if (renderingDictionary.ContainsKey(u) &&
                    renderingDictionary[u].HasRenderWareModelFile() &&
                    renderingDictionary[u].GetRenderWareModelFile() != null)
                {
                    List<Vector3> vertexList = renderingDictionary[u].GetRenderWareModelFile().vertexListG;

                    vertices = new Vector3[vertexList.Count];
                    for (int i = 0; i < vertexList.Count; i++)
                        vertices[i] = (Vector3)Vector3.Transform(vertexList[i], world);
                    boundingBox = BoundingBox.FromPoints(vertices);

                    if (renderingDictionary.ContainsKey(u))
                    {
                        if (renderingDictionary[u] is AssetMINF MINF)
                        {
                            if (MINF.HasRenderWareModelFile())
                                triangles = renderingDictionary[u].GetRenderWareModelFile().triangleList.ToArray();
                            else
                                triangles = null;
                        }
                        else
                            triangles = renderingDictionary[u].GetRenderWareModelFile().triangleList.ToArray();
                    }
                    else
                        triangles = null;
                }
            }
            else
            {
                vertices = new Vector3[SharpRenderer.cubeVertices.Count];
                for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                    vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i] * 0.5f, world);
                boundingBox = BoundingBox.FromPoints(vertices);
            }
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            bool drew = false;
            if (AssetTPIK.tpikEntries.ContainsKey(PickupHash))
            {
                var tpikEntry = AssetTPIK.tpikEntries[PickupHash];
                if (renderingDictionary.ContainsKey(tpikEntry.Model_AssetID))
                {
                    renderingDictionary[tpikEntry.Model_AssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor : Vector4.One, Vector3.Zero);
                    drew = true;
                }
                if (renderingDictionary.ContainsKey(tpikEntry.RingModel_AssetID))
                {
                    var color = new Vector4(tpikEntry.RingColorR, tpikEntry.RingColorG, tpikEntry.RingColorB, 1f);
                    renderingDictionary[tpikEntry.RingModel_AssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, Vector3.Zero);
                    drew = true;
                }
            }
            if (!drew)
                renderer.DrawCube(world, isSelected);
        }
        
        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox))
                return TriangleIntersection(ray);
            return null;
        }

        private float? TriangleIntersection(Ray r)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }

            if (hasIntersected)
                return smallestDistance;
            else return null;
        }
    }
}