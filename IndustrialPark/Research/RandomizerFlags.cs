namespace IndustrialPark
{
    public enum RandomizerFlags
    {
        Warps                   = 1 << 0,
        Pickup_Positions        = 1 << 1,
        Tiki_Types              = 1 << 2,
        Tiki_Models             = 1 << 3,
        Tiki_Allow_Any_Type     = 1 << 4,
        Enemy_Types             = 1 << 5,
        Enemies_Allow_Any_Type  = 1 << 6,
        MovePoint_Radius        = 1 << 7,
        Platform_Speeds         = 1 << 8,
        Boulder_Settings        = 1 << 9,
        Marker_Positions        = 1 << 10,
        Pointer_Positions       = 1 << 11,
        Player_Start            = 1 << 12,
        Timers                  = 1 << 13,
        Music                   = 1 << 14,
        Disco_Floors            = 1 << 15,
        Textures                = 1 << 16,
        Sounds                  = 1 << 17,
        Cameras                 = 1 << 18,
        Reduce_Warps_To_HB01    = 1 << 19,
        Double_BootHip_LODT     = 1 << 20
    }

    public enum RandomizerFlagsP2
    {
        Level_Files              = 1 << 0,
        Teleport_Box_Positions   = 1 << 1,
        Taxi_Positions           = 1 << 2,
        Bus_Stop_Positions       = 1 << 3,
        Mix_SND_SNDS             = 1 << 4,
        SIMP_Positions           = 1 << 5,
        Models                   = 1 << 6
    }

    public enum RandomizerFlagsP3
    {
        BootToHB01            = 1 << 0,
        RandomBootLevel       = 1 << 1,
        DontShowMenuOnBoot    = 1 << 2,
        AllMenuWarpsHB01      = 1 << 3,
        Cheat_Invincible      = 1 << 4,
        BobCheat_BubbleBowl   = 1 << 5,
        BobCheat_CruiseBubble = 1 << 6,
        ScoobyCheat_Spring    = 1 << 7,
        ScoobyCheat_Helmet    = 1 << 8,
        ScoobyCheat_Smash     = 1 << 9,
        ScoobyCheat_Umbrella  = 1 << 10,
    }
}