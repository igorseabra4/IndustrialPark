using HipHopFile;
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
    public class DynaEffectScreenWarp : RenderableDynaBase
    {
        private const string dynaCategoryName = "effect:ScreenWarp";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public uint EffectType { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor(
            "Enabled",
            "Running");

        protected float _radius;
        [Category("DYNA Placement")]
        public AssetSingle Radius
        {
            get => _radius;
            set { _radius = value; CreateTransformMatrix(); }
        }

        [Category(dynaCategoryName)]
        public AssetSingle Duration { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RepeatDelay { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Intensity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Frequency { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TriggerDistance { get; set; }

        public DynaEffectScreenWarp(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__ScreenWarp, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                EffectType = reader.ReadUInt32();
                Flags.FlagValueInt = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _radius = reader.ReadSingle();
                Duration = reader.ReadSingle();
                RepeatDelay = reader.ReadSingle();
                Intensity = reader.ReadSingle();
                Frequency = reader.ReadSingle();
                TriggerDistance = reader.ReadSingle();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(EffectType);
            writer.Write(Flags.FlagValueInt);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_radius);
            writer.Write(Duration);
            writer.Write(RepeatDelay);
            writer.Write(Intensity);
            writer.Write(Frequency);
            writer.Write(TriggerDistance);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.sphereVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.sphereTriangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Scaling(_radius * 2f) * Matrix.Translation(_position);
            CreateBoundingBox();
        }

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawSphere(world, isSelected, renderer.trigColor);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}