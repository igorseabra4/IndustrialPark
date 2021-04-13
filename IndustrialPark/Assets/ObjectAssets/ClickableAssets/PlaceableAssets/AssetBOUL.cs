using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBOUL : EntityAsset
    {
        private const string categoryName = "Boulder";

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Gravity { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Mass { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float BounceFactor { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Friction { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))] // bfbb only
        public float StartFriction { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxLinearVelocity { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxAngularVelocity { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Stickiness { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float BounceDamp { get; set; }
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
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float KillTimer { get; set; }
        [Category(categoryName)]
        public int Hitpoints { get; set; }
        [Category(categoryName)]
        public AssetID Sound_AssetID { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))] // bfbb only
        public float Volume { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MinSoundVel { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MaxSoundVel { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float InnerRadius { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float OuterRadius { get; set; }

        public AssetBOUL(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityEndPosition;

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

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));

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

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
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