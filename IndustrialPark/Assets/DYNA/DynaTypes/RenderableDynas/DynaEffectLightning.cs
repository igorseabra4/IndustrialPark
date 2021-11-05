using AssetEditorColors;
using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaEffectLightning : RenderableDynaBase
    {
        private const string dynaCategoryName = "effect:Lightning";

        protected override short constVersion => 2;

        public override bool HasReference(uint assetID) =>
            LightningTexture_AssetID == assetID || GlowTexture_AssetID == assetID || SIMP1_AssetID == assetID || SIMP2_AssetID == assetID;
        
        public override void Verify(ref List<string> result)
        {
            Verify(LightningTexture_AssetID, ref result);
            Verify(GlowTexture_AssetID, ref result);
            Verify(SIMP1_AssetID, ref result);
            Verify(SIMP2_AssetID, ref result);
        }

        protected Vector3 _positionEnd;
        [Category(dynaCategoryName)]
        public AssetSingle PositionEndX
        {
            get => _positionEnd.X;
            set { _positionEnd.X = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public AssetSingle PositionEndY
        {
            get => _positionEnd.Y;
            set { _positionEnd.Y = value; CreateTransformMatrix(); }
        }
        [Category(dynaCategoryName)]
        public AssetSingle PositionEndZ
        {
            get => _positionEnd.Z;
            set { _positionEnd.Z = value; CreateTransformMatrix(); }
        }

        [Category(dynaCategoryName)]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Width { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BranchSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetID LightningTexture_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID GlowTexture_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public int Damage { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle KnockbackSpeed { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundGroupID_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundHit1_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundHit2_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SIMP1_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SIMP2_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public bool DamagePlayer { get; set; }

        public DynaEffectLightning(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__Lightning, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                _positionEnd = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                Color = reader.ReadColor();
                Width = reader.ReadSingle();
                BranchSpeed = reader.ReadSingle();
                LightningTexture_AssetID = reader.ReadUInt32();
                GlowTexture_AssetID = reader.ReadUInt32();
                Damage = reader.ReadInt32();
                KnockbackSpeed = reader.ReadSingle();
                SoundGroupID_AssetID = reader.ReadUInt32();
                SoundHit1_AssetID = reader.ReadUInt32();
                SoundHit2_AssetID = reader.ReadUInt32();
                SIMP1_AssetID = reader.ReadUInt32();
                SIMP2_AssetID = reader.ReadUInt32();
                DamagePlayer = reader.ReadInt32Bool();

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(_position.X);
                writer.Write(_position.Y);
                writer.Write(_position.Z);
                writer.Write(_positionEnd.X);
                writer.Write(_positionEnd.Y);
                writer.Write(_positionEnd.Z);
                writer.Write(Color);
                writer.Write(Width);
                writer.Write(BranchSpeed);
                writer.Write(LightningTexture_AssetID);
                writer.Write(GlowTexture_AssetID);
                writer.Write(Damage);
                writer.Write(KnockbackSpeed);
                writer.Write(SoundGroupID_AssetID);
                writer.Write(SoundHit1_AssetID);
                writer.Write(SoundHit2_AssetID);
                writer.Write(SIMP1_AssetID);
                writer.Write(SIMP2_AssetID);
                writer.Write(DamagePlayer ? 1 : 0);

                return writer.ToArray();
            }
        }

        private Matrix world2;

        public override void CreateTransformMatrix()
        {
            world2 = Matrix.Translation(_positionEnd);
            base.CreateTransformMatrix();
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
            renderer.DrawCube(world2, isSelected);
        }
    }
}