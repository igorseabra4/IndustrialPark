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
        public override string AssetInfo => VIL.AssetInfo;

        private const string categoryName = "Duplicator";
        private const string categoryName2 = "Duplicator VIL";

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
        public AssetVIL VIL { get; set; }

        public AssetDUPC(string assetName, Vector3 position) : base(assetName, AssetType.Duplicator, BaseAssetType.Duplicator)
        {
            VIL = new AssetVIL(assetName, position, AssetTemplate.VIL, 0);
            renderableAssets.Remove(VIL);
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
                VIL = new AssetVIL(reader);
            }

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));

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

                writer.Write(assetID);
                writer.Write((byte)BaseAssetType.NPC);
                writer.Write((byte)_links.Length);
                writer.Write(VIL.Serialize(game, endianness).Skip(6).ToArray());

                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(NavMesh1, ref result);

            VIL.Verify(ref result);

            base.Verify(ref result);
        }

        public void CreateTransformMatrix() => VIL.CreateTransformMatrix();
        public float GetDistanceFrom(Vector3 position) => VIL.GetDistanceFrom(position);
        public bool ShouldDraw(SharpRenderer renderer)
        {
            VIL.isInvisible = isInvisible;
            VIL.isSelected = isSelected;
            return VIL.ShouldDraw(renderer);
        }
        public void Draw(SharpRenderer renderer)
        {
            VIL.isInvisible = isInvisible;
            VIL.isSelected = isSelected;
            VIL.Draw(renderer);
        }
        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray) => VIL.GetIntersectionPosition(renderer, ray);
        public BoundingBox GetBoundingBox() => VIL.GetBoundingBox();
        [Browsable(false)]
        public bool SpecialBlendMode => VIL.SpecialBlendMode;

        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle PositionX { get => VIL.PositionX; set => VIL.PositionX = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle PositionY { get => VIL.PositionY; set => VIL.PositionY = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle PositionZ { get => VIL.PositionZ; set => VIL.PositionZ = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle Yaw { get => VIL.Yaw; set => VIL.Yaw = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle Pitch { get => VIL.Pitch; set => VIL.Pitch = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle Roll { get => VIL.Roll; set => VIL.Roll = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle ScaleX { get => VIL.ScaleX; set => VIL.ScaleX = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle ScaleY { get => VIL.ScaleY; set => VIL.ScaleY = value; }
        [Category(categoryName2), ReadOnly(true)]
        public AssetSingle ScaleZ { get => VIL.ScaleZ; set => VIL.ScaleZ = value; }
    }
}