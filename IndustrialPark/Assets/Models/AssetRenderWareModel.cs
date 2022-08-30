using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class AssetRenderWareModel : AssetWithData
    {
        protected RenderWareModelFile model;

        public override string AssetInfo => $"{RwVersion(renderWareVersion)} {(IsNativeData ? "native" : "")} {base.AssetInfo}";

        public AssetRenderWareModel(string assetName, AssetType assetType, byte[] data, SharpRenderer renderer) : base(assetName, assetType, data)
        {
            if (renderer != null)
                Setup(renderer);
        }

        public AssetRenderWareModel(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game, endianness)
        {
            Setup(renderer);
        }

        public override byte[] Serialize(Game game, Endianness endianness) => Data;

        public virtual void Setup(SharpRenderer renderer)
        {
            if (model != null)
                model.Dispose();

#if !DEBUG
            try
            {
#endif
            ReadFileMethods.treatStuffAsByteArray = false;
            var rwSecArray = ReadFileMethods.ReadRenderWareFile(Data);
            model = new RenderWareModelFile(renderer.device, rwSecArray);
            if (rwSecArray.Length > 0)
                renderWareVersion = rwSecArray[0].renderWareVersion;
            SetupAtomicFlagsForRender();
#if !DEBUG
            }
            catch (Exception ex)
            {
                if (model != null)
                    model.Dispose();
                model = null;
                throw new Exception("Error: " + ToString() + " has an unsupported format and cannot be rendered. " + ex.Message);
            }
#endif
        }

        public RenderWareModelFile GetRenderWareModelFile() => model;

        [Browsable(false)]
        public bool IsNativeData => model != null && model.isNativeData;

        [Browsable(false)]
        public string[] Textures
        {
            get
            {
                List<string> names = new List<string>();

                foreach (RWSection rws in ModelAsRWSections)
                    if (rws is Clump_0010 clump)
                    {
                        foreach (Geometry_000F geo in clump.geometryList.geometryList)
                            if (geo.materialList != null)
                                if (geo.materialList.materialList != null)
                                    foreach (Material_0007 mat in geo.materialList.materialList)
                                        if (mat.texture != null)
                                            if (mat.texture.diffuseTextureName != null)
                                                if (!names.Contains(mat.texture.diffuseTextureName.stringString))
                                                    names.Add(mat.texture.diffuseTextureName.stringString);
                    }
                    else if (rws is World_000B world)
                    {
                        if (world.materialList != null)
                            if (world.materialList.materialList != null)
                                foreach (Material_0007 mat in world.materialList.materialList)
                                    if (mat.texture != null)
                                        if (mat.texture.diffuseTextureName != null)
                                            if (!names.Contains(mat.texture.diffuseTextureName.stringString))
                                                names.Add(mat.texture.diffuseTextureName.stringString);
                    }

                return names.ToArray();
            }
        }

        public override bool HasReference(uint assetID) =>
            Textures.Any(s => Functions.BKDRHash(s + ".RW3") == assetID || Functions.BKDRHash(s) == assetID) ||
            base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            if (ModelAsRWSections.Length == 0)
                result.Add("Failed to read MODL asset. This might be just a library error and does not necessarily mean the model is broken.");

            foreach (string s in Textures)
                if (!Program.MainForm.AssetExists(Functions.BKDRHash(s + ".RW3")) && !Program.MainForm.AssetExists(Functions.BKDRHash(s)))
                    result.Add($"I haven't found texture {s}, used by the model. This might just mean I haven't looked properly for it, though.");

            if (Program.MainForm.WhoTargets(assetID).Count == 0)
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

        protected bool[] _dontDrawMeshNumber;

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
                SetupAtomicFlagsForRender();
            }
        }

        private void SetupAtomicFlagsForRender()
        {
            var value = AtomicFlags;
            if (value.Length == 0)
            {
                _dontDrawMeshNumber = new bool[model.meshList.Count];
                for (int j = 0; j < value.Length; j++)
                    _dontDrawMeshNumber[j] = false;
            }
            else
            {
                _dontDrawMeshNumber = new bool[value.Length];
                for (int j = 0; j < value.Length; j++)
                    _dontDrawMeshNumber[j] = !((value[j] & RenderWareFile.Sections.AtomicFlags.Render) != 0);
            }
        }

        public void SetVertexColors(Vector4 color, Operation operation)
        {
            RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
            renderWareVersion = sections[0].renderWareVersion;

            foreach (RWSection rws in sections)
                if (rws is Clump_0010 clump)
                    for (int i = 0; i < clump.geometryList.geometryList.Count; i++)
                        if (clump.geometryList.geometryList[i].geometryStruct.vertexColors != null)
                            for (int j = 0; j < clump.geometryList.geometryList[i].geometryStruct.vertexColors.Length; j++)
                            {
                                var oldColor = clump.geometryList.geometryList[i].geometryStruct.vertexColors[j];

                                var newColor = PerformOperationAndClamp(
                                    new Vector4((float)oldColor.R / 255, (float)oldColor.G / 255, (float)oldColor.B / 255, (float)oldColor.A / 255),
                                    color, operation);

                                clump.geometryList.geometryList[i].geometryStruct.vertexColors[j] = new RenderWareFile.Color(
                                        (byte)(newColor.X * 255),
                                        (byte)(newColor.Y * 255),
                                        (byte)(newColor.Z * 255),
                                        (byte)(newColor.W * 255));
                            }

            Data = ReadFileMethods.ExportRenderWareFile(sections, renderWareVersion);
            Setup(Program.MainForm.renderer);
        }

        private Vector4 PerformOperationAndClamp(Vector4 v1, Vector4 v2, Operation op)
        {
            return new Vector4(
                PerformOperationAndClamp(v1.X, v2.X, op),
                PerformOperationAndClamp(v1.Y, v2.Y, op),
                PerformOperationAndClamp(v1.Z, v2.Z, op),
                PerformOperationAndClamp(v1.W, v2.W, op));
        }

        private float PerformOperationAndClamp(float v1, float v2, Operation op)
        {
            float value;
            switch (op)
            {
                case Operation.Replace:
                    value = v2;
                    break;
                case Operation.Add:
                    value = v1 + v2;
                    break;
                case Operation.Subtract:
                    value = v1 - v2;
                    break;
                case Operation.Multiply:
                    value = v1 * v2;
                    break;
                case Operation.Divide:
                    value = v1 / v2;
                    break;
                case Operation.RightHandSubtract:
                    value = v2 - v1;
                    break;
                case Operation.RightHandDivide:
                    value = v2 / v1;
                    break;
                case Operation.Minimum:
                    value = Math.Min(v1, v2);
                    break;
                case Operation.Maximum:
                    value = Math.Max(v1, v2);
                    break;
                default:
                    throw new Exception("Unsupported operation");
            }

            if (value < 0)
                return 0;
            if (value > 1)
                return 1;

            return value;
        }
    }
}