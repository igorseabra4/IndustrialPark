using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class EnemyProbabilities
    {
        public int Fodder { get; set; } = 1;
        public int Hammer { get; set; } = 1;
        public int Tartar { get; set; } = 1;
        public int GLove { get; set; } = 1;
        public int Chuck { get; set; } = 1;
        public int Monsoon { get; set; } = 1;
        public int Sleepytime { get; set; } = 1;
        [Description("Note: Arf and Tubelets are set to -1 by default due to chance of crashing the game.")]
        public int Arf { get; set; } = -1;
        [Description("Note: Arf and Tubelets are set to -1 by default due to chance of crashing the game.")]
        public int Tubelets { get; set; } = -1;
        public int Slick { get; set; } = 1;
        public int ChompBot { get; set; } = 1;
        public int BombBot { get; set; } = 1;
        public int BzztBot { get; set; } = 1;
    }
}