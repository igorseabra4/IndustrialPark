using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectFlameEmitter : RenderableDynaBase
    {
        private const string dynaCategoryName = "game_object:flame_emitter";

        protected override int constVersion => 4;

        [Category(dynaCategoryName)]
        public int UnknownInt_00 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_14 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_18 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_1C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_20 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_24 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_28 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_2C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_30 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_34 { get; set; }

        public DynaGObjectFlameEmitter(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__flame_emitter, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            UnknownInt_00 = reader.ReadInt32();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            UnknownFloat_10 = reader.ReadSingle();
            UnknownFloat_14 = reader.ReadSingle();
            UnknownFloat_18 = reader.ReadSingle();
            UnknownFloat_1C = reader.ReadSingle();
            UnknownFloat_20 = reader.ReadSingle();
            UnknownFloat_24 = reader.ReadSingle();
            UnknownFloat_28 = reader.ReadSingle();
            UnknownFloat_2C = reader.ReadSingle();
            UnknownFloat_30 = reader.ReadSingle();
            UnknownFloat_34 = reader.ReadSingle();

            CreateTransformMatrix();
            renderableAssets.Add(this);
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(UnknownInt_00);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(UnknownFloat_10);
            writer.Write(UnknownFloat_14);
            writer.Write(UnknownFloat_18);
            writer.Write(UnknownFloat_1C);
            writer.Write(UnknownFloat_20);
            writer.Write(UnknownFloat_24);
            writer.Write(UnknownFloat_28);
            writer.Write(UnknownFloat_2C);
            writer.Write(UnknownFloat_30);
            writer.Write(UnknownFloat_34);

            return writer.ToArray();
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
        }
    }
}