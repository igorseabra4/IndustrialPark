using HipHopFile;
using IndustrialPark.AssetEditorColors;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
   public enum DynaEffectParticleGeneratorAttachType
    {
        attach_fixed = 0,
        attach_entity = 1,
        attach_entity_tag = 2
    }
    public enum DynaEffectParticleGeneratorMotionType
    {
        motion_none = 0,
        motion_spiral = 1,
    }
    public enum DynaEffectParticleGeneratorVolumeType
    {
        volume_point = 0,
        volume_sphere = 1,
        volume_circle = 2,
        volume_line = 3,
        volume_model = 4,
        volume_box = 5
    }

    public class DynaEffectParticleGenerator : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "effect:particle generator";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Texture);

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public FlagBitmask ParticleGeneratorFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetSingle Rate { get; set; }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Texture { get; set; }
        [Category(dynaCategoryName), Description("ROTU only")]
        public AssetSingle EmitDistance { get; set; }

        private const string dynaCategoryNameAttach = dynaCategoryName + ": Attach Data";

        [Category(dynaCategoryNameAttach)]
        public DynaEffectParticleGeneratorAttachType AttachType { get; set; }
        [Category(dynaCategoryNameAttach)]
        public FlagBitmask AttachFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryNameAttach), Description("attach_entity and attach_entity_tag only")]
        public AssetID Entity { get; set; }
        [Category(dynaCategoryNameAttach), Description("attach_entity only")]
        public byte Bone { get; set; }
        [Category(dynaCategoryNameAttach), Description("attach_entity_tag only")]
        public AssetSingle AttachEntityTagX { get; set; }
        [Category(dynaCategoryNameAttach), Description("attach_entity_tag only")]
        public AssetSingle AttachEntityTagY { get; set; }
        [Category(dynaCategoryNameAttach), Description("attach_entity_tag only")]
        public AssetSingle AttachEntityTagZ { get; set; }

        private const string dynaCategoryNameMotion = dynaCategoryName + ": Motion Data";
        private const string motionSpiralOnly = "motion_spiral only";

        [Category(dynaCategoryNameMotion)]
        public DynaEffectParticleGeneratorMotionType MotionType { get; set; }
        [Category(dynaCategoryNameMotion)]
        public FlagBitmask MotionFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryNameMotion), Description(motionSpiralOnly)]
        public FlagBitmask MotionSpiralFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryName), Description(motionSpiralOnly)]
        public AssetByte Points { get; set; }
        [Category(dynaCategoryName), Description(motionSpiralOnly)]
        public AssetSingle RadiusInner { get; set; }
        [Category(dynaCategoryName), Description(motionSpiralOnly)]
        public AssetSingle RadiusOuter { get; set; }
        [Category(dynaCategoryName), Description(motionSpiralOnly)]
        public AssetSingle Duration { get; set; }
        [Category(dynaCategoryName), Description(motionSpiralOnly)]
        public AssetSingle Frequency { get; set; }

        private const string dynaCategoryNameVolume = dynaCategoryName + ": Volume Data";
        [Category(dynaCategoryNameVolume)]
        public DynaEffectParticleGeneratorVolumeType VolumeType { get; set; }
        [Category(dynaCategoryNameVolume)]
        public FlagBitmask VolumeFlags { get; set; } = ByteFlagsDescriptor();

        [Category(dynaCategoryNameVolume), Description("volume_sphere and volume_circle only")]
        public AssetSingle Radius { get; set; }
        [Category(dynaCategoryNameVolume), Description("volume_circle only")]
        public AssetSingle CircleArcLength { get; set; }
        [Category(dynaCategoryName), Description("volume_circle rotu only")]
        public AssetSingle Height { get; set; }

        [Category(dynaCategoryNameVolume), Description("volume_line only")]
        public FlagBitmask LineFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryNameVolume), Description("volume_line only")]
        public AssetSingle LineRadius { get; set; }
        [Category(dynaCategoryNameVolume), Description("volume_line only")]
        public AssetSingle LineLength { get; set; }

        [Category(dynaCategoryNameVolume), Description("volume_model only")]
        public FlagBitmask ModelFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryNameVolume), Description("volume_model only")]
        public AssetByte ModelExclude { get; set; }
        [Category(dynaCategoryNameVolume), Description("volume_model only")]
        public AssetSingle ModelExpand { get; set; }

        [Category(dynaCategoryNameVolume), Description("volume_box only")]
        public AssetSingle DimensionsX { get; set; }
        [Category(dynaCategoryNameVolume), Description("volume_box only")]
        public AssetSingle DimensionsY { get; set; }
        [Category(dynaCategoryNameVolume), Description("volume_box only")]
        public AssetSingle DimensionsZ { get; set; }

        private const string dynaCategoryNameParSystem = dynaCategoryName + ": Particle System";
        private ParticleSystemType _systemtype;
        [Category(dynaCategoryNameParSystem)]
        public ParticleSystemType SystemType
        {
            get => _systemtype;
            set
            {
                _systemtype = value;
                switch (value)
                {
                    case ParticleSystemType.Waterfall:
                        ParticleSystem = new ParticleSystem_Waterfall();
                        break;
                    case ParticleSystemType.WaterfallMist:
                        ParticleSystem = new ParticleSystem_WaterfallMist();
                        break;
                    case ParticleSystemType.WaterfallSplash:
                        ParticleSystem = new ParticleSystem_WaterfallSplash();
                        break;
                    case ParticleSystemType.Snow:
                        ParticleSystem = new ParticleSystem_Snow();
                        break;
                    case ParticleSystemType.Rain:
                        ParticleSystem = new ParticleSystem_Rain();
                        break;
                    case ParticleSystemType.EarthDust:
                        ParticleSystem = new ParticleSystem_EarthDust();
                        break;
                    case ParticleSystemType.Pop:
                        ParticleSystem = new ParticleSystem_Pop();
                        break;
                    case ParticleSystemType.BurrowKickup:
                        ParticleSystem = new ParticleSystem_BurrowKick();
                        break;
                    case ParticleSystemType.Puffer:
                        ParticleSystem = new ParticleSystem_Puffer();
                        break;
                }
            }
        }
        [Category(dynaCategoryNameParSystem), TypeConverter(typeof(ExpandableObjectConverter))]
        public ParticleSystem_Generic ParticleSystem { get; set; }

        public DynaEffectParticleGenerator(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Effect__particle_generator, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                ParticleGeneratorFlags.FlagValueByte = reader.ReadByte();
                AttachFlags.FlagValueByte = reader.ReadByte();
                MotionFlags.FlagValueByte = reader.ReadByte();
                VolumeFlags.FlagValueByte = reader.ReadByte();
                Rate = reader.ReadSingle();
                Texture = reader.ReadUInt32();
                if (game >= Game.ROTU)
                    EmitDistance = reader.ReadSingle();
                AttachType = (DynaEffectParticleGeneratorAttachType)reader.ReadByte();
                MotionType = (DynaEffectParticleGeneratorMotionType)reader.ReadByte();
                VolumeType = (DynaEffectParticleGeneratorVolumeType)reader.ReadByte();
                SystemType = (ParticleSystemType)reader.ReadByte();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();

                Entity = reader.ReadUInt32();
                if (AttachType == DynaEffectParticleGeneratorAttachType.attach_entity_tag)
                {
                    AttachEntityTagX = reader.ReadSingle();
                    AttachEntityTagY = reader.ReadSingle();
                    AttachEntityTagZ = reader.ReadSingle();
                }
                else
                {
                    Bone = reader.ReadByte();
                    reader.BaseStream.Position += 11;
                }

                MotionSpiralFlags.FlagValueByte = reader.ReadByte();
                Points = reader.ReadByte();
                reader.BaseStream.Position += 2;
                RadiusInner = reader.ReadSingle();
                RadiusOuter = reader.ReadSingle();
                Duration = reader.ReadSingle();
                Frequency = reader.ReadSingle();

                if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_point)
                {
                    reader.BaseStream.Position += 12;
                }
                else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_sphere || VolumeType == DynaEffectParticleGeneratorVolumeType.volume_circle)
                {
                    Radius = reader.ReadSingle();
                    CircleArcLength = reader.ReadSingle();
                    if (game >= Game.ROTU)
                        Height = reader.ReadSingle();
                    else
                        reader.BaseStream.Position += 4;
                }
                else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_line)
                {
                    LineFlags.FlagValueByte = reader.ReadByte();
                    reader.BaseStream.Position += 3;
                    LineRadius = reader.ReadSingle();
                    LineLength = reader.ReadSingle();
                }
                else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_model)
                {
                    ModelFlags.FlagValueByte = reader.ReadByte();
                    ModelExclude = reader.ReadByte();
                    reader.BaseStream.Position += 2;
                    ModelExpand = reader.ReadSingle();
                    reader.BaseStream.Position += 4;
                }
                else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_box)
                {
                    DimensionsX = reader.ReadSingle();
                    DimensionsY = reader.ReadSingle();
                    DimensionsZ = reader.ReadSingle();
                }

                switch (SystemType)
                {
                    case ParticleSystemType.Waterfall:
                        ParticleSystem = new ParticleSystem_Waterfall(reader);
                        break;
                    case ParticleSystemType.WaterfallMist:
                        ParticleSystem = new ParticleSystem_WaterfallMist(reader);
                        break;
                    case ParticleSystemType.WaterfallSplash:
                        ParticleSystem = new ParticleSystem_WaterfallSplash(reader);
                        break;
                    case ParticleSystemType.Snow:
                        ParticleSystem = new ParticleSystem_Snow(reader);
                        break;
                    case ParticleSystemType.Rain:
                        ParticleSystem = new ParticleSystem_Rain(reader);
                        break;
                    case ParticleSystemType.EarthDust:
                        ParticleSystem = new ParticleSystem_EarthDust(reader);
                        break;
                    case ParticleSystemType.Pop:
                        ParticleSystem = new ParticleSystem_Pop(reader);
                        break;
                    case ParticleSystemType.BurrowKickup:
                        ParticleSystem = new ParticleSystem_BurrowKick(reader);
                        break;
                    case ParticleSystemType.Puffer:
                        ParticleSystem = new ParticleSystem_Puffer(reader);
                        break;
                    default:
                        throw new Exception($"Unknown particle system type: {SystemType}");
                }


                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(ParticleGeneratorFlags.FlagValueByte);
            writer.Write(AttachFlags.FlagValueByte);
            writer.Write(MotionFlags.FlagValueByte);
            writer.Write(VolumeFlags.FlagValueByte);
            writer.Write(Rate);
            writer.Write(Texture);
            if (game >= Game.ROTU)
                writer.Write(EmitDistance);
            writer.Write((byte)AttachType);
            writer.Write((byte)MotionType);
            writer.Write((byte)VolumeType);
            writer.Write((byte)SystemType);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(Entity);

            if (AttachType == DynaEffectParticleGeneratorAttachType.attach_entity_tag)
            {
                writer.Write(AttachEntityTagX);
                writer.Write(AttachEntityTagY);
                writer.Write(AttachEntityTagZ);
            }
            else
            {
                writer.Write(Bone);
                writer.Write(new byte[11]);
            }

            writer.Write(MotionSpiralFlags.FlagValueByte);
            writer.Write(Points);
            writer.Write(new byte[2]);
            writer.Write(RadiusInner);
            writer.Write(RadiusOuter);
            writer.Write(Duration);
            writer.Write(Frequency);

            if (VolumeType == 0)
            {
                writer.Write(new byte[12]);
            }
            else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_sphere || VolumeType == DynaEffectParticleGeneratorVolumeType.volume_circle)
            {
                writer.Write(Radius);
                writer.Write(CircleArcLength);
                if (game >= Game.ROTU)
                    writer.Write(Height);
                else
                    writer.Write(0);
            }
            else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_line)
            {
                writer.Write(LineFlags.FlagValueByte);
                writer.Write(new byte[3]);
                writer.Write(LineRadius);
                writer.Write(LineLength);
            }
            else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_model)
            {
                writer.Write(ModelFlags.FlagValueByte);
                writer.Write(ModelExclude);
                writer.Write(new byte[2]);
                writer.Write(ModelExpand);
                writer.Write(new byte[4]);
            }
            else if (VolumeType == DynaEffectParticleGeneratorVolumeType.volume_box)
            {
                writer.Write(DimensionsX);
                writer.Write(DimensionsY);
                writer.Write(DimensionsZ);
            }
            ParticleSystem.Serialize(writer);

        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationX(-MathUtil.PiOverTwo)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(PositionX, PositionY, PositionZ);

            base.CreateBoundingBox();
        }

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}