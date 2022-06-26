using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaPointer : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "pointer";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        public DynaPointer(string assetName, Vector3 position) : base(assetName, DynaType.pointer, 1, position)
        {
            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        public DynaPointer(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.pointer, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();

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

                return writer.ToArray();
            }
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}