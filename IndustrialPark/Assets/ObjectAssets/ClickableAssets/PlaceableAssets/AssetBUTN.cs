using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetBUTN : AssetWithMotion
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x6C + Offset + ScoobyOffset + Motion.Size(game);

        public AssetBUTN(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) => PressedModel_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(PressedModel_AssetID, ref result);
        }

        [Category("Button")]
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

        [Category("Button")]
        public ButnActMethod ActMethod
        {
            get => (ButnActMethod)ReadInt(0x58 + Offset + ScoobyOffset);
            set => Write(0x58 + Offset + ScoobyOffset, (int)value);
        }

        [Category("Button")]
        public int InitialButtonState
        {
            get => ReadInt(0x5C + Offset + ScoobyOffset);
            set => Write(0x5C + Offset + ScoobyOffset, value);
        }

        [Category("Button")]
        public bool ResetAfterDelay
        {
            get => ReadInt(0x60 + Offset + ScoobyOffset) != 0;
            set => Write(0x60 + Offset + ScoobyOffset, value ? 1 : 0);
        }

        [Category("Button"), TypeConverter(typeof(FloatTypeConverter))]
        public float ResetDelay
        {
            get => ReadFloat(0x64 + Offset + ScoobyOffset);
            set => Write(0x64 + Offset + ScoobyOffset, value);
        }

        [Category("Button Hitmask")]
        private uint HitMask
        {
            get => ReadUInt(0x68 + Offset + ScoobyOffset);
            set => Write(0x68 + Offset + ScoobyOffset, value);
        }

        [Category("Button Hitmask")]
        public bool BubbleSpin
        {
            get => (HitMask & Mask(0)) != 0;
            set => HitMask = value ? (HitMask | Mask(0)) : (HitMask & InvMask(0));
        }

        [Category("Button Hitmask")]
        public bool BubbleBounce
        {
            get => (HitMask & Mask(1)) != 0;
            set => HitMask = value ? (HitMask | Mask(1)) : (HitMask & InvMask(1));
        }

        [Category("Button Hitmask")]
        public bool BubbleBash
        {
            get => (HitMask & Mask(2)) != 0;
            set => HitMask = value ? (HitMask | Mask(2)) : (HitMask & InvMask(2));
        }

        [Category("Button Hitmask")]
        public bool BubbleBowlOrBoulder
        {
            get => (HitMask & Mask(3)) != 0;
            set => HitMask = value ? (HitMask | Mask(3)) : (HitMask & InvMask(3));
        }

        [Category("Button Hitmask")]
        public bool CruiseBubble
        {
            get => (HitMask & Mask(4)) != 0;
            set => HitMask = value ? (HitMask | Mask(4)) : (HitMask & InvMask(4));
        }

        [Category("Button Hitmask")]
        public bool Bungee
        {
            get => (HitMask & Mask(5)) != 0;
            set => HitMask = value ? (HitMask | Mask(5)) : (HitMask & InvMask(5));
        }

        [Category("Button Hitmask")]
        public bool ThrowEnemyOrTiki
        {
            get => (HitMask & Mask(6)) != 0;
            set => HitMask = value ? (HitMask | Mask(6)) : (HitMask & InvMask(6));
        }

        [Category("Button Hitmask")]
        public bool ThrowFruit
        {
            get => (HitMask & Mask(7)) != 0;
            set => HitMask = value ? (HitMask | Mask(7)) : (HitMask & InvMask(7));
        }

        [Category("Button Hitmask")]
        public bool PatrickSlam
        {
            get => (HitMask & Mask(8)) != 0;
            set => HitMask = value ? (HitMask | Mask(8)) : (HitMask & InvMask(8));
        }

        [Category("Button Hitmask")]
        public bool HitMask9
        {
            get => (HitMask & Mask(9)) != 0;
            set => HitMask = value ? (HitMask | Mask(9)) : (HitMask & InvMask(9));
        }

        [Category("Button Hitmask")]
        public bool PlayerOnPressurePlate
        {
            get => (HitMask & Mask(10)) != 0;
            set => HitMask = value ? (HitMask | Mask(10)) : (HitMask & InvMask(10));
        }

        [Category("Button Hitmask")]
        public bool EnemyOnPressurePlate
        {
            get => (HitMask & Mask(11)) != 0;
            set => HitMask = value ? (HitMask | Mask(11)) : (HitMask & InvMask(11));
        }

        [Category("Button Hitmask")]
        public bool BubbleBowlOrBoulderPressurePlate
        {
            get => (HitMask & Mask(12)) != 0;
            set => HitMask = value ? (HitMask | Mask(12)) : (HitMask & InvMask(12));
        }

        [Category("Button Hitmask")]
        public bool AnyThrowableOnPressurePlate
        {
            get => (HitMask & Mask(13)) != 0;
            set => HitMask = value ? (HitMask | Mask(13)) : (HitMask & InvMask(13));
        }

        [Category("Button Hitmask")]
        public bool SandyMelee
        {
            get => (HitMask & Mask(14)) != 0;
            set => HitMask = value ? (HitMask | Mask(14)) : (HitMask & InvMask(14));
        }

        [Category("Button Hitmask")]
        public bool PatrickMelee
        {
            get => (HitMask & Mask(15)) != 0;
            set => HitMask = value ? (HitMask | Mask(15)) : (HitMask & InvMask(15));
        }

        [Category("Button Hitmask")]
        public bool ThrowFruitOnPressurePlate
        {
            get => (HitMask & Mask(16)) != 0;
            set => HitMask = value ? (HitMask | Mask(16)) : (HitMask & InvMask(16));
        }

        [Category("Button Hitmask")]
        public bool HitMask17
        {
            get => (HitMask & Mask(17)) != 0;
            set => HitMask = value ? (HitMask | Mask(17)) : (HitMask & InvMask(17));
        }

        [Category("Button Hitmask")]
        public bool HitMask18
        {
            get => (HitMask & Mask(18)) != 0;
            set => HitMask = value ? (HitMask | Mask(18)) : (HitMask & InvMask(18));
        }

        [Category("Button Hitmask")]
        public bool HitMask19
        {
            get => (HitMask & Mask(19)) != 0;
            set => HitMask = value ? (HitMask | Mask(19)) : (HitMask & InvMask(19));
        }

        [Category("Button Hitmask")]
        public bool HitMask20
        {
            get => (HitMask & Mask(20)) != 0;
            set => HitMask = value ? (HitMask | Mask(20)) : (HitMask & InvMask(20));
        }

        [Category("Button Hitmask")]
        public bool HitMask21
        {
            get => (HitMask & Mask(21)) != 0;
            set => HitMask = value ? (HitMask | Mask(21)) : (HitMask & InvMask(21));
        }

        [Category("Button Hitmask")]
        public bool HitMask22
        {
            get => (HitMask & Mask(22)) != 0;
            set => HitMask = value ? (HitMask | Mask(22)) : (HitMask & InvMask(22));
        }

        [Category("Button Hitmask")]
        public bool HitMask23
        {
            get => (HitMask & Mask(23)) != 0;
            set => HitMask = value ? (HitMask | Mask(23)) : (HitMask & InvMask(23));
        }

        [Category("Button Hitmask")]
        public bool HitMask24
        {
            get => (HitMask & Mask(24)) != 0;
            set => HitMask = value ? (HitMask | Mask(24)) : (HitMask & InvMask(24));
        }

        [Category("Button Hitmask")]
        public bool HitMask25
        {
            get => (HitMask & Mask(25)) != 0;
            set => HitMask = value ? (HitMask | Mask(25)) : (HitMask & InvMask(25));
        }

        [Category("Button Hitmask")]
        public bool HitMask26
        {
            get => (HitMask & Mask(26)) != 0;
            set => HitMask = value ? (HitMask | Mask(26)) : (HitMask & InvMask(26));
        }

        [Category("Button Hitmask")]
        public bool HitMask27
        {
            get => (HitMask & Mask(27)) != 0;
            set => HitMask = value ? (HitMask | Mask(27)) : (HitMask & InvMask(27));
        }

        [Category("Button Hitmask")]
        public bool HitMask28
        {
            get => (HitMask & Mask(28)) != 0;
            set => HitMask = value ? (HitMask | Mask(28)) : (HitMask & InvMask(28));
        }

        [Category("Button Hitmask")]
        public bool HitMask29
        {
            get => (HitMask & Mask(29)) != 0;
            set => HitMask = value ? (HitMask | Mask(29)) : (HitMask & InvMask(29));
        }

        [Category("Button Hitmask")]
        public bool HitMask30
        {
            get => (HitMask & Mask(30)) != 0;
            set => HitMask = value ? (HitMask | Mask(30)) : (HitMask & InvMask(30));
        }

        [Category("Button Hitmask")]
        public bool HitMask31
        {
            get => (HitMask & Mask(31)) != 0;
            set => HitMask = value ? (HitMask | Mask(31)) : (HitMask & InvMask(31));
        }
        
        protected override int MotionStart => 0x6C + Offset + ScoobyOffset;
    }
}