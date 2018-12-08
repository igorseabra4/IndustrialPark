using HipHopFile;
using SharpDX;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetVIL : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x6C + Offset;

        public AssetVIL(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if ((uint)VilType == assetID)
                return true;
            if (AssetID_DYNA_NPCSettings == assetID)
                return true;
            if (AssetID_MVPT == assetID)
                return true;
            if (AssetID_DYNA_1 == assetID)
                return true;
            if (AssetID_DYNA_2 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        private float localFrameCounter = -1;

        public void Reset()
        {
            localFrameCounter = -1;
        }

        public override Matrix LocalWorld()
        {
            if (movementPreview && AssetID_MVPT != 0)
            {
                AssetMVPT driver = FindMVPT(out bool found);

                if (found)
                {
                    return Matrix.Scaling(_scale)
                        * Matrix.Translation(driver.PositionX, driver.PositionY, driver.PositionZ)
                        * Matrix.Translation((float)(driver.MovementRadius * Math.Cos(localFrameCounter * Math.PI / 180f)), 0f, (float)(driver.MovementRadius * Math.Sin(localFrameCounter * Math.PI / 180f)));
                }
            }

            return base.LocalWorld();
        }

        private AssetMVPT FindMVPT(out bool found)
        {
            foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(AssetID_MVPT))
                {
                    Asset asset = ae.archive.GetFromAssetID(AssetID_MVPT);
                    if (asset is AssetMVPT MVPT)
                        return FindMVPTWithRadius(MVPT, out found);
                }

            found = false;
            return null;
        }

        private AssetMVPT FindMVPTWithRadius(AssetMVPT MVPT, out bool found)
        {
            if (MVPT != null)
            {
                if (MVPT.MovementRadius != -1f)
                {
                    found = true;
                    return MVPT;
                }
                foreach (AssetID assetID in MVPT.SiblingMVPTs)
                    foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                        if (ae.archive.ContainsAsset(assetID))
                        {
                            Asset asset = ae.archive.GetFromAssetID(assetID);
                            if (asset is AssetMVPT MVPT2)
                                return FindMVPTWithRadius(MVPT2, out found);
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

            if (DontRender || isInvisible)
                return;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Category("VIL")]
        public int Unknown54
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("VIL")]
        public VilType VilType
        {
            get => (VilType)ReadUInt(0x58 + Offset);
            set => Write(0x58 + Offset, (uint)value);
        }

        [Category("VIL")]
        public AssetID AssetID_DYNA_NPCSettings
        {
            get => ReadUInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("VIL")]
        public AssetID AssetID_MVPT
        {
            get => ReadUInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("VIL")]
        public AssetID AssetID_DYNA_1
        {
            get => ReadUInt(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("VIL")]
        public AssetID AssetID_DYNA_2
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }
    }
}