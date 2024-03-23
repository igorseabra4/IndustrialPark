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
    public enum eLightType
    {
        Ambient = 0,
        Spotlight = 1,
        Directional = 2,
        Point = 3,
    }
    public class DynaEffectLight : RenderableDynaBase
    {
        private const string dynaCategoryName = "effect:light";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public eLightType LightType { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LightEffectID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LightEffectSpeed { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask LightFlags { get; set; } = IntFlagsDescriptor();

        [Category(dynaCategoryName), DisplayName("Red (0 - 1)")]
        public AssetSingle RedMult { get; set; }
        [Category(dynaCategoryName), DisplayName("Green (0 - 1)")]
        public AssetSingle GreenMult { get; set; }
        [Category(dynaCategoryName), DisplayName("Blue (0 - 1)")]
        public AssetSingle BlueMult { get; set; }
        [Category(dynaCategoryName), DisplayName("Alpha (0 - 1)")]
        public AssetSingle SeeThru { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor ColorRGBA
        {
            get => AssetColor.FromVector4(RedMult, GreenMult, BlueMult, SeeThru);
            set
            {
                var val = value.ToVector4();
                RedMult = val.X;
                GreenMult = val.Y;
                BlueMult = val.Z;
                SeeThru = val.W;
            }
        }

        protected float _radius;
        [Category("DYNA Placement")]
        public AssetSingle Radius
        {
            get => _radius;
            set { _radius = value; CreateTransformMatrix(); }
        }

        [Category(dynaCategoryName)]
        public AssetID AttachID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LightCardID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LightCardScale { get; set; }

        public DynaEffectLight(string assetName, Vector3 position) : base(assetName, DynaType.effect__light, position)
        {
            _radius = 2;
            LightEffectSpeed = 1f;
            LightCardScale = 1f;
            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        public DynaEffectLight(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__light, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                LightType = (eLightType)reader.ReadUInt32();
                LightEffectID = reader.ReadUInt32();
                LightEffectSpeed = reader.ReadSingle();
                LightFlags.FlagValueInt = reader.ReadUInt32();
                RedMult = reader.ReadSingle();
                GreenMult = reader.ReadSingle();
                BlueMult = reader.ReadSingle();
                SeeThru = reader.ReadSingle();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _radius = reader.ReadSingle();
                AttachID = reader.ReadUInt32();
                LightCardID = reader.ReadUInt32();
                LightCardScale = reader.ReadSingle();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write((uint)LightType);
            writer.Write(LightEffectID);
            writer.Write(LightEffectSpeed);
            writer.Write(LightFlags.FlagValueInt);
            writer.Write(RedMult);
            writer.Write(GreenMult);
            writer.Write(BlueMult);
            writer.Write(SeeThru);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_radius);
            writer.Write(AttachID);
            writer.Write(LightCardID);
            writer.Write(LightCardScale);
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