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
                switch (Functions.currentGame)
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

        public AssetPLAT(Section_AHDR AHDR) : base(AHDR)
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
            get => _position.X;
            set
            {
                _position.X = value;
                Write(0x20 + Offset, _position.X);
                CreateTransformMatrix();
                ChoosePlatSpecific();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                Write(0x24 + Offset, _position.Y);
                CreateTransformMatrix();
                ChoosePlatSpecific();
            }
        }

        [Category("Placement")]
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                Write(0x28 + Offset, _position.Z);
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
        [Description("0 = None\n1 = Shake on Mount\n2 = Unknown\n4=Solid\nAdd numbers to enable multiple")]
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
                    _platSpecific = new PlatSpecific_ConveryorBelt(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
                case PlatType.FallingPlatform:
                    _platSpecific = new PlatSpecific_FallingPlatform(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
                case PlatType.FR:
                    _platSpecific = new PlatSpecific_FR(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
                case PlatType.BreakawayPlatform:
                    _platSpecific = new PlatSpecific_BreakawayPlatform(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
                case PlatType.Springboard:
                    _platSpecific = new PlatSpecific_Springboard(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
                case PlatType.TeeterTotter:
                    _platSpecific = new PlatSpecific_TeeterTotter(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
                case PlatType.Paddle:
                    _platSpecific = new PlatSpecific_Paddle(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
                default:
                    _platSpecific = new PlatSpecific_Generic(Data.Skip(PlatSpecificStart + Offset).ToArray());
                    break;
            }

            switch (PlatformSubtype)
            {
                case PlatTypeSpecific.ExtendRetract:
                    _motion = new Motion_ExtendRetract(Data.Skip(MotionStart).ToArray());
                    break;
                case PlatTypeSpecific.Orbit:
                    _motion = new Motion_Orbit(Data.Skip(MotionStart).ToArray());
                    break;
                case PlatTypeSpecific.Spline:
                    _motion = new Motion_Spline(Data.Skip(MotionStart).ToArray());
                    break;
                case PlatTypeSpecific.Pendulum:
                    _motion = new Motion_Pendulum(Data.Skip(MotionStart).ToArray());
                    break;
                case PlatTypeSpecific.MovePoint:
                    _motion = new Motion_MovePoint(Data.Skip(MotionStart).ToArray(), _position);
                    break;
                default:
                    _motion = new Motion_Mechanism(Data.Skip(MotionStart).ToArray());
                    break;
            }

            PlatSpecificChosen?.Invoke();
        }

        public Action PlatSpecificChosen;
        
        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
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
                before.AddRange(Data.Skip(PlatSpecificStart + Offset + PlatSpecific_Generic.Size));
                Data = before.ToArray();
            }
        }

        protected override int MotionStart => Functions.currentGame == Game.Scooby ? 0x78 : 0x90 + Offset;
    }
}