using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaEffectParticleGenerator : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "effect:particle generator";

        protected override short constVersion => 1;

        //0x28	attach_data attach
        //0x2C	motion_data motion
        //0x30	volume_data volume

        [Category(dynaCategoryName)]
        public FlagBitmask ParticleGeneratorFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryName)]
        public FlagBitmask AttachFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryName)]
        public FlagBitmask MotionFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryName)]
        public FlagBitmask VolumeFlags { get; set; } = ByteFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetSingle Rate { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Texture_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AttachType { get; set; } 
        [Category(dynaCategoryName)]
        public AssetByte MotionType { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte VolumeType { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte SystemType { get; set; }

        public DynaEffectParticleGenerator(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Vent, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                ParticleGeneratorFlags.FlagValueByte = reader.ReadByte();
                AttachFlags.FlagValueByte = reader.ReadByte();
                MotionFlags.FlagValueByte = reader.ReadByte();
                VolumeFlags.FlagValueByte = reader.ReadByte();
                Rate = reader.ReadSingle();
                Texture_AssetID = reader.ReadUInt32();
                AttachType = reader.ReadByte();
                MotionType = reader.ReadByte();
                VolumeType = reader.ReadByte();
                SystemType = reader.ReadByte();
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
                writer.Write(ParticleGeneratorFlags.FlagValueByte);
                writer.Write(AttachFlags.FlagValueByte);
                writer.Write(MotionFlags.FlagValueByte);
                writer.Write(VolumeFlags.FlagValueByte);
                writer.Write(Rate);
                writer.Write(Texture_AssetID);
                writer.Write(AttachType);
                writer.Write(MotionType);
                writer.Write(VolumeType);
                writer.Write(SystemType);
                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(_yaw);
                writer.Write(_pitch);
                writer.Write(_roll);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => Texture_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            Verify(Texture_AssetID, ref result);
            base.Verify(ref result);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationX(-MathUtil.PiOverTwo)
                * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                * Matrix.Translation(PositionX, PositionY, PositionZ);

            base.CreateBoundingBox();
        }

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
        }
    }
}