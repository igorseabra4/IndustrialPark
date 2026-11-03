using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IndustrialPark.Archive;

public static class AssetFactoryService
{
    public static ToolStripMenuItem GetTemplateMenuItem(AssetTemplate template, EventHandler eventHandler)
    {
        var item = new ToolStripMenuItem(template.GetName()) { Tag = template };
        item.Click += eventHandler;
        return item;
    }

    public static List<ToolStripMenuItem> PopulateTemplateMenusAt(ToolStripMenuItem menu, EventHandler eventHandler)
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
            GetTemplateMenuItem(AssetTemplate.Cam_Tweak, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Disco_Floor, eventHandler),
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
            GetTemplateMenuItem(AssetTemplate.MovePoint_Arena, eventHandler),
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
            GetTemplateMenuItem(AssetTemplate.Duplicator, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Electric_Arc_Generator, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Hangable, eventHandler),
            GetTemplateMenuItem(AssetTemplate.NPC, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Pendulum, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Platform, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Simple_Object, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Villain, eventHandler),
            GetTemplateMenuItem(AssetTemplate.User_Interface, eventHandler),
            GetTemplateMenuItem(AssetTemplate.User_Interface_Font, eventHandler),
        });

        #region Scooby
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
            GetTemplateMenuItem(AssetTemplate.Spider, eventHandler),
            new ToolStripSeparator(),
            GetTemplateMenuItem(AssetTemplate.Groundskeeper, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Holly, eventHandler),
            new ToolStripSeparator(),
            GetTemplateMenuItem(AssetTemplate.Killer_Plant, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Shark, eventHandler),

            //GetTemplateMenuItem(AssetTemplate.Shaggy0, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Shaggy1, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Shaggy4, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Shaggy5, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Shaggy8, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Fred, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Daphne, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Velma, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Black_Knight, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Green_Ghost, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Redbeard, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Mastermind, eventHandler),
        });

        ToolStripMenuItem stageitemsScooby = new ToolStripMenuItem("Stage Items");
        stageitemsScooby.DropDownItems.AddRange(new ToolStripItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Gust, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Volume_Box, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Volume_Sphere, eventHandler),
            new ToolStripSeparator(),
            GetTemplateMenuItem(AssetTemplate.Red_Button, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Red_Button_Smash, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Floor_Button, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Floor_Button_Smash, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Crate, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Cauldron, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Flower, eventHandler),
        });

        ToolStripMenuItem scooby = new ToolStripMenuItem("Scooby");
        scooby.DropDownItems.AddRange(new ToolStripItem[]
        {
            scoobyPickups,
            scoobyEnemies,
            stageitemsScooby
        });
        #endregion

        #region BFBB
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
            GetTemplateMenuItem(AssetTemplate.NPC_Settings_Object, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Duplicatotron_Settings, eventHandler),
        });

        ToolStripMenuItem stageitemsBFBB = new ToolStripMenuItem("Stage Items");
        stageitemsBFBB.DropDownItems.AddRange(new ToolStripItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Button_Red, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Button_Red_WithBase, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Pressure_Plate, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Checkpoint_Set, eventHandler),
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
            GetTemplateMenuItem(AssetTemplate.Button_Red_Base, eventHandler),
        });

        ToolStripMenuItem bfbb = new ToolStripMenuItem("BFBB");
        bfbb.DropDownItems.AddRange(new ToolStripItem[]
        {
            pickupsBFBB,
            enemiesBFBB,
            stageitemsBFBB
        });
        #endregion

        #region TSSM
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
            GetTemplateMenuItem(AssetTemplate.Button_Red_WithBase, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Checkpoint_Set, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Checkpoint_Invisible, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Teleport_Box, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Throw_Fruit, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Freezy_Fruit, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Swinger, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Swinger_Platform, eventHandler),
            GetTemplateMenuItem(AssetTemplate.CollapsePlatform_Planktopolis, eventHandler),
            GetTemplateMenuItem(AssetTemplate.CollapsePlatform_Spongeball, eventHandler),
            GetTemplateMenuItem(AssetTemplate.CollapsePlatform_ThugTug, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Button_Red_Base, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Ring, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Ring_Control, eventHandler),
        });

        ToolStripMenuItem floatingBlocksTSSM = new ToolStripMenuItem("Floating Blocks");
        floatingBlocksTSSM.DropDownItems.AddRange(new ToolStripItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Floating_Block, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Floating_Block_Spiked, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Scale_Block, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Scale_Block_Driven, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Scale_Block_Spiked_Driven, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Ice_Block, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Ice_Block_Spiked, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Trampoline_Block, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Trampoline_Block_Driven, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Trampoline_Block_Spiked_Driven, eventHandler),
            new ToolStripSeparator(),
            GetTemplateMenuItem(AssetTemplate.Wooden_Platform, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Block_Spikes, eventHandler),
        });

        ToolStripMenuItem tssm = new ToolStripMenuItem("Movie");
        tssm.DropDownItems.AddRange(new ToolStripItem[]
        {
            pickupsTSSM,
            enemiesTSSM,
            moreEnemiesTSSM,
            stageitemsTSSM,
            floatingBlocksTSSM
        });
        #endregion

        #region Incredibles
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

        ToolStripMenuItem incrediblesObjects = new ToolStripMenuItem("Stage Items");
        incrediblesObjects.DropDownItems.AddRange(new ToolStripItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Rubble_Generator, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Zip_Line, eventHandler),
        });

        ToolStripMenuItem incredibles = new ToolStripMenuItem("Incredibles");
        incredibles.DropDownItems.AddRange(new ToolStripItem[]
        {
            incrediblesPickups,
            incrediblesObjects
        });
        #endregion

        #region ROTU
        ToolStripMenuItem rotuEnemies = new ToolStripMenuItem("Enemies");
        rotuEnemies.DropDownItems.AddRange(new ToolStripMenuItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Bomber, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Boss_Underminer_Drill, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Boss_Underminer_UM, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Chicken, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Driller, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Enforcer, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Humanoid, eventHandler),
            GetTemplateMenuItem(AssetTemplate.RotuRat, eventHandler),
            GetTemplateMenuItem(AssetTemplate.RobotTank, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Scientist, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Shooter, eventHandler),
        });

        ToolStripMenuItem rotu = new ToolStripMenuItem("Underminer");
        rotu.DropDownItems.AddRange(new ToolStripItem[]
        {
            rotuEnemies,
        });
        #endregion

        #region RatProto
        ToolStripMenuItem stageitemsRatproto = new ToolStripMenuItem("Stage Items");
        stageitemsRatproto.DropDownItems.AddRange(new ToolStripItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Carryable_Property_Attract, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Carryable_Property_Generic, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Carryable_Property_Repel, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Carryable_Property_Swipe, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Hud_Compass_System, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Hud_Compass_Object, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Pole_Swing, eventHandler),
            GetTemplateMenuItem(AssetTemplate.CObject_SpringBoard, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Tightrope, eventHandler),
        });

        ToolStripMenuItem ratprotoEnemies = new ToolStripMenuItem("Enemies");
        ratprotoEnemies.DropDownItems.AddRange(new ToolStripItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Waiter, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Thief, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Swarm_Bug, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Swarm_Owl, eventHandler),
        });

        ToolStripMenuItem ratproto = new ToolStripMenuItem("RatProto");
        ratproto.DropDownItems.AddRange(new ToolStripItem[]
        {
            stageitemsRatproto,
            ratprotoEnemies,
        });
        #endregion

        ToolStripMenuItem others = new ToolStripMenuItem("Other");
        others.DropDownItems.AddRange(new ToolStripItem[]
        {
            GetTemplateMenuItem(AssetTemplate.Animation_List, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Boulder_Generator, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Collision_Table, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Credits, eventHandler),
            GetTemplateMenuItem(AssetTemplate.DefaultGlowSceneProp, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Destructible, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Environment, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Jaw_Data_Table, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Level_Of_Detail_Table, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Model_Info, eventHandler),
            GetTemplateMenuItem(AssetTemplate.One_Liner, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Pipe_Info_Table, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Shadow_Table, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Shrapnel, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Sound_Info, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Surface_Mapper, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Surface, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Throwable_Table, eventHandler),
            new ToolStripSeparator(),
            GetTemplateMenuItem(AssetTemplate.Empty_BSP, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Jsp_Info, eventHandler),
            new ToolStripSeparator(),
            GetTemplateMenuItem(AssetTemplate.Camera_Curve, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Checkpoint, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Light_Effect, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Light_Effect_Flicker, eventHandler),
            GetTemplateMenuItem(AssetTemplate.Light_Effect_Strobe, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Empty_Sound, eventHandler),
            //GetTemplateMenuItem(AssetTemplate.Empty_Streaming_Sound, eventHandler),
        });

        var paste = GetTemplateMenuItem(AssetTemplate.Paste_Clipboard, eventHandler);

        var items = new ToolStripItem[] { controllers, placeable, new ToolStripSeparator(), scooby, bfbb, tssm, incredibles, rotu, ratproto, new ToolStripSeparator(), others, paste };
        menu.DropDownItems.AddRange(items);

        var result = new List<ToolStripMenuItem>();
        foreach (var i in items)
            if (i is ToolStripMenuItem item)
                result.AddRange(GetAllItems(item));
        return result;
    }

    private static List<ToolStripMenuItem> GetAllItems(ToolStripMenuItem item)
    {
        var result = new List<ToolStripMenuItem>();
        if (item is ToolStripMenuItem tsmi)
        {
            if (tsmi.Tag is AssetTemplate)
                result.Add(tsmi);
            if (tsmi.HasDropDownItems)
                foreach (var i in tsmi.DropDownItems)
                    if (i is ToolStripMenuItem tsmi2)
                        result.AddRange(GetAllItems(tsmi2));
        }
        return result;
    }

    public static string GetName(this AssetTemplate template)
    {
        return template switch
        {
            AssetTemplate.NPC => "NPC (BFBB/Incredibles)",
            AssetTemplate.Villain => "Villain (Scooby)",
            AssetTemplate.MovePoint => "Move Point",
            AssetTemplate.MovePoint_Arena => "Move Point (arena, off)",
            AssetTemplate.SFX_OnEvent => "SFX (on event)",
            AssetTemplate.SFX_OnRadius => "SFX (on radius)",
            AssetTemplate.Shiny_Blue => "Shiny Object (Blue)",
            AssetTemplate.Shiny_Green => "Shiny Object (Green)",
            AssetTemplate.Shiny_Purple => "Shiny Object (Purple)",
            AssetTemplate.Shiny_Red => "Shiny Object (Red)",
            AssetTemplate.Shiny_Yellow => "Shiny Object (Yellow)",
            AssetTemplate.Manliness_Red => "Manliness Point (Red)",
            AssetTemplate.Manliness_Yellow => "Manliness Point (Yellow)",
            AssetTemplate.Manliness_Green => "Manliness Point (Green)",
            AssetTemplate.Manliness_Blue => "Manliness Point (Blue)",
            AssetTemplate.Manliness_Purple => "Manliness Point (Purple)",
            AssetTemplate.Hammer => "Ham-mer",
            AssetTemplate.TarTar => "Tar-Tar",
            AssetTemplate.ChompBot => "Chomp Bot",
            AssetTemplate.GLove => "G-Love",
            AssetTemplate.Chuck_Trigger => "Chuck (trigger)",
            AssetTemplate.Monsoon_Trigger => "Monsoon (trigger)",
            AssetTemplate.Sleepytime => "Sleepytime (stationary)",
            AssetTemplate.Sleepytime_Moving => "Sleepytime (moving)",
            AssetTemplate.BombBot => "Bomb Bot",
            AssetTemplate.BzztBot => "Bzzt Bot",
            AssetTemplate.Slick_Trigger => "Slick (trigger)",
            AssetTemplate.Jellyfish_Pink => "Jellyfish (Pink)",
            AssetTemplate.Jellyfish_Blue => "Jellyfish (Blue)",
            AssetTemplate.Jellyfish => "Jellyfish",
            AssetTemplate.Jellyfish_Bucket => "Jellyfish (Buckethead)",
            AssetTemplate.Fogger_GoofyGoober => "Fogger (Goofy Goober)",
            AssetTemplate.Fogger_Desert => "Fogger (Desert)",
            AssetTemplate.Fogger_ThugTug => "Fogger (Thug Tug)",
            AssetTemplate.Fogger_Trench => "Fogger (Trench)",
            AssetTemplate.Fogger_Junkyard => "Fogger (Junkyard)",
            AssetTemplate.Fogger_Planktopolis => "Fogger (Planktopolis)",
            AssetTemplate.Fogger_v1 => "Fogger (v1)",
            AssetTemplate.Fogger_v2 => "Fogger (v2)",
            AssetTemplate.Fogger_v3 => "Fogger (v3)",
            AssetTemplate.Slammer_GoofyGoober => "Slammer (Goofy Goober)",
            AssetTemplate.Slammer_Desert => "Slammer (Desert)",
            AssetTemplate.Slammer_ThugTug => "Slammer (Thug Tug)",
            AssetTemplate.Flinger_Desert => "Flinger (Desert)",
            AssetTemplate.Flinger_Trench => "Flinger (Trench)",
            AssetTemplate.Flinger_Junkyard => "Flinger (Junkyard)",
            AssetTemplate.Spinner_ThugTug => "Spinner (Thug Tug)",
            AssetTemplate.Spinner_Junkyard => "Spinner (Junkyard)",
            AssetTemplate.Spinner_Planktopolis => "Spinner (Planktopolis)",
            AssetTemplate.Popper_Trench => "Popper (Trench)",
            AssetTemplate.Popper_Planktopolis => "Popper (Planktopolis)",
            AssetTemplate.Turret_v1 => "Turret (v1)",
            AssetTemplate.Turret_v2 => "Turret (v2)",
            AssetTemplate.Turret_v3 => "Turret (v3)",
            AssetTemplate.Spawner_BB => "Spawner (Goofy Goober)",
            AssetTemplate.Spawner_DE => "Spawner (Desert)",
            AssetTemplate.Spawner_TR => "Spawner (Trench)",
            AssetTemplate.Spawner_JK => "Spawner (Junkyard)",
            AssetTemplate.Spawner_PT => "Spawner (Planktopolis)",
            AssetTemplate.Spawner_GG => "Spawner (GG, unused)",
            AssetTemplate.Button_Red => "Red Button",
            AssetTemplate.Button_Red_WithBase => "Red Button + Base",
            AssetTemplate.Button_Red_Base => "Button Base",
            AssetTemplate.Texas_Hitch_Platform => "Texas Hitch (Platform)",
            AssetTemplate.Swinger_Platform => "Swinger (Platform)",
            AssetTemplate.Checkpoint_Invisible => "Checkpoint (Invisible)",
            AssetTemplate.CollapsePlatform_Planktopolis => "Collapse Platform (Planktopolis)",
            AssetTemplate.CollapsePlatform_Spongeball => "Collapse Platform (Spongeball)",
            AssetTemplate.CollapsePlatform_ThugTug => "Collapse Platform (Thug Tug)",
            AssetTemplate.Health_10 => "Health (10)",
            AssetTemplate.Health_25 => "Health (25)",
            AssetTemplate.Health_50 => "Health (50)",
            AssetTemplate.Power_25 => "Power (25)",
            AssetTemplate.Power_50 => "Power (50)",
            AssetTemplate.Bonus => "Bonus Item",
            AssetTemplate.Red_Button => "Red Button (Helmet)",
            AssetTemplate.Red_Button_Smash => "Red Button (Super Smash)",
            AssetTemplate.Floor_Button => "Floor Button (Step)",
            AssetTemplate.Floor_Button_Smash => "Floor Button (Super Smash)",
            AssetTemplate.Volume_Box => "Volume (Box)",
            AssetTemplate.Volume_Sphere => "Volume (Sphere)",
            AssetTemplate.Floating_Block_Spiked => "Floating Block (spiked)",
            AssetTemplate.Scale_Block_Driven => "Scale Block (with driver)",
            AssetTemplate.Scale_Block_Spiked_Driven => "Scale Block (spiked with driver)",
            AssetTemplate.Ice_Block_Spiked => "Ice Block (spiked)",
            AssetTemplate.Trampoline_Block_Driven => "Trampoline Block (with driver)",
            AssetTemplate.Trampoline_Block_Spiked_Driven => "Trampoline Block (spiked with driver)",
            AssetTemplate.Block_Spikes => "Spikes",
            AssetTemplate.CObject_SpringBoard => "SpringBoard",
            AssetTemplate.RotuRat => "Rat",
            _ => template.ToString().Replace('_', ' '),
        };
    }

    private static readonly HashSet<AssetTemplate> noModelTemplates =
    [
        AssetTemplate.Boulder,
        AssetTemplate.Button,
        AssetTemplate.Destructible_Object,
        AssetTemplate.Electric_Arc_Generator,
        AssetTemplate.Hangable,
        AssetTemplate.Pendulum,
        AssetTemplate.Platform,
        AssetTemplate.Simple_Object,
        AssetTemplate.User_Interface
    ];

    public static AssetID TemplateModelAssetID = 0;

    public static void AssignDefaultModelToTemplate(AssetTemplate template, Asset asset)
    {
        if (TemplateModelAssetID != 0 &&
            noModelTemplates.Contains(template) &&
            asset is EntityAsset entity &&
            entity.Model == 0)
            entity.Model = TemplateModelAssetID;
    }
}
