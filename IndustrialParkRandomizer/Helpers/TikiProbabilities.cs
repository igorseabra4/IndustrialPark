using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class TikiProbabilities
    {
        [DisplayName("Wooden")]
        public int WoodenTiki { get; set; } = 1;
        [DisplayName("Floating")]
        [Description("Note: Floating Tiki set to -1 by default due to glitchy behavior when randomized.")]
        public int FloatingTiki { get; set; } = -1;
        [DisplayName("Thunder/Exploding")]
        public int ThunderTiki { get; set; } = 1;
        [DisplayName("Shhh/Shrink")]
        public int ShhhTiki { get; set; } = 1;
        [DisplayName("Stone/Steel")]
        public int StoneTiki { get; set; } = 1;

        public override int GetHashCode()
        {
            int hashCode = 919350124;
            hashCode = hashCode * -1521134295 + WoodenTiki.GetHashCode();
            hashCode = hashCode * -1521134295 + FloatingTiki.GetHashCode();
            hashCode = hashCode * -1521134295 + ThunderTiki.GetHashCode();
            hashCode = hashCode * -1521134295 + ShhhTiki.GetHashCode();
            hashCode = hashCode * -1521134295 + StoneTiki.GetHashCode();
            return hashCode;
        }
    }
}