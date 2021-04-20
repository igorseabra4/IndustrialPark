using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public enum EPickupFlags : ushort
    {
        None = 0,
        ReappearAfterCollect = 1,
        InitiallyVisible = 2,
        Both = 3
    }

    public class AssetPKUP : EntityAsset
    {
        private const string categoryName = "Pickup";

        [Category(categoryName)]
        public AssetByte Shape { get => TypeFlag; set => TypeFlag = value; }

        [Category(categoryName)]
        public AssetID PickReferenceID { get; set; }

        [Category(categoryName)]
        public EPickupFlags PickupFlags { get; set; }

        [Category(categoryName)]
        public short PickupValue { get; set; }

        public AssetPKUP(string assetName, Vector3 position) : base(assetName, AssetType.PKUP, BaseAssetType.Pickup, position)
        {
            if (game == Game.BFBB)
                Model_AssetID = pkupsMinfName;
            else if (game == Game.Incredibles)
                Model_AssetID = 0x94E25463;
        }

        public AssetPKUP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityHeaderEndPosition;

            PickReferenceID = reader.ReadUInt32();
            PickupFlags = (EPickupFlags)reader.ReadUInt16();
            PickupValue = reader.ReadInt16();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));

            writer.Write(PickReferenceID);
            writer.Write((ushort)PickupFlags);
            writer.Write(PickupValue);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) => PickReferenceID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (PickReferenceID == 0)
                result.Add("PKUP with PickReferenceID set to 0");
        }

        protected override void CreateBoundingBox()
        {
            if (AssetPICK.pickEntries.ContainsKey(PickReferenceID))
            {
                var model = GetFromRenderingDictionary(AssetPICK.pickEntries[PickReferenceID]);
                if (model != null)
                {
                    var vertexList = model.vertexListG;

                    vertices = new Vector3[vertexList.Count];
                    for (int i = 0; i < vertexList.Count; i++)
                        vertices[i] = (Vector3)Vector3.Transform(vertexList[i], world);
                    boundingBox = BoundingBox.FromPoints(vertices);

                    triangles = model.triangleList.ToArray();
                    return;
                }
            }

            vertices = new Vector3[SharpRenderer.cubeVertices.Count];
            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i] * 0.5f, world);
            boundingBox = BoundingBox.FromPoints(vertices);

            triangles = null;
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (AssetPICK.pickEntries.ContainsKey(PickReferenceID))
                if (renderingDictionary.ContainsKey(AssetPICK.pickEntries[PickReferenceID]))
                {
                    renderingDictionary[AssetPICK.pickEntries[PickReferenceID]].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * _color : _color, UvAnimOffset);
                    return;
                }

            renderer.DrawCube(LocalWorld(), isSelected);
        }

        [Browsable(false)]
        public override bool SpecialBlendMode =>
            !AssetPICK.pickEntries.ContainsKey(PickReferenceID) || !renderingDictionary.ContainsKey(AssetPICK.pickEntries[PickReferenceID]) || renderingDictionary[AssetPICK.pickEntries[PickReferenceID]].SpecialBlendMode;
    }
}