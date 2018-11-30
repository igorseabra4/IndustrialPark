using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HipHopFile;
using SharpDX;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public void SetAssetPositionToView(uint assetID)
        {
            Vector3 Position = Program.MainForm.renderer.Camera.Position + 2 * Program.MainForm.renderer.Camera.GetForward();

            if (GetFromAssetID(assetID) is AssetTRIG trig)
            {
                trig.PositionX = Position.X;
                trig.PositionY = Position.Y;
                trig.PositionZ = Position.Z;

                trig.Position0X = Position.X;
                trig.Position0Y = Position.Y;
                trig.Position0Z = Position.Z;
            }
            else if (GetFromAssetID(assetID) is PlaceableAsset ir)
            {
                ir.PositionX = Position.X;
                ir.PositionY = Position.Y;
                ir.PositionZ = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetCAM cam)
            {
                cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                cam.SetNormalizedForward(Program.MainForm.renderer.Camera.GetForward());
                cam.SetNormalizedUp(Program.MainForm.renderer.Camera.GetUp());
            }
            else if (GetFromAssetID(assetID) is AssetMRKR mrkr)
            {
                mrkr.PositionX = Position.X;
                mrkr.PositionY = Position.Y;
                mrkr.PositionZ = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetMVPT mvpt)
            {
                mvpt.PositionX = Position.X;
                mvpt.PositionY = Position.Y;
                mvpt.PositionZ = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetSFX sfx)
            {
                sfx.PositionX = Position.X;
                sfx.PositionY = Position.Y;
                sfx.PositionZ = Position.Z;
            }
        }

        public static byte[] GetTemplate(AssetType assetType)
        {
            string[] files = Directory.GetFiles(Application.StartupPath + "\\Resources\\Templates\\" + currentGame.ToString() + "\\");
            foreach (string s in files)
                if (Path.GetFileName(s) == assetType.ToString())
                    return File.ReadAllBytes(s);

            return null;
        }

        public static AssetTemplate CurrentAssetTemplate { get; set; } = AssetTemplate.Null;
        public static string CurrentUserTemplate { get; set; } = "";

        public uint PlaceTemplate(Vector3 position, int layerIndex, out bool success, ref List<uint> assetIDs, string customName = "", AssetTemplate template = AssetTemplate.Null)
        {
            AssetType newAssetType = AssetType.Null;

            if (template == AssetTemplate.Null)
                template = CurrentAssetTemplate;

            if (template == AssetTemplate.UserTemplate)
            {
                return PlaceUserTemplate(position, layerIndex, out success, ref assetIDs);
            }

            switch (template)
            {
                case AssetTemplate.AnimationList:
                    newAssetType = AssetType.ALST;
                    break;
                case AssetTemplate.Boulder_Generic:
                    newAssetType = AssetType.BOUL;
                    break;
                case AssetTemplate.Button_Generic:
                case AssetTemplate.Button_Red:
                case AssetTemplate.PressurePlate:
                    newAssetType = AssetType.BUTN;
                    break;
                case AssetTemplate.Camera:
                case AssetTemplate.CharacterSwitch_Camera:
                    newAssetType = AssetType.CAM;
                    break;
                case AssetTemplate.Counter:
                    newAssetType = AssetType.CNTR;
                    break;
                case AssetTemplate.CollisionTable:
                    newAssetType = AssetType.COLL;
                    break;
                case AssetTemplate.Conditional:
                    newAssetType = AssetType.COND;
                    break;
                case AssetTemplate.Dispatcher:
                    newAssetType = AssetType.DPAT;
                    break;
                case AssetTemplate.Destructible_Generic:
                    newAssetType = AssetType.DSTR;
                    break;
                case AssetTemplate.CharSwitch:
                case AssetTemplate.DuplicatotronSettings:
                    newAssetType = AssetType.DYNA;
                    break;
                case AssetTemplate.ElectricArc_Generic:
                    newAssetType = AssetType.EGEN;
                    break;
                case AssetTemplate.Environment:
                    newAssetType = AssetType.ENV;
                    break;
                case AssetTemplate.Group:
                    newAssetType = AssetType.GRUP;
                    break;
                case AssetTemplate.JawData:
                    newAssetType = AssetType.JAW;
                    break;
                case AssetTemplate.LevelOfDetailTable:
                    newAssetType = AssetType.LODT;
                    break;
                case AssetTemplate.MaterialMap:
                    newAssetType = AssetType.MAPR;
                    break;
                case AssetTemplate.Marker:
                    newAssetType = AssetType.MRKR;
                    break;
                case AssetTemplate.EnemyAreaMVPT:
                case AssetTemplate.PointMVPT:
                    newAssetType = AssetType.MVPT;
                    break;
                case AssetTemplate.PipeInfoTable:
                    newAssetType = AssetType.PIPT;
                    break;
                case AssetTemplate.Shiny_Red:
                case AssetTemplate.Shiny_Yellow:
                case AssetTemplate.Shiny_Green:
                case AssetTemplate.Shiny_Blue:
                case AssetTemplate.Shiny_Purple:
                case AssetTemplate.Underwear:
                case AssetTemplate.Spatula:
                case AssetTemplate.Sock:
                case AssetTemplate.Spongeball:
                    newAssetType = AssetType.PKUP;
                    break;
                case AssetTemplate.Platform_Generic:
                    newAssetType = AssetType.PLAT;
                    break;
                case AssetTemplate.Player_Generic:
                    newAssetType = AssetType.PLYR;
                    break;
                case AssetTemplate.Portal:
                    newAssetType = AssetType.PORT;
                    break;
                case AssetTemplate.ShadowTable:
                    newAssetType = AssetType.SHDW;
                    break;
                case AssetTemplate.SIMP_Generic:
                case AssetTemplate.TaxiStand:
                case AssetTemplate.TexasHitch:
                case AssetTemplate.PressurePlateBase:
                case AssetTemplate.CharacterSwitch_BusSimp:
                case AssetTemplate.BusStop:
                case AssetTemplate.BusStop_LightSimp:
                    newAssetType = AssetType.SIMP;
                    break;
                case AssetTemplate.SoundInfo:
                    customName = "sound_info";
                    newAssetType = AssetType.SNDI;
                    break;
                case AssetTemplate.SphereTrigger:
                case AssetTemplate.BusStop_Trigger:
                    newAssetType = AssetType.TRIG;
                    break;
                case AssetTemplate.Text:
                    newAssetType = AssetType.TEXT;
                    break;
                case AssetTemplate.Timer:
                    newAssetType = AssetType.TIMR;
                    break;
                case AssetTemplate.WoodenTiki:
                case AssetTemplate.FloatingTiki:
                case AssetTemplate.ThunderTiki:
                case AssetTemplate.ShhhTiki:
                case AssetTemplate.StoneTiki:
                case AssetTemplate.Fodder:
                case AssetTemplate.Hammer:
                case AssetTemplate.TarTar:
                case AssetTemplate.ChompBot:
                case AssetTemplate.GLove:
                case AssetTemplate.Chuck:
                case AssetTemplate.Chuck_Trigger:
                case AssetTemplate.Monsoon:
                case AssetTemplate.Monsoon_Trigger:
                case AssetTemplate.Sleepytime:
                case AssetTemplate.Sleepytime_Moving:
                case AssetTemplate.BombBot:
                case AssetTemplate.Tubelet:
                case AssetTemplate.TubeletSlave:
                case AssetTemplate.BzztBot:
                case AssetTemplate.Slick:
                case AssetTemplate.Slick_Trigger:
                case AssetTemplate.Jellyfish_Pink:
                case AssetTemplate.Jellyfish_Blue:
                case AssetTemplate.Duplicatotron:
                case AssetTemplate.VIL_Generic:
                    newAssetType = AssetType.VIL;
                    break;
                default:
                    if (template != AssetTemplate.Null)
                        MessageBox.Show("Unsupported asset template");
                    success = false;
                    return 0;
            }

            Section_AHDR newAsset = new Section_AHDR
            {
                assetType = newAssetType,
                flags = AHDRFlagsFromAssetType(newAssetType),
                data = GetTemplate(newAssetType)
            };

            if (newAssetType == AssetType.DYNA)
                newAsset.data = new byte[0x10];

            if (string.IsNullOrWhiteSpace(customName))
                newAsset.ADBG = new Section_ADBG(0, template.ToString().ToUpper() + "_01", "", 0);
            else
                newAsset.ADBG = new Section_ADBG(0, customName + "_01", "", 0);

            Asset asset = GetFromAssetID(AddAssetWithUniqueID(layerIndex, newAsset, "_", true));

            success = true;

            if (asset is PlaceableAsset placeableAsset)
            {
                placeableAsset.PositionX = position.X;
                placeableAsset.PositionY = position.Y;
                placeableAsset.PositionZ = position.Z;
            }
            else if (asset is AssetMRKR mrkr)
            {
                mrkr.PositionX = position.X;
                mrkr.PositionY = position.Y;
                mrkr.PositionZ = position.Z;
            }
            else if (asset is AssetCAM cam)
            {
                cam.PositionX = position.X;
                cam.PositionY = position.Y;
                cam.PositionZ = position.Z;
            }

            switch (template)
            {
                case AssetTemplate.Shiny_Red:
                    ((AssetPKUP)asset).Shape = 0x3E;
                    ((AssetPKUP)asset).PickReferenceID = 0x7C8AC53E;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Yellow:
                    ((AssetPKUP)asset).Shape = 0x3B;
                    ((AssetPKUP)asset).PickReferenceID = 0xB3D6283B;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Green:
                    ((AssetPKUP)asset).Shape = 0x34;
                    ((AssetPKUP)asset).PickReferenceID = 0x079A0734;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Blue:
                    ((AssetPKUP)asset).Shape = 0x81;
                    ((AssetPKUP)asset).PickReferenceID = 0x6D4A4181;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Purple:
                    ((AssetPKUP)asset).Shape = 0xCB;
                    ((AssetPKUP)asset).PickReferenceID = 0xFA607BCB;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Underwear:
                    ((AssetPKUP)asset).Shape = 0x13;
                    ((AssetPKUP)asset).PickReferenceID = 0x28F55613;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Spatula:
                    ((AssetPKUP)asset).Shape = 0xDD;
                    ((AssetPKUP)asset).PickReferenceID = 0x8BDFE8DD;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Sock:
                    ((AssetPKUP)asset).Shape = 0x24;
                    ((AssetPKUP)asset).PickReferenceID = 0x74B46F24;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Spongeball:
                    ((AssetPKUP)asset).Shape = 0x15;
                    ((AssetPKUP)asset).PickReferenceID = 0xF09A1415;
                    ((AssetPKUP)asset).UnknownShort58 = 3;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.WoodenTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_wooden_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_wooden_bind;
                    break;
                case AssetTemplate.FloatingTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_lovey_dovey_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_lovey_dovey_bind;
                    break;
                case AssetTemplate.ThunderTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_thunder_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_thunder_bind;
                    break;
                case AssetTemplate.ShhhTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_shhhh_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_shhhh_bind;
                    break;
                case AssetTemplate.StoneTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_stone_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_stone_bind;
                    break;
                case AssetTemplate.Fodder:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_fodder_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_fodder_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Hammer:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("ham_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.ham_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.TarTar:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_tar_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_tar_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.ChompBot:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_chomper_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_chomper_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.GLove:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("g_love_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.g_love_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Chuck:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_chuck_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Chuck_Trigger:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_chuck_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };
                    AssetTRIG chuckTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.SphereTrigger));
                    chuckTrigger.Position1X_Radius = 15f;
                    chuckTrigger.EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.EnterPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOn
                        },
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ExitPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Monsoon:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_4a_monsoon_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_4a_monsoon_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Monsoon_Trigger:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_4a_monsoon_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_4a_monsoon_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG monsoonTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.SphereTrigger));
                    monsoonTrigger.Position1X_Radius = 15f;
                    monsoonTrigger.EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.EnterPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOn
                        },
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ExitPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Sleepytime_Moving:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_sleepy-time_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_sleepytime_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Sleepytime:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_sleepy-time_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_sleepytime_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetMVPT)GetFromAssetID(((AssetVIL)asset).AssetID_MVPT)).MovementRadius = -1;
                    break;
                case AssetTemplate.BombBot:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_bomb_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_bomb_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Tubelet:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tubelet_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tubelet_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SLAVEA", AssetTemplate.TubeletSlave),
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.Connect_IOwnYou
                        },
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SLAVEB", AssetTemplate.TubeletSlave),
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.Connect_IOwnYou
                        }
                    };
                    break;
                case AssetTemplate.TubeletSlave:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tubelet_slave_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tubelet_slave_bind;
                    break;
                case AssetTemplate.BzztBot:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_bzzt_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_bzzt_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Slick:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_9a_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Slick_Trigger:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_9a_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG slickTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.SphereTrigger));
                    slickTrigger.Position1X_Radius = 15f;
                    slickTrigger.EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.EnterPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOn
                        },
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ExitPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Jellyfish_Pink:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("jellyfish_pink_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.jellyfish_pink_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Jellyfish_Blue:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("jellyfish_blue_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.jellyfish_blue_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Duplicatotron:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("duplicatotron1000_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.duplicatotron1000_bind;
                    ((AssetVIL)asset).AssetID_DYNA_NPCSettings = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SETTINGS", AssetTemplate.DuplicatotronSettings);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_GROUP", AssetTemplate.Group),
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.Connect_IOwnYou
                        }
                    };
                    break;
                case AssetTemplate.DuplicatotronSettings:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type = DynaType.game_object__NPCSettings;
                    ((AssetDYNA)asset).DynaBase = new DynaNPCSettings()
                    {
                        Flags1 = 1,
                        Flags2 = 1,
                        Flags3 = 1,
                        Flags5 = 1,
                        Flags9 = 1,
                        Flags10 = 1,
                        DuploSpawnRate = 1f,
                        DuploEnemyLimit = -1
                    };
                    break;
                case AssetTemplate.Button_Red:
                    ((AssetBUTN)asset).ModelAssetID = BKDRHash("button");
                    ((AssetBUTN)asset).PressedModelAssetID = BKDRHash("button_grn");
                    ((AssetBUTN)asset).UnknownByte6C = 4;
                    ((AssetBUTN)asset).UnknownByte6F = 4;
                    ((AssetBUTN)asset).UnknownByte70 = 2;
                    ((AssetBUTN)asset).UnknownByte71 = 1;
                    ((AssetBUTN)asset).UnknownByte72 = 1;
                    ((AssetBUTN)asset).PressedOffset = -0.2f;
                    ((AssetBUTN)asset).TransitionTime = 0.5f;
                    ((AssetBUTN)asset).TransitionEaseOut = 0.2f;
                    ((AssetBUTN)asset).BubbleSpin = true;
                    ((AssetBUTN)asset).BubbleBowl = true;
                    ((AssetBUTN)asset).CruiseBubble = true;
                    ((AssetBUTN)asset).Throwable = true;
                    ((AssetBUTN)asset).PatrickBelly = true;
                    ((AssetBUTN)asset).SandyMelee = true;
                    break;
                case AssetTemplate.PressurePlate:
                    ((AssetBUTN)asset).ButtonType = AssetBUTN.ButnHitMode.PressurePlate;
                    ((AssetBUTN)asset).ModelAssetID = BKDRHash("plate_pressure");
                    ((AssetBUTN)asset).PressedModelAssetID = 0xCE7F8131;
                    ((AssetBUTN)asset).UnknownByte6C = 4;
                    ((AssetBUTN)asset).UnknownByte6F = 4;
                    ((AssetBUTN)asset).UnknownByte70 = 2;
                    ((AssetBUTN)asset).UnknownByte71 = 1;
                    ((AssetBUTN)asset).UnknownByte72 = 1;
                    ((AssetBUTN)asset).PressedOffset = -0.15f;
                    ((AssetBUTN)asset).TransitionTime = 0.15f;
                    ((AssetBUTN)asset).PlayerOnPressurePlate = true;
                    ((AssetBUTN)asset).AnyThrowableOnPressurePlate = true;
                    ((AssetBUTN)asset).ThrowFruitOnPressurePlate = true;
                    ((AssetBUTN)asset).BubbleBowlOnPressurePlate = true;

                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_BASE", AssetTemplate.PressurePlateBase);
                    break;
                case AssetTemplate.PressurePlateBase:
                    ((AssetSIMP)asset).ModelAssetID = BKDRHash("plate_pressure_base");
                    break;
                case AssetTemplate.TaxiStand:
                    ((AssetSIMP)asset).ModelAssetID = BKDRHash("taxi_stand");
                    break;
                case AssetTemplate.TexasHitch:
                    ((AssetSIMP)asset).ModelAssetID = BKDRHash("trailer_hitch");
                    break;
                case AssetTemplate.EnemyAreaMVPT:
                    ((AssetMVPT)asset).PositionX = position.X;
                    ((AssetMVPT)asset).PositionY = position.Y;
                    ((AssetMVPT)asset).PositionZ = position.Z;
                    ((AssetMVPT)asset).UnknownByte14 = 0x27;
                    ((AssetMVPT)asset).UnknownByte15 = 0x10;
                    ((AssetMVPT)asset).UnknownByte16 = 0x01;
                    ((AssetMVPT)asset).PointType = 0x00;
                    ((AssetMVPT)asset).MovementAngle = 360;
                    ((AssetMVPT)asset).MovementRadius = 4;
                    ((AssetMVPT)asset).DistanceICanSeeYou = 8;
                    break;
                case AssetTemplate.PointMVPT:
                    ((AssetMVPT)asset).PositionX = position.X;
                    ((AssetMVPT)asset).PositionY = position.Y;
                    ((AssetMVPT)asset).PositionZ = position.Z;
                    ((AssetMVPT)asset).UnknownByte14 = 0x27;
                    ((AssetMVPT)asset).UnknownByte15 = 0x10;
                    ((AssetMVPT)asset).UnknownByte16 = 0x01;
                    ((AssetMVPT)asset).PointType = 0x00;
                    ((AssetMVPT)asset).MovementAngle = 0;
                    ((AssetMVPT)asset).MovementRadius = -1;
                    ((AssetMVPT)asset).DistanceICanSeeYou = -1;
                    break;
                case AssetTemplate.SphereTrigger:
                    ((AssetTRIG)asset).PositionX = position.X;
                    ((AssetTRIG)asset).PositionY = position.Y;
                    ((AssetTRIG)asset).PositionZ = position.Z;
                    ((AssetTRIG)asset).Position1X_Radius = 10f;
                    break;
                case AssetTemplate.CharSwitch:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type = DynaType.game_object__BusStop;
                    ((AssetDYNA)asset).DynaBase = new DynaBusStop()
                    {
                        MRKR_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MRKR", AssetTemplate.Marker),
                        Player = 0,
                        CAM_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_CAM", AssetTemplate.CharacterSwitch_Camera),
                        SIMP_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SIMP", AssetTemplate.CharacterSwitch_BusSimp),
                        CharacterSwitchTimer = 1.5f
                    };
                    break;
                case AssetTemplate.CharacterSwitch_Camera:
                    ((AssetCAM)asset).PositionX -= 7f;
                    ((AssetCAM)asset).PositionY += 2f;
                    ((AssetCAM)asset).NormalizedForwardX = 0.980334f;
                    ((AssetCAM)asset).NormalizedForwardY = -0.119257f;
                    ((AssetCAM)asset).NormalizedForwardZ = -0.157237f;
                    ((AssetCAM)asset).NormalizedUpX = 0.117752f;
                    ((AssetCAM)asset).NormalizedUpX = 0.992864f;
                    ((AssetCAM)asset).NormalizedUpX = -0.018886f;
                    ((AssetCAM)asset).Float2C = -0.158367f;
                    ((AssetCAM)asset).UnknownValue30 = -1317011456;
                    ((AssetCAM)asset).UnknownValue34 = 0x3F7CC4FD;
                    ((AssetCAM)asset).UnknownShort44 = 30;
                    ((AssetCAM)asset).UnknownShort46 = 45;
                    ((AssetCAM)asset).CameraSpeed = 60f;
                    ((AssetCAM)asset).Float4C = 0.5f;
                    ((AssetCAM)asset).UnknownFloat64 = -2f;
                    ((AssetCAM)asset).UnknownFloat68 = 1f;
                    ((AssetCAM)asset).UnknownFloat6C = 1f;
                    ((AssetCAM)asset).Flags1 = 00;
                    ((AssetCAM)asset).Flags2 = 01;
                    ((AssetCAM)asset).Flags3 = 01;
                    ((AssetCAM)asset).Flags4 = 0x8F;
                    break;
                case AssetTemplate.CharacterSwitch_BusSimp:
                    ((AssetSIMP)asset).PositionX -= 3f;
                    ((AssetSIMP)asset).VisibilityFlag = 0;
                    ((AssetSIMP)asset).SolidityFlag = 2;
                    ((AssetSIMP)asset).ModelAssetID = BKDRHash("bus_bind");
                    ((AssetSIMP)asset).AnimationAssetID = BKDRHash("BUSSTOP_ANIMLIST_01");
                    ((AssetSIMP)asset).UnknownFloat_54 = 1f;
                    ((AssetSIMP)asset).Unknown_5C = 0;
                    break;
            }

            assetIDs.Add(asset.AHDR.assetID);

            return asset.AHDR.assetID;
        }

        private uint PlaceUserTemplate(Vector3 Position, int layerIndex, out bool success, ref List<uint> assetIDs)
        {
            if (!File.Exists(Path.Combine(Program.MainForm.userTemplatesFolder, CurrentUserTemplate)))
            {
                success = false;
                return 0;
            }

            string assetTypeName = CurrentUserTemplate.Substring(CurrentUserTemplate.IndexOf('[') + 1, CurrentUserTemplate.IndexOf(']') - CurrentUserTemplate.IndexOf('[') - 1);
            AssetType assetType = AssetType.Null;

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
            {
                if (o.ToString() == assetTypeName.Trim().ToUpper())
                {
                    assetType = o;
                    break;
                }
            }
            if (assetType == AssetType.Null) throw new Exception("Unknown asset type: " + assetType);

            Section_AHDR newAsset = new Section_AHDR
            {
                assetType = assetType,
                flags = AHDRFlagsFromAssetType(assetType),
                data = File.ReadAllBytes(Path.Combine(Program.MainForm.userTemplatesFolder, CurrentUserTemplate))
            };

            newAsset.ADBG = new Section_ADBG(0, CurrentUserTemplate.Substring(CurrentUserTemplate.IndexOf(']') + 2) + "_T001", "", 0);

            Asset asset = GetFromAssetID(AddAssetWithUniqueID(layerIndex, newAsset, "_T", true));

            success = true;

            if (asset is AssetTRIG trig)
            {
                trig.PositionX = Position.X;
                trig.PositionY = Position.Y;
                trig.PositionZ = Position.Z;

                trig.Position0X = Position.X;
                trig.Position0Y = Position.Y;
                trig.Position0Z = Position.Z;
            }
            if (asset is PlaceableAsset placeableAsset)
            {
                placeableAsset.PositionX = Position.X;
                placeableAsset.PositionY = Position.Y;
                placeableAsset.PositionZ = Position.Z;
            }
            else if (asset is AssetCAM cam)
            {
                cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                cam.SetNormalizedForward(Program.MainForm.renderer.Camera.GetForward());
                cam.SetNormalizedUp(Program.MainForm.renderer.Camera.GetUp());
            }
            else if (asset is AssetMRKR mrkr)
            {
                mrkr.PositionX = Position.X;
                mrkr.PositionY = Position.Y;
                mrkr.PositionZ = Position.Z;
            }
            else if (asset is AssetMVPT mvpt)
            {
                mvpt.PositionX = Position.X;
                mvpt.PositionY = Position.Y;
                mvpt.PositionZ = Position.Z;
            }
            else if (asset is AssetSFX sfx)
            {
                sfx.PositionX = Position.X;
                sfx.PositionY = Position.Y;
                sfx.PositionZ = Position.Z;
            }
            else if (asset is AssetDYNA dyna)
            {
                dyna.PositionX = Position.X;
                dyna.PositionY = Position.Y;
                dyna.PositionZ = Position.Z;
            }

            assetIDs.Add(asset.AHDR.assetID);

            return asset.AHDR.assetID;
        }
    }
}