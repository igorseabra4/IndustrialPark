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

        public AssetVIL(string assetName, Vector3 position, AssetTemplate template, uint mvptAssetID) : base(assetName, AssetType.VIL, BaseAssetType.NPC, position)
        {
            MovePoint = mvptAssetID;

            switch (template)
            {
                case AssetTemplate.Wooden_Tiki:
                    Model = "tiki_wooden_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_wooden_bind;
                    break;
                case AssetTemplate.Floating_Tiki:
                    Model = "tiki_lovey_dovey_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_lovey_dovey_bind;
                    break;
                case AssetTemplate.Thunder_Tiki:
                    Model = "tiki_thunder_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_thunder_bind;
                    break;
                case AssetTemplate.Shhh_Tiki:
                    Model = "tiki_shhhh_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_shhhh_bind;
                    break;
                case AssetTemplate.Stone_Tiki:
                    Model = "tiki_stone_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tiki_stone_bind;
                    break;
                case AssetTemplate.Fodder:
                    Model = "robot_0a_fodder_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_fodder_bind;
                    break;
                case AssetTemplate.Hammer:
                    Model = "ham_bind.MINF";
                    VilType_BFBB = VilType_BFBB.ham_bind;
                    break;
                case AssetTemplate.TarTar:
                    Model = "robot_tar_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_tar_bind;
                    break;
                case AssetTemplate.ChompBot:
                    Model = "robot_0a_chomper_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_chomper_bind;
                    break;
                case AssetTemplate.GLove:
                    Model = "g_love_bind.MINF";
                    VilType_BFBB = VilType_BFBB.g_love_bind;
                    break;
                case AssetTemplate.Chuck:
                case AssetTemplate.Chuck_Trigger:
                    Model = "robot_chuck_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_chuck_bind;
                    break;
                case AssetTemplate.Monsoon:
                case AssetTemplate.Monsoon_Trigger:
                    Model = "robot_4a_monsoon_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_4a_monsoon_bind;
                    break;
                case AssetTemplate.Sleepytime:
                case AssetTemplate.Sleepytime_Moving:
                    Model = "robot_sleepy-time_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_sleepytime_bind;
                    break;
                case AssetTemplate.Arf:
                    Model = "robot_arf_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_arf_bind;
                    break;
                case AssetTemplate.ArfDog:
                    Model = "robot_arf_dog_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_arf_dog_bind;
                    break;
                case AssetTemplate.BombBot:
                    Model = "robot_0a_bomb_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_bomb_bind;
                    break;
                case AssetTemplate.Tubelet:
                    Model = "tubelet_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tubelet_bind;
                    break;
                case AssetTemplate.TubeletSlave:
                    Model = "tubelet_slave_bind.MINF";
                    VilType_BFBB = VilType_BFBB.tubelet_slave_bind;
                    break;
                case AssetTemplate.BzztBot:
                    Model = "robot_0a_bzzt_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_0a_bzzt_bind;
                    break;
                case AssetTemplate.Slick:
                case AssetTemplate.Slick_Trigger:
                    Model = "robot_9a_bind.MINF";
                    VilType_BFBB = VilType_BFBB.robot_9a_bind;
                    break;
                case AssetTemplate.Jellyfish_Pink:
                    Model = "jellyfish_pink_bind.MINF";
                    VilType_BFBB = VilType_BFBB.jellyfish_pink_bind;
                    break;
                case AssetTemplate.Jellyfish_Blue:
                    Model = "jellyfish_blue_bind.MINF";
                    VilType_BFBB = VilType_BFBB.jellyfish_blue_bind;
                    break;
                case AssetTemplate.Duplicatotron:
                    Model = "duplicatotron1000_bind.MINF";
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
                NPCSettingsObject = reader.ReadUInt32();
                MovePoint = reader.ReadUInt32();
                TaskBox1 = reader.ReadUInt32();
                TaskBox2 = reader.ReadUInt32();

                if (game == Game.Incredibles)
                {
                    NavMesh2 = reader.ReadUInt32();
                    NPCSettings = reader.ReadUInt32();
                }
            }
        }

        // meant for use with DUPC VIL only
        public AssetVIL(EndianBinaryReader reader) : base(reader)
        {
            VilFlags.FlagValueInt = reader.ReadUInt32();
            VilType = reader.ReadUInt32();
            NPCSettingsObject = reader.ReadUInt32();
            MovePoint = reader.ReadUInt32();
            TaskBox1 = reader.ReadUInt32();
            TaskBox2 = reader.ReadUInt32();
            NavMesh2 = reader.ReadUInt32();
            NPCSettings = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(VilFlags.FlagValueInt);
                writer.Write(VilType);
                writer.Write(NPCSettingsObject);
                writer.Write(MovePoint);
                writer.Write(TaskBox1);
                writer.Write(TaskBox2);
                if (game == Game.Incredibles)
                {
                    writer.Write(NavMesh2);
                    writer.Write(NPCSettings);
                }
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (game == Game.BFBB && VilType_BFBB.ToString() == VilType.ToString())
                result.Add("VIL with unknown VilType 0x" + VilType.ToString("X8"));

            Verify(NPCSettingsObject, ref result);
            Verify(MovePoint, ref result);
            Verify(TaskBox1, ref result);
            Verify(TaskBox2, ref result);
            if (game == Game.Incredibles)
            {
                Verify(NavMesh2, ref result);
                Verify(NPCSettings, ref result);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.BFBB)
            {
                dt.RemoveProperty("VilType");
                dt.RemoveProperty("NavMesh2");
                dt.RemoveProperty("NPCSettings");
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

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_model))
                ArchiveEditorFunctions.renderingDictionary[_model].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
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