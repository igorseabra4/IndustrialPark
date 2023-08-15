using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class PlatSpecific_Generic : GenericAssetDataContainer
    {
        public PlatSpecific_Generic(Game game)
        {
            _game = game;
        }

        public override void Serialize(EndianBinaryWriter writer) { }
    }

    public class PlatSpecific_ConveryorBelt : PlatSpecific_Generic
    {
        public AssetSingle Speed { get; set; }

        public PlatSpecific_ConveryorBelt(Game game) : base(game) { }
        public PlatSpecific_ConveryorBelt(EndianBinaryReader reader, Game game) : this(game)
        {
            Speed = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Speed);
        }
    }

    public class PlatSpecific_FallingPlatform : PlatSpecific_Generic
    {
        public AssetSingle Speed { get; set; }
        public AssetID BustModel { get; set; }

        public PlatSpecific_FallingPlatform(Game game) : base(game)
        {
            BustModel = 0;
        }
        public PlatSpecific_FallingPlatform(EndianBinaryReader reader, Game game) : this(game)
        {
            Speed = reader.ReadSingle();
            BustModel = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Speed);
            writer.Write(BustModel);
        }
    }

    public class PlatSpecific_FR : PlatSpecific_Generic
    {
        public AssetSingle fspeed { get; set; }
        public AssetSingle rspeed { get; set; }
        public AssetSingle ret_delay { get; set; }
        public AssetSingle post_ret_delay { get; set; }

        public PlatSpecific_FR(Game game) : base(game) { }
        public PlatSpecific_FR(EndianBinaryReader reader, Game game) : this(game)
        {
            fspeed = reader.ReadSingle();
            rspeed = reader.ReadSingle();
            ret_delay = reader.ReadSingle();
            post_ret_delay = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(fspeed);
            writer.Write(rspeed);
            writer.Write(ret_delay);
            writer.Write(post_ret_delay);
        }
    }

    public class PlatSpecific_BreakawayPlatform : PlatSpecific_Generic
    {
        public string Note => "BustModel is not present in Movie/Incredibles. UnknownFloat is only present in Movie/Incredibles.";

        public AssetSingle BreakawayDelay { get; set; }
        public AssetID BustModel { get; set; }
        public AssetSingle ResetDelay { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor("Allow sneak");
        public AssetSingle UnknownFloat { get; set; }

        public PlatSpecific_BreakawayPlatform(Game game) : base(game)
        {
            BustModel = 0;
        }

        public PlatSpecific_BreakawayPlatform(AssetTemplate template, Game game) : this(game)
        {
            BreakawayDelay = 1f;
            ResetDelay = 3f;
            Settings.FlagValueInt = 1;
            UnknownFloat = 0.1f;

            if (template == AssetTemplate.CollapsePlatform_Spongeball)
                BreakawayDelay = 0.4f;
        }

        public PlatSpecific_BreakawayPlatform(EndianBinaryReader reader, Game game) : this(game)
        {
            BreakawayDelay = reader.ReadSingle();
            if (game != Game.Incredibles)
                BustModel = reader.ReadUInt32();
            ResetDelay = reader.ReadSingle();
            Settings.FlagValueInt = reader.ReadUInt32();
            if (game == Game.Incredibles)
                UnknownFloat = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(BreakawayDelay);
            if (game != Game.Incredibles)
                writer.Write(BustModel);
            writer.Write(ResetDelay);
            writer.Write(Settings.FlagValueInt);
            if (game == Game.Incredibles)
                writer.Write(UnknownFloat);
        }
    }

    public class PlatSpecific_Springboard : PlatSpecific_Generic
    {
        public string Note => "HeightBubbleBounce and Settings are not present in Scooby.";

        public AssetSingle Height1 { get; set; }
        public AssetSingle Height2 { get; set; }
        public AssetSingle Height3 { get; set; }
        public AssetSingle HeightBubbleBounce { get; set; }
        public AssetID Animation1 { get; set; }
        public AssetID Animation2 { get; set; }
        public AssetID Animation3 { get; set; }
        public AssetSingle DirectionX { get; set; }
        public AssetSingle DirectionY { get; set; }
        public AssetSingle DirectionZ { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor(
            "Lock Camera Down",
            null,
            "Lock Player Control");

        public PlatSpecific_Springboard(Game game) : base(game)
        {
            Animation1 = 0;
            Animation2 = 0;
            Animation3 = 0;
        }

        public PlatSpecific_Springboard(EndianBinaryReader reader, Game game) : this(game)
        {
            Height1 = reader.ReadSingle();
            Height2 = reader.ReadSingle();
            Height3 = reader.ReadSingle();
            if (game != Game.Scooby)
                HeightBubbleBounce = reader.ReadSingle();
            Animation1 = reader.ReadUInt32();
            Animation2 = reader.ReadUInt32();
            Animation3 = reader.ReadUInt32();
            DirectionX = reader.ReadSingle();
            DirectionY = reader.ReadSingle();
            DirectionZ = reader.ReadSingle();
            if (game != Game.Scooby)
                Settings.FlagValueInt = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Height1);
            writer.Write(Height2);
            writer.Write(Height3);
            if (game != Game.Scooby)
                writer.Write(HeightBubbleBounce);
            writer.Write(Animation1);
            writer.Write(Animation2);
            writer.Write(Animation3);
            writer.Write(DirectionX);
            writer.Write(DirectionY);
            writer.Write(DirectionZ);
            if (game != Game.Scooby)
                writer.Write(Settings.FlagValueInt);
        }
    }

    public class PlatSpecific_TeeterTotter : PlatSpecific_Generic
    {
        private float InitialTilt_Rad { get; set; }
        private float MaxTilt_Rad { get; set; }
        public AssetSingle InitialTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(InitialTilt_Rad);
            set => InitialTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        public AssetSingle MaxTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(MaxTilt_Rad);
            set => MaxTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        public AssetSingle InverseMass { get; set; }

        public PlatSpecific_TeeterTotter(Game game) : base(game) { }
        public PlatSpecific_TeeterTotter(EndianBinaryReader reader, Game game) : base(game)
        {
            InitialTilt_Rad = reader.ReadSingle();
            MaxTilt_Rad = reader.ReadSingle();
            InverseMass = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(InitialTilt_Rad);
            writer.Write(MaxTilt_Rad);
            writer.Write(InverseMass);
        }
    }

    public class PlatSpecific_Paddle : PlatSpecific_Generic
    {
        public int StartOrient { get; set; }
        public int OrientCount { get; set; }
        public AssetSingle OrientLoop { get; set; }
        public AssetSingle Orient1 { get; set; }
        public AssetSingle Orient2 { get; set; }
        public AssetSingle Orient3 { get; set; }
        public AssetSingle Orient4 { get; set; }
        public AssetSingle Orient5 { get; set; }
        public AssetSingle Orient6 { get; set; }
        public FlagBitmask Settings { get; set; } = IntFlagsDescriptor();
        public AssetSingle RotateSpeed { get; set; }
        public AssetSingle AccelTime { get; set; }
        public AssetSingle DecelTime { get; set; }
        public AssetSingle HubRadius { get; set; }

        public PlatSpecific_Paddle(Game game) : base(game) { }
        public PlatSpecific_Paddle(EndianBinaryReader reader, Game game) : this(game)
        {
            StartOrient = reader.ReadInt32();
            OrientCount = reader.ReadInt32();
            OrientLoop = reader.ReadSingle();
            Orient1 = reader.ReadSingle();
            Orient2 = reader.ReadSingle();
            Orient3 = reader.ReadSingle();
            Orient4 = reader.ReadSingle();
            Orient5 = reader.ReadSingle();
            Orient6 = reader.ReadSingle();
            Settings.FlagValueInt = reader.ReadUInt32();
            RotateSpeed = reader.ReadSingle();
            AccelTime = reader.ReadSingle();
            DecelTime = reader.ReadSingle();
            HubRadius = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(StartOrient);
            writer.Write(OrientCount);
            writer.Write(OrientLoop);
            writer.Write(Orient1);
            writer.Write(Orient2);
            writer.Write(Orient3);
            writer.Write(Orient4);
            writer.Write(Orient5);
            writer.Write(Orient6);
            writer.Write(Settings.FlagValueInt);
            writer.Write(RotateSpeed);
            writer.Write(AccelTime);
            writer.Write(DecelTime);
            writer.Write(HubRadius);
        }
    }
}