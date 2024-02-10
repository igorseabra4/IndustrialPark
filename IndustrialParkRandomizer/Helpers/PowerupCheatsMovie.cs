using System.ComponentModel;

namespace IndustrialPark.Randomizer
{
    public enum HealthCount
    {
        Health_3,
        Health_4,
        Health_5,
        Health_6
    }

    public enum PowerupStatus
    {
        Inactive,
        Given,
        Upgraded
    }

    public enum Preset
    {
        Nothing,
        Basic,
        Macho
    }

    public class PowerupCheatsMovie
    {
        private Preset _preset = Preset.Basic;
        public Preset Preset
        {
            get => _preset;
            set
            {
                _preset = value;
                switch (value)
                {
                    case Preset.Nothing:
                        SB_Health = HealthCount.Health_3;
                        SB_KarateSpin = PowerupStatus.Given;
                        SB_Bash = PowerupStatus.Inactive;
                        SB_Bowl = PowerupStatus.Inactive;
                        SB_Guitar = PowerupStatus.Inactive;
                        Pat_Health = HealthCount.Health_3;
                        Pat_StarSpin = PowerupStatus.Given;
                        Pat_Cartwheel = PowerupStatus.Inactive;
                        Pat_BellyFlop = PowerupStatus.Inactive;
                        Pat_Throw = PowerupStatus.Inactive;
                        break;
                    case Preset.Basic:
                        SB_Health = HealthCount.Health_3;
                        SB_KarateSpin = PowerupStatus.Given;
                        SB_Bash = PowerupStatus.Given;
                        SB_Bowl = PowerupStatus.Given;
                        SB_Guitar = PowerupStatus.Given;
                        Pat_Health = HealthCount.Health_3;
                        Pat_StarSpin = PowerupStatus.Given;
                        Pat_Cartwheel = PowerupStatus.Given;
                        Pat_BellyFlop = PowerupStatus.Given;
                        Pat_Throw = PowerupStatus.Given;
                        break;
                    case Preset.Macho:
                        SB_Health = HealthCount.Health_6;
                        SB_KarateSpin = PowerupStatus.Upgraded;
                        SB_Bash = PowerupStatus.Upgraded;
                        SB_Bowl = PowerupStatus.Upgraded;
                        SB_Guitar = PowerupStatus.Upgraded;
                        Pat_Health = HealthCount.Health_6;
                        Pat_StarSpin = PowerupStatus.Upgraded;
                        Pat_Cartwheel = PowerupStatus.Upgraded;
                        Pat_BellyFlop = PowerupStatus.Upgraded;
                        Pat_Throw = PowerupStatus.Upgraded;
                        break;
                }
            }
        }

        [Description(Constants.powerupExample)]
        public HealthCount SB_Health { get; set; } = HealthCount.Health_3;
        [Description(Constants.powerupExample)]
        public PowerupStatus SB_KarateSpin { get; set; } = PowerupStatus.Given;
        [Description(Constants.powerupExample)]
        public PowerupStatus SB_Bash { get; set; } = PowerupStatus.Given;
        [Description(Constants.powerupExample)]
        public PowerupStatus SB_Bowl { get; set; } = PowerupStatus.Given;
        [Description(Constants.powerupExample)]
        public PowerupStatus SB_Guitar { get; set; } = PowerupStatus.Given;

        [Description(Constants.powerupExample)]
        public HealthCount Pat_Health { get; set; } = HealthCount.Health_3;
        [Description(Constants.powerupExample)]
        public PowerupStatus Pat_StarSpin { get; set; } = PowerupStatus.Given;
        [Description(Constants.powerupExample)]
        public PowerupStatus Pat_Cartwheel { get; set; } = PowerupStatus.Given;
        [Description(Constants.powerupExample)]
        public PowerupStatus Pat_BellyFlop { get; set; } = PowerupStatus.Given;
        [Description(Constants.powerupExample)]
        public PowerupStatus Pat_Throw { get; set; } = PowerupStatus.Given;

        public override int GetHashCode()
        {
            int hashCode = -1866944443;
            hashCode = hashCode * -1521134295 + _preset.GetHashCode();
            hashCode = hashCode * -1521134295 + Preset.GetHashCode();
            hashCode = hashCode * -1521134295 + SB_Health.GetHashCode();
            hashCode = hashCode * -1521134295 + SB_KarateSpin.GetHashCode();
            hashCode = hashCode * -1521134295 + SB_Bash.GetHashCode();
            hashCode = hashCode * -1521134295 + SB_Bowl.GetHashCode();
            hashCode = hashCode * -1521134295 + SB_Guitar.GetHashCode();
            hashCode = hashCode * -1521134295 + Pat_Health.GetHashCode();
            hashCode = hashCode * -1521134295 + Pat_StarSpin.GetHashCode();
            hashCode = hashCode * -1521134295 + Pat_Cartwheel.GetHashCode();
            hashCode = hashCode * -1521134295 + Pat_BellyFlop.GetHashCode();
            hashCode = hashCode * -1521134295 + Pat_Throw.GetHashCode();
            return hashCode;
        }
    }
}