using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaCameraPreset : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "camera:preset";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Checkpoint);

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public FlagBitmask CameraPresetFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID Checkpoint { get; set; }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;

        public DynaCameraPreset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.camera__preset, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                CameraPresetFlags.FlagValueInt = reader.ReadUInt32();
                Checkpoint = reader.ReadUInt32();

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(CameraPresetFlags.FlagValueInt);
            writer.Write(Checkpoint);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void Draw(SharpRenderer renderer) => renderer.DrawPyramid(world, isSelected);
    }
}