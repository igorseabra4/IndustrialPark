using HipHopFile;
using IndustrialPark.AssetEditorColors;
using IndustrialPark.Models;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaEffectSparkEmitter : RenderableDynaBase
    {
        private const string dynaCategoryName = "effect:spark emitter";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 6;

        [Category(dynaCategoryName)]
        public AssetSingle UpdateDistance { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Texture { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID AttachObjID { get; set; }
        [Category(dynaCategoryName)]
        public int NumSparks { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Period { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PeriodRandom { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LifetimeMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LifetimeRandom { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Yaw { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Pitch { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DirectionVary { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelocityMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelocityRandom { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Gravity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SizeMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SizeRandom { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundAsset { get; set; }

        private const string dynaCategoryNameGlint = dynaCategoryName + ": Glint";
        [Category(dynaCategoryNameGlint)]
        public AssetID TextureGlint { get; set; }
        [Category(dynaCategoryNameGlint)]
        public AssetSingle LengthMin { get; set; }
        [Category(dynaCategoryNameGlint)]
        public AssetSingle LengthRandom { get; set; }
        [Category(dynaCategoryNameGlint)]
        public AssetSingle LengthMax { get; set; }

        private const string dynaCategoryNameLight = dynaCategoryName + ": LightInfo";
        [Category(dynaCategoryNameLight)]
        public AssetSingle LightColorRed { get; set; }
        [Category(dynaCategoryNameLight)]
        public AssetSingle LightColorGreen { get; set; }
        [Category(dynaCategoryNameLight)]
        public AssetSingle LightColorBlue { get; set; }
        [Category(dynaCategoryNameLight)]
        public AssetSingle LightColorAlpha { get; set; }
        [Category(dynaCategoryNameLight)]
        public AssetColor LightColor
        {
            get => AssetColor.FromVector4(LightColorRed, LightColorGreen, LightColorBlue, LightColorAlpha);
            set
            {
                var val = value.ToVector4();
                LightColorRed = val.X;
                LightColorGreen = val.Y;
                LightColorBlue = val.Z;
                LightColorAlpha = val.W;
            }
        }
        [Category(dynaCategoryNameLight)]
        public AssetSingle Radius { get; set; }
        [Category(dynaCategoryNameLight)]
        public AssetSingle Up { get; set; }
        [Category(dynaCategoryNameLight)]
        public AssetSingle Max { get; set; }
        [Category(dynaCategoryNameLight)]
        public AssetSingle Down { get; set; }

        public DynaEffectSparkEmitter(string assetName, Vector3 position) : base(assetName, DynaType.effect__spark_emitter, position)
        {
            LightColor = new AssetColor();
        }

        public DynaEffectSparkEmitter(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__spark_emitter, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                UpdateDistance = reader.ReadSingle();
                Texture = reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
                AttachObjID = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                NumSparks = reader.ReadInt32();
                Period = reader.ReadSingle();
                PeriodRandom = reader.ReadSingle();
                LifetimeMin = reader.ReadSingle();
                LifetimeRandom = reader.ReadSingle();
                Yaw = reader.ReadSingle();
                Pitch = reader.ReadSingle();
                DirectionVary = reader.ReadSingle();
                VelocityMin = reader.ReadSingle();
                VelocityRandom = reader.ReadSingle();
                Gravity = reader.ReadSingle();
                SizeMin = reader.ReadSingle();
                SizeRandom = reader.ReadSingle();
                TextureGlint = reader.ReadUInt32();
                LengthMin = reader.ReadSingle();
                LengthRandom = reader.ReadSingle();
                LengthMax = reader.ReadSingle();
                LightColorRed = reader.ReadSingle();
                LightColorGreen = reader.ReadSingle();
                LightColorBlue = reader.ReadSingle();
                LightColorAlpha = reader.ReadSingle();
                Radius = reader.ReadSingle();
                Up = reader.ReadSingle();
                Max = reader.ReadSingle();
                Down = reader.ReadSingle();
                SoundAsset = reader.ReadUInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(UpdateDistance);
            writer.Write(Texture);
            writer.Write(Flags.FlagValueInt);
            writer.Write(AttachObjID);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(NumSparks);
            writer.Write(Period);
            writer.Write(PeriodRandom);
            writer.Write(LifetimeMin);
            writer.Write(LifetimeRandom);
            writer.Write(Yaw);
            writer.Write(Pitch);
            writer.Write(DirectionVary);
            writer.Write(VelocityMin);
            writer.Write(VelocityRandom);
            writer.Write(Gravity);
            writer.Write(SizeMin);
            writer.Write(SizeRandom);
            writer.Write(TextureGlint);
            writer.Write(LengthMin);
            writer.Write(LengthRandom);
            writer.Write(LengthMax);
            writer.Write(LightColorRed);
            writer.Write(LightColorGreen);
            writer.Write(LightColorBlue);
            writer.Write(LightColorAlpha);
            writer.Write(Radius);
            writer.Write(Up);
            writer.Write(Max);
            writer.Write(Down);
            writer.Write(SoundAsset);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer) => renderer.DrawCube(world, isSelected);

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}
