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

        public AssetPKUP(string assetName, Game game, Vector3 position, AssetTemplate template) : base(assetName, AssetType.PKUP, BaseAssetType.Pickup, position)
        {
            if (game == Game.BFBB)
                Model_AssetID = pkupsMinfName;
            else if (game == Game.Incredibles)
                Model_AssetID = 0x94E25463;

            if (template.ToString().Contains("Shiny") || template.ToString().Contains("Manliness"))
                if (persistentShinies)
                    StateIsPersistent = true;

            PickupFlags = EPickupFlags.InitiallyVisible;
            PickupValue = 4;
            PositionY += 0.5f;

            switch (template)
            {
                case AssetTemplate.Shiny_Red:
                    Shape = 0x3E;
                    PickReferenceID = 0x7C8AC53E;
                    break;
                case AssetTemplate.Shiny_Yellow:
                    Shape = 0x3B;
                    PickReferenceID = 0xB3D6283B;
                    break;
                case AssetTemplate.Shiny_Green:
                    Shape = 0x34;
                    PickReferenceID = 0x079A0734;
                    break;
                case AssetTemplate.Shiny_Blue:
                    Shape = 0x81;
                    PickReferenceID = 0x6D4A4181;
                    break;
                case AssetTemplate.Shiny_Purple:
                    Shape = 0xCB;
                    PickReferenceID = 0xFA607BCB;
                    break;
                case AssetTemplate.Underwear:
                    Shape = 0x13;
                    PickReferenceID = 0x28F55613;
                    break;
                case AssetTemplate.Spatula:
                    StateIsPersistent = true;
                    Shape = 0xDD;
                    PickReferenceID = 0x8BDFE8DD;
                    break;
                case AssetTemplate.Sock:
                    StateIsPersistent = true;
                    Shape = 0x24;
                    PickReferenceID = 0x74B46F24;
                    break;
                case AssetTemplate.Spongeball:
                    Shape = 0x15;
                    PickReferenceID = 0xF09A1415;
                    PickupFlags = EPickupFlags.Both;
                    break;
                case AssetTemplate.Golden_Underwear:
                    StateIsPersistent = true;
                    Shape = 0x2E;
                    PickReferenceID = 0xF650DA2E;
                    break;
                case AssetTemplate.Artwork:
                    StateIsPersistent = true;
                    Shape = 0x10;
                    PickReferenceID = 0x18140B10;
                    break;
                case AssetTemplate.SteeringWheel:
                    StateIsPersistent = true;
                    Shape = 0x32;
                    PickReferenceID = 0x4C67C832;
                    break;
                case AssetTemplate.PowerCrystal:
                    StateIsPersistent = true;
                    Shape = 0xBB;
                    PickReferenceID = 0xFE7A89BB;
                    break;
                case AssetTemplate.Smelly_Sundae:
                    Shape = 0x54;
                    PickReferenceID = 0x6A779454;
                    break;
                case AssetTemplate.Manliness_Red:
                    Shape = 0x17;
                    PickReferenceID = 0x7C134517;
                    break;
                case AssetTemplate.Manliness_Yellow:
                    Shape = 0x5A;
                    PickReferenceID = 0xFA454C5A;
                    break;
                case AssetTemplate.Manliness_Green:
                    Shape = 0xD9;
                    PickReferenceID = 0xA869F4D9;
                    break;
                case AssetTemplate.Manliness_Blue:
                    Shape = 0x4C;
                    PickReferenceID = 0x7BB95F4C;
                    break;
                case AssetTemplate.Manliness_Purple:
                    Shape = 0x8A;
                    PickReferenceID = 0x3C48E68A;
                    break;
                case AssetTemplate.KrabbyPatty:
                    Shape = 0xD1;
                    PickReferenceID = 0xACC4FBD1;
                    break;
                case AssetTemplate.GoofyGooberToken:
                    StateIsPersistent = true;
                    Shape = 0xB7;
                    PickReferenceID = 0x60F808B7;
                    break;
                case AssetTemplate.TreasureChest:
                    StateIsPersistent = true;
                    Shape = 0x8A;
                    PickReferenceID = 0xA613E48A;
                    break;
                case AssetTemplate.Nitro:
                    Shape = 0x1A;
                    PickReferenceID = 0x630BD71A;
                    PickupFlags = EPickupFlags.Both;
                    break;
            }

            CreateTransformMatrix();
        }

        public AssetPKUP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                PickReferenceID = reader.ReadUInt32();
                PickupFlags = (EPickupFlags)reader.ReadUInt16();
                PickupValue = reader.ReadInt16();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(PickReferenceID);
                writer.Write((ushort)PickupFlags);
                writer.Write(PickupValue);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
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

        public override Matrix PlatLocalRotation()
        {
            return Matrix.RotationY(localFrameCounter * MathUtil.Pi / 60f);
        }

        public override Matrix LocalWorld()
        {
            if (movementPreview)
            {
                var driver = FindDrivenByAsset(out bool useRotation);

                if (driver != null)
                {
                    return PlatLocalRotation() * Matrix.Scaling(_scale)
                        * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                        * Matrix.Translation(_position - new Vector3(driver.PositionX, driver.PositionY, driver.PositionZ))
                        * (useRotation ? driver.PlatLocalRotation() : Matrix.Identity)
                        * Matrix.Translation((Vector3)Vector3.Transform(Vector3.Zero, driver.LocalWorld()));
                }

                return PlatLocalRotation()
                    * Matrix.Scaling(_scale)
                    * Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll)
                    * Matrix.Translation(_position);
            }

            return world;
        }
    }
}