using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IndustrialPark.Randomizer;

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

    [Category("\tRandomizer"), DisplayName("Mix Sound Types"), Description("Mixes sound effects and voices (SND/SNDS). Not recommended as this can cause crashes.")]
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

    [Category("Enemies"), DisplayName("Allow Any Type"), Description("Allows enemies which were not present in the level before. Not recommended for Movie.")]
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

    [Category("Positions"), Description("Randomizes where you spawn after warps, and where checkpoints respawn you.")]
    [Bfbb(true)]
    public bool Markers { get; set; }
    
    [Category("Positions"), Description("Randomizes where you spawn after warps, and where checkpoints respawn you.")]
    [Movie(true)]
    public bool Pointers { get; set; }
    
    [Category("Positions"), DisplayName("Teleport Boxes"), Description("Randomizes the locations of teleport boxes in a level. This is not recommended for BFBB, as boxes sometimes appear on top of each other, causing a softlock.")]
    [Bfbb(false), Movie(true)]
    public bool Teleport_Box_Positions { get; set; }
    
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

    [Category("Scales (Not recommended)"), DisplayName("Scales"), Description("Randomizes scale (size) of Entity assets.")]
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

    [Category("Scooby Level Patches"), DisplayName("Wreck 1: Softlock prevention"), Description("Changes your spawn position in Wreck on the Deck, Part 1 to prevent being stuck if you arrive here without the springs.")]
    [Scooby(true)]
    public bool Wreck1_Softlock { get; set; }

    [Category("Scooby Level Patches"), DisplayName("Don't Look Down 2: Warp Gate"), Description("Adds a warp gate near the exit door in Don't Look Down, Scooby-Doo, Part 2 to prevent you from getting stuck there.")]
    [Scooby(true)]
    public bool DontLookDown_Softlock { get; set; }

    [Category("Scooby Level Patches"), DisplayName("Shiver 2: Warp Gate"), Description("Adds a warp gate to Shiver Your Timbers, Scooby, Part 2 (Redbeard battle). The warp gate only spawns if you arrive here without a power-up that can defeat enemies.")]
    [Scooby(true)]
    public bool Redbeard_Softlock { get; set; }

    [Category("Scooby Level Patches"), DisplayName("Aghast 1: Plungers stick to deck"), Description("Allows the plungers to function on the slippery surfaces in Aghast by the Mast, Part 1.")]
    [Scooby(false)]
    public bool Aghast1_PlungersWork { get; set; }

    [Category("Scooby Level Patches"), DisplayName("Cellar 1: Unlock door"), Description("Pulling down the chain will unlock both the exit door and the secret room door in Who's Yella in the Cellar, Part 1.")]
    [Scooby(false)]
    public bool Cellar1_UnlockDoor { get; set; }

    [Category("Scooby Level Patches"), DisplayName("Hedge 1: Unlock gate"), Description("Unlocks the locked gate near the start of On Edge in the Hedge, Part 1 from both sides.")]
    [Scooby(false)]
    public bool Hedge1_UnlockGate { get; set; }

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

    public override bool Equals(object obj)
    {
        return obj is RandomizerSettings settings &&
               EqualityComparer<string[]>.Default.Equals(skipFiles, settings.skipFiles) &&
               Pickups == settings.Pickups &&
               Disco_Floors == settings.Disco_Floors &&
               Music == settings.Music &&
               FloatingBlockChallenge == settings.FloatingBlockChallenge &&
               Cameras == settings.Cameras &&
               Textures == settings.Textures &&
               Textures_Special == settings.Textures_Special &&
               Sounds == settings.Sounds &&
               Mix_Sound_Types == settings.Mix_Sound_Types &&
               PlayerSounds == settings.PlayerSounds &&
               Power_Up_Levels_Scooby == settings.Power_Up_Levels_Scooby &&
               Include_MonsterTokens == settings.Include_MonsterTokens &&
               Include_GumSoap_Upgrades == settings.Include_GumSoap_Upgrades &&
               Unlock_Groundskeeper == settings.Unlock_Groundskeeper &&
               Skip_Springs == settings.Skip_Springs &&
               Skip_Slippers_Lampshade == settings.Skip_Slippers_Lampshade &&
               Power_Up_Levels_Scooby_Hints == settings.Power_Up_Levels_Scooby_Hints &&
               BoulderSettings == settings.BoulderSettings &&
               boulderMin == settings.boulderMin &&
               boulderMax == settings.boulderMax &&
               Warps == settings.Warps &&
               EqualityComparer<string[]>.Default.Equals(skipFilesWarps, settings.skipFilesWarps) &&
               Tiki_Types == settings.Tiki_Types &&
               Tiki_Models == settings.Tiki_Models &&
               Tiki_Allow_Any_Type == settings.Tiki_Allow_Any_Type &&
               EqualityComparer<TikiProbabilities>.Default.Equals(TikiProbabilities, settings.TikiProbabilities) &&
               Enemy_Types == settings.Enemy_Types &&
               Enemies_Allow_Any_Type == settings.Enemies_Allow_Any_Type &&
               EqualityComparer<EnemyProbabilities>.Default.Equals(EnemyProbabilities, settings.EnemyProbabilities) &&
               EqualityComparer<EnemyProbabilitiesMovie>.Default.Equals(EnemyProbabilitiesMovie, settings.EnemyProbabilitiesMovie) &&
               MovePoint_Radius == settings.MovePoint_Radius &&
               mvptMin == settings.mvptMin &&
               mvptMax == settings.mvptMax &&
               PlatformSpeed == settings.PlatformSpeed &&
               speedMin == settings.speedMin &&
               speedMax == settings.speedMax &&
               Markers == settings.Markers &&
               Pointers == settings.Pointers &&
               Teleport_Box_Positions == settings.Teleport_Box_Positions &&
               Timers == settings.Timers &&
               timerMin == settings.timerMin &&
               timerMax == settings.timerMax &&
               Set_Scale == settings.Set_Scale &&
               scaleFactorX == settings.scaleFactorX &&
               scaleFactorY == settings.scaleFactorY &&
               scaleFactorZ == settings.scaleFactorZ &&
               Scale_Of_Things == settings.Scale_Of_Things &&
               scaleMin == settings.scaleMin &&
               scaleMax == settings.scaleMax &&
               RingSizes == settings.RingSizes &&
               ringScaleMin == settings.ringScaleMin &&
               ringScaleMax == settings.ringScaleMax &&
               Shiny_Object_Gates == settings.Shiny_Object_Gates &&
               shinyReqMin == settings.shinyReqMin &&
               shinyReqMax == settings.shinyReqMax &&
               SnackGates == settings.SnackGates &&
               snackReqMin == settings.snackReqMin &&
               snackReqMax == settings.snackReqMax &&
               Spatula_Gates == settings.Spatula_Gates &&
               spatReqMin == settings.spatReqMin &&
               spatReqMax == settings.spatReqMax &&
               CombatArenaCounts == settings.CombatArenaCounts &&
               combatMin == settings.combatMin &&
               combatMax == settings.combatMax &&
               Texture_Animations == settings.Texture_Animations &&
               surfMin == settings.surfMin &&
               surfMax == settings.surfMax &&
               Colors == settings.Colors &&
               VertexColors == settings.VertexColors &&
               brightColors == settings.brightColors &&
               strongColors == settings.strongColors &&
               PlayerCharacters == settings.PlayerCharacters &&
               UnlockCharacters == settings.UnlockCharacters &&
               RandomCharacters == settings.RandomCharacters &&
               cheatInvincible == settings.cheatInvincible &&
               EqualityComparer<PowerupCheatsBFBB>.Default.Equals(PowerupCheatsBFBB, settings.PowerupCheatsBFBB) &&
               EqualityComparer<PowerupCheatsScooby>.Default.Equals(PowerupCheatsScooby, settings.PowerupCheatsScooby) &&
               EqualityComparer<PowerupCheatsMovie>.Default.Equals(PowerupCheatsMovie, settings.PowerupCheatsMovie) &&
               bootLevelMode == settings.bootLevelMode &&
               bootLevel == settings.bootLevel &&
               BootLevel == settings.BootLevel &&
               dontShowMenuOnBoot == settings.dontShowMenuOnBoot &&
               allMenuWarpsHB01 == settings.allMenuWarpsHB01 &&
               restoreRobotLaugh == settings.restoreRobotLaugh &&
               widescreen == settings.widescreen &&
               disableCutscenes == settings.disableCutscenes &&
               disableFlythroughs == settings.disableFlythroughs &&
               skipSpatulaAnims == settings.skipSpatulaAnims &&
               openTeleportBoxes == settings.openTeleportBoxes &&
               spatReqChum == settings.spatReqChum &&
               invisibleLevel == settings.invisibleLevel &&
               invisibleObjects == settings.invisibleObjects &&
               bootHipLodtMulti == settings.bootHipLodtMulti &&
               lodtValue == settings.lodtValue &&
               Hedge1_UnlockGate == settings.Hedge1_UnlockGate &&
               DontLookDown_Softlock == settings.DontLookDown_Softlock &&
               Cellar1_UnlockDoor == settings.Cellar1_UnlockDoor &&
               Wreck1_Softlock == settings.Wreck1_Softlock &&
               Redbeard_Softlock == settings.Redbeard_Softlock &&
               Aghast1_PlungersWork == settings.Aghast1_PlungersWork;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(skipFiles);
        hash.Add(Pickups);
        hash.Add(Disco_Floors);
        hash.Add(Music);
        hash.Add(FloatingBlockChallenge);
        hash.Add(Cameras);
        hash.Add(Textures);
        hash.Add(Textures_Special);
        hash.Add(Sounds);
        hash.Add(Mix_Sound_Types);
        hash.Add(PlayerSounds);
        hash.Add(Power_Up_Levels_Scooby);
        hash.Add(Include_MonsterTokens);
        hash.Add(Include_GumSoap_Upgrades);
        hash.Add(Unlock_Groundskeeper);
        hash.Add(Skip_Springs);
        hash.Add(Skip_Slippers_Lampshade);
        hash.Add(Power_Up_Levels_Scooby_Hints);
        hash.Add(BoulderSettings);
        hash.Add(boulderMin);
        hash.Add(boulderMax);
        hash.Add(Warps);
        hash.Add(skipFilesWarps);
        hash.Add(Tiki_Types);
        hash.Add(Tiki_Models);
        hash.Add(Tiki_Allow_Any_Type);
        hash.Add(TikiProbabilities);
        hash.Add(Enemy_Types);
        hash.Add(Enemies_Allow_Any_Type);
        hash.Add(EnemyProbabilities);
        hash.Add(EnemyProbabilitiesMovie);
        hash.Add(MovePoint_Radius);
        hash.Add(mvptMin);
        hash.Add(mvptMax);
        hash.Add(PlatformSpeed);
        hash.Add(speedMin);
        hash.Add(speedMax);
        hash.Add(Markers);
        hash.Add(Pointers);
        hash.Add(Teleport_Box_Positions);
        hash.Add(Timers);
        hash.Add(timerMin);
        hash.Add(timerMax);
        hash.Add(Set_Scale);
        hash.Add(scaleFactorX);
        hash.Add(scaleFactorY);
        hash.Add(scaleFactorZ);
        hash.Add(Scale_Of_Things);
        hash.Add(scaleMin);
        hash.Add(scaleMax);
        hash.Add(RingSizes);
        hash.Add(ringScaleMin);
        hash.Add(ringScaleMax);
        hash.Add(Shiny_Object_Gates);
        hash.Add(shinyReqMin);
        hash.Add(shinyReqMax);
        hash.Add(SnackGates);
        hash.Add(snackReqMin);
        hash.Add(snackReqMax);
        hash.Add(Spatula_Gates);
        hash.Add(spatReqMin);
        hash.Add(spatReqMax);
        hash.Add(CombatArenaCounts);
        hash.Add(combatMin);
        hash.Add(combatMax);
        hash.Add(Texture_Animations);
        hash.Add(surfMin);
        hash.Add(surfMax);
        hash.Add(Colors);
        hash.Add(VertexColors);
        hash.Add(brightColors);
        hash.Add(strongColors);
        hash.Add(PlayerCharacters);
        hash.Add(UnlockCharacters);
        hash.Add(RandomCharacters);
        hash.Add(cheatInvincible);
        hash.Add(PowerupCheatsBFBB);
        hash.Add(PowerupCheatsScooby);
        hash.Add(PowerupCheatsMovie);
        hash.Add(bootLevelMode);
        hash.Add(bootLevel);
        hash.Add(BootLevel);
        hash.Add(dontShowMenuOnBoot);
        hash.Add(allMenuWarpsHB01);
        hash.Add(restoreRobotLaugh);
        hash.Add(widescreen);
        hash.Add(disableCutscenes);
        hash.Add(disableFlythroughs);
        hash.Add(skipSpatulaAnims);
        hash.Add(openTeleportBoxes);
        hash.Add(spatReqChum);
        hash.Add(invisibleLevel);
        hash.Add(invisibleObjects);
        hash.Add(bootHipLodtMulti);
        hash.Add(lodtValue);
        hash.Add(Hedge1_UnlockGate);
        hash.Add(DontLookDown_Softlock);
        hash.Add(Cellar1_UnlockDoor);
        hash.Add(Wreck1_Softlock);
        hash.Add(Redbeard_Softlock);
        hash.Add(Aghast1_PlungersWork);
        return hash.ToHashCode();
    }
}