using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

namespace IndustrialPark.Randomizer
{
    public class RandomizerSettings
    {
        [Category("Boulder Settings"), Description("Randomizes floating point values in boulders, such as speed, gravity, mass, lifetime etc.")]
        public bool BoulderSettings { get; set; } = true;
        [Category("Boulder Settings"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float boulderMin { get; set; } = 0.5f;
        [Category("Boulder Settings"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float boulderMax { get; set; } = 2f;


        [Category("Settings"),
            DisplayName("Patterns/files to skip entirely"),
            Description("The randomizer will leave these files unnafected.")]
        public string[] skipFiles { get; set; } = new string[]
        {
            "font", "boot", "plat", "mn", "sp", "pl", "hb10", "db05", "b301", "s006", "b402"
        };


        [Category("Randomizer"), Description("Randomizes pickup positions.")]
        public bool Pickups { get; set; } = true;
        [Category("Randomizer"), Description("Randomizes patterns of disco floors.")]
        public bool Disco_Floors { get; set; } = true;
        [Category("Randomizer"), Description("Randomizes music.")]
        public bool Music { get; set; } = true;
        [Category("Randomizer"), Description("Randomizes the Floating Block Challenges.")]
        public bool FloatingBlockChallenge { get; set; } = true;
        [Category("Randomizer"), Description("Randomizes camera positions.")]
        public bool Cameras { get; set; } = false;
        [Category("Randomizer"), Description("Randomizes textures.")]
        public bool Textures { get; set; } = false;
        [Category("Randomizer"), Description("Randomizes menu/HUD and particle textures.")]
        public bool Textures_Special { get; set; } = false;
        [Category("Randomizer"), Description("Randomizes sounds.")]
        public bool Sounds { get; set; } = false;

        [Category("Randomizer (Not recommended)"), Description("NOT RECOMMENDED\nMixes sound effects and voices.")]
        public bool Mix_Sound_Types { get; set; } = false;
        [Category("Randomizer (Not recommended)"), Description("NOT RECOMMENDED\nRandomizes links of button assets. High chance of softlocks and unbeatable tasks.")]
        public bool Buttons { get; set; } = false;
        [Category("Randomizer (Not recommended)"), Description("NOT RECOMMENDED\nRandomizes level files with each other. WILL break multiple levels and tasks and cause softlocks.")]
        public bool Level_Files { get; set; } = false;


        [Category("Warps"), Description("Randomizes destination of level warps.")]
        public bool Warps { get; set; } = true;
        [Category("Warps"), DisplayName("Skip Warps"), Description("The randomizer will leave warps to/from these files unnafected.")]
        public string[] skipFilesWarps { get; set; } = new string[]
        {
            "hb00", "gy04", "b3", "pg", "s005"
        };



        [Category("Tikis"), Description("Randomizes tiki types.")]
        public bool Tiki_Types { get; set; } = true;
        [Category("Tikis"), Description("Allows a tiki's look and behavior to be different.")]
        public bool Tiki_Models { get; set; } = true;
        [Category("Tikis"), Description("Allows tikis which were not present in the level before.")]
        public bool Tiki_Allow_Any_Type { get; set; } = true;
        [Category("Tikis"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Tiki Probabilities")]
        public TikiProbabilities TikiProbabilities { get; set; } = new TikiProbabilities();


        [Category("Crates"), Description("Randomizes crate types.")]
        public bool Crate_Types { get => Tiki_Types; set => Tiki_Types = value; }
        [Category("Crates"), Description("Allows crates which were not present in the level before.")]
        public bool Crates_Allow_Any_Type { get => Tiki_Allow_Any_Type; set => Tiki_Allow_Any_Type = value; }
        [Category("Crates"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Crate Probabilities")]
        public CrateProbabilities CrateProbabilities { get; set; } = new CrateProbabilities();


        [Category("Enemies"), Description("Randomizes enemy types.")]
        public bool Enemy_Types { get; set; } = true;
        [Category("Enemies"), Description("Allows enemies which were not present in the level before.")]
        public bool Enemies_Allow_Any_Type { get; set; } = true;
        [Category("Enemies"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Enemy Probabilities")]
        public EnemyProbabilities EnemyProbabilities { get; set; } = new EnemyProbabilities();
        [Category("Enemies"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Enemy Probabilities")]
        public EnemyProbabilitiesMovie EnemyProbabilitiesMovie { get; set; } = new EnemyProbabilitiesMovie();


        [Category("MovePoint"), Description("Randomizes MovePoint asset radius. Enemies can move more and see you from further away.")]
        public bool MovePoint_Radius { get; set; } = true;
        [Category("MovePoint"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float mvptMin { get; set; } = 0.9f;
        [Category("MovePoint"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float mvptMax { get; set; } = 1.8f;


        [Category("Platform Speed"), Description("Randomizes spped of platform (moving) assets. Higher values mean faster platforms. Negative values create interesting results.")]
        public bool PlatformSpeeds { get; set; } = true;
        [Category("Platform Speed"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float speedMin { get; set; } = 1f;
        [Category("Platform Speed"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float speedMax { get; set; } = 5f;


        [Category("Position Helpers"), Description(Constants.posHelpExample + " Markers are used in BFBB and Scooby for warping and checkpoints.")]
        public bool Marker_Positions { get; set; } = true;
        [Category("Position Helpers"), Description(Constants.posHelpExample)]
        public bool Player_Start { get; set; } = true;
        [Category("Position Helpers"), Description("NOT RECOMMENDED FOR BFBB\n" + Constants.posHelpExample + " Pointers are used for talking to NPCs in BFBB, but for warping and checkpoints in Movie.")]
        public bool Pointer_Positions { get; set; } = false;
        [Category("Position Helpers"), Description("NOT RECOMMENDED FOR BFBB\n" + Constants.posHelpExample)]
        public bool Teleport_Box_Positions { get; set; } = false;
        [Category("Position Helpers"), Description("NOT RECOMMENDED\n" + Constants.posHelpExample)]
        public bool Taxi_Trigger_Positions { get; set; } = false;
        [Category("Position Helpers"), Description("NOT RECOMMENDED\n" + Constants.posHelpExample)]
        public bool Bus_Stop_Trigger_Positions { get; set; } = false;


        [Category("Timers"), Description("Randomizes timer values")]
        public bool Timers { get; set; } = true;
        [Category("Timers"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float timerMin { get; set; } = 0.75f;
        [Category("Timers"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float timerMax { get; set; } = 1.75f;


        [Category("Scale (Not recommended)"), Description("NOT RECOMMENDED\nRandomizes scale of Entity assets.")]
        public bool Scale_Of_Things { get; set; } = false;
        [Category("Scale (Not recommended)"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float scaleMin { get; set; } = 0.8f;
        [Category("Scale (Not recommended)"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float scaleMax { get; set; } = 1.3f;


        [Category("Shiny Object Gates"), Description("Randomizes clam/toll gate shiny object requirements in BFBB.")]
        public bool Shiny_Object_Gates { get; set; } = true;
        [Category("Shiny Object Gates"),
            DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float shinyReqMin { get; set; } = 0.6f;
        [Category("Shiny Object Gates"),
            DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float shinyReqMax { get; set; } = 1.5f;


        [Category("Spatula Gates"), Description("Randomizes spatula toll gate requirements in BFBB.")]
        public bool Spatula_Gates { get; set; } = true;
        [Category("Spatula Gates"),
            DisplayName("Minimum"), Description(Constants.minMaxExample)]
        public int spatReqMin { get; set; } = 5;
        [Category("Spatula Gates"),
            DisplayName("Maximum"), Description(Constants.minMaxExample)]
        public int spatReqMax { get; set; } = 70;


        [Category("Texture Animations"), Description("Randomizes speed of UV animations on surfaces.")]
        public bool Texture_Animations { get; set; } = true;
        [Category("Texture Animations"),
            DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float surfMin { get; set; } = -2f;
        [Category("Texture Animations"),
            DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(FloatTypeConverter))]
        public float surfMax { get; set; } = 2f;


        [Category("Colors"), DisplayName("Colors"), Description("Randomizes colors of stuff.")]
        public bool Colors { get; set; } = true;
        [Category("Colors"), DisplayName("Bright Colors"), Description("Colors randomizer results in lighter colors")]
        public bool brightColors { get; set; } = true;
        [Category("Colors"), DisplayName("Strong Colors"), Description("Colors randomizer results in very saturated colors")]
        public bool strongColors { get; set; } = false;


        [Category("Characters"), DisplayName("Player Characters"), Description("Randomizes the characters in each Bus Stop (Patrick or Sandy). This will automatically enable Unlock Characters.")]
        public bool PlayerCharacters { get; set; } = false;
        [Category("Characters"), DisplayName("Unlock Characters"), Description("Allows playing as Patrick and Sandy on every level. This will automatically disable Enemies Allow Any Type")]
        public bool UnlockCharacters { get; set; } = false;
        [Category("Characters"), DisplayName("Random Characters"), Description("Switches you to Spongebob, Patrick or Sandy randomly on every spawn.")]
        public bool RandomCharacters { get; set; } = false;


        [Category("INI Mods (Cheats)"), DisplayName("Invincible")]
        public bool cheatInvincible { get; set; } = false;
        [Category("INI Mods (Cheats)"), DisplayName("BFBB Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        public PowerupCheatsBFBB PowerupCheatsBFBB { get; set; } = new PowerupCheatsBFBB();
        [Category("INI Mods (Cheats)"), DisplayName("Scooby Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        public PowerupCheatsScooby PowerupCheatsScooby { get; set; } = new PowerupCheatsScooby();
        [Category("INI Mods (Cheats)"), DisplayName("Movie Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        public PowerupCheatsMovie PowerupCheatsMovie { get; set; } = new PowerupCheatsMovie();


        [Category("INI Mods"), DisplayName("Boot Level"), Description("Which level to start the game in.")]
        public BootLevelMode bootLevelMode { get; set; } = BootLevelMode.Set;

        private string bootLevel = "HB01";

        [Category("INI Mods"), DisplayName("Set Boot Level"), Description("If Boot Level is 'set', start the game in this level.")]
        public string BootLevel
        {
            get => bootLevel;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("Value must have 4 characters.");
                bootLevel = value;
            }
        }

        [Category("INI Mods"), DisplayName("Don't Show Menu on Boot")]
        public bool dontShowMenuOnBoot { get; set; } = true;

        [Category("INI Mods"), DisplayName("All Menu Warps HB01"),
            Description("If true, all menu warps will lead to the hub.")]
        public bool allMenuWarpsHB01 { get; set; } = true;

        [Category("Non-Randomizer"), DisplayName("Open Teleport Boxes")]
        public bool openTeleportBoxes { get; set; } = false;

        [Category("Non-Randomizer"), DisplayName("Invisible Level"), Description("Makes all levels invisible.")]
        public bool invisibleLevel { get; set; } = false;

        [Category("Non-Randomizer"), DisplayName("Invisible Objects"), Description("Makes all objects invisible.")]
        public bool invisibleObjects { get; set; } = false;

        [Category("Non-Randomizer"), DisplayName("Disable Cutscenes")]
        public bool disableCutscenes { get; set; } = true;

        [Category("Non-Randomizer"), DisplayName("Disable Flythroughs")]
        public bool disableFlythroughs { get; set; } = true;

        [Category("Non-Randomizer"),
            DisplayName("Spatulas for Chum Bucket Lab"),
            Description("This will be the amount of spatulas needed for the final boss. Set to -1 to include it in the Spatula Gates randomizer method.")]
        public int spatReqChum { get; set; } = 75;

        [Category("Non-Randomizer: LODT"), DisplayName("boot.HIP LODT multiplier"),
            Description("If true, multiply the render distance for the pickups by this amount.")]
        public bool bootHipLodtMulti { get; set; } = true;

        [Category("Non-Randomizer: LODT"), DisplayName("boot.HIP LODT multiplier"),
            Description("If true, multiply the render distance for the pickups by this amount."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float lodtValue { get; set; } = 2f;

        [Category("Non-Randomizer"), DisplayName("Restore Robot Laugh"), Description("Restores robot laugh sound, which is not present normally in the GameCube version of the game.")]
        public bool restoreRobotLaugh { get; set; } = true;

        public void SetAllFalse()
        {
            Warps = false;
            Pickups = false;
            Tiki_Types = false;
            Tiki_Models = false;
            Tiki_Allow_Any_Type = false;
            Enemy_Types = false;
            Enemies_Allow_Any_Type = false;
            MovePoint_Radius = false;
            PlatformSpeeds = false;
            BoulderSettings = false;
            Marker_Positions = false;
            Player_Start = false;
            Pointer_Positions = false;
            Teleport_Box_Positions = false;
            Taxi_Trigger_Positions = false;
            Bus_Stop_Trigger_Positions = false;
            Scale_Of_Things = false;
            Shiny_Object_Gates = false;
            Spatula_Gates = false;
            Texture_Animations = false;
            Disco_Floors = false;
            Music = false;
            Colors = false;
            Cameras = false;
            Textures = false;
            Textures_Special = false;
            Sounds = false;
            Mix_Sound_Types = false;
            FloatingBlockChallenge = false;
            Buttons = false;
            Timers = false;
            Level_Files = false;
            brightColors = false;
            strongColors = false;
            PlayerCharacters = false;
            cheatInvincible = false;
            PowerupCheatsMovie.Preset = Preset.Nothing;
            bootLevelMode = BootLevelMode.Default;
            dontShowMenuOnBoot = false;
            allMenuWarpsHB01 = false;
            disableCutscenes = false;
            spatReqChum = 75;
            bootHipLodtMulti = false;
            restoreRobotLaugh = false;
            openTeleportBoxes = false;
            disableFlythroughs = false;
            UnlockCharacters = false;
            RandomCharacters = false;
            invisibleLevel = false;
            invisibleObjects = false;
        }

        public void SetDynamicProperties(DynamicTypeDescriptor dt, int game)
        {
            switch (game)
            {
                case 0: //BFBB
                    dt.RemoveProperty("Crate_Types");
                    dt.RemoveProperty("Crates_Allow_Any_Type");
                    dt.RemoveProperty("CrateProbabilities");
                    dt.RemoveProperty("EnemyProbabilitiesMovie");
                    dt.RemoveProperty("PowerupCheatsScooby");
                    dt.RemoveProperty("PowerupCheatsMovie");
                    dt.RemoveProperty("FloatingBlockChallenge");

                    bootLevel = "HB01";
                    break;
                case 1: // Scooby
                    dt.RemoveProperty("Disco_Floors");
                    dt.RemoveProperty("Tiki_Types");
                    dt.RemoveProperty("Tiki_Models");
                    dt.RemoveProperty("Tiki_Allow_Any_Type");
                    dt.RemoveProperty("TikiProbabilities");
                    dt.RemoveProperty("Crate_Types");
                    dt.RemoveProperty("Crates_Allow_Any_Type");
                    dt.RemoveProperty("CrateProbabilities");
                    dt.RemoveProperty("Enemy_Types");
                    dt.RemoveProperty("Enemies_Allow_Any_Type");
                    dt.RemoveProperty("EnemyProbabilities");
                    dt.RemoveProperty("EnemyProbabilitiesMovie");
                    dt.RemoveProperty("Pointer_Positions");
                    dt.RemoveProperty("Teleport_Box_Positions");
                    dt.RemoveProperty("Taxi_Trigger_Positions");
                    dt.RemoveProperty("Bus_Stop_Trigger_Positions");
                    dt.RemoveProperty("Shiny_Object_Gates");
                    dt.RemoveProperty("shinyReqMin");
                    dt.RemoveProperty("shinyReqMax");
                    dt.RemoveProperty("Spatula_Gates");
                    dt.RemoveProperty("spatReqMin");
                    dt.RemoveProperty("spatReqMax");
                    dt.RemoveProperty("PlayerCharacters");
                    dt.RemoveProperty("UnlockCharacters");
                    dt.RemoveProperty("RandomCharacters");
                    dt.RemoveProperty("PowerupCheatsBFBB");
                    dt.RemoveProperty("PowerupCheatsMovie");
                    dt.RemoveProperty("allMenuWarpsHB01");
                    dt.RemoveProperty("invisibleLevel");
                    dt.RemoveProperty("openTeleportBoxes");
                    dt.RemoveProperty("disableFlythroughs");
                    dt.RemoveProperty("disableCutscenes");
                    dt.RemoveProperty("spatReqChum");
                    dt.RemoveProperty("restoreRobotLaugh");
                    dt.RemoveProperty("FloatingBlockChallenge");

                    bootLevel = "h001";
                    break;
                case 2: // Movie
                    dt.RemoveProperty("Tiki_Types");
                    dt.RemoveProperty("Tiki_Models");
                    dt.RemoveProperty("Tiki_Allow_Any_Type");
                    dt.RemoveProperty("TikiProbabilities");
                    dt.RemoveProperty("EnemyProbabilities");
                    dt.RemoveProperty("Taxi_Trigger_Positions");
                    dt.RemoveProperty("Shiny_Object_Gates");
                    dt.RemoveProperty("shinyReqMin");
                    dt.RemoveProperty("shinyReqMax");
                    dt.RemoveProperty("Spatula_Gates");
                    dt.RemoveProperty("spatReqMin");
                    dt.RemoveProperty("spatReqMax");
                    dt.RemoveProperty("PlayerCharacters");
                    dt.RemoveProperty("UnlockCharacters");
                    dt.RemoveProperty("RandomCharacters");
                    dt.RemoveProperty("PowerupCheatsBFBB");
                    dt.RemoveProperty("PowerupCheatsScooby");
                    dt.RemoveProperty("allMenuWarpsHB01");
                    dt.RemoveProperty("disableFlythroughs");
                    dt.RemoveProperty("spatReqChum");
                    dt.RemoveProperty("restoreRobotLaugh");

                    bootLevel = "BB02";
                    break;
                default:
                    throw new Exception("Invalid game");
            }
        }
    }

    public enum BootLevelMode
    {
        Default,
        Set,
        Random
    }
}