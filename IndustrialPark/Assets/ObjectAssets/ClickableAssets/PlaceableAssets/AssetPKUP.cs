using HipHopFile;
using SharpDX;
using System;
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

        public override string AssetInfo
        {
            get
            {
                if (AssetPICK.pickEntries.ContainsKey(PickReferenceID))
                    return HexUIntTypeConverter.StringFromAssetID(AssetPICK.pickEntries[PickReferenceID]);
                return HexUIntTypeConverter.StringFromAssetID(PickReferenceID);
            }
        }

        [Category(categoryName)]
        public AssetByte Shape { get => TypeFlag; set => TypeFlag = value; }

        [Category(categoryName)]
        public AssetID PickReferenceID { get; set; }

        [Category(categoryName)]
        public EPickupFlags PickupFlags { get; set; }

        [Category(categoryName)]
        public short PickupValue { get; set; }

        public AssetPKUP(string assetName, Game game, Vector3 position, AssetTemplate template) : base(assetName, AssetType.Pickup, BaseAssetType.Pickup, position)
        {
            if (game == Game.Incredibles)
                Model = 0x94E25463;
            else
                Model = pkupsMinfName;

            if (game != Game.Scooby)
                PickupFlags = EPickupFlags.InitiallyVisible;

            PickupValue = 4;
            PositionY += 0.5f;

            switch (template)
            {
                case AssetTemplate.Shiny_Red:
                    PickReferenceID = 0x7C8AC53E;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Shiny_Yellow:
                    PickReferenceID = 0xB3D6283B;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Shiny_Green:
                    PickReferenceID = 0x079A0734;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Shiny_Blue:
                    PickReferenceID = 0x6D4A4181;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Shiny_Purple:
                    PickReferenceID = 0xFA607BCB;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Underwear:
                    PickReferenceID = 0x28F55613;
                    break;
                case AssetTemplate.Spatula:
                    StateIsPersistent = true;
                    PickReferenceID = 0x8BDFE8DD;
                    break;
                case AssetTemplate.Sock:
                    StateIsPersistent = true;
                    PickReferenceID = 0x74B46F24;
                    break;
                case AssetTemplate.Spongeball:
                    PickReferenceID = 0xF09A1415;
                    PickupFlags = EPickupFlags.Both;
                    break;
                case AssetTemplate.Golden_Underwear:
                    StateIsPersistent = true;
                    PickReferenceID = 0xF650DA2E;
                    break;
                case AssetTemplate.Artwork:
                    StateIsPersistent = true;
                    PickReferenceID = 0x18140B10;
                    break;
                case AssetTemplate.Steering_Wheel:
                    StateIsPersistent = true;
                    PickReferenceID = 0x4C67C832;
                    break;
                case AssetTemplate.Power_Crystal:
                    StateIsPersistent = true;
                    PickReferenceID = 0xFE7A89BB;
                    break;
                case AssetTemplate.Smelly_Sundae:
                    PickReferenceID = 0x6A779454;
                    break;
                case AssetTemplate.Manliness_Red:
                    PickReferenceID = 0x7C134517;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Manliness_Yellow:
                    PickReferenceID = 0xFA454C5A;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Manliness_Green:
                    PickReferenceID = 0xA869F4D9;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Manliness_Blue:
                    PickReferenceID = 0x7BB95F4C;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Manliness_Purple:
                    PickReferenceID = 0x3C48E68A;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Krabby_Patty:
                    PickReferenceID = 0xACC4FBD1;
                    break;
                case AssetTemplate.Goofy_Goober_Token:
                    StateIsPersistent = true;
                    PickReferenceID = 0x60F808B7;
                    break;
                case AssetTemplate.Treasure_Chest:
                    StateIsPersistent = true;
                    PickReferenceID = 0xA613E48A;
                    break;
                case AssetTemplate.Nitro:
                    PickReferenceID = 0x630BD71A;
                    PickupFlags = EPickupFlags.Both;
                    break;
                case AssetTemplate.Scooby_Snack:
                    PickReferenceID = 0xA19FB2BC;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Snack_Box:
                    PickReferenceID = 0xBBE326EC;
                    if (persistentShinies)
                        StateIsPersistent = true;
                    break;
                case AssetTemplate.Save_Point:
                    StateIsPersistent = true;
                    PickReferenceID = 0x3842735C;
                    break;
                case AssetTemplate.Warp_Gate:
                    StateIsPersistent = true;
                    PickReferenceID = 0xE16EE363;
                    PickupValue = 0;
                    break;
                case AssetTemplate.Snack_Gate:
                    StateIsPersistent = true;
                    PickReferenceID = 0x25E4C286;
                    PickupValue = 0;
                    break;
                case AssetTemplate.Key:
                    StateIsPersistent = true;
                    PickReferenceID = 0x6F24CBD8;
                    break;
                case AssetTemplate.Clue:
                    StateIsPersistent = true;
                    PickReferenceID = 0xDEC3B628;
                    break;
                case AssetTemplate.Gum:
                    PickReferenceID = 0xE80B345F;
                    break;
                case AssetTemplate.Gum_Pack:
                    PickReferenceID = 0x9CAACD2F;
                    break;
                case AssetTemplate.Soap:
                    PickReferenceID = 0xDA8B4640;
                    break;
                case AssetTemplate.Soap_Pack:
                    PickReferenceID = 0x15AC3C90;
                    break;
                case AssetTemplate.Sandwich:
                    PickReferenceID = 0xDF2B6501;
                    break;
                case AssetTemplate.Turkey:
                    PickReferenceID = 0xC729266B;
                    break;
                case AssetTemplate.Ice_Cream:
                    PickReferenceID = 0x9F257364;
                    break;
                case AssetTemplate.Hamburger:
                    PickReferenceID = 0x48C67C0C;
                    break;
                case AssetTemplate.Cake:
                    PickReferenceID = 0x322226D1;
                    break;
                case AssetTemplate.Shovel:
                    StateIsPersistent = true;
                    PickReferenceID = 0xA9D3F180;
                    break;
                case AssetTemplate.Springs:
                    StateIsPersistent = true;
                    PickReferenceID = 0x36A529FA;
                    break;
                case AssetTemplate.Slippers:
                    StateIsPersistent = true;
                    PickReferenceID = 0xF67E0CFF;
                    break;
                case AssetTemplate.Lampshade:
                    StateIsPersistent = true;
                    PickReferenceID = 0x597E7EC4;
                    break;
                case AssetTemplate.Helmet:
                    StateIsPersistent = true;
                    PickReferenceID = 0x731AF2B3;
                    break;
                case AssetTemplate.Knight_Helmet:
                    StateIsPersistent = true;
                    PickReferenceID = 0x4B674863;
                    break;
                case AssetTemplate.Boots:
                    StateIsPersistent = true;
                    PickReferenceID = 0x762BF547;
                    break;
                case AssetTemplate.Super_Smash:
                    StateIsPersistent = true;
                    PickReferenceID = 0x8DE96F8E;
                    break;
                case AssetTemplate.Plungers:
                    StateIsPersistent = true;
                    PickReferenceID = 0x36302F2D;
                    break;
                case AssetTemplate.Super_Sonic_Smash:
                    StateIsPersistent = true;
                    PickReferenceID = 0xC69693F0;
                    break;
                case AssetTemplate.Umbrella:
                    StateIsPersistent = true;
                    PickReferenceID = 0xDB0201DD;
                    break;
                case AssetTemplate.Gum_Machine:
                    StateIsPersistent = true;
                    PickReferenceID = 0xB296AAA2;
                    break;
                case AssetTemplate.Soap_Bubble:
                    StateIsPersistent = true;
                    PickReferenceID = 0xD1840C2B;
                    break;
            }

            Shape = BitConverter.GetBytes(PickReferenceID)[3];

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