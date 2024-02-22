using System;

namespace IndustrialParkRandomizer.Helpers
{
    public enum HMM_Game
    {
        Null,
        Scooby,
        BFBB,
        Movie,
        Incredibles,
        Underminer,
        RatProto,
        Ratatouille,
        WallE,
        Up,
        TruthOrSquare,
        UFC,
        FamilyGuy,
        HollywoodWorkout
    }

    public class Mod
    {
        public HMM_Game Game { get; set; } = HMM_Game.Null;

        public string ModName { get; set; } = "";

        public string Author { get; set; } = "";

        public string Description { get; set; } = "";

        public string ModId { get; set; } = "";

        public string GameId { get; set; } = "";

        public string INIReplacements { get; set; } = "";

        public string MergeFiles { get; set; } = "";

        public string RemoveFiles { get; set; } = "";

        public string DOLPatches { get; set; } = "";

        public string ArCodes { get; set; } = "";

        public string GeckoCodes { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Mod()
        {
            CreatedAt = DateTime.Now.ToUniversalTime().Date;
            UpdatedAt = DateTime.Now.ToUniversalTime().Date;
        }
    }
}