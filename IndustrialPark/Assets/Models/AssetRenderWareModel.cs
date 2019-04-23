using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetRenderWareModel : Asset
    {
        protected RenderWareModelFile model;

        public AssetRenderWareModel(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public virtual void Setup(SharpRenderer renderer)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            try
            {
                model.SetForRendering(renderer.device, ReadFileMethods.ReadRenderWareFile(AHDR.data), AHDR.data);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ToString() + " (MODL) has an unsupported format and cannot be rendered. " + ex.Message);
                model.Dispose();
                model = null;
            }
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

        protected RWSection[] ModelAsRWSections
        {
            get
            {
                try
                {
                    ReadFileMethods.treatStuffAsByteArray = true;
                    RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
                    renderWareVersion = sections[0].renderWareVersion;
                    ReadFileMethods.treatStuffAsByteArray = false;
                    return sections;
                }
                catch
                {
                    return new RWSection[0];
                }
            }
            set
            {
                ReadFileMethods.treatStuffAsByteArray = true;
                Data = ReadFileMethods.ExportRenderWareFile(value, renderWareVersion);
                ReadFileMethods.treatStuffAsByteArray = false;

                model.Dispose();
                Setup(Program.MainForm.renderer);
            }
        }

        private int tempCount;

        [DisplayName("Colors ([A,] R, G, B)")]
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

                tempCount = colors.Count;
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
                                if (i >= value.Length)
                                    continue;

                                mat.materialStruct.color = new RenderWareFile.Color(value[i].R, value[i].G, value[i].B, value[i].A);
                                i++;
                            }

                ModelAsRWSections = sections;
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

                tempCount = names.Count;
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
                                                if (i >= value.Length)
                                                    continue;

                                                mat.texture.diffuseTextureName.stringString = value[i];
                                                i++;
                                            }

                ModelAsRWSections = sections;
            }
        }
    }
}