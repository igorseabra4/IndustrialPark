using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBOUL : EntityAsset
    {
        private const string categoryName = "Boulder";

        [Category(categoryName)]
        public AssetSingle Gravity { get; set; }
        [Category(categoryName)]
        public AssetSingle Mass { get; set; }
        [Category(categoryName)]
        public AssetSingle BounceFactor { get; set; }
        [Category(categoryName)]
        public AssetSingle Friction { get; set; }
        [Category(categoryName)] // bfbb only
        public AssetSingle StartFriction { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxLinearVelocity { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxAngularVelocity { get; set; }
        [Category(categoryName)]
        public AssetSingle Stickiness { get; set; }
        [Category(categoryName)]
        public AssetSingle BounceDamp { get; set; }
        [Category(categoryName)]
        public FlagBitmask BoulderFlags { get; set; } = IntFlagsDescriptor(
            "Can Hit Walls",
                "Damage Player",
                "Something related to destructibles",
                "Damage NPCs",
                null,
                "Die on OOB surfaces",
                null,
                null,
                "Die on player attack",
                "Die after kill timer");
        [Category(categoryName)]
        public AssetSingle KillTimer { get; set; }
        [Category(categoryName)]
        public int Hitpoints { get; set; }
        [Category(categoryName)]
        public AssetID Sound_AssetID { get; set; }
        [Category(categoryName)] // bfbb only
        public AssetSingle Volume { get; set; }
        [Category(categoryName)]
        public AssetSingle MinSoundVel { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxSoundVel { get; set; }
        [Category(categoryName)]
        public AssetSingle InnerRadius { get; set; }
        [Category(categoryName)]
        public AssetSingle OuterRadius { get; set; }

        public AssetBOUL(string assetName, Vector3 position) : base(assetName, AssetType.BOUL, BaseAssetType.Boulder, position)
        {
            SolidityFlags.FlagValueByte = 0;
            ColorAlpha = 0;
            ColorAlphaSpeed = 0;
        }

        public AssetBOUL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                Gravity = reader.ReadSingle();
                Mass = reader.ReadSingle();
                BounceFactor = reader.ReadSingle();
                Friction = reader.ReadSingle();
                if (game == Game.BFBB)
                    StartFriction = reader.ReadSingle();
                MaxLinearVelocity = reader.ReadSingle();
                MaxAngularVelocity = reader.ReadSingle();
                Stickiness = reader.ReadSingle();
                BounceDamp = reader.ReadSingle();
                BoulderFlags.FlagValueInt = reader.ReadUInt32();
                KillTimer = reader.ReadSingle();
                Hitpoints = reader.ReadInt32();
                Sound_AssetID = reader.ReadUInt32();
                if (game == Game.BFBB)
                    Volume = reader.ReadSingle();
                MinSoundVel = reader.ReadSingle();
                MaxSoundVel = reader.ReadSingle();
                InnerRadius = reader.ReadSingle();
                OuterRadius = reader.ReadSingle();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(Gravity);
                writer.Write(Mass);
                writer.Write(BounceFactor);
                writer.Write(Friction);
                if (game == Game.BFBB)
                    writer.Write(StartFriction);
                writer.Write(MaxLinearVelocity);
                writer.Write(MaxAngularVelocity);
                writer.Write(Stickiness);
                writer.Write(BounceDamp);
                writer.Write(BoulderFlags.FlagValueInt);
                writer.Write(KillTimer);
                writer.Write(Hitpoints);
                writer.Write(Sound_AssetID);
                if (game == Game.BFBB)
                    writer.Write(Volume);
                writer.Write(MinSoundVel);
                writer.Write(MaxSoundVel);
                writer.Write(InnerRadius);
                writer.Write(OuterRadius);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) => Sound_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Sound_AssetID, ref result);
        }

        public override void Draw(SharpRenderer renderer)
        {
            Vector4 Color = _color;
            Color.W = Color.W == 0f ? 1f : Color.W;

            if (ArchiveEditorFunctions.renderingDictionary.ContainsKey(_modelAssetID))
                ArchiveEditorFunctions.renderingDictionary[_modelAssetID].Draw(renderer, LocalWorld(), isSelected ? renderer.selectedObjectColor * Color : Color, UvAnimOffset);
            else
                renderer.DrawCube(LocalWorld(), isSelected);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game != Game.BFBB)
            {
                dt.RemoveProperty("Volume");
                dt.RemoveProperty("StartFriction");
            }
            base.SetDynamicProperties(dt);
        }
    }

}