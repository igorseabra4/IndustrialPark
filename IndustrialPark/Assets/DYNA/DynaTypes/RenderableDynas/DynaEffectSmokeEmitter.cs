using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaEffectSmokeEmitter : RenderableDynaBase
    {
        private const string dynaCategoryName = "effect:smoke_emitter";

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public int UnknownInt_00 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_04 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_14 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_18 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_1C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_20 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_24 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_28 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Texture_AssetID { get; set; }
        [Category(dynaCategoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_30 { get; set; }
        [Category(dynaCategoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_32 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_34 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_38 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_3C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_40 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_44 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_48 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_4C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_50 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_54 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_58 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_5C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_60 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_64 { get; set; }
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
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            UnknownInt_00 = reader.ReadInt32();
            UnknownInt_04 = reader.ReadInt32();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            UnknownFloat_14 = reader.ReadSingle();
            UnknownFloat_18 = reader.ReadSingle();
            UnknownFloat_1C = reader.ReadSingle();
            UnknownFloat_20 = reader.ReadSingle();
            UnknownFloat_24 = reader.ReadSingle();
            UnknownFloat_28 = reader.ReadSingle();
            Texture_AssetID = reader.ReadUInt32();
            UnknownShort_30 = reader.ReadUInt16();
            UnknownShort_32 = reader.ReadUInt16();
            UnknownFloat_34 = reader.ReadSingle();
            UnknownFloat_38 = reader.ReadSingle();
            UnknownFloat_3C = reader.ReadSingle();
            UnknownFloat_40 = reader.ReadSingle();
            UnknownFloat_44 = reader.ReadSingle();
            UnknownFloat_48 = reader.ReadSingle();
            UnknownFloat_4C = reader.ReadSingle();
            UnknownFloat_50 = reader.ReadSingle();
            UnknownFloat_54 = reader.ReadSingle();
            UnknownFloat_58 = reader.ReadSingle();
            UnknownFloat_5C = reader.ReadSingle();
            UnknownFloat_60 = reader.ReadSingle();
            UnknownFloat_64 = reader.ReadSingle();
            UnknownShort_68 = reader.ReadInt16();
            UnknownShort_6A = reader.ReadInt16();
            UnknownShort_6C = reader.ReadInt16();
            UnknownShort_6E = reader.ReadInt16();

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(UnknownInt_00);
            writer.Write(UnknownInt_04);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(UnknownFloat_14);
            writer.Write(UnknownFloat_18);
            writer.Write(UnknownFloat_1C);
            writer.Write(UnknownFloat_20);
            writer.Write(UnknownFloat_24);
            writer.Write(UnknownFloat_28);
            writer.Write(Texture_AssetID);
            writer.Write(UnknownShort_30);
            writer.Write(UnknownShort_32);
            writer.Write(UnknownFloat_34);
            writer.Write(UnknownFloat_38);
            writer.Write(UnknownFloat_3C);
            writer.Write(UnknownFloat_40);
            writer.Write(UnknownFloat_44);
            writer.Write(UnknownFloat_48);
            writer.Write(UnknownFloat_4C);
            writer.Write(UnknownFloat_50);
            writer.Write(UnknownFloat_54);
            writer.Write(UnknownFloat_58);
            writer.Write(UnknownFloat_5C);
            writer.Write(UnknownFloat_60);
            writer.Write(UnknownFloat_64);
            writer.Write(UnknownShort_68);
            writer.Write(UnknownShort_6A);
            writer.Write(UnknownShort_6C);
            writer.Write(UnknownShort_6E);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (Texture_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Texture_AssetID, ref result);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
        }
    }
}