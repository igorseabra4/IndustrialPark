using AssetEditorColors;
using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
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
        [Category(dynaCategoryName), Description("0 = none, 1 = pulse, 2 = flicker")]
        public AssetByte ColorAnimType { get; set; }
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
        public AssetID BeamTexture { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BeamThickness { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BeamFadeDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BeamMaxDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BeamTaper { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte BeamVolume { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte BeamBlendMode { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor BeamColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetID RibbonTexture { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RibbonLifeTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RibbonScale { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte RibbonOrient { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte RibbonResponseCurve { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte RibbonBlendMode { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte RibbonGlow { get; set; }
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
                Speed = reader.ReadSingle();
                SegmentDist = reader.ReadSingle();
                Knockback = reader.ReadSingle();
                Movement = reader.ReadByte();
                AttachBone = reader.ReadByte();
                TargetBone = reader.ReadByte();
                ColorAnimType = reader.ReadByte();
                FadeInType = reader.ReadByte();
                FadeOutType = reader.ReadByte();
                reader.ReadInt16();
                FadeInTime = reader.ReadSingle();
                FadeOutTime = reader.ReadSingle();
                DamagePlayer = reader.ReadSingle();
                DamageNPC = reader.ReadSingle();
                DamageOther = reader.ReadSingle();
                BeamTexture = reader.ReadUInt32();
                BeamThickness = reader.ReadSingle();
                BeamFadeDist = reader.ReadSingle();
                BeamMaxDist = reader.ReadSingle();
                BeamTaper = reader.ReadSingle();
                BeamVolume = reader.ReadByte();
                BeamBlendMode = reader.ReadByte();
                reader.ReadInt16();
                BeamColor = reader.ReadColor();
                RibbonTexture = reader.ReadUInt32();
                RibbonLifeTime = reader.ReadSingle();
                RibbonScale = reader.ReadSingle();
                RibbonOrient = reader.ReadByte();
                RibbonResponseCurve = reader.ReadByte();
                RibbonBlendMode = reader.ReadByte();
                RibbonGlow = reader.ReadByte();
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
                writer.Write(Speed);
                writer.Write(SegmentDist);
                writer.Write(Knockback);
                writer.Write(Movement);
                writer.Write(AttachBone);
                writer.Write(TargetBone);
                writer.Write(ColorAnimType);
                writer.Write(FadeInType);
                writer.Write(FadeOutType);
                writer.Write((short)0);
                writer.Write(FadeInTime);
                writer.Write(FadeOutTime);
                writer.Write(DamagePlayer);
                writer.Write(DamageNPC);
                writer.Write(DamageOther);
                writer.Write(BeamTexture);
                writer.Write(BeamThickness);
                writer.Write(BeamFadeDist);
                writer.Write(BeamMaxDist);
                writer.Write(BeamTaper);
                writer.Write(BeamVolume);
                writer.Write(BeamBlendMode);
                writer.Write((short)0);
                writer.Write(BeamColor);
                writer.Write(RibbonTexture);
                writer.Write(RibbonLifeTime);
                writer.Write(RibbonScale);
                writer.Write(RibbonOrient);
                writer.Write(RibbonResponseCurve);
                writer.Write(RibbonBlendMode);
                writer.Write(RibbonGlow);
                writer.Write(ColorAnimPulseFrequency);
                writer.Write(ColorAnimPulseIntensityMin);
                writer.Write(ColorAnimPulseIntensityMax);
                writer.Write(ColorAnimPulseGlowMin);
                writer.Write(ColorAnimPulseGlowMax);

                
        }
    }
}