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

        public override bool DontRender => dontRender;

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

        public override bool HasReference(uint assetID) => PlatSpecific.HasReference(assetID) || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            PlatSpecific.Verify(ref result);
            base.Verify(ref result);
        }
        
        [Category("Entity")]
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

        [Category("Entity")]
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

        [Category("Entity")]
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
            get => (PlatType)ReadByte(0x54 + Offset);
            set
            {
                if ((int)value > 3)
                    TypeFlag = (byte)value;
                else
                    TypeFlag = 0;

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
        public DynamicTypeDescriptor PlatFlags => ShortFlagsDescriptor(0x56 + Offset, "Shake on Mount", "Unknown", "Solid");
        [Browsable(false)]
        public short PlatFlagsShort
        {
            get => ReadShort(0x56 + Offset);
            set => Write(0x56 + Offset, value);
        }
        
        private void ChoosePlatSpecific()
        {
            switch ((PlatType)TypeFlag)
            {
                case PlatType.ConveyorBelt:
                    PlatSpecific = new PlatSpecific_ConveryorBelt(this);
                    break;
                case PlatType.FallingPlatform:
                    PlatSpecific = new PlatSpecific_FallingPlatform(this);
                    break;
                case PlatType.FR:
                    PlatSpecific = new PlatSpecific_FR(this);
                    break;
                case PlatType.BreakawayPlatform:
                    PlatSpecific = new PlatSpecific_BreakawayPlatform(this);
                    break;
                case PlatType.Springboard:
                    PlatSpecific = new PlatSpecific_Springboard(this);
                    break;
                case PlatType.TeeterTotter:
                    PlatSpecific = new PlatSpecific_TeeterTotter(this);
                    break;
                case PlatType.Paddle:
                    PlatSpecific = new PlatSpecific_Paddle(this);
                    break;
                default:
                    PlatSpecific = new PlatSpecific_Generic(this);
                    break;
            }

            switch (PlatformType)
            {
                case PlatType.ExtendRetract:
                    Motion = new Motion_ExtendRetract(this);
                    break;
                case PlatType.Orbit:
                    Motion = new Motion_Orbit(this);
                    break;
                case PlatType.Spline:
                    Motion = new Motion_Spline(this);
                    break;
                case PlatType.Pendulum:
                    Motion = new Motion_Pendulum(this);
                    break;
                case PlatType.MovePoint:
                    Motion = new Motion_MovePoint(this, Position);
                    break;
                default:
                    Motion = game == Game.Incredibles ? new Motion_Mechanism_TSSM(this) : new Motion_Mechanism(this);
                    break;
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

                    return base.LocalWorld() * Matrix.Translation(-Position) * Matrix.Translation(skyTranslation);
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
        
        public override int MotionStart => game == Game.Scooby ? 0x78 : 0x90 + Offset;

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