using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class AssetDUPC : BaseAsset, IRenderableAsset, IClickableAsset, IRotatableAsset, IScalableAsset
    {
        private const string categoryName = "Duplicator";

        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_08 { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_0A { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_0C { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort UnknownShort_0E { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_10 { get; set; }
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
        public AssetID NavMesh1_AssetID { get; set; }
        [Category(categoryName)]
        public int UnknownInt_30 { get; set; }
        [Category(categoryName)]
        public AssetID NavMesh2_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID Settings_AssetID { get; set; }

        [Category(categoryName), TypeConverter(typeof(ExpandableObjectConverter))]
        public AssetVIL VIL { get; set; }

        public AssetDUPC(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityEndPosition;

            UnknownShort_08 = reader.ReadUInt16();
            UnknownShort_0A = reader.ReadUInt16();
            UnknownShort_0C = reader.ReadUInt16();
            UnknownShort_0E = reader.ReadUInt16();
            UnknownFloat_10 = reader.ReadSingle();
            UnknownInt_14 = reader.ReadInt32();
            UnknownInt_18 = reader.ReadInt32();
            UnknownInt_1C = reader.ReadInt32();
            UnknownInt_20 = reader.ReadInt32();
            UnknownInt_24 = reader.ReadInt32();
            UnknownInt_28 = reader.ReadInt32();
            NavMesh1_AssetID = reader.ReadUInt32();
            UnknownInt_30 = reader.ReadInt32();
            VIL = new AssetVIL(reader);
            NavMesh2_AssetID = reader.ReadUInt32();
            Settings_AssetID = reader.ReadUInt32();

            CreateTransformMatrix();
            renderableAssets.Add(this);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(UnknownShort_08);
            writer.Write(UnknownShort_0A);
            writer.Write(UnknownShort_0C);
            writer.Write(UnknownShort_0E);
            writer.Write(UnknownFloat_10);
            writer.Write(UnknownInt_14);
            writer.Write(UnknownInt_18);
            writer.Write(UnknownInt_1C);
            writer.Write(UnknownInt_20);
            writer.Write(UnknownInt_24);
            writer.Write(UnknownInt_28);
            writer.Write(NavMesh1_AssetID);
            writer.Write(UnknownInt_30);

            writer.Write(AssetID);
            writer.Write((byte)ObjectAssetType.NPC);
            writer.Write((byte)_links.Length);
            writer.Write(VIL.Serialize(game, platform).Skip(6).ToArray());

            writer.Write(NavMesh2_AssetID);
            writer.Write(Settings_AssetID);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => NavMesh1_AssetID == assetID || NavMesh2_AssetID == assetID ||
            Settings_AssetID == assetID || VIL.HasReference(assetID) || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            Verify(NavMesh1_AssetID, ref result);
            Verify(NavMesh2_AssetID, ref result);
            Verify(Settings_AssetID, ref result);

            VIL.Verify(ref result);

            base.Verify(ref result);
        }

        public void CreateTransformMatrix() => VIL.CreateTransformMatrix();
        public float GetDistanceFrom(Vector3 position) => VIL.GetDistanceFrom(position);
        public bool ShouldDraw(SharpRenderer renderer) => VIL.ShouldDraw(renderer);
        public void Draw(SharpRenderer renderer) => VIL.Draw(renderer);
        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray) => VIL.GetIntersectionPosition(renderer, ray);
        public BoundingBox GetBoundingBox() => VIL.GetBoundingBox();        
        public bool SpecialBlendMode => VIL.SpecialBlendMode;
        public float PositionX { get => VIL.PositionX; set => VIL.PositionX = value; }
        public float PositionY { get => VIL.PositionY; set => VIL.PositionY = value; }
        public float PositionZ { get => VIL.PositionZ; set => VIL.PositionZ = value; }
        public float Yaw { get => VIL.Yaw; set => VIL.Yaw = value; }
        public float Pitch { get => VIL.Pitch; set => VIL.Pitch = value; }
        public float Roll { get => VIL.Roll; set => VIL.Roll = value; }
        public float ScaleX { get => VIL.ScaleX; set => VIL.ScaleX = value; }
        public float ScaleY { get => VIL.ScaleY; set => VIL.ScaleY = value; }
        public float ScaleZ { get => VIL.ScaleZ; set => VIL.ScaleZ = value; }
    }
}