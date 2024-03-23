using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum ConditionalOperation
    {
        EQUAL_TO = 0,
        GREATER_THAN = 1,
        LESS_THAN = 2,
        GREATER_THAN_OR_EQUAL_TO = 3,
        LESS_THAN_OR_EQUAL_TO = 4,
        NOT_EQUAL_TO = 5
    }

    public enum ConditionalVariableScooby : uint
    {
        Chances = 0x8774A0EB,
        ScoobySnacks = 0x1BAA8CDE,
        SoundMode = 0x29600EB0,
        MasterVolume = 0x25C62258,
        MusicVolume = 0x84D4A26D,
        SFXVolume = 0x1E0EEB55,
        BubbleGum = 0xB92E6B67,
        SoapBar = 0xD6569A52,
        StickyBoots = 0xFF277F8A,
        Plungers = 0xDA82A36C,
        Slippers = 0x9AD0813E,
        Lampshade = 0xCDB894F9,
        BlackKnight = 0x73C46EBE,
        Flowerpos = 0x0D5591E3,
        DivingHelmet = 0x3FFC08DC,
        Spring = 0x133D9101,
        LightningBolt = 0x9DF961C7,
        FootballHelmet = 0x2F03BFDC,
        Umbrella = 0x7F54761C,
        Shovel = 0x866C5887,
        MemoryCardAvailable = 0x42453758,
        VibrationIsOn = 0x3B93C93F,
        LetterOfScene = 0x704D04A9,
        Room = 0x0B11B427,
        IsSneaking = 0x96F9FAD8,
        MapItemRecieved = 0x8F103F86,
        R001Unlocked = 0x42AA05B4,
        S001Unlocked = 0x834E83A5,
        FPieceCollected = 0xE395B9B9,
        LPieceCollected = 0x7D82D53F,
        WPieceCollected = 0xED0ADD0A,
        EPieceCollected = 0x1F438A78,
        CPieceCollected = 0x969F2BF6,
        GPieceCollected = 0xA7E7E8FA,
        PPieceCollected = 0x8ECB9243,
        BPieceCollected = 0xD24CFCB5,
        SPieceCollected = 0xDBC22006,
        IPieceCollected = 0x308C477C,
        RPieceCollected = 0x176FF0C5,
        OPieceCollected = 0xCA796302,
        HPieceCollected = 0x6C3A183B,
        F001Unlocked = 0x3AF41E68,
        E001Unlocked = 0xFA4FA077,
        MonsterToken1 = 0x2B4BE9E8,
        MonsterToken2 = 0x2B4BE9E9,
        MonsterToken3 = 0x2B4BE9EA,
        MonsterToken4 = 0x2B4BE9EB,
        MonsterToken5 = 0x2B4BE9EC,
        MonsterToken6 = 0x2B4BE9ED,
        MonsterToken7 = 0x2B4BE9EE,
        MonsterToken8 = 0x2B4BE9EF,
        MonsterToken9 = 0x2B4BE9F0,
        MonsterToken10 = 0x27D8B1E8,
        MonsterToken11 = 0x27D8B1E9,
        MonsterToken12 = 0x27D8B1EA,
        MonsterToken13 = 0x27D8B1EB,
        MonsterToken14 = 0x27D8B1EC,
        MonsterToken15 = 0x27D8B1ED,
        MonsterToken16 = 0x27D8B1EE,
        MonsterToken17 = 0x27D8B1EF,
        MonsterToken18 = 0x27D8B1F0,
        MonsterToken19 = 0x27D8B1F1,
        MonsterToken20 = 0x27D8B26B,
        MonsterToken21 = 0x27D8B26C,
        WarpPointB4 = 0xBE42FA2A,
        WarpPointC4 = 0xBE42FAAD,
        WarpPointE4 = 0xBE42FBB3,
        WarpPointE6 = 0xBE42FBB5,
        WarpPointE9 = 0xBE42FBB8,
        WarpPointF3 = 0xBE42FC35,
        WarpPointF7 = 0xBE42FC39,
        WarpPointF10 = 0x5C470E49,
        WarpPointG1 = 0xBE42FCB6,
        WarpPointG5 = 0xBE42FCBA,
        WarpPointG8 = 0xBE42FCBD,
        WarpPointH1 = 0xBE42FD39,
        WarpPointI3 = 0xBE42FDBE,
        WarpPointI6 = 0xBE42FDC1,
        WarpPointL14 = 0x5C48A083,
        WarpPointL15 = 0x5C48A084,
        WarpPointL18 = 0x5C48A087,
        WarpPointO4 = 0xBE4300D1,
        WarpPointO6 = 0xBE4300D3,
        WarpPointP3 = 0xBE430153,
        WarpPointP5 = 0xBE430155,
        WarpPointR3 = 0xBE430259,
        WarpPointS3 = 0xBE4302DC,
        WarpPointW22 = 0x5C4B8267,
        WarpPointW26 = 0x5C4B826B,
        LightningBoltStun = 0xCB345787,
        DefeatedMastermind = 0xA936CE02,
        WarpPointH3 = 0xBE42FD3B,
        AllSnacksBonusopen = 0x09B72C09,
        Cinematic1Unlocked = 0xEC766001,
        Cinematic2Unlocked = 0xC04D0164,
        Cinematic3Unlocked = 0x9423A2C7,
        Cinematic4Unlocked = 0x67FA442A,
        Cinematic5Unlocked = 0x3BD0E58D,
        Cinematic6Unlocked = 0x0FA786F0,
        Cinematic7Unlocked = 0xE37E2853,
        Cinematic8Unlocked = 0xB754C9B6,
        Cinematic9Unlocked = 0x8B2B6B19,
        Cinematic10Unlocked = 0x999E13F5,
        Cinematic11Unlocked = 0x6D74B558,
        Cinematic12Unlocked = 0x414B56BB,
        Cinematic13Unlocked = 0x1521F81E,
        Cinematic14Unlocked = 0xE8F89981,
    }

    public enum ConditionalVariableBFBB : uint
    {
        SoundMode = 0x29600EB0,
        MusicVolume = 0x84D4A26D,
        SFXVolume = 0x1E0EEB55,
        MemoryCardAvailable = 0x42453758,
        IsVibrationOn = 0x3B93C93F,
        LetterOfScene = 0x704D04A9,
        Room = 0x0B11B427,
        CurrentLevelCollectable = 0x9653DA31,
        PatSocks = 0x18249056,
        TotalPatsSocks = 0xD1FEEEE2,
        ShinyObjects = 0xD6FCCFE7,
        GoldenSpatulas = 0xC7E0F71C,
        CurrentDate = 0x9482683D,
        CurrentHour = 0x950F49B7,
        CurrentMinute = 0xBD2884E7,
        CounterValue = 0x4329EFFD,
        IsEnabled = 0xA6956B3F,
        IsVisible = 0x1E42996C
    }

    public enum ConditionalVariableTSSM : uint
    {
        SoundMode = 0x29600EB0,
        MusicVolume = 0x84D4A26D,
        SFXVolume = 0x1E0EEB55,
        MemoryCardAvailable = 0x42453758,
        IsVibrationOn = 0x3B93C93F,
        AreSubtitlesEnabled = 0xD1A7DE2C,
        LetterOfScene = 0x704D04A9,
        Room = 0x0B11B427,
        CurrentDate = 0x9482683D,
        CurrentHour = 0x950F49B7,
        CurrentMinute = 0xBD2884E7,
        CounterValue = 0x4329EFFD,
        IsEnabled = 0xA6956B3F,
        IsVisible = 0x1E42996C,
        TimerSecondsLeft = 0x6897B48B,
        TimerMillisecondsLeft = 0xF4FE2282,
        IsMNUS = 0x649FA12A,
        DemoType = 0x0B9F22CF,
        GoofyGooberTokens = 0x43DD1E00,
        ManlinessPoints = 0xD8A29291,
        LevelTreasureChests = 0xFE31C583,
        PlayerCurrentHealth = 0x25CD9F4A,
        IsReferenceNULL = 0x1F5BAA4D,
        AlwaysPortal = 0x5B85F809
    }

    public enum ConditionalVariableIncredibles : uint
    {
        SoundMode = 0x29600EB0,
        MusicVolume = 0x84D4A26D,
        SFXVolume = 0x1E0EEB55,
        MemoryCardAvailable = 0x42453758,
        VibrationIsOn = 0x3B93C93F,
        SubtitlesAreEnabled = 0xD1A7DE2C,
        LetterOfScene = 0x704D04A9,
        Room = 0x0B11B427,
        CurrentDate = 0x9482683D,
        CurrentHour = 0x950F49B7,
        CurrentMinute = 0xBD2884E7,
        CounterValue = 0x4329EFFD,
        IsEnabled = 0xA6956B3F,
        IsVisible = 0x1E42996C,
        TimerSecondsLeft = 0x6897B48B,
        TimerMillisecondsLeft = 0xF4FE2282,
        IsMNUS = 0x649FA12A,
        DemoType = 0x0B9F22CF,
        IsReferenceNULL = 0x1F5BAA4D,
        HitCheckpoints = 0x92D98D42,
        TotalCheckpoints = 0x37672DA7,
        TypeofPauseScreen = 0xFF9C67E9,
        UserPressYes = 0xB073B8AE,
        UserPressNo = 0x5D9F1817,
        UserPressBack = 0xEC46B2A1,
        GoStraightToMainMenu = 0x4698E733,
        PlayerType = 0x677D048D,
        IsSignedIn = 0x8B075E11,
        FriendRequestReceived = 0xE869B4AC,
        GameInviteReceived = 0x851982A6,
        ShowEnglishVideos = 0xEC21645F,
    }

    public enum ConditionalVariableROTU : uint
    {
        SoundMode = 0x29600EB0,
        MusicVolume = 0x84D4A26D,
        SFXVolume = 0x1E0EEB55,
        MemoryCardAvailable = 0x42453758,
        VibrationisonPlayer1 = 0x622827FB,
        Subtitlesareenabled = 0xD1A7DE2C,
        LetterOfScene = 0x704D04A9,
        Room = 0x0B11B427,
        CurrentDate = 0x9482683D,
        CurrentHour = 0x950F49B7,
        CurrentMinute = 0xBD2884E7,
        CounterValue = 0x4329EFFD,
        IsEnabled = 0xA6956B3F,
        IsVisible = 0x1E42996C,
        TimerSecondsLeft = 0x6897B48B,
        TimerMillisecondsLeft = 0xF4FE2282,
        IsMNUS = 0x649FA12A,
        DemoType = 0x0B9F22CF,
        IsReferenceNULL = 0x1F5BAA4D,
        HitCheckpoints = 0x92D98D42,
        TotalCheckpoints = 0x37672DA7,
        TypeofPauseScreen = 0xFF9C67E9,
        UserPressYes = 0xB073B8AE,
        UserPressNo = 0x5D9F1817,
        UserPressBack = 0xEC46B2A1,
        GoStraightToMainMenu = 0x4698E733,
        PlayerType = 0x677D048D,
        IsSignedIn = 0x8B075E11,
        FriendRequestReceived = 0xE869B4AC,
        GameInviteReceived = 0x851982A6,
        ShowEnglishVideos = 0xEC21645F,
        IsSceneUnlocked = 0xF3EDAAAA,
        IsSceneCompleted = 0xA307D09E,
        ShowHints = 0x9AAE2986,
        UpgradeMrIHealth = 0x24F7A6F0,
        UpgradeMrINuke = 0xD0D53495,
        UpgradeMrIMelee = 0xC9679B8C,
        UpgradeMrIThrow = 0x44B017FA,
        UpgradeMrISlam = 0xD17E5835,
        UpgradeFroHealth = 0x267BBD7D,
        UpgradeFroNuke = 0x7226B8FA,
        UpgradeFroMelee = 0x561E5B3B,
        UpgradeFroFreezeRay = 0x106EC5CE,
        UpgradeFroIceGlide = 0x8E41C5CB,
        IsOnePlayer = 0x63F93285,
        IsMrIAIHelper = 0x9F57C7BA,
        IsFrozoneAIHelper = 0x273B5B1B,
        IsHelperLooseFollow = 0x293FD7FB,
        IsHelperCloseFollow = 0x5FB4D5AD,
        IsHelperGuard = 0xBA0E1FAD,
        IsPAL = 0x6E0A67CD,
        VibrationisonPlayer2 = 0x622827FC,
        ShouldShowBonusOption = 0x4B520B7C,
        IsLanguageUS = 0x63A4F008,
        IsLanguageUK = 0x63A4F000,
        IsLanguageDE = 0x63A4E747,
        IsLanguageJP = 0x63A4EA64,
        IsLanguageKR = 0x63A4EAE9,
        IsLanguageRU = 0x63A4EE81,
        IsLanguageES = 0x63A4E7D8,
        IsLanguageIT = 0x63A4E9E5,
        IsLanguageFR = 0x63A4E85A,
    }

    public class AssetCOND : BaseAsset
    {
        private static string OperationShort(ConditionalOperation op)
        {
            switch (op)
            {
                case ConditionalOperation.EQUAL_TO:
                    return "=";
                case ConditionalOperation.GREATER_THAN:
                    return ">";
                case ConditionalOperation.LESS_THAN:
                    return "<";
                case ConditionalOperation.GREATER_THAN_OR_EQUAL_TO:
                    return ">=";
                case ConditionalOperation.LESS_THAN_OR_EQUAL_TO:
                    return "<=";
                case ConditionalOperation.NOT_EQUAL_TO:
                    return "!=";
            }
            return "";
        }

        private const string catName = "Conditional";

        public override string AssetInfo
        {
            get
            {
                string result;
                if (game == Game.Scooby)
                    result = Conditional_Scooby.ToString();
                else if (game == Game.BFBB)
                    result = Conditional_BFBB.ToString();
                else if (Enum.IsDefined(typeof(ConditionalVariableTSSM), (uint)ConditionalVariable_Hash))
                    result = Conditional_TSSM.ToString();
                else if (Enum.IsDefined(typeof(ConditionalVariableIncredibles), (uint)ConditionalVariable_Hash))
                    result = Conditional_Incredibles.ToString();
                else if (Enum.IsDefined(typeof(ConditionalVariableROTU), (uint)ConditionalVariable_Hash))
                    result = Conditional_ROTU.ToString();
                else
                    result = ConditionalVariable_Hash.ToString();
                result += " " + OperationShort(Operation) + " " + EvaluationAmount.ToString();

                if (game != Game.Scooby && AssetUnderEvaluation != 0)
                    result += " on " + HexUIntTypeConverter.StringFromAssetID(AssetUnderEvaluation);

                return result;
            }
        }

        [Category(catName), DisplayName("Conditional (Hash)"), IgnoreVerification]
        public AssetID ConditionalVariable_Hash { get; set; }
        [Category(catName), DisplayName("Conditional (Scooby)")]
        public ConditionalVariableScooby Conditional_Scooby
        {
            get => (ConditionalVariableScooby)(uint)ConditionalVariable_Hash;
            set => ConditionalVariable_Hash = (uint)value;
        }
        [Category(catName), DisplayName("Conditional (BFBB)")]
        public ConditionalVariableBFBB Conditional_BFBB
        {
            get => (ConditionalVariableBFBB)(uint)ConditionalVariable_Hash;
            set => ConditionalVariable_Hash = (uint)value;
        }
        [Category(catName), DisplayName("Conditional (TSSM)")]
        public ConditionalVariableTSSM Conditional_TSSM
        {
            get => (ConditionalVariableTSSM)(uint)ConditionalVariable_Hash;
            set => ConditionalVariable_Hash = (uint)value;
        }
        [Category(catName), DisplayName("Conditional (Incredibles)")]
        public ConditionalVariableIncredibles Conditional_Incredibles
        {
            get => (ConditionalVariableIncredibles)(uint)ConditionalVariable_Hash;
            set => ConditionalVariable_Hash = (uint)value;
        }
        [Category(catName), DisplayName("Conditional (ROTU)")]
        public ConditionalVariableROTU Conditional_ROTU
        {
            get => (ConditionalVariableROTU)(uint)ConditionalVariable_Hash;
            set => ConditionalVariable_Hash = (uint)value;
        }
        [Category(catName)]
        public ConditionalOperation Operation { get; set; }
        [Category(catName)]
        public int EvaluationAmount { get; set; }
        [Category(catName)]
        public AssetID AssetUnderEvaluation { get; set; }

        public AssetCOND(string assetName) : base(assetName, AssetType.Conditional, BaseAssetType.Cond)
        {
        }

        public AssetCOND(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                EvaluationAmount = reader.ReadInt32();
                ConditionalVariable_Hash = reader.ReadUInt32();
                Operation = (ConditionalOperation)reader.ReadUInt32();
                if (game != Game.Scooby)
                    AssetUnderEvaluation = reader.ReadUInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(EvaluationAmount);
            writer.Write(ConditionalVariable_Hash);
            writer.Write((uint)Operation);
            if (game != Game.Scooby)
                writer.Write(AssetUnderEvaluation);
            SerializeLinks(writer);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if ((int)Operation > 6)
                result.Add("Unknown operation type: " + Operation.ToString());
            if (ConditionalVariable_Hash == 0)
                result.Add("Conditional variable set to 0");
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("AssetUnderEvaluation");
            else
                dt.RemoveProperty("Conditional_Scooby");

            if (game != Game.BFBB)
                dt.RemoveProperty("Conditional_BFBB");

            if (game < Game.Incredibles)
            {
                dt.RemoveProperty("Conditional_TSSM");
                dt.RemoveProperty("Conditional_Incredibles");
                dt.RemoveProperty("Conditional_ROTU");
            }

            base.SetDynamicProperties(dt);
        }
    }
}