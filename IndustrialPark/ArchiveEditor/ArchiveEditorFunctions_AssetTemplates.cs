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
                cam.SetNormalizedLeft(Program.MainForm.renderer.Camera.GetRight());
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

        public static bool persistentShinies = true;

        public uint PlaceTemplate(Vector3 position, int layerIndex, out bool success, ref List<uint> assetIDs, string customName = "", AssetTemplate template = AssetTemplate.Null)
        {
            AssetType newAssetType = AssetType.Null;

            if (template == AssetTemplate.Null)
                template = CurrentAssetTemplate;
            if (template == AssetTemplate.UserTemplate)
                return PlaceUserTemplate(position, layerIndex, out success, ref assetIDs);
            
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
                case AssetTemplate.BusStop_Camera:
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
                case AssetTemplate.BusStop_DYNA:
                case AssetTemplate.DuplicatotronSettings:
                case AssetTemplate.TeleportBox:
                case AssetTemplate.Checkpoint_Talkbox:
                case AssetTemplate.BungeeHook:
                case AssetTemplate.BungeeDrop:
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
                case AssetTemplate.Smelly_Sundae:
                case AssetTemplate.Golden_Underwear:
                case AssetTemplate.Artwork:
                case AssetTemplate.SteeringWheel:
                case AssetTemplate.PowerCrystal:
                    newAssetType = AssetType.PKUP;
                    break;
                case AssetTemplate.Pendulum_Generic:
                    newAssetType = AssetType.PEND;
                    break;
                case AssetTemplate.Platform_Generic:
                case AssetTemplate.TexasHitch_PLAT:
                case AssetTemplate.HoveringPlatform:
                case AssetTemplate.Springboard:
                    newAssetType = AssetType.PLAT;
                    break;
                case AssetTemplate.Player_Generic:
                    newAssetType = AssetType.PLYR;
                    break;
                case AssetTemplate.Portal:
                    newAssetType = AssetType.PORT;
                    break;
                case AssetTemplate.Script:
                    newAssetType = AssetType.SCRP;
                    break;
                case AssetTemplate.ShadowTable:
                    newAssetType = AssetType.SHDW;
                    break;
                case AssetTemplate.SIMP_Generic:
                case AssetTemplate.TaxiStand:
                case AssetTemplate.TexasHitch:
                case AssetTemplate.PressurePlateBase:
                case AssetTemplate.BusStop_BusSimp:
                case AssetTemplate.BusStop:
                case AssetTemplate.BusStop_Lights:
                case AssetTemplate.ThrowFruit:
                case AssetTemplate.ThrowFruitBase:
                case AssetTemplate.FreezyFruit:
                case AssetTemplate.Checkpoint_SIMP:
                case AssetTemplate.BungeeHook_SIMP:
                    newAssetType = AssetType.SIMP;
                    break;
                case AssetTemplate.SoundInfo:
                    customName = "sound_info";
                    newAssetType = AssetType.SNDI;
                    break;
                case AssetTemplate.Checkpoint:
                    customName = "CHECKPOINT_TRIG";
                    newAssetType = AssetType.TRIG;
                    break;
                case AssetTemplate.SphereTrigger:
                case AssetTemplate.BusStop_Trigger:
                case AssetTemplate.Checkpoint_Invisible:
                    newAssetType = AssetType.TRIG;
                    break;
                case AssetTemplate.Text:
                    newAssetType = AssetType.TEXT;
                    break;
                case AssetTemplate.Timer:
                case AssetTemplate.Checkpoint_Timer:
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
                case AssetTemplate.Monsoon:
                case AssetTemplate.Arf:
                case AssetTemplate.ArfDog:
                case AssetTemplate.Sleepytime:
                case AssetTemplate.Sleepytime_Moving:
                case AssetTemplate.BombBot:
                case AssetTemplate.Tubelet:
                case AssetTemplate.TubeletSlave:
                case AssetTemplate.BzztBot:
                case AssetTemplate.Slick:
                case AssetTemplate.Jellyfish_Pink:
                case AssetTemplate.Jellyfish_Blue:
                case AssetTemplate.Duplicatotron:
                case AssetTemplate.VIL_Generic:
                    newAssetType = AssetType.VIL;
                    break;
                case AssetTemplate.Chuck_Trigger:
                case AssetTemplate.Monsoon_Trigger:
                case AssetTemplate.Slick_Trigger:
                    customName = template.ToString().ToUpper().Replace("_TRIGGER", "");
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
                    ((AssetPKUP)asset).StateIsPersistent = persistentShinies;
                    ((AssetPKUP)asset).Shape = 0x3E;
                    ((AssetPKUP)asset).PickReferenceID = 0x7C8AC53E;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Yellow:
                    ((AssetPKUP)asset).StateIsPersistent = persistentShinies;
                    ((AssetPKUP)asset).Shape = 0x3B;
                    ((AssetPKUP)asset).PickReferenceID = 0xB3D6283B;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Green:
                    ((AssetPKUP)asset).StateIsPersistent = persistentShinies;
                    ((AssetPKUP)asset).Shape = 0x34;
                    ((AssetPKUP)asset).PickReferenceID = 0x079A0734;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Blue:
                    ((AssetPKUP)asset).StateIsPersistent = persistentShinies;
                    ((AssetPKUP)asset).Shape = 0x81;
                    ((AssetPKUP)asset).PickReferenceID = 0x6D4A4181;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Purple:
                    ((AssetPKUP)asset).StateIsPersistent = persistentShinies;
                    ((AssetPKUP)asset).Shape = 0xCB;
                    ((AssetPKUP)asset).PickReferenceID = 0xFA607BCB;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Underwear:
                    ((AssetPKUP)asset).Shape = 0x13;
                    ((AssetPKUP)asset).PickReferenceID = 0x28F55613;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Spatula:
                    ((AssetPKUP)asset).StateIsPersistent = true;
                    ((AssetPKUP)asset).Shape = 0xDD;
                    ((AssetPKUP)asset).PickReferenceID = 0x8BDFE8DD;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Sock:
                    ((AssetPKUP)asset).StateIsPersistent = true;
                    ((AssetPKUP)asset).Shape = 0x24;
                    ((AssetPKUP)asset).PickReferenceID = 0x74B46F24;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Spongeball:
                    ((AssetPKUP)asset).Shape = 0x15;
                    ((AssetPKUP)asset).PickReferenceID = 0xF09A1415;
                    ((AssetPKUP)asset).PickupFlags = 3;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Golden_Underwear:
                    ((AssetPKUP)asset).StateIsPersistent = true;
                    ((AssetPKUP)asset).Shape = 0x2E;
                    ((AssetPKUP)asset).PickReferenceID = 0xF650DA2E;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Artwork:
                    ((AssetPKUP)asset).StateIsPersistent = true;
                    ((AssetPKUP)asset).Shape = 0x10;
                    ((AssetPKUP)asset).PickReferenceID = 0x18140B10;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.SteeringWheel:
                    ((AssetPKUP)asset).StateIsPersistent = true;
                    ((AssetPKUP)asset).Shape = 0x32;
                    ((AssetPKUP)asset).PickReferenceID = 0x4C67C832;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.PowerCrystal:
                    ((AssetPKUP)asset).StateIsPersistent = true;
                    ((AssetPKUP)asset).Shape = 0xBB;
                    ((AssetPKUP)asset).PickReferenceID = 0xFE7A89BB;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Smelly_Sundae:
                    ((AssetPKUP)asset).Shape = 0x54;
                    ((AssetPKUP)asset).PickReferenceID = 0x6A779454;
                    ((AssetPKUP)asset).PickupFlags = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.WoodenTiki:
                    ((AssetVIL)asset).Model_AssetID = "tiki_wooden_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tiki_wooden_bind;
                    break;
                case AssetTemplate.FloatingTiki:
                    ((AssetVIL)asset).Model_AssetID = "tiki_lovey_dovey_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tiki_lovey_dovey_bind;
                    break;
                case AssetTemplate.ThunderTiki:
                    ((AssetVIL)asset).Model_AssetID = "tiki_thunder_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tiki_thunder_bind;
                    break;
                case AssetTemplate.ShhhTiki:
                    ((AssetVIL)asset).Model_AssetID = "tiki_shhhh_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tiki_shhhh_bind;
                    break;
                case AssetTemplate.StoneTiki:
                    ((AssetVIL)asset).Model_AssetID = "tiki_stone_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tiki_stone_bind;
                    break;
                case AssetTemplate.Fodder:
                    ((AssetVIL)asset).Model_AssetID = "robot_0a_fodder_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_0a_fodder_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Hammer:
                    ((AssetVIL)asset).Model_AssetID = "ham_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.ham_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.TarTar:
                    ((AssetVIL)asset).Model_AssetID = "robot_tar_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_tar_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.ChompBot:
                    ((AssetVIL)asset).Model_AssetID = "robot_0a_chomper_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_0a_chomper_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.GLove:
                    ((AssetVIL)asset).Model_AssetID = "g_love_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.g_love_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Chuck:
                    ((AssetVIL)asset).Model_AssetID = "robot_chuck_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Chuck_Trigger:
                    ((AssetVIL)asset).Model_AssetID = "robot_chuck_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };
                    AssetTRIG chuckTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.SphereTrigger));
                    chuckTrigger.Position1X_Radius = 15f;
                    chuckTrigger.LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.DetectPlayerOn
                        },
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ExitPlayer,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Monsoon:
                    ((AssetVIL)asset).Model_AssetID = "robot_4a_monsoon_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_4a_monsoon_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Monsoon_Trigger:
                    ((AssetVIL)asset).Model_AssetID = "robot_4a_monsoon_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_4a_monsoon_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG monsoonTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.SphereTrigger));
                    monsoonTrigger.Position1X_Radius = 15f;
                    monsoonTrigger.LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.DetectPlayerOn
                        },
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ExitPlayer,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Sleepytime_Moving:
                    ((AssetVIL)asset).Model_AssetID = "robot_sleepy-time_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_sleepytime_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Sleepytime:
                    ((AssetVIL)asset).Model_AssetID = "robot_sleepy-time_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_sleepytime_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetMVPT)GetFromAssetID(((AssetVIL)asset).MovePoint_AssetID)).ZoneRadius = -1;
                    break;
                case AssetTemplate.Arf:
                    ((AssetVIL)asset).Model_AssetID = "robot_arf_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_arf_bind;
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_DOGA", AssetTemplate.ArfDog),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        },
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_DOGB", AssetTemplate.ArfDog),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        },
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_DOGC", AssetTemplate.ArfDog),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        }
                    };
                    break;
                case AssetTemplate.ArfDog:
                    ((AssetVIL)asset).Model_AssetID = "robot_arf_dog_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_arf_dog_bind;
                    break;
                case AssetTemplate.BombBot:
                    ((AssetVIL)asset).Model_AssetID = "robot_0a_bomb_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_0a_bomb_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Tubelet:
                    ((AssetVIL)asset).Model_AssetID = "tubelet_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tubelet_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SLAVEA", AssetTemplate.TubeletSlave),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        },
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SLAVEB", AssetTemplate.TubeletSlave),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        }
                    };
                    break;
                case AssetTemplate.TubeletSlave:
                    ((AssetVIL)asset).Model_AssetID = "tubelet_slave_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tubelet_slave_bind;
                    break;
                case AssetTemplate.BzztBot:
                    ((AssetVIL)asset).Model_AssetID = "robot_0a_bzzt_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_0a_bzzt_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Slick:
                    ((AssetVIL)asset).Model_AssetID = "robot_9a_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Slick_Trigger:
                    ((AssetVIL)asset).Model_AssetID = "robot_9a_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG slickTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.SphereTrigger));
                    slickTrigger.Position1X_Radius = 15f;
                    slickTrigger.LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.DetectPlayerOn
                        },
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ExitPlayer,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Jellyfish_Pink:
                    ((AssetVIL)asset).Model_AssetID = "jellyfish_pink_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.jellyfish_pink_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Jellyfish_Blue:
                    ((AssetVIL)asset).Model_AssetID = "jellyfish_blue_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.jellyfish_blue_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.EnemyAreaMVPT);
                    break;
                case AssetTemplate.Duplicatotron:
                    ((AssetVIL)asset).Model_AssetID = "duplicatotron1000_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.duplicatotron1000_bind;
                    ((AssetVIL)asset).NPCSettings_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SETTINGS", AssetTemplate.DuplicatotronSettings);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_GROUP", AssetTemplate.Group),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        }
                    };
                    break;
                case AssetTemplate.DuplicatotronSettings:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__NPCSettings;
                    ((AssetDYNA)asset).DynaBase = new DynaNPCSettings()
                    {
                        AllowDetect = 1,
                        ReduceCollide = 1,
                        UseNavSplines = 1,
                        AllowAttack = 1,
                        AssumeLOS = 1,
                        AssumeFOV = 1,
                        DuploSpawnDelay = 1f,
                        DuploSpawnLifeMax = -1
                    };
                    break;
                case AssetTemplate.Button_Red:
                    ((AssetBUTN)asset).Model_AssetID = "button";
                    ((AssetBUTN)asset).PressedModel_AssetID = "button_grn";
                    ((AssetBUTN)asset).UnknownByte6C = 4;
                    ((AssetBUTN)asset).UnknownByte6F = 4;
                    ((AssetBUTN)asset).UnknownByte70 = 2;
                    ((AssetBUTN)asset).UnknownByte71 = 1;
                    ((AssetBUTN)asset).UnknownByte72 = 1;
                    ((AssetBUTN)asset).PressedOffset = -0.2f;
                    ((AssetBUTN)asset).TransitionTime = 0.5f;
                    ((AssetBUTN)asset).TransitionEaseOut = 0.2f;
                    ((AssetBUTN)asset).BubbleSpin = true;
                    ((AssetBUTN)asset).BubbleBowlOrBoulder = true;
                    ((AssetBUTN)asset).CruiseBubble = true;
                    ((AssetBUTN)asset).ThrowFruit = true;
                    ((AssetBUTN)asset).ThrowEnemyOrTiki = true;
                    ((AssetBUTN)asset).PatrickMelee = true;
                    ((AssetBUTN)asset).SandyMelee = true;
                    break;
                case AssetTemplate.PressurePlate:
                    ((AssetBUTN)asset).ActMethod = AssetBUTN.ButnActMethod.PressurePlate;
                    ((AssetBUTN)asset).Model_AssetID = "plate_pressure";
                    ((AssetBUTN)asset).PressedModel_AssetID = 0xCE7F8131;
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
                    ((AssetBUTN)asset).BubbleBowlOrBoulderPressurePlate = true;

                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_BASE", AssetTemplate.PressurePlateBase);
                    break;
                case AssetTemplate.PressurePlateBase:
                    ((AssetSIMP)asset).Model_AssetID = "plate_pressure_base";
                    break;
                case AssetTemplate.TaxiStand:
                    ((AssetSIMP)asset).Model_AssetID = "taxi_stand";
                    break;
                case AssetTemplate.TexasHitch:
                    ((AssetSIMP)asset).Model_AssetID = "trailer_hitch";
                    break;
                case AssetTemplate.TexasHitch_PLAT:
                    ((AssetPLAT)asset).Model_AssetID = "trailer_hitch";
                    ((AssetPLAT)asset).UnknownByte_90 = 4;
                    break;
                case AssetTemplate.EnemyAreaMVPT:
                    ((AssetMVPT)asset).PositionX = position.X;
                    ((AssetMVPT)asset).PositionY = position.Y;
                    ((AssetMVPT)asset).PositionZ = position.Z;
                    ((AssetMVPT)asset).Wt = 0x2710;
                    ((AssetMVPT)asset).IsZone = 0x01;
                    ((AssetMVPT)asset).BezIndex = 0x00;
                    ((AssetMVPT)asset).Delay = 360;
                    ((AssetMVPT)asset).ZoneRadius = 4;
                    ((AssetMVPT)asset).ArenaRadius = 8;
                    break;
                case AssetTemplate.PointMVPT:
                    ((AssetMVPT)asset).PositionX = position.X;
                    ((AssetMVPT)asset).PositionY = position.Y;
                    ((AssetMVPT)asset).PositionZ = position.Z;
                    ((AssetMVPT)asset).Wt = 0x2710;
                    ((AssetMVPT)asset).IsZone = 0x01;
                    ((AssetMVPT)asset).BezIndex = 0x00;
                    ((AssetMVPT)asset).Delay = 0;
                    ((AssetMVPT)asset).ZoneRadius = -1;
                    ((AssetMVPT)asset).ArenaRadius = -1;
                    break;
                case AssetTemplate.SphereTrigger:
                    ((AssetTRIG)asset).Position1X_Radius = 10f;
                    break;
                case AssetTemplate.BusStop:
                    ((AssetSIMP)asset).Model_AssetID = "bus_stop";
                    ((AssetSIMP)asset).ScaleX = 2f;
                    ((AssetSIMP)asset).ScaleY = 2f;
                    ((AssetSIMP)asset).ScaleZ = 2f;
                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.BusStop_Trigger);
                    position.Y += 0.1f;
                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, template: AssetTemplate.BusStop_DYNA);
                    break;
                case AssetTemplate.BusStop_Lights:
                    ((AssetSIMP)asset).Model_AssetID = "bus_stop_lights";
                    ((AssetSIMP)asset).ScaleX = 2f;
                    ((AssetSIMP)asset).ScaleY = 2f;
                    ((AssetSIMP)asset).ScaleZ = 2f;
                    ((AssetSIMP)asset).SolidityFlag = 0;
                    ((AssetSIMP)asset).VisibilityFlag = 0;
                    ((AssetSIMP)asset).CollType = 0;
                    break;
                case AssetTemplate.BusStop_Trigger:
                    ((AssetTRIG)asset).Position1X_Radius = 2.5f;
                    uint lightsAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper().Replace("TRIGGER", "LIGHTS").Replace("TRIG", "LIGHTS"), AssetTemplate.BusStop_Lights);
                    ((AssetTRIG)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = lightsAssetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.Visible
                        },
                        new LinkBFBB
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = lightsAssetID,
                            EventReceiveID = EventBFBB.ExitPlayer,
                            EventSendID = EventBFBB.Invisible
                        }
                    };
                    break;
                case AssetTemplate.BusStop_DYNA:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__BusStop;
                    ((AssetDYNA)asset).DynaBase = new DynaBusStop()
                    {
                        MRKR_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "MRKR"), AssetTemplate.Marker),
                        Player = 0,
                        CAM_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "CAM"), AssetTemplate.BusStop_Camera),
                        SIMP_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "SIMP"), AssetTemplate.BusStop_BusSimp),
                        Delay = 1.5f
                    };
                    break;
                case AssetTemplate.BusStop_Camera:
                    ((AssetCAM)asset).PositionX -= 7f;
                    ((AssetCAM)asset).PositionY += 2f;
                    ((AssetCAM)asset).NormalizedForwardX = 1f;
                    ((AssetCAM)asset).NormalizedForwardY = 0f;
                    ((AssetCAM)asset).NormalizedForwardZ = 0f;
                    ((AssetCAM)asset).NormalizedUpX = 0f;
                    ((AssetCAM)asset).NormalizedUpX = 1f;
                    ((AssetCAM)asset).NormalizedUpX = 0f;
                    ((AssetCAM)asset).NormalizedLeftX = 0f;
                    ((AssetCAM)asset).NormalizedLeftY = 0f;
                    ((AssetCAM)asset).NormalizedLeftZ = 1f;
                    ((AssetCAM)asset).OffsetStartFrames = 30;
                    ((AssetCAM)asset).OffsetEndFrames = 45;
                    ((AssetCAM)asset).FieldOfView = 60f;
                    ((AssetCAM)asset).TransitionTime = 0.5f;
                    ((AssetCAM)asset).UnknownFloat64 = -2f;
                    ((AssetCAM)asset).UnknownFloat68 = 1f;
                    ((AssetCAM)asset).UnknownFloat6C = 1f;
                    ((AssetCAM)asset).Flags1 = 00;
                    ((AssetCAM)asset).Flags2 = 01;
                    ((AssetCAM)asset).Flags3 = 01;
                    ((AssetCAM)asset).Flags4 = 0x8F;
                    break;
                case AssetTemplate.BusStop_BusSimp:
                    ((AssetSIMP)asset).PositionX -= 3f;
                    ((AssetSIMP)asset).SolidityFlag = 0;
                    ((AssetSIMP)asset).VisibilityFlag = 0;
                    ((AssetSIMP)asset).CollType = 0;
                    ((AssetSIMP)asset).Model_AssetID = "bus_bind";
                    ((AssetSIMP)asset).Animation_AssetID = "BUSSTOP_ANIMLIST_01";
                    break;
                case AssetTemplate.TeleportBox:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__Teleport;
                    ((AssetDYNA)asset).DynaBase = new DynaTeleport_BFBB(2)
                    {
                        MRKR_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MRKR", AssetTemplate.Marker)
                    };
                    break;
                case AssetTemplate.ThrowFruit:
                    ((AssetSIMP)asset).Model_AssetID = "fruit_throw.MINF";
                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "BASE", AssetTemplate.ThrowFruitBase);
                    break;
                case AssetTemplate.FreezyFruit:
                    ((AssetSIMP)asset).Model_AssetID = "fruit_freezy_bind.MINF";
                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "BASE", AssetTemplate.ThrowFruitBase);
                    break;
                case AssetTemplate.ThrowFruitBase:
                    ((AssetSIMP)asset).Model_AssetID = "fruit_throw_base";
                    ((AssetSIMP)asset).CollType = 0;
                    break;
                case AssetTemplate.Checkpoint:
                case AssetTemplate.Checkpoint_Invisible:
                    {
                        ((AssetTRIG)asset).Position1X_Radius = 6f;
                        AssetID checkpointDisp = "CHECKPOINT_DISP_00";
                        if (!ContainsAsset(checkpointDisp))
                            checkpointDisp = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "CHECKPOINT_DISP", AssetTemplate.Dispatcher);

                        List<LinkBFBB> events = new List<LinkBFBB>
                        {
                            new LinkBFBB
                            {
                                Arguments_Hex = new AssetID[] { 0, 0, 0, 0, PlaceTemplate(position, layerIndex, out success, ref assetIDs, "CHECKPOINT_MRKR", AssetTemplate.Marker), 0 },
                                TargetAssetID = checkpointDisp,
                                EventReceiveID = EventBFBB.EnterPlayer,
                                EventSendID = EventBFBB.SetCheckPoint
                            }
                        };

                        if (template == AssetTemplate.Checkpoint)
                            events.Add(new LinkBFBB
                            {
                                Arguments_Float = new float[4],
                                TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "CHECKPOINT_TIMER", AssetTemplate.Checkpoint_Timer),
                                EventReceiveID = EventBFBB.EnterPlayer,
                                EventSendID = EventBFBB.Run
                            });

                        ((AssetTRIG)asset).LinksBFBB = events.ToArray();
                        break;
                    }
                case AssetTemplate.Checkpoint_Timer:
                    {
                        ((AssetTIMR)asset).Time = 0.5f;
                        uint checkpointSimp = PlaceTemplate(new Vector3(position.X + 2f, position.Y, position.Z), layerIndex, out success, ref assetIDs, "CHECKPOINT_SIMP", AssetTemplate.Checkpoint_SIMP);
                        uint checkpointTalkbox = BKDRHash("CHECKPOINT_TALKBOX_00");
                        if (!ContainsAsset(checkpointTalkbox))
                            checkpointTalkbox = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "CHECKPOINT_TALKBOX", AssetTemplate.Checkpoint_Talkbox);
                        ((AssetTIMR)asset).LinksBFBB = new LinkBFBB[] {
                            new LinkBFBB
                            {
                                Arguments_Float = new float[] { 2, 0, 0, 0 },
                                TargetAssetID = checkpointSimp,
                                EventReceiveID = EventBFBB.Run,
                                EventSendID = EventBFBB.AnimPlayLoop
                            },
                            new LinkBFBB
                            {
                                Arguments_Hex = new AssetID[] { "checkpoint_text", 0, 0, 0 },
                                TargetAssetID = checkpointTalkbox,
                                EventReceiveID = EventBFBB.Run,
                                EventSendID = EventBFBB.StartConversation
                            },
                            new LinkBFBB
                            {
                                Arguments_Float = new float[] { 3, 0, 0, 0 },
                                TargetAssetID = checkpointSimp,
                                EventReceiveID = EventBFBB.Expired,
                                EventSendID = EventBFBB.AnimPlayLoop
                            },
                            new LinkBFBB
                            {
                                Arguments_Float = new float[4],
                                TargetAssetID = asset.AHDR.assetID,
                                EventReceiveID = EventBFBB.Expired,
                                EventSendID = EventBFBB.Disable
                            },
                        };
                        break;
                    }
                case AssetTemplate.Checkpoint_SIMP:
                    ((AssetSIMP)asset).ScaleX = 0.75f;
                    ((AssetSIMP)asset).ScaleY = 0.75f;
                    ((AssetSIMP)asset).ScaleZ = 0.75f;
                    ((AssetSIMP)asset).Model_AssetID = "checkpoint_bind";
                    ((AssetSIMP)asset).Animation_AssetID = "CHECKPOINT_ANIMLIST_01";
                    break;
                case AssetTemplate.Checkpoint_Talkbox:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 11;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__talk_box;
                    ((AssetDYNA)asset).DynaBase = new DynaTalkBox()
                    {
                        TextBoxID1 = 0x9BC49154,
                        Flags5 = 1,
                        UnknownFloat = 2f
                    };
                    break;
                case AssetTemplate.Springboard:
                    ((AssetPLAT)asset).Model_AssetID = 0x55E9EAB5;
                    ((AssetPLAT)asset).Animation_AssetID = 0x7AAA99BB;
                    ((AssetPLAT)asset).PlatformType = PlatType.Springboard;
                    ((AssetPLAT)asset).PlatformSubtype = PlatTypeSpecific.Springboard;
                    ((AssetPLAT)asset).CollisionType = 4;
                    ((AssetPLAT)asset).Float58 = 10;
                    ((AssetPLAT)asset).ANIM_AssetID_1 = 0x6DAE0759;
                    ((AssetPLAT)asset).ANIM_AssetID_2 = 0xBC4A9A5F;
                    ((AssetPLAT)asset).LaunchDirectionY = 1;
                    ((AssetPLAT)asset).UnknownByte_90 = 6;
                    break;
                case AssetTemplate.HoveringPlatform:
                    ((AssetPLAT)asset).Model_AssetID = 0x335EE0C8;
                    ((AssetPLAT)asset).Animation_AssetID = 0x730847B6;
                    ((AssetPLAT)asset).PlatformType = PlatType.Mechanism;
                    ((AssetPLAT)asset).PlatformSubtype = PlatTypeSpecific.Mechanism;
                    ((AssetPLAT)asset).CollisionType = 4;
                    ((AssetPLAT)asset).UnknownByte_90 = 6;
                    ((AssetPLAT)asset).MovementLoopType = 1;
                    ((AssetPLAT)asset).MovementTranslation_EaseEnd = 0.4f;
                    ((AssetPLAT)asset).MovementTranslation_EaseStart = 0.4f;
                    break;
                case AssetTemplate.BungeeHook:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 13;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__bungee_hook;
                    ((AssetDYNA)asset).DynaBase = new DynaBungeeHook()
                    {
                        Unknown_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "BUNGEE_SIMP", AssetTemplate.BungeeHook_SIMP),
                        UnknownFloat1 = 3,
                        UnknownFloat2 = 0.5f,
                        UnknownFloat3 = 10,
                        UnknownFloat4 = 1,
                        UnknownFloat5 = 2,
                        UnknownFloat6 = 25,
                        UnknownFloat7 = 0.95f,
                        UnknownFloat8 = 2,
                        UnknownFloat9 = 9.8f,
                        UnknownFloat10 = 2,
                        UnknownFloat11 = 2,
                        UnknownFloat12 = 40,
                        UnknownFloat13 = 0.05f,
                        UnknownFloat14 = 2,
                        UnknownFloat15 = 5,
                        UnknownFloat16 = 220,
                        UnknownFloat17 = 0.5f,
                        UnknownFloat18 = 180,
                        UnknownFloat19 = 0.05f,
                        UnknownFloat20 = 0,
                        UnknownFloat21 = 0.05f,
                        UnknownFloat22 = 0.2f,
                        UnknownFloat23 = 0.25f,
                        UnknownFloat24 = 0.2f,
                        UnknownFloat25 = 0.1f,
                        UnknownFloat26 = 0.6f,
                        UnknownFloat27 = 0.2f,
                    };
                    break;
                case AssetTemplate.BungeeHook_SIMP:
                    ((AssetSIMP)asset).Model_AssetID = "bungee_hook";
                    ((AssetSIMP)asset).CollType = 0;
                    break;
                case AssetTemplate.BungeeDrop:
                    ((AssetDYNA)asset).Flags = 0x1D;
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__bungee_drop;
                    ((AssetDYNA)asset).DynaBase = new DynaBungeeDrop()
                    {
                        MRKR_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "BUNGEE_MRKR", AssetTemplate.Marker),
                        Unknown = 1,
                    };
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
                cam.SetNormalizedLeft(Program.MainForm.renderer.Camera.GetRight());
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