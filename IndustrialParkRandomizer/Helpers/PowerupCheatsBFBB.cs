using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class PowerupCheatsBFBB
    {
        [Description("Switches player on D-Pad.")]
        public bool PlayerSwitch { get; set; } = false;
        [Description(Constants.powerupExample)]
        public bool BubbleBowl { get; set; } = false;
        [Description(Constants.powerupExample)]
        public bool CruiseBubble { get; set; } = false;
    }
}