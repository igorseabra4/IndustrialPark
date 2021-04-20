using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectVent : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "game_object:Vent";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID VentType_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat1C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat20 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat24 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat28 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VentDistance { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat30 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LaunchSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownInt38 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat3C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat40 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat44 { get; set; }

        public DynaGObjectVent(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__Vent, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            VentType_AssetID = reader.ReadUInt32();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _yaw = reader.ReadSingle();
            _pitch = reader.ReadSingle();
            _roll = reader.ReadSingle();
            UnknownFloat1C = reader.ReadSingle();
            UnknownFloat20 = reader.ReadSingle();
            UnknownFloat24 = reader.ReadSingle();
            UnknownFloat28 = reader.ReadSingle();
            VentDistance = reader.ReadSingle();
            UnknownFloat30 = reader.ReadSingle();
            LaunchSpeed = reader.ReadSingle();
            UnknownInt38 = reader.ReadSingle();
            UnknownFloat3C = reader.ReadSingle();
            UnknownFloat40 = reader.ReadSingle();
            UnknownFloat44 = reader.ReadSingle();

            CreateTransformMatrix();
            renderableAssets.Add(this);
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(VentType_AssetID);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(UnknownFloat1C);
            writer.Write(UnknownFloat20);
            writer.Write(UnknownFloat24);
            writer.Write(UnknownFloat28);
            writer.Write(VentDistance);
            writer.Write(UnknownFloat30);
            writer.Write(LaunchSpeed);
            writer.Write(UnknownInt38);
            writer.Write(UnknownFloat3C);
            writer.Write(UnknownFloat40);
            writer.Write(UnknownFloat44);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => VentType_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            Verify(VentType_AssetID, ref result);
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