using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class RandomizerSettings
    {
        [Category("Patterns/Files To Skip"),
            DisplayName("Skip Entirely"),
            Description("The randomizer will leave these files unnafected.")]
        public string[] skipFiles { get; set; } = new string[]
        {
            "font", "boot", "plat", "mn", "sp", "pl", "hb00", "hb10", "db05", "b301", "s006"
        };

        [Category("Patterns/Files To Skip"),
            DisplayName("Skip Warps"),
            Description("The randomizer will leave warps to/from these files unnafected (but still randomize other things).")]
        public string[] skipFilesWarps { get; set; } = new string[]
        {
            "gy04", "b3", "pg", "s005"
        };

        private const string probExample = "Example: say Wooden, Thunder, Shhh and Stone Tiki probabilities are set to 1, 3, 0 and 2 respectively. Add up the numbers and the total is 6. Tikis have 1/6 probability of being Wooden, 3/6 Thunder and 2/6 Stone. There will be no Shhh Tikis. This applies similarly to enemies. Setting the value to -1 makes the randomizer ignore all existing instances of that tiki/enemy completely (doesn't replace them).";

        [Category("Tiki Probabilities"), Description(probExample)]
        public int WoodenTiki { get; set; } = 1;
        [DisplayName("FloatingTiki (Not recommended)"), Category("Tiki Probabilities"), Description(probExample)]
        public int FloatingTiki { get; set; } = -1;
        [Category("Tiki Probabilities"), Description(probExample)]
        public int ThunderTiki { get; set; } = 1;
        [Category("Tiki Probabilities"), Description(probExample)]
        public int ShhhTiki { get; set; } = 1;
        [Category("Tiki Probabilities"), Description(probExample)]
        public int StoneTiki { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int Fodder { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int Hammer { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int Tartar { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int GLove { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int Chuck { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int Monsoon { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int Sleepytime { get; set; } = 1;
        [Category("Enemy Probabilities"), Description("Note: Arf and Tubelets are set to -1 by default due to chance of crashing the game.")]
        public int Arf { get; set; } = -1;
        [Category("Enemy Probabilities"), Description("Note: Arf and Tubelets are set to -1 by default due to chance of crashing the game.")]
        public int Tubelets { get; set; } = -1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int Slick { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int ChompBot { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int BombBot { get; set; } = 1;
        [Category("Enemy Probabilities"), Description(probExample)]
        public int BzztBot { get; set; } = 1;

        [Category("Platform Speed Multiplier"),
            DisplayName("Minimum"),
            Description("A random value between min. and max. is chosen to multiply the default speed for platforms. Higher speed multipliers mean faster platforms. Negative values create interesting results."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float speedMin { get; set; } = 1f;

        [Category("Platform Speed Multiplier"),
            DisplayName("Maximum"),
            Description("A random value between min. and max. is chosen to multiply the default speed for platforms. Higher speed multipliers mean faster platforms. Negative values create interesting results."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float speedMax { get; set; } = 5f;

        [Category("MovePoint Radius Multiplier"),
            DisplayName("Minimum"),
            Description("A random value between min. and max. is chosen to multiply the radius for MovePoint assets used by enemies."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float mvptMin { get; set; } = 0.9f;

        [Category("MovePoint Radius Multiplier"),
            DisplayName("Maximum"),
            Description("A random value between min. and max. is chosen to multiply the radius for MovePoint assets used by enemies."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float mvptMax { get; set; } = 1.8f;

        [Category("Boulder Settings Multiplier"),
            DisplayName("Minimum"),
            Description("A random value between min. and max. is chosen to multiply different settings for boulders, such as speed, gravity, mass, lifetime etc."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float boulderMin { get; set; } = 0.5f;

        [Category("Boulder Settings Multiplier"),
            DisplayName("Maximum"),
            Description("A random value between min. and max. is chosen to multiply different settings for boulders, such as speed, gravity, mass, lifetime etc."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float boulderMax { get; set; } = 2f;

        [Category("Timers Multiplier"),
            DisplayName("Minimum"),
            Description("A random value between min. and max. is chosen to multiply the time for Timer assets."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float timerMin { get; set; } = 0.75f;

        [Category("Timers Multiplier"),
            DisplayName("Maximum"),
            Description("A random value between min. and max. is chosen to multiply the time for Timer assets."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float timerMax { get; set; } = 1.75f;

        [Category("Scale Multiplier"),
            DisplayName("Minimum"),
            Description("A random value between min. and max. is chosen to multiply the scale of placeable assets."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float scaleMin { get; set; } = 0.8f;

        [Category("Scale Multiplier"),
            DisplayName("Maximum"),
            Description("A random value between min. and max. is chosen to multiply the scale of placeable assets."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float scaleMax { get; set; } = 1.3f;

        [Category("Shiny Object Gate Multiplier"),
            DisplayName("Minimum"),
            Description("A random value between min. and max. is chosen to multiply the shiny object requirement for clams/toll gates."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float shinyReqMin { get; set; } = 0.6f;

        [Category("Shiny Object Gate Multiplier"),
            DisplayName("Maximum"),
            Description("A random value between min. and max. is chosen to multiply the shiny object requirement for clams/toll gates."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float shinyReqMax { get; set; } = 1.5f;

        [Category("Spatula Gates"),
            DisplayName("Minimum"),
            Description("A random value between min. and max. is chosen for each spatula toll gate.")]
        public int spatReqMin { get; set; } = 5;

        [Category("Spatula Gates"),
            DisplayName("Maximum"),
            Description("A random value between min. and max. is chosen for each spatula toll gate.")]
        public int spatReqMax { get; set; } = 70;

        [Category("Spatula Gates"),
            DisplayName("Chum Bucket Lab Req."),
            Description("This value will override the amount of spatulas needed for the final boss.")]
        public int spatReqChum { get; set; } = 75;

        private string bootLevel = "HB01";

        [Category("Other"),
            DisplayName("Boot Level"),
            Description("Start the game in this level if 'Boot To Set Level' is on.")]
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

        [Category("Other"),
            DisplayName("Amount of warps to HB01"),
            Description("'Reduce Warps to HB01' reduces the amount of warps the game has to HB01 (around 17) to this number. The leftover warps are random.")]
        public int hb01Warps { get; set; } = 1;

        [Category("Other"),
            DisplayName("boot.HIP LODT multiplier"),
            Description("The 'Multiply boot.HIP LODT' method multiplies the render distance for the pickups by this amount."),
            TypeConverter(typeof(FloatTypeConverter))]
        public float bootHipLodtMulti { get; set; } = 2f;
    }
}