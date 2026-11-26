using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class PowerupCheatsBFBB
    {
        [Description("Switches between SpongeBob, Patrick or Sandy on D-Pad, in levels where they are available.")]
        public bool PlayerSwitch { get; set; } = false;
        [Description(Constants.powerupExample)]
        public bool BubbleBowl { get; set; } = false;
        [Description(Constants.powerupExample)]
        public bool CruiseBubble { get; set; } = false;

        public override int GetHashCode()
        {
            int hashCode = -1483288565;
            hashCode = hashCode * -1521134295 + PlayerSwitch.GetHashCode();
            hashCode = hashCode * -1521134295 + BubbleBowl.GetHashCode();
            hashCode = hashCode * -1521134295 + CruiseBubble.GetHashCode();
            return hashCode;
        }
    }
}