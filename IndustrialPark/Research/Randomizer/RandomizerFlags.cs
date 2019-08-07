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
        Player_Start            = 1 << 11,
        Shiny_Object_Gates      = 1 << 12,
        Spatula_Gates           = 1 << 13,
        Timers                  = 1 << 14,
        Music                   = 1 << 15,
        Disco_Floors            = 1 << 16,
        Textures                = 1 << 17,
        Sounds                  = 1 << 18,
        Cameras                 = 1 << 19,
        Reduce_Warps_To_HB01    = 1 << 20,
        Disable_Cutscenes       = 1 << 21,
        Set_FinalBoss_Spatulas  = 1 << 22,
        Multiply_BootHip_LODT   = 1 << 23,
    }

    public enum RandomizerFlagsP2
    {
        Level_Files              = 1 << 0,
        Pointer_Positions        = 1 << 1,
        Teleport_Box_Positions   = 1 << 2,
        Taxi_Positions           = 1 << 3,
        Bus_Stop_Positions       = 1 << 4,
        Scale_Of_Things          = 1 << 5,
        ButtonEvents             = 1 << 6,
        Mix_SND_SNDS             = 1 << 7,
        SIMP_Positions           = 1 << 8,
        Models                   = 1 << 9
    }

    public enum RandomizerFlagsP3
    {
        Boot_To_Set_Level      = 1 << 0,
        Random_Boot_Level      = 1 << 1,
        Dont_Show_Menu_On_Boot = 1 << 2,
        All_Menu_Warps_HB01    = 1 << 3,
        Cheat_Invincible       = 1 << 4,
        BobCheat_BubbleBowl    = 1 << 5,
        BobCheat_CruiseBubble  = 1 << 6,
        ScoobyCheat_Spring     = 1 << 7,
        ScoobyCheat_Helmet     = 1 << 8,
        ScoobyCheat_Smash      = 1 << 9,
        ScoobyCheat_Umbrella   = 1 << 10,
    }
}