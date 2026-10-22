using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IndustrialPark.Randomizer
{
    public class RandomizerSettings
    {
        [Category("\tRandomizer"), DisplayName("Patterns/files to skip entirely"), Description("The randomizer will leave these files unnafected.\nRecommended: leave as default")]
        [Bfbb(new string[] { "hb10", "db05", "b301" })]
        [Scooby(new string[] { "s006" })]
        [Movie(new string[] { "b402" })]
        public string[] skipFiles { get; set; }

        [Category("\tRandomizer"), Description("Randomize pickup locations among each other within a room.")]
        [All(true)]
        public bool Pickups { get; set; }

        [Category("\tRandomizer"), DisplayName("Disco Floor Patterns")]
        [Bfbb(true)]
        [Movie(true)]
        public bool Disco_Floors { get; set; }

        [Category("\tRandomizer")]
        [All(true)]
        public bool Music { get; set; }

        [Category("\tRandomizer"), DisplayName("Floating Block Challenge")]
        [Movie(true)]
        public bool FloatingBlockChallenge { get; set; }

        [Category("\tRandomizer")]
        [All(false)]
        public bool Cameras { get; set; }

        [Category("\tRandomizer")]
        [All(false)]
        public bool Textures { get; set; }

        [Category("\tRandomizer"), DisplayName("Textures (Special)"), Description("Includes menu/HUD and particle textures.")]
        [All(false)]
        public bool Textures_Special { get; set; }

        [Category("\tRandomizer")]
        [All(false)]
        public bool Sounds { get; set; }

        [Category("\tRandomizer"), DisplayName("Mix Sound Types"), Description("NOT RECOMMENDED\nMixes sound effects and voices (SND/SNDS).")]
        [All(false)]
        public bool Mix_Sound_Types { get; set; }

        [Category("\tRandomizer"), DisplayName("Player Sounds"), Description("Randomizes the player's sound effects")]
        [Bfbb(false)]
        public bool PlayerSounds { get; set; }


        [Category("Power-Ups"), DisplayName("Power-Up Rooms"), Description("Randomizes the rooms where power-ups are located.")]
        [Scooby(true)]
        public bool Power_Up_Levels_Scooby { get; set; }

        [Category("Power-Ups"), DisplayName("Include Monster Tokens"), Description("Includes Monster Tokens in the Power-Up Rooms randomizer.")]
        [Scooby(true)]
        public bool Include_MonsterTokens { get; set; }

        [Category("Power-Ups"), DisplayName("Include Gum and Soap Upgrades"), Description("Includes Gum and Soap upgrades as power-ups in the Power-Up Rooms randomizer. Does not include the items in Mastermind Unmasked 2.")]
        [Scooby(true)]
        public bool Include_GumSoap_Upgrades { get; set; }

        [Category("Power-Ups"), DisplayName("Unlock Groundskeeper at Hub"), Description("The Groundskeeper will appear at the hub without requiring you to obtain the map first.")]
        [Scooby(true)]
        public bool Unlock_Groundskeeper { get; set; }

        [Category("Power-Ups"), DisplayName("Skip Springs Location"), Description("If Pickups (randomize pickup positions) is on, this keeps the power-up in Fear on the Pier 3 in its original location, preventing it from appearing in a difficult spot. This does not prevent the room's power-up from being randomized to something other than the Springs if Power-Up Rooms is enabled.")]
        [Scooby(false)]
        public bool Skip_Springs { get; set; }

        [Category("Power-Ups"), DisplayName("Skip Slippers and Lampshade"), Description("Keeps the Slippers and Lampshade in their original locations in On Edge in the Hedge 2 instead of randomizing them among the other power-ups.")]
        [Scooby(false)]
        public bool Skip_Slippers_Lampshade { get; set; }

        [Category("Power-Ups"), DisplayName("Enable Boss Hints"), Description("Reveals which power-up you will receive before fighting the Black Knight, Green Ghost, and Redbeard. Look for the hint near the room's start point.")]
        [Scooby(true)]
        public bool Power_Up_Levels_Scooby_Hints { get; set; }


        [Category("Boulder Settings"), DisplayName("Boulder Settings"), Description("Randomizes floating point values in boulders, such as speed, gravity, mass, lifetime etc.")]
        [Bfbb(false), Movie(false)]
        public bool BoulderSettings { get; set; }
        
        [Category("Boulder Settings"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(0.5f), Movie(0.5f)]
        public float boulderMin { get; set; }

        [Category("Boulder Settings"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(2f), Movie(2f)]
        public float boulderMax { get; set; }

        [Category("Warps"), Description("Randomizes destination of level warps.")]
        [Bfbb(true), Scooby(true), Movie(false)]
        public bool Warps { get; set; }

        [Category("Warps"), DisplayName("Skip Warps"), Description("The randomizer will leave warps to/from these files unnafected.\nRecommended: leave as default.\nDefault: final boss and post-game levels, plus Flying Dutchman boss as changing those warps can cause issues.")]
        [Bfbb(new string[] { "hb00", "gy04", "b3", "pg" })]
        [Scooby(new string[] { "s005", "s006" })]
        [Movie(new string[] { })] 
        public string[] skipFilesWarps { get; set; }

        [Category("Tikis/Crates"), DisplayName("Types"), Description("Randomizes tiki (BFBB)/crate (TSSM) types.")]
        [Bfbb(true), Movie(true)]
        public bool Tiki_Types { get; set; }

        [Category("Tikis/Crates"), DisplayName("Models"), Description("Allows a tiki's look and behavior to be different.")]
        [Bfbb(true)]
        public bool Tiki_Models { get; set; }
        
        [Category("Tikis/Crates"), DisplayName("Allow Any Type"), Description("Allows tikis/crates which were not present in the level before.")]
        [Bfbb(true)]
        [Movie(true)]
        public bool Tiki_Allow_Any_Type { get; set; }

        [Category("Tikis/Crates"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Probabilities")]
        [Bfbb, Movie]
        public TikiProbabilities TikiProbabilities { get; set; }

        [Category("Enemies"), DisplayName("Types"), Description("Randomizes enemy types.")]
        [Bfbb(true), Movie(true)]
        public bool Enemy_Types { get; set; }

        [Category("Enemies"), DisplayName("Allow Any Type"), Description("NOT RECOMMENDED FOR MOVIE\nAllows enemies which were not present in the level before.")]
        [Bfbb(true), Movie(false)]
        public bool Enemies_Allow_Any_Type { get; set; }

        [Category("Enemies"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Probabilities")]
        [Bfbb]
        public EnemyProbabilities EnemyProbabilities { get; set; }
        
        [Category("Enemies"), Description(Constants.probExample), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Probabilities")]
        [Movie]
        public EnemyProbabilitiesMovie EnemyProbabilitiesMovie { get; set; }

        [Category("MovePoint"), DisplayName("MovePoint Radius"), Description("Randomizes MovePoint asset radius. Enemies can move more and see you from further away.")]
        [Bfbb(true), Movie(true)]
        public bool MovePoint_Radius { get; set; }

        [Category("MovePoint"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(0.9f), Movie(0.9f)]
        public float mvptMin { get; set; }

        [Category("MovePoint"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(1.8f), Movie(1.8f)] 
        public float mvptMax { get; set; }

        [Category("Platform Speed"), DisplayName("Platform Speed"), Description("Randomizes spped of platform (moving) assets. Higher values mean faster platforms. Negative values create interesting results.")]
        [All(true)]
        public bool PlatformSpeed { get; set; }
        
        [Category("Platform Speed"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(1f), Movie(1f), Scooby(0.8f)]
        public float speedMin { get; set; }
        
        [Category("Platform Speed"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(5f), Movie(5f), Scooby(2f)]
        public float speedMax { get; set; }

        [Category("Positions"), Description(Constants.posHelpExample + " Markers are used in BFBB and Scooby for warping and checkpoints.")]
        [Bfbb(true), Movie(true)]
        public bool Markers { get; set; }
        
        [Category("Positions"), DisplayName("Pointer Positions"), Description("NOT RECOMMENDED FOR BFBB\n" + Constants.posHelpExample + " Pointers are used for talking to NPCs in BFBB, but for warps and checkpoints in Movie.")]
        [Bfbb(false), Movie(true)]
        public bool Pointer_Positions { get; set; }
        
        [Category("Positions"), DisplayName("Teleport Box Positions"), Description("NOT RECOMMENDED FOR BFBB\n" + Constants.posHelpExample)]
        [Bfbb(false), Movie(true)]
        public bool Teleport_Box_Positions { get; set; }
        
        [Category("Positions"), DisplayName("Taxi Trigger Positions"), Description("NOT RECOMMENDED\n" + Constants.posHelpExample)]
        [Bfbb(false)]
        public bool Taxi_Trigger_Positions { get; set; }
        
        [Category("Positions"), DisplayName("Bus Stop Trigger Positions"), Description("NOT RECOMMENDED\n" + Constants.posHelpExample)]
        [Bfbb(false), Movie(false)]
        public bool Bus_Stop_Trigger_Positions { get; set; }

        [Category("Timers"), Description("Randomizes timer values")]
        [All(true)]
        public bool Timers { get; set; }

        [Category("Timers"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [All(0.75f)]
        public float timerMin { get; set; }
        
        [Category("Timers"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [All(1.75f)]
        public float timerMax { get; set; }

        [Category("Scales"), DisplayName("Set Scales"), Description("Applies a fixed scale (size) to assets.")]
        [All(false)]
        public bool Set_Scale { get; set; }
        
        [Category("Scales"), DisplayName("Scale Factor X"), Description("Factor to scale the assets."), TypeConverter(typeof(SingleTypeConverter))]
        [All(1f)]
        public float scaleFactorX { get; set; }
        
        [Category("Scales"), DisplayName("Scale Factor Y"), Description("Factor to scale the assets."), TypeConverter(typeof(SingleTypeConverter))]
        [All(1f)]
        public float scaleFactorY { get; set; }
        
        [Category("Scales"), DisplayName("Scale Factor Z"), Description("Factor to scale the assets."), TypeConverter(typeof(SingleTypeConverter))]
        [All(1f)]
        public float scaleFactorZ { get; set; }

        [Category("Scales (Not recommended)"), DisplayName("Scales"), Description("NOT RECOMMENDED\nRandomizes scale (size) of Entity assets.")]
        [All(false)]
        public bool Scale_Of_Things { get; set; }
        
        [Category("Scales (Not recommended)"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [All(0.8f)]
        public float scaleMin { get; set; }
        
        [Category("Scales (Not recommended)"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [All(1.3f)]
        public float scaleMax { get; set; }

        [Category("Ring Challenges"), DisplayName("Ring Sizes"), Description("Randomizes size of rings.")]
        [Movie(true)]
        public bool RingSizes { get; set; }
        
        [Category("Ring Challenges"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Movie(0.2f)]
        public float ringScaleMin { get; set; }

        [Category("Ring Challenges"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Movie(2f)]
        public float ringScaleMax { get; set; }

        [Category("Shiny Object Gates"), DisplayName("Shiny Object Gates"), Description("Randomizes clam toll gate shiny object requirements. The requirement will be multiplied by a random value between minimum and maximum.")]
        [Bfbb(true)]
        public bool Shiny_Object_Gates { get; set; }

        [Category("Shiny Object Gates"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(0.6f)]
        public float shinyReqMin { get; set; }

        [Category("Shiny Object Gates"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Bfbb(1.5f)]
        public float shinyReqMax { get; set; }

        [Category("Snack Gates"), DisplayName("Snack Gates"), Description("Randomizes snack gate requirements. The requirement will be multiplied by a random value between minimum and maximum.")]
        [Scooby(true)]
        public bool SnackGates { get; set; }

        [Category("Snack Gates"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Scooby(0.5f)]
        public float snackReqMin { get; set; }

        [Category("Snack Gates"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [Scooby(1.5f)]
        public float snackReqMax { get; set; }

        [Category("Spatula Gates"), DisplayName("Spatula Gates"), Description("Randomizes spatula toll gate requirements in BFBB.")]
        [Bfbb(true)]
        public bool Spatula_Gates { get; set; }
        
        [Category("Spatula Gates"), DisplayName("Minimum"), Description(Constants.minMaxExample)]
        [Bfbb(5)]
        public int spatReqMin { get; set; }
        
        [Category("Spatula Gates"), DisplayName("Maximum"), Description(Constants.minMaxExample)]
        [Bfbb(70)]
        public int spatReqMax { get; set; }

        [Category("Combat Arena Challenges"), DisplayName("Enemy Counts")]
        [Movie(true)]
        public bool CombatArenaCounts { get; set; }

        [Category("Combat Arena Challenges"), DisplayName("Minimum"), Description(Constants.minMaxExample)]
        [Movie(5)]
        public int combatMin { get; set; }
        
        [Category("Combat Arena Challenges"), DisplayName("Maximum"), Description(Constants.minMaxExample)]
        [Movie(100)]
        public int combatMax { get; set; }


        [Category("Texture Animations"), DisplayName("Texture Animations"), Description("Randomizes speed of UV animations on surfaces.")]
        [All(true)]
        public bool Texture_Animations { get; set; }
        
        [Category("Texture Animations"), DisplayName("Minimum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [All(-1.5f)]
        public float surfMin { get; set; }
        
        [Category("Texture Animations"), DisplayName("Maximum"), Description(Constants.minMaxExample), TypeConverter(typeof(SingleTypeConverter))]
        [All(1.5f)]
        public float surfMax { get; set; }


        [Category("Colors"), Description("Randomizes colors of stuff.")]
        [Bfbb(true), Movie(true)]
        public bool Colors { get; set; }
       
        [Category("Colors"), DisplayName("Vertex Colors"), Description("Randomizes vertex colors of level models instead of mesh colors (basically, it's more chaotic)")]
        [Bfbb(false), Movie(false)]
        public bool VertexColors { get; set; }
        
        [Category("Colors"), DisplayName("Bright Colors"), Description("Colors randomizer results in lighter colors")]
        [Bfbb(true), Movie(true)]
        public bool brightColors { get; set; }
        
        [Category("Colors"), DisplayName("Strong Colors"), Description("Colors randomizer results in very saturated colors")]
        [Bfbb(false), Movie(false)]
        public bool strongColors { get; set; }


        [Category("Characters"), DisplayName("Player Characters"), Description("Randomizes the characters in each Bus Stop (Patrick or Sandy). This will automatically enable Unlock Characters.")]
        [Bfbb(false)]
        public bool PlayerCharacters { get; set; }
        
        [Category("Characters"), DisplayName("Unlock Characters"), Description("Allows playing as Patrick and Sandy on every level. This will automatically disable Enemies Allow Any Type")]
        [Bfbb(false)]
        public bool UnlockCharacters { get; set; }
        
        [Category("Characters"), DisplayName("Random Characters"), Description("Switches you to Spongebob, Patrick or Sandy randomly on every spawn.")]
        [Bfbb(false)]
        public bool RandomCharacters { get; set; }


        [Category("\tCheats"), DisplayName("Invincible")]
        [All(false)]
        public bool cheatInvincible { get; set; }

        [Category("\tCheats"), DisplayName("BFBB Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        [Bfbb]
        public PowerupCheatsBFBB PowerupCheatsBFBB { get; set; }
        
        [Category("\tCheats"), DisplayName("Scooby Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        [Scooby]
        public PowerupCheatsScooby PowerupCheatsScooby { get; set; }
        
        [Category("\tCheats"), DisplayName("Movie Cheats"), TypeConverter(typeof(ExpandableObjectConverter))]
        [Movie]
        public PowerupCheatsMovie PowerupCheatsMovie { get; set; }


        [Category("\t\tINI Mods"), DisplayName("Boot Level"), Description("Which level to start the game in.\n- Default: Unchanged.\n- Set: the one set in the box below.\n- Random: picks a random level.")]
        [All(BootLevelMode.Set)]
        public BootLevelMode bootLevelMode { get; set; }

        private string bootLevel;

        [Category("\t\tINI Mods"), DisplayName("Set Boot Level"), Description("If Boot Level is 'set', start the game in this level.")]
        [Bfbb("HB01")]
        [Scooby("h001")]
        [Movie("BB02")]
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

        [Category("\t\tINI Mods"), DisplayName("Skip Main Menu")]
        [All(false)]
        public bool dontShowMenuOnBoot { get; set; }

        [Category("\t\tINI Mods"), DisplayName("All Menu Warps HB01"), Description("If true, all menu warps will lead to the hub.")]
        [Bfbb(true)]
        public bool allMenuWarpsHB01 { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Restore Robot Laugh"), Description("Restores robot laugh sound, which is not present normally in the GameCube version of the game.")]
        [Bfbb(true)]
        public bool restoreRobotLaugh { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Widescreen Patch"), Description("Patches the game to run in a 16:9 resolution by modifying some assets and enabling a widescreen Gecko code.")]
        [Bfbb(false)]
        public bool widescreen { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Disable Cutscenes")]
        [Bfbb(true)]
        public bool disableCutscenes { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Disable Flythroughs")]
        [Bfbb(true)]
        public bool disableFlythroughs { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Skip Spatula Animations")]
        [Bfbb(true)]
        public bool skipSpatulaAnims { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Open Teleport Boxes")]
        [Bfbb(false)]
        [Movie(false)]
        public bool openTeleportBoxes { get; set; }

        [Category("\t\t\tPatches"),
            DisplayName("Spatulas for Chum Bucket Lab"),
            Description("This will be the amount of spatulas needed for the final boss. Set to -1 to include it in the Spatula Gates randomizer method.")]
        [Bfbb(75)]
        public int spatReqChum { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Invisible Level"), Description("Makes all levels invisible.")]
        [Bfbb(false)]
        [Movie(false)]
        public bool invisibleLevel { get; set; }

        [Category("\t\t\tPatches"), DisplayName("Invisible Objects"), Description("Makes all objects invisible.")]
        [Bfbb(false)]
        [Movie(false)]
        public bool invisibleObjects { get; set; }

        [Category("\t\t\tPatches: LODT"), DisplayName("boot.HIP LODT multiplier"),
            Description("If true, multiply the render distance for the pickups by this amount.")]
        [All(true)]
        public bool bootHipLodtMulti { get; set; }

        [Category("\t\t\tPatches: LODT"), DisplayName("boot.HIP LODT multiplier"),
            Description("If true, multiply the render distance for the pickups by this amount."),
            TypeConverter(typeof(SingleTypeConverter))]
        [All(2f)]
        public float lodtValue { get; set; }

        [Category("Scooby Level Patches"), DisplayName("Hedge 1: Unlock shortcut gate"), Description("Unlocks the locked gate near the start of On Edge in the Hedge Part 1 from both sides. This can prevent an important item from being stuck on the other side when warps, pickups and power-ups are randomized.")]
        [Scooby(true)]
        public bool Hedge1_UnlockGate { get; set; }

        [Category("Scooby Level Patches"), DisplayName("Don't Look Down 2: Softlock prevention"), Description("Adds a warp gate near the exit door in Don't Look Down, Scooby-Doo Part 2.")]
        [Scooby(true)]
        public bool DontLookDown_Softlock { get; set; }

        [Category("Scooby Level Patches"), DisplayName("Cellar 1: Door from this side"), Description("Lets you unlock the secret area in Who's Yella in the Cellar Part 1 from the initial side of the room. This can prevent an important item from being stuck on the other side.")]
        [Scooby(true)]
        public bool Cellar1_UnlockDoor { get; set; }
        
        [Category("Scooby Level Patches"), DisplayName("Wreck 1: Softlock prevention"), Description("Changes your warp position in Wreck on the Deck Part 1 to prevent being softlocked if you arrive here without the springs.")]
        [Scooby(true)]
        public bool Wreck1_Softlock { get; set; }
        
        [Category("Scooby Level Patches"), DisplayName("Redbeard: Softlock prevention"), Description("Adds a warp gate to Shiver Your Timbers, Scooby Part 2 (Redbeard battle). The warp gate only spawns if you arrive here without a power-up that can defeat enemies.")]
        [Scooby(true)]
        public bool Redbeard_Softlock { get; set; }

        [Category("Scooby Level Patches"), DisplayName("Aghast 1: Decks are slides"), Description("Turns slippery surfaces in Aghast by the Mast Part 1 into slides. Plungers let you walk on them, but you will slide off without them.")]
        [Scooby(false)]
        public bool Aghast1_DecksAreSlides { get; set; }

        public RandomizerSettings()
        {
            TikiProbabilities = new TikiProbabilities();
            EnemyProbabilities = new EnemyProbabilities();
            EnemyProbabilitiesMovie = new EnemyProbabilitiesMovie();

            PowerupCheatsBFBB = new PowerupCheatsBFBB();
            PowerupCheatsScooby = new PowerupCheatsScooby();
            PowerupCheatsMovie = new PowerupCheatsMovie();
        }

        public void SetAllFalse()
        {
            PowerupCheatsMovie.Preset = Preset.Nothing;
            bootLevelMode = BootLevelMode.Default;


            foreach (PropertyInfo property in GetType().GetProperties())
                if (property.PropertyType == typeof(bool) && property.CanWrite)
                    property.SetValue(this, false);
        }

        public void ResetToDefault(int game)
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var attributes = property.GetCustomAttributes<GameAttribute>(true).ToArray();

                var attribute =
                    attributes.FirstOrDefault(a => a.Game == game) ??
                    attributes.FirstOrDefault(a => a.Game == -1);

                if (attribute != null)
                {
                    if (attribute.Value != null && property.PropertyType.IsInstanceOfType(attribute.Value))
                        property.SetValue(this, attribute.Value);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(this, false);
                }
            }
        }

        public void SetDynamicProperties(DynamicTypeDescriptor dt, int game)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                var attributes = GetType()
                    .GetProperty(property.Name)?
                    .GetCustomAttributes<GameAttribute>(true)
                    .ToArray() ?? Array.Empty<GameAttribute>();

                if (!attributes.Any(a => a.Game == -1 || a.Game == game))
                    dt.RemoveProperty(property.Name);
            }
        }
    }
}