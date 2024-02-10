using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public class PowerupCheatsScooby
    {
        [Description(Constants.powerupExample)]
        public bool Spring { get; set; } = false;
        [Description(Constants.powerupExample)]
        public bool Helmet { get; set; } = false;
        [Description(Constants.powerupExample)]
        public bool Smash { get; set; } = false;
        [Description(Constants.powerupExample)]
        public bool Umbrella { get; set; } = false;

        public override int GetHashCode()
        {
            int hashCode = -504631648;
            hashCode = hashCode * -1521134295 + Spring.GetHashCode();
            hashCode = hashCode * -1521134295 + Helmet.GetHashCode();
            hashCode = hashCode * -1521134295 + Smash.GetHashCode();
            hashCode = hashCode * -1521134295 + Umbrella.GetHashCode();
            return hashCode;
        }
    }
}