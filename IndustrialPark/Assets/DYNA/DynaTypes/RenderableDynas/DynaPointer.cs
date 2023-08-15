using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaPointer : RenderableDynaBase, IRotatableAsset
    {
        private const string dynaCategoryName1 = "DYNA Placement";

        protected float _yaw;
        [Category(dynaCategoryName1)]
        public AssetSingle Yaw
        {
            get => _yaw;
            set { _yaw = value; CreateTransformMatrix(); }
        }

        protected float _pitch;
        [Category(dynaCategoryName1)]
        public AssetSingle Pitch
        {
            get => _pitch;
            set { _pitch = value; CreateTransformMatrix(); }
        }

        protected float _roll;
        [Category(dynaCategoryName1)]
        public AssetSingle Roll
        {
            get => _roll;
            set { _roll = value; CreateTransformMatrix(); }
        }

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationYawPitchRoll(
                MathUtil.DegreesToRadians(_yaw),
                MathUtil.DegreesToRadians(_pitch),
                MathUtil.DegreesToRadians(_roll)) * Matrix.Translation(_position);
            CreateBoundingBox();
        }

        public override string TypeString => "pointer";

        protected override short constVersion => 1;

        public DynaPointer(string assetName, Vector3 position) : base(assetName, DynaType.pointer, position)
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

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);


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