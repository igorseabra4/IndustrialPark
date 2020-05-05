using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBUTN : AssetWithMotion
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x6C + Offset + ScoobyOffset + (game == Game.Incredibles ? 0x3C : 0x30);

        public AssetBUTN(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }
        
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

        private const string categoryName = "Button";

        [Category(categoryName)]
        [Description("Not present in Scooby")]
        public AssetID PressedModel_AssetID
        {
            get => game == Game.Scooby ? 0 : ReadUInt(0x54 + Offset);
            set
            {
                if (game != Game.Scooby)
                    Write(0x54 + Offset, value);
            }
        }

        private int ScoobyOffset => game == Game.Scooby ? -0x04 : 0;

        public enum ButnActMethod
        {
            Button = 0,
            PressurePlate = 1
        }

        [Category(categoryName)]
        public ButnActMethod ActMethod
        {
            get => (ButnActMethod)ReadInt(0x58 + Offset + ScoobyOffset);
            set => Write(0x58 + Offset + ScoobyOffset, (int)value);
        }

        [Category(categoryName)]
        public int InitialButtonState
        {
            get => ReadInt(0x5C + Offset + ScoobyOffset);
            set => Write(0x5C + Offset + ScoobyOffset, value);
        }

        [Category(categoryName)]
        public bool ResetAfterDelay
        {
            get => ReadInt(0x60 + Offset + ScoobyOffset) != 0;
            set => Write(0x60 + Offset + ScoobyOffset, value ? 1 : 0);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float ResetDelay
        {
            get => ReadFloat(0x64 + Offset + ScoobyOffset);
            set => Write(0x64 + Offset + ScoobyOffset, value);
        }

        [Category(categoryName)]
        public DynamicTypeDescriptor HitMask => IntFlagsDescriptor(0x68 + Offset + ScoobyOffset,
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

        [Browsable(false)]
        public uint HitMaskUint
        {
            get => ReadUInt(0x68 + Offset + ScoobyOffset);
            set => Write(0x68 + Offset + ScoobyOffset, value);
        }

        public override int MotionStart => 0x6C + Offset + ScoobyOffset;
    }
}