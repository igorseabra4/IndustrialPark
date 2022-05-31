using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaIncrediblesIcon : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "Incredibles:Icon";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetSingle Radius { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask IconFlags { get; set; } = IntFlagsDescriptor();

        public DynaIncrediblesIcon(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Incredibles__Icon, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                Radius = reader.ReadSingle();
                IconFlags.FlagValueInt = reader.ReadUInt32();

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
                writer.Write(Radius);
                writer.Write(IconFlags.FlagValueInt);

                return writer.ToArray();
            }
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}