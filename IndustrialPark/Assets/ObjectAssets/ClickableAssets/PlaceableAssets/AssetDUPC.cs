using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetDUPC : BaseAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        public override string AssetInfo => NPC.AssetInfo;

        private const string categoryName = "Duplicator";
        private const string categoryName2 = "Duplicator NPC";

        [Category(categoryName)]
        public short InitialSpawn { get; set; }
        [Category(categoryName)]
        public short MaximumInGame { get; set; }
        [Category(categoryName)]
        public short MaximumToSpawn { get; set; }
        [Category(categoryName)]
        public AssetSingle SpawnRate { get; set; }
        [Category(categoryName)]
        public int UnknownInt_14 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_18 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_1C { get; set; }
        [Category(categoryName)]
        public int UnknownInt_20 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_24 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_28 { get; set; }
        [Category(categoryName)]
        public AssetID NavMesh1 { get; set; }
        [Category(categoryName)]
        public int UnknownInt_30 { get; set; }

        [Category(categoryName2), TypeConverter(typeof(ExpandableObjectConverter))]
        public AssetVIL NPC { get; set; }

        public AssetDUPC(string assetName, Vector3 position) : base(assetName, AssetType.Duplicator, BaseAssetType.Duplicator)
        {
            InitialSpawn = 1;
            MaximumInGame = 1;
            MaximumToSpawn = 1;
            SpawnRate = 1f;
            NPC = new AssetVIL(assetName, position, AssetTemplate.NPC, 0);
            renderableAssets.Remove(NPC);
            AddToRenderableAssets(this);
        }

        public AssetDUPC(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                InitialSpawn = reader.ReadInt16();
                MaximumInGame = reader.ReadInt16();
                MaximumToSpawn = reader.ReadInt16();
                reader.BaseStream.Position += 2;
                SpawnRate = reader.ReadSingle();
                UnknownInt_14 = reader.ReadInt32();
                UnknownInt_18 = reader.ReadInt32();
                UnknownInt_1C = reader.ReadInt32();
                UnknownInt_20 = reader.ReadInt32();
                UnknownInt_24 = reader.ReadInt32();
                UnknownInt_28 = reader.ReadInt32();
                NavMesh1 = reader.ReadUInt32();
                UnknownInt_30 = reader.ReadInt32();
                NPC = new AssetVIL(reader, game);
            }

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(InitialSpawn);
            writer.Write(MaximumInGame);
            writer.Write(MaximumToSpawn);
            writer.Write((short)0);
            writer.Write(SpawnRate);
            writer.Write(UnknownInt_14);
            writer.Write(UnknownInt_18);
            writer.Write(UnknownInt_1C);
            writer.Write(UnknownInt_20);
            writer.Write(UnknownInt_24);
            writer.Write(UnknownInt_28);
            writer.Write(NavMesh1);
            writer.Write(UnknownInt_30);

            var npcStart = writer.BaseStream.Position;
            NPC.Serialize(writer);
            var npcEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = npcStart;
            writer.Write(assetID);
            writer.Write((byte)BaseAssetType.NPC);
            writer.Write((byte)_links.Length);
            writer.BaseStream.Position = npcEnd;

            SerializeLinks(writer);
        }

        public void CreateTransformMatrix() => NPC.CreateTransformMatrix();
        public float GetDistanceFrom(Vector3 position) => NPC.GetDistanceFrom(position);
        public bool ShouldDraw(SharpRenderer renderer)
        {
            NPC.isInvisible = isInvisible;
            NPC.isSelected = isSelected;
            return NPC.ShouldDraw(renderer);
        }
        public void Draw(SharpRenderer renderer)
        {
            NPC.isInvisible = isInvisible;
            NPC.isSelected = isSelected;
            NPC.Draw(renderer);
        }
        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray) => NPC.GetIntersectionPosition(renderer, ray);
        public BoundingBox GetBoundingBox() => NPC.GetBoundingBox();
        [Browsable(false)]
        public bool SpecialBlendMode => NPC.SpecialBlendMode;

        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle PositionX { get => NPC.PositionX; set => NPC.PositionX = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle PositionY { get => NPC.PositionY; set => NPC.PositionY = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle PositionZ { get => NPC.PositionZ; set => NPC.PositionZ = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle Yaw { get => NPC.Yaw; set => NPC.Yaw = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle Pitch { get => NPC.Pitch; set => NPC.Pitch = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle Roll { get => NPC.Roll; set => NPC.Roll = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle ScaleX { get => NPC.ScaleX; set => NPC.ScaleX = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle ScaleY { get => NPC.ScaleY; set => NPC.ScaleY = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle ScaleZ { get => NPC.ScaleZ; set => NPC.ScaleZ = value; }
    }
}