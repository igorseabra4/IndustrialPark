using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPLAT : AssetWithMotion
    {
        private const string categoryName = "Platform";

        private PlatType _platformType;
        [Category(categoryName)]
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
                        PlatSpecific = new PlatSpecific_ConveryorBelt();
                        break;
                    case PlatType.FallingPlatform:
                        PlatSpecific = new PlatSpecific_FallingPlatform();
                        break;
                    case PlatType.FR:
                        PlatSpecific = new PlatSpecific_FR();
                        break;
                    case PlatType.BreakawayPlatform:
                        PlatSpecific = new PlatSpecific_BreakawayPlatform();
                        break;
                    case PlatType.Springboard:
                        PlatSpecific = new PlatSpecific_Springboard();
                        break;
                    case PlatType.TeeterTotter:
                        PlatSpecific = new PlatSpecific_TeeterTotter();
                        break;
                    case PlatType.Paddle:
                        PlatSpecific = new PlatSpecific_Paddle();
                        break;
                    default:
                        PlatSpecific = new PlatSpecific_Generic();
                        break;
                }

                switch (PlatformType)
                {
                    case PlatType.ExtendRetract:
                        Motion = new Motion_ExtendRetract();
                        break;
                    case PlatType.Orbit:
                        Motion = new Motion_Orbit();
                        break;
                    case PlatType.Spline:
                        Motion = new Motion_Spline();
                        break;
                    case PlatType.Pendulum:
                        Motion = new Motion_Pendulum();
                        break;
                    case PlatType.MovePoint:
                        Motion = new Motion_MovePoint(_position);
                        break;
                    case PlatType.Mechanism:
                        Motion = new Motion_Mechanism();
                        break;
                    default:
                        Motion = new Motion(MotionType.Other);
                        break;
                }
            }
        }

        [Category(categoryName)]
        public FlagBitmask PlatFlags { get; set; } = ShortFlagsDescriptor(
            "Shake on Mount",
            "Unknown",
            "Solid");

        private int motionStart(Game game) =>
            game == Game.Scooby ? 0x78 :
            game == Game.BFBB ? 0x90 :
            game == Game.Incredibles ? 0x8C : 0;

        public AssetPLAT(Game game, string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.Platform, BaseAssetType.Platform, position)
        {
            this.game = game;

            PlatformType = PlatType.Mechanism;
            PlatFlags.FlagValueShort = 4;

            switch (template)
            {
                case AssetTemplate.HoveringPlatform:
                    Model_AssetID = 0x335EE0C8;
                    Animation_AssetID = 0x730847B6;
                    Motion = new Motion_Mechanism()
                    {
                        Type = MotionType.Other,
                        MovementLoopMode = EMechanismFlags.ReturnToStart,
                        SlideAccelTime = 0.4f,
                        SlideDecelTime = 0.4f
                    };
                    break;
                case AssetTemplate.TexasHitch_PLAT:
                case AssetTemplate.Swinger:
                    Model_AssetID = "trailer_hitch";
                    break;
                case AssetTemplate.Springboard:
                    Model_AssetID = 0x55E9EAB5;
                    Animation_AssetID = 0x7AAA99BB;
                    PlatformType = PlatType.Springboard;
                    PlatSpecific = new PlatSpecific_Springboard()
                    {
                        Height1 = 10,
                        Height2 = 10,
                        Height3 = 10,
                        HeightBubbleBounce = 10,
                        Anim1_AssetID = 0x6DAE0759,
                        Anim2_AssetID = 0xBC4A9A5F,
                        DirectionY = 1f,
                    };
                    break;
                case AssetTemplate.CollapsePlatform_Planktopolis:
                case AssetTemplate.CollapsePlatform_ThugTug:
                case AssetTemplate.CollapsePlatform_Spongeball:
                    PlatformType = PlatType.BreakawayPlatform;
                    Animation_AssetID = 0x7A9BF321;
                    if (template == AssetTemplate.CollapsePlatform_Planktopolis)
                    {
                        Model_AssetID = 0x6F462432;
                    }
                    else if (template == AssetTemplate.CollapsePlatform_ThugTug)
                    {
                        Animation_AssetID = 0x62C6520F;
                        Model_AssetID = 0xED7F1021;
                    }
                    else if (template == AssetTemplate.CollapsePlatform_Spongeball)
                    {
                        Model_AssetID = 0x1A38B9AB;
                    }
                    PlatSpecific = new PlatSpecific_BreakawayPlatform(template);
                    Motion = new Motion_Mechanism(MotionType.Other);
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

                PlatFlags.FlagValueShort = reader.ReadUInt16();

                switch ((PlatType)(byte)TypeFlag)
                {
                    case PlatType.ConveyorBelt:
                        PlatSpecific = new PlatSpecific_ConveryorBelt(reader);
                        break;
                    case PlatType.FallingPlatform:
                        PlatSpecific = new PlatSpecific_FallingPlatform(reader);
                        break;
                    case PlatType.FR:
                        PlatSpecific = new PlatSpecific_FR(reader);
                        break;
                    case PlatType.BreakawayPlatform:
                        PlatSpecific = new PlatSpecific_BreakawayPlatform(reader, game);
                        break;
                    case PlatType.Springboard:
                        PlatSpecific = new PlatSpecific_Springboard(reader, game);
                        break;
                    case PlatType.TeeterTotter:
                        PlatSpecific = new PlatSpecific_TeeterTotter(reader);
                        break;
                    case PlatType.Paddle:
                        PlatSpecific = new PlatSpecific_Paddle(reader);
                        break;
                    default:
                        PlatSpecific = new PlatSpecific_Generic();
                        break;
                }

                reader.BaseStream.Position = motionStart(game);

                switch (PlatformType)
                {
                    case PlatType.ExtendRetract:
                        Motion = new Motion_ExtendRetract(reader);
                        break;
                    case PlatType.Orbit:
                        Motion = new Motion_Orbit(reader);
                        break;
                    case PlatType.Spline:
                        Motion = new Motion_Spline(reader);
                        break;
                    case PlatType.Pendulum:
                        Motion = new Motion_Pendulum(reader);
                        break;
                    case PlatType.MovePoint:
                        Motion = new Motion_MovePoint(reader, _position);
                        break;
                    case PlatType.Mechanism:
                        Motion = new Motion_Mechanism(reader, game);
                        break;
                    default:
                        Motion = new Motion(reader);
                        break;
                }
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));

                writer.Write((byte)_platformType);
                writer.Write((byte)0);
                writer.Write(PlatFlags.FlagValueShort);
                writer.Write(PlatSpecific.Serialize(game, endianness));

                var motionStart = this.motionStart(game);
                while (writer.BaseStream.Length < motionStart)
                    writer.Write((byte)0);
                writer.Write(Motion.Serialize(game, endianness));

                int linkStart =
                    game == Game.BFBB ? 0xC0 :
                    game == Game.Incredibles ? 0xC8 :
                    game == Game.Scooby ? 0xA8 : throw new System.ArgumentException("Invalid game");

                while (writer.BaseStream.Length < linkStart)
                    writer.Write((byte)0);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => PlatSpecific.HasReference(assetID) || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            PlatSpecific.Verify(ref result);
            base.Verify(ref result);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        [Category("Entity")]
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

        [Category("Entity")]
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

        [Category("Entity")]
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

        [Category("Platform")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PlatSpecific_Generic PlatSpecific { get; set; }

        private bool isSkyBox = false;
        private bool skyBoxUseY = false;

        public override void Reset()
        {
            isSkyBox = false;
            skyBoxUseY = false;
            foreach (Link link in _links)
                if ((EventBFBB)link.EventSendID == EventBFBB.SetasSkydome && link.TargetAssetID.Equals(assetID))
                {
                    isSkyBox = true;
                    if (link.FloatParameter2 == 1f)
                        skyBoxUseY = true;
                }
            base.Reset();
        }
    }
}