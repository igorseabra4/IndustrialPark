using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetPLAT : AssetWithMotion
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset
        {
            get
            {
                switch (game)
                {
                    case Game.BFBB:
                        return 0xC0;
                    case Game.Incredibles:
                        return 0xC8;
                    case Game.Scooby:
                        return 0xA8;
                }

                return 0;
            }
        }

        public AssetPLAT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            ChoosePlatSpecific();
        }

        public override bool HasReference(uint assetID) => _platSpecific.HasReference(assetID) || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            _platSpecific.Verify(ref result);
            base.Verify(ref result);
        }
        
        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => Position.X;
            set
            {
                _position.X = value;
                Write(0x20 + Offset, Position.X);
                CreateTransformMatrix();
                ChoosePlatSpecific();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => Position.Y;
            set
            {
                _position.Y = value;
                Write(0x24 + Offset, Position.Y);
                CreateTransformMatrix();
                ChoosePlatSpecific();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => Position.Z;
            set
            {
                _position.Z = value;
                Write(0x28 + Offset, Position.Z);
                CreateTransformMatrix();
                ChoosePlatSpecific();
            }
        }
        
        [Category("Platform")]
        public PlatType PlatformType
        {
            get => (PlatType)ReadByte(0x09);
            set
            {
                Write(0x09, (byte)value);
                ChoosePlatSpecific();
            }
        }

        [Category("Platform")]
        public PlatTypeSpecific PlatformSubtype
        {
            get => (PlatTypeSpecific)ReadByte(0x54 + Offset);
            set
            {
                Write(0x54 + Offset, (byte)value);
                ChoosePlatSpecific();
            }
        }

        [Category("Platform")]
        public byte Padding55
        {
            get => ReadByte(0x55 + Offset);
            set => Write(0x55 + Offset, value);
        }

        [Category("Platform")]
        [Description("0 = None\n1 = Shake on Mount\n2 = Unknown\n4 = Solid\nAdd numbers to enable multiple flags")]
        public short PlatFlags
        {
            get => ReadShort(0x56 + Offset);
            set => Write(0x56 + Offset, value);
        }

        private const int PlatSpecificStart = 0x58;

        private void ChoosePlatSpecific()
        {
            switch (PlatformType)
            {
                case PlatType.ConveyorBelt:
                    _platSpecific = new PlatSpecific_ConveryorBelt(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
                case PlatType.FallingPlatform:
                    _platSpecific = new PlatSpecific_FallingPlatform(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
                case PlatType.FR:
                    _platSpecific = new PlatSpecific_FR(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
                case PlatType.BreakawayPlatform:
                    _platSpecific = new PlatSpecific_BreakawayPlatform(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
                case PlatType.Springboard:
                    _platSpecific = new PlatSpecific_Springboard(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
                case PlatType.TeeterTotter:
                    _platSpecific = new PlatSpecific_TeeterTotter(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
                case PlatType.Paddle:
                    _platSpecific = new PlatSpecific_Paddle(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
                default:
                    _platSpecific = new PlatSpecific_Generic(Data.Skip(PlatSpecificStart + Offset).ToArray(), game, platform);
                    break;
            }

            switch (PlatformSubtype)
            {
                case PlatTypeSpecific.ExtendRetract:
                    _motion = new Motion_ExtendRetract(Data.Skip(MotionStart).ToArray(), game, platform);
                    break;
                case PlatTypeSpecific.Orbit:
                    _motion = new Motion_Orbit(Data.Skip(MotionStart).ToArray(), game, platform);
                    break;
                case PlatTypeSpecific.Spline:
                    _motion = new Motion_Spline(Data.Skip(MotionStart).ToArray(), game, platform);
                    break;
                case PlatTypeSpecific.Pendulum:
                    _motion = new Motion_Pendulum(Data.Skip(MotionStart).ToArray(), game, platform);
                    break;
                case PlatTypeSpecific.MovePoint:
                    _motion = new Motion_MovePoint(Data.Skip(MotionStart).ToArray(), game, platform, Position);
                    break;
                default:
                    _motion = new Motion_Mechanism(Data.Skip(MotionStart).ToArray(), game, platform);
                    break;
            }

            PlatSpecificChosen?.Invoke();
        }

        public Action PlatSpecificChosen;
        
        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                if (isSkyBox)
                {
                    Vector3 skyTranslation = Program.MainForm.renderer.Camera.Position;
                    if (!skyBoxUseY)
                        skyTranslation.Y = PositionY;

                    return base.LocalWorld() * Matrix.Translation(-Position) * Matrix.Translation(skyTranslation);
                }
                
                if (PlatformSubtype == PlatTypeSpecific.MovePoint)
                    return Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * PlatLocalTranslation();
                return base.LocalWorld();
            }

            return world;
        }

        private PlatSpecific_Generic _platSpecific;

        [Category("Platform"), ReadOnly(true)]
        public PlatSpecific_Generic PlatSpecific
        {
            get => _platSpecific;
            set
            {
                _platSpecific = value;

                List<byte> before = Data.Take(PlatSpecificStart + Offset).ToList();
                before.AddRange(value.ToByteArray());
                before.AddRange(Data.Skip(PlatSpecificStart + Offset + PlatSpecific_Generic.Size(game)));
                Data = before.ToArray();
            }
        }

        protected override int MotionStart => game == Game.Scooby ? 0x78 : 0x90 + Offset;

        private bool isSkyBox = false;
        private bool skyBoxUseY = false;

        public override void Reset()
        {
            isSkyBox = false;
            skyBoxUseY = false;
            foreach (LinkBFBB link in LinksBFBB)
                if (link.EventSendID == EventBFBB.SetasSkydome && link.TargetAssetID.Equals(AssetID))
                {
                    isSkyBox = true;
                    if (link.Arguments_Float[1] == 1f)
                        skyBoxUseY = true;
                }
            base.Reset();
        }
    }
}