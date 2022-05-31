using HipHopFile;
using Newtonsoft.Json;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public static void PopulateTemplateMenusAt(ToolStripMenuItem menu, EventHandler eventHandler)
        {
            ToolStripMenuItem controllers = new ToolStripMenuItem("Stage Controllers");
            controllers.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Counter.ToString()),
                new ToolStripMenuItem(AssetTemplate.Conditional.ToString()),
                new ToolStripMenuItem(AssetTemplate.Dispatcher.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fog.ToString()),
                new ToolStripMenuItem(AssetTemplate.Flythrough.ToString()),
                new ToolStripMenuItem(AssetTemplate.Group.ToString()),
                new ToolStripMenuItem(AssetTemplate.Portal.ToString()),
                new ToolStripMenuItem(AssetTemplate.Script.ToString()),
                new ToolStripMenuItem(AssetTemplate.SoundGroup.ToString()),
                new ToolStripMenuItem(AssetTemplate.Text.ToString()),
                new ToolStripMenuItem(AssetTemplate.Timer.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.CamTweak.ToString())
            });
            foreach (ToolStripItem i in controllers.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            // BFBB
            ToolStripMenuItem pickupsBFBB = new ToolStripMenuItem("Pickups and Tikis");
            pickupsBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Shiny_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Yellow.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Green.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Blue.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shiny_Purple.ToString()),
                new ToolStripMenuItem(AssetTemplate.Underwear.ToString()),
                new ToolStripMenuItem(AssetTemplate.Spongeball.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.WoodenTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.FloatingTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.ThunderTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.ShhhTiki.ToString()),
                new ToolStripMenuItem(AssetTemplate.StoneTiki.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Spatula.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sock.ToString()),
                new ToolStripMenuItem(AssetTemplate.Golden_Underwear.ToString()),
                new ToolStripMenuItem(AssetTemplate.Smelly_Sundae.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.SteeringWheel.ToString()),
                new ToolStripMenuItem(AssetTemplate.Artwork.ToString()),
                new ToolStripMenuItem(AssetTemplate.PowerCrystal.ToString()),
            });
            foreach (ToolStripItem i in pickupsBFBB.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem enemiesBFBB = new ToolStripMenuItem("Enemies");
            enemiesBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Fodder.ToString()),
                new ToolStripMenuItem(AssetTemplate.Hammer.ToString()),
                new ToolStripMenuItem(AssetTemplate.TarTar.ToString()),
                new ToolStripMenuItem(AssetTemplate.ChompBot.ToString()),
                new ToolStripMenuItem(AssetTemplate.GLove.ToString()),
                new ToolStripMenuItem(AssetTemplate.Chuck.ToString()),
                new ToolStripMenuItem(AssetTemplate.Chuck_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Monsoon.ToString()),
                new ToolStripMenuItem(AssetTemplate.Monsoon_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sleepytime.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sleepytime_Moving.ToString()),
                new ToolStripMenuItem(AssetTemplate.Arf.ToString()),
                new ToolStripMenuItem(AssetTemplate.BombBot.ToString()),
                new ToolStripMenuItem(AssetTemplate.Tubelet.ToString()),
                new ToolStripMenuItem(AssetTemplate.BzztBot.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slick.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slick_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Jellyfish_Pink.ToString()),
                new ToolStripMenuItem(AssetTemplate.Jellyfish_Blue.ToString()),
                new ToolStripMenuItem(AssetTemplate.Duplicatotron.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.DuplicatotronSettings.ToString())
            });
            foreach (ToolStripItem i in enemiesBFBB.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem stageitemsBFBB = new ToolStripMenuItem("Stage Items");
            stageitemsBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Button_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.PressurePlate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint_Invisible.ToString()),
                new ToolStripMenuItem(AssetTemplate.BusStop.ToString()),
                new ToolStripMenuItem(AssetTemplate.TeleportBox.ToString()),
                new ToolStripMenuItem(AssetTemplate.ThrowFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.FreezyFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.TexasHitch.ToString()),
                new ToolStripMenuItem(AssetTemplate.TexasHitch_PLAT.ToString()),
                new ToolStripMenuItem(AssetTemplate.BungeeHook.ToString()),
                new ToolStripMenuItem(AssetTemplate.BungeeDrop.ToString()),
                new ToolStripMenuItem(AssetTemplate.HoveringPlatform.ToString()),
                new ToolStripMenuItem(AssetTemplate.Springboard.ToString()),
                new ToolStripMenuItem(AssetTemplate.TaxiStand.ToString()),
            });
            foreach (ToolStripItem i in stageitemsBFBB.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem bfbb = new ToolStripMenuItem("Battle For Bikini Bottom");
            bfbb.DropDownItems.AddRange(new ToolStripItem[]
            {
                pickupsBFBB,
                enemiesBFBB,
                stageitemsBFBB
            });

            // Movie
            ToolStripMenuItem pickupsTSSM = new ToolStripMenuItem("Pickups and Crates");
            pickupsTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Manliness_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Yellow.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Green.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Blue.ToString()),
                new ToolStripMenuItem(AssetTemplate.Manliness_Purple.ToString()),
                new ToolStripMenuItem(AssetTemplate.KrabbyPatty.ToString()),
                new ToolStripMenuItem(AssetTemplate.GoofyGooberToken.ToString()),
                new ToolStripMenuItem(AssetTemplate.TreasureChest.ToString()),
                new ToolStripMenuItem(AssetTemplate.Nitro.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Wood_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Hover_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Explode_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Shrink_Crate.ToString()),
                new ToolStripMenuItem(AssetTemplate.Steel_Crate.ToString()),
            });
            foreach (ToolStripItem i in pickupsTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem enemiesTSSM = new ToolStripMenuItem("Enemies");
            enemiesTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Fogger_GoofyGoober.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Desert.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_ThugTug.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Trench.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Junkyard.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_Planktopolis.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_v1.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_v2.ToString()),
                new ToolStripMenuItem(AssetTemplate.Fogger_v3.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Slammer_GoofyGoober.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slammer_Desert.ToString()),
                new ToolStripMenuItem(AssetTemplate.Slammer_ThugTug.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Flinger_Desert.ToString()),
                new ToolStripMenuItem(AssetTemplate.Flinger_Trench.ToString()),
                new ToolStripMenuItem(AssetTemplate.Flinger_Junkyard.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Spinner_ThugTug.ToString()),
                new ToolStripMenuItem(AssetTemplate.Spinner_Junkyard.ToString()),
                new ToolStripMenuItem(AssetTemplate.Spinner_Planktopolis.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Popper_Trench.ToString()),
                new ToolStripMenuItem(AssetTemplate.Popper_Planktopolis.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Minimerv.ToString()),
                new ToolStripMenuItem(AssetTemplate.Mervyn.ToString()),
            });
            foreach (ToolStripItem i in enemiesTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem moreEnemiesTSSM = new ToolStripMenuItem("More Enemies");
            moreEnemiesTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Jelly_Critter.ToString()),
                new ToolStripMenuItem(AssetTemplate.Jelly_Bucket.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Turret_v1.ToString()),
                new ToolStripMenuItem(AssetTemplate.Turret_v2.ToString()),
                new ToolStripMenuItem(AssetTemplate.Turret_v3.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_BB.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_DE.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_GG.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_TR.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_JK.ToString()),
                new ToolStripMenuItem(AssetTemplate.BucketOTron_PT.ToString())
            });
            foreach (ToolStripItem i in moreEnemiesTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem stageitemsTSSM = new ToolStripMenuItem("Stage Items");
            stageitemsTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Button_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint.ToString()),
                new ToolStripMenuItem(AssetTemplate.Checkpoint_Invisible.ToString()),
                new ToolStripMenuItem(AssetTemplate.TeleportBox.ToString()),
                new ToolStripMenuItem(AssetTemplate.ThrowFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.FreezyFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.Swinger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Swinger_PLAT.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollapsePlatform_Planktopolis.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollapsePlatform_Spongeball.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollapsePlatform_ThugTug.ToString()),
                new ToolStripMenuItem(AssetTemplate.Ring.ToString()),
                new ToolStripMenuItem(AssetTemplate.RingControl.ToString())
            });
            foreach (ToolStripItem i in stageitemsTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem tssm = new ToolStripMenuItem("Movie Game");
            tssm.DropDownItems.AddRange(new ToolStripItem[]
            {
                pickupsTSSM,
                enemiesTSSM,
                moreEnemiesTSSM,
                stageitemsTSSM
            });

            ToolStripMenuItem placeable = new ToolStripMenuItem("Placeable");
            placeable.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Camera.ToString()),
                new ToolStripMenuItem(AssetTemplate.Marker.ToString()),
                new ToolStripMenuItem(AssetTemplate.Point_MVPT.ToString()),
                new ToolStripMenuItem(AssetTemplate.Area_MVPT.ToString()),
                new ToolStripMenuItem(AssetTemplate.Box_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Sphere_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Cylinder_Trigger.ToString()),
                new ToolStripMenuItem(AssetTemplate.SFX_OnEvent.ToString()),
                new ToolStripMenuItem(AssetTemplate.SFX_OnRadius.ToString()),
                new ToolStripMenuItem(AssetTemplate.Dyna_Pointer.ToString()),
                new ToolStripMenuItem(AssetTemplate.Player.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Boulder_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Button_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Destructible_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.ElectricArc_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Hangable_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.NPC_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Pendulum_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Platform_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.SIMP_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.VIL_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.UI_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.UIFT_Generic.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.SDFX.ToString()),
                new ToolStripMenuItem(AssetTemplate.LightEmitter_Generic.ToString()),
            });
            foreach (ToolStripItem i in placeable.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem others = new ToolStripMenuItem("Other");
            others.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.AnimationList.ToString()),
                new ToolStripMenuItem(AssetTemplate.CollisionTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.Default_Glow_Scene_Prop.ToString()),
                new ToolStripMenuItem(AssetTemplate.Environment.ToString()),
                new ToolStripMenuItem(AssetTemplate.JawData.ToString()),
                new ToolStripMenuItem(AssetTemplate.LevelOfDetailTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.MaterialMap.ToString()),
                new ToolStripMenuItem(AssetTemplate.MINF_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.PipeInfoTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.ShadowTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.SoundInfo.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.EmptyBSP.ToString()),
                new ToolStripMenuItem(AssetTemplate.EmptySND.ToString()),
                new ToolStripMenuItem(AssetTemplate.EmptySNDS.ToString()),
            });
            foreach (ToolStripItem i in others.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem paste = new ToolStripMenuItem(AssetTemplate.PasteClipboard.ToString());
            paste.Click += eventHandler;

            menu.DropDownItems.AddRange(new ToolStripItem[] { controllers, placeable, bfbb, tssm, others, paste });
        }

        public void SetAssetPositionToView(uint assetID)
        {
            Vector3 Position = Program.MainForm.renderer.Camera.Position + 3 * Program.MainForm.renderer.Camera.Forward;

            if (GetFromAssetID(assetID) is AssetTRIG trig)
            {
                trig.PositionX = Position.X;
                trig.PositionY = Position.Y;
                trig.PositionZ = Position.Z;

                trig.MinimumX = Position.X;
                trig.MinimumY = Position.Y;
                trig.MinimumZ = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetCAM cam)
            {
                cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                cam.SetNormalizedForward(Program.MainForm.renderer.Camera.Forward);
                cam.SetNormalizedUp(Program.MainForm.renderer.Camera.Up);
                cam.SetNormalizedLeft(Program.MainForm.renderer.Camera.Right);
            }
            else if (GetFromAssetID(assetID) is IClickableAsset ir)
            {
                ir.PositionX = Position.X;
                ir.PositionY = Position.Y;
                ir.PositionZ = Position.Z;
            }
        }

        public static AssetTemplate CurrentAssetTemplate { get; set; } = AssetTemplate.Null;
        public static string CurrentUserTemplate { get; set; } = "";

        public static bool persistentShinies = true;
        public static bool chainPointMVPTs = false;
        public static uint chainPointMVPTlast = 0;

        private Asset PlaceUserTemplate(Vector3 position, int layerIndex, ref List<uint> assetIDs, AssetTemplate template)
        {
            if (template == AssetTemplate.PasteClipboard)
                PasteAssetsFromClipboard(layerIndex, out assetIDs, dontReplace: true);
            else
            {
                try
                {
                    var clipboard = JsonConvert.DeserializeObject<AssetClipboard>(File.ReadAllText(Path.Combine(Program.MainForm.userTemplatesFolder, CurrentUserTemplate)));
                    PasteAssetsFromClipboard(layerIndex, out assetIDs, clipboard, forceRefUpdate: true, dontReplace: true);
                }
                catch
                {
                    return null;
                }
            }

            if (assetIDs.Count > 0)
                foreach (uint assetID in assetIDs)
                    if (GetFromAssetID(assetID) is AssetCAM cam)
                    {
                        cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                        cam.SetNormalizedForward(Program.MainForm.renderer.Camera.Forward);
                        cam.SetNormalizedUp(Program.MainForm.renderer.Camera.Up);
                        cam.SetNormalizedLeft(Program.MainForm.renderer.Camera.Right);
                    }
                    else if (GetFromAssetID(assetID) is IClickableAsset ica)
                    {
                        Vector3 delta = position - new Vector3(ica.PositionX, ica.PositionY, ica.PositionZ);

                        ica.PositionX = position.X;
                        ica.PositionY = position.Y;
                        ica.PositionZ = position.Z;

                        if (ica is AssetTRIG trig)
                        {
                            if (trig.Shape == TriggerShape.Box)
                            {
                                trig.SetPositions(
                                    trig.MinimumX + delta.X,
                                    trig.MinimumY + delta.Y,
                                    trig.MinimumZ + delta.Z,
                                    trig.MaximumX + delta.X,
                                    trig.MaximumY + delta.Y,
                                    trig.MaximumZ + delta.Z);
                            }
                            else
                            {
                                trig.MinimumX = position.X;
                                trig.MinimumY = position.Y;
                                trig.MinimumZ = position.Z;
                            }
                        }
                    }

            return null;
        }

        public Asset PlaceTemplate(Vector3 position, int layerIndex, string customName = "", AssetTemplate template = AssetTemplate.Null)
        {
            var assetIDs = new List<uint>();
            return PlaceTemplate(position, layerIndex, ref assetIDs, customName, template);
        }

        public Asset PlaceTemplate(Vector3 position, int layerIndex, ref List<uint> assetIDs, string assetName = null, AssetTemplate template = AssetTemplate.Null)
        {
            if (template == AssetTemplate.Null)
                template = CurrentAssetTemplate;
            if (template == AssetTemplate.UserTemplate || template == AssetTemplate.PasteClipboard)
                return PlaceUserTemplate(position, layerIndex, ref assetIDs, template);

            bool ignoreNumber = false;

            if (assetName == null)
                assetName = template.ToString().ToUpper() + "_01";

            bool giveIdRegardless = false;

            switch (template)
            {
                case AssetTemplate.Default_Glow_Scene_Prop:
                case AssetTemplate.EmptyBSP:
                case AssetTemplate.Environment:
                case AssetTemplate.LKIT_lights:
                case AssetTemplate.LKIT_JF_SB_lights:
                case AssetTemplate.LKIT_jf01_light_kit:
                case AssetTemplate.MINF_Generic:
                case AssetTemplate.Flythrough_Widget:
                    ignoreNumber = true;
                    break;
                case AssetTemplate.StartCamera:
                    assetName = startCamName;
                    ignoreNumber = true;
                    break;
                case AssetTemplate.SoundInfo:
                    assetName = "sound_info";
                    ignoreNumber = true;
                    break;

                case AssetTemplate.Player:
                    assetName = playerName;
                    giveIdRegardless = true;
                    break;
                case AssetTemplate.Checkpoint:
                    assetName = "CHECKPOINT_TRIG_01";
                    giveIdRegardless = true;
                    break;
                default:
                    giveIdRegardless = true;
                    break;
            }

            if (ContainsAsset(new AssetID(assetName)))
                ignoreNumber = false;

            assetName = GetUniqueAssetName(assetName, BKDRHash(assetName), giveIdRegardless, ignoreNumber);

            Asset asset;

            switch (template)
            {
                case AssetTemplate.Counter:
                    asset = new AssetCNTR(assetName);
                    break;
                case AssetTemplate.Conditional:
                    asset = new AssetCOND(assetName);
                    break;
                case AssetTemplate.Dispatcher:
                    asset = new AssetDPAT(assetName);
                    break;
                case AssetTemplate.Fog:
                    asset = new AssetFOG(assetName);
                    break;
                case AssetTemplate.Group:
                    asset = new AssetGRUP(assetName);
                    break;
                case AssetTemplate.Portal:
                    asset = new AssetPORT(assetName);
                    break;
                case AssetTemplate.Script:
                    asset = new AssetSCRP(assetName);
                    break;
                case AssetTemplate.SoundGroup:
                    asset = new AssetSGRP(assetName);
                    break;
                case AssetTemplate.Text:
                    asset = new AssetTEXT(assetName);
                    break;
                case AssetTemplate.Timer:
                    asset = new AssetTIMR(assetName);
                    break;
                case AssetTemplate.Checkpoint_Timer:
                    {
                        var timer = new AssetTIMR(assetName);
                        asset = timer;

                        timer.Time = 0.5f;
                        var checkpointSimp = PlaceTemplate(new Vector3(position.X + 2f, position.Y, position.Z), layerIndex, ref assetIDs, "CHECKPOINT_SIMP", AssetTemplate.Checkpoint_SIMP);
                        var checkpointTalkbox = BKDRHash("CHECKPOINT_TALKBOX_00");
                        if (!ContainsAsset(checkpointTalkbox))
                            checkpointTalkbox = PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_TALKBOX", AssetTemplate.Checkpoint_Talkbox).assetID;

                        timer.Links = new Link[] {
                            new Link(game)
                            {
                                FloatParameter1 = 2,
                                TargetAssetID = checkpointSimp.assetID,
                                EventReceiveID = (ushort)EventBFBB.Run,
                                EventSendID = (ushort)EventBFBB.AnimPlay
                            },
                            new Link(game)
                            {
                                Parameter1 = "checkpoint_text",
                                TargetAssetID = checkpointTalkbox,
                                EventReceiveID = (ushort)EventBFBB.Run,
                                EventSendID = (ushort)EventBFBB.StartConversation
                            },
                            new Link(game)
                            {
                                FloatParameter1 = 3,
                                TargetAssetID = checkpointSimp.assetID,
                                EventReceiveID = (ushort)EventBFBB.Expired,
                                EventSendID = (ushort)EventBFBB.AnimPlayLoop
                            },
                            new Link(game)
                            {
                                TargetAssetID = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.Expired,
                                EventSendID = (ushort)EventBFBB.Disable
                            },
                        };
                    }
                    break;
                case AssetTemplate.CamTweak:
                    asset = new DynaGObjectCamTweak(assetName);
                    break;
                case AssetTemplate.DuplicatotronSettings:
                    asset = new DynaGObjectNPCSettings(assetName);
                    break;
                case AssetTemplate.Camera:
                case AssetTemplate.StartCamera:
                case AssetTemplate.BusStop_Camera:
                    asset = new AssetCAM(assetName, position, template);
                    break;
                case AssetTemplate.Marker:
                    asset = new AssetMRKR(assetName, position);
                    break;
                case AssetTemplate.Box_Trigger:
                case AssetTemplate.Sphere_Trigger:
                case AssetTemplate.Cylinder_Trigger:
                    asset = new AssetTRIG(assetName, position, template);
                    break;
                case AssetTemplate.Checkpoint:
                case AssetTemplate.Checkpoint_Invisible:
                    asset = new AssetTRIG(assetName, position, template);
                    {
                        AssetID checkpointDisp = "CHECKPOINT_DISP_01";
                        if (!ContainsAsset(checkpointDisp))
                            checkpointDisp = PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_DISP", AssetTemplate.Dispatcher).assetID;

                        var links = new List<Link>
                        {
                            new Link(game)
                            {
                                ArgumentAssetID = game == Game.Incredibles ?
                                PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_POINTER", AssetTemplate.Dyna_Pointer).assetID :
                                PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_MRKR", AssetTemplate.Marker).assetID,
                                TargetAssetID = checkpointDisp,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.SetCheckPoint
                            }
                        };

                        if (template == AssetTemplate.Checkpoint && game == Game.BFBB)
                            links.Add(new Link(game)
                            {
                                TargetAssetID = PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_TIMER", AssetTemplate.Checkpoint_Timer).assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.Run
                            });

                        if (template == AssetTemplate.Checkpoint && game == Game.Incredibles)
                        {
                            links.Add(new Link(game)
                            {
                                TargetAssetID = PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_SCRIPT", AssetTemplate.Checkpoint_Script).assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.Run
                            });
                            links.Add(new Link(game)
                            {
                                TargetAssetID = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.Disable
                            });
                        }

                        ((AssetTRIG)asset).Links = links.ToArray();
                    }
                    break;
                case AssetTemplate.BusStop_Trigger:
                    {
                        asset = new AssetTRIG(assetName, position, template);
                        var lightsSimp = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("TRIGGER", "LIGHTS").Replace("TRIG", "LIGHTS"), AssetTemplate.BusStop_Lights);

                        ((AssetTRIG)asset).Links = new Link[] {
                            new Link(game)
                            {
                                TargetAssetID = lightsSimp.assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.Visible
                            },
                            new Link(game)
                            {
                                TargetAssetID = lightsSimp.assetID,
                                EventReceiveID = (ushort)EventBFBB.ExitPlayer,
                                EventSendID = (ushort)EventBFBB.Invisible
                            }
                        };
                    }
                    break;
                case AssetTemplate.Area_MVPT:
                case AssetTemplate.Point_MVPT:
                    asset = new AssetMVPT(assetName, position, game, template);
                    if (template == AssetTemplate.Point_MVPT && chainPointMVPTs && ContainsAsset(chainPointMVPTlast))
                    {
                        var prev = (AssetMVPT)GetFromAssetID(chainPointMVPTlast);
                        var prevNexts = prev.NextMVPTs.ToList();
                        prevNexts.Add(asset.assetID);
                        prev.NextMVPTs = prevNexts.ToArray();
                    }
                    chainPointMVPTlast = asset.assetID;
                    break;
                case AssetTemplate.Dyna_Pointer:
                    asset = new DynaPointer(assetName, position);
                    break;
                case AssetTemplate.Player:
                    asset = new AssetPLYR(assetName, position, game);
                    break;
                case AssetTemplate.Boulder_Generic:
                    asset = new AssetBOUL(assetName, position);
                    break;
                case AssetTemplate.Button_Generic:
                case AssetTemplate.Button_Red:
                case AssetTemplate.PressurePlate:
                    asset = new AssetBUTN(game, assetName, position, template);
                    if (template == AssetTemplate.PressurePlate)
                        PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_BASE", AssetTemplate.PressurePlateBase);
                    break;
                case AssetTemplate.Destructible_Generic:
                    asset = new AssetDSTR(assetName, position);
                    break;
                case AssetTemplate.ElectricArc_Generic:
                    asset = new AssetEGEN(assetName, position);
                    break;
                case AssetTemplate.Hangable_Generic:
                    asset = new AssetHANG(assetName, position);
                    break;
                case AssetTemplate.NPC_Generic:
                    asset = new AssetNPC(assetName, position);
                    break;
                case AssetTemplate.Pendulum_Generic:
                    asset = new AssetPEND(assetName, position);
                    break;
                case AssetTemplate.Platform_Generic:
                case AssetTemplate.TexasHitch_PLAT:
                case AssetTemplate.Swinger_PLAT:
                case AssetTemplate.Springboard:
                case AssetTemplate.HoveringPlatform:
                case AssetTemplate.CollapsePlatform_Planktopolis:
                case AssetTemplate.CollapsePlatform_ThugTug:
                case AssetTemplate.CollapsePlatform_Spongeball:
                    asset = new AssetPLAT(game, assetName, position, template);
                    break;
                case AssetTemplate.SIMP_Generic:
                case AssetTemplate.TaxiStand:
                case AssetTemplate.TexasHitch:
                case AssetTemplate.Swinger:
                case AssetTemplate.BusStop:
                case AssetTemplate.ThrowFruit:
                case AssetTemplate.FreezyFruit:
                case AssetTemplate.PressurePlateBase:
                case AssetTemplate.ThrowFruitBase:
                case AssetTemplate.BusStop_BusSimp:
                case AssetTemplate.BusStop_Lights:
                case AssetTemplate.Checkpoint_SIMP:
                case AssetTemplate.Checkpoint_SIMP_TSSM:
                case AssetTemplate.BungeeHook_SIMP:
                    asset = new AssetSIMP(assetName, position, template);
                    switch (template)
                    {
                        case AssetTemplate.BusStop:
                            PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.BusStop_Trigger);
                            position.Y += 0.1f;
                            PlaceTemplate(position, layerIndex, ref assetIDs, template: AssetTemplate.BusStop_DYNA);
                            break;
                        case AssetTemplate.ThrowFruit:
                            PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "BASE", AssetTemplate.ThrowFruitBase);
                            break;
                        case AssetTemplate.FreezyFruit:
                            PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "BASE", AssetTemplate.ThrowFruitBase);
                            break;
                    }
                    break;
                case AssetTemplate.VIL_Generic:
                    asset = new AssetVIL(assetName, position, template, 0);
                    break;
                case AssetTemplate.UI_Generic:
                    asset = new AssetUI(assetName, position);
                    break;
                case AssetTemplate.UIFT_Generic:
                    asset = new AssetUIFT(assetName, position);
                    break;
                case AssetTemplate.SFX_OnEvent:
                case AssetTemplate.SFX_OnRadius:
                    asset = new AssetSFX(assetName, position, game, template == AssetTemplate.SFX_OnRadius);
                    break;
                case AssetTemplate.SDFX:
                    asset = new AssetSDFX(assetName, position);
                    break;
                case AssetTemplate.LightEmitter_Generic:
                    asset = new AssetLITE(assetName, position);
                    break;
                case AssetTemplate.AnimationList:
                    asset = new AssetALST(assetName);
                    break;
                case AssetTemplate.CollisionTable:
                    asset = new AssetCOLL(assetName);
                    break;
                case AssetTemplate.Environment:
                    asset = new AssetENV(assetName, startCamName);
                    break;
                case AssetTemplate.Flythrough:
                    asset = new AssetFLY(assetName);
                    var flyWidget = (DynaGObjectFlythrough)PlaceTemplate(position, layerIndex, ref assetIDs, asset.assetName + "_WIDGET", AssetTemplate.Flythrough_Widget);
                    flyWidget.FLY_ID = asset.assetID;
                    break;
                case AssetTemplate.Flythrough_Widget:
                    asset = new DynaGObjectFlythrough(assetName);
                    break;
                case AssetTemplate.JawData:
                    asset = new AssetJAW(assetName);
                    break;
                case AssetTemplate.LevelOfDetailTable:
                    asset = new AssetLODT(assetName);
                    break;
                case AssetTemplate.MaterialMap:
                    asset = new AssetMAPR(assetName);
                    break;
                case AssetTemplate.MINF_Generic:
                    asset = new AssetMINF(assetName);
                    break;
                case AssetTemplate.PipeInfoTable:
                    asset = new AssetPIPT(assetName);
                    break;
                case AssetTemplate.ShadowTable:
                    asset = new AssetSHDW(assetName);
                    break;
                case AssetTemplate.SoundInfo:
                    if (platform == Platform.Xbox)
                        asset = new AssetSNDI_XBOX(assetName);
                    else if (platform == Platform.PS2)
                        asset = new AssetSNDI_PS2(assetName);
                    else if (platform == Platform.GameCube)
                    {
                        if (game == Game.Incredibles)
                            asset = new AssetSNDI_GCN_V2(assetName);
                        else
                            asset = new AssetSNDI_GCN_V1(assetName);
                    }
                    else asset = null;
                    break;
                case AssetTemplate.EmptySND:
                case AssetTemplate.EmptySNDS:
                    asset = new AssetSound(assetName, template == AssetTemplate.EmptySND ? AssetType.Sound : AssetType.StreamingSound, game, platform, new byte[0]);
                    break;
                case AssetTemplate.EmptyBSP:
                    asset = new AssetJSP(assetName, AssetType.BSP, GenerateBlankBSP(), standalone ? Program.MainForm.renderer : null);
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
                case AssetTemplate.Golden_Underwear:
                case AssetTemplate.Artwork:
                case AssetTemplate.SteeringWheel:
                case AssetTemplate.PowerCrystal:
                case AssetTemplate.Smelly_Sundae:
                case AssetTemplate.Manliness_Red:
                case AssetTemplate.Manliness_Yellow:
                case AssetTemplate.Manliness_Green:
                case AssetTemplate.Manliness_Blue:
                case AssetTemplate.Manliness_Purple:
                case AssetTemplate.KrabbyPatty:
                case AssetTemplate.GoofyGooberToken:
                case AssetTemplate.TreasureChest:
                case AssetTemplate.Nitro:
                    asset = new AssetPKUP(assetName, game, position, template);
                    break;
                case AssetTemplate.WoodenTiki:
                case AssetTemplate.FloatingTiki:
                case AssetTemplate.ThunderTiki:
                case AssetTemplate.ShhhTiki:
                case AssetTemplate.StoneTiki:
                case AssetTemplate.ArfDog:
                case AssetTemplate.TubeletSlave:
                case AssetTemplate.Duplicatotron:
                    {
                        asset = new AssetVIL(assetName, position, template, 0);
                        if (template == AssetTemplate.Duplicatotron)
                        {
                            var vil = (AssetVIL)asset;
                            vil.NPCSettings_AssetID = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_SETTINGS", AssetTemplate.DuplicatotronSettings).assetID;
                            vil.Links = new Link[] {
                                new Link(game)
                                {
                                    TargetAssetID = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_GROUP", AssetTemplate.Group).assetID,
                                    EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                                    EventSendID = (ushort)EventBFBB.Connect_IOwnYou
                                }
                            };
                        }
                    }
                    break;
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
                        var movePoint = (AssetMVPT)PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                        var vil = new AssetVIL(assetName, position, template, movePoint.assetID);
                        asset = vil;
                        if (template == AssetTemplate.Chuck_Trigger || template == AssetTemplate.Monsoon_Trigger || template == AssetTemplate.Slick_Trigger)
                        {
                            vil.Links = new Link[] {
                            new Link(game)
                            {
                                TargetAssetID = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                                EventSendID = (ushort)EventBFBB.DetectPlayerOff
                            }
                        };

                            var trigger = (AssetTRIG)PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.Sphere_Trigger);
                            trigger.Radius = 15f;
                            trigger.Links = new Link[] {
                            new Link(game)
                            {
                                TargetAssetID = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.DetectPlayerOn
                            },
                            new Link(game)
                            {
                                TargetAssetID = asset.assetID,
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
                                var dog = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_DOG_" + i, AssetTemplate.ArfDog);
                                links.Add(new Link(game)
                                {
                                    TargetAssetID = dog.assetID,
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
                                var slave = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_SLAVE_" + i, AssetTemplate.TubeletSlave);
                                links.Add(new Link(game)
                                {
                                    TargetAssetID = slave.assetID,
                                    EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                                    EventSendID = (ushort)EventBFBB.Connect_IOwnYou
                                });
                            }
                            vil.Links = links.ToArray();
                        }
                    }
                    break;
                case AssetTemplate.Wood_Crate:
                case AssetTemplate.Hover_Crate:
                case AssetTemplate.Explode_Crate:
                case AssetTemplate.Shrink_Crate:
                case AssetTemplate.Steel_Crate:
                    asset = new DynaEnemySupplyCrate(assetName, template, position);
                    break;
                case AssetTemplate.Jelly_Critter:
                case AssetTemplate.Jelly_Bucket:
                    {
                        var mvpt = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Point_MVPT);
                        asset = new DynaEnemyCritter(assetName, template, position, mvpt.assetID);
                    }
                    break;
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
                        var mvpt = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_MP",
                            (template.ToString().Contains("Flinger") || template.ToString().Contains("Popper")) ?
                            AssetTemplate.Point_MVPT : AssetTemplate.Area_MVPT);
                        asset = new DynaEnemyStandard(assetName, template, position, mvpt.assetID);
                    }
                    break;
                case AssetTemplate.Turret_v1:
                case AssetTemplate.Turret_v2:
                case AssetTemplate.Turret_v3:
                    asset = new DynaEnemyTurret(assetName, template, position);
                    break;
                case AssetTemplate.BucketOTron_BB:
                case AssetTemplate.BucketOTron_DE:
                case AssetTemplate.BucketOTron_GG:
                case AssetTemplate.BucketOTron_JK:
                case AssetTemplate.BucketOTron_TR:
                case AssetTemplate.BucketOTron_PT:
                    {
                        var group = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_GROUP", AssetTemplate.Group);
                        asset = new DynaEnemyBucketOTron(assetName, template, position, group.assetID);
                    }
                    break;
                case AssetTemplate.TeleportBox:
                    {
                        var mrkr = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_MRKR", AssetTemplate.Marker);
                        asset = new DynaGObjectTeleport(assetName, mrkr.assetID, GetMRKR);
                    }
                    break;
                case AssetTemplate.BungeeHook:
                    {
                        var simp = PlaceTemplate(position, layerIndex, ref assetIDs, "BUNGEE_SIMP", AssetTemplate.BungeeHook_SIMP);
                        asset = new DynaGObjectBungeeHook(assetName, simp.assetID);
                    }
                    break;
                case AssetTemplate.BungeeDrop:
                    {
                        var mrkr = PlaceTemplate(position, layerIndex, ref assetIDs, "BUNGEE_MRKR", AssetTemplate.Marker);
                        asset = new DynaGObjectBungeeDrop(assetName, mrkr.assetID);
                    }
                    break;
                case AssetTemplate.BusStop_DYNA:
                    {
                        var mrkr = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "MRKR"), AssetTemplate.Marker);
                        var cam = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "CAM"), AssetTemplate.BusStop_Camera);
                        var simp = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "SIMP"), AssetTemplate.BusStop_BusSimp);

                        asset = new DynaGObjectBusStop(assetName, mrkr.assetID, cam.assetID, simp.assetID);
                    }
                    break;
                case AssetTemplate.Checkpoint_Talkbox:
                    asset = new DynaGObjectTalkBox(assetName, true);
                    break;
                case AssetTemplate.Checkpoint_Script:
                    {
                        var scrp = new AssetSCRP(assetName);
                        asset = scrp;

                        var checkpointSdfx = (AssetSDFX)PlaceTemplate(new Vector3(position.X + 2f, position.Y, position.Z), layerIndex, ref assetIDs, "CHECKPOINT_SFX", AssetTemplate.SDFX);
                        var checkpointSimp = PlaceTemplate(new Vector3(position.X + 2f, position.Y, position.Z), layerIndex, ref assetIDs, "CHECKPOINT_SIMP", AssetTemplate.Checkpoint_SIMP_TSSM);

                        scrp.TimedLinks = new Link[] {
                            new Link(game)
                            {
                                TargetAssetID = checkpointSdfx.assetID,
                                EventSendID = (ushort)EventTSSM.Play
                            },
                            new Link(game)
                            {
                                FloatParameter4 = 10f,
                                TargetAssetID = checkpointSimp.assetID,
                                EventSendID = (ushort)EventTSSM.LaunchFireWorks
                            },
                            new Link(game)
                            {
                                FloatParameter1 = 2f,
                                TargetAssetID = checkpointSimp.assetID,
                                EventSendID = (ushort)EventTSSM.AnimPlay
                            },
                            new Link(game)
                            {
                                Time = 0.5f,
                                FloatParameter1 = 3f,
                                TargetAssetID = checkpointSimp.assetID,
                                EventSendID = (ushort)EventTSSM.AnimPlayLoop
                            },
                        };

                        checkpointSdfx.SoundGroup_AssetID = "CHECKPOINT_SGRP";
                        checkpointSdfx.Emitter_AssetID = checkpointSimp.assetID;
                        break;
                    }
                case AssetTemplate.Ring:
                    asset = new DynaGObjectRing(assetName, position);
                    break;
                case AssetTemplate.RingControl:
                    asset = new DynaGObjectRingControl(assetName);
                    break;
                case AssetTemplate.Default_Glow_Scene_Prop:
                    asset = new DynaSceneProperties(assetName);
                    break;
                case AssetTemplate.LKIT_lights:
                case AssetTemplate.LKIT_JF_SB_lights:
                case AssetTemplate.LKIT_jf01_light_kit:
                    {
                        var fileName =
                            template == AssetTemplate.LKIT_lights ? "lights" :
                            template == AssetTemplate.LKIT_JF_SB_lights ? "JF_SB_lights" :
                            template == AssetTemplate.LKIT_jf01_light_kit ? "jf01_light_kit" : "";
                        var data = File.ReadAllBytes(Path.Combine(Application.StartupPath, "Resources", fileName));
                        asset = new AssetLKIT(assetName, data, Endianness.Big);
                    }
                    break;
                default:
                    MessageBox.Show("Unsupported template");
                    return null;
            }

            AddAsset(layerIndex, asset, false);

            assetIDs.Add(asset.assetID);

            return asset;
        }

        public static int MaximumBoundary => 1000;

        private byte[] GenerateBlankBSP()
        {
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
                    worldFlags = WorldFlags.HasOneSetOfTextCoords | WorldFlags.HasVertexColors | WorldFlags.WorldSectorsOverlap | (WorldFlags)0x00010000,
                    boxMaximum = new Vertex3(),
                    boxMinimum = new Vertex3(),
                },

                materialList = new MaterialList_0008()
                {
                    materialListStruct = new MaterialListStruct_0001()
                    {
                        materialCount = 0
                    },
                    materialList = new Material_0007[0]
                },

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
                        triangleArray = new Triangle[0]
                    },
                    atomicSectorExtension = new Extension_0003()
                    {
                        extensionSectionList = new List<RWSection>()
                        {
                            new BinMeshPLG_050E()
                            {
                                binMeshHeaderFlags = BinMeshHeaderFlags.TriangleList,
                                numMeshes = 0,
                                totalIndexCount = 0,
                                binMeshList = new BinMesh[0]
                            },
                            new CollisionPLG_011D_Scooby()
                            {
                                splits = new Split_Scooby[0],
                                startIndex_amountOfTriangles = new short[][] { new short[] { 0, 0 } },
                                triangles = new int[0]
                            }
                        }
                    }
                },

                worldExtension = new Extension_0003()
            }, 0x0310);
        }

        public static string pkupsMinfName => "pickups.MINF";
        public string playerName => game == Game.Scooby ? "SCOOBY" : game == Game.BFBB ? "SPONGEBOB" : "HERO";
        public string environmentName => game == Game.Scooby ? "ENV" : game == Game.BFBB ? "ENVIRONMENT" : "ENV_THEWORLD";
        public string startCamName => game == Game.Scooby ? "START" : game == Game.BFBB ? "STARTCAM" : "CAM_START";

        private void PlaceDefaultAssets()
        {
            if (game != Game.Incredibles)
                AddLayer((int)LayerType_BFBB.BSP);
            AddLayer();

            int defaultLayer = game == Game.Incredibles ? 0 : 1;

            AssetPLYR player = (AssetPLYR)PlaceTemplate(new Vector3(), defaultLayer, template: AssetTemplate.Player);

            AssetENV env = (AssetENV)PlaceTemplate(new Vector3(), defaultLayer, customName: environmentName, template: AssetTemplate.Environment);

            env.StartCameraAssetID = PlaceTemplate(new Vector3(0, 100, 100), defaultLayer, customName: startCamName, template: AssetTemplate.StartCamera).assetID;

            if (game != Game.Incredibles)
            {
                env.BSP_AssetID = PlaceTemplate(new Vector3(), 0, "empty_bsp", template: AssetTemplate.EmptyBSP).assetID;
                PlaceTemplate(new Vector3(), defaultLayer, pkupsMinfName, template: AssetTemplate.MINF_Generic);
            }

            if (game == Game.BFBB)
            {
                env.Object_LKIT_AssetID = PlaceTemplate(new Vector3(), defaultLayer, customName: "lights", template: AssetTemplate.LKIT_lights).assetID;
                player.LightKit_AssetID = PlaceTemplate(new Vector3(), defaultLayer, customName: "JF_SB_lights", template: AssetTemplate.LKIT_JF_SB_lights).assetID;
            }
            else if (game == Game.Incredibles)
            {
                var light_kit = (AssetLKIT)PlaceTemplate(new Vector3(), defaultLayer, customName: "jf01_light_kit", template: AssetTemplate.LKIT_lights);
                player.LightKit_AssetID = light_kit.assetID;
                env.Object_LKIT_AssetID = light_kit.assetID;

                PlaceTemplate(new Vector3(), defaultLayer, customName: "DEFAULT_GLOW_SCENE_PROP", template: AssetTemplate.Default_Glow_Scene_Prop);
            }
        }
    }
}