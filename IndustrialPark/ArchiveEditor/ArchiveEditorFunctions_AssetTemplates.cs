using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using Newtonsoft.Json;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
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
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Jelly_Critter.ToString()),
                new ToolStripMenuItem(AssetTemplate.Jelly_Bucket.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Turret_v1.ToString()),
                new ToolStripMenuItem(AssetTemplate.Turret_v2.ToString()),
                new ToolStripMenuItem(AssetTemplate.Turret_v3.ToString())
            });
            foreach (ToolStripItem i in enemiesTSSM.DropDownItems)
                if (i is ToolStripMenuItem j)
                    j.Click += eventHandler;

            ToolStripMenuItem stageitemsTSSM = new ToolStripMenuItem("Stage Items");
            stageitemsTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem(AssetTemplate.Button_Red.ToString()),
                new ToolStripMenuItem(AssetTemplate.TeleportBox.ToString()),
                new ToolStripMenuItem(AssetTemplate.ThrowFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.FreezyFruit.ToString()),
                new ToolStripMenuItem(AssetTemplate.Swinger.ToString()),
                new ToolStripMenuItem(AssetTemplate.Swinger_PLAT.ToString()),
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
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.Boulder_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Button_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Destructible_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.ElectricArc_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Hangable_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.NPC_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Pendulum_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Platform_Generic.ToString()),
                new ToolStripMenuItem(AssetTemplate.Player_Generic.ToString()),
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
                new ToolStripMenuItem(AssetTemplate.Environment.ToString()),
                new ToolStripMenuItem(AssetTemplate.JawData.ToString()),
                new ToolStripMenuItem(AssetTemplate.LevelOfDetailTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.MaterialMap.ToString()),
                new ToolStripMenuItem(AssetTemplate.PipeInfoTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.ShadowTable.ToString()),
                new ToolStripMenuItem(AssetTemplate.SoundInfo.ToString()),
                new ToolStripSeparator(),
                new ToolStripMenuItem(AssetTemplate.EmptyBSP.ToString()),
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

                trig.Position0X = Position.X;
                trig.Position0Y = Position.Y;
                trig.Position0Z = Position.Z;
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

        public uint PlaceTemplate(Vector3 position, int layerIndex, out bool success, ref List<uint> assetIDs, string customName = "", AssetTemplate template = AssetTemplate.Null)
        {
            if (template == AssetTemplate.Null)
                template = CurrentAssetTemplate;
            if (template == AssetTemplate.UserTemplate || template == AssetTemplate.PasteClipboard)
            {
                if (template == AssetTemplate.PasteClipboard)
                    PasteAssetsFromClipboard(layerIndex, out assetIDs);
                else
                {
                    try
                    {
                        var clipboard = JsonConvert.DeserializeObject<AssetClipboard>(
                            File.ReadAllText(Path.Combine(Program.MainForm.userTemplatesFolder, CurrentUserTemplate)));
                        PasteAssetsFromClipboard(layerIndex, out assetIDs, clipboard, true);
                    }
                    catch
                    {
                        success = false;
                        return 0;
                    }
                }

                if (assetIDs.Count > 0)
                {
                    success = true;
                    foreach (uint assetID in assetIDs)
                    {
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
                                        trig.Position0X + delta.X,
                                        trig.Position0Y + delta.Y,
                                        trig.Position0Z + delta.Z,
                                        trig.Position1X + delta.X,
                                        trig.Position1Y + delta.Y,
                                        trig.Position1Z + delta.Z);
                                }
                                else
                                {
                                    trig.Position0X = position.X;
                                    trig.Position0Y = position.Y;
                                    trig.Position0Z = position.Z;
                                }
                            }
                        }
                    }
                }
                else
                    success = false;

                return 0;
            }

            AssetType newAssetType;
            int dataSize = -1;

            switch (template)
            {
                case AssetTemplate.AnimationList:
                    newAssetType = AssetType.ALST;
                    dataSize = 0x28;
                    break;
                case AssetTemplate.Boulder_Generic:
                    newAssetType = AssetType.BOUL;
                    dataSize = 0x9C + Asset.DataSizeOffset(game);
                    break;
                case AssetTemplate.EmptyBSP:
                    dataSize = 0;
                    newAssetType = AssetType.BSP;
                    break;
                case AssetTemplate.Button_Generic:
                case AssetTemplate.Button_Red:
                case AssetTemplate.PressurePlate:
                    if (game == Game.BFBB)
                        dataSize = 0x9C + Asset.DataSizeOffset(game);
                    else if (game == Game.Incredibles)
                        dataSize = 0xA8 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.BUTN;
                    break;
                case AssetTemplate.Camera:
                case AssetTemplate.BusStop_Camera:
                    dataSize = 0x88;
                    newAssetType = AssetType.CAM;
                    break;
                case AssetTemplate.Counter:
                    dataSize = 0xC;
                    newAssetType = AssetType.CNTR;
                    break;
                case AssetTemplate.CollisionTable:
                    dataSize = 4;
                    newAssetType = AssetType.COLL;
                    break;
                case AssetTemplate.Conditional:
                    dataSize = game == Game.Scooby ? 0x14 : 0x18;
                    newAssetType = AssetType.COND;
                    break;
                case AssetTemplate.Dispatcher:
                    dataSize = 8;
                    newAssetType = AssetType.DPAT;
                    break;
                case AssetTemplate.Destructible_Generic:
                    dataSize = 0x8C + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.DSTR;
                    break;
                case AssetTemplate.BusStop_DYNA:
                case AssetTemplate.DuplicatotronSettings:
                case AssetTemplate.TeleportBox:
                case AssetTemplate.Checkpoint_Talkbox:
                case AssetTemplate.BungeeHook:
                case AssetTemplate.BungeeDrop:
                case AssetTemplate.Dyna_Pointer:
                case AssetTemplate.Wood_Crate:
                case AssetTemplate.Hover_Crate:
                case AssetTemplate.Explode_Crate:
                case AssetTemplate.Shrink_Crate:
                case AssetTemplate.Steel_Crate:
                case AssetTemplate.Jelly_Critter:
                case AssetTemplate.Jelly_Bucket:
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
                case AssetTemplate.Flinger_Desert:
                case AssetTemplate.Flinger_Trench:
                case AssetTemplate.Flinger_Junkyard:
                case AssetTemplate.Spinner_ThugTug:
                case AssetTemplate.Spinner_Junkyard:
                case AssetTemplate.Spinner_Planktopolis:
                case AssetTemplate.Popper_Trench:
                case AssetTemplate.Popper_Planktopolis:
                case AssetTemplate.Minimerv:
                case AssetTemplate.Mervyn:
                case AssetTemplate.Turret_v1:
                case AssetTemplate.Turret_v2:
                case AssetTemplate.Turret_v3:
                case AssetTemplate.Ring:
                case AssetTemplate.RingControl:
                case AssetTemplate.CamTweak:
                    dataSize = 0x10;
                    newAssetType = AssetType.DYNA;
                    break;
                case AssetTemplate.ElectricArc_Generic:
                    dataSize = 0x6C + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.EGEN;
                    break;
                case AssetTemplate.Environment:
                    dataSize = game == Game.Incredibles ? 0x5C : 0x44;
                    newAssetType = AssetType.ENV;
                    break;
                case AssetTemplate.Flythrough:
                    dataSize = 0;
                    newAssetType = AssetType.FLY;
                    break;
                case AssetTemplate.Group:
                    dataSize = 0xC;
                    newAssetType = AssetType.GRUP;
                    break;
                case AssetTemplate.Hangable_Generic:
                    dataSize = 0x74 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.HANG;
                    break;
                case AssetTemplate.JawData:
                    dataSize = 4;
                    newAssetType = AssetType.JAW;
                    break;
                case AssetTemplate.LightEmitter_Generic:
                    dataSize = 0x44;
                    newAssetType = AssetType.LITE;
                    break;
                case AssetTemplate.LevelOfDetailTable:
                    dataSize = 4;
                    newAssetType = AssetType.LODT;
                    break;
                case AssetTemplate.MaterialMap:
                    dataSize = 8;
                    newAssetType = AssetType.MAPR;
                    break;
                case AssetTemplate.Marker:
                    dataSize = 12;
                    newAssetType = AssetType.MRKR;
                    break;
                case AssetTemplate.Area_MVPT:
                case AssetTemplate.Point_MVPT:
                    dataSize = game == Game.Scooby ? 0x20 : 0x28;
                    newAssetType = AssetType.MVPT;
                    break;
                case AssetTemplate.NPC_Generic:
                    dataSize = 0xC8 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.NPC;
                    break;
                case AssetTemplate.Pendulum_Generic:
                    dataSize = 0x88 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.PEND;
                    break;
                case AssetTemplate.PipeInfoTable:
                    dataSize = 4;
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
                case AssetTemplate.Manliness_Red:
                case AssetTemplate.Manliness_Yellow:
                case AssetTemplate.Manliness_Green:
                case AssetTemplate.Manliness_Blue:
                case AssetTemplate.Manliness_Purple:
                case AssetTemplate.KrabbyPatty:
                case AssetTemplate.GoofyGooberToken:
                case AssetTemplate.TreasureChest:
                case AssetTemplate.Nitro:
                    dataSize = 0x5C + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.PKUP;
                    break;
                case AssetTemplate.Platform_Generic:
                case AssetTemplate.TexasHitch_PLAT:
                case AssetTemplate.Swinger_PLAT:
                case AssetTemplate.HoveringPlatform:
                case AssetTemplate.Springboard:
                    switch (game)
                    {
                        case Game.BFBB:
                            dataSize = 0xC0;
                            break;
                        case Game.Incredibles:
                            dataSize = 0xC8;
                            break;
                        case Game.Scooby:
                            dataSize = 0xA8;
                            break;
                    }
                    newAssetType = AssetType.PLAT;
                    break;
                case AssetTemplate.Player_Generic:
                    dataSize = 0x58 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.PLYR;
                    break;
                case AssetTemplate.Portal:
                    dataSize = 0x18;
                    newAssetType = AssetType.PORT;
                    break;
                case AssetTemplate.Script:
                    dataSize = 0x14;
                    newAssetType = AssetType.SCRP;
                    break;
                case AssetTemplate.SoundGroup:
                    dataSize = 0x20;
                    newAssetType = AssetType.SGRP;
                    break;
                case AssetTemplate.SFX_OnEvent:
                case AssetTemplate.SFX_OnRadius:
                    dataSize = 0x30;
                    newAssetType = AssetType.SFX;
                    break;
                case AssetTemplate.SDFX:
                    dataSize = 0x20;
                    newAssetType = AssetType.SDFX;
                    break;
                case AssetTemplate.ShadowTable:
                    dataSize = 4;
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
                case AssetTemplate.Swinger:
                    dataSize = 0x60 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.SIMP;
                    break;
                case AssetTemplate.SoundInfo:
                    customName = "sound_info";
                    if (platform == Platform.Xbox)
                        dataSize = 0xC;
                    else if (platform == Platform.PS2)
                        dataSize = 0x8;
                    else if (platform == Platform.GameCube)
                    {
                        if (game == Game.Scooby)
                            dataSize = 0xC;
                        else if (game == Game.BFBB)
                            dataSize = 0x10;
                        else if (game == Game.Incredibles)
                            dataSize = 0x20;
                    }
                    newAssetType = AssetType.SNDI;
                    break;
                case AssetTemplate.Text:
                    dataSize = 8;
                    newAssetType = AssetType.TEXT;
                    break;
                case AssetTemplate.Timer:
                case AssetTemplate.Checkpoint_Timer:
                    dataSize = 0x10;
                    newAssetType = AssetType.TIMR;
                    break;
                case AssetTemplate.Checkpoint:
                    customName = "CHECKPOINT_TRIG";
                    dataSize = 0x94 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.TRIG;
                    break;
                case AssetTemplate.Box_Trigger:
                case AssetTemplate.Sphere_Trigger:
                case AssetTemplate.Cylinder_Trigger:
                case AssetTemplate.BusStop_Trigger:
                case AssetTemplate.Checkpoint_Invisible:
                    dataSize = 0x94 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.TRIG;
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
                    dataSize = 0x6C + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.VIL;
                    break;
                case AssetTemplate.Chuck_Trigger:
                case AssetTemplate.Monsoon_Trigger:
                case AssetTemplate.Slick_Trigger:
                    dataSize = 0x6C + Asset.DataSizeOffset(game);
                    customName = template.ToString().ToUpper().Replace("_TRIGGER", "");
                    newAssetType = AssetType.VIL;
                    break;
                case AssetTemplate.UI_Generic:
                    dataSize = 0x80 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.UI;
                    break;
                case AssetTemplate.UIFT_Generic:
                    dataSize = 0xA4 + Asset.DataSizeOffset(game);
                    newAssetType = AssetType.UIFT;
                    break;
                default:
                    if (template != AssetTemplate.Null)
                        MessageBox.Show("Unsupported asset template");
                    success = false;
                    return 0;
            }

            string assetName = string.IsNullOrWhiteSpace(customName) ? template.ToString().ToUpper() + "_01" : customName;
            
            Section_AHDR newAsset = new Section_AHDR
            {
                assetType = newAssetType,
                flags = AHDRFlagsFromAssetType(newAssetType),
                ADBG = new Section_ADBG(0, assetName, "", 0),
                data = new byte[dataSize]
            };

            Asset asset = GetFromAssetID(AddAssetWithUniqueID(layerIndex, newAsset, giveIDregardless: true));

            success = true;

            if (asset is BaseAsset oa)
                oa.BaseUshortFlags = 0x1D;
            if (asset is EntityAsset placeableAsset)
            {
                placeableAsset.Data[0x8] = 0x01; // visible
                placeableAsset.Data[0xB] = 0x02; // solid

                placeableAsset.PositionX = position.X;
                placeableAsset.PositionY = position.Y;
                placeableAsset.PositionZ = position.Z;

                placeableAsset.ScaleX = 1f;
                placeableAsset.ScaleY = 1f;
                placeableAsset.ScaleZ = 1f;

                placeableAsset.ColorRed = 1f;
                placeableAsset.ColorGreen = 1f;
                placeableAsset.ColorBlue = 1f;
                placeableAsset.ColorAlpha = 1f;
                placeableAsset.ColorAlphaSpeed = 255f;

                if (asset is AssetBUTN butn)
                {
                    butn.AssetType = ObjectAssetType.BUTN;
                }
                else if (asset is AssetTRIG trig)
                {
                    trig.AssetType = ObjectAssetType.TRIG;
                    trig.Data[0xB] = 0;

                    trig.ColorAlpha = 0;
                    trig.ColorAlphaSpeed = 0;

                    trig.Data[0x88] = 0x80;
                    trig.DirectionZ = 1f;
                }
                else if (asset is AssetPKUP pkup)
                {
                    pkup.AssetType = ObjectAssetType.PKUP;
                    if (game == Game.BFBB)
                        pkup.Model_AssetID = "pickups.MINF";
                    else if (game == Game.Incredibles)
                        pkup.Model_AssetID = 0x94E25463;
                }
                else if (asset is AssetSIMP simp)
                {
                    simp.AssetType = ObjectAssetType.SIMP;
                    simp.AnimSpeed = 1f;
                    simp.CollTypeByte = 2;
                }
                else if (asset is AssetVIL vil)
                {
                    vil.AssetType = ObjectAssetType.VIL;
                }
            }

            switch (template)
            {
                case AssetTemplate.Boulder_Generic:
                    ((AssetBOUL)asset).AssetType = ObjectAssetType.BOUL;
                    ((AssetBOUL)asset).Data[0xB] = 0;
                    ((AssetBOUL)asset).ColorAlpha = 0;
                    ((AssetBOUL)asset).ColorAlphaSpeed = 0;
                    break;
                case AssetTemplate.Camera:
                    ((AssetCAM)asset).AssetType = ObjectAssetType.CAM;
                    ((AssetCAM)asset).PositionX = position.X;
                    ((AssetCAM)asset).PositionY = position.Y;
                    ((AssetCAM)asset).PositionZ = position.Z;
                    ((AssetCAM)asset).OffsetStartFrames = 30;
                    ((AssetCAM)asset).OffsetEndFrames = 45;
                    ((AssetCAM)asset).FieldOfView = 100;
                    ((AssetCAM)asset).Data[0x78] = 0;
                    ((AssetCAM)asset).Data[0x79] = 1;
                    ((AssetCAM)asset).Data[0x7A] = 1;
                    ((AssetCAM)asset).Data[0x7C] = 0xC0;
                    ((AssetCAM)asset).CamType = CamType.Static;
                    break;
                case AssetTemplate.Counter:
                    ((AssetCNTR)asset).AssetType = ObjectAssetType.CNTR;
                    break;
                case AssetTemplate.Conditional:
                    ((BaseAsset)asset).AssetType = ObjectAssetType.COND;
                    break;
                case AssetTemplate.Dispatcher:
                    ((AssetDPAT)asset).AssetType = ObjectAssetType.DPAT;
                    break;
                case AssetTemplate.Destructible_Generic:
                    ((AssetDSTR)asset).AssetType = ObjectAssetType.DSTR;
                    ((AssetDSTR)asset).CollType = 2;
                    ((AssetDSTR)asset).BlastRadius = 4f;
                    ((AssetDSTR)asset).BlastStrength = 1f;
                    break;
                case AssetTemplate.ElectricArc_Generic:
                    ((AssetEGEN)asset).AssetType = ObjectAssetType.EGEN;
                    ((AssetEGEN)asset).OnAnim_AssetID = 0xCE7F8131;
                    break;
                case AssetTemplate.Environment:
                    ((AssetENV)asset).AssetType = ObjectAssetType.ENV;
                    ((AssetENV)asset).StartCameraAssetID = "STARTCAM";
                    ((AssetENV)asset).LoldHeight = 10f;
                    break;
                case AssetTemplate.Group:
                    ((AssetGRUP)asset).AssetType = ObjectAssetType.GRUP;
                    break;
                case AssetTemplate.Pendulum_Generic:
                    ((AssetPEND)asset).AssetType = ObjectAssetType.PEND;
                    break;
                case AssetTemplate.Hangable_Generic:
                    ((AssetPEND)asset).AssetType = ObjectAssetType.HANG;
                    break;
                case AssetTemplate.UI_Generic:
                case AssetTemplate.UIFT_Generic:
                    ((AssetUI)asset).AssetType = ObjectAssetType.UI;
                    ((AssetUI)asset).TextCoordTopRightX = 1f;
                    ((AssetUI)asset).TextCoordBottomRightX = 1f;
                    ((AssetUI)asset).TextCoordBottomRightY = 1f;
                    ((AssetUI)asset).TextCoordBottomLeftY = 1f;
                    if (template == AssetTemplate.UIFT_Generic)
                    {
                        ((AssetUIFT)asset).AssetType = ObjectAssetType.UIFT;
                        ((AssetUIFT)asset).BackgroundColor = new AssetEditorColors.MyColor();
                        ((AssetUIFT)asset).FontColor = new AssetEditorColors.MyColor(255, 255, 255, 255);
                        ((AssetUIFT)asset).Inset_90 = 2;
                        ((AssetUIFT)asset).Inset_92 = 2;
                        ((AssetUIFT)asset).Inset_94 = 2;
                        ((AssetUIFT)asset).Inset_96 = 2;
                        ((AssetUIFT)asset).Space_98 = 24;
                        ((AssetUIFT)asset).Space_9A = 24;
                        ((AssetUIFT)asset).Cdim_9C = 24;
                        ((AssetUIFT)asset).Cdim_9E = 24;
                    }
                    break;
                case AssetTemplate.LightEmitter_Generic:
                    ((AssetLITE)asset).AssetType = ObjectAssetType.LITE;
                    ((AssetLITE)asset).PositionX = position.X;
                    ((AssetLITE)asset).PositionY = position.Y;
                    ((AssetLITE)asset).PositionZ = position.Z;
                    break;
                case AssetTemplate.Platform_Generic:
                    ((AssetPLAT)asset).AssetType = ObjectAssetType.PLAT;
                    ((AssetPLAT)asset).PlatformType = PlatType.Mechanism;
                    ((AssetPLAT)asset).PlatFlagsShort = 4;
                    ((AssetPLAT)asset).PlatSpecific = new PlatSpecific_Generic((AssetPLAT)asset);
                    break;
                case AssetTemplate.Player_Generic:
                    ((AssetPLYR)asset).AssetType = ObjectAssetType.PLYR;
                    ((AssetPLYR)asset).BaseUshortFlags = 0x0D;
                    ((AssetPLYR)asset).Data[0xB] = 0;
                    ((AssetPLYR)asset).ColorAlpha = 0;
                    ((AssetPLYR)asset).ColorAlphaSpeed = 0;
                    ((AssetPLYR)asset).Model_AssetID = 0x003FE4D5;
                    break;
                case AssetTemplate.Marker:
                    ((AssetMRKR)asset).PositionX = position.X;
                    ((AssetMRKR)asset).PositionY = position.Y;
                    ((AssetMRKR)asset).PositionZ = position.Z;
                    break;
                case AssetTemplate.Portal:
                    ((AssetPORT)asset).AssetType = ObjectAssetType.PORT;
                    ((AssetPORT)asset).Camera_AssetID = "STARTCAM";
                    ((AssetPORT)asset).DestinationLevel = "AA00";
                    break;
                case AssetTemplate.Script:
                    ((AssetSCRP)asset).AssetType = ObjectAssetType.SCRP;
                    ((AssetSCRP)asset).UnknownFloat08 = 1f;
                    break;
                case AssetTemplate.SoundGroup:
                    ((AssetSGRP)asset).AssetType = ObjectAssetType.SGRP;
                    ((AssetSGRP)asset).UnknownByte0E = 0x30;
                    ((AssetSGRP)asset).UnknownByte0F = 0x80;
                    ((AssetSGRP)asset).UnknownByte13 = 0x42;
                    ((AssetSGRP)asset).InnerRadius = 8f;
                    ((AssetSGRP)asset).OuterRadius = 25f;
                    ((AssetSGRP)asset).SGRP_Entries = new EntrySGRP[] { new EntrySGRP() };
                    break;
                case AssetTemplate.SoundInfo:
                    if (asset is AssetSNDI_GCN_V1 sndi)
                        sndi.Padding = 0xCDCDCDCD;
                    break;
                case AssetTemplate.Timer:
                    ((AssetTIMR)asset).AssetType = ObjectAssetType.TIMR;
                    break;
                case AssetTemplate.SFX_OnEvent:
                case AssetTemplate.SFX_OnRadius:
                    ((AssetSFX)asset).AssetType = ObjectAssetType.SFX;
                    ((AssetSFX)asset).PositionX = position.X;
                    ((AssetSFX)asset).PositionY = position.Y;
                    ((AssetSFX)asset).PositionZ = position.Z;
                    if (template == AssetTemplate.SFX_OnRadius)
                    {
                        ((AssetSFX)asset).Data[0x8] = 0x03;
                        ((AssetSFX)asset).Data[0x9] = 0xE6;
                        ((AssetSFX)asset).LinksBFBB = new LinkBFBB[]
                        {
                            new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                            {
                                Arguments_Float = new float[4],
                                TargetAssetID = asset.AHDR.assetID,
                                EventReceiveID = EventBFBB.ScenePrepare,
                                EventSendID = EventBFBB.Play
                            }
                        };
                    }
                    else
                    {
                        ((AssetSFX)asset).Data[0x8] = 0x01;
                        ((AssetSFX)asset).Data[0x9] = 0xC0;
                    }
                    ((AssetSFX)asset).MinFrequency = 1f;
                    ((AssetSFX)asset).Priority = 128;
                    ((AssetSFX)asset).Volume = 91;
                    ((AssetSFX)asset).InnerRadius = 5f;
                    ((AssetSFX)asset).OuterRadius = 10f;
                    break;
                case AssetTemplate.SDFX:
                    ((AssetSDFX)asset).AssetType = ObjectAssetType.SDFX;
                    ((AssetSDFX)asset).PositionX = position.X;
                    ((AssetSDFX)asset).PositionY = position.Y;
                    ((AssetSDFX)asset).PositionZ = position.Z;
                    break;
                case AssetTemplate.Shiny_Red:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x3E;
                    ((AssetPKUP)asset).PickReferenceID = 0x7C8AC53E;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Yellow:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x3B;
                    ((AssetPKUP)asset).PickReferenceID = 0xB3D6283B;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Green:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x34;
                    ((AssetPKUP)asset).PickReferenceID = 0x079A0734;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Blue:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x81;
                    ((AssetPKUP)asset).PickReferenceID = 0x6D4A4181;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Shiny_Purple:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0xCB;
                    ((AssetPKUP)asset).PickReferenceID = 0xFA607BCB;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Underwear:
                    ((AssetPKUP)asset).Shape = 0x13;
                    ((AssetPKUP)asset).PickReferenceID = 0x28F55613;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Spatula:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0xDD;
                    ((AssetPKUP)asset).PickReferenceID = 0x8BDFE8DD;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Sock:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0x24;
                    ((AssetPKUP)asset).PickReferenceID = 0x74B46F24;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Spongeball:
                    ((AssetPKUP)asset).Shape = 0x15;
                    ((AssetPKUP)asset).PickReferenceID = 0xF09A1415;
                    ((AssetPKUP)asset).PickupFlagsShort = 3;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Golden_Underwear:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0x2E;
                    ((AssetPKUP)asset).PickReferenceID = 0xF650DA2E;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Artwork:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0x10;
                    ((AssetPKUP)asset).PickReferenceID = 0x18140B10;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.SteeringWheel:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0x32;
                    ((AssetPKUP)asset).PickReferenceID = 0x4C67C832;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.PowerCrystal:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0xBB;
                    ((AssetPKUP)asset).PickReferenceID = 0xFE7A89BB;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Smelly_Sundae:
                    ((AssetPKUP)asset).Shape = 0x54;
                    ((AssetPKUP)asset).PickReferenceID = 0x6A779454;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
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
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Hammer:
                    ((AssetVIL)asset).Model_AssetID = "ham_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.ham_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.TarTar:
                    ((AssetVIL)asset).Model_AssetID = "robot_tar_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_tar_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.ChompBot:
                    ((AssetVIL)asset).Model_AssetID = "robot_0a_chomper_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_0a_chomper_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.GLove:
                    ((AssetVIL)asset).Model_AssetID = "g_love_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.g_love_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Chuck:
                    ((AssetVIL)asset).Model_AssetID = "robot_chuck_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Chuck_Trigger:
                    ((AssetVIL)asset).Model_AssetID = "robot_chuck_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };
                    AssetTRIG chuckTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.Sphere_Trigger));
                    chuckTrigger.Radius = 15f;
                    chuckTrigger.LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.DetectPlayerOn
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
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
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Monsoon_Trigger:

                    ((AssetVIL)asset).Model_AssetID = "robot_4a_monsoon_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_4a_monsoon_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG monsoonTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.Sphere_Trigger));
                    monsoonTrigger.Radius = 15f;
                    monsoonTrigger.LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.DetectPlayerOn
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
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
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Sleepytime:
                    ((AssetVIL)asset).Model_AssetID = "robot_sleepy-time_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_sleepytime_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    ((AssetMVPT)GetFromAssetID(((AssetVIL)asset).MovePoint_AssetID)).ZoneRadius = -1;
                    break;
                case AssetTemplate.Arf:
                    ((AssetVIL)asset).Model_AssetID = "robot_arf_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_arf_bind;
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_DOGA", AssetTemplate.ArfDog),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_DOGB", AssetTemplate.ArfDog),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
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
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Tubelet:
                    ((AssetVIL)asset).Model_AssetID = "tubelet_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.tubelet_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SLAVEA", AssetTemplate.TubeletSlave),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
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
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Slick:
                    ((AssetVIL)asset).Model_AssetID = "robot_9a_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Slick_Trigger:
                    ((AssetVIL)asset).Model_AssetID = "robot_9a_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG slickTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.Sphere_Trigger));
                    slickTrigger.Radius = 15f;
                    slickTrigger.LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.DetectPlayerOn
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
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
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Jellyfish_Blue:
                    ((AssetVIL)asset).Model_AssetID = "jellyfish_blue_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.jellyfish_blue_bind;
                    ((AssetVIL)asset).MovePoint_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT);
                    break;
                case AssetTemplate.Duplicatotron:
                    ((AssetVIL)asset).Model_AssetID = "duplicatotron1000_bind.MINF";
                    ((AssetVIL)asset).VilType = VilType.duplicatotron1000_bind;
                    ((AssetVIL)asset).NPCSettings_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_SETTINGS", AssetTemplate.DuplicatotronSettings);
                    ((AssetVIL)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_GROUP", AssetTemplate.Group),
                            EventReceiveID = EventBFBB.ScenePrepare,
                            EventSendID = EventBFBB.Connect_IOwnYou
                        }
                    };
                    break;
                case AssetTemplate.DuplicatotronSettings:
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__NPCSettings;
                    ((AssetDYNA)asset).DynaBase = new DynaNPCSettings(platform)
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
                    ((AssetBUTN)asset).Motion = game == Game.Incredibles ?
                        new Motion_Mechanism_TSSM((AssetBUTN)asset)
                        {
                            Flags = 4,
                            MovementType = Motion_Mechanism.EMovementType.SlideAndRotate,
                            MovementLoopModeByte = 1,
                            SlideAxis = Motion_Mechanism.Axis.Y,
                            SlideDistance = -0.2f,
                            SlideTime = 0.5f,
                            SlideDecelTime = 0.2f
                        } :
                        new Motion_Mechanism((AssetBUTN)asset)
                        {
                            Flags = 4,
                            MovementType = Motion_Mechanism.EMovementType.SlideAndRotate,
                            MovementLoopModeByte = 1,
                            SlideAxis = Motion_Mechanism.Axis.Y,
                            SlideDistance = -0.2f,
                            SlideTime = 0.5f,
                            SlideDecelTime = 0.2f
                        };

                    ((AssetBUTN)asset).HitMaskUint =
                        (uint)Math.Pow(2, 0) |
                        (uint)Math.Pow(2, 3) |
                        (uint)Math.Pow(2, 4) |
                        (uint)Math.Pow(2, 6) |
                        (uint)Math.Pow(2, 7) |
                        (uint)Math.Pow(2, 14) |
                        (uint)Math.Pow(2, 15);
                    break;
                case AssetTemplate.PressurePlate:
                    ((AssetBUTN)asset).ActMethod = AssetBUTN.ButnActMethod.PressurePlate;
                    ((AssetBUTN)asset).Model_AssetID = "plate_pressure";
                    ((AssetBUTN)asset).PressedModel_AssetID = 0xCE7F8131;
                    ((AssetBUTN)asset).Motion = game == Game.Incredibles ?
                    new Motion_Mechanism_TSSM((AssetBUTN)asset)
                    {
                        Flags = 4,
                        MovementType = Motion_Mechanism.EMovementType.SlideAndRotate,
                        MovementLoopModeByte = 1,
                        SlideAxis = Motion_Mechanism.Axis.Y,
                        SlideDistance = -0.15f,
                        SlideTime = 0.15f,
                    } :
                    new Motion_Mechanism((AssetBUTN)asset)
                    {
                        Flags = 4,
                        MovementType = Motion_Mechanism.EMovementType.SlideAndRotate,
                        MovementLoopModeByte = 1,
                        SlideAxis = Motion_Mechanism.Axis.Y,
                        SlideDistance = -0.15f,
                        SlideTime = 0.15f,
                    };
                    ((AssetBUTN)asset).HitMaskUint =
                        (uint)Math.Pow(2, 10) |
                        (uint)Math.Pow(2, 12) |
                        (uint)Math.Pow(2, 13) |
                        (uint)Math.Pow(2, 16);

                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_BASE", AssetTemplate.PressurePlateBase);
                    break;
                case AssetTemplate.PressurePlateBase:
                    ((AssetSIMP)asset).Model_AssetID = "plate_pressure_base";
                    break;
                case AssetTemplate.TaxiStand:
                    ((AssetSIMP)asset).Model_AssetID = "taxi_stand";
                    break;
                case AssetTemplate.TexasHitch:
                case AssetTemplate.Swinger:
                    ((AssetSIMP)asset).Model_AssetID = "trailer_hitch";
                    break;
                case AssetTemplate.TexasHitch_PLAT:
                case AssetTemplate.Swinger_PLAT:
                    ((AssetPLAT)asset).AssetType = ObjectAssetType.PLAT;
                    ((AssetPLAT)asset).Model_AssetID = "trailer_hitch";
                    ((AssetPLAT)asset).PlatSpecific = new PlatSpecific_Generic((AssetPLAT)asset);
                    ((AssetPLAT)asset).Motion = new Motion_Mechanism((AssetPLAT)asset);
                    break;
                case AssetTemplate.Area_MVPT:
                    ((AssetMVPT_Scooby)asset).AssetType = ObjectAssetType.MVPT;
                    ((AssetMVPT_Scooby)asset).PositionX = position.X;
                    ((AssetMVPT_Scooby)asset).PositionY = position.Y;
                    ((AssetMVPT_Scooby)asset).PositionZ = position.Z;
                    ((AssetMVPT_Scooby)asset).Wt = 0x2710;
                    ((AssetMVPT_Scooby)asset).IsZone = 0x00;
                    ((AssetMVPT_Scooby)asset).BezIndex = 0x00;
                    if (asset is AssetMVPT)
                    {
                        ((AssetMVPT)asset).Delay = 360;
                        ((AssetMVPT)asset).ZoneRadius = 4;
                        ((AssetMVPT)asset).ArenaRadius = 8;
                    }
                    else
                        ((AssetMVPT_Scooby)asset).ArenaRadius = 8;
                    break;
                case AssetTemplate.Point_MVPT:
                    ((AssetMVPT_Scooby)asset).AssetType = ObjectAssetType.MVPT;
                    ((AssetMVPT_Scooby)asset).PositionX = position.X;
                    ((AssetMVPT_Scooby)asset).PositionY = position.Y;
                    ((AssetMVPT_Scooby)asset).PositionZ = position.Z;
                    ((AssetMVPT_Scooby)asset).Wt = 0x2710;
                    ((AssetMVPT_Scooby)asset).IsZone = 0x01;
                    ((AssetMVPT_Scooby)asset).BezIndex = 0x00;
                    if (asset is AssetMVPT)
                    {
                        if (game == Game.Incredibles)
                            ((AssetMVPT)asset).Delay = 2;
                        else
                            ((AssetMVPT)asset).Delay = 0;

                        ((AssetMVPT)asset).ZoneRadius = -1;
                        ((AssetMVPT)asset).ArenaRadius = -1;
                    }
                    else
                        ((AssetMVPT_Scooby)asset).ArenaRadius = -1;
                    if (chainPointMVPTs && ContainsAsset(chainPointMVPTlast))
                    {
                        var prev = ((AssetMVPT)GetFromAssetID(chainPointMVPTlast));
                        var nexts = prev.NextMVPTs.ToList();
                        nexts.Add(asset.AHDR.assetID);
                        prev.NextMVPTs = nexts.ToArray();
                    }
                    chainPointMVPTlast = asset.AHDR.assetID;
                    break;
                case AssetTemplate.Box_Trigger:
                    ((AssetTRIG)asset).Shape = TriggerShape.Box;
                    ((AssetTRIG)asset).Model_AssetID = 0x19A4D2E9;
                    ((AssetTRIG)asset).SetPositions(position.X + 5f, position.Y + 5f, position.Z + 5f, position.X - 5f, position.Y - 5f, position.Z - 5f);
                    break;
                case AssetTemplate.Sphere_Trigger:
                    ((AssetTRIG)asset).Shape = TriggerShape.Sphere;
                    ((AssetTRIG)asset).Model_AssetID = 0xDD77064D;
                    ((AssetTRIG)asset).Radius = 10f;
                    ((AssetTRIG)asset).Position0X = position.X;
                    ((AssetTRIG)asset).Position0Y = position.Y;
                    ((AssetTRIG)asset).Position0Z = position.Z;
                    break;
                case AssetTemplate.Cylinder_Trigger:
                    ((AssetTRIG)asset).Shape = TriggerShape.Cylinder;
                    ((AssetTRIG)asset).Model_AssetID = 0xDD77064D;
                    ((AssetTRIG)asset).Radius = 10f;
                    ((AssetTRIG)asset).Height = 5f;
                    ((AssetTRIG)asset).Position0X = position.X;
                    ((AssetTRIG)asset).Position0Y = position.Y;
                    ((AssetTRIG)asset).Position0Z = position.Z;
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
                    ((AssetSIMP)asset).Data[0x8] = 0;
                    ((AssetSIMP)asset).Data[0xB] = 0;
                    ((AssetSIMP)asset).CollTypeByte = 0;
                    break;
                case AssetTemplate.BusStop_Trigger:
                    ((AssetTRIG)asset).AssetType = ObjectAssetType.TRIG;
                    ((AssetTRIG)asset).Shape = TriggerShape.Sphere;
                    ((AssetTRIG)asset).Position0X = position.X;
                    ((AssetTRIG)asset).Position0Y = position.Y;
                    ((AssetTRIG)asset).Position0Z = position.Z;
                    ((AssetTRIG)asset).Radius = 2.5f;
                    uint lightsAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper().Replace("TRIGGER", "LIGHTS").Replace("TRIG", "LIGHTS"), AssetTemplate.BusStop_Lights);
                    ((AssetTRIG)asset).LinksBFBB = new LinkBFBB[] {
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = lightsAssetID,
                            EventReceiveID = EventBFBB.EnterPlayer,
                            EventSendID = EventBFBB.Visible
                        },
                        new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                        {
                            Arguments_Float = new float[4],
                            TargetAssetID = lightsAssetID,
                            EventReceiveID = EventBFBB.ExitPlayer,
                            EventSendID = EventBFBB.Invisible
                        }
                    };
                    break;
                case AssetTemplate.BusStop_DYNA:
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__BusStop;
                    ((AssetDYNA)asset).DynaBase = new DynaBusStop(platform)
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
                    ((AssetCAM)asset).Data[0x78] = 00;
                    ((AssetCAM)asset).Data[0x79] = 01;
                    ((AssetCAM)asset).Data[0x7A] = 01;
                    ((AssetCAM)asset).Data[0x7B] = 0x8F;
                    ((AssetCAM)asset).CamType = CamType.Static;
                    break;
                case AssetTemplate.BusStop_BusSimp:
                    ((AssetSIMP)asset).PositionX -= 3f;
                    ((AssetSIMP)asset).Data[0x8] = 0;
                    ((AssetSIMP)asset).Data[0xB] = 0;
                    ((AssetSIMP)asset).CollTypeByte = 0;
                    ((AssetSIMP)asset).Model_AssetID = "bus_bind";
                    ((AssetSIMP)asset).Animation_AssetID = "BUSSTOP_ANIMLIST_01";
                    break;
                case AssetTemplate.TeleportBox:
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__Teleport;
                    ((AssetDYNA)asset).DynaBase = new DynaTeleport_BFBB(platform, 2)
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
                    ((AssetSIMP)asset).CollTypeByte = 0;
                    break;
                case AssetTemplate.Checkpoint:
                case AssetTemplate.Checkpoint_Invisible:
                    {
                        ((AssetTRIG)asset).AssetType = ObjectAssetType.TRIG;
                        ((AssetTRIG)asset).Shape = TriggerShape.Sphere;
                        ((AssetTRIG)asset).Radius = 6f;
                        ((AssetTRIG)asset).Position0X = position.X;
                        ((AssetTRIG)asset).Position0Y = position.Y;
                        ((AssetTRIG)asset).Position0Z = position.Z;

                        AssetID checkpointDisp = "CHECKPOINT_DISP_00";
                        if (!ContainsAsset(checkpointDisp))
                            checkpointDisp = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "CHECKPOINT_DISP", AssetTemplate.Dispatcher);

                        List<LinkBFBB> events = new List<LinkBFBB>
                        {
                            new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                            {
                                Arguments_Float = new float[4],
                                ArgumentAssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "CHECKPOINT_MRKR", AssetTemplate.Marker),
                                TargetAssetID = checkpointDisp,
                                EventReceiveID = EventBFBB.EnterPlayer,
                                EventSendID = EventBFBB.SetCheckPoint
                            }
                        };

                        if (template == AssetTemplate.Checkpoint)
                            events.Add(new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
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
                        ((AssetTIMR)asset).AssetType = ObjectAssetType.TIMR;
                        ((AssetTIMR)asset).Time = 0.5f;
                        uint checkpointSimp = PlaceTemplate(new Vector3(position.X + 2f, position.Y, position.Z), layerIndex, out success, ref assetIDs, "CHECKPOINT_SIMP", AssetTemplate.Checkpoint_SIMP);
                        uint checkpointTalkbox = BKDRHash("CHECKPOINT_TALKBOX_00");
                        if (!ContainsAsset(checkpointTalkbox))
                            checkpointTalkbox = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "CHECKPOINT_TALKBOX", AssetTemplate.Checkpoint_Talkbox);
                        ((AssetTIMR)asset).LinksBFBB = new LinkBFBB[] {
                            new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                            {
                                Arguments_Float = new float[] { 2, 0, 0, 0 },
                                TargetAssetID = checkpointSimp,
                                EventReceiveID = EventBFBB.Run,
                                EventSendID = EventBFBB.AnimPlay
                            },
                            new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                            {
                                Arguments_Hex = new AssetID[] { "checkpoint_text", 0, 0, 0 },
                                TargetAssetID = checkpointTalkbox,
                                EventReceiveID = EventBFBB.Run,
                                EventSendID = EventBFBB.StartConversation
                            },
                            new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
                            {
                                Arguments_Float = new float[] { 3, 0, 0, 0 },
                                TargetAssetID = checkpointSimp,
                                EventReceiveID = EventBFBB.Expired,
                                EventSendID = EventBFBB.AnimPlayLoop
                            },
                            new LinkBFBB(EndianConverter.PlatformEndianness(platform), false)
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
                    ((AssetDYNA)asset).Version = 11;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__talk_box;
                    ((AssetDYNA)asset).DynaBase = new DynaTalkBox(platform)
                    {
                        Dialog_TextBoxID = 0x9BC49154,
                        AutoWaitTypeTime = 1,
                        AutoWaitDelay = 2f
                    };
                    break;
                case AssetTemplate.Springboard:
                    ((AssetPLAT)asset).AssetType = ObjectAssetType.PLAT;
                    ((AssetPLAT)asset).Model_AssetID = 0x55E9EAB5;
                    ((AssetPLAT)asset).Animation_AssetID = 0x7AAA99BB;
                    ((AssetPLAT)asset).PlatformType = PlatType.Springboard;
                    ((AssetPLAT)asset).PlatFlagsShort = 4;
                    ((AssetPLAT)asset).PlatSpecific = new PlatSpecific_Springboard((AssetPLAT)asset)
                    {
                        Height1 = 10,
                        Height2 = 10,
                        Height3 = 10,
                        HeightBubbleBounce = 10,
                        Anim1_AssetID = 0x6DAE0759,
                        Anim2_AssetID = 0xBC4A9A5F,
                        DirectionY = 1f,
                    };
                    break;
                case AssetTemplate.HoveringPlatform:
                    ((AssetPLAT)asset).AssetType = ObjectAssetType.PLAT;
                    ((AssetPLAT)asset).Model_AssetID = 0x335EE0C8;
                    ((AssetPLAT)asset).Animation_AssetID = 0x730847B6;
                    ((AssetPLAT)asset).PlatformType = PlatType.Mechanism;
                    ((AssetPLAT)asset).PlatFlagsShort = 4;
                    ((AssetPLAT)asset).Motion = game == Game.Incredibles ?
                    new Motion_Mechanism_TSSM((AssetPLAT)asset)
                    {
                        Type = MotionType.Other,
                        MovementLoopModeByte = 1,
                        SlideAccelTime = 0.4f,
                        SlideDecelTime = 0.4f
                    } :
                    new Motion_Mechanism((AssetBUTN)asset)
                    {
                        Type = MotionType.Other,
                        MovementLoopModeByte = 1,
                        SlideAccelTime = 0.4f,
                        SlideDecelTime = 0.4f
                    };
                    break;
                case AssetTemplate.BungeeHook:
                    ((AssetDYNA)asset).Version = 13;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__bungee_hook;
                    ((AssetDYNA)asset).DynaBase = new DynaBungeeHook(platform)
                    {
                        Placeable_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "BUNGEE_SIMP", AssetTemplate.BungeeHook_SIMP),
                        AttachDist = 3,
                        AttachTravelTime = 0.5f,
                        DetachDist = 10,
                        DetachFreeFallTime = 1,
                        DetachAccel = 2,
                        TurnUnused1 = 25,
                        TurnUnused2 = 0.95f,
                        VerticalFrequency = 2,
                        VerticalGravity = 9.8f,
                        VerticalDive = 2,
                        VerticalMinDist = 2,
                        VerticalMaxDist = 40,
                        VerticalDamp = 0.05f,
                        HorizontalMaxDist = 2,
                        CameraRestDist = 5,
                        Cameraview_angle = 220,
                        CameraOffset = 0.5f,
                        CameraOffsetDir = 180,
                        CameraTurnSpeed = 0.05f,
                        CameraVelScale = 0,
                        CameraRollSpeed = 0.05f,
                        CameraUnused1_X = 0.2f,
                        CameraUnused1_Y = 0.25f,
                        CameraUnused1_Z = 0.2f,
                        CollisionHitLoss = 0.1f,
                        CollisionDamageVelocity = 0.6f,
                        CollisionHitVelocity = 0.2f,
                    };
                    break;
                case AssetTemplate.BungeeHook_SIMP:
                    ((AssetSIMP)asset).Model_AssetID = "bungee_hook";
                    ((AssetSIMP)asset).CollTypeByte = 0;
                    break;
                case AssetTemplate.BungeeDrop:
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__bungee_drop;
                    ((AssetDYNA)asset).DynaBase = new DynaBungeeDrop(platform)
                    {
                        MRKR_ID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, "BUNGEE_MRKR", AssetTemplate.Marker),
                        SetViewAngle = 1,
                    };
                    break;
                case AssetTemplate.Dyna_Pointer:
                    ((AssetDYNA)asset).Version = 1;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.pointer;
                    ((AssetDYNA)asset).DynaBase = new DynaPointer(platform)
                    {
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z
                    };
                    break;
                case AssetTemplate.Manliness_Red:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x17;
                    ((AssetPKUP)asset).PickReferenceID = 0x7C134517;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Manliness_Yellow:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x5A;
                    ((AssetPKUP)asset).PickReferenceID = 0xFA454C5A;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Manliness_Green:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0xD9;
                    ((AssetPKUP)asset).PickReferenceID = 0xA869F4D9;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Manliness_Blue:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x4C;
                    ((AssetPKUP)asset).PickReferenceID = 0x7BB95F4C;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Manliness_Purple:
                    ((AssetPKUP)asset).BaseUshortFlags |= (ushort)(persistentShinies ? 2 : 0);
                    ((AssetPKUP)asset).Shape = 0x8A;
                    ((AssetPKUP)asset).PickReferenceID = 0x3C48E68A;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.KrabbyPatty:
                    ((AssetPKUP)asset).Shape = 0xD1;
                    ((AssetPKUP)asset).PickReferenceID = 0xACC4FBD1;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.GoofyGooberToken:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0xB7;
                    ((AssetPKUP)asset).PickReferenceID = 0x60F808B7;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.TreasureChest:
                    ((AssetPKUP)asset).BaseUshortFlags |= 2;
                    ((AssetPKUP)asset).Shape = 0x8A;
                    ((AssetPKUP)asset).PickReferenceID = 0xA613E48A;
                    ((AssetPKUP)asset).PickupFlagsShort = 2;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Nitro:
                    ((AssetPKUP)asset).Shape = 0x1A;
                    ((AssetPKUP)asset).PickReferenceID = 0x630BD71A;
                    ((AssetPKUP)asset).PickupFlagsShort = 3;
                    ((AssetPKUP)asset).PickupValue = 4;
                    ((AssetPKUP)asset).PositionY += 0.5f;
                    break;
                case AssetTemplate.Wood_Crate:
                case AssetTemplate.Hover_Crate:
                case AssetTemplate.Explode_Crate:
                case AssetTemplate.Shrink_Crate:
                case AssetTemplate.Steel_Crate:
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_TSSM = DynaType_TSSM.Enemy__SB__SupplyCrate;
                    ((AssetDYNA)asset).DynaBase = new DynaSupplyCrate(platform)
                    {
                        VisibilityFlag = 1,
                        SolidityFlag = 1,
                        Flags06 = 0x1D,
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f,
                        ColorRed = 1f,
                        ColorGreen = 1f,
                        ColorBlue = 1f,
                        ColorAlpha = 1f,
                        Type =
                        template == AssetTemplate.Wood_Crate ? EnemySupplyCrateType.crate_wood_bind :
                        template == AssetTemplate.Hover_Crate ? EnemySupplyCrateType.crate_hover_bind :
                        template == AssetTemplate.Explode_Crate ? EnemySupplyCrateType.crate_explode_bind :
                        template == AssetTemplate.Shrink_Crate ? EnemySupplyCrateType.crate_shrink_bind :
                        template == AssetTemplate.Steel_Crate ? EnemySupplyCrateType.crate_steel_bind : 0
                    };
                    break;
                case AssetTemplate.Jelly_Critter:
                case AssetTemplate.Jelly_Bucket:
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_TSSM = DynaType_TSSM.Enemy__SB__Critter;
                    ((AssetDYNA)asset).BaseUshortFlags = 0x0D;
                    ((AssetDYNA)asset).DynaBase = new DynaEnemyCritter(platform)
                    {
                        VisibilityFlag = 1,
                        SolidityFlag = 1,
                        Flags06 = 0x1D,
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f,
                        ColorRed = 1f,
                        ColorGreen = 1f,
                        ColorBlue = 1f,
                        ColorAlpha = 1f,
                        MVPT_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Point_MVPT),
                        Type =
                        template == AssetTemplate.Jelly_Critter ? EnemyCritterType.jellyfish_v1_bind :
                        template == AssetTemplate.Jelly_Bucket ? EnemyCritterType.jellybucket_v1_bind : 0
                    };
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
                    ((AssetDYNA)asset).Version = 7;
                    ((AssetDYNA)asset).Type_TSSM = DynaType_TSSM.Enemy__SB__Standard;
                    ((AssetDYNA)asset).DynaBase = new DynaEnemyStandard(platform)
                    {
                        VisibilityFlag = 1,
                        SolidityFlag = 1,
                        Flags06 = 0x1D,
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f,
                        ColorRed = 1f,
                        ColorGreen = 1f,
                        ColorBlue = 1f,
                        ColorAlpha = 1f,
                        MVPT_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Area_MVPT),
                        Type =
                        template == AssetTemplate.Fogger_GoofyGoober ? EnemyStandardType.fogger_gg_bind :
                        template == AssetTemplate.Fogger_Desert ? EnemyStandardType.fogger_de_bind :
                        template == AssetTemplate.Fogger_ThugTug ? EnemyStandardType.fogger_tt_bind :
                        template == AssetTemplate.Fogger_Trench ? EnemyStandardType.fogger_tr_bind :
                        template == AssetTemplate.Fogger_Junkyard ? EnemyStandardType.fogger_jk_bind :
                        template == AssetTemplate.Fogger_Planktopolis ? EnemyStandardType.fogger_pt_bind :
                        template == AssetTemplate.Fogger_v1 ? EnemyStandardType.fogger_v1_bind :
                        template == AssetTemplate.Fogger_v2 ? EnemyStandardType.fogger_v2_bind :
                        template == AssetTemplate.Fogger_v3 ? EnemyStandardType.fogger_v3_bind :
                        template == AssetTemplate.Slammer_GoofyGoober ? EnemyStandardType.slammer_v1_bind :
                        template == AssetTemplate.Slammer_Desert ? EnemyStandardType.slammer_des_bind :
                        template == AssetTemplate.Slammer_ThugTug ? EnemyStandardType.slammer_v3_bind :
                        template == AssetTemplate.Spinner_ThugTug ? EnemyStandardType.spinner_v1_bind :
                        template == AssetTemplate.Spinner_Junkyard ? EnemyStandardType.spinner_v2_bind :
                        template == AssetTemplate.Spinner_Planktopolis ? EnemyStandardType.spinner_v3_bind :
                        template == AssetTemplate.Minimerv ? EnemyStandardType.minimerv_v1_bind :
                        template == AssetTemplate.Mervyn ? EnemyStandardType.mervyn_v3_bind : 0
                    };
                    break;
                case AssetTemplate.Flinger_Desert:
                case AssetTemplate.Flinger_Trench:
                case AssetTemplate.Flinger_Junkyard:
                case AssetTemplate.Popper_Trench:
                case AssetTemplate.Popper_Planktopolis:
                    ((AssetDYNA)asset).Version = 7;
                    ((AssetDYNA)asset).Type_TSSM = DynaType_TSSM.Enemy__SB__Standard;
                    ((AssetDYNA)asset).DynaBase = new DynaEnemyStandard(platform)
                    {
                        VisibilityFlag = 1,
                        SolidityFlag = 1,
                        Flags06 = 0x1D,
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f,
                        ColorRed = 1f,
                        ColorGreen = 1f,
                        ColorBlue = 1f,
                        ColorAlpha = 1f,
                        MVPT_AssetID = PlaceTemplate(position, layerIndex, out success, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.Point_MVPT),
                        Type =
                        template == AssetTemplate.Flinger_Desert ? EnemyStandardType.flinger_v1_bind :
                        template == AssetTemplate.Flinger_Trench ? EnemyStandardType.flinger_v2_bind :
                        template == AssetTemplate.Flinger_Junkyard ? EnemyStandardType.flinger_v3_bind :
                        template == AssetTemplate.Popper_Trench ? EnemyStandardType.popper_v1_bind :
                        template == AssetTemplate.Popper_Planktopolis ? EnemyStandardType.popper_v3_bind : 0
                    };
                    break;
                case AssetTemplate.Turret_v1:
                case AssetTemplate.Turret_v2:
                case AssetTemplate.Turret_v3:
                    ((AssetDYNA)asset).Version = 4;
                    ((AssetDYNA)asset).Type_TSSM = DynaType_TSSM.Enemy__SB__Turret;
                    ((AssetDYNA)asset).DynaBase = new DynaEnemyTurret(platform)
                    {
                        VisibilityFlag = 1,
                        SolidityFlag = 1,
                        Flags06 = 0x1D,
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f,
                        ColorRed = 1f,
                        ColorGreen = 1f,
                        ColorBlue = 1f,
                        ColorAlpha = 1f,
                        UnknownFloat50 = 30f,
                        UnknownInt58 = 1,
                        Type =
                        template == AssetTemplate.Turret_v1 ? EnemyTurretType.turret_v1_bind :
                        template == AssetTemplate.Turret_v2 ? EnemyTurretType.turret_v2_bind :
                        template == AssetTemplate.Turret_v3 ? EnemyTurretType.turret_v3_bind : 0
                    };
                    break;
                case AssetTemplate.Ring:
                    ((AssetDYNA)asset).Version = 2;
                    ((AssetDYNA)asset).Type_TSSM = DynaType_TSSM.game_object__Ring;
                    ((AssetDYNA)asset).DynaBase = new DynaRing(platform)
                    {
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z,
                        ScaleX = 1f,
                        ScaleY = 1f,
                        ScaleZ = 1f,
                        UnknownShadowFlag = 1,
                        CollisionRadius = 3.5f,
                        UnknownFloat1 = 4f,
                        UnknownFloat2 = 4f,
                        NormalTimer = 5f,
                        RedTimer = -1f
                    };
                    break;
                case AssetTemplate.RingControl:
                    ((AssetDYNA)asset).Version = 3;
                    ((AssetDYNA)asset).Type_TSSM = DynaType_TSSM.game_object__RingControl;
                    ((AssetDYNA)asset).DynaBase = new DynaRingControl(platform)
                    {
                        RingModel_AssetID = "test_ring",
                        UnknownInt1 = 40,
                        RingSoundGroup_AssetID = "RING_SGRP",
                        RingsAreVisible = 1,
                        Rings_AssetIDs = new AssetID[0]
                    };
                    break;
                case AssetTemplate.EmptyBSP:
                        asset.AHDR.data = GenerateBlankBSP();
                    break;
                case AssetTemplate.CamTweak:
                    ((AssetDYNA)asset).Version = 1;
                    ((AssetDYNA)asset).Type_BFBB = DynaType_BFBB.game_object__Camera_Tweak;
                    ((AssetDYNA)asset).DynaBase = new DynaCamTweak(platform);
                    break;
            }

            if (asset is AssetDYNA DYNA)
                DYNA.SetDynaSpecific(false);

            assetIDs.Add(asset.AHDR.assetID);

            return asset.AHDR.assetID;
        }

        private byte[] GenerateBlankBSP()
        {
            Vertex3 Max = new Vertex3(100000, 100000, 100000);
            Vertex3 Min = new Vertex3(-100000, -100000, -100000);

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
    }
}