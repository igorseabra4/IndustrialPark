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
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(VentType);

        protected override short constVersion => 1;

        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID VentType { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageBoxLowerLeftX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageBoxLowerLeftY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageBoxLowerLeftZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageBoxUpperRightX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageBoxUpperRightY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageBoxUpperRightZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BoulderInfluence { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask VentFlags { get; set; } = IntFlagsDescriptor("Break boulders", "Automatic", "Damage spongeball");
        [Category(dynaCategoryName)]
        public AssetSingle IdleTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WarnTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DamageTime { get; set; }
        [Category(dynaCategoryName)]
        public int DamagePoints { get; set; }

        public DynaGObjectVent(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Vent, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                VentType = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                DamageBoxLowerLeftX = reader.ReadSingle();
                DamageBoxLowerLeftY = reader.ReadSingle();
                DamageBoxLowerLeftZ = reader.ReadSingle();
                DamageBoxUpperRightX = reader.ReadSingle();
                DamageBoxUpperRightY = reader.ReadSingle();
                DamageBoxUpperRightZ = reader.ReadSingle();
                BoulderInfluence = reader.ReadSingle();
                VentFlags.FlagValueInt = reader.ReadUInt32();
                IdleTime = reader.ReadSingle();
                WarnTime = reader.ReadSingle();
                DamageTime = reader.ReadSingle();
                if (game >= Game.ROTU)
                    DamagePoints = reader.ReadInt32();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(VentType);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(DamageBoxLowerLeftX);
            writer.Write(DamageBoxLowerLeftY);
            writer.Write(DamageBoxLowerLeftZ);
            writer.Write(DamageBoxUpperRightX);
            writer.Write(DamageBoxUpperRightY);
            writer.Write(DamageBoxUpperRightZ);
            writer.Write(BoulderInfluence);
            writer.Write(VentFlags.FlagValueInt);
            writer.Write(IdleTime);
            writer.Write(WarnTime);
            writer.Write(DamageTime);
            if (game >= Game.ROTU)
                writer.Write(DamagePoints);

        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.ROTU)
                dt.RemoveProperty("DamagePoints");
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

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}