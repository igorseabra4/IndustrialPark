using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaPointer : RenderableRotatableDynaBase
    {
        protected override int constVersion => 1;

        public DynaPointer(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.pointer, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _yaw = reader.ReadSingle();
            _pitch = reader.ReadSingle();
            _roll = reader.ReadSingle();

            CreateTransformMatrix();
            renderableAssets.Add(this);
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);

            return writer.ToArray();
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
        }
    }
}