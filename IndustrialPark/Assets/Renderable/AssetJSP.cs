using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class AssetJSP : Asset, IRenderableAsset
    {
        private BoundingBox boundingBox;

        public RenderWareModelFile model;
        public static bool dontRender = false;

        public AssetJSP(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup(SharpRenderer renderer)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            try
            {
                model.SetForRendering(renderer.device, ReadFileMethods.ReadRenderWareFile(AHDR.data), AHDR.data);
                CreateTransformMatrix();
                ArchiveEditorFunctions.renderableAssetSetJSP.Add(this);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ToString() + " (" + AHDR.assetType.ToString() + ") has an unsupported format and cannot be rendered. " + ex.Message);
            }
        }

        public void CreateTransformMatrix()
        {
            boundingBox = BoundingBox.FromPoints(model.vertexListG.ToArray());
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return 0f;
        }

        public void Draw(SharpRenderer renderer)
        {
            if (dontRender || isInvisible) return;

            model.Render(renderer, Matrix.Identity, isSelected ? renderer.selectedObjectColor : Vector4.One);
        }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender || isInvisible)
                return null;

            return TriangleIntersection(ray);
        }

        private float? TriangleIntersection(Ray r)
        {
            bool hasIntersected = false;
            float smallestDistance = 2000f;

            foreach (Triangle t in model.triangleList)
            {
                Vector3 v1 = model.vertexListG[t.vertex1];
                Vector3 v2 = model.vertexListG[t.vertex2];
                Vector3 v3 = model.vertexListG[t.vertex3];

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }
            }

            if (hasIntersected)
                return smallestDistance;
            return null;
        }

        public string[] Textures
        {
            get
            {
                List<string> textures = new List<string>();
                foreach (string s in model.MaterialList)
                    if (!textures.Contains(s))
                        textures.Add(s);
                return textures.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            foreach (string s in Textures)
            {
                if (Functions.BKDRHash(s + ".RW3") == assetID)
                    return true;
                if (Functions.BKDRHash(s) == assetID)
                    return true;
            }

            return base.HasReference(assetID);
        }

        private int renderWareVersion;

        private RWSection[] ModelAsRWSections
        {
            get
            {
                try
                {
                    RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
                    renderWareVersion = sections[0].renderWareVersion;
                    return sections;
                }
                catch
                {
                    return new RWSection[0];
                }
            }
            set
            {
                Data = ReadFileMethods.ExportRenderWareFile(value, renderWareVersion);
            }
        }

        private int colorCount;

        public RenderWareFile.Color[] Colors
        {
            get
            {
                List<RenderWareFile.Color> colors = new List<RenderWareFile.Color>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            foreach (Material_0007 mat in geo.materialList.materialList)
                                colors.Add(mat.materialStruct.color);

                colorCount = colors.Count;
                return colors.ToArray();
            }

            set
            {
                int i = 0;
                RWSection[] sections = ModelAsRWSections;

                foreach (RWSection rws in sections)
                    if (rws is Clump_0010 clump)
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            foreach (Material_0007 mat in geo.materialList.materialList)
                            {
                                if (value.Length < colorCount)
                                    return;

                                mat.materialStruct.color = value[i];
                                i++;
                            }

                ModelAsRWSections = sections;
                model.Dispose();
                Setup(Program.MainForm.renderer);
            }
        }

        public string[] TextureNames
        {
            get
            {
                List<string> names = new List<string>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            foreach (Material_0007 mat in geo.materialList.materialList)
                                names.Add(mat.texture.diffuseTextureName.stringString);

                colorCount = names.Count;
                return names.ToArray();
            }

            set
            {
                int i = 0;
                RWSection[] sections = ModelAsRWSections;

                foreach (RWSection rws in sections)
                    if (rws is Clump_0010 clump)
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            foreach (Material_0007 mat in geo.materialList.materialList)
                            {
                                if (value.Length < colorCount)
                                    return;

                                mat.texture.diffuseTextureName.stringString = value[i];
                                i++;
                            }

                ModelAsRWSections = sections;
                model.Dispose();
                Setup(Program.MainForm.renderer);
            }
        }
    }
}