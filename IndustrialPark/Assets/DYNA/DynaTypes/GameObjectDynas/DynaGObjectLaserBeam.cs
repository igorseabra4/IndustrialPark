using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public enum ColorAnimType : byte
    {
        None,
        Pulse,
        Flicker
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LaserBeam_Beam : GenericAssetDataContainer
    {
        public AssetID Texture { get; set; }
        public AssetSingle Thickness { get; set; }
        public AssetSingle FadeDist { get; set; }
        public AssetSingle MaxDist { get; set; }
        public AssetSingle Taper { get; set; }
        public AssetByte Volume { get; set; }
        public AssetByte BlendMode { get; set; }
        public AssetColor Color { get; set; }

        public LaserBeam_Beam()
        {
            Color = new AssetColor();
        }

        public LaserBeam_Beam(EndianBinaryReader reader)
        {
            Texture = reader.ReadUInt32();
            Thickness = reader.ReadSingle();
            FadeDist = reader.ReadSingle();
            MaxDist = reader.ReadSingle();
            Taper = reader.ReadSingle();
            Volume = reader.ReadByte();
            BlendMode = reader.ReadByte();
            reader.ReadInt16();
            Color = reader.ReadColor();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Texture);
            writer.Write(Thickness);
            writer.Write(FadeDist);
            writer.Write(MaxDist);
            writer.Write(Taper);
            writer.Write(Volume);
            writer.Write(BlendMode);
            writer.Write((short)0);
            writer.Write(Color);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LaserBeam_Ribbon : GenericAssetDataContainer
    {
        public AssetID Texture { get; set; }
        public AssetSingle Lifetime { get; set; }
        public AssetSingle Scale { get; set; }
        public AssetByte Orient { get; set; }
        public AssetByte ResponseCurve { get; set; }
        public AssetByte BlendMode { get; set; }
        public AssetByte Glow { get; set; }

        public LaserBeam_Ribbon(EndianBinaryReader reader)
        {
            Texture = reader.ReadUInt32();
            Lifetime = reader.ReadSingle();
            Scale = reader.ReadSingle();
            Orient = reader.ReadByte();
            ResponseCurve = reader.ReadByte();
            BlendMode = reader.ReadByte();
            Glow = reader.ReadByte();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Texture);
            writer.Write(Lifetime);
            writer.Write(Scale);
            writer.Write(Orient);
            writer.Write(ResponseCurve);
            writer.Write(BlendMode);
            writer.Write(Glow);
        }
    }

    public class DynaGObjectLaserBeam : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:laser_beam";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public FlagBitmask LaserBeamFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID Attach { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Target { get; set; }
        [Category(dynaCategoryName)]
        public AssetID OriginEmitter { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StrikeEmitter { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OriginX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OriginY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OriginZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TargetOffsetX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TargetOffsetY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TargetOffsetZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Speed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SegmentDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Knockback { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Movement { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AttachBone { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte TargetBone { get; set; }
        [Category(dynaCategoryName)]
        public ColorAnimType ColorAnimType { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte FadeInType { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte FadeOutType { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FadeInTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FadeOutTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamagePlayer { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageNPC { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageOther { get; set; }
        [Category(dynaCategoryName)]
        public LaserBeam_Beam Beam { get; set; }
        [Category(dynaCategoryName)]
        public LaserBeam_Ribbon Ribbon { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ColorAnimPulseFrequency { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ColorAnimPulseIntensityMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ColorAnimPulseIntensityMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ColorAnimPulseGlowMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ColorAnimPulseGlowMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ColorAnimFlickerParam { get => ColorAnimPulseFrequency; set => ColorAnimPulseFrequency = value; }

        public DynaGObjectLaserBeam(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__laser_beam, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                LaserBeamFlags.FlagValueInt = reader.ReadUInt32();
                Attach = reader.ReadUInt32();
                Target = reader.ReadUInt32();
                OriginEmitter = reader.ReadUInt32();
                StrikeEmitter = reader.ReadUInt32();
                OriginX = reader.ReadSingle();
                OriginY = reader.ReadSingle();
                OriginZ = reader.ReadSingle();
                if (game >= Game.ROTU)
                {
                    TargetOffsetX = reader.ReadSingle();
                    TargetOffsetY = reader.ReadSingle();
                    TargetOffsetZ = reader.ReadSingle();
                }
                Speed = reader.ReadSingle();
                SegmentDist = reader.ReadSingle();
                Knockback = reader.ReadSingle();
                Movement = reader.ReadByte();
                AttachBone = reader.ReadByte();
                TargetBone = reader.ReadByte();
                ColorAnimType = (ColorAnimType)reader.ReadByte();
                FadeInType = reader.ReadByte();
                FadeOutType = reader.ReadByte();
                reader.ReadInt16();
                FadeInTime = reader.ReadSingle();
                FadeOutTime = reader.ReadSingle();
                DamagePlayer = reader.ReadSingle();
                if (game < Game.ROTU)
                    DamageNPC = reader.ReadSingle();
                DamageOther = reader.ReadSingle();
                Beam = new LaserBeam_Beam(reader);
                Ribbon = new LaserBeam_Ribbon(reader);
                ColorAnimPulseFrequency = reader.ReadSingle();
                ColorAnimPulseIntensityMin = reader.ReadSingle();
                ColorAnimPulseIntensityMax = reader.ReadSingle();
                ColorAnimPulseGlowMin = reader.ReadSingle();
                ColorAnimPulseGlowMax = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(LaserBeamFlags.FlagValueInt);
            writer.Write(Attach);
            writer.Write(Target);
            writer.Write(OriginEmitter);
            writer.Write(StrikeEmitter);
            writer.Write(OriginX);
            writer.Write(OriginY);
            writer.Write(OriginZ);
            if (game >= Game.ROTU)
            {
                writer.Write(TargetOffsetX);
                writer.Write(TargetOffsetY);
                writer.Write(TargetOffsetZ);
            }
            writer.Write(Speed);
            writer.Write(SegmentDist);
            writer.Write(Knockback);
            writer.Write(Movement);
            writer.Write(AttachBone);
            writer.Write(TargetBone);
            writer.Write((byte)ColorAnimType);
            writer.Write(FadeInType);
            writer.Write(FadeOutType);
            writer.Write((short)0);
            writer.Write(FadeInTime);
            writer.Write(FadeOutTime);
            writer.Write(DamagePlayer);
            if (game < Game.ROTU)
                writer.Write(DamageNPC);
            writer.Write(DamageOther);
            Beam.Serialize(writer);
            Ribbon.Serialize(writer);
            writer.Write(ColorAnimPulseFrequency);
            writer.Write(ColorAnimPulseIntensityMin);
            writer.Write(ColorAnimPulseIntensityMax);
            writer.Write(ColorAnimPulseGlowMin);
            writer.Write(ColorAnimPulseGlowMax);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.ROTU)
            {
                dt.RemoveProperty("TargetOffsetX");
                dt.RemoveProperty("TargetOffsetY");
                dt.RemoveProperty("TargetOffsetZ");
            }
            else
                dt.RemoveProperty("DamageNPC");
        }
    }
}