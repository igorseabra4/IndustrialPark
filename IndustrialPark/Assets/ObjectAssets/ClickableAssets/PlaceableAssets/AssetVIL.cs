using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetVIL : EntityAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x6C + Offset;

        public AssetVIL(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) => (uint)VilType == assetID || NPCSettings_AssetID == assetID || MovePoint_AssetID == assetID ||
            TaskDYNA1_AssetID == assetID || TaskDYNA2_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (VilType.ToString() == ((int)VilType).ToString())
                result.Add("VIL with unknown VilType 0x" + VilType.ToString("X8"));

            Verify(NPCSettings_AssetID, ref result);
            Verify(MovePoint_AssetID, ref result);
            Verify(TaskDYNA1_AssetID, ref result);
            Verify(TaskDYNA2_AssetID, ref result);
        }
        
        public override Matrix LocalWorld()
        {
            if (movementPreview && MovePoint_AssetID != 0)
            {
                AssetMVPT driver = FindMVPT(out bool found);

                if (found)
                {
                    return Matrix.Scaling(_scale)
                        * Matrix.Translation(driver.PositionX, driver.PositionY, driver.PositionZ)
                        * Matrix.Translation((float)(driver.ZoneRadius * Math.Cos(localFrameCounter * Math.PI / 180f)), 0f, (float)(driver.ZoneRadius * Math.Sin(localFrameCounter * Math.PI / 180f)));
                }
            }

            return base.LocalWorld();
        }

        private AssetMVPT FindMVPT(out bool found)
        {
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(MovePoint_AssetID))
                {
                    Asset asset = ae.archive.GetFromAssetID(MovePoint_AssetID);
                    if (asset is AssetMVPT MVPT)
                        return FindMVPTWithRadius(MVPT, out found, new List<uint>() { MovePoint_AssetID });
                }

            found = false;
            return null;
        }

        private AssetMVPT FindMVPTWithRadius(AssetMVPT MVPT, out bool found, List<uint> list)
        {
            if (MVPT != null)
            {
                if (MVPT.ZoneRadius != -1f)
                {
                    found = true;
                    return MVPT;
                }
                foreach (AssetID assetID in MVPT.NextMVPTs)
                    foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                        if (ae.archive.ContainsAsset(assetID))
                        {
                            Asset asset = ae.archive.GetFromAssetID(assetID);
                            if (asset is AssetMVPT MVPT2 && !list.Contains(assetID))
                            {
                                list.Add(assetID);
                                return FindMVPTWithRadius(MVPT2, out found, list);
                            }
                        }
            }
            found = false;
            return null;
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (movementPreview)
            {
                localFrameCounter++;
                if (localFrameCounter >= int.MaxValue)
                    localFrameCounter = 0;
            }

            if (!isSelected && (dontRender || isInvisible))
                return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Category("VIL")]
        public DynamicTypeDescriptor VilFlags => IntFlagsDescriptor(0x54 + Offset);

        [Category("VIL")]
        public VilType VilType
        {
            get => (VilType)ReadUInt(0x58 + Offset);
            set => Write(0x58 + Offset, (uint)value);
        }

        [Category("VIL"), DisplayName("VilType (Alphabetical)")]
        public VilType_Alphabetical VilType_Alphabetical
        {
            get
            {
                foreach (VilType_Alphabetical o in Enum.GetValues(typeof(VilType_Alphabetical)))
                    if (o.ToString() == VilType.ToString())
                        return o;

                return VilType_Alphabetical.Null;
            }
            set
            {
                foreach (VilType o in Enum.GetValues(typeof(VilType)))
                    if (o.ToString() == value.ToString())
                    {
                        Write(0x58 + Offset, (uint)o);
                        return;
                    }

                throw new Exception();
            }
        }

        [Category("VIL")]
        public AssetID NPCSettings_AssetID
        {
            get => ReadUInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("VIL")]
        public AssetID MovePoint_AssetID
        {
            get => ReadUInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("VIL")]
        public AssetID TaskDYNA1_AssetID
        {
            get => ReadUInt(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("VIL")]
        public AssetID TaskDYNA2_AssetID
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }
    }
}