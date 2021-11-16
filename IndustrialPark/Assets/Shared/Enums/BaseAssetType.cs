﻿namespace IndustrialPark
{
    public enum BaseAssetType : byte
    {
        Unknown_Other = 0x00,
        Trigger = 0x01,
        Villain = 0x02,
        Player = 0x03,
        Pickup = 0x04,
        Env = 0x05,
        Platform = 0x06,
        Camera = 0x07,
        Door = 0x08,
        SavePoint = 0x09,
        Item = 0x0A,
        Static = 0x0B,
        Dynamic = 0x0C,
        MovePoint = 0x0D,
        Timer = 0x0E,
        Bubble = 0x0F,
        Portal = 0x10,
        Group = 0x11,
        Pendulum = 0x12,
        SFX = 0x13,
        FFX = 0x14,
        VFX = 0x15,
        Counter = 0x16,
        Hangable = 0x17,
        Button = 0x18,
        Projectile = 0x19,
        Surface = 0x1A,
        DestructObj = 0x1B,
        Gust = 0x1C,
        Volume = 0x1D,
        Dispatcher = 0x1E,
        Cond = 0x1F,
        UI = 0x20,
        UIFont = 0x21,
        ProjectileType = 0x22,
        LobMaster = 0x23,
        Fog = 0x24,
        Light = 0x25,
        ParticleEmitter = 0x26,
        ParticleSystem = 0x27,
        CutsceneMgr = 0x28,
        EGenerator = 0x29,
        Script = 0x2A,
        NPC = 0x2B,
        Hud = 0x2C,
        NPCProps = 0x2D,
        ParticleEmitterProps = 0x2E,
        Boulder = 0x2F,
        CruiseBubble = 0x30,
        TeleportBox = 0x31,
        BusStop = 0x32,
        TextBox = 0x33,
        TalkBox = 0x34,
        TaskBox = 0x35,
        BoulderGenerator = 0x36,
        NPCSettings = 0x37,
        DiscoFloor = 0x38,
        Taxi = 0x39,
        HUD_model = 0x3A,
        HUD_font_meter = 0x3B,
        HUD_unit_meter = 0x3C,
        BungeeHook = 0x3D,
        CameraFly = 0x3E,
        TrackPhysics = 0x3F,
        ZipLine = 0x40,
        Arena = 0x41,
        Duplicator = 0x42,
        LaserBeam = 0x43,
        Turret = 0x44,
        CameraTweak = 0x45,
        SlideProps = 0x46,
        HUD_text = 0x47,
        SGRP = 0x4A,
        SDFX = 0x4B,
        UIM = 0x53,
        SSET = 0x54,
        PGRS = 0x75,
        CCRV = 0x8D,
        RotuNGMS = 0x9A,
        DTRK_GRSM_IncNGMS = 0xCD
    }
}