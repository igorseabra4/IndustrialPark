using System;
using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class RandomizerSettings
    {
        [Category("Boulder Settings"), DisplayName("Boulder Settings"), Description("Randomizes floating point values in boulders, such as speed, gravity, mass, lifetime etc.")]
        public bool BoulderSettings { get; set; }
        [Category("Boulder Settings"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float boulderMin { get; set; } = 0.5f;
        [Category("Boulder Settings"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float boulderMax { get; set; } = 2f;


        [Category("Randomizer"),
            DisplayName("Patterns/files to skip entirely"),
            Description("The randomizer will leave these files unnafected.")]
        public string[] skipFiles { get; set; }
        [Category("Randomizer")]
        public bool Pickups { get; set; }
        [Category("Randomizer"), DisplayName("Disco Floors")]
        public bool Disco_Floors { get; set; }
        [Category("Randomizer")]
        public bool Music { get; set; }
        [Category("Randomizer"), DisplayName("Floating Block Challenge")]
        public bool FloatingBlockChallenge { get; set; }
        [Category("Randomizer")]
        public bool Cameras { get; set; }
        [Category("Randomizer")]
        public bool Textures { get; set; }
        [Category("Randomizer"), DisplayName("Textures (Special)"), Description("Includes menu/HUD and particle textures.")]
        public bool Textures_Special { get; set; }
        [Category("Randomizer")]
        public bool Sounds { get; set; }

        [Category("Randomizer (Not recommended)"), DisplayName("Mix Sound Types"), Description("NOT RECOMMENDED\nMixes sound effects and voices.")]
        public bool Mix_Sound_Types { get; set; }


        [Category("Warps"), Description("Randomizes destination of level warps.")]
        public bool Warps { get; set; }
        [Category("Warps"), DisplayName("Skip Warps"), Description("The randomizer will leave warps to/from these files unnafected.")]
        public string[] skipFilesWarps { get; set; }

        [Category("Tikis/Crates"), DisplayName("Types"), Description("Randomizes tiki (BFBB)/crate (TSSM) types.")]
        public bool Tiki_Types { get; set; }
        [Category("Tikis/Crates"), DisplayName("Models"), Description("Allows a tiki's look and behavior to be different.")]
        public bool Tiki_Models { get; set; }
        [Category("Tikis/Crates"), DisplayName("Allow Any Type"), Description("Allows tikis/crates which were not present in the level before.")]
        public bool Tiki_Allow_Any_Type { get; set; }
        [Category("Tikis/Crates"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Probabilities")]
        public TikiProbabilities TikiProbabilities { get; set; } = new TikiProbabilities();

        [Category("Enemies"), DisplayName("Types"), Description("Randomizes enemy types.")]
        public bool Enemy_Types { get; set; }
        [Category("Enemies"), DisplayName("Allow Any Type"), Description("NOT RECOMMENDED FOR MOVIE\nAllows enemies which were not present in the level before.")]
        public bool Enemies_Allow_Any_Type { get; set; }
        [Category("Enemies"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Probabilities")]
        public EnemyProbabilities EnemyProbabilities { get; set; } = new EnemyProbabilities();
        [Category("Enemies"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Probabilities")]
        public EnemyProbabilitiesMovie EnemyProbabilitiesMovie { get; set; } = new EnemyProbabilitiesMovie();


        [Category("MovePoint"), DisplayName("MovePoint Radius"), Description("Randomizes MovePoint asset radius. Enemies can move more and see you from further away.")]
        public bool MovePoint_Radius { get; set; }
        [Category("MovePoint"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float mvptMin { get; set; } = 0.9f;
        [Category("MovePoint"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float mvptMax { get; set; } = 1.8f;


        [Category("Platform Speed"), DisplayName("Platform Speed"), Description("Randomizes spped of platform (moving) assets. Higher values mean faster platforms. Negative values create interesting results.")]
        public bool PlatformSpeed { get; set; }
        [Category("Platform Speed"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float speedMin { get; set; }
        [Category("Platform Speed"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float speedMax { get; set; }


        [Category("Position Helpers"), Description(Constants.posHelpExample + " Markers are used in BFBB and Scooby for warping and checkpoints.")]
        public bool Markers { get; set; }
        [Category("Position Helpers"), DisplayName("Player Start"), Description(Constants.posHelpExample)]
        public bool Player_Start { get; set; }
        [Category("Position Helpers"), DisplayName("Pointer Positions"), Description("NOT RECOMMENDED FOR BFBB\n" + Constants.posHelpExample + " Pointers are used for talking to NPCs in BFBB, but for warping and checkpoints in Movie.")]
        public bool Pointer_Positions { get; set; }
        [Category("Position Helpers"), DisplayName("Teleport Box Positions"), Description("NOT RECOMMENDED FOR BFBB\n" + Constants.posHelpExample)]
        public bool Teleport_Box_Positions { get; set; }
        [Category("Position Helpers"), DisplayName("Taxi Trigger Positions"), Description("NOT RECOMMENDED\n" + Constants.posHelpExample)]
        public bool Taxi_Trigger_Positions { get; set; }
        [Category("Position Helpers"), DisplayName("Bus Stop Trigger Positions"), Description("NOT RECOMMENDED\n" + Constants.posHelpExample)]
        public bool Bus_Stop_Trigger_Positions { get; set; }


        [Category("Timers"), Description("Randomizes timer values")]
        public bool Timers { get; set; }
        [Category("Timers"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float timerMin { get; set; }
        [Category("Timers"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float timerMax { get; set; }


        [Category("Scales"), DisplayName("Set Scales"), Description("Applies a fixed scale (size) to assets.")]
        public bool Set_Scale { get; set; }
        [Category("Scales"), DisplayName("Scale Factor X"), Description("Factor to scale the assets."), TypeConverter(typeof(SingleTypeConverter))]
        public float scaleFactorX { get; set; }
        [Category("Scales"), DisplayName("Scale Factor Y"), Description("Factor to scale the assets."), TypeConverter(typeof(SingleTypeConverter))]
        public float scaleFactorY { get; set; }
        [Category("Scales"), DisplayName("Scale Factor Z"), Description("Factor to scale the assets."), TypeConverter(typeof(SingleTypeConverter))]
        public float scaleFactorZ { get; set; }
        //[Category("Scales"), DisplayName("Scale Physics"), Description("Applies the scale to the player's movement (experimental).")]
        //public bool Set_Scale_Physics { get; set; }

        [Category("Scales (Not recommended)"), DisplayName("Scales"), Description("NOT RECOMMENDED\nRandomizes scale (size) of Entity assets.")]
        public bool Scale_Of_Things { get; set; }
        [Category("Scales (Not recommended)"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float scaleMin { get; set; }
        [Category("Scales (Not recommended)"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float scaleMax { get; set; }


        [Category("Ring Challenges"), DisplayName("Ring Sizes"), Description("Randomizes size of rings.")]
        public bool RingSizes { get; set; }
        [Category("Ring Challenges"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float ringScaleMin { get; set; }
        [Category("Ring Challenges"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float ringScaleMax { get; set; }


        [Category("Shiny Object/Snack Gates"), DisplayName("Shiny Object/Snack Gates"), Description("Randomizes clam/toll gate shiny object requirements in BFBB or snack gate requirements in Scooby.")]
        public bool Shiny_Object_Gates { get; set; }
        [Category("Shiny Object/Snack Gates"),
            DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float shinyReqMin { get; set; }
        [Category("Shiny Object/Snack Gates"),
            DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float shinyReqMax { get; set; }


        [Category("Spatula Gates"), DisplayName("Spatula Gates"), Description("Randomizes spatula toll gate requirements in BFBB.")]
        public bool Spatula_Gates { get; set; }
        [Category("Spatula Gates"),
            DisplayName("Minimum"), Description(Constants.minMaxExample)]
        public int spatReqMin { get; set; }
        [Category("Spatula Gates"),
            DisplayName("Maximum"), Description(Constants.minMaxExample)]
        public int spatReqMax { get; set; }


        [Category("Combat Arena Challenges"), DisplayName("Enemy Counts")]
        public bool CombatArenaCounts { get; set; }
        [Category("Combat Arena Challenges"),
            DisplayName("Minimum"), Description(Constants.minMaxExample)]
        public int combatMin { get; set; }
        [Category("Combat Arena Challenges"),
            DisplayName("Maximum"), Description(Constants.minMaxExample)]
        public int combatMax { get; set; }


        [Category("Texture Animations"), DisplayName("Texture Animations"), Description("Randomizes speed of UV animations on surfaces.")]
        public bool Texture_Animations { get; set; }
        [Category("Texture Animations"),
            DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float surfMin { get; set; }
        [Category("Texture Animations"),
            DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        public float surfMax { get; set; }


        [Category("Colors"), Description("Randomizes colors of stuff.")]
        public bool Colors { get; set; }
        [Category("Colors"), DisplayName("Vertex Colors"), Description("Randomizes vertex colors of level models instead of mesh colors (basically, it's more chaotic)")]
        public bool VertexColors { get; set; }
        [Category("Colors"), DisplayName("Bright Colors"), Description("Colors randomizer results in lighter colors")]
        public bool brightColors { get; set; }
        [Category("Colors"), DisplayName("Strong Colors"), Description("Colors randomizer results in very saturated colors")]
        public bool strongColors { get; set; }


        [Category("Characters"), DisplayName("Player Characters"), Description("Randomizes the characters in each Bus Stop (Patrick or Sandy). This will automatically enable Unlock Characters.")]
        public bool PlayerCharacters { get; set; }
        [Category("Characters"), DisplayName("Unlock Characters"), Description("Allows playing as Patrick and Sandy on every level. This will automatically disable Enemies Allow Any Type")]
        public bool UnlockCharacters { get; set; }
        [Category("Characters"), DisplayName("Random Characters"), Description("Switches you to Spongebob, Patrick or Sandy randomly on every spawn.")]
        public bool RandomCharacters { get; set; }


        [Category("Cheats"), DisplayName("Invincible")]
        public bool cheatInvincible { get; set; } = false;
        [Category("Cheats"), DisplayName("BFBB Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        public PowerupCheatsBFBB PowerupCheatsBFBB { get; set; }
        [Category("Cheats"), DisplayName("Scooby Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        public PowerupCheatsScooby PowerupCheatsScooby { get; set; }
        [Category("Cheats"), DisplayName("Movie Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        public PowerupCheatsMovie PowerupCheatsMovie { get; set; }


        [Category("INI Mods"), DisplayName("Boot Level"), Description("Which level to start the game in.\n- Default: Unchanged.\n- Set: the one set in the box below.\n- Random: picks a random level.")]
        public BootLevelMode bootLevelMode { get; set; }

        private string bootLevel;

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

        [Category("INI Mods"), DisplayName("Skip Main Menu")]
        public bool dontShowMenuOnBoot { get; set; }

        [Category("INI Mods"), DisplayName("All Menu Warps HB01"),
            Description("If true, all menu warps will lead to the hub.")]
        public bool allMenuWarpsHB01 { get; set; }

        [Category("Patches"), DisplayName("Restore Robot Laugh"), Description("Restores robot laugh sound, which is not present normally in the GameCube version of the game.")]
        public bool restoreRobotLaugh { get; set; }

        [Category("Patches"), DisplayName("Widescreen Menu"), Description("Makes the pause menu background fit a 16:9 resolution. Should be used with a widescreen code.")]
        public bool widescreenMenu { get; set; }

        [Category("Patches"), DisplayName("Disable Cutscenes")]
        public bool disableCutscenes { get; set; }

        [Category("Patches"), DisplayName("Disable Flythroughs")]
        public bool disableFlythroughs { get; set; }

        [Category("Patches"), DisplayName("Open Teleport Boxes")]
        public bool openTeleportBoxes { get; set; }

        [Category("Patches"),
            DisplayName("Spatulas for Chum Bucket Lab"),
            Description("This will be the amount of spatulas needed for the final boss. Set to -1 to include it in the Spatula Gates randomizer method.")]
        public int spatReqChum { get; set; }

        [Category("Patches"), DisplayName("Invisible Level"), Description("Makes all levels invisible.")]
        public bool invisibleLevel { get; set; }

        [Category("Patches"), DisplayName("Invisible Objects"), Description("Makes all objects invisible.")]
        public bool invisibleObjects { get; set; }

        [Category("Patches: LODT"), DisplayName("boot.HIP LODT multiplier"),
            Description("If true, multiply the render distance for the pickups by this amount.")]
        public bool bootHipLodtMulti { get; set; }

        [Category("Patches: LODT"), DisplayName("boot.HIP LODT multiplier"),
            Description("If true, multiply the render distance for the pickups by this amount."),
            TypeConverter(typeof(SingleTypeConverter))]
        public float lodtValue { get; set; }

        public RandomizerSettings(int game)
        {
            Pickups = true;
            BoulderSettings = true;
            MovePoint_Radius = true;
            PlatformSpeed = true;
            Music = true;
            Cameras = false;
            Textures = false;
            Textures_Special = false;
            Sounds = false;
            Mix_Sound_Types = false;
            Warps = true;
            Scale_Of_Things = false;
            Texture_Animations = true;
            Markers = true;
            Player_Start = true;
            Timers = true;
            Colors = true;
            brightColors = true;
            strongColors = false;
            cheatInvincible = false;
            bootLevelMode = BootLevelMode.Set;
            bootHipLodtMulti = true;
            dontShowMenuOnBoot = true;
            invisibleLevel = false;
            invisibleObjects = false;
            disableCutscenes = true;
            PlayerCharacters = false;
            UnlockCharacters = false;
            RandomCharacters = false;
            Taxi_Trigger_Positions = false;
            Bus_Stop_Trigger_Positions = false;
            openTeleportBoxes = false;
            widescreenMenu = false;
            Set_Scale = false;
            VertexColors = false;

            boulderMin = 0.5f;
            boulderMax = 2f;
            mvptMin = 0.9f;
            mvptMax = 1.8f;
            speedMin = 1f;
            speedMax = 5f;
            timerMin = 0.75f;
            timerMax = 1.75f;
            scaleMin = 0.8f;
            scaleMax = 1.3f;
            ringScaleMin = 0.2f;
            ringScaleMax = 2f;
            shinyReqMin = 0.6f;
            shinyReqMax = 1.5f;
            spatReqMin = 5;
            spatReqMax = 70;
            surfMin = -2f;
            surfMax = 2f;
            spatReqChum = 75;
            lodtValue = 2f;
            combatMin = 5;
            combatMax = 100;
            scaleFactorX = 1f;
            scaleFactorY = 1f;
            scaleFactorZ = 1f;

            skipFiles = new string[] { "font", "boot", "plat", "mn", "sp", "pl", "hb10", "db05", "b301", "s006", "b402" };
            skipFilesWarps = new string[] { "hb00", "gy04", "b3", "pg", "s005" };

            TikiProbabilities = new TikiProbabilities();
            EnemyProbabilities = new EnemyProbabilities();
            EnemyProbabilitiesMovie = new EnemyProbabilitiesMovie();

            PowerupCheatsBFBB = new PowerupCheatsBFBB();
            PowerupCheatsScooby = new PowerupCheatsScooby();
            PowerupCheatsMovie = new PowerupCheatsMovie();

            ChangeForGame(game);
        }

        public void ChangeForGame(int game)
        {
            if (game == 0)
            {
                Spatula_Gates = true;
                Tiki_Models = true;
                Enemies_Allow_Any_Type = true;
                allMenuWarpsHB01 = true;
                disableFlythroughs = true;
                restoreRobotLaugh = true;
                bootLevel = "HB01";
            }
            else
            {
                Spatula_Gates = false;
                Tiki_Models = false;
                Enemies_Allow_Any_Type = false;
                allMenuWarpsHB01 = false;
                disableFlythroughs = false;
                restoreRobotLaugh = false;
                PlayerCharacters = false;
                UnlockCharacters = false;
                RandomCharacters = false;
                Taxi_Trigger_Positions = false;

                disableCutscenes = false; // remove this line later
            }

            if (game == 1)
            {
                Disco_Floors = false;
                Tiki_Types = false;
                Tiki_Allow_Any_Type = false;
                Enemy_Types = false;
                invisibleLevel = false;
                openTeleportBoxes = false;
                Bus_Stop_Trigger_Positions = false;
                bootLevel = "h001";
            }
            else
            {
                Disco_Floors = true;
                Tiki_Types = true;
                Tiki_Allow_Any_Type = true;
                Enemy_Types = true;
            }

            if (game == 2)
            {
                FloatingBlockChallenge = true;
                Pointer_Positions = true;
                Teleport_Box_Positions = true;
                CombatArenaCounts = true;
                RingSizes = true;
                Shiny_Object_Gates = false;
                bootLevel = "BB02";
            }
            else
            {
                FloatingBlockChallenge = false;
                Pointer_Positions = false;
                Teleport_Box_Positions = false;
                CombatArenaCounts = false;
                Shiny_Object_Gates = true;
                RingSizes = false;
            }
        }

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
            PlatformSpeed = false;
            BoulderSettings = false;
            Markers = false;
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
            Timers = false;
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
            CombatArenaCounts = false;
            Set_Scale = false;
        }

        public void SetDynamicProperties(DynamicTypeDescriptor dt, int game)
        {
            switch (game)
            {
                case 0: //BFBB
                    dt.RemoveProperty("EnemyProbabilitiesMovie");
                    dt.RemoveProperty("PowerupCheatsScooby");
                    dt.RemoveProperty("PowerupCheatsMovie");
                    dt.RemoveProperty("FloatingBlockChallenge");
                    dt.RemoveProperty("CombatArenaCounts");
                    dt.RemoveProperty("combatMin");
                    dt.RemoveProperty("combatMax");
                    dt.RemoveProperty("RingSizes");
                    dt.RemoveProperty("ringScaleMin");
                    dt.RemoveProperty("ringScaleMax");
                    break;
                case 1: // Scooby
                    dt.RemoveProperty("Disco_Floors");
                    dt.RemoveProperty("Tiki_Types");
                    dt.RemoveProperty("Tiki_Models");
                    dt.RemoveProperty("Tiki_Allow_Any_Type");
                    dt.RemoveProperty("TikiProbabilities");
                    dt.RemoveProperty("Enemy_Types");
                    dt.RemoveProperty("Enemies_Allow_Any_Type");
                    dt.RemoveProperty("EnemyProbabilities");
                    dt.RemoveProperty("EnemyProbabilitiesMovie");
                    dt.RemoveProperty("Pointer_Positions");
                    dt.RemoveProperty("Teleport_Box_Positions");
                    dt.RemoveProperty("Taxi_Trigger_Positions");
                    dt.RemoveProperty("Bus_Stop_Trigger_Positions");
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
                    dt.RemoveProperty("CombatArenaCounts");
                    dt.RemoveProperty("combatMin");
                    dt.RemoveProperty("combatMax");
                    dt.RemoveProperty("RingSizes");
                    dt.RemoveProperty("ringScaleMin");
                    dt.RemoveProperty("ringScaleMax");
                    dt.RemoveProperty("widescreenMenu");
                    break;
                case 2: // Movie
                    dt.RemoveProperty("Tiki_Models");
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
                    dt.RemoveProperty("widescreenMenu");


                    dt.RemoveProperty("disableCutscenes"); // remove this line later
                    break;
                default:
                    throw new Exception("Invalid game");
            }
        }
    }
}