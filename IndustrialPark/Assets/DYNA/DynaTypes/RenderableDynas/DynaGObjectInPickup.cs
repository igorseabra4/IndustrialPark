using HipHopFile;
using IndustrialPark.Archives;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark.Assets
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
        }

        public DynaGObjectInPickup(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.game_object__IN_Pickup, reader)
        {
            PickupHash = reader.ReadUInt32();
            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(PickupHash);
            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
        }

        protected override List<Vector3> vertexSource(SharpRenderer renderer) => renderer.cubeVertices;

        protected override List<RenderWareFile.Triangle> triangleSource(SharpRenderer renderer) => renderer.cubeTriangles;

        public override void CalculateTransformation(SharpRenderer renderer)
        {
            world = Matrix.Translation(_position);

            var model = AssetTPIK.tpikEntries.ContainsKey(PickupHash) ? renderer.Assets.GetModelFromRenderingDictionary(AssetTPIK.tpikEntries[PickupHash].Model) : null;
            boundingBox = CreateBoundingBox(model?.vertexListG ?? renderer.cubeVertices, world, model != null ? 1f : 0.5f);
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
                if (renderer.Assets.GetFromRenderingDictionary(tpikEntry.Model) is IAssetWithModel value)
                {
                    value.Draw(renderer, world, Vector4.One, Vector3.Zero, isSelected);
                    drew = true;
                }
                if (renderer.Assets.GetFromRenderingDictionary(tpikEntry.PulseModel) is IAssetWithModel pulseValue)
                {
                    var color = new Vector4(tpikEntry.ColorRed, tpikEntry.ColorGreen, tpikEntry.ColorBlue, 1f);
                    pulseValue.Draw(renderer, world, color, Vector3.Zero, isSelected);
                    drew = true;
                }
            }
            if (!drew)
                renderer.DrawCube(world, isSelected);
        }
    }
}