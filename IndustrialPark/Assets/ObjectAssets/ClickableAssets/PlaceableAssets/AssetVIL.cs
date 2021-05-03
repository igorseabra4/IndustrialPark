using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetVIL : EntityAsset
    {
        protected const string categoryName = "VIL";

        [Category(categoryName)]
        public FlagBitmask VilFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName), DisplayName("VilType (Incredibles)")]
        public AssetID VilType { get; set; }
        [Category(categoryName), DisplayName("VilType (BFBB)")]
        public VilType_BFBB VilType_BFBB 
        {
            get => (VilType_BFBB)(uint)VilType;
            set => VilType = (uint)value;
        }
        [Category(categoryName), DisplayName("VilType (BFBB, Alphabetical)")]
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
                foreach (VilType_BFBB o in Enum.GetValues(typeof(VilType_BFBB)))
                    if (o.ToString() == value.ToString())
                    {
                        VilType_BFBB = o;
                        return;
                    }

                throw new ArgumentException("Invalid VilType");
            }
        }

        [Category(categoryName)]
        public AssetID NPCSettings_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID MovePoint_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID TaskDYNA1_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID TaskDYNA2_AssetID { get; set; }

        public AssetVIL(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.VIL, BaseAssetType.NPC, position)
        {
            switch (template)
            {
                case AssetTemplate.WoodenTiki:
                    Model_AssetID = "tiki_wooden_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_wooden_bind;
                    break;
                case AssetTemplate.FloatingTiki:
                    Model_AssetID = "tiki_lovey_dovey_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_lovey_dovey_bind;
                    break;
                case AssetTemplate.ThunderTiki:
                    Model_AssetID = "tiki_thunder_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_thunder_bind;
                    break;
                case AssetTemplate.ShhhTiki:
                    Model_AssetID = "tiki_shhhh_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_shhhh_bind;
                    break;
                case AssetTemplate.StoneTiki:
                    Model_AssetID = "tiki_stone_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_stone_bind;
                    break;
            }
        }
        
        public AssetVIL(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityHeaderEndPosition;

            VilFlags.FlagValueInt = reader.ReadUInt32();
            VilType = reader.ReadUInt32();
            NPCSettings_AssetID = reader.ReadUInt32();
            MovePoint_AssetID = reader.ReadUInt32();
            TaskDYNA1_AssetID = reader.ReadUInt32();
            TaskDYNA2_AssetID = reader.ReadUInt32();
        }

        // meant for use with DUPC VIL only
        public AssetVIL(EndianBinaryReader reader) : base(reader)
        {
            VilFlags.FlagValueInt = reader.ReadUInt32();
            VilType = reader.ReadUInt32();
            NPCSettings_AssetID = reader.ReadUInt32();
            MovePoint_AssetID = reader.ReadUInt32();
            TaskDYNA1_AssetID = reader.ReadUInt32();
            TaskDYNA2_AssetID = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));

            writer.Write(VilFlags.FlagValueInt);
            writer.Write(VilType);
            writer.Write(NPCSettings_AssetID);
            writer.Write(MovePoint_AssetID);
            writer.Write(TaskDYNA1_AssetID);
            writer.Write(TaskDYNA2_AssetID);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => VilType == assetID || NPCSettings_AssetID == assetID ||
            MovePoint_AssetID == assetID || TaskDYNA1_AssetID == assetID || TaskDYNA2_AssetID == assetID ||
            base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (game == Game.BFBB && VilType_BFBB.ToString() == VilType.ToString())
                result.Add("VIL with unknown VilType 0x" + VilType.ToString("X8"));

            Verify(NPCSettings_AssetID, ref result);
            Verify(MovePoint_AssetID, ref result);
            Verify(TaskDYNA1_AssetID, ref result);
            Verify(TaskDYNA2_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.BFBB)
                dt.RemoveProperty("VilType");
            else if (game == Game.Incredibles)
            {
                dt.RemoveProperty("VilType_BFBB");
                dt.RemoveProperty("VilType_Alphabetical");
            }

            base.SetDynamicProperties(dt);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override void Draw(SharpRenderer renderer)
        {
            if (movementPreview)
            {
                localFrameCounter++;
                if (localFrameCounter >= int.MaxValue)
                    localFrameCounter = 0;
            }
            
            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
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
    }
}