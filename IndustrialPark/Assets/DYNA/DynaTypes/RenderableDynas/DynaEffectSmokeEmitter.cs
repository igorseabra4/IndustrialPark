using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaEffectSmokeEmitter : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "effect:smoke_emitter";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public FlagBitmask SmokeEmitterFlags { get; set; } = IntFlagsDescriptor();
        [Category(dynaCategoryName)]
        public AssetID AttachTo { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Texture { get; set; }
        [Category(dynaCategoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort TextureRows { get; set; }
        [Category(dynaCategoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort TextureColumns { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Rate { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LifeMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LifeMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SizeMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SizeMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelMin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Growth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelDirX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelDirY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelDirZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelDirVary { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Wind { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_68 { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_6A { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_6C { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_6E { get; set; }

        public DynaEffectSmokeEmitter(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__smoke_emitter, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                SmokeEmitterFlags.FlagValueInt = reader.ReadUInt32();
                AttachTo = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _yaw = reader.ReadSingle();
                _pitch = reader.ReadSingle();
                _roll = reader.ReadSingle();
                ScaleX = reader.ReadSingle();
                ScaleY = reader.ReadSingle();
                ScaleZ = reader.ReadSingle();
                Texture = reader.ReadUInt32();
                TextureRows = reader.ReadUInt16();
                TextureColumns = reader.ReadUInt16();
                Rate = reader.ReadSingle();
                LifeMin = reader.ReadSingle();
                LifeMax = reader.ReadSingle();
                SizeMin = reader.ReadSingle();
                SizeMax = reader.ReadSingle();
                VelMin = reader.ReadSingle();
                VelMax = reader.ReadSingle();
                Growth = reader.ReadSingle();
                VelDirX = reader.ReadSingle();
                VelDirY = reader.ReadSingle();
                VelDirZ = reader.ReadSingle();
                VelDirVary = reader.ReadSingle();
                Wind = reader.ReadSingle();
                UnknownShort_68 = reader.ReadInt16();
                UnknownShort_6A = reader.ReadInt16();
                UnknownShort_6C = reader.ReadInt16();
                UnknownShort_6E = reader.ReadInt16();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SmokeEmitterFlags.FlagValueInt);
                writer.Write(AttachTo);
                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(_yaw);
                writer.Write(_pitch);
                writer.Write(_roll);
                writer.Write(ScaleX);
                writer.Write(ScaleY);
                writer.Write(ScaleZ);
                writer.Write(Texture);
                writer.Write(TextureRows);
                writer.Write(TextureColumns);
                writer.Write(Rate);
                writer.Write(LifeMin);
                writer.Write(LifeMax);
                writer.Write(SizeMin);
                writer.Write(SizeMax);
                writer.Write(VelMin);
                writer.Write(VelMax);
                writer.Write(Growth);
                writer.Write(VelDirX);
                writer.Write(VelDirY);
                writer.Write(VelDirZ);
                writer.Write(VelDirVary);
                writer.Write(Wind);
                writer.Write(UnknownShort_68);
                writer.Write(UnknownShort_6A);
                writer.Write(UnknownShort_6C);
                writer.Write(UnknownShort_6E);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Texture, ref result);
            Verify(AttachTo, ref result);

            base.Verify(ref result);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.pyramidVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.pyramidTriangles;

        public override void Draw(SharpRenderer renderer) => renderer.DrawPyramid(world, isSelected);

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;
    }
}