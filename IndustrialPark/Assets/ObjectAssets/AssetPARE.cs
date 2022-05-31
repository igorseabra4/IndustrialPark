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
        private const string categoryName = "Particle Emitter";

        [Category(categoryName)]
        public FlagBitmask EmitterFlags { get; set; } = ByteFlagsDescriptor();
        private EmitterType _emitterType;
        [Category(categoryName)]
        public EmitterType EmitterType
        {
            get => _emitterType;
            set
            {
                _emitterType = value;
                switch (_emitterType)
                {
                    case EmitterType.CircleEdge:
                    case EmitterType.Circle:
                    case EmitterType.OCircleEdge:
                    case EmitterType.OCircle:
                        ParticleEmitterSettings = new PareSpecific_xPECircle();
                        break;
                    case EmitterType.RectEdge:
                    case EmitterType.Rect:
                        ParticleEmitterSettings = new PareSpecific_tagEmitRect();
                        break;
                    case EmitterType.Line:
                        ParticleEmitterSettings = new PareSpecific_tagEmitLine();
                        break;
                    case EmitterType.Volume:
                        ParticleEmitterSettings = new PareSpecific_tagEmitVolume();
                        break;
                    case EmitterType.SphereEdge:
                    case EmitterType.Sphere:
                    case EmitterType.SphereEdge10:
                    case EmitterType.SphereEdge11:
                        ParticleEmitterSettings = new PareSpecific_tagEmitSphere();
                        break;
                    case EmitterType.OffsetPoint:
                        ParticleEmitterSettings = new PareSpecific_tagEmitOffsetPoint();
                        break;
                    case EmitterType.VCylEdge:
                        ParticleEmitterSettings = new PareSpecific_xPEVCyl();
                        break;
                    case EmitterType.EntityBone:
                        ParticleEmitterSettings = new PareSpecific_xPEEntBone();
                        break;
                    case EmitterType.EntityBound:
                        ParticleEmitterSettings = new PareSpecific_xPEEntBound();
                        break;
                    default:
                        ParticleEmitterSettings = new PareSpecific_Generic();
                        break;
                }
            }
        }
        [Category(categoryName)]
        public AssetID ParticleProperties { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public PareSpecific_Generic ParticleEmitterSettings { get; set; }
        [Category(categoryName)]
        public AssetID Emitter { get; set; }
        [Category(categoryName)]
        public AssetSingle Emitter_PosX { get; set; }
        [Category(categoryName)]
        public AssetSingle Emitter_PosY { get; set; }
        [Category(categoryName)]
        public AssetSingle Emitter_PosZ { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_X { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_Y { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_Z { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_AngleVariation { get; set; }
        [Category(categoryName)]
        public int CullMode { get; set; }
        [Category(categoryName)]
        public AssetSingle CullDistanceSqr { get; set; }

        public AssetPARE(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                EmitterFlags.FlagValueByte = reader.ReadByte();
                _emitterType = (EmitterType)reader.ReadByte();
                reader.BaseStream.Position += 2;
                ParticleProperties = reader.ReadUInt32();

                // should be at 0x10 now
                switch (_emitterType)
                {
                    case EmitterType.CircleEdge:
                    case EmitterType.Circle:
                    case EmitterType.OCircleEdge:
                    case EmitterType.OCircle:
                        ParticleEmitterSettings = new PareSpecific_xPECircle(reader);
                        break;
                    case EmitterType.RectEdge:
                    case EmitterType.Rect:
                        ParticleEmitterSettings = new PareSpecific_tagEmitRect(reader);
                        break;
                    case EmitterType.Line:
                        ParticleEmitterSettings = new PareSpecific_tagEmitLine(reader);
                        break;
                    case EmitterType.Volume:
                        ParticleEmitterSettings = new PareSpecific_tagEmitVolume(reader);
                        break;
                    case EmitterType.SphereEdge:
                    case EmitterType.Sphere:
                    case EmitterType.SphereEdge10:
                    case EmitterType.SphereEdge11:
                        ParticleEmitterSettings = new PareSpecific_tagEmitSphere(reader);
                        break;
                    case EmitterType.OffsetPoint:
                        ParticleEmitterSettings = new PareSpecific_tagEmitOffsetPoint(reader);
                        break;
                    case EmitterType.VCylEdge:
                        ParticleEmitterSettings = new PareSpecific_xPEVCyl(reader);
                        break;
                    case EmitterType.EntityBone:
                        ParticleEmitterSettings = new PareSpecific_xPEEntBone(reader);
                        break;
                    case EmitterType.EntityBound:
                        ParticleEmitterSettings = new PareSpecific_xPEEntBound(reader);
                        break;
                    default:
                        ParticleEmitterSettings = new PareSpecific_Generic();
                        break;
                } // 0x1C in size

                reader.BaseStream.Position = 0x2C;
                Emitter = reader.ReadUInt32();
                Emitter_PosX = reader.ReadSingle();
                Emitter_PosY = reader.ReadSingle();
                Emitter_PosZ = reader.ReadSingle();
                Velocity_X = reader.ReadSingle();
                Velocity_Y = reader.ReadSingle();
                Velocity_Z = reader.ReadSingle();
                Velocity_AngleVariation = reader.ReadSingle();
                CullMode = reader.ReadInt32();
                CullDistanceSqr = reader.ReadSingle();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(EmitterFlags.FlagValueByte);
                writer.Write((byte)_emitterType);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write(ParticleProperties);
                writer.Write(ParticleEmitterSettings.Serialize(game, endianness));
                while (writer.BaseStream.Length < 0x2C)
                    writer.Write((byte)0);
                writer.Write(Emitter);
                writer.Write(Emitter_PosX);
                writer.Write(Emitter_PosY);
                writer.Write(Emitter_PosZ);
                writer.Write(Velocity_X);
                writer.Write(Velocity_Y);
                writer.Write(Velocity_Z);
                writer.Write(Velocity_AngleVariation);
                writer.Write(CullMode);
                writer.Write(CullDistanceSqr);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            ParticleEmitterSettings.Verify(ref result);

            if (ParticleProperties == 0)
                result.Add("Particle Emitter with ParticleProperties set to 0");
            Verify(ParticleProperties, ref result);
            Verify(Emitter, ref result);
        }
    }
}