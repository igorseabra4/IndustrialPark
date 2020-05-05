using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EmitterType : byte
    {
        Point = 0,
        CircleEdge = 1,
        Circle = 2,
        RectEdge = 3,
        Rect = 4,
        Line = 5,
        Volume = 6,
        SphereEdge = 7,
        Sphere = 8,
        OffsetPoint = 9,
        SphereEdge10 = 10,
        SphereEdge11 = 11,
        VCylEdge = 12,
        OCircleEdge = 13,
        OCircle = 14,
        EntityBone = 15,
        EntityBound = 16,
    }

    public class AssetPARE : BaseAsset
    {
        public AssetPARE(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x54;

        public override bool HasReference(uint assetID) => PARP_AssetID == assetID || Emitter_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (PARP_AssetID == 0)
                result.Add("PARE with PARP_AssetID set to 0");
            Verify(PARP_AssetID, ref result);
            Verify(Emitter_AssetID, ref result);
        }

        [Category("Particle Emitter")]
        public DynamicTypeDescriptor EmitterFlags => ByteFlagsDescriptor(0x8);

        [Category("Particle Emitter")]
        public EmitterType EmitterType
        {
            get => (EmitterType)ReadByte(0x9);
            set => Write(0x9, (byte)value);
        }

        [Category("Particle Emitter")]
        public short Padding0A
        {
            get => ReadShort(0xA);
            set => Write(0xA, value);
        }

        [Category("Particle Emitter")]
        public AssetID PARP_AssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(ExpandableObjectConverter))]
        public PareSpecific_Generic PareSpecific
        {
            get
            {
                switch (EmitterType)
                {
                    case EmitterType.CircleEdge:
                    case EmitterType.Circle:
                    case EmitterType.OCircleEdge:
                    case EmitterType.OCircle:
                        return new PareSpecific_xPECircle(this);
                    case EmitterType.RectEdge:
                    case EmitterType.Rect:
                        return new PareSpecific_tagEmitRect(this);
                    case EmitterType.Line:
                        return new PareSpecific_tagEmitLine(this);
                    case EmitterType.Volume:
                        return new PareSpecific_tagEmitVolume(this);
                    case EmitterType.SphereEdge:
                    case EmitterType.Sphere:
                    case EmitterType.SphereEdge10:
                    case EmitterType.SphereEdge11:
                        return new PareSpecific_tagEmitSphere(this);
                    case EmitterType.OffsetPoint:
                        return new PareSpecific_tagEmitOffsetPoint(this);
                    case EmitterType.VCylEdge:
                        return new PareSpecific_xPEVCyl(this);
                    case EmitterType.EntityBone:
                        return new PareSpecific_xPEEntBone(this);
                    case EmitterType.EntityBound:
                        return new PareSpecific_xPEEntBound(this);
                    default:
                        return new PareSpecific_Generic(this);
                }
            }
        }

        [Category("Particle Emitter")]
        public AssetID Emitter_AssetID
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Emitter_PosX
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Emitter_PosY
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Emitter_PosZ
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_X
        {
            get => ReadFloat(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_Y
        {
            get => ReadFloat(0x40);
            set => Write(0x40, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_Z
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float Velocity_AngleVariation
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }

        [Category("Particle Emitter")]
        public int CullMode
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category("Particle Emitter"), TypeConverter(typeof(FloatTypeConverter))]
        public float CullDistanceSqr
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
    }
}