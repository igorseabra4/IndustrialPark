using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum ButnActMethod
    {
        Button = 0,
        PressurePlate = 1
    }

    public class AssetBUTN : AssetWithMotion
    {
        private const string categoryName = "Button";

        [Category(categoryName)]
        public AssetID PressedModel_AssetID { get; set; }
        [Category(categoryName)]
        public ButnActMethod ActMethod { get; set; }
        [Category(categoryName)]
        public int InitialButtonState { get; set; }
        [Category(categoryName)]
        public bool ResetAfterDelay { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float ResetDelay { get; set; }
        [Category(categoryName)]
        public FlagBitmask HitMask { get; set; } = IntFlagsDescriptor(
               "Bubble Spin/Sliding",
               "Bubble Bounce",
               "Bubble Bash",
               "Boulder/Bubble Bowl",
               "Cruise Bubble",
               "Bungee",
               "Thrown Enemy/Tiki",
               "Throw Fruit",
               "Patrick Slam",
               null,
               "(Pressure Plate) Player Stand",
               "(Pressure Plate) Enemy Stand",
               "(Pressure Plate) Boulder/Bubble Bowl",
               "(Pressure Plate) Stone Tiki",
               "Sandy Melee/Sliding",
               "Patrick Melee/Sliding",
               "(Pressure Plate) Throw Fruit",
               "Patrick Cartwheel");

        public AssetBUTN(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityEndPosition;

            if (game != Game.Scooby)
                PressedModel_AssetID = reader.ReadUInt32();
            ActMethod = (ButnActMethod)reader.ReadInt32();
            InitialButtonState = reader.ReadInt32();
            ResetAfterDelay = reader.ReadInt32() != 0;
            ResetDelay = reader.ReadInt32();
            HitMask.FlagValueInt = reader.ReadUInt32();

            Motion = new Motion_Mechanism(reader, game);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));

            if (game != Game.Scooby)
                writer.Write(PressedModel_AssetID);
            writer.Write((int)ActMethod);
            writer.Write(InitialButtonState);
            writer.Write(ResetAfterDelay ? 1: 0);
            writer.Write(ResetDelay);
            writer.Write(HitMask.FlagValueInt);
            writer.Write(Motion.Serialize(game, platform));

            int linkStart =
                game == Game.Scooby ? 0x94 :
                game == Game.BFBB ? 0x9C :
                game == Game.Incredibles ? 0xA8 : throw new System.ArgumentException("Invalid game");

            while (writer.BaseStream.Length < linkStart)
                writer.Write((byte)0);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
                
        public override bool HasReference(uint assetID) => PressedModel_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(PressedModel_AssetID, ref result);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("PressedModel_AssetID");
            base.SetDynamicProperties(dt);
        }
    }
}