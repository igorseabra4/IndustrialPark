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
                    if (o.ToString() == VilType_BFBB.ToString())
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
        [Category(categoryName)]
        public AssetID NavMesh2_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID Settings_AssetID { get; set; }

        public AssetVIL(string assetName, Vector3 position, AssetTemplate template, uint mvptAssetID) : base(assetName, AssetType.VIL, BaseAssetType.NPC, position)
        {
            MovePoint_AssetID = mvptAssetID;

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
                case AssetTemplate.Fodder:
                    Model_AssetID = "robot_0a_fodder_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_fodder_bind;
                    break;
                case AssetTemplate.Hammer:
                    Model_AssetID = "ham_bind.MINF";
                    VilType_BFBB = VilType_BFBB.ham_bind;
                    break;
                case AssetTemplate.TarTar:
                    Model_AssetID = "robot_tar_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_tar_bind;
                    break;
                case AssetTemplate.ChompBot:
                    Model_AssetID = "robot_0a_chomper_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_chomper_bind;
                    break;
                case AssetTemplate.GLove:
                    Model_AssetID = "g_love_bind.MINF";
                    VilType_BFBB = VilType_BFBB.g_love_bind;
                    break;
                case AssetTemplate.Chuck:
                case AssetTemplate.Chuck_Trigger:
                    Model_AssetID = "robot_chuck_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_chuck_bind;
                    break;
                case AssetTemplate.Monsoon:
                case AssetTemplate.Monsoon_Trigger:
                    Model_AssetID = "robot_4a_monsoon_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_4a_monsoon_bind;
                    break;
                case AssetTemplate.Sleepytime:
                case AssetTemplate.Sleepytime_Moving:
                    Model_AssetID = "robot_sleepy-time_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_sleepytime_bind;
                    break;
                case AssetTemplate.Arf:
                    Model_AssetID = "robot_arf_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_arf_bind;
                    break;
                case AssetTemplate.ArfDog:
                    Model_AssetID = "robot_arf_dog_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_arf_dog_bind;
                    break;
                case AssetTemplate.BombBot:
                    Model_AssetID = "robot_0a_bomb_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_bomb_bind;
                    break;
                case AssetTemplate.Tubelet:
                    Model_AssetID = "tubelet_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tubelet_bind;
                    break;
                case AssetTemplate.TubeletSlave:
                    Model_AssetID = "tubelet_slave_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tubelet_slave_bind;
                    break;
                case AssetTemplate.BzztBot:
                    Model_AssetID = "robot_0a_bzzt_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_bzzt_bind;
                    break;
                case AssetTemplate.Slick:
                case AssetTemplate.Slick_Trigger:
                    Model_AssetID = "robot_9a_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_9a_bind;
                    break;
                case AssetTemplate.Jellyfish_Pink:
                    Model_AssetID = "jellyfish_pink_bind.MINF";
                    VilType_BFBB = VilType_BFBB.jellyfish_pink_bind;
                    break;
                case AssetTemplate.Jellyfish_Blue:
                    Model_AssetID = "jellyfish_blue_bind.MINF";
                    VilType_BFBB = VilType_BFBB.jellyfish_blue_bind;
                    break;
                case AssetTemplate.Duplicatotron:
                    Model_AssetID = "duplicatotron1000_bind.MINF";
                    VilType_BFBB = VilType_BFBB.duplicatotron1000_bind;
                    break;
            }
        }

        public AssetVIL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                VilFlags.FlagValueInt = reader.ReadUInt32();
                VilType = reader.ReadUInt32();
                NPCSettings_AssetID = reader.ReadUInt32();
                MovePoint_AssetID = reader.ReadUInt32();
                TaskDYNA1_AssetID = reader.ReadUInt32();
                TaskDYNA2_AssetID = reader.ReadUInt32();

                if (game == Game.Incredibles)
                {
                    NavMesh2_AssetID = reader.ReadUInt32();
                    Settings_AssetID = reader.ReadUInt32();
                }
            }
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
            NavMesh2_AssetID = reader.ReadUInt32();
            Settings_AssetID = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(VilFlags.FlagValueInt);
                writer.Write(VilType);
                writer.Write(NPCSettings_AssetID);
                writer.Write(MovePoint_AssetID);
                writer.Write(TaskDYNA1_AssetID);
                writer.Write(TaskDYNA2_AssetID);
                if (game == Game.Incredibles)
                {
                    writer.Write(NavMesh2_AssetID);
                    writer.Write(Settings_AssetID);
                }
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => VilType == assetID || NPCSettings_AssetID == assetID ||
            MovePoint_AssetID == assetID || TaskDYNA1_AssetID == assetID || TaskDYNA2_AssetID == assetID ||
            NavMesh2_AssetID == assetID || Settings_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (game == Game.BFBB && VilType_BFBB.ToString() == VilType.ToString())
                result.Add("VIL with unknown VilType 0x" + VilType.ToString("X8"));

            Verify(NPCSettings_AssetID, ref result);
            Verify(MovePoint_AssetID, ref result);
            Verify(TaskDYNA1_AssetID, ref result);
            Verify(TaskDYNA2_AssetID, ref result);
            Verify(NavMesh2_AssetID, ref result);
            Verify(Settings_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.BFBB)
            {
                dt.RemoveProperty("VilType");
                dt.RemoveProperty("NavMesh2_AssetID");
                dt.RemoveProperty("Settings_AssetID");
            }
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