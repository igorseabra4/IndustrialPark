using AssetEditorColors;
using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaEffectFlamethrower : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "effect:Flamethrower";

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetByte Visible { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OnLength { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffLength { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName)]
        public int Damage { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Knockback { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageRadius { get; set; }

        public DynaEffectFlamethrower(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__Flamethrower, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                Visible = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                OnLength = reader.ReadSingle();
                OffLength = reader.ReadSingle();
                Color = reader.ReadColor();
                Damage = reader.ReadInt32();
                Knockback = reader.ReadSingle();
                DamageRadius = reader.ReadSingle();

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
                writer.Write(_yaw);
                writer.Write(_pitch);
                writer.Write(_roll);
                writer.Write(Visible);
                writer.Write((short)0);
                writer.Write((byte)0);
                writer.Write(OnLength);
                writer.Write(OffLength);
                writer.Write(Color);
                writer.Write(Damage);
                writer.Write(Knockback);
                writer.Write(DamageRadius);

                return writer.ToArray();
            }
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void Draw(SharpRenderer renderer) => renderer.DrawPyramid(world, isSelected);

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}