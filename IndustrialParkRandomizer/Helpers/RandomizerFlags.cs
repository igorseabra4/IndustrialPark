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
        Player_Characters = 1 << 19,
        Cameras = 1 << 20,
        Textures = 1 << 21,
        Textures_Special = 1 << 22,
        Sounds = 1 << 23,
    }

    public enum RandomizerFlags2
    {
        Pointer_Positions        = 1 << 0,
        Teleport_Box_Positions   = 1 << 1,
        Taxi_Positions           = 1 << 2,
        Bus_Stop_Positions       = 1 << 3,
        Scale_Of_Things          = 1 << 4,
        Buttons                  = 1 << 5,
        Mix_SND_SNDS             = 1 << 6,
        Level_Files              = 1 << 7
    }
}