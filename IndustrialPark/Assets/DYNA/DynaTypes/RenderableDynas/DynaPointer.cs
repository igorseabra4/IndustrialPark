using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public class DynaPointer : RenderableDynaBase, IRotatableAsset
    {
        private const string dynaCategoryName1 = "Placement";

        [Category(dynaCategoryName1)]
        public AssetSingle Yaw { get; set; }
        [Category(dynaCategoryName1)]
        public AssetSingle Pitch { get; set; }
        [Category(dynaCategoryName1)]
        public AssetSingle Roll { get; set; }

        public override void CalculateTransformation(SharpRenderer renderer)
        {
            world = Matrix.RotationYawPitchRoll(
                MathUtil.DegreesToRadians(Yaw),
                MathUtil.DegreesToRadians(Pitch),
                MathUtil.DegreesToRadians(Roll)) * Matrix.Translation(_position);
            boundingBox = CreateBoundingBox(vertexSource(renderer), world);
        }

        public override string TypeString => "pointer";

        protected override short constVersion => 1;

        public DynaPointer(string assetName, Vector3 position) : base(assetName, DynaType.pointer, position)
        {
        }

        public DynaPointer(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.pointer, reader)
        {
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Yaw = reader.ReadSingle();
            Pitch = reader.ReadSingle();
            Roll = reader.ReadSingle();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(Yaw);
            writer.Write(Pitch);
            writer.Write(Roll);
        }

        protected override List<Vector3> vertexSource(SharpRenderer renderer) => renderer.cubeVertices;

        protected override List<RenderWareFile.Triangle> triangleSource(SharpRenderer renderer) => renderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
        }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}