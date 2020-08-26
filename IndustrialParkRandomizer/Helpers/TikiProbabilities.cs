using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class TikiProbabilities
    {
        public int WoodenTiki { get; set; } = 1;
        [Description("Note: Floating Tiki set to -1 by default due to glitchy behavior when randomized.")]
        public int FloatingTiki { get; set; } = -1;
        public int ThunderTiki { get; set; } = 1;
        public int ShhhTiki { get; set; } = 1;
        public int StoneTiki { get; set; } = 1;
    }

    public class CrateProbabilities
    {
        [DisplayName("Wooden Crate")]
        public int WoodenTiki { get; set; } = 1;
        [DisplayName("Floating Crate")]
        [Description("Note: Floating Crate set to -1 by default due to glitchy behavior when randomized.")]
        public int FloatingTiki { get; set; } = -1;
        [DisplayName("Exploding Crate")]
        public int ThunderTiki { get; set; } = 1;
        [DisplayName("Shrink Crate")]
        public int ShhhTiki { get; set; } = 1;
        [DisplayName("Steel Crate")]
        public int StoneTiki { get; set; } = 1;
    }
}