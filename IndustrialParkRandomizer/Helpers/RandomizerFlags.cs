namespace IndustrialPark.Randomizer
{
    public enum RandomizerFlags
    {
        Warps = 1 << 0,
        Pickup_Positions = 1 << 1,
        Tiki_Types = 1 << 2,
        Tiki_Models = 1 << 3,
        Tiki_Allow_Any_Type = 1 << 4,
        Enemy_Types = 1 << 5,
        Enemies_Allow_Any_Type = 1 << 6,
        MovePoint_Radius = 1 << 7,
        Platform_Speeds = 1 << 8,
        Boulder_Settings = 1 << 9,
        Marker_Positions = 1 << 10,
        Player_Start = 1 << 11,
        Timers = 1 << 12,
        Shiny_Object_Gates = 1 << 13,
        Spatula_Gates = 1 << 14,
        Disco_Floors = 1 << 15,
        Colors = 1 << 16,
        Texture_Animations = 1 << 17,
        Music = 1 << 18,
        Cameras = 1 << 19,
        Textures = 1 << 20,
        Sounds = 1 << 21,
    }

    public enum RandomizerFlags2
    {
        // Not recommended
        Level_Files              = 1 << 0,
        Pointer_Positions        = 1 << 1,
        Teleport_Box_Positions   = 1 << 2,
        Taxi_Positions           = 1 << 3,
        Bus_Stop_Positions       = 1 << 4,
        Scale_Of_Things          = 1 << 5,
        Button_Events            = 1 << 6,
        Mix_SND_SNDS             = 1 << 7,
        SIMP_Positions           = 1 << 8,
        Models                   = 1 << 9
    }
}