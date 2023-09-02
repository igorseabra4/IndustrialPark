using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPLAT : AssetWithMotion
    {
        private const string categoryName = "Platform";
        public override string AssetInfo => $"{PlatformType} {HexUIntTypeConverter.StringFromAssetID(Model)}";

        [Category(categoryName), DisplayName("Flags")]
        public FlagBitmask PlatformFlags { get; set; } = ShortFlagsDescriptor(
            "Shake on Mount",
            null,
            "Solid");

        private PlatType _platformType;
        [Category(categoryName), DisplayName("Type")]
        public PlatType PlatformType
        {
            get => _platformType;
            set
            {
                _platformType = value;

                if ((int)value > 3)
                    TypeFlag = (byte)value;
                else
                    TypeFlag = 0;

                switch ((PlatType)(byte)TypeFlag)
                {
                    case PlatType.ConveyorBelt:
                        PlatformSpecific = new PlatSpecific_ConveryorBelt(game);
                        break;
                    case PlatType.FallingPlatform:
                        PlatformSpecific = new PlatSpecific_FallingPlatform(game);
                        break;
                    case PlatType.FR:
                        PlatformSpecific = new PlatSpecific_FR(game);
                        break;
                    case PlatType.BreakawayPlatform:
                        PlatformSpecific = new PlatSpecific_BreakawayPlatform(game);
                        break;
                    case PlatType.Springboard:
                        PlatformSpecific = new PlatSpecific_Springboard(game);
                        break;
                    case PlatType.TeeterTotter:
                        PlatformSpecific = new PlatSpecific_TeeterTotter(game);
                        break;
                    case PlatType.Paddle:
                        PlatformSpecific = new PlatSpecific_Paddle(game);
                        break;
                    default:
                        PlatformSpecific = new PlatSpecific_Generic(game);
                        break;
                }

                switch (PlatformType)
                {
                    case PlatType.ExtendRetract:
                        Motion = new Motion_ExtendRetract(game);
                        break;
                    case PlatType.Orbit:
                        Motion = new Motion_Orbit(game);
                        break;
                    case PlatType.Spline:
                        Motion = new Motion_Spline(game);
                        break;
                    case PlatType.Pendulum:
                        Motion = new Motion_Pendulum(game);
                        break;
                    case PlatType.MovePoint:
                        Motion = new Motion_MovePoint(game, _position);
                        break;
                    case PlatType.Mechanism:
                        Motion = new Motion_Mechanism(game);
                        break;
                    default:
                        Motion = new Motion(game, MotionType.Other);
                        break;
                }
            }
        }

        private int motionStart(Game game) =>
            game == Game.Scooby ? 0x78 :
            game == Game.BFBB ? 0x90 :
            game == Game.Incredibles ? 0x8C : 0;

        public AssetPLAT(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.Platform, BaseAssetType.Platform, position)
        {
            PlatformType = PlatType.Mechanism;
            PlatformFlags.FlagValueShort = 4;

            switch (template)
            {
                case AssetTemplate.Hovering_Platform:
                    Model = 0x335EE0C8;
                    Animation = 0x730847B6;
                    Motion = new Motion_Mechanism(game)
                    {
                        Type = MotionType.Other,
                        MovementLoopMode = EMechanismFlags.ReturnToStart,
                        SlideAccelTime = 0.4f,
                        SlideDecelTime = 0.4f
                    };
                    break;
                case AssetTemplate.Texas_Hitch_Platform:
                case AssetTemplate.Swinger:
                    Model = "trailer_hitch";
                    break;
                case AssetTemplate.Springboard:
                    Model = 0x55E9EAB5;
                    Animation = 0x7AAA99BB;
                    PlatformType = PlatType.Springboard;
                    PlatformSpecific = new PlatSpecific_Springboard(game)
                    {
                        Height1 = 10,
                        Height2 = 10,
                        Height3 = 10,
                        HeightBubbleBounce = 10,
                        Animation1 = 0x6DAE0759,
                        Animation2 = 0xBC4A9A5F,
                        DirectionY = 1f,
                    };
                    break;
                case AssetTemplate.CollapsePlatform_Planktopolis:
                case AssetTemplate.CollapsePlatform_ThugTug:
                case AssetTemplate.CollapsePlatform_Spongeball:
                    PlatformType = PlatType.BreakawayPlatform;
                    Animation = 0x7A9BF321;
                    if (template == AssetTemplate.CollapsePlatform_Planktopolis)
                    {
                        Model = 0x6F462432;
                    }
                    else if (template == AssetTemplate.CollapsePlatform_ThugTug)
                    {
                        Animation = 0x62C6520F;
                        Model = 0xED7F1021;
                    }
                    else if (template == AssetTemplate.CollapsePlatform_Spongeball)
                    {
                        Model = 0x1A38B9AB;
                    }
                    PlatformSpecific = new PlatSpecific_BreakawayPlatform(template, game);
                    Motion = new Motion_Mechanism(game, MotionType.Other);
                    break;
                case AssetTemplate.Flower_Dig:
                    VisibilityFlags.FlagValueByte = 0;
                    Model = "path_dig_C.MINF";
                    PlatformType = PlatType.ExtendRetract;
                    var m = (Motion_ExtendRetract)Motion;
                    m.MotionFlags.FlagValueShort = 0x4;
                    m.RetractPositionX = _position.X;
                    m.RetractPositionY = _position.Y;
                    m.RetractPositionZ = _position.Z;
                    break;
                case AssetTemplate.Floating_Block:
                case AssetTemplate.Floating_Block_Spiked:
                    _scale = new Vector3(1.9f, 1.9f, 1.9f);
                    Motion = new Motion_Mechanism(game)
                    {
                        Type = MotionType.Mechanism,
                        MovementLoopMode = EMechanismFlags.ReturnToStart,
                        ScaleAmount = 1f,
                        ScaleDuration = 1f
                    };
                    Model = "block_basic";
                    Surface = "WALLHANG_SURF";
                    SolidityFlags.FlagValueByte = 0x82;
                    break;
                case AssetTemplate.Scale_Block:
                case AssetTemplate.Scale_Block_Driven:
                case AssetTemplate.Scale_Block_Spiked_Driven:
                    _scale = new Vector3(1.9f, 1.9f, 1.9f);
                    Motion = new Motion_Mechanism(game)
                    {
                        Type = MotionType.Mechanism,
                        MovementType = EMovementType.Scale,
                        MovementLoopMode = EMechanismFlags.DontLoop,
                        ScaleAxis = 6,
                        ScaleAmount = 0.01f,
                        ScaleDuration = 1f
                    };
                    Motion.MotionFlags.FlagValueShort = 0x04;
                    Model = "block_scale";
                    Surface = "WALLHANG_SURF";
                    SolidityFlags.FlagValueByte = 0x82;
                    break;
                case AssetTemplate.Ice_Block:
                case AssetTemplate.Ice_Block_Spiked:
                    _scale = new Vector3(1.9f, 1.9f, 1.9f);
                    Motion = new Motion_Mechanism(game)
                    {
                        Type = MotionType.Mechanism,
                        MovementLoopMode = EMechanismFlags.ReturnToStart,
                        ScaleAmount = 1f,
                        ScaleDuration = 1f
                    };
                    Model = "block_ice";
                    Surface = "ICE_SURF";
                    SolidityFlags.FlagValueByte = 0x82;
                    break;
                case AssetTemplate.Trampoline_Block:
                case AssetTemplate.Trampoline_Block_Driven:
                case AssetTemplate.Trampoline_Block_Spiked_Driven:
                    _scale = new Vector3(1.9f, 1.9f, 1.9f);
                    PlatformType = PlatType.Springboard;
                    PlatformSpecific = new PlatSpecific_Springboard(game)
                    {
                        Height1 = 18,
                        Height2 = 18,
                        Height3 = 18,
                        HeightBubbleBounce = 18,
                        Animation1 = 0x6B68D5DB,
                        Animation2 = 0x6B68D5DB,
                        DirectionY = 1f
                    };
                    ((PlatSpecific_Springboard)PlatformSpecific).Settings.FlagValueInt = 1;
                    Model = "block_spring";
                    break;
                case AssetTemplate.Block_Driver:
                    _scale = new Vector3(1f, 1f, 1f);
                    Motion = new Motion_Mechanism(game)
                    {
                        Type = MotionType.Mechanism,
                        MovementLoopMode = EMechanismFlags.ReturnToStart
                    };
                    Model = "block_basic";
                    VisibilityFlags.FlagValueByte = 0;
                    SolidityFlags.FlagValueByte = 0;
                    PlatformFlags.FlagValueByte = 0;
                    break;
                case AssetTemplate.Wooden_Platform:
                    _scale = new Vector3(0.6f, 0.6f, 0.6f);
                    Model = "fb_platform";
                    Surface = "WALLHANG_SURF";
                    SolidityFlags.FlagValueByte = 0x82;
                    break;
            }
        }

        public AssetPLAT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                _platformType = (PlatType)reader.ReadByte();

                reader.ReadByte();

                PlatformFlags.FlagValueShort = reader.ReadUInt16();

                switch ((PlatType)(byte)TypeFlag)
                {
                    case PlatType.ConveyorBelt:
                        PlatformSpecific = new PlatSpecific_ConveryorBelt(reader, game);
                        break;
                    case PlatType.FallingPlatform:
                        PlatformSpecific = new PlatSpecific_FallingPlatform(reader, game);
                        break;
                    case PlatType.FR:
                        PlatformSpecific = new PlatSpecific_FR(reader, game);
                        break;
                    case PlatType.BreakawayPlatform:
                        PlatformSpecific = new PlatSpecific_BreakawayPlatform(reader, game);
                        break;
                    case PlatType.Springboard:
                        PlatformSpecific = new PlatSpecific_Springboard(reader, game);
                        break;
                    case PlatType.TeeterTotter:
                        PlatformSpecific = new PlatSpecific_TeeterTotter(reader, game);
                        break;
                    case PlatType.Paddle:
                        PlatformSpecific = new PlatSpecific_Paddle(reader, game);
                        break;
                    default:
                        PlatformSpecific = new PlatSpecific_Generic(game);
                        break;
                }

                reader.BaseStream.Position = motionStart(game);

                switch (PlatformType)
                {
                    case PlatType.ExtendRetract:
                        Motion = new Motion_ExtendRetract(reader, game);
                        break;
                    case PlatType.Orbit:
                        Motion = new Motion_Orbit(reader, game);
                        break;
                    case PlatType.Spline:
                        Motion = new Motion_Spline(reader, game);
                        break;
                    case PlatType.Pendulum:
                        Motion = new Motion_Pendulum(reader, game);
                        break;
                    case PlatType.MovePoint:
                        Motion = new Motion_MovePoint(reader, game, _position);
                        break;
                    case PlatType.Mechanism:
                        Motion = new Motion_Mechanism(reader, game);
                        break;
                    default:
                        Motion = new Motion(reader, game);
                        break;
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte)_platformType);
            writer.Write((byte)0);
            writer.Write(PlatformFlags.FlagValueShort);
            PlatformSpecific.Serialize(writer);

            var motionStart = this.motionStart(game);
            while (writer.BaseStream.Length < motionStart)
                writer.Write((byte)0);
            Motion.Serialize(writer);

            int linkStart =
                game == Game.BFBB ? 0xC0 :
                game == Game.Incredibles ? 0xC8 :
                game == Game.Scooby ? 0xA8 : throw new System.ArgumentException("Invalid game");

            while (writer.BaseStream.Length < linkStart)
                writer.Write((byte)0);
            SerializeLinks(writer);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        [Category(categoryNamePlacement)]
        public override AssetSingle PositionX
        {
            get => base.PositionX;
            set
            {
                if (Motion is Motion_MovePoint mp)
                    mp.SetInitialPosition(_position);
                base.PositionX = value;
            }
        }

        [Category(categoryNamePlacement)]
        public override AssetSingle PositionY
        {
            get => base.PositionY;
            set
            {
                if (Motion is Motion_MovePoint mp)
                    mp.SetInitialPosition(_position);
                base.PositionY = value;
            }
        }

        [Category(categoryNamePlacement)]
        public override AssetSingle PositionZ
        {
            get => base.PositionZ;
            set
            {
                if (Motion is Motion_MovePoint mp)
                    mp.SetInitialPosition(_position);
                base.PositionZ = value;
            }
        }

        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                if (isSkyBox)
                {
                    Vector3 skyTranslation = Program.MainForm.renderer.Camera.Position;
                    if (!skyBoxUseY)
                        skyTranslation.Y = PositionY;

                    return base.LocalWorld() * Matrix.Translation(-_position) * Matrix.Translation(skyTranslation);
                }

                if (PlatformType == PlatType.MovePoint)
                    return Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * PlatLocalTranslation();
                return base.LocalWorld();
            }

            return world;
        }

        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter)), DisplayName("Data")]
        public PlatSpecific_Generic PlatformSpecific { get; set; }

        private bool isSkyBox = false;
        private bool skyBoxUseY = false;

        public override void Reset()
        {
            isSkyBox = false;
            skyBoxUseY = false;
            foreach (Link link in _links)
                if ((EventBFBB)link.EventSendID == EventBFBB.SetasSkydome && link.TargetAsset.Equals(assetID))
                {
                    isSkyBox = true;
                    if (link.FloatParameter2 == 1f)
                        skyBoxUseY = true;
                }
            base.Reset();
        }
    }
}