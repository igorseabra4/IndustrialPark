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

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public FlagBitmask CameraPresetFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID Checkpoint_AssetID { get; set; }

        public DynaCameraPreset(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.camera__preset, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                CameraPresetFlags.FlagValueInt = reader.ReadUInt32();
                Checkpoint_AssetID = reader.ReadUInt32();

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
                writer.Write(CameraPresetFlags.FlagValueInt);
                writer.Write(Checkpoint_AssetID);
                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(_yaw);
                writer.Write(_pitch);
                writer.Write(_roll);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => Checkpoint_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(Checkpoint_AssetID, ref result);

            base.Verify(ref result);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void Draw(SharpRenderer renderer) => renderer.DrawPyramid(world, isSelected);
    }
}