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

        public override int GetHashCode()
        {
            int hashCode = 1036476329;
            hashCode = hashCode * -1521134295 + Fodder.GetHashCode();
            hashCode = hashCode * -1521134295 + Hammer.GetHashCode();
            hashCode = hashCode * -1521134295 + Tartar.GetHashCode();
            hashCode = hashCode * -1521134295 + GLove.GetHashCode();
            hashCode = hashCode * -1521134295 + Chuck.GetHashCode();
            hashCode = hashCode * -1521134295 + Monsoon.GetHashCode();
            hashCode = hashCode * -1521134295 + Sleepytime.GetHashCode();
            hashCode = hashCode * -1521134295 + Arf.GetHashCode();
            hashCode = hashCode * -1521134295 + Tubelets.GetHashCode();
            hashCode = hashCode * -1521134295 + Slick.GetHashCode();
            hashCode = hashCode * -1521134295 + ChompBot.GetHashCode();
            hashCode = hashCode * -1521134295 + BombBot.GetHashCode();
            hashCode = hashCode * -1521134295 + BzztBot.GetHashCode();
            return hashCode;
        }
    }
}