using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class AssetRenderWareModel : Asset, IAssetWithModel
    {
        RenderWareModelFile model;

        public AssetRenderWareModel(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup(SharpRenderer renderer)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            try
            {
                RWSection[] rw = ReadFileMethods.ReadRenderWareFile(AHDR.data);
                model.SetForRendering(renderer.device, rw, AHDR.data);
                ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ToString() + " (MODL) has an unsupported format and cannot be rendered. " + ex.Message);
            }
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color)
        {
            model.Render(renderer, world, isSelected ? renderer.selectedObjectColor * color : color);
        }

        public RenderWareModelFile GetRenderWareModelFile()
        {
            return model;
        }

        public bool HasRenderWareModelFile()
        {
            return model != null;
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

        public System.Drawing.Color[] Colors
        {
            get
            {
                List<System.Drawing.Color> colors = new List<System.Drawing.Color>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            foreach (Material_0007 mat in geo.materialList.materialList)
                            {
                                RenderWareFile.Color rwColor = mat.materialStruct.color;
                                System.Drawing.Color color = System.Drawing.Color.FromArgb(rwColor.A, rwColor.R, rwColor.G, rwColor.B);
                                colors.Add(color);
                            }

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
                                    break;

                                mat.materialStruct.color = new RenderWareFile.Color(value[i].R, value[i].G, value[i].B, value[i].A);
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
                            if (geo.materialList != null)
                                if (geo.materialList.materialList != null)
                                    foreach (Material_0007 mat in geo.materialList.materialList)
                                        if (mat.texture != null)
                                            if (mat.texture.diffuseTextureName != null)
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
                            if (geo.materialList != null)
                                if (geo.materialList.materialList != null)
                                    foreach (Material_0007 mat in geo.materialList.materialList)
                                        if (mat.texture != null)
                                            if (mat.texture.diffuseTextureName != null)
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