using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaGObjectInPickup : RenderableDynaBase
    {
        private const string dynaCategoryName = "game_object:IN_pickup";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo
        {
            get
            {
                if (AssetTPIK.tpikEntries.ContainsKey(PickupHash))
                    return $"{dynaCategoryName} {HexUIntTypeConverter.StringFromAssetID(AssetTPIK.tpikEntries[PickupHash].Model)}";
                return $"{dynaCategoryName} {HexUIntTypeConverter.StringFromAssetID(PickupHash)}";
            }
        }

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID PickupHash { get; set; }

        public static bool dontRender = false;
        protected override bool DontRender => dontRender;

        public DynaGObjectInPickup(string assetName, Vector3 position, AssetTemplate template) : base(assetName, DynaType.game_object__IN_Pickup, position)
        {
            switch (template)
            {
                case AssetTemplate.Health_10:
                    PickupHash = 0x66BFB77B;
                    break;
                case AssetTemplate.Health_25:
                    PickupHash = 0x66BFB803;
                    break;
                case AssetTemplate.Health_50:
                    PickupHash = 0x66BFB987;
                    break;
                case AssetTemplate.Power_25:
                    PickupHash = 0xD4F7394A;
                    break;
                case AssetTemplate.Power_50:
                    PickupHash = 0xD4F73ACE;
                    break;
                case AssetTemplate.Bonus:
                    PickupHash = 0x91338C47;
                    break;
            }

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        public DynaGObjectInPickup(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__IN_Pickup, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                PickupHash = reader.ReadUInt32();
                _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                CreateTransformMatrix();
                AddToRenderableAssets(this);
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(PickupHash);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(PositionX, PositionY, PositionZ);

            if (AssetTPIK.tpikEntries.ContainsKey(PickupHash))
            {
                var model = GetFromRenderingDictionary(AssetTPIK.tpikEntries[PickupHash].Model);
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

        public override bool ShouldDraw(SharpRenderer renderer)
        {
            if (AssetMODL.renderBasedOnLodt)
            {
                if (AssetTPIK.tpikEntries.ContainsKey(PickupHash))
                {
                    var tpikEntry = AssetTPIK.tpikEntries[PickupHash].Model;
                    if (GetDistanceFrom(renderer.Camera.Position) < AssetLODT.MaxDistanceTo(tpikEntry))
                        return renderer.frustum.Intersects(ref boundingBox);
                }

                return base.ShouldDraw(renderer);
            }

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public override void Draw(SharpRenderer renderer)
        {
            bool drew = false;
            if (AssetTPIK.tpikEntries.ContainsKey(PickupHash))
            {
                var tpikEntry = AssetTPIK.tpikEntries[PickupHash];
                if (renderingDictionary.ContainsKey(tpikEntry.Model))
                {
                    renderingDictionary[tpikEntry.Model].Draw(renderer, world, Vector4.One, Vector3.Zero, isSelected);
                    drew = true;
                }
                if (renderingDictionary.ContainsKey(tpikEntry.PulseModel))
                {
                    var color = new Vector4(tpikEntry.ColorRed, tpikEntry.ColorGreen, tpikEntry.ColorBlue, 1f);
                    renderingDictionary[tpikEntry.PulseModel].Draw(renderer, world, color, Vector3.Zero, isSelected);
                    drew = true;
                }
            }
            if (!drew)
                renderer.DrawCube(world, isSelected);
        }
    }
}