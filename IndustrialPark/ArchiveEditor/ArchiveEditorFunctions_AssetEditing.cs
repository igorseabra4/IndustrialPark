using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        private class LHDRComparer : IComparer<LayerType>
        {
            private static readonly List<LayerType> layerOrder = new List<LayerType> {
                    LayerType.TEXTURE,
                    LayerType.BSP,
                    LayerType.JSPINFO,
                    LayerType.MODEL,
                    LayerType.ANIMATION,
                    LayerType.DEFAULT,
                    LayerType.CUTSCENE,
                    LayerType.SRAM,
                    LayerType.SNDTOC
                };

            public int Compare(LayerType l1, LayerType l2)
            {
                if (l1 == l2)
                    return 0;

                if (layerOrder.Contains(l1) && layerOrder.Contains(l2))
                    return layerOrder.IndexOf(l1) > layerOrder.IndexOf(l2) ? 1 : -1;

                return 0;
            }
        }

        public static List<uint> hiddenAssets = new List<uint>();

        public List<uint> GetHiddenAssets()
        {
            return (from asset in assetDictionary.Values where asset.isInvisible select asset.AHDR.assetID).ToList();
        }

        private List<IInternalEditor> internalEditors = new List<IInternalEditor>();

        public void CloseInternalEditor(IInternalEditor i)
        {
            internalEditors.Remove(i);
        }

        public void CloseInternalEditor(uint assetID)
        {
            for (int i = 0; i < internalEditors.Count; i++)
                if (internalEditors[i].GetAssetID() == assetID)
                    internalEditors[i].Close();
        }

        public void OpenInternalEditor(List<uint> list, bool openAnyway)
        {
            bool willOpen = true;
            if (list.Count > 15 && !openAnyway)
            {
                willOpen = MessageBox.Show($"Warning: you're going to open {list.Count} Asset Data Editor windows. Are you sure you want to do that?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }
                
            if (willOpen)
            foreach (uint u in list)
                if (assetDictionary.ContainsKey(u))
                    OpenInternalEditor(assetDictionary[u]);
        }

        private void OpenInternalEditor(Asset asset)
        {
            CloseInternalEditor(asset.AHDR.assetID);

            switch (asset.AHDR.assetType)
            {
                case AssetType.CAM:
                    internalEditors.Add(new InternalCamEditor((AssetCAM)asset, this));
                    break;
                case AssetType.DYNA:
                    internalEditors.Add(new InternalDynaEditor((AssetDYNA)asset, this));
                    break;
                case AssetType.FLY:
                    internalEditors.Add(new InternalFlyEditor((AssetFLY)asset, this));
                    break;
                case AssetType.GRUP:
                    internalEditors.Add(new InternalGrupEditor((AssetGRUP)asset, this));
                    break;
                case AssetType.PLAT:
                    internalEditors.Add(new InternalPlatEditor((AssetPLAT)asset, this));
                    break;
                case AssetType.SHRP:
                    internalEditors.Add(new InternalShrapnelEditor((AssetSHRP)asset, this));
                    break;
                case AssetType.SND:
                case AssetType.SNDS:
                    internalEditors.Add(new InternalSoundEditor(asset, this));
                    break;
                case AssetType.TEXT:
                    internalEditors.Add(new InternalTextEditor((AssetTEXT)asset, this));
                    break;
                default:
                    internalEditors.Add(new InternalAssetEditor(asset, this));
                    break;
            }

            internalEditors.Last().Show();
        }

        public static Vector3 GetRayInterserctionPosition(Ray ray)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                l.AddRange(renderableAssetSetCommon);
                l.AddRange(renderableAssetSetTrans);
                l.AddRange(renderableAssetSetJSP);
            }
            catch { return Vector3.Zero; }

            float smallerDistance = 2000f;
            bool found = false;

            foreach (IRenderableAsset ra in l)
            {
                float? distance = ra.IntersectsWith(ray);
                if (distance != null && distance < smallerDistance)
                {
                    found = true;
                    smallerDistance = (float)distance;
                }
            }

            return ray.Position + Vector3.Normalize(ray.Direction) * (found ? smallerDistance : 2f);
        }

        public static uint GetClickedAssetID(Ray ray)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                l.AddRange(renderableAssetSetCommon);
                l.AddRange(renderableAssetSetTrans);
            }
            catch { return 0; }

            float smallerDistance = 1000f;
            uint assetID = 0;

            foreach (Asset ra in l)
            {
                if (!ra.isSelected && ra is IClickableAsset)
                {
                    float? distance = ((IClickableAsset)ra).IntersectsWith(ray);
                    if (distance != null && distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.AHDR.assetID;
                    }
                }
            }

            return assetID;
        }

        public static uint GetClickedAssetID2D(Ray ray, float farPlane)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                foreach (IRenderableAsset a in renderableAssetSetCommon)
                    if (a is AssetUI ui)
                        l.Add(ui);
                    else if (a is AssetUIFT uift)
                        l.Add(uift);
            }
            catch { return 0; }

            float smallerDistance = 3 * farPlane;
            uint assetID = 0;

            foreach (Asset ra in l)
            {
                if (!ra.isSelected && ra is IClickableAsset)
                {
                    float? distance = ((IClickableAsset)ra).IntersectsWith(ray);
                    if (distance != null && distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.AHDR.assetID;
                    }
                }
            }

            return assetID;
        }

        public List<uint> FindWhoTargets(uint assetID)
        {
            List<uint> whoTargets = new List<uint>();
            foreach (Asset asset in assetDictionary.Values)
                if (asset.HasReference(assetID))
                    whoTargets.Add(asset.AHDR.assetID);

            return whoTargets;
        }

        public void ExportHip(string fileName)
        {
            HipSection[] hipFile = SetupStream(ref HIPA, ref PACK, ref DICT, ref STRM);
            HipArrayToIni(hipFile, fileName, true, true);
        }

        public void ImportHip(string[] fileNames)
        {
            foreach (string fileName in fileNames)
                ImportHip(fileName);
        }

        public void ImportHip(string fileName)
        {
            if (Path.GetExtension(fileName).ToLower() == ".hip" || Path.GetExtension(fileName).ToLower() == ".hop")
                ImportHip(HipFileToHipArray(fileName));
            else if (Path.GetExtension(fileName).ToLower() == ".ini")
                ImportHip(IniToHipArray(fileName));
            else
                MessageBox.Show("Invalid file: " + fileName);
        }

        public void ImportHip(HipSection[] hipSections)
        {
            UnsavedChanges = true;

            foreach (HipSection i in hipSections)
            {
                if (i is Section_DICT dict)
                {
                    foreach (Section_AHDR AHDR in dict.ATOC.AHDRList)
                    {
                        if (AHDR.assetType == AssetType.COLL && ContainsAssetWithType(AssetType.COLL))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeCOLL(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.JAW && ContainsAssetWithType(AssetType.JAW))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeJAW(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.LODT && ContainsAssetWithType(AssetType.LODT))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeLODT(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.PIPT && ContainsAssetWithType(AssetType.PIPT))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergePIPT(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.SHDW && ContainsAssetWithType(AssetType.SHDW))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeSHDW(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.SNDI && ContainsAssetWithType(AssetType.SNDI))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeSNDI(AHDR);
                            continue;
                        }

                        if (ContainsAsset(AHDR.assetID))
                        {
                            DialogResult result = MessageBox.Show($"Asset [{AHDR.assetID.ToString("X8")}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                RemoveAsset(AHDR.assetID);
                                DICT.ATOC.AHDRList.Add(AHDR);
                                AddAssetToDictionary(AHDR);
                            }
                            else
                            {
                                foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                    LHDR.assetIDlist.Remove(AHDR.assetID);
                            }
                        }
                        else
                        {
                            DICT.ATOC.AHDRList.Add(AHDR);
                            AddAssetToDictionary(AHDR);
                        }
                    }

                    foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                        if (LHDR.assetIDlist.Count != 0)
                            DICT.LTOC.LHDRList.Add(LHDR);

                    break;
                }
            }

            DICT.LTOC.LHDRList = DICT.LTOC.LHDRList.OrderBy(f => f.layerType, new LHDRComparer()).ToList();

            RecalculateAllMatrices();
        }

        private void MergeCOLL(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetCOLL COLL)
                {
                    COLL.Merge(new AssetCOLL(AHDR));
                    return;
                }
            }
        }

        private void MergeJAW(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetJAW JAW)
                {
                    JAW.Merge(new AssetJAW(AHDR));
                    return;
                }
            }
        }

        private void MergeLODT(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetLODT LODT)
                {
                    LODT.Merge(new AssetLODT(AHDR));
                    return;
                }
            }
        }

        private void MergePIPT(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetPIPT PIPT)
                {
                    PIPT.Merge(new AssetPIPT(AHDR));
                    return;
                }
            }
        }

        private void MergeSHDW(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSHDW SHDW)
                {
                    SHDW.Merge(new AssetSHDW(AHDR));
                    return;
                }
            }
        }

        private void MergeSNDI(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    SNDI_G1.Merge(new AssetSNDI_GCN_V1(AHDR));
                    return;
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    SNDI_X.Merge(new AssetSNDI_XBOX(AHDR));
                    return;
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    SNDI_P.Merge(new AssetSNDI_PS2(AHDR));
                    return;
                }
            }
        }

        public void ExportTextureDictionary(string fileName)
        {
            ReadFileMethods.treatStuffAsByteArray = true;

            List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

            int fileVersion = 0;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetRWTX RWTX && RWTX.AHDR.ADBG.assetName.Contains(".RW3"))
                {
                    foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(RWTX.Data))
                        if (rw is TextureDictionary_0016 td)
                            foreach (TextureNative_0015 tn in td.textureNativeList)
                            {
                                fileVersion = tn.renderWareVersion;
                                tn.textureNativeStruct.textureName = RWTX.AHDR.ADBG.assetName.Replace(".RW3", "");
                                textNativeList.Add(tn);
                            }
                }

            TextureDictionary_0016 rws = new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001()
                {
                    textureCount = (short)textNativeList.Count(),
                    unknown = 0
                },
                textureNativeList = textNativeList,
                textureDictionaryExtension = new Extension_0003()
                {
                    extensionSectionList = new List<RWSection>()
                }
            };

            rws.textureNativeList = rws.textureNativeList.OrderBy(f => f.textureNativeStruct.textureName).ToList();

            File.WriteAllBytes(fileName, ReadFileMethods.ExportRenderWareFile(rws, fileVersion));

            ReadFileMethods.treatStuffAsByteArray = false;
        }

        public void AddTextureDictionary(string fileName)
        {
            UnsavedChanges = true;
            int layerIndex = 0;

            List<Section_LHDR> LHDRs = new List<Section_LHDR>
            {
                new Section_LHDR()
                {
                    layerType = LayerType.TEXTURE,
                    assetIDlist = new List<uint>(),
                    LDBG = new Section_LDBG(-1)
                }
            };
            LHDRs.AddRange(DICT.LTOC.LHDRList);
            DICT.LTOC.LHDRList = LHDRs;

            ReadFileMethods.treatStuffAsByteArray = true;

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(fileName))
            {
                if (rw is TextureDictionary_0016 td)
                {
                    // For each texture in the dictionary...
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        string textureName = tn.textureNativeStruct.textureName;
                        if (!textureName.Contains(".RW3"))
                            textureName += ".RW3";

                        // Create a new dictionary that has only that texture.
                        byte[] data = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
                        {
                            textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = 1, unknown = 0 },
                            textureDictionaryExtension = new Extension_0003(),
                            textureNativeList = new List<TextureNative_0015>() { tn }
                        }, tn.renderWareVersion);
                        
                        // And add the new dictionary as an asset.
                        Section_ADBG ADBG = new Section_ADBG(0, textureName, "", 0);
                        Section_AHDR AHDR = new Section_AHDR(BKDRHash(textureName), AssetType.RWTX, AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM, ADBG, data);

                        if (ContainsAsset(AHDR.assetID))
                            RemoveAsset(AHDR.assetID);

                        AddAsset(layerIndex, AHDR);
                    }
                }
            }

            ReadFileMethods.treatStuffAsByteArray = false;
        }

        public void AddSoundToSNDI(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    SNDI_G1.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    SNDI_X.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    SNDI_P.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
            }

            throw new Exception("Unable to add sound: SNDI asset not found");
        }

        public void RemoveSoundFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    if (SNDI_G1.HasReference(assetID))
                        SNDI_G1.RemoveEntry(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    if (SNDI_X.HasReference(assetID))
                        SNDI_X.RemoveEntry(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    if (SNDI_P.HasReference(assetID))
                        SNDI_P.RemoveEntry(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
            }
        }

        public byte[] GetHeaderFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    if (SNDI_G1.HasReference(assetID))
                        return SNDI_G1.GetHeader(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    if (SNDI_X.HasReference(assetID))
                        return SNDI_X.GetHeader(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    if (SNDI_P.HasReference(assetID))
                        return SNDI_P.GetHeader(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
            }

            throw new Exception("Error: could not find SNDI asset which contains this sound in this archive.");
        }

        public void AddJawDataToJAW(byte[] jawData, uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetJAW JAW)
                {
                    JAW.AddEntry(jawData, assetID);
                    return;
                }
            }

            throw new Exception("Unable to add jaw data: JAW asset not found");
        }

        public void RemoveJawDataFromJAW(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetJAW JAW)
                {
                    if (JAW.HasReference(assetID))
                        JAW.RemoveEntry(assetID);
                }
            }
        }

        public static AHDRFlags AHDRFlagsFromAssetType(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.ALST:
                case AssetType.BOUL:
                case AssetType.BUTN:
                case AssetType.CAM:
                case AssetType.CNTR:
                case AssetType.COLL:
                case AssetType.COND:
                case AssetType.CSNM:
                case AssetType.CTOC:
                case AssetType.DPAT:
                case AssetType.DSCO:
                case AssetType.DSTR:
                case AssetType.DYNA:
                case AssetType.EGEN:
                case AssetType.ENV:
                case AssetType.FOG:
                case AssetType.HANG:
                case AssetType.GRUP:
                case AssetType.JAW:
                case AssetType.LODT:
                case AssetType.MAPR:
                case AssetType.MINF:
                case AssetType.MRKR:
                case AssetType.MVPT:
                case AssetType.PARE:
                case AssetType.PARP:
                case AssetType.PARS:
                case AssetType.PEND:
                case AssetType.PICK:
                case AssetType.PIPT:
                case AssetType.PKUP:
                case AssetType.PLAT:
                case AssetType.PLYR:
                case AssetType.PORT:
                case AssetType.SFX:
                case AssetType.SHDW:
                case AssetType.SHRP:
                case AssetType.SIMP:
                case AssetType.SNDI:
                case AssetType.SURF:
                case AssetType.TEXT:
                case AssetType.TIMR:
                case AssetType.TRIG:
                case AssetType.UI:
                case AssetType.UIFT:
                case AssetType.VIL:
                case AssetType.VILP:
                    return AHDRFlags.SOURCE_VIRTUAL;
                case AssetType.CSN:
                case AssetType.FLY:
                case AssetType.RAW:
                    return AHDRFlags.SOURCE_FILE;
                case AssetType.ANIM:
                case AssetType.CRDT:
                case AssetType.SND:
                case AssetType.SNDS:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.WRITE_TRANSFORM;
                case AssetType.MODL:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM;
                case AssetType.ATBL:
                case AssetType.JSP:
                case AssetType.RWTX:
                    return AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM;
                case AssetType.LKIT:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM | AHDRFlags.WRITE_TRANSFORM;
                default:
                    return 0;
            }
        }

        public void CollapseLayers()
        {
            Dictionary<LayerType, Section_LHDR> layers = new Dictionary<LayerType, Section_LHDR>();

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
            {
                if (layers.ContainsKey(LHDR.layerType))
                {
                    layers[LHDR.layerType].assetIDlist.AddRange(LHDR.assetIDlist);
                }
                else if (LHDR.assetIDlist.Count != 0)
                {
                    layers.Add(LHDR.layerType, LHDR);
                }
            }

            DICT.LTOC.LHDRList = new List<Section_LHDR>();
            DICT.LTOC.LHDRList.AddRange(layers.Values.ToList().OrderBy(f => f.layerType, new LHDRComparer()));
        }
        
        public void ApplyScale(Vector3 factor)
        {
            float singleFactor = (factor.X + factor.Y + factor.Z) / 3;

            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetTRIG TRIG)
                {
                    if (TRIG.Shape == TriggerShape.Sphere)
                    {
                        TRIG.Position0X *= factor.X;
                        TRIG.Position0Y *= factor.Y;
                        TRIG.Position0Z *= factor.Z;
                        TRIG.Position1X_Radius *= singleFactor;
                    }
                    else
                    {
                        Vector3 TrigCenter = new Vector3(TRIG.Position0X + TRIG.Position1X_Radius, TRIG.Position0Y + TRIG.Position1Y, TRIG.Position0Z + TRIG.Position1Z) / 2f;

                        TRIG.Position0X -= TrigCenter.X;
                        TRIG.Position0Y -= TrigCenter.Y;
                        TRIG.Position0Z -= TrigCenter.Z;
                        TRIG.Position1X_Radius -= TrigCenter.X;
                        TRIG.Position1Y -= TrigCenter.Y;
                        TRIG.Position1Z -= TrigCenter.Z;

                        TRIG.Position0X *= factor.X;
                        TRIG.Position0Y *= factor.Y;
                        TRIG.Position0Z *= factor.Z;
                        TRIG.Position1X_Radius *= factor.X;
                        TRIG.Position1Y *= factor.Y;
                        TRIG.Position1Z *= factor.Z;

                        TRIG.Position0X += TrigCenter.X * factor.X;
                        TRIG.Position0Y += TrigCenter.Y * factor.Y;
                        TRIG.Position0Z += TrigCenter.Z * factor.Z;
                        TRIG.Position1X_Radius += TrigCenter.X * factor.X;
                        TRIG.Position1Y += TrigCenter.Y *factor.Y;
                        TRIG.Position1Z += TrigCenter.Z * factor.Z;
                    }

                    TRIG._position.X = TRIG.Position0X;
                    TRIG._position.Y = TRIG.Position0Y;
                    TRIG._position.Z = TRIG.Position0Z;
                }
                else if (a is IClickableAsset clickableAsset)
                {
                    ((IClickableAsset)a).PositionX *= factor.X;
                    ((IClickableAsset)a).PositionY *= factor.Y;
                    ((IClickableAsset)a).PositionZ *= factor.Z;

                    if (a is AssetMVPT MVPT)
                    {
                        if (MVPT.ZoneRadius != -1)
                            MVPT.ZoneRadius *= singleFactor;
                        if (MVPT.ArenaRadius != -1)
                            MVPT.ArenaRadius *= singleFactor;
                    }
                    else if (a is AssetSFX SFX)
                    {
                        SFX.OuterRadius *= singleFactor;
                        SFX.InnerRadius *= singleFactor;
                    }
                    else if (a is PlaceableAsset placeable && !(a is AssetPLYR || a is AssetPKUP || a is AssetUI || a is AssetUIFT || a is AssetVIL || (a is AssetDYNA DYNA && DYNA.Type == DynaType.game_object__Teleport)))
                    {
                        placeable.ScaleX *= factor.X;
                        placeable.ScaleY *= factor.Y;
                        placeable.ScaleZ *= factor.Z;
                    }
                }
            }

            UnsavedChanges = true;
            RecalculateAllMatrices();
        }
    }
}