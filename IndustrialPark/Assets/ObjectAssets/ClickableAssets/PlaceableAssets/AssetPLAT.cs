using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPLAT : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0xC0 + Offset;

        public AssetPLAT(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (ANIM_AssetID_1 == assetID)
                return true;
            if (ANIM_AssetID_2 == assetID)
                return true;
            if (MVPT_AssetID_98 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Browsable(false)]
        public override AssetID ReferenceAssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("Platform")]
        public AssetID AnimationAssetID
        {
            get { return ReadUInt(0x50 + Offset); }
            set { Write(0x50 + Offset, value); }
        }

        [Category("Platform")]
        public PlatType PlatformType_Header
        {
            get => (PlatType)ReadByte(0x09 + Offset);
            set => Write(0x09 + Offset, (byte)value);
        }

        [Category("Platform")]
        public PlatType PlatformType
        {
            get => (PlatType)ReadByte(0x54 + Offset);
            set => Write(0x54 + Offset, (byte)value);
        }

        [Category("Platform")]
        public byte UnknownByte_55
        {
            get => ReadByte(0x55 + Offset);
            set => Write(0x55 + Offset, value);
        }

        [Category("Platform")]
        public short CollisionType
        {
            get => ReadShort(0x56 + Offset);
            set => Write(0x56 + Offset, value);
        }

        [Category("Platform")]
        public float MaybeLaunchStrength
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Platform")]
        public float TeeterRotationAngle
        {
            get => MathUtil.RadiansToDegrees(ReadFloat(0x5C + Offset));
            set => Write(0x5C + Offset, MathUtil.DegreesToRadians(value));
        }

        [Category("Platform")]
        public float TeeterRotationSpeed
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Platform")]
        public float AdditionalSlamHeight
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Platform")]
        public AssetID ANIM_AssetID_1
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Platform")]
        public AssetID ANIM_AssetID_2
        {
            get => ReadUInt(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Platform")]
        public float UnknownFloat_70
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("Platform")]
        public float LaunchDirectionX
        {
            get => ReadFloat(0x74 + Offset);
            set => Write(0x74 + Offset, value);
        }

        [Category("Platform")]
        public float LaunchDirectionY
        {
            get => ReadFloat(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Platform")]
        public float LaunchDirectionZ
        {
            get => ReadFloat(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Platform")]
        public int MovementLockMode
        {
            get => ReadInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Platform")]
        public float UnknownFloat_84
        {
            get => ReadFloat(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Platform")]
        public float UnknownFloat_88
        {
            get => ReadFloat(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        [Category("Platform")]
        public float UnknownFloat_8C
        {
            get => ReadFloat(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_90
        {
            get => ReadByte(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_91
        {
            get => ReadByte(0x91 + Offset);
            set => Write(0x91 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_92
        {
            get => ReadByte(0x92 + Offset);
            set => Write(0x92 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_93
        {
            get => ReadByte(0x93 + Offset);
            set => Write(0x93 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_94
        {
            get => ReadByte(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_95
        {
            get => ReadByte(0x95 + Offset);
            set => Write(0x95 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_96
        {
            get => ReadByte(0x96 + Offset);
            set => Write(0x96 + Offset, value);
        }

        [Category("Platform")]
        public byte UnknownByte_97
        {
            get => ReadByte(0x97 + Offset);
            set => Write(0x97 + Offset, value);
        }

        [Category("Platform")]
        public AssetID MVPT_AssetID_98
        {
            get { return ReadUInt(0x98 + Offset); }
            set { Write(0x98 + Offset, value); }
        }

        [Category("Platform")]
        public float MovementTranslation_Distance_98
        {
            get => ReadFloat(0x98 + Offset);
            set => Write(0x98 + Offset, value);
        }

        [Category("Platform")]
        public float MovementTranslation_Time
        {
            get => ReadFloat(0x9C + Offset);
            set => Write(0x9C + Offset, value);
        }

        [Category("Platform")]
        public float MovementTranslation_UnknownA0
        {
            get => ReadFloat(0xA0 + Offset);
            set => Write(0xA0 + Offset, value);
        }

        [Category("Platform")]
        public float MovementTranslation_UnknownA4
        {
            get => ReadFloat(0xA4 + Offset);
            set => Write(0xA4 + Offset, value);
        }

        [Category("Platform")]
        public float MovementRotation_Degrees
        {
            get => ReadFloat(0xA8 + Offset);
            set => Write(0xA8 + Offset, value);
        }

        [Category("Platform")]
        public float MovementRotation_Time
        {
            get => ReadFloat(0xAC + Offset);
            set => Write(0xAC + Offset, value);
        }

        [Category("Platform")]
        public float MovementRotation_UnknownB0
        {
            get => ReadFloat(0xB0 + Offset);
            set => Write(0xB0 + Offset, value);
        }

        [Category("Platform")]
        public float MovementRotation_UnknownB4
        {
            get => ReadFloat(0xB4 + Offset);
            set => Write(0xB4 + Offset, value);
        }

        [Category("Platform")]
        public float MovementEndPointTime
        {
            get => ReadFloat(0xB8 + Offset);
            set => Write(0xB8 + Offset, value);
        }

        [Category("Platform")]
        public float MovementStartPointTime
        {
            get => ReadFloat(0xBC + Offset);
            set => Write(0xBC + Offset, value);
        }
    }
}