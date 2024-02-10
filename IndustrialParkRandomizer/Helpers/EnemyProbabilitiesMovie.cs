namespace IndustrialPark.Randomizer
{
    public class EnemyProbabilitiesMovie
    {
        public int Fogger_Desert { get; set; } = 1;
        public int Fogger_Goofy { get; set; } = 2;
        public int Fogger_Junk { get; set; } = 2;
        public int Fogger_Plankton { get; set; } = 1;
        public int Fogger_Trench { get; set; } = 1;
        public int Fogger_Thug { get; set; } = 2;
        public int Fogger_Arena1 { get; set; } = 1;
        public int Fogger_Arena2 { get; set; } = 1;
        public int Fogger_Arena3 { get; set; } = 1;
        public int Slammer_Goofy { get; set; } = 2;
        public int Slammer_Desert { get; set; } = 2;
        public int Slammer_Thug { get; set; } = 2;
        public int Flinger_Desert { get; set; } = 2;
        public int Flinger_Trench { get; set; } = 2;
        public int Flinger_Junk { get; set; } = 2;
        public int Spinner_Thug { get; set; } = 2;
        public int Spinner_Junk { get; set; } = 2;
        public int Spinner_Plankton { get; set; } = 2;
        public int Popper_Trench { get; set; } = 2;
        public int Popper_Plankton { get; set; } = 2;
        public int Minimerv { get; set; } = 2;
        public int Mervyn { get; set; } = 2;

        public override int GetHashCode()
        {
            int hashCode = -466139840;
            hashCode = hashCode * -1521134295 + Fogger_Desert.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Goofy.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Junk.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Plankton.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Trench.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Thug.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Arena1.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Arena2.GetHashCode();
            hashCode = hashCode * -1521134295 + Fogger_Arena3.GetHashCode();
            hashCode = hashCode * -1521134295 + Slammer_Goofy.GetHashCode();
            hashCode = hashCode * -1521134295 + Slammer_Desert.GetHashCode();
            hashCode = hashCode * -1521134295 + Slammer_Thug.GetHashCode();
            hashCode = hashCode * -1521134295 + Flinger_Desert.GetHashCode();
            hashCode = hashCode * -1521134295 + Flinger_Trench.GetHashCode();
            hashCode = hashCode * -1521134295 + Flinger_Junk.GetHashCode();
            hashCode = hashCode * -1521134295 + Spinner_Thug.GetHashCode();
            hashCode = hashCode * -1521134295 + Spinner_Junk.GetHashCode();
            hashCode = hashCode * -1521134295 + Spinner_Plankton.GetHashCode();
            hashCode = hashCode * -1521134295 + Popper_Trench.GetHashCode();
            hashCode = hashCode * -1521134295 + Popper_Plankton.GetHashCode();
            hashCode = hashCode * -1521134295 + Minimerv.GetHashCode();
            hashCode = hashCode * -1521134295 + Mervyn.GetHashCode();
            return hashCode;
        }
    }
}