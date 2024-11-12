using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetVIL : EntityAsset
    {
        protected const string categoryName = "NPC";

        [Category(categoryName)]
        public FlagBitmask NpcFlags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName), DisplayName("NPC Type (Incredibles)")]
        public AssetID NpcType { get; set; }
        [Category(categoryName), DisplayName("NPC Type (BFBB)")]
        public NpcType_BFBB NpcType_BFBB
        {
            get => (NpcType_BFBB)(uint)NpcType;
            set => NpcType = (uint)value;
        }
        [Category(categoryName), DisplayName("NPC Type (BFBB, Alphabetical)")]
        public NpcType_Alphabetical NpcType_Alphabetical
        {
            get
            {
                foreach (NpcType_Alphabetical o in Enum.GetValues(typeof(NpcType_Alphabetical)))
                    if (o.ToString() == NpcType_BFBB.ToString())
                        return o;

                return NpcType_Alphabetical.Null;
            }
            set
            {
                foreach (NpcType_BFBB o in Enum.GetValues(typeof(NpcType_BFBB)))
                    if (o.ToString() == value.ToString())
                    {
                        NpcType_BFBB = o;
                        return;
                    }

                throw new ArgumentException("Invalid NpcType");
            }
        }

        [Category(categoryName)]
        public AssetID NPCSettingsObject { get; set; }
        [Category(categoryName)]
        public AssetID MovePoint { get; set; }
        [Category(categoryName)]
        public AssetID TaskBox1 { get; set; }
        [Category(categoryName)]
        public AssetID TaskBox2 { get; set; }
        [Category(categoryName)]
        public AssetID NavMesh2 { get; set; }
        [Category(categoryName)]
        public AssetID NPCSettings { get; set; }

        public AssetVIL(string assetName, Vector3 position, AssetTemplate template, uint mvptAssetID) : base(assetName, AssetType.NPC, BaseAssetType.NPC, position)
        {
            MovePoint = mvptAssetID;

            switch (template)
            {
                case AssetTemplate.Wooden_Tiki:
                    Model = "tiki_wooden_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.tiki_wooden_bind;
                    break;
                case AssetTemplate.Floating_Tiki:
                    Model = "tiki_lovey_dovey_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.tiki_lovey_dovey_bind;
                    break;
                case AssetTemplate.Thunder_Tiki:
                    Model = "tiki_thunder_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.tiki_thunder_bind;
                    break;
                case AssetTemplate.Shhh_Tiki:
                    Model = "tiki_shhhh_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.tiki_shhhh_bind;
                    break;
                case AssetTemplate.Stone_Tiki:
                    Model = "tiki_stone_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.tiki_stone_bind;
                    break;
                case AssetTemplate.Fodder:
                    Model = "robot_0a_fodder_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_0a_fodder_bind;
                    break;
                case AssetTemplate.Hammer:
                    Model = "ham_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.ham_bind;
                    break;
                case AssetTemplate.TarTar:
                    Model = "robot_tar_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_tar_bind;
                    break;
                case AssetTemplate.ChompBot:
                    Model = "robot_0a_chomper_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_0a_chomper_bind;
                    break;
                case AssetTemplate.GLove:
                    Model = "g_love_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.g_love_bind;
                    break;
                case AssetTemplate.Chuck:
                case AssetTemplate.Chuck_Trigger:
                    Model = "robot_chuck_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_chuck_bind;
                    break;
                case AssetTemplate.Monsoon:
                case AssetTemplate.Monsoon_Trigger:
                    Model = "robot_4a_monsoon_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_4a_monsoon_bind;
                    break;
                case AssetTemplate.Sleepytime:
                case AssetTemplate.Sleepytime_Moving:
                    Model = "robot_sleepy-time_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_sleepytime_bind;
                    break;
                case AssetTemplate.Arf:
                    Model = "robot_arf_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_arf_bind;
                    break;
                case AssetTemplate.ArfDog:
                    Model = "robot_arf_dog_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_arf_dog_bind;
                    break;
                case AssetTemplate.BombBot:
                    Model = "robot_0a_bomb_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_0a_bomb_bind;
                    break;
                case AssetTemplate.Tubelet:
                    Model = "tubelet_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.tubelet_bind;
                    break;
                case AssetTemplate.TubeletSlave:
                    Model = "tubelet_slave_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.tubelet_slave_bind;
                    break;
                case AssetTemplate.BzztBot:
                    Model = "robot_0a_bzzt_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_0a_bzzt_bind;
                    break;
                case AssetTemplate.Slick:
                case AssetTemplate.Slick_Trigger:
                    Model = "robot_9a_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.robot_9a_bind;
                    break;
                case AssetTemplate.Jellyfish_Pink:
                    Model = "jellyfish_pink_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.jellyfish_pink_bind;
                    break;
                case AssetTemplate.Jellyfish_Blue:
                    Model = "jellyfish_blue_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.jellyfish_blue_bind;
                    break;
                case AssetTemplate.Duplicatotron:
                    Model = "duplicatotron1000_bind.MINF";
                    NpcType_BFBB = NpcType_BFBB.duplicatotron1000_bind;
                    break;
            }
        }

        public AssetVIL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                NpcFlags.FlagValueInt = reader.ReadUInt32();
                NpcType = reader.ReadUInt32();
                NPCSettingsObject = reader.ReadUInt32();
                MovePoint = reader.ReadUInt32();
                TaskBox1 = reader.ReadUInt32();
                TaskBox2 = reader.ReadUInt32();

                if (game >= Game.Incredibles)
                {
                    NavMesh2 = reader.ReadUInt32();
                    NPCSettings = reader.ReadUInt32();
                }
            }
        }

        // meant for use with DUPC VIL only
        public AssetVIL(EndianBinaryReader reader, Game game) : base(reader, game)
        {
            NpcFlags.FlagValueInt = reader.ReadUInt32();
            NpcType = reader.ReadUInt32();
            NPCSettingsObject = reader.ReadUInt32();
            MovePoint = reader.ReadUInt32();
            TaskBox1 = reader.ReadUInt32();
            TaskBox2 = reader.ReadUInt32();
            NavMesh2 = reader.ReadUInt32();
            NPCSettings = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(NpcFlags.FlagValueInt);
            writer.Write(NpcType);
            writer.Write(NPCSettingsObject);
            writer.Write(MovePoint);
            writer.Write(TaskBox1);
            writer.Write(TaskBox2);
            if (game >= Game.Incredibles)
            {
                writer.Write(NavMesh2);
                writer.Write(NPCSettings);
            }
            SerializeLinks(writer);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (game == Game.BFBB && NpcType_BFBB.ToString() == NpcType.ToString())
                result.Add("Unknown NpcType 0x" + NpcType.ToString("X8"));
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.BFBB)
            {
                dt.RemoveProperty("NpcType");
                dt.RemoveProperty("NavMesh2");
                dt.RemoveProperty("NPCSettings");
            }
            else if (game >= Game.Incredibles)
            {
                dt.RemoveProperty("NpcType_BFBB");
                dt.RemoveProperty("NpcType_Alphabetical");
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

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_model))
                ArchiveEditorFunctions.renderingDictionary[_model].Draw(renderer, LocalWorld(), _color, UvAnimOffset, isSelected);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        public override Matrix LocalWorld()
        {
            if (movementPreview && MovePoint != 0)
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
                if (ae.archive.ContainsAsset(MovePoint))
                {
                    Asset asset = ae.archive.GetFromAssetID(MovePoint);
                    if (asset is AssetMVPT MVPT)
                        return FindMVPTWithRadius(MVPT, out found, new List<uint>() { MovePoint });
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
                foreach (AssetID assetID in MVPT.NextMovePoints)
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