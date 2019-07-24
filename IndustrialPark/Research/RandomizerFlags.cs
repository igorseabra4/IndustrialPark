namespace IndustrialPark
{
    public enum RandomizerFlags
    {
        LevelFiles              = 1 << 0,
        Textures                = 1 << 1,
        Sounds                  = 1 << 2,
        Boulder_Settings        = 1 << 3,
        Pickup_Positions        = 1 << 4,
        PLAT_Speeds             = 1 << 5,
        Tiki_Types              = 1 << 6,
        Tiki_Models             = 1 << 7,
        Tiki_Allow_Any_Type     = 1 << 8,
        Enemy_Types             = 1 << 9,
        MRKR_Positions          = 1 << 10,
        DYNA_Pointers           = 1 << 11,
        MVPT_Radius             = 1 << 12,
        Cameras                 = 1 << 13,
        Timers                  = 1 << 14,
        Double_LODT_Distances   = 1 << 15,
        _Not_Recommended_       = 1 << 16,
        Mix_SND_SNDS            = 1 << 17,
        SIMP_Positions          = 1 << 18,
        Enemy_Models            = 1 << 19,
        Enemies_Allow_Any_Type  = 1 << 20,
        Models                  = 1 << 21
    }
}