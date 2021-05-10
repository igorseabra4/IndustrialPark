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
        protected override short constVersion => 1;

        [Category("game_object:IN_pickup")]
        public AssetID PickupHash { get; set; }

        public DynaGObjectInPickup(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__IN_Pickup, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            PickupHash = reader.ReadUInt32();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(PickupHash);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => PickupHash == assetID;

        protected override List<Vector3> vertexSource => null;

        protected override List<Triangle> triangleSource => null;

        public override void CreateTransformMatrix()
        {
            world = Matrix.Translation(PositionX, PositionY, PositionZ);

            if (AssetTPIK.tpikEntries.ContainsKey(PickupHash))
            {
                var model = GetFromRenderingDictionary(AssetTPIK.tpikEntries[PickupHash].Model_AssetID);
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
                    var tpikEntry = AssetTPIK.tpikEntries[PickupHash].Model_AssetID;
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
                if (renderingDictionary.ContainsKey(tpikEntry.Model_AssetID))
                {
                    renderingDictionary[tpikEntry.Model_AssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor : Vector4.One, Vector3.Zero);
                    drew = true;
                }
                if (renderingDictionary.ContainsKey(tpikEntry.RingModel_AssetID))
                {
                    var color = new Vector4(tpikEntry.RingColorR, tpikEntry.RingColorG, tpikEntry.RingColorB, 1f);
                    renderingDictionary[tpikEntry.RingModel_AssetID].Draw(renderer, world, isSelected ? renderer.selectedObjectColor * color : color, Vector3.Zero);
                    drew = true;
                }
            }
            if (!drew)
                renderer.DrawCube(world, isSelected);
        }
    }
}