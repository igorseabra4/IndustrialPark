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
    }
}