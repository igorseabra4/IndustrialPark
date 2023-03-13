using HipHopFile;
using IndustrialPark.AssetEditorColors;
using SharpDX;
using System;
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
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(ParticleProperties == 0 ? ParticleSystem : ParticleProperties);

        [Category(categoryName)]
        public FlagBitmask EmitterFlags { get; set; } = ByteFlagsDescriptor("On");
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
                        throw new ArgumentException($"Unknown particle emitter type: {_emitterType}");
                }
            }
        }
        [Category(categoryName)]
        public AssetByte Count { get; set; }
        [Category(categoryName)]
        public AssetByte CountVariation { get; set; }
        [Category(categoryName)]
        public AssetSingle Interval { get; set; }
        [Category(categoryName), IgnoreVerification]
        public AssetID ParticleProperties { get; set; }
        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public PareSpecific_Generic ParticleEmitterSettings { get; set; }
        [Category(categoryName)]
        public AssetID Emitter { get; set; }
        [Category(categoryName), IgnoreVerification]
        public AssetID ParticleSystem { get; set; }
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
        public AssetSingle Velocity_AngleVariation_Rad { get; set; }
        [Category(categoryName)]
        public AssetSingle Velocity_AngleVariation_Deg 
        { 
            get => MathUtil.RadiansToDegrees(Velocity_AngleVariation_Rad);
            set => Velocity_AngleVariation_Rad = MathUtil.DegreesToRadians(value);
        }
        [Category(categoryName)]
        public AssetColor StartColor { get; set; }
        [Category(categoryName)]
        public AssetColor EndColor { get; set; }
        [Category(categoryName)]
        public AssetSingle SizeBirth { get; set; }
        [Category(categoryName)]
        public AssetSingle SizeBirthVariation { get; set; }
        [Category(categoryName)]
        public AssetSingle SizeDeath { get; set; }
        [Category(categoryName)]
        public AssetSingle Life { get; set; }
        [Category(categoryName)]
        public AssetSingle LifeVariation { get; set; }
        [Category(categoryName)]
        public int CullMode { get; set; }
        [Category(categoryName)]
        public AssetSingle CullDistanceSquared { get; set; }
        [Category(categoryName)]
        public AssetByte MaxEmit { get; set; }

        public AssetPARE(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.ParticleEmitter, BaseAssetType.ParticleEmitter)
        {
            Emitter_PosX = position.X;
            Emitter_PosY = position.Y;
            Emitter_PosZ = position.Z;

            if (template == AssetTemplate.Cauldron_Emitter)
            {
                EmitterFlags.FlagValueByte = 0x01;
                EmitterType = EmitterType.Circle;
                Count = 0x03;
                CountVariation = 0x01;
                Interval = 0.1f;
                ParticleEmitterSettings = new PareSpecific_xPECircle() 
                { 
                    Radius = 0.5f
                };
                ParticleSystem = "CAULDRON SYSTEM";
                Velocity_Y = 1f;
                Velocity_AngleVariation_Rad = 5.2359879e-002f;
                StartColor = new AssetColor(0x20, 0xC8, 0x40, 0xFF);
                EndColor = new AssetColor(0x00, 0x96, 0x40, 0x00);
                SizeBirth = 0.15f;
                SizeBirthVariation = 0.05f;
                SizeDeath = 0.05f;
                Life = 1.5f;
                LifeVariation = 0.5f;
                CullMode = 0x03;
                CullDistanceSquared = 100f;
            }
        }

        public AssetPARE(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                EmitterFlags.FlagValueByte = reader.ReadByte();
                _emitterType = (EmitterType)reader.ReadByte();

                if (game == Game.Scooby)
                {
                    Count = reader.ReadByte();
                    CountVariation = reader.ReadByte();
                    Interval = reader.ReadSingle();
                }
                else
                {
                    reader.BaseStream.Position += 2;
                    ParticleProperties = reader.ReadUInt32();
                }

                switch (_emitterType)
                {
                    case EmitterType.Point:
                        ParticleEmitterSettings = new PareSpecific_Generic();
                        break;
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
                        throw new ArgumentException($"Unknown particle emitter type: {_emitterType}");
                } // 0x1C in size

                reader.BaseStream.Position = 0x2C;
                Emitter = reader.ReadUInt32();
                if (game == Game.Scooby)
                    ParticleSystem = reader.ReadUInt32();
                Emitter_PosX = reader.ReadSingle();
                Emitter_PosY = reader.ReadSingle();
                Emitter_PosZ = reader.ReadSingle();
                Velocity_X = reader.ReadSingle();
                Velocity_Y = reader.ReadSingle();
                Velocity_Z = reader.ReadSingle();
                Velocity_AngleVariation_Rad = reader.ReadSingle();
                if (game == Game.Scooby)
                {
                    StartColor = reader.ReadColor();
                    EndColor = reader.ReadColor();
                    SizeBirth = reader.ReadSingle();
                    SizeBirthVariation = reader.ReadSingle();
                    SizeDeath = reader.ReadSingle();
                    Life = reader.ReadSingle();
                    LifeVariation = reader.ReadSingle();
                    reader.ReadByte();
                    reader.ReadByte();
                    CullMode = reader.ReadByte();
                    reader.ReadByte();
                    CullDistanceSquared = reader.ReadSingle();
                    MaxEmit = reader.ReadByte();
                }
                else
                {
                    CullMode = reader.ReadInt32();
                    CullDistanceSquared = reader.ReadSingle();
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(EmitterFlags.FlagValueByte);
            writer.Write((byte)_emitterType);

            if (game == Game.Scooby)
            {
                writer.Write(Count);
                writer.Write(CountVariation);
                writer.Write(Interval);
            }
            else
            {
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write(ParticleProperties);
            }
            ParticleEmitterSettings.Serialize(writer);
            while (writer.BaseStream.Length < 0x2C)
                writer.Write((byte)0);
            writer.Write(Emitter);
            if (game == Game.Scooby)
                writer.Write(ParticleSystem);
            writer.Write(Emitter_PosX);
            writer.Write(Emitter_PosY);
            writer.Write(Emitter_PosZ);
            writer.Write(Velocity_X);
            writer.Write(Velocity_Y);
            writer.Write(Velocity_Z);
            writer.Write(Velocity_AngleVariation_Rad);
            if (game == Game.Scooby)
            {
                writer.Write(StartColor);
                writer.Write(EndColor);
                writer.Write(SizeBirth);
                writer.Write(SizeBirthVariation);
                writer.Write(SizeDeath);
                writer.Write(Life);
                writer.Write(LifeVariation);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)CullMode);
                writer.Write((byte)0);
                writer.Write(CullDistanceSquared);
                writer.Write(MaxEmit);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
            }
            else
            {
                writer.Write(CullMode);
                writer.Write(CullDistanceSquared);
            }
            SerializeLinks(writer);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (game == Game.Scooby)
                Verify(ParticleSystem, "ParticleSystem", true, ref result);
            else
                Verify(ParticleProperties, "ParticleProperties", true, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
            {
                dt.RemoveProperty("ParticleProperties");
            }
            else
            {
                dt.RemoveProperty("Count");
                dt.RemoveProperty("CountVariation");
                dt.RemoveProperty("Interval");
                dt.RemoveProperty("ParticleSystem");
                dt.RemoveProperty("StartColor");
                dt.RemoveProperty("EndColor");
                dt.RemoveProperty("SizeBirth");
                dt.RemoveProperty("SizeBirthVariation");
                dt.RemoveProperty("SizeDeath");
                dt.RemoveProperty("Life");
                dt.RemoveProperty("LifeVariation");
                dt.RemoveProperty("MaxEmit");
            }
            base.SetDynamicProperties(dt);
        }
    }
}