using HipHopFile;
using IndustrialPark.AssetEditorColors;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectSpotlight : AssetDYNA
    {
        private const string dynaCategoryName = "effect:spotlight";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(OriginEntity);

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID OriginEntity { get; set; }
        [Category(dynaCategoryName)]
        public AssetID TargetEntity { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AttachBone { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte TargetBone { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Radius { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ViewAngle_Rad { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ViewAngle_Deg
        {
            get => MathUtil.RadiansToDegrees(ViewAngle_Rad);
            set => ViewAngle_Rad = MathUtil.DegreesToRadians(value);
        }
        [Category(dynaCategoryName)]
        public AssetSingle MaxDist { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor LightColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor AuraColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetID FlareTexture { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor FlareColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SizeMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SizeMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte GlowMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte GlowMax { get; set; }

        public DynaEffectSpotlight(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__spotlight, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Flags.FlagValueInt = reader.ReadUInt32();
                OriginEntity = reader.ReadUInt32();
                TargetEntity = reader.ReadUInt32();
                AttachBone = reader.ReadByte();
                TargetBone = reader.ReadByte();
                reader.ReadInt16();
                Radius = reader.ReadSingle();
                ViewAngle_Rad = reader.ReadSingle();
                MaxDist = reader.ReadSingle();
                LightColor = reader.ReadColor();
                AuraColor = reader.ReadColor();
                FlareTexture = reader.ReadUInt32();
                FlareColor = reader.ReadColor();
                SizeMin = reader.ReadSingle();
                SizeMax = reader.ReadSingle();
                GlowMin = reader.ReadByte();
                GlowMax = reader.ReadByte();
                reader.ReadInt16();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(Flags.FlagValueInt);
            writer.Write(OriginEntity);
            writer.Write(TargetEntity);
            writer.Write(AttachBone);
            writer.Write(TargetBone);
            writer.Write((short)0);
            writer.Write(Radius);
            writer.Write(ViewAngle_Rad);
            writer.Write(MaxDist);
            writer.Write(LightColor);
            writer.Write(AuraColor);
            writer.Write(FlareTexture);
            writer.Write(FlareColor);
            writer.Write(SizeMin);
            writer.Write(SizeMax);
            writer.Write(GlowMin);
            writer.Write(GlowMax);
            writer.Write((short)0);
        }
    }
}