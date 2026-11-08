using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.Archive;

public class AssetFactory
{
    private Game game;
    private Platform platform;
    private List<Asset> assets { get; set; }
    private bool ignoreNumber;
    private string assetName;
    private Func<uint, bool> exists;

    public static List<Asset> GenerateTemplate(Game game, Platform platform, Func<uint, bool> exists, AssetTemplate template, string assetName = null, Vector3? position = null, bool ignoreNumber = false)
    {
        var factory = new AssetFactory(game, platform, exists, template, assetName, ignoreNumber);
        factory.GenerateTemplate(position ?? Vector3.Zero, "", template);
        return factory.assets;
    }

    private AssetFactory(Game game, Platform platform, Func<uint, bool> exists, AssetTemplate template, string assetName, bool ignoreNumber)
    {
        assets = [];
        this.game = game;
        this.platform = platform;
        this.assetName = assetName;
        this.ignoreNumber = ignoreNumber;
        this.exists = exists;

        switch (template)
        {
            case AssetTemplate.DefaultGlowSceneProp:
            case AssetTemplate.Empty_BSP:
            case AssetTemplate.Environment:
            case AssetTemplate.LKIT_lights:
            case AssetTemplate.LKIT_JF_SB_lights:
            case AssetTemplate.LKIT_jf01_light_kit:
            case AssetTemplate.Model_Info:
            case AssetTemplate.Flythrough_Widget:
                this.ignoreNumber = true;
                break;
            case AssetTemplate.Start_Camera:
                this.assetName = startCamName(game);
                this.ignoreNumber = true;
                break;
            case AssetTemplate.Sound_Info:
                this.assetName = "sound_info";
                this.ignoreNumber = true;
                break;
            case AssetTemplate.Player:
                this.assetName = playerName(game);
                this.ignoreNumber = true;
                break;
            case AssetTemplate.Checkpoint_Set:
                this.assetName = "CHECKPOINT_TRIG";
                this.ignoreNumber = false;
                break;
        }

        if (string.IsNullOrEmpty(this.assetName))
            this.assetName = template.ToString().ToUpper() + "_01";

        if (assetName != null && exists(new AssetID(assetName)))
            this.ignoreNumber = false;

        if (!this.ignoreNumber)
            this.assetName = AssetService.GetUniqueAssetName(this.assetName, new AssetID(this.assetName), true, false, exists);
    }

    private Asset GenerateTemplate(Vector3 position, string assetName, AssetTemplate template, bool? ignoreNumber = null)
    {
        if (string.IsNullOrEmpty(assetName))
            assetName = this.assetName;
        else
            ignoreNumber ??= true;

        if (exists(new AssetID(assetName)))
        {
            ignoreNumber ??= this.ignoreNumber;
            this.assetName = AssetService.GetUniqueAssetName(this.assetName, new AssetID(this.assetName), !ignoreNumber.Value, ignoreNumber.Value, exists);
        }

        Asset asset = CreateFromTemplate(position, assetName, template);

        AssetFactoryService.AssignDefaultModelToTemplate(template, asset);

        if (asset == null)
            return null;

        asset.SetGame(game);
        assets.Add(asset);
        return asset;
    }

    private Asset CreateFromTemplate(Vector3 position, string assetName, AssetTemplate template)
    {
        switch (template)
        {
            case AssetTemplate.Boulder_Generator:
                return new DynaGObjectBoulderGen(assetName);
            case AssetTemplate.Counter:
                return new AssetCNTR(assetName);
            case AssetTemplate.Conditional:
                return new AssetCOND(assetName);
            case AssetTemplate.Dispatcher:
                return new AssetDPAT(assetName);
            case AssetTemplate.Fog:
                return new AssetFOG(assetName);
            case AssetTemplate.Group:
                return new AssetGRUP(assetName);
            case AssetTemplate.Portal:
                return new AssetPORT(assetName);
            case AssetTemplate.Progress_Script:
                return new AssetPGRS(assetName);
            case AssetTemplate.Script:
            case AssetTemplate.Scale_Block_Script:
                return new AssetSCRP(assetName);
            case AssetTemplate.Sound_Group:
                return new AssetSGRP(assetName);
            case AssetTemplate.Text:
                return new AssetTEXT(assetName);
            case AssetTemplate.Timer:
                return new AssetTIMR(assetName);
            case AssetTemplate.Checkpoint_Timer:
                {
                    var timer = new AssetTIMR(assetName)
                    {
                        Time = 0.5f
                    };
                    var checkpointSimp = GenerateTemplate(new Vector3(position.X + 2f, position.Y, position.Z), $"{assetName}_SIMP", AssetTemplate.Checkpoint_SIMP);
                    var checkpointTalkbox = new AssetID("CHECKPOINT_TALKBOX");
                    if (!exists(checkpointTalkbox))
                        checkpointTalkbox = GenerateTemplate(position, "CHECKPOINT_TALKBOX", AssetTemplate.Checkpoint_Talkbox).assetID;
                    timer.Links = new Link[] {
                        new Link(game)
                        {
                            FloatParameter1 = 2,
                            TargetAsset = checkpointSimp.assetID,
                            EventReceiveID = (ushort)EventBFBB.Run,
                            EventSendID = (ushort)EventBFBB.AnimPlay
                        },
                        new Link(game)
                        {
                            Parameter1 = "checkpoint_text",
                            TargetAsset = checkpointTalkbox,
                            EventReceiveID = (ushort)EventBFBB.Run,
                            EventSendID = (ushort)EventBFBB.StartConversation
                        },
                        new Link(game)
                        {
                            FloatParameter1 = 3,
                            TargetAsset = checkpointSimp.assetID,
                            EventReceiveID = (ushort)EventBFBB.Expired,
                            EventSendID = (ushort)EventBFBB.AnimPlayLoop
                        },
                        new Link(game)
                        {
                            TargetAsset = timer.assetID,
                            EventReceiveID = (ushort)EventBFBB.Expired,
                            EventSendID = (ushort)EventBFBB.Disable
                        },
                    };
                    return timer;
                }
            case AssetTemplate.Cam_Tweak:
                return new DynaGObjectCamTweak(assetName, (game >= Game.ROTU) ? 2 : 1);
            case AssetTemplate.Duplicatotron_Settings:
            case AssetTemplate.NPC_Settings_Object:
                return new DynaGObjectNPCSettings(assetName, template);
            case AssetTemplate.Disco_Floor:
                return new AssetDSCO(assetName);
            case AssetTemplate.Camera:
            case AssetTemplate.Start_Camera:
            case AssetTemplate.Bus_Stop_Camera:
                return new AssetCAM(assetName, position, template);
            case AssetTemplate.Marker:
                return new AssetMRKR(assetName, position);
            case AssetTemplate.Box_Trigger:
            case AssetTemplate.Sphere_Trigger:
            case AssetTemplate.Cylinder_Trigger:
                return new AssetTRIG(assetName, position, template);
            case AssetTemplate.Checkpoint_Set:
            case AssetTemplate.Checkpoint_Invisible:
                {
                    var trigger = new AssetTRIG(assetName, position, template);
                    AssetID checkpointDisp = "CHECKPOINT_DISP";
                    if (!exists(checkpointDisp))
                        checkpointDisp = GenerateTemplate(position, "CHECKPOINT_DISP", AssetTemplate.Dispatcher).assetID;

                    var links = new List<Link>
                {
                    new Link(game)
                    {
                        ArgumentAsset = game >= Game.Incredibles ?
                        GenerateTemplate(position, $"{assetName}_POINTER", AssetTemplate.Pointer).assetID :
                        GenerateTemplate(position, $"{assetName}_MARKER", AssetTemplate.Marker).assetID,
                        TargetAsset = checkpointDisp,
                        EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                        EventSendID = (ushort)EventBFBB.SetCheckPoint
                    }
                };

                    if (template == AssetTemplate.Checkpoint_Set && game == Game.BFBB)
                        links.Add(new Link(game)
                        {
                            TargetAsset = GenerateTemplate(position, $"{assetName}_TIMER", AssetTemplate.Checkpoint_Timer).assetID,
                            EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                            EventSendID = (ushort)EventBFBB.Run
                        });

                    if (template == AssetTemplate.Checkpoint_Set && game == Game.Incredibles)
                    {
                        links.Add(new Link(game)
                        {
                            TargetAsset = GenerateTemplate(position, $"{assetName}_SCRIPT", AssetTemplate.Checkpoint_Script).assetID,
                            EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                            EventSendID = (ushort)EventBFBB.Run
                        });
                        links.Add(new Link(game)
                        {
                            TargetAsset = trigger.assetID,
                            EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                            EventSendID = (ushort)EventBFBB.Disable
                        });
                    }

                    trigger.Links = links.ToArray();
                    return trigger;
                }
            case AssetTemplate.Bus_Stop_Trigger:
                {
                    var trigger = new AssetTRIG(assetName, position, template);
                    var lightsSimp = GenerateTemplate(position, assetName.Replace("TRIG", "LIGHTS"), AssetTemplate.Bus_Stop_Lights);

                    trigger.Links = new Link[] {
                        new Link(game)
                        {
                            TargetAsset = lightsSimp.assetID,
                            EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                            EventSendID = (ushort)EventBFBB.Visible
                        },
                        new Link(game)
                        {
                            TargetAsset = lightsSimp.assetID,
                            EventReceiveID = (ushort)EventBFBB.ExitPlayer,
                            EventSendID = (ushort)EventBFBB.Invisible
                        }
                    };
                    return trigger;
                }
            case AssetTemplate.MovePoint_Arena:
            case AssetTemplate.MovePoint:
                return new AssetMVPT(assetName, position, game, template);
            case AssetTemplate.Pointer:
                return new DynaPointer(assetName, position);
            case AssetTemplate.Player:
                return new AssetPLYR(assetName, position, game);
            case AssetTemplate.Boulder:
                return new AssetBOUL(assetName, position);
            case AssetTemplate.Button:
            case AssetTemplate.Button_Red:
            case AssetTemplate.Button_Red_WithBase:
            case AssetTemplate.Pressure_Plate:
            case AssetTemplate.Red_Button:
            case AssetTemplate.Red_Button_Smash:
            case AssetTemplate.Floor_Button:
            case AssetTemplate.Floor_Button_Smash:
            {
                var button = new AssetBUTN(assetName, position, template);
                if (template == AssetTemplate.Pressure_Plate)
                    GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Pressure_Plate_Base);
                else if (template == AssetTemplate.Button_Red_WithBase)
                    GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Button_Red_Base);
                else if (template == AssetTemplate.Red_Button)
                    GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Red_Button_Base);
                else if (template == AssetTemplate.Red_Button_Smash)
                    GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Red_Button_Smash_Base);
                else if (template == AssetTemplate.Floor_Button)
                    GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Floor_Button_Base);
                else if (template == AssetTemplate.Floor_Button_Smash)
                    GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Floor_Button_Smash_Base);
                return button;
            }
            case AssetTemplate.Destructible_Object:
            case AssetTemplate.Crate:
                return new AssetDSTR(assetName, position, template);
            case AssetTemplate.Electric_Arc_Generator:
                return new AssetEGEN(assetName, position);
            case AssetTemplate.Hangable:
                return new AssetHANG(assetName, position);
            case AssetTemplate.Villain:
            case AssetTemplate.Caveman:
            case AssetTemplate.Creeper:
            case AssetTemplate.Gargoyle:
            case AssetTemplate.Geronimo:
            case AssetTemplate.Ghost:
            case AssetTemplate.Ghost_of_Captain_Moody:
            case AssetTemplate.Ghost_Diver:
            case AssetTemplate.Funland_Robot:
            case AssetTemplate.Headless_Specter:
            case AssetTemplate.Sea_Creature:
            case AssetTemplate.Scarecrow:
            case AssetTemplate.Space_Kook:
            case AssetTemplate.Tar_Monster:
            case AssetTemplate.Witch:
            case AssetTemplate.Witch_Doctor:
            case AssetTemplate.Wolfman:
            case AssetTemplate.Zombie:
            case AssetTemplate.Bat:
            case AssetTemplate.Crab:
            case AssetTemplate.Flying_Fish:
            case AssetTemplate.Rat:
            case AssetTemplate.Spider:
            case AssetTemplate.Killer_Plant:
            case AssetTemplate.Holly:
            case AssetTemplate.Groundskeeper:
                return new AssetNPC(assetName, position, template);
            case AssetTemplate.Pendulum:
                return new AssetPEND(assetName, position);
            case AssetTemplate.Platform:
            case AssetTemplate.Texas_Hitch_Platform:
            case AssetTemplate.Swinger_Platform:
            case AssetTemplate.Springboard:
            case AssetTemplate.Hovering_Platform:
            case AssetTemplate.CollapsePlatform_Planktopolis:
            case AssetTemplate.CollapsePlatform_ThugTug:
            case AssetTemplate.CollapsePlatform_Spongeball:
            case AssetTemplate.Flower_Dig:
                return new AssetPLAT(assetName, position, template);
            case AssetTemplate.Simple_Object:
            case AssetTemplate.Taxi_Stand:
            case AssetTemplate.Texas_Hitch:
            case AssetTemplate.Swinger:
            case AssetTemplate.Bus_Stop:
            case AssetTemplate.Throw_Fruit:
            case AssetTemplate.Freezy_Fruit:
            case AssetTemplate.Pressure_Plate_Base:
            case AssetTemplate.Throw_Fruit_Base:
            case AssetTemplate.Bus_Stop_BusSimp:
            case AssetTemplate.Bus_Stop_Lights:
            case AssetTemplate.Checkpoint_SIMP:
            case AssetTemplate.Checkpoint_SIMP_TSSM:
            case AssetTemplate.Bungee_Hook_SIMP:
            case AssetTemplate.Red_Button_Base:
            case AssetTemplate.Red_Button_Smash_Base:
            case AssetTemplate.Floor_Button_Base:
            case AssetTemplate.Floor_Button_Smash_Base:
            case AssetTemplate.Cauldron:
            case AssetTemplate.Flower:
            case AssetTemplate.Block_Spikes:
            case AssetTemplate.Button_Red_Base:
            {
                var simp = new AssetSIMP(assetName, position, template);
                switch (template)
                {
                    case AssetTemplate.Bus_Stop:
                        GenerateTemplate(position, assetName + "_TRIG", AssetTemplate.Bus_Stop_Trigger);
                        position.Y += 0.1f;
                        GenerateTemplate(position, assetName + "_ASSET", AssetTemplate.Bus_Stop_DYNA);
                        break;
                    case AssetTemplate.Throw_Fruit:
                        GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Throw_Fruit_Base);
                        break;
                    case AssetTemplate.Freezy_Fruit:
                        GenerateTemplate(position, assetName + "_BASE", AssetTemplate.Throw_Fruit_Base);
                        break;
                    case AssetTemplate.Cauldron:
                        var sfx = (AssetSFX)GenerateTemplate(position, assetName + "_SFX", AssetTemplate.Cauldron_Sfx);
                        sfx.Attach = simp.assetID;
                        var lite = (AssetLITE)GenerateTemplate(position, assetName + "_LIGHT", AssetTemplate.Cauldron_Light);
                        lite.Attach = simp.assetID;
                        position.Y += 1f;
                        GenerateTemplate(position, assetName + "_EMITTER", AssetTemplate.Cauldron_Emitter);
                        break;
                    case AssetTemplate.Flower:
                        position.X += 4f;
                        var dig = GenerateTemplate(position, assetName + "_DIG", AssetTemplate.Flower_Dig);
                        simp.Links = new Link[]
                        {
                            new Link(game)
                            {
                                EventReceiveID = (ushort)EventScooby.Digup,
                                EventSendID = (ushort)EventScooby.CollisionVisibleOn,
                                TargetAsset = dig.assetID,
                            }
                        };
                        break;
                }
                return simp;
            }
            case AssetTemplate.NPC:
                return new AssetVIL(assetName, position, template, 0);
            case AssetTemplate.User_Interface:
                return new AssetUI(assetName, position);
            case AssetTemplate.User_Interface_Font:
                return new AssetUIFT(assetName, position);
            case AssetTemplate.SFX_OnEvent:
            case AssetTemplate.SFX_OnRadius:
            case AssetTemplate.Cauldron_Sfx:
                return new AssetSFX(assetName, position, game, template);
            case AssetTemplate.SDFX:
            case AssetTemplate.Scale_Block_Sdfx:
                return new AssetSDFX(assetName, position);
            case AssetTemplate.Light:
            case AssetTemplate.Cauldron_Light:
                return new AssetLITE(assetName, position, template);
            case AssetTemplate.Animation_List:
                return new AssetALST(assetName);
            case AssetTemplate.Camera_Curve:
                return new AssetCCRV(assetName);
            case AssetTemplate.Collision_Table:
                return new AssetCOLL(assetName);
            case AssetTemplate.Credits:
                return new AssetCRDT(assetName, game);
            case AssetTemplate.Environment:
                return new AssetENV(assetName, startCamName(game));
            case AssetTemplate.Flythrough:
            {
                var fly = new AssetFLY(assetName);
                var flyWidget = (DynaGObjectFlythrough)GenerateTemplate(position, assetName + "_WIDGET", AssetTemplate.Flythrough_Widget);
                flyWidget.Flythrough = fly.assetID;
                return fly;
            }
            case AssetTemplate.Flythrough_Widget:
                return new DynaGObjectFlythrough(assetName);
            case AssetTemplate.Jaw_Data_Table:
                return new AssetJAW(assetName);
            case AssetTemplate.Level_Of_Detail_Table:
                return new AssetLODT(assetName);
            case AssetTemplate.Surface:
                return new AssetSURF(assetName, game);
            case AssetTemplate.Surface_Mapper:
                return new AssetMAPR(assetName);
            case AssetTemplate.Throwable_Table:
                return new AssetTRWT(assetName);
            case AssetTemplate.Model_Info:
                return new AssetMINF(assetName);
            case AssetTemplate.One_Liner:
                return new AssetONEL(assetName);
            case AssetTemplate.Pipe_Info_Table:
                return new AssetPIPT(assetName);
            case AssetTemplate.Shadow_Table:
                return new AssetSHDW(assetName);
            case AssetTemplate.Shrapnel:
                return new AssetSHRP(assetName);
            case AssetTemplate.Sound_Info:
            {
                if (platform == Platform.Xbox)
                    return new AssetSNDI_XBOX(assetName);
                if (platform == Platform.PS2)
                    return new AssetSNDI_PS2(assetName);
                if (game >= Game.Incredibles)
                    return new AssetSNDI_GCN_V2(assetName);
                return new AssetSNDI_GCN_V1(assetName);
            }
            case AssetTemplate.Empty_BSP:
                return new AssetJSP(assetName, AssetType.BSP, GenerateBlankBSP());
            case AssetTemplate.Shiny_Red:
            case AssetTemplate.Shiny_Yellow:
            case AssetTemplate.Shiny_Green:
            case AssetTemplate.Shiny_Blue:
            case AssetTemplate.Shiny_Purple:
            case AssetTemplate.Underwear:
            case AssetTemplate.Spatula:
            case AssetTemplate.Sock:
            case AssetTemplate.Spongeball:
            case AssetTemplate.Golden_Underwear:
            case AssetTemplate.Artwork:
            case AssetTemplate.Steering_Wheel:
            case AssetTemplate.Power_Crystal:
            case AssetTemplate.Smelly_Sundae:
            case AssetTemplate.Manliness_Red:
            case AssetTemplate.Manliness_Yellow:
            case AssetTemplate.Manliness_Green:
            case AssetTemplate.Manliness_Blue:
            case AssetTemplate.Manliness_Purple:
            case AssetTemplate.Krabby_Patty:
            case AssetTemplate.Goofy_Goober_Token:
            case AssetTemplate.Treasure_Chest:
            case AssetTemplate.Nitro:
            case AssetTemplate.Scooby_Snack:
            case AssetTemplate.Snack_Box:
            case AssetTemplate.Save_Point:
            case AssetTemplate.Warp_Gate:
            case AssetTemplate.Snack_Gate:
            case AssetTemplate.Clue:
            case AssetTemplate.Key:
            case AssetTemplate.Gum:
            case AssetTemplate.Gum_Pack:
            case AssetTemplate.Soap:
            case AssetTemplate.Soap_Pack:
            case AssetTemplate.Turkey:
            case AssetTemplate.Cake:
            case AssetTemplate.Hamburger:
            case AssetTemplate.Ice_Cream:
            case AssetTemplate.Sandwich:
            case AssetTemplate.Shovel:
            case AssetTemplate.Springs:
            case AssetTemplate.Slippers:
            case AssetTemplate.Lampshade:
            case AssetTemplate.Helmet:
            case AssetTemplate.Knight_Helmet:
            case AssetTemplate.Boots:
            case AssetTemplate.Super_Smash:
            case AssetTemplate.Plungers:
            case AssetTemplate.Super_Sonic_Smash:
            case AssetTemplate.Umbrella:
            case AssetTemplate.Gum_Machine:
            case AssetTemplate.Soap_Bubble:
                return new AssetPKUP(assetName, game, position, template);
            case AssetTemplate.Wooden_Tiki:
            case AssetTemplate.Floating_Tiki:
            case AssetTemplate.Thunder_Tiki:
            case AssetTemplate.Shhh_Tiki:
            case AssetTemplate.Stone_Tiki:
            case AssetTemplate.ArfDog:
            case AssetTemplate.TubeletSlave:
            case AssetTemplate.Duplicatotron:
            {
                var vil = new AssetVIL(assetName, position, template, 0);
                if (template == AssetTemplate.Duplicatotron)
                {
                    vil.NPCSettingsObject = GenerateTemplate(position, assetName + "_SETTINGS", AssetTemplate.Duplicatotron_Settings).assetID;
                    vil.Links = new Link[] {
                            new Link(game)
                            {
                                TargetAsset = GenerateTemplate(position, assetName + "_GROUP", AssetTemplate.Group).assetID,
                                EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                                EventSendID = (ushort)EventBFBB.Connect_IOwnYou
                            }
                        };
                }
                return vil;
            }
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
            case AssetTemplate.Arf:
            case AssetTemplate.BombBot:
            case AssetTemplate.BzztBot:
            case AssetTemplate.Tubelet:
            case AssetTemplate.Slick:
            case AssetTemplate.Slick_Trigger:
            case AssetTemplate.Jellyfish_Pink:
            case AssetTemplate.Jellyfish_Blue:
            {
                var movePoint = (AssetMVPT)GenerateTemplate(position, assetName + "_MP", AssetTemplate.MovePoint_Arena);
                var vil = new AssetVIL(assetName, position, template, movePoint.assetID);
                if (template == AssetTemplate.Chuck_Trigger || template == AssetTemplate.Monsoon_Trigger || template == AssetTemplate.Slick_Trigger)
                {
                    vil.Links = new Link[] {
                        new Link(game)
                        {
                            TargetAsset = vil.assetID,
                            EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                            EventSendID = (ushort)EventBFBB.DetectPlayerOff
                        }
                    };

                    var trigger = (AssetTRIG)GenerateTemplate(position, assetName + "_TRIG", AssetTemplate.Sphere_Trigger);
                    trigger.Radius = 15f;
                    trigger.Links = new Link[] {
                        new Link(game)
                        {
                            TargetAsset = vil.assetID,
                            EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                            EventSendID = (ushort)EventBFBB.DetectPlayerOn
                        },
                        new Link(game)
                        {
                            TargetAsset = vil.assetID,
                            EventReceiveID = (ushort)EventBFBB.ExitPlayer,
                            EventSendID = (ushort)EventBFBB.DetectPlayerOff
                        }
                    };
                }
                else if (template == AssetTemplate.Sleepytime)
                {
                    movePoint.ZoneRadius = -1;
                }
                else if (template == AssetTemplate.Arf)
                {
                    var links = new List<Link>();
                    foreach (string i in new string[] { "A", "B", "C" })
                    {
                        var dog = GenerateTemplate(position, assetName + "_DOG_" + i, AssetTemplate.ArfDog);
                        links.Add(new Link(game)
                        {
                            TargetAsset = dog.assetID,
                            EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                            EventSendID = (ushort)EventBFBB.Connect_IOwnYou
                        });
                    }
                    vil.Links = links.ToArray();
                }
                else if (template == AssetTemplate.Tubelet)
                {
                    var links = new List<Link>();
                    foreach (string i in new string[] { "A", "B" })
                    {
                        var slave = GenerateTemplate(position, assetName + "_SLAVE_" + i, AssetTemplate.TubeletSlave);
                        links.Add(new Link(game)
                        {
                            TargetAsset = slave.assetID,
                            EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                            EventSendID = (ushort)EventBFBB.Connect_IOwnYou
                        });
                    }
                    vil.Links = links.ToArray();
                }
                return vil;
            }
            case AssetTemplate.Wood_Crate:
            case AssetTemplate.Hover_Crate:
            case AssetTemplate.Explode_Crate:
            case AssetTemplate.Shrink_Crate:
            case AssetTemplate.Steel_Crate:
                return new DynaEnemySupplyCrate(assetName, template, position);
            case AssetTemplate.Jellyfish:
            case AssetTemplate.Jellyfish_Bucket:
            {
                var mvpt = GenerateTemplate(position, assetName + "_MP", AssetTemplate.MovePoint);
                return new DynaEnemyCritter(assetName, template, position, mvpt.assetID);
            }
            case AssetTemplate.Fogger_GoofyGoober:
            case AssetTemplate.Fogger_Desert:
            case AssetTemplate.Fogger_ThugTug:
            case AssetTemplate.Fogger_Trench:
            case AssetTemplate.Fogger_Junkyard:
            case AssetTemplate.Fogger_Planktopolis:
            case AssetTemplate.Fogger_v1:
            case AssetTemplate.Fogger_v2:
            case AssetTemplate.Fogger_v3:
            case AssetTemplate.Slammer_GoofyGoober:
            case AssetTemplate.Slammer_Desert:
            case AssetTemplate.Slammer_ThugTug:
            case AssetTemplate.Spinner_ThugTug:
            case AssetTemplate.Spinner_Junkyard:
            case AssetTemplate.Spinner_Planktopolis:
            case AssetTemplate.Minimerv:
            case AssetTemplate.Mervyn:
            case AssetTemplate.Flinger_Desert:
            case AssetTemplate.Flinger_Trench:
            case AssetTemplate.Flinger_Junkyard:
            case AssetTemplate.Popper_Trench:
            case AssetTemplate.Popper_Planktopolis:
            {
                var mvpt = GenerateTemplate(position, assetName + "_MP",
                    (template.ToString().Contains("Flinger") || template.ToString().Contains("Popper")) ?
                    AssetTemplate.MovePoint : AssetTemplate.MovePoint_Arena, true);
                return new DynaEnemyStandard(assetName, template, position, mvpt.assetID);
            }
            case AssetTemplate.Turret_v1:
            case AssetTemplate.Turret_v2:
            case AssetTemplate.Turret_v3:
                return new DynaEnemyTurret(assetName, template, position);
            case AssetTemplate.Spawner_BB:
            case AssetTemplate.Spawner_DE:
            case AssetTemplate.Spawner_GG:
            case AssetTemplate.Spawner_JK:
            case AssetTemplate.Spawner_TR:
            case AssetTemplate.Spawner_PT:
            {
                var group = GenerateTemplate(position, assetName + "_GROUP", AssetTemplate.Group);
                return new DynaEnemyBucketOTron(assetName, template, position, group.assetID);
            }
            case AssetTemplate.Teleport_Box:
            {
                var mrkr = GenerateTemplate(position, assetName + "_MARKER", AssetTemplate.Marker);
                return new DynaGObjectTeleport(assetName, mrkr.assetID);
            }
            case AssetTemplate.Bungee_Hook:
            {
                var simp = GenerateTemplate(position, assetName.Replace("HOOK", "SIMP"), AssetTemplate.Bungee_Hook_SIMP);
                return new DynaGObjectBungeeHook(assetName, simp.assetID);
            }
            case AssetTemplate.Bungee_Drop:
            {
                var mrkr = GenerateTemplate(position, "BUNGEE_MARKER", AssetTemplate.Marker);
                return new DynaGObjectBungeeDrop(assetName, mrkr.assetID);
            }
            case AssetTemplate.Bus_Stop_DYNA:
            {
                var mrkr = GenerateTemplate(position, assetName.Replace("ASSET", "MARKER"), AssetTemplate.Marker);
                var cam = GenerateTemplate(position, assetName.Replace("ASSET", "CAM"), AssetTemplate.Bus_Stop_Camera);
                var simp = GenerateTemplate(position, assetName.Replace("ASSET", "SIMP"), AssetTemplate.Bus_Stop_BusSimp);

                return new DynaGObjectBusStop(assetName, mrkr.assetID, cam.assetID, simp.assetID);
            }
            case AssetTemplate.Checkpoint_Talkbox:
                return new DynaGObjectTalkBox(assetName, true);
            case AssetTemplate.Checkpoint_Script:
            {
                var scrp = new AssetSCRP(assetName);

                var checkpointSdfx = (AssetSDFX)GenerateTemplate(new Vector3(position.X + 2f, position.Y, position.Z), "CHECKPOINT_SFX", AssetTemplate.SDFX, false);
                var checkpointSimp = GenerateTemplate(new Vector3(position.X + 2f, position.Y, position.Z), "CHECKPOINT_SIMP", AssetTemplate.Checkpoint_SIMP_TSSM, false);

                scrp.TimedLinks = new Link[] {
                        new Link(game)
                        {
                            TargetAsset = checkpointSdfx.assetID,
                            EventSendID = (ushort)EventTSSM.Play
                        },
                        new Link(game)
                        {
                            FloatParameter4 = 10f,
                            TargetAsset = checkpointSimp.assetID,
                            EventSendID = (ushort)EventTSSM.LaunchFireWorks
                        },
                        new Link(game)
                        {
                            FloatParameter1 = 2f,
                            TargetAsset = checkpointSimp.assetID,
                            EventSendID = (ushort)EventTSSM.AnimPlay
                        },
                        new Link(game)
                        {
                            Time = 0.5f,
                            FloatParameter1 = 3f,
                            TargetAsset = checkpointSimp.assetID,
                            EventSendID = (ushort)EventTSSM.AnimPlayLoop
                        },
                    };

                checkpointSdfx.SoundGroup = "CHECKPOINT_SGRP";
                checkpointSdfx.Emitter = checkpointSimp.assetID;
                return scrp;
            }
            case AssetTemplate.Ring:
                return new DynaGObjectRing(assetName, position);
            case AssetTemplate.Ring_Control:
                return new DynaGObjectRingControl(assetName);
            case AssetTemplate.DefaultGlowSceneProp:
                return new DynaSceneProperties(assetName);
            case AssetTemplate.LKIT_lights:
            case AssetTemplate.LKIT_JF_SB_lights:
            case AssetTemplate.LKIT_jf01_light_kit:
            {
                var fileName =
                    template == AssetTemplate.LKIT_lights ? "lights" :
                    template == AssetTemplate.LKIT_JF_SB_lights ? "JF_SB_lights" :
                    template == AssetTemplate.LKIT_jf01_light_kit ? "jf01_light_kit" : "";
                var data = File.ReadAllBytes(Path.Combine(Application.StartupPath, "Resources", fileName));
                return new AssetLKIT(assetName, data, Endianness.Big);
            }
            case AssetTemplate.Health_10:
            case AssetTemplate.Health_25:
            case AssetTemplate.Health_50:
            case AssetTemplate.Power_25:
            case AssetTemplate.Power_50:
            case AssetTemplate.Bonus:
                return new DynaGObjectInPickup(assetName, position, template);
            case AssetTemplate.Cauldron_Emitter:
                return new AssetPARE(assetName, position, template);
            case AssetTemplate.Destructible:
                return new AssetDEST(assetName);
            case AssetTemplate.Gust:
                return new AssetGUST(assetName);
            case AssetTemplate.Volume_Box:
            case AssetTemplate.Volume_Sphere:
                return new AssetVOLU(assetName, position, template);
            case AssetTemplate.Duplicator:
                return new AssetDUPC(assetName, position);
            case AssetTemplate.Zip_Line:
                return new AssetZLIN(assetName, position);
            case AssetTemplate.Rubble_Generator:
                return new DynaGObjectRubbleGenerator(assetName, (game >= Game.ROTU) ? 8 : 5);
            case AssetTemplate.Floating_Block:
            case AssetTemplate.Floating_Block_Spiked:
            case AssetTemplate.Scale_Block:
            case AssetTemplate.Scale_Block_Driven:
            case AssetTemplate.Scale_Block_Spiked_Driven:
            case AssetTemplate.Ice_Block:
            case AssetTemplate.Ice_Block_Spiked:
            case AssetTemplate.Trampoline_Block:
            case AssetTemplate.Trampoline_Block_Driven:
            case AssetTemplate.Trampoline_Block_Spiked_Driven:
            case AssetTemplate.Wooden_Platform:
            case AssetTemplate.Block_Driver:
                var block = new AssetPLAT(assetName, position, template);

                if (new AssetTemplate[] {
                    AssetTemplate.Floating_Block_Spiked,
                    AssetTemplate.Ice_Block_Spiked,
                    AssetTemplate.Scale_Block_Spiked_Driven,
                    AssetTemplate.Trampoline_Block_Spiked_Driven
                }.Contains(template))
                {
                    var spikes = (AssetSIMP)GenerateTemplate(position, block.assetName + "_SPIKES", AssetTemplate.Block_Spikes);
                    spikes.Links = new Link[]
                    {
                        new Link(game)
                        {
                            EventReceiveID = (ushort)EventTSSM.ScenePrepare,
                            EventSendID = (ushort)EventTSSM.Drivenby,
                            TargetAsset = block.assetID,
                            FloatParameter1 = 1f
                        }
                    };
                }

                if (new AssetTemplate[] {
                    AssetTemplate.Scale_Block_Driven,
                    AssetTemplate.Scale_Block_Spiked_Driven,
                    AssetTemplate.Trampoline_Block_Driven,
                    AssetTemplate.Trampoline_Block_Spiked_Driven
                }.Contains(template))
                {
                    var driver = (AssetPLAT)GenerateTemplate(position, block.assetName + "_DRIVER", AssetTemplate.Block_Driver);
                    block.Links = new Link[]
                    {
                        new Link(game)
                        {
                            EventReceiveID = (ushort)EventTSSM.ScenePrepare,
                            EventSendID = (ushort)EventTSSM.Drivenby,
                            TargetAsset = driver.assetID,
                            FloatParameter1 = 1f
                        }
                    };
                }

                if (new AssetTemplate[] {
                    AssetTemplate.Scale_Block,
                    AssetTemplate.Scale_Block_Driven,
                    AssetTemplate.Scale_Block_Spiked_Driven
                }.Contains(template))
                {
                    var sdfx = (AssetSDFX)GenerateTemplate(position, block.assetName + "_SDFX", AssetTemplate.Scale_Block_Sdfx);
                    sdfx.SoundGroup = "SHRINK_SGRP";
                    sdfx.Emitter = block.assetID;
                    sdfx.SDFXFlags.FlagValueInt = 4;

                    var script = (AssetSCRP)GenerateTemplate(position, block.assetName + "_SCRIPT", AssetTemplate.Scale_Block_Script);
                    script.TimedLinks = new Link[]
                    {
                        new Link(game)
                        {
                            Time = 0,
                            EventSendID = (ushort)EventTSSM.Run,
                            TargetAsset = block.assetID
                        },
                        new Link(game)
                        {
                            Time = 1,
                            EventSendID = (ushort)EventTSSM.CollisionVisibleOff,
                            TargetAsset = block.assetID
                        },
                        new Link(game)
                        {
                            Time = 1,
                            EventSendID = (ushort)EventTSSM.Play,
                            TargetAsset = sdfx.assetID
                        },
                    };

                    var links = block.Links.ToList();
                    links.Add(new Link(game)
                    {
                        EventReceiveID = (ushort)EventTSSM.Mount,
                        EventSendID = (ushort)EventTSSM.Run,
                        TargetAsset = script.assetID,
                    });
                    block.Links = links.ToArray();
                }
                return block;
            case AssetTemplate.Jsp_Info:
                return new AssetJSP_INFO(assetName, game, platform);

            case AssetTemplate.Swarm_Bug:
                return new DynaEnemyRATSSwarmBug(assetName, position);
            case AssetTemplate.Swarm_Owl:
                return new DynaEnemyRATSSwarmOwl(assetName, position);
            case AssetTemplate.Thief:
            {
                var mvpt = GenerateTemplate(position, assetName + "_MP", AssetTemplate.MovePoint, true);
                return new DynaEnemyRATSThief(assetName, position, mvpt.assetID);
            }
            case AssetTemplate.Waiter:
            {
                var mvpt = GenerateTemplate(position, assetName + "_MP", AssetTemplate.MovePoint, true);
                return new DynaEnemyRATSWaiter(assetName, position, mvpt.assetID);
            }
            case AssetTemplate.Carryable_Property_Attract:
                return new DynaCarryablePropertyAttract(assetName);
            case AssetTemplate.Carryable_Property_Generic:
                return new DynaCarryablePropertyGeneric(assetName);
            case AssetTemplate.Carryable_Property_Repel:
                return new DynaCarryablePropertyRepel(assetName);
            case AssetTemplate.Carryable_Property_Swipe:
                return new DynaCarryablePropertySwipe(assetName);
            case AssetTemplate.Checkpoint:
                return new DynaCheckpoint(assetName);
            case AssetTemplate.Light_Effect:
                return new DynaEffectLight(assetName, position);
            case AssetTemplate.Light_Effect_Flicker:
            case AssetTemplate.Light_Effect_Strobe:
                var light = (DynaEffectLight)GenerateTemplate(position, null, AssetTemplate.Light_Effect);
                if (template == AssetTemplate.Light_Effect_Flicker)
                {
                    var flicker = new DynaEffectLightFlicker(assetName);
                    light.LightEffectID = flicker.assetID;
                    return flicker;
                }
                else if (template == AssetTemplate.Light_Effect_Strobe)
                {
                    var strobe = new DynaEffectLightStrobe(assetName);
                    light.LightEffectID = strobe.assetID;
                    return strobe;
                }
                return light;
            case AssetTemplate.Hud_Compass_System:
                return new DynaHudCompassSystem(assetName);
            case AssetTemplate.Hud_Compass_Object:
                return new DynaHudCompassObject(assetName, position);
            case AssetTemplate.Pole_Swing:
                return new DynaCObjectPoleSwing(assetName);
            case AssetTemplate.CObject_SpringBoard:
                return new DynaCObjectSpringBoard(assetName, position);
            case AssetTemplate.Tightrope:
                return new DynaCObjectTightrope(assetName);

            case AssetTemplate.Bomber:
                return new DynaEnemyIN2Bomber(assetName, position);
            case AssetTemplate.Boss_Underminer_Drill:
                return new DynaEnemyIN2BossUnderminerDrill(assetName, position);
            case AssetTemplate.Boss_Underminer_UM:
                return new DynaEnemyIN2BossUnderminerUM(assetName, position);
            case AssetTemplate.Chicken:
                return new DynaEnemyIN2Chicken(assetName, position);
            case AssetTemplate.Driller:
                return new DynaEnemyIN2Driller(assetName, position);
            case AssetTemplate.Enforcer:
                return new DynaEnemyIN2Enforcer(assetName, position);
            case AssetTemplate.Humanoid:
                return new DynaEnemyIN2Humanoid(assetName, position);
            case AssetTemplate.RotuRat:
                return new DynaEnemyIN2Rat(assetName, position);
            case AssetTemplate.RobotTank:
                return new DynaEnemyIN2RobotTank(assetName, position);
            case AssetTemplate.Scientist:
                return new DynaEnemyIN2Scientist(assetName, position);
            case AssetTemplate.Shooter:
                return new DynaEnemyIN2Shooter(assetName, position);
        }
        MessageBox.Show("Unsupported template");
        return null;
    }

    public static string pkupsMinfName => "pickups.MINF";
    public static string playerName(Game game) => game == Game.Scooby ? "SCOOBY" : game == Game.BFBB ? "SPONGEBOB" : "HERO";
    public static string environmentName(Game game) => game == Game.Scooby ? "ENV" : game == Game.BFBB ? "ENVIRONMENT" : "ENV_THEWORLD";
    public static string startCamName(Game game) => game == Game.Scooby ? "START" : game == Game.BFBB ? "STARTCAM" : "CAM_START";

    public static (IEnumerable<Layer> layers, IEnumerable<Asset> assets) GenerateDefaultAssets(Game game, Platform platform)
    {
        static bool exists(uint _) => false;

        var assets = new List<Asset>();

        Asset bsp = null;

        if (game < Game.Incredibles)
        {
            bsp = GenerateTemplate(game, platform, exists, AssetTemplate.Empty_BSP, "empty_bsp")[0];
            assets.Add(bsp);
        }

        var player = (AssetPLYR)GenerateTemplate(game, platform, exists, AssetTemplate.Player, playerName(game))[0];
        assets.Add(player);

        var env = (AssetENV)GenerateTemplate(game, platform, exists, AssetTemplate.Environment, environmentName(game))[0];
        assets.Add(env);

        AssetCAM startCam = (AssetCAM)GenerateTemplate(game, platform, exists, AssetTemplate.Start_Camera, startCamName(game), new Vector3(0, 100, 100))[0];
        assets.Add(startCam);
        env.StartCamera = startCam.assetID;

        if (game < Game.Incredibles)
        {
            env.BSP = bsp.assetID;
            var minf = GenerateTemplate(game, platform, exists, AssetTemplate.Model_Info, pkupsMinfName)[0];
            assets.Add(minf);
        }

        if (game == Game.BFBB)
        {
            var objlkit = GenerateTemplate(game, platform, exists, AssetTemplate.LKIT_lights, "lights")[0];
            var playerlkit = GenerateTemplate(game, platform, exists, AssetTemplate.LKIT_JF_SB_lights, "JF_SB_lights")[0];

            env.Object_LightKit = objlkit.assetID;
            player.LightKit = playerlkit.assetID;

            assets.Add(objlkit);
            assets.Add(playerlkit);
        }
        else if (game >= Game.Incredibles)
        {
            var light_kit = GenerateTemplate(game, platform, exists, AssetTemplate.LKIT_lights, "jf01_light_kit")[0];

            player.LightKit = light_kit.assetID;
            env.Object_LightKit = light_kit.assetID;

            var defaultGlowSceneProp = GenerateTemplate(game, platform, exists, AssetTemplate.DefaultGlowSceneProp)[0];

            assets.Add(light_kit);
            assets.Add(defaultGlowSceneProp);
        }

        var layers = new List<Layer>();

        if (game < Game.Incredibles)
        {
            var bspLayer = new Layer(LayerType.BSP);
            bspLayer.AssetIDs.Add(bsp.assetID);
            layers.Add(bspLayer);
        }
        var defaultLayer = new Layer(LayerType.DEFAULT);
        defaultLayer.AssetIDs.AddRange(assets.Select(a => a.assetID).Where(u => u != bsp.assetID));
        layers.Add(defaultLayer);

        return (layers, assets);
    }

    public static byte[] GenerateBlankBSP()
    {
        int MaximumBoundary = 1000;

        Vertex3 Max = new Vertex3(MaximumBoundary, MaximumBoundary, MaximumBoundary);
        Vertex3 Min = new Vertex3(-MaximumBoundary, -MaximumBoundary, -MaximumBoundary);

        return ReadFileMethods.ExportRenderWareFile(new World_000B()
        {
            worldStruct = new WorldStruct_0001()
            {
                rootIsWorldSector = 1,
                inverseOrigin = new Vertex3(-0f, -0f, -0f),
                numTriangles = 0,
                numVertices = 0,
                numPlaneSectors = 0,
                numAtomicSectors = 1,
                colSectorSize = 0,
                worldFlags = (WorldFlags)0x00010000,
                boxMaximum = new Vertex3(),
                boxMinimum = new Vertex3(),
            },

            materialList = new MaterialList_0008([]),

            firstWorldChunk = new AtomicSector_0009()
            {
                atomicSectorStruct = new AtomicSectorStruct_0001()
                {
                    matListWindowBase = 0,
                    numTriangles = 0,
                    numVertices = 0,
                    boxMaximum = Max,
                    boxMinimum = Min,
                    collSectorPresent = 0,
                    unused = 0,
                    vertexArray = new Vertex3[0],
                    colorArray = new RenderWareFile.Color[0],
                    uvArray = new Vertex2[0],
                    triangleArray = new RenderWareFile.Triangle[0]
                },
                atomicSectorExtension = new Extension_0003([
                    new BinMeshPLG_050E()
                    {
                        binMeshHeaderFlags = BinMeshHeaderFlags.TriangleList,
                        numMeshes = 0,
                        totalIndexCount = 0,
                        binMeshList = new BinMesh[0]
                    },
                    new CollisionPLG_011D_Pre36001(new RpCollBSPBranchNode[0], new RpCollBSPLeafNode[] { new RpCollBSPLeafNode() }, new int[0]),
                ])
            },

            worldExtension = new Extension_0003()
        }, 0x0310);
    }

    public static List<Asset> ConvertScriptToGroupOfTimers(Platform platform, AssetSCRP script, Func<uint, bool> exists)
    {
        var assets = new List<Asset>();
        var num = 1;
        foreach (Link link in script.TimedLinks)
        {
            var timer = (AssetTIMR)GenerateTemplate(script.game, platform, exists, AssetTemplate.Timer, $"{script.assetName}_TIMER_{num:D2}", ignoreNumber: true)[0];
            timer.Time = link.Time;
            timer.Links = [
                new Link(timer.game)
                {
                    EventReceiveID = 0x0014,
                    EventSendID = link.EventSendID,
                    TargetAsset = link.TargetAsset,
                    ArgumentAsset = link.ArgumentAsset,
                    Parameter1 = link.Parameter1,
                    Parameter2 = link.Parameter2,
                    Parameter3 = link.Parameter3,
                    Parameter4 = link.Parameter4,
                }
            ];
            assets.Add(timer);
            num++;
        }
        var group = (AssetGRUP)GenerateTemplate(script.game, platform, exists, AssetTemplate.Group, $"{script.assetName}_GROUP", ignoreNumber: true)[0];
        group.Items = assets.Select(a => new AssetID(a.assetID)).ToArray();
        group.Links = script.Links;
        assets.Insert(0, group);
        return assets;
    }
}