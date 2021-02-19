using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class PlatSpecific_Generic : AssetSpecific_Generic
    {
        public PlatSpecific_Generic(AssetPLAT asset) : base(asset, 0x58 + asset.Offset) { }
    }

    public class PlatSpecific_ConveryorBelt : PlatSpecific_Generic
    {
        public PlatSpecific_ConveryorBelt(AssetPLAT plat) : base(plat) { }

        [Category("Conveyor Belt")]
        public float Speed 
        {
            get => ReadFloat(0);
            set => Write(0, value);
        }
    }

    public class PlatSpecific_FallingPlatform : PlatSpecific_Generic
    {
        public PlatSpecific_FallingPlatform(AssetPLAT plat) : base(plat) { }

        [Category("Falling Platform")]
        public float Speed
        {
            get => ReadFloat(0);
            set => Write(0, value);
        }
        [Category("Falling Platform")]
        public AssetID BustModel_AssetID
        {
            get => ReadUInt(4);
            set => Write(4, value);
        }
        
        public override bool HasReference(uint assetID)
        {
            return BustModel_AssetID == assetID;
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(BustModel_AssetID, ref result);
        }
    }

    public class PlatSpecific_FR : PlatSpecific_Generic
    {
        public PlatSpecific_FR(AssetPLAT plat) : base(plat) { }

        [Category("FR")]
        public float fspeed
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [Category("FR")]
        public float rspeed
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [Category("FR")]
        public float ret_delay
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [Category("FR")]
        public float post_ret_delay
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
    }

    public class PlatSpecific_BreakawayPlatform : PlatSpecific_Generic
    {
        public PlatSpecific_BreakawayPlatform(AssetPLAT plat) : base(plat)
        {
            Settings = plat.IntFlagsDescriptor(0xC + specificStart);
        }

        [Category("Breakaway Platform")]
        public float BreakawayDelay
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [Category("Breakaway Platform")]
        public AssetID BustModel_AssetID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
        [Category("Breakaway Platform")]
        public float ResetDelay
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [Category("Breakaway Platform")]
        public DynamicTypeDescriptor Settings { get; set; }
        [Category("Breakaway Platform"), Browsable(false)]
        public int Settings_Int
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }
    }

    public class PlatSpecific_BreakawayPlatform_TSSM : PlatSpecific_Generic
    {
        public PlatSpecific_BreakawayPlatform_TSSM(AssetPLAT plat) : base(plat)
        {
            Settings = plat.IntFlagsDescriptor(0x8 + specificStart);
        }

        [Category("Breakaway Platform")]
        public float BreakawayDelay
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [Category("Breakaway Platform")]
        public float ResetDelay
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [Category("Breakaway Platform")]
        public DynamicTypeDescriptor Settings { get; set; }
        [Category("Breakaway Platform"), Browsable(false)]
        public int Settings_Int
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }
        [Category("Breakaway Platform")]
        public float UnknownFloat0C
        {
            get => ReadFloat(0x0c);
            set => Write(0x0c, value);
        }
    }

    public class PlatSpecific_Springboard : PlatSpecific_Generic
    {
        public PlatSpecific_Springboard(AssetPLAT plat) : base(plat)
        {
            Settings = plat.IntFlagsDescriptor(0x28 + specificStart,
                "Lock Camera Down",
                null,
                "Lock Player Control");
        }

        [Category("Springboard")]
        public float Height1
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [Category("Springboard")]
        public float Height2
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [Category("Springboard")]
        public float Height3
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [Category("Springboard")]
        public float HeightBubbleBounce
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [Category("Springboard")]
        public AssetID Anim1_AssetID
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }
        [Category("Springboard")]
        public AssetID Anim2_AssetID
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
        [Category("Springboard")]
        public AssetID Anim3_AssetID
        {
            get => ReadUInt(0x18);
            set => Write(0x18, value);
        }
        [Category("Springboard")]
        public float DirectionX
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [Category("Springboard")]
        public float DirectionY
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [Category("Springboard")]
        public float DirectionZ
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [Category("Springboard")]
        public DynamicTypeDescriptor Settings { get; set; }

        public override bool HasReference(uint assetID) => Anim1_AssetID == assetID || Anim2_AssetID == assetID || Anim3_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Anim1_AssetID, ref result);
            Asset.Verify(Anim2_AssetID, ref result);
            Asset.Verify(Anim3_AssetID, ref result);
        }
    }

    public class PlatSpecific_TeeterTotter : PlatSpecific_Generic
    {
        public PlatSpecific_TeeterTotter(AssetPLAT plat) : base(plat) { }

        [Category("Teeter-Totter")]
        private float InitialTilt_Rad
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [Category("Teeter-Totter")]
        private float MaxTilt_Rad
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }

        [Category("Teeter-Totter")]
        public float InitialTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(InitialTilt_Rad);
            set => InitialTilt_Rad = MathUtil.DegreesToRadians(value);
        }
        [Category("Teeter-Totter")]
        public float MaxTilt_Deg
        {
            get => MathUtil.RadiansToDegrees(MaxTilt_Rad);
            set => MaxTilt_Rad = MathUtil.DegreesToRadians(value);
        }

        [Category("Teeter-Totter")]
        public float InverseMass
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
    }

    public class PlatSpecific_Paddle : PlatSpecific_Generic
    {
        public PlatSpecific_Paddle(AssetPLAT plat) : base(plat)
        {
            Settings = plat.IntFlagsDescriptor(0x24 + specificStart);
        }

        [Category("Paddle")]
        public int StartOrient
        {
            get => ReadInt(0);
            set => Write(0, value);
        }

        [Category("Paddle")]
        public int OrientCount
        {
            get => ReadInt(4);
            set => Write(4, value);
        }

        [Category("Paddle")]
        public float OrientLoop
        {
            get => ReadFloat(8);
            set => Write(8, value);
        }

        [Category("Paddle")]
        public float Orient1
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }
        [Category("Paddle")]
        public float Orient2
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [Category("Paddle")]
        public float Orient3
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [Category("Paddle")]
        public float Orient4
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [Category("Paddle")]
        public float Orient5
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [Category("Paddle")]
        public float Orient6
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [Category("Paddle")]
        public DynamicTypeDescriptor Settings { get; set; }
        [Category("Paddle")]
        public float RotateSpeed
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        [Category("Paddle")]
        public float AccelTime
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [Category("Paddle")]
        public float DecelTime
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [Category("Paddle")]
        public float HubRadius
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
    }
}