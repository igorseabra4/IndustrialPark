using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class AssetRenderWareModel : Asset
    {
        protected RenderWareModelFile model;

        public AssetRenderWareModel(Section_AHDR AHDR, Game game, Platform platform, SharpRenderer renderer) : base(AHDR, game, platform)
        {
            Setup(renderer);
        }

        public virtual void Setup(SharpRenderer renderer)
        {
            if (model != null)
                model.Dispose();

            try
            {
                ReadFileMethods.treatStuffAsByteArray = false;
                model = new RenderWareModelFile(renderer.device, ReadFileMethods.ReadRenderWareFile(Data));
                _atomicFlags = AtomicFlags;
            }
            catch (Exception ex)
            {
                if (model != null)
                    model.Dispose();
                model = null;
                throw new Exception("Error: " + ToString() + " has an unsupported format and cannot be rendered. " + ex.Message);
            }
        }

        public RenderWareModelFile GetRenderWareModelFile() => model;

        public bool HasRenderWareModelFile() => model != null;

        [Category("Model Data"), Description("If IsNativeData is true, you cannot use the Export function.")]
        public bool IsNativeData => model != null ? model.isNativeData : false;

        [Browsable(false)]
        public string[] Textures
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
                                                if (!names.Contains(mat.texture.diffuseTextureName.stringString))
                                                    names.Add(mat.texture.diffuseTextureName.stringString);

                return names.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            foreach (string s in Textures)
                if (Functions.BKDRHash(s + ".RW3") == assetID || Functions.BKDRHash(s) == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            if (ModelAsRWSections.Length == 0)
                result.Add("Failed to read MODL asset. This might be just a library error and does not necessarily mean the model is broken.");

            foreach (string s in Textures)
                if (!Program.MainForm.AssetExists(Functions.BKDRHash(s + ".RW3")) && !Program.MainForm.AssetExists(Functions.BKDRHash(s)))
                    result.Add($"I haven't found texture {s}, used by the model. This might just mean I haven't looked properly for it, though.");
            
            if (Program.MainForm.WhoTargets(AHDR.assetID).Count == 0)
                result.Add("Model appears to be unused, as no other asset references it. This might just mean I haven't looked properly for an asset which does does, though.");
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

        [Category("Model Data"), Editor(typeof(MaterialListEditor), typeof(UITypeEditor))]
        public Material_0007[] Materials
        {
            get
            {
                var materials = new List<Material_0007>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            materials.AddRange(geo.materialList.materialList);
                
                return materials.ToArray();
            }
            set
            {
                RWSection[] sections = ModelAsRWSections;

                int k = 0;
                foreach (RWSection rws in sections)
                    if (rws is Clump_0010 clump)
                    {
                        for (int i = 0; i < clump.geometryList.geometryList.Count; i++)
                        {
                            bool hasMaterialEffects = false;
                            for (int j = 0; j < clump.geometryList.geometryList[i].materialList.materialList.Length; j++)
                            {
                                if (k >= value.Length)
                                    break;
                                else
                                    clump.geometryList.geometryList[i].materialList.materialList[j] = value[k];

                                foreach (var rwss in value[k].materialExtension.extensionSectionList)
                                    if (rwss is MaterialEffectsPLG_0120)
                                        hasMaterialEffects = true;
                                k++;
                            }

                            // giving the atomic the material effects
                            if (hasMaterialEffects)
                            {
                                var plg = new MaterialEffectsPLG_0120() { value = MaterialEffectType.BumpMap, isAtomicExtension = true };

                                bool newMatEffsFound = false;
                                for (int j = 0; j < clump.atomicList[i].atomicExtension.extensionSectionList.Count; j++)                                
                                    if (clump.atomicList[i].atomicExtension.extensionSectionList[j] is MaterialEffectsPLG_0120)
                                    {
                                        clump.atomicList[i].atomicExtension.extensionSectionList[j] = plg;
                                        newMatEffsFound = true;
                                        break;
                                    }

                                if (!newMatEffsFound)
                                    clump.atomicList[i].atomicExtension.extensionSectionList.Add(plg);
                            }
                            else
                            {
                                for (int j = 0; j < clump.atomicList[i].atomicExtension.extensionSectionList.Count; j++)
                                    if (clump.atomicList[i].atomicExtension.extensionSectionList[j] is MaterialEffectsPLG_0120 plg)
                                        clump.atomicList[i].atomicExtension.extensionSectionList.RemoveAt(j--);
                            }
                        }
                    }
                
                ModelAsRWSections = sections;
                Setup(Program.MainForm.renderer);
            }
        }
        
        protected AtomicFlags[] _atomicFlags;

        [Category("Model Data")]
        public AtomicFlags[] AtomicFlags
        {
            get
            {
                List<AtomicFlags> flags = new List<AtomicFlags>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                        foreach (var atomic in clump.atomicList)
                            flags.Add(atomic.atomicStruct.flags);

                return flags.ToArray();
            }

            set
            {
                int i = 0;
                RWSection[] sections = ModelAsRWSections;

                foreach (RWSection rws in sections)
                    if (rws is Clump_0010 clump)
                        foreach (var atomic in clump.atomicList)
                        {
                            if (i >= value.Length)
                                continue;

                            atomic.atomicStruct.flags = value[i];
                            i++;
                        }

                ModelAsRWSections = sections;
                _atomicFlags = value;
            }
        }
    }
}