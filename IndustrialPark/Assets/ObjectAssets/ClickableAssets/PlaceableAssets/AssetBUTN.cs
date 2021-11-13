using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum ButnActMethod
    {
        Button = 0,
        PressurePlate = 1,
        Other = 2
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
        [Category(categoryName)]
        public AssetSingle ResetDelay { get; set; }
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

        public AssetBUTN(string assetName, Vector3 position, AssetTemplate template) : base(assetName, AssetType.BUTN, BaseAssetType.Button, position)
        {
            if (template == AssetTemplate.Button_Red)
            {
                Model_AssetID = "button";
                PressedModel_AssetID = "button_grn";
                HitMask.FlagValueInt =
                    2 << 0 |
                    2 << 3 |
                    2 << 4 |
                    2 << 6 |
                    2 << 7 |
                    2 << 14 |
                    2 << 15;
                Motion = new Motion_Mechanism()
                {
                    MovementType = EMovementType.SlideAndRotate,
                    MovementLoopMode = EMechanismFlags.ReturnToStart,
                    SlideAxis = Axis.Y,
                    SlideDistance = -0.2f,
                    SlideTime = 0.5f,
                    SlideDecelTime = 0.2f
                };
                Motion.MotionFlags.FlagValueInt = 4;
            }
            else if (template == AssetTemplate.PressurePlate)
            {
                ActMethod = ButnActMethod.PressurePlate;
                Model_AssetID = "plate_pressure";
                PressedModel_AssetID = 0xCE7F8131;
                HitMask.FlagValueInt =
                    2 << 10 |
                    2 << 12 |
                    2 << 13 |
                    2 << 16;
                Motion = new Motion_Mechanism()
                {
                    MovementType = EMovementType.SlideAndRotate,
                    MovementLoopMode = EMechanismFlags.ReturnToStart,
                    SlideAxis = Axis.Y,
                    SlideDistance = -0.15f,
                    SlideTime = 0.15f,
                };
                Motion.MotionFlags.FlagValueInt = 4;
            }
            else
                Motion = new Motion_Mechanism();
        }

        public AssetBUTN(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                if (game != Game.Scooby)
                    PressedModel_AssetID = reader.ReadUInt32();
                ActMethod = (ButnActMethod)reader.ReadInt32();
                InitialButtonState = reader.ReadInt32();
                ResetAfterDelay = reader.ReadInt32Bool();
                ResetDelay = reader.ReadSingle();
                HitMask.FlagValueInt = reader.ReadUInt32();
                Motion = new Motion_Mechanism(reader, game);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));

                if (game != Game.Scooby)
                    writer.Write(PressedModel_AssetID);
                writer.Write((int)ActMethod);
                writer.Write(InitialButtonState);
                writer.Write(ResetAfterDelay ? 1 : 0);
                writer.Write(ResetDelay);
                writer.Write(HitMask.FlagValueInt);
                writer.Write(Motion.Serialize(game, endianness));

                int linkStart =
                    game == Game.Scooby ? 0x94 :
                    game == Game.BFBB ? 0x9C :
                    game == Game.Incredibles ? 0xA4 : throw new ArgumentException("Invalid game");

                while (writer.BaseStream.Length < linkStart)
                    writer.Write((byte)0);
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
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