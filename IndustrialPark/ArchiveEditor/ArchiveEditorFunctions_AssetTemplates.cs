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
        public static ToolStripMenuItem GetTemplateMenuItem(AssetTemplate template, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(GetName(template).Replace('_', ' ')) { Tag = template };
            item.Click += eventHandler;
            return item;
        }

        public static void PopulateTemplateMenusAt(ToolStripMenuItem menu, EventHandler eventHandler)
        {
            ToolStripMenuItem controllers = new ToolStripMenuItem("Stage Controllers");
            controllers.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Counter, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Conditional, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Dispatcher, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fog, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Flythrough, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Group, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Portal, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Progress_Script, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Script, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sound_Group, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Text, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Timer, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Cam_Tweak, eventHandler)
            });

            // BFBB
            ToolStripMenuItem pickupsBFBB = new ToolStripMenuItem("Pickups and Tikis");
            pickupsBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Shiny_Red, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shiny_Yellow, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shiny_Green, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shiny_Blue, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shiny_Purple, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Underwear, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spongeball, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Wooden_Tiki, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Floating_Tiki, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Thunder_Tiki, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shhh_Tiki, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Stone_Tiki, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Spatula, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sock, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Golden_Underwear, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Smelly_Sundae, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Steering_Wheel, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Artwork, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Power_Crystal, eventHandler),
            });

            ToolStripMenuItem enemiesBFBB = new ToolStripMenuItem("Enemies");
            enemiesBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Fodder, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Hammer, eventHandler),
                GetTemplateMenuItem(AssetTemplate.TarTar, eventHandler),
                GetTemplateMenuItem(AssetTemplate.ChompBot, eventHandler),
                GetTemplateMenuItem(AssetTemplate.GLove, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Chuck, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Chuck_Trigger, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Monsoon, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Monsoon_Trigger, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sleepytime, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sleepytime_Moving, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Arf, eventHandler),
                GetTemplateMenuItem(AssetTemplate.BombBot, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Tubelet, eventHandler),
                GetTemplateMenuItem(AssetTemplate.BzztBot, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Slick, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Slick_Trigger, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Jellyfish_Pink, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Jellyfish_Blue, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Duplicatotron, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Duplicatotron_Settings, eventHandler),
            });

            ToolStripMenuItem stageitemsBFBB = new ToolStripMenuItem("Stage Items");
            stageitemsBFBB.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Button_Red, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Pressure_Plate, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Checkpoint, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Checkpoint_Invisible, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Bus_Stop, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Teleport_Box, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Throw_Fruit, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Freezy_Fruit, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Texas_Hitch, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Texas_Hitch_Platform, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Bungee_Hook, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Bungee_Drop, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Hovering_Platform, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Springboard, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Taxi_Stand, eventHandler),
            });

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
                GetTemplateMenuItem(AssetTemplate.Manliness_Red, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Manliness_Yellow, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Manliness_Green, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Manliness_Blue, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Manliness_Purple, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Krabby_Patty, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Goofy_Goober_Token, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Treasure_Chest, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Nitro, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Wood_Crate, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Hover_Crate, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Explode_Crate, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shrink_Crate, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Steel_Crate, eventHandler),
            });

            ToolStripMenuItem enemiesTSSM = new ToolStripMenuItem("Enemies");
            enemiesTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Fogger_GoofyGoober, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_Desert, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_ThugTug, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_Trench, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_Junkyard, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_Planktopolis, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_v1, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_v2, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Fogger_v3, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Slammer_GoofyGoober, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Slammer_Desert, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Slammer_ThugTug, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Flinger_Desert, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Flinger_Trench, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Flinger_Junkyard, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Spinner_ThugTug, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spinner_Junkyard, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spinner_Planktopolis, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Popper_Trench, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Popper_Planktopolis, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Minimerv, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Mervyn, eventHandler),
            });

            ToolStripMenuItem moreEnemiesTSSM = new ToolStripMenuItem("More Enemies");
            moreEnemiesTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Jellyfish, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Jellyfish_Bucket, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Turret_v1, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Turret_v2, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Turret_v3, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Spawner_BB, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spawner_DE, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spawner_GG, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spawner_TR, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spawner_JK, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spawner_PT, eventHandler),
            });

            ToolStripMenuItem stageitemsTSSM = new ToolStripMenuItem("Stage Items");
            stageitemsTSSM.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Button_Red, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Checkpoint, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Checkpoint_Invisible, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Teleport_Box, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Throw_Fruit, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Freezy_Fruit, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Swinger, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Swinger_Platform, eventHandler),
                GetTemplateMenuItem(AssetTemplate.CollapsePlatform_Planktopolis, eventHandler),
                GetTemplateMenuItem(AssetTemplate.CollapsePlatform_Spongeball, eventHandler),
                GetTemplateMenuItem(AssetTemplate.CollapsePlatform_ThugTug, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Ring, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Ring_Control, eventHandler),
            });

            ToolStripMenuItem tssm = new ToolStripMenuItem("Movie Game");
            tssm.DropDownItems.AddRange(new ToolStripItem[]
            {
                pickupsTSSM,
                enemiesTSSM,
                moreEnemiesTSSM,
                stageitemsTSSM
            });

            ToolStripMenuItem scoobyPickups = new ToolStripMenuItem("Pickups");
            scoobyPickups.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Scooby_Snack, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Snack_Box, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Warp_Gate, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Snack_Gate, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Save_Point, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Clue, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Key, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Gum, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Gum_Pack, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Soap, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Soap_Pack, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Cake, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Hamburger, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Ice_Cream, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sandwich, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Turkey, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Shovel, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Springs, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Slippers, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Lampshade, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Helmet, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Knight_Helmet, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Boots, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Super_Smash, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Plungers, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Super_Sonic_Smash, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Umbrella, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Gum_Machine, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Soap_Bubble, eventHandler),
            });

            ToolStripMenuItem scoobyEnemies = new ToolStripMenuItem("Enemies");
            scoobyEnemies.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Caveman, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Creeper, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Funland_Robot, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Gargoyle, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Geronimo, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Ghost, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Ghost_Diver, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Ghost_of_Captain_Moody, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Headless_Specter, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sea_Creature, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Scarecrow, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Space_Kook, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Tar_Monster, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Witch, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Witch_Doctor, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Wolfman, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Zombie, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Bat, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Crab, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Flying_Fish, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Rat, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shark, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Spider, eventHandler),
                //new ToolStripSeparator(),
                //GetTemplateMenuItem(AssetTemplate.Black_Knight, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Green_Ghost, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Redbeard, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Mastermind, eventHandler),
                //new ToolStripSeparator(),
                //GetTemplateMenuItem(AssetTemplate.Killer_Plant, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Holly, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Groundskeeper, eventHandler),

                //GetTemplateMenuItem(AssetTemplate.Shaggy0, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Shaggy1, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Shaggy4, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Shaggy5, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Shaggy8, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Fred, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Daphne, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Velma, eventHandler),
            });

            ToolStripMenuItem stageitemsScooby = new ToolStripMenuItem("Stage Items");
            stageitemsScooby.DropDownItems.AddRange(new ToolStripItem[]
            {
                //GetTemplateMenuItem(AssetTemplate.Red_Button, eventHandler),
                //GetTemplateMenuItem(AssetTemplate.Floor_Button, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Crate, eventHandler),
            });

            ToolStripMenuItem scooby = new ToolStripMenuItem("Scooby");
            scooby.DropDownItems.AddRange(new ToolStripItem[]
            {
                scoobyPickups,
                scoobyEnemies,
                stageitemsScooby
            });

            ToolStripMenuItem incrediblesPickups = new ToolStripMenuItem("Pickups");
            incrediblesPickups.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Health_10, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Health_25, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Health_50, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Power_25, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Power_50, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Bonus, eventHandler),
            });

            ToolStripMenuItem incredibles = new ToolStripMenuItem("Incredibles");
            incredibles.DropDownItems.AddRange(new ToolStripItem[]
            {
                incrediblesPickups
            });

            ToolStripMenuItem placeable = new ToolStripMenuItem("Placeable");
            placeable.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Box_Trigger, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sphere_Trigger, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Cylinder_Trigger, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Camera, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Marker, eventHandler),
                GetTemplateMenuItem(AssetTemplate.MovePoint, eventHandler),
                GetTemplateMenuItem(AssetTemplate.MovePoint_Area, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Light, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Player, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Pointer, eventHandler),
                GetTemplateMenuItem(AssetTemplate.SDFX, eventHandler),
                GetTemplateMenuItem(AssetTemplate.SFX_OnEvent, eventHandler),
                GetTemplateMenuItem(AssetTemplate.SFX_OnRadius, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Boulder, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Button, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Destructible_Object, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Electric_Arc_Generator, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Hangable, eventHandler),
                GetTemplateMenuItem(AssetTemplate.NPC, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Pendulum, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Platform, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Simple_Object, eventHandler),
                GetTemplateMenuItem(AssetTemplate.VIL, eventHandler),
                GetTemplateMenuItem(AssetTemplate.User_Interface, eventHandler),
                GetTemplateMenuItem(AssetTemplate.User_Interface_Font, eventHandler),
            });

            ToolStripMenuItem others = new ToolStripMenuItem("Other");
            others.DropDownItems.AddRange(new ToolStripItem[]
            {
                GetTemplateMenuItem(AssetTemplate.Animation_List, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Collision_Table, eventHandler),
                GetTemplateMenuItem(AssetTemplate.DefaultGlowSceneProp, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Environment, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Jaw_Data_Table, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Level_Of_Detail_Table, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Model_Info, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Pipe_Info_Table, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Shadow_Table, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Sound_Info, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Surface_Mapper, eventHandler),
                new ToolStripSeparator(),
                GetTemplateMenuItem(AssetTemplate.Empty_BSP, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Empty_Sound, eventHandler),
                GetTemplateMenuItem(AssetTemplate.Empty_Streaming_Sound, eventHandler),
            });

            var paste = GetTemplateMenuItem(AssetTemplate.PasteClipboard, eventHandler);

            menu.DropDownItems.AddRange(new ToolStripItem[] { controllers, placeable, bfbb, tssm, scooby, incredibles, others, paste });
        }

        private static string GetName(AssetTemplate template)
        {
            switch (template)
            {
                case AssetTemplate.MovePoint_Area:
                    return "MovePoint (Area)";
                case AssetTemplate.SFX_OnEvent:
                    return "SFX (on event)";
                case AssetTemplate.SFX_OnRadius:
                    return "SFX (on radius)";
                case AssetTemplate.Shiny_Blue:
                    return "Shiny Object (Blue)";
                case AssetTemplate.Shiny_Green:
                    return "Shiny Object (Green)";
                case AssetTemplate.Shiny_Purple:
                    return "Shiny Object (Purple)";
                case AssetTemplate.Shiny_Red:
                    return "Shiny Object (Red)";
                case AssetTemplate.Shiny_Yellow:
                    return "Shiny Object (Yellow)";
                case AssetTemplate.Manliness_Red:
                    return "Manliness Point (Red)";
                case AssetTemplate.Manliness_Yellow:
                    return "Manliness Point (Yellow)";
                case AssetTemplate.Manliness_Green:
                    return "Manliness Point (Green)";
                case AssetTemplate.Manliness_Blue:
                    return "Manliness Point (Blue)";
                case AssetTemplate.Manliness_Purple:
                    return "Manliness Point (Purple)";
                case AssetTemplate.Hammer:
                    return "Ham-mer";
                case AssetTemplate.TarTar:
                    return "Tar-Tar";
                case AssetTemplate.ChompBot:
                    return "Chomp Bot";
                case AssetTemplate.GLove:
                    return "G-Love";
                case AssetTemplate.Chuck_Trigger:
                    return "Chuck (trigger)";
                case AssetTemplate.Monsoon_Trigger:
                    return "Monsoon (trigger)";
                case AssetTemplate.Sleepytime:
                    return "Sleepytime (stationary)";
                case AssetTemplate.Sleepytime_Moving:
                    return "Sleepytime (moving)";
                case AssetTemplate.BombBot:
                    return "Bomb Bot";
                case AssetTemplate.BzztBot:
                    return "Bzzt Bot";
                case AssetTemplate.Slick_Trigger:
                    return "Slick (trigger)";
                case AssetTemplate.Jellyfish_Pink:
                    return "Jellyfish (Pink)";
                case AssetTemplate.Jellyfish_Blue:
                    return "Jellyfish (Blue)";
                case AssetTemplate.Jellyfish:
                    return "Jellyfish";
                case AssetTemplate.Jellyfish_Bucket:
                    return "Jellyfish (Buckethead)";
                case AssetTemplate.Fogger_GoofyGoober:
                    return "Fogger (Goofy Goober)";
                case AssetTemplate.Fogger_Desert:
                    return "Fogger (Desert)";
                case AssetTemplate.Fogger_ThugTug:
                    return "Fogger (Thug Tug)";
                case AssetTemplate.Fogger_Trench:
                    return "Fogger (Trench)";
                case AssetTemplate.Fogger_Junkyard:
                    return "Fogger (Junkyard)";
                case AssetTemplate.Fogger_Planktopolis:
                    return "Fogger (Planktopolis)";
                case AssetTemplate.Fogger_v1:
                    return "Fogger (v1)";
                case AssetTemplate.Fogger_v2:
                    return "Fogger (v2)";
                case AssetTemplate.Fogger_v3:
                    return "Fogger (v3)";
                case AssetTemplate.Slammer_GoofyGoober:
                    return "Slammer (Goofy Goober)";
                case AssetTemplate.Slammer_Desert:
                    return "Slammer (Desert)";
                case AssetTemplate.Slammer_ThugTug:
                    return "Slammer (Thug Tug)";
                case AssetTemplate.Flinger_Desert:
                    return "Flinger (Desert)";
                case AssetTemplate.Flinger_Trench:
                    return "Flinger (Trench)";
                case AssetTemplate.Flinger_Junkyard:
                    return "Flinger (Junkyard)";
                case AssetTemplate.Spinner_ThugTug:
                    return "Spinner (Thug Tug)";
                case AssetTemplate.Spinner_Junkyard:
                    return "Spinner (Junkyard)";
                case AssetTemplate.Spinner_Planktopolis:
                    return "Spinner (Planktopolis)";
                case AssetTemplate.Popper_Trench:
                    return "Popper (Trench)";
                case AssetTemplate.Popper_Planktopolis:
                    return "Popper (Planktopolis)";
                case AssetTemplate.Turret_v1:
                    return "Turret (v1)";
                case AssetTemplate.Turret_v2:
                    return "Turret (v2)";
                case AssetTemplate.Turret_v3:
                    return "Turret (v3)";
                case AssetTemplate.Spawner_BB:
                    return "Spawner (Goofy Goober)";
                case AssetTemplate.Spawner_DE:
                    return "Spawner (Desert)";
                case AssetTemplate.Spawner_TR:
                    return "Spawner (Trench)";
                case AssetTemplate.Spawner_JK:
                    return "Spawner (Junkyard)";
                case AssetTemplate.Spawner_PT:
                    return "Spawner (Planktopolis)";
                case AssetTemplate.Spawner_GG:
                    return "Spawner (GG, unused)";
                case AssetTemplate.Button_Red:
                    return "Red Button";
                case AssetTemplate.Texas_Hitch_Platform:
                    return "Texas Hitch (Platform)";
                case AssetTemplate.Swinger_Platform:
                    return "Swinger (Platform)";
                case AssetTemplate.Checkpoint_Invisible:
                    return "Checkpoint (Invisible)";
                case AssetTemplate.CollapsePlatform_Planktopolis:
                    return "Collapse Platform (Planktopolis)";
                case AssetTemplate.CollapsePlatform_Spongeball:
                    return "Collapse Platform (Spongeball)";
                case AssetTemplate.CollapsePlatform_ThugTug:
                    return "Collapse Platform (Thug Tug)";
                case AssetTemplate.Health_10:
                    return "Health (10)";
                case AssetTemplate.Health_25:
                    return "Health (25)";
                case AssetTemplate.Health_50:
                    return "Health (50)";
                case AssetTemplate.Power_25:
                    return "Power (25)";
                case AssetTemplate.Power_50:
                    return "Power (50)";
                case AssetTemplate.Bonus:
                    return "Bonus Item";
            }

            var tstring = template.ToString().Replace('_', ' ');
            var result = new List<char>();
            for (int i = 0; i < tstring.Length; i++)
            {
                if (i > 0 && tstring[i] >= 'A' && tstring[i] <= 'Z' && tstring[i-1] != ' ')
                    result.Add(' ');
                result.Add(tstring[i]);
            }
            return template.ToString();
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
                case AssetTemplate.DefaultGlowSceneProp:
                case AssetTemplate.Empty_BSP:
                case AssetTemplate.Environment:
                case AssetTemplate.LKIT_lights:
                case AssetTemplate.LKIT_JF_SB_lights:
                case AssetTemplate.LKIT_jf01_light_kit:
                case AssetTemplate.Model_Info:
                case AssetTemplate.Flythrough_Widget:
                    ignoreNumber = true;
                    break;
                case AssetTemplate.Start_Camera:
                    assetName = startCamName;
                    ignoreNumber = true;
                    break;
                case AssetTemplate.Sound_Info:
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
                case AssetTemplate.Progress_Script:
                    asset = new AssetPGRS(assetName);
                    break;
                case AssetTemplate.Script:
                    asset = new AssetSCRP(assetName);
                    break;
                case AssetTemplate.Sound_Group:
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
                                TargetAsset = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.Expired,
                                EventSendID = (ushort)EventBFBB.Disable
                            },
                        };
                    }
                    break;
                case AssetTemplate.Cam_Tweak:
                    asset = new DynaGObjectCamTweak(assetName);
                    break;
                case AssetTemplate.Duplicatotron_Settings:
                    asset = new DynaGObjectNPCSettings(assetName);
                    break;
                case AssetTemplate.Camera:
                case AssetTemplate.Start_Camera:
                case AssetTemplate.Bus_Stop_Camera:
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
                                ArgumentAsset = game == Game.Incredibles ?
                                PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_POINTER", AssetTemplate.Pointer).assetID :
                                PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_MRKR", AssetTemplate.Marker).assetID,
                                TargetAsset = checkpointDisp,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.SetCheckPoint
                            }
                        };

                        if (template == AssetTemplate.Checkpoint && game == Game.BFBB)
                            links.Add(new Link(game)
                            {
                                TargetAsset = PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_TIMER", AssetTemplate.Checkpoint_Timer).assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.Run
                            });

                        if (template == AssetTemplate.Checkpoint && game == Game.Incredibles)
                        {
                            links.Add(new Link(game)
                            {
                                TargetAsset = PlaceTemplate(position, layerIndex, ref assetIDs, "CHECKPOINT_SCRIPT", AssetTemplate.Checkpoint_Script).assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.Run
                            });
                            links.Add(new Link(game)
                            {
                                TargetAsset = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.Disable
                            });
                        }

                        ((AssetTRIG)asset).Links = links.ToArray();
                    }
                    break;
                case AssetTemplate.Bus_Stop_Trigger:
                    {
                        asset = new AssetTRIG(assetName, position, template);
                        var lightsSimp = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("TRIGGER", "LIGHTS").Replace("TRIG", "LIGHTS"), AssetTemplate.Bus_Stop_Lights);

                        ((AssetTRIG)asset).Links = new Link[] {
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
                    }
                    break;
                case AssetTemplate.MovePoint_Area:
                case AssetTemplate.MovePoint:
                    asset = new AssetMVPT(assetName, position, game, template);
                    if (template == AssetTemplate.MovePoint && chainPointMVPTs && ContainsAsset(chainPointMVPTlast))
                    {
                        var prev = (AssetMVPT)GetFromAssetID(chainPointMVPTlast);
                        var prevNexts = prev.NextMovePoints.ToList();
                        prevNexts.Add(asset.assetID);
                        prev.NextMovePoints = prevNexts.ToArray();
                    }
                    chainPointMVPTlast = asset.assetID;
                    break;
                case AssetTemplate.Pointer:
                    asset = new DynaPointer(assetName, position);
                    break;
                case AssetTemplate.Player:
                    asset = new AssetPLYR(assetName, position, game);
                    break;
                case AssetTemplate.Boulder:
                    asset = new AssetBOUL(assetName, position);
                    break;
                case AssetTemplate.Button:
                case AssetTemplate.Button_Red:
                case AssetTemplate.Pressure_Plate:
                case AssetTemplate.Red_Button:
                case AssetTemplate.Floor_Button:
                    asset = new AssetBUTN(assetName, position, template);
                    if (template == AssetTemplate.Pressure_Plate)
                        PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_BASE", AssetTemplate.Pressure_Plate_Base);
                    break;
                case AssetTemplate.Destructible_Object:
                case AssetTemplate.Crate:
                    asset = new AssetDSTR(assetName, position, template);
                    break;
                case AssetTemplate.Electric_Arc_Generator:
                    asset = new AssetEGEN(assetName, position);
                    break;
                case AssetTemplate.Hangable:
                    asset = new AssetHANG(assetName, position);
                    break;
                case AssetTemplate.NPC:
                case AssetTemplate.Caveman:
                case AssetTemplate.Creeper:
                case AssetTemplate.Gargoyle:
                case AssetTemplate.Geronimo:
                case AssetTemplate.Ghost:
                case AssetTemplate.Ghost_of_Captain_Moody:
                case AssetTemplate.Ghost_Diver:
                case AssetTemplate.Flying_Fish:
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
                case AssetTemplate.Rat:
                case AssetTemplate.Shark:
                case AssetTemplate.Spider:
                case AssetTemplate.Black_Knight:
                case AssetTemplate.Green_Ghost:
                case AssetTemplate.Redbeard:
                case AssetTemplate.Mastermind:
                case AssetTemplate.Killer_Plant:
                case AssetTemplate.Holly:
                case AssetTemplate.Groundskeeper:
                    asset = new AssetNPC(assetName, position, template);
                    break;
                case AssetTemplate.Pendulum:
                    asset = new AssetPEND(assetName, position);
                    break;
                case AssetTemplate.Platform:
                case AssetTemplate.Texas_Hitch_Platform:
                case AssetTemplate.Swinger_Platform:
                case AssetTemplate.Springboard:
                case AssetTemplate.Hovering_Platform:
                case AssetTemplate.CollapsePlatform_Planktopolis:
                case AssetTemplate.CollapsePlatform_ThugTug:
                case AssetTemplate.CollapsePlatform_Spongeball:
                    asset = new AssetPLAT(assetName, position, template);
                    break;
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
                    asset = new AssetSIMP(assetName, position, template);
                    switch (template)
                    {
                        case AssetTemplate.Bus_Stop:
                            PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.Bus_Stop_Trigger);
                            position.Y += 0.1f;
                            PlaceTemplate(position, layerIndex, ref assetIDs, template: AssetTemplate.Bus_Stop_DYNA);
                            break;
                        case AssetTemplate.Throw_Fruit:
                            PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "BASE", AssetTemplate.Throw_Fruit_Base);
                            break;
                        case AssetTemplate.Freezy_Fruit:
                            PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "BASE", AssetTemplate.Throw_Fruit_Base);
                            break;
                    }
                    break;
                case AssetTemplate.VIL:
                    asset = new AssetVIL(assetName, position, template, 0);
                    break;
                case AssetTemplate.User_Interface:
                    asset = new AssetUI(assetName, position);
                    break;
                case AssetTemplate.User_Interface_Font:
                    asset = new AssetUIFT(assetName, position);
                    break;
                case AssetTemplate.SFX_OnEvent:
                case AssetTemplate.SFX_OnRadius:
                    asset = new AssetSFX(assetName, position, game, template == AssetTemplate.SFX_OnRadius);
                    break;
                case AssetTemplate.SDFX:
                    asset = new AssetSDFX(assetName, position);
                    break;
                case AssetTemplate.Light:
                    asset = new AssetLITE(assetName, position);
                    break;
                case AssetTemplate.Animation_List:
                    asset = new AssetALST(assetName);
                    break;
                case AssetTemplate.Collision_Table:
                    asset = new AssetCOLL(assetName);
                    break;
                case AssetTemplate.Environment:
                    asset = new AssetENV(assetName, startCamName);
                    break;
                case AssetTemplate.Flythrough:
                    asset = new AssetFLY(assetName);
                    var flyWidget = (DynaGObjectFlythrough)PlaceTemplate(position, layerIndex, ref assetIDs, asset.assetName + "_WIDGET", AssetTemplate.Flythrough_Widget);
                    flyWidget.Flythrough = asset.assetID;
                    break;
                case AssetTemplate.Flythrough_Widget:
                    asset = new DynaGObjectFlythrough(assetName);
                    break;
                case AssetTemplate.Jaw_Data_Table:
                    asset = new AssetJAW(assetName);
                    break;
                case AssetTemplate.Level_Of_Detail_Table:
                    asset = new AssetLODT(assetName);
                    break;
                case AssetTemplate.Surface_Mapper:
                    asset = new AssetMAPR(assetName);
                    break;
                case AssetTemplate.Model_Info:
                    asset = new AssetMINF(assetName);
                    break;
                case AssetTemplate.Pipe_Info_Table:
                    asset = new AssetPIPT(assetName);
                    break;
                case AssetTemplate.Shadow_Table:
                    asset = new AssetSHDW(assetName);
                    break;
                case AssetTemplate.Sound_Info:
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
                case AssetTemplate.Empty_Sound:
                case AssetTemplate.Empty_Streaming_Sound:
                    asset = new AssetSound(assetName, template == AssetTemplate.Empty_Sound ? AssetType.Sound : AssetType.StreamingSound, game, platform, new byte[0]);
                    break;
                case AssetTemplate.Empty_BSP:
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
                    asset = new AssetPKUP(assetName, game, position, template);
                    break;
                case AssetTemplate.Wooden_Tiki:
                case AssetTemplate.Floating_Tiki:
                case AssetTemplate.Thunder_Tiki:
                case AssetTemplate.Shhh_Tiki:
                case AssetTemplate.Stone_Tiki:
                case AssetTemplate.ArfDog:
                case AssetTemplate.TubeletSlave:
                case AssetTemplate.Duplicatotron:
                    {
                        asset = new AssetVIL(assetName, position, template, 0);
                        if (template == AssetTemplate.Duplicatotron)
                        {
                            var vil = (AssetVIL)asset;
                            vil.NPCSettingsObject = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_SETTINGS", AssetTemplate.Duplicatotron_Settings).assetID;
                            vil.Links = new Link[] {
                                new Link(game)
                                {
                                    TargetAsset = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_GROUP", AssetTemplate.Group).assetID,
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
                        var movePoint = (AssetMVPT)PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.MovePoint_Area);
                        var vil = new AssetVIL(assetName, position, template, movePoint.assetID);
                        asset = vil;
                        if (template == AssetTemplate.Chuck_Trigger || template == AssetTemplate.Monsoon_Trigger || template == AssetTemplate.Slick_Trigger)
                        {
                            vil.Links = new Link[] {
                            new Link(game)
                            {
                                TargetAsset = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.ScenePrepare,
                                EventSendID = (ushort)EventBFBB.DetectPlayerOff
                            }
                        };

                            var trigger = (AssetTRIG)PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_TRIG", AssetTemplate.Sphere_Trigger);
                            trigger.Radius = 15f;
                            trigger.Links = new Link[] {
                            new Link(game)
                            {
                                TargetAsset = asset.assetID,
                                EventReceiveID = (ushort)EventBFBB.EnterPlayer,
                                EventSendID = (ushort)EventBFBB.DetectPlayerOn
                            },
                            new Link(game)
                            {
                                TargetAsset = asset.assetID,
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
                                var slave = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_SLAVE_" + i, AssetTemplate.TubeletSlave);
                                links.Add(new Link(game)
                                {
                                    TargetAsset = slave.assetID,
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
                case AssetTemplate.Jellyfish:
                case AssetTemplate.Jellyfish_Bucket:
                    {
                        var mvpt = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_MP", AssetTemplate.MovePoint);
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
                            AssetTemplate.MovePoint : AssetTemplate.MovePoint_Area);
                        asset = new DynaEnemyStandard(assetName, template, position, mvpt.assetID);
                    }
                    break;
                case AssetTemplate.Turret_v1:
                case AssetTemplate.Turret_v2:
                case AssetTemplate.Turret_v3:
                    asset = new DynaEnemyTurret(assetName, template, position);
                    break;
                case AssetTemplate.Spawner_BB:
                case AssetTemplate.Spawner_DE:
                case AssetTemplate.Spawner_GG:
                case AssetTemplate.Spawner_JK:
                case AssetTemplate.Spawner_TR:
                case AssetTemplate.Spawner_PT:
                    {
                        var group = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_GROUP", AssetTemplate.Group);
                        asset = new DynaEnemyBucketOTron(assetName, template, position, group.assetID);
                    }
                    break;
                case AssetTemplate.Teleport_Box:
                    {
                        var mrkr = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper() + "_MRKR", AssetTemplate.Marker);
                        asset = new DynaGObjectTeleport(assetName, mrkr.assetID, GetMRKR);
                    }
                    break;
                case AssetTemplate.Bungee_Hook:
                    {
                        var simp = PlaceTemplate(position, layerIndex, ref assetIDs, "BUNGEE_SIMP", AssetTemplate.Bungee_Hook_SIMP);
                        asset = new DynaGObjectBungeeHook(assetName, simp.assetID);
                    }
                    break;
                case AssetTemplate.Bungee_Drop:
                    {
                        var mrkr = PlaceTemplate(position, layerIndex, ref assetIDs, "BUNGEE_MRKR", AssetTemplate.Marker);
                        asset = new DynaGObjectBungeeDrop(assetName, mrkr.assetID);
                    }
                    break;
                case AssetTemplate.Bus_Stop_DYNA:
                    {
                        var mrkr = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "MRKR"), AssetTemplate.Marker);
                        var cam = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "CAM"), AssetTemplate.Bus_Stop_Camera);
                        var simp = PlaceTemplate(position, layerIndex, ref assetIDs, template.ToString().ToUpper().Replace("DYNA", "SIMP"), AssetTemplate.Bus_Stop_BusSimp);

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
                        break;
                    }
                case AssetTemplate.Ring:
                    asset = new DynaGObjectRing(assetName, position);
                    break;
                case AssetTemplate.Ring_Control:
                    asset = new DynaGObjectRingControl(assetName);
                    break;
                case AssetTemplate.DefaultGlowSceneProp:
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
                case AssetTemplate.Health_10:
                case AssetTemplate.Health_25:
                case AssetTemplate.Health_50:
                case AssetTemplate.Power_25:
                case AssetTemplate.Power_50:
                case AssetTemplate.Bonus:
                    asset = new DynaGObjectInPickup(assetName, position, template);
                    break;
                default:
                    MessageBox.Show("Unsupported template");
                    return null;
            }

            asset.game = game;
            asset.endianness = platform.Endianness();

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

            env.StartCamera = PlaceTemplate(new Vector3(0, 100, 100), defaultLayer, customName: startCamName, template: AssetTemplate.Start_Camera).assetID;

            if (game != Game.Incredibles)
            {
                env.BSP = PlaceTemplate(new Vector3(), 0, "empty_bsp", template: AssetTemplate.Empty_BSP).assetID;
                PlaceTemplate(new Vector3(), defaultLayer, pkupsMinfName, template: AssetTemplate.Model_Info);
            }

            if (game == Game.BFBB)
            {
                env.Object_LightKit = PlaceTemplate(new Vector3(), defaultLayer, customName: "lights", template: AssetTemplate.LKIT_lights).assetID;
                player.LightKit = PlaceTemplate(new Vector3(), defaultLayer, customName: "JF_SB_lights", template: AssetTemplate.LKIT_JF_SB_lights).assetID;
            }
            else if (game == Game.Incredibles)
            {
                var light_kit = (AssetLKIT)PlaceTemplate(new Vector3(), defaultLayer, customName: "jf01_light_kit", template: AssetTemplate.LKIT_lights);
                player.LightKit = light_kit.assetID;
                env.Object_LightKit = light_kit.assetID;

                PlaceTemplate(new Vector3(), defaultLayer, customName: "DEFAULT_GLOW_SCENE_PROP", template: AssetTemplate.DefaultGlowSceneProp);
            }
        }
    }
}