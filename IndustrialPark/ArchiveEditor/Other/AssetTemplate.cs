namespace IndustrialPark
{
    public enum AssetTemplate
    {
        Null,
        UserTemplate,

        // Controllers
        Group,
        Counter,
        Conditional,
        Dispatcher,
        Portal,
        Text,
        Timer,
        DuplicatotronSettings,

        // Pickups and Tikis
        Shiny_Red,
        Shiny_Yellow,
        Shiny_Green,
        Shiny_Blue,
        Shiny_Purple,
        Underwear,
        Spatula,
        Sock,
        Spongeball,
        WoodenTiki,
        FloatingTiki,
        ThunderTiki,
        ShhhTiki,
        StoneTiki,

        // Enemies
        Fodder,
        Hammer,
        TarTar,
        ChompBot,
        GLove,
        Chuck,
        Chuck_Trigger,
        Monsoon,
        Monsoon_Trigger,
        Sleepytime,
        Sleepytime_Moving,
        BombBot,
        Tubelet,
        BzztBot,
        Slick,
        Slick_Trigger,
        Jellyfish_Pink,
        Jellyfish_Blue,
        Duplicatotron,

        // ...but not for user
        TubeletSlave,

        // Stage Items
        Button_Red,
        PressurePlate,
        TaxiStand,
        TexasHitch,
        CharSwitch,
        BusStop,

        // ...but not for user
        PressurePlateBase,
        CharacterSwitch_BusSimp,
        CharacterSwitch_Camera,
        BusStop_LightSimp,
        BusStop_Trigger,

        // Placeable
        Marker,
        SphereTrigger,
        PointMVPT,
        EnemyAreaMVPT,
        Boulder_Generic,
        Button_Generic,
        Camera,
        Destructible_Generic,
        ElectricArc_Generic,
        Platform_Generic,
        Player_Generic,
        SIMP_Generic,
        VIL_Generic,

        // Other
        AnimationList,
        CollisionTable,
        Environment,
        JawData,
        LevelOfDetailTable,
        MaterialMap,
        PipeInfoTable,
        ShadowTable,
        SoundInfo,
    }
}
