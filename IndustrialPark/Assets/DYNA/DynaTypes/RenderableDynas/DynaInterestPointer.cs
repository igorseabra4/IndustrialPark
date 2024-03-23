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
    public class DynaInterestPointer : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "Interest Pointer";
        public override string TypeString => dynaCategoryName;
        protected override short constVersion => 1;
        protected override bool inRadians => false;

        [Category(dynaCategoryName)]
        public uint InterestType { get; set; }

        public DynaInterestPointer(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Interest_Pointer, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                InterestType = reader.ReadUInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(InterestType);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void Draw(SharpRenderer renderer) => renderer.DrawPyramid(world, isSelected);

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}