namespace IndustrialPark
{
    public enum RandomizerFlags
    {
        LevelFiles = 0x01,
        Textures = 0x02,
        Sounds = 0x04,
        Boulder_Settings = 0x08,
        Pickup_Positions = 0x10,
        PLAT_Speeds = 0x20,
        Tiki_Types = 0x40,
        Tiki_Models = 0x80,
        Tiki_Allow_Any_Type = 0x100,
        MRKR_Positions = 0x200,
        DYNA_Pointers = 0x400,
        MVPT_Radius = 0x800,
        Cameras = 0x1000,
        Timers = 0x2000,
        LODT_Entries = 0x4000,
        ____ = 0x8000,
        Enemy_Types = 0x10000,
        Enemy_Models = 0x20000,
        Enemies_Allow_Any_Type = 0x40000,
        Mix_SND_SNDS = 0x80000,
        SIMP_Positions = 0x100000,
        Models = 0x200000
    }
}