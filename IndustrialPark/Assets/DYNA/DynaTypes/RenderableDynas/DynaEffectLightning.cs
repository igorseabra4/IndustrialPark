using AssetEditorColors;
using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaEffectLightning : RenderableDynaBase
    {
        private const string dynaCategoryName = "effect:Lightning";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 2;

        public override void Verify(ref List<string> result)
        {
            Verify(LightningTexture, ref result);
            Verify(GlowTexture, ref result);
            Verify(SimpleObject1, ref result);
            Verify(SimpleObject2, ref result);
            base.Verify(ref result);
        }

        protected Vector3 _positionEnd;
        [Category(dynaCategoryName)]
        public AssetSingle PositionEndX
        {
            get => _positionEnd.X;
            set { _positionEnd.X = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public AssetSingle PositionEndY
        {
            get => _positionEnd.Y;
            set { _positionEnd.Y = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public AssetSingle PositionEndZ
        {
            get => _positionEnd.Z;
            set { _positionEnd.Z = value; CreateTransformMatrix(); }
        }

        [Category(dynaCategoryName)]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Width { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BranchSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LightningTexture { get; set; }
        [Category(dynaCategoryName)]
        public AssetID GlowTexture { get; set; }
        [Category(dynaCategoryName)]
        public int Damage { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle KnockbackSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundGroup { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundHit1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundHit2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SimpleObject1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SimpleObject2 { get; set; }
        [Category(dynaCategoryName)]
        public bool DamagePlayer { get; set; }

        public DynaEffectLightning(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__Lightning, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _positionEnd = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Color = reader.ReadColor();
                Width = reader.ReadSingle();
                BranchSpeed = reader.ReadSingle();
                LightningTexture = reader.ReadUInt32();
                GlowTexture = reader.ReadUInt32();
                Damage = reader.ReadInt32();
                KnockbackSpeed = reader.ReadSingle();
                SoundGroup = reader.ReadUInt32();
                SoundHit1 = reader.ReadUInt32();
                SoundHit2 = reader.ReadUInt32();
                SimpleObject1 = reader.ReadUInt32();
                SimpleObject2 = reader.ReadUInt32();
                DamagePlayer = reader.ReadInt32Bool();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(_positionEnd.X);
                writer.Write(_positionEnd.Y);
                writer.Write(_positionEnd.Z);
                writer.Write(Color);
                writer.Write(Width);
                writer.Write(BranchSpeed);
                writer.Write(LightningTexture);
                writer.Write(GlowTexture);
                writer.Write(Damage);
                writer.Write(KnockbackSpeed);
                writer.Write(SoundGroup);
                writer.Write(SoundHit1);
                writer.Write(SoundHit2);
                writer.Write(SimpleObject1);
                writer.Write(SimpleObject2);
                writer.Write(DamagePlayer ? 1 : 0);

                return writer.ToArray();
            }
        }

        private Matrix world2;

        public override void CreateTransformMatrix()
        {
            world2 = Matrix.Translation(_positionEnd);
            base.CreateTransformMatrix();
        }

        protected override List<Vector3> vertexSource => SimpleObject1 == 0 ? SharpRenderer.cubeVertices : new List<Vector3>();

        protected override List<Triangle> triangleSource => SimpleObject1 == 0 ? SharpRenderer.cubeTriangles : new List<Triangle>();

        public override void Draw(SharpRenderer renderer)
        {
            if (SimpleObject1 == 0)
                renderer.DrawCube(world, isSelected);
            if (isSelected && SimpleObject2 == 0)
                renderer.DrawCube(world2, true);
        }

        public override bool ShouldDraw(SharpRenderer renderer)
        {
            if (SimpleObject1 == 0)
                return base.ShouldDraw(renderer);
            return false;
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}