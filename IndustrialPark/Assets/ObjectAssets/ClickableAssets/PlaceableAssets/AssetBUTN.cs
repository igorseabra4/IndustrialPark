﻿using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBUTN : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender { get => dontRender; }

        protected override int EventStartOffset { get => 0x9C + Offset; }

        public AssetBUTN(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (PressedModelAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Button")]
        public AssetID PressedModelAssetID
        {
            get => ReadUInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        public enum ButnHitMode
        {
            Button = 0,
            PressurePlate = 1
        }

        [Category("Button")]
        public ButnHitMode ButtonType
        {
            get => (ButnHitMode)ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, (int)value);
        }

        [Category("Button")]
        public int UnknownInt5C
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Button")]
        public bool HoldEnabled
        {
            get => ReadInt(0x60 + Offset) != 0;
            set => Write(0x60 + Offset, value ? 1 : 0);
        }

        [Category("Button"), TypeConverter(typeof(FloatTypeConverter))]
        public float HoldTime
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Button Hitmask")]
        private uint HitMask
        {
            get => ReadUInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        private uint Mask(uint bit)
        {
            return (uint)Math.Pow(2, bit);
        }

        private uint InvMask(uint bit)
        {
            return uint.MaxValue - Mask(bit);
        }

        [Category("Button Hitmask")]
        public bool BubbleSpin
        {
            get => (HitMask & Mask(0)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(0);
                else
                    HitMask = HitMask & InvMask(0);
            }
        }

        [Category("Button Hitmask")]
        public bool BubbleBounce
        {
            get => (HitMask & Mask(1)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(1);
                else
                    HitMask = HitMask & InvMask(1);
            }
        }

        [Category("Button Hitmask")]
        public bool BubbleBash
        {
            get => (HitMask & Mask(2)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(2);
                else
                    HitMask = HitMask & InvMask(2);
            }
        }

        [Category("Button Hitmask")]
        public bool BubbleBowl
        {
            get => (HitMask & Mask(3)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(3);
                else
                    HitMask = HitMask & InvMask(3);
            }
        }

        [Category("Button Hitmask")]
        public bool CruiseBubble
        {
            get => (HitMask & Mask(4)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(4);
                else
                    HitMask = HitMask & InvMask(4);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask5
        {
            get => (HitMask & Mask(5)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(5);
                else
                    HitMask = HitMask & InvMask(5);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask6
        {
            get => (HitMask & Mask(6)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(6);
                else
                    HitMask = HitMask & InvMask(6);
            }
        }

        [Category("Button Hitmask")]
        public bool Throwable
        {
            get => (HitMask & Mask(7)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(7);
                else
                    HitMask = HitMask & InvMask(7);
            }
        }

        [Category("Button Hitmask")]
        public bool PatrickSlam
        {
            get => (HitMask & Mask(8)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(8);
                else
                    HitMask = HitMask & InvMask(8);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask9
        {
            get => (HitMask & Mask(9)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(9);
                else
                    HitMask = HitMask & InvMask(9);
            }
        }

        [Category("Button Hitmask")]
        public bool PlayerOnPressurePlate
        {
            get => (HitMask & Mask(10)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(10);
                else
                    HitMask = HitMask & InvMask(10);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask11
        {
            get => (HitMask & Mask(11)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(11);
                else
                    HitMask = HitMask & InvMask(11);
            }
        }

        [Category("Button Hitmask")]
        public bool BubbleBowlOnPressurePlate
        {
            get => (HitMask & Mask(12)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(12);
                else
                    HitMask = HitMask & InvMask(12);
            }
        }

        [Category("Button Hitmask")]
        public bool AnyThrowableOnPressurePlate
        {
            get => (HitMask & Mask(13)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(13);
                else
                    HitMask = HitMask & InvMask(13);
            }
        }

        [Category("Button Hitmask")]
        public bool SandyMelee
        {
            get => (HitMask & Mask(14)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(14);
                else
                    HitMask = HitMask & InvMask(14);
            }
        }

        [Category("Button Hitmask")]
        public bool PatrickBelly
        {
            get => (HitMask & Mask(15)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(15);
                else
                    HitMask = HitMask & InvMask(15);
            }
        }

        [Category("Button Hitmask")]
        public bool ThrowFruitOnPressurePlate
        {
            get => (HitMask & Mask(16)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(16);
                else
                    HitMask = HitMask & InvMask(16);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask17
        {
            get => (HitMask & Mask(17)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(17);
                else
                    HitMask = HitMask & InvMask(17);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask18
        {
            get => (HitMask & Mask(18)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(18);
                else
                    HitMask = HitMask & InvMask(18);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask19
        {
            get => (HitMask & Mask(19)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(19);
                else
                    HitMask = HitMask & InvMask(19);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask20
        {
            get => (HitMask & Mask(20)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(20);
                else
                    HitMask = HitMask & InvMask(20);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask21
        {
            get => (HitMask & Mask(21)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(21);
                else
                    HitMask = HitMask & InvMask(21);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask22
        {
            get => (HitMask & Mask(22)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(22);
                else
                    HitMask = HitMask & InvMask(22);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask23
        {
            get => (HitMask & Mask(23)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(23);
                else
                    HitMask = HitMask & InvMask(23);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask24
        {
            get => (HitMask & Mask(24)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(24);
                else
                    HitMask = HitMask & InvMask(24);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask25
        {
            get => (HitMask & Mask(25)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(25);
                else
                    HitMask = HitMask & InvMask(25);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask26
        {
            get => (HitMask & Mask(26)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(26);
                else
                    HitMask = HitMask & InvMask(26);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask27
        {
            get => (HitMask & Mask(27)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(27);
                else
                    HitMask = HitMask & InvMask(27);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask28
        {
            get => (HitMask & Mask(28)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(28);
                else
                    HitMask = HitMask & InvMask(28);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask29
        {
            get => (HitMask & Mask(29)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(29);
                else
                    HitMask = HitMask & InvMask(29);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask30
        {
            get => (HitMask & Mask(30)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(30);
                else
                    HitMask = HitMask & InvMask(30);
            }
        }

        [Category("Button Hitmask")]
        public bool HitMask31
        {
            get => (HitMask & Mask(31)) != 0;
            set
            {
                if (value)
                    HitMask = HitMask | Mask(31);
                else
                    HitMask = HitMask & InvMask(31);
            }
        }

        [Category("Button")]
        public byte UnknownByte6C
        {
            get => ReadByte(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte6D
        {
            get => ReadByte(0x6D + Offset);
            set => Write(0x6D + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte6E
        {
            get => ReadByte(0x6E + Offset);
            set => Write(0x6E + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte6F
        {
            get => ReadByte(0x6F + Offset);
            set => Write(0x6F + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte70
        {
            get => ReadByte(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte71
        {
            get => ReadByte(0x71 + Offset);
            set => Write(0x71 + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte72
        {
            get => ReadByte(0x72 + Offset);
            set => Write(0x72 + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte73
        {
            get => ReadByte(0x73 + Offset);
            set => Write(0x73 + Offset, value);
        }

        [Category("Button")]
        public float PressedOffset
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }

        [Category("Button")]
        public float TransitionTime
        {
            get => ReadFloat(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Button")]
        public float TransitionEaseIn
        {
            get => ReadFloat(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Button")]
        public float TransitionEaseOut
        {
            get => ReadFloat(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt84
        {
            get => ReadInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt88
        {
            get => ReadInt(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt8C
        {
            get => ReadInt(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt90
        {
            get => ReadInt(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt94
        {
            get => ReadInt(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt98
        {
            get => ReadInt(0x98 + Offset);
            set => Write(0x98 + Offset, value);
        }
    }
}