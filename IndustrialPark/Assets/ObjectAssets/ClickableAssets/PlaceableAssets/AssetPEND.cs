using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPEND : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;
        
        public AssetPEND(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x84 + Offset;

        [Category("Pendulum")]
        public byte UnknownByte54
        {
            get => ReadByte(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Pendulum")]
        public byte UnknownByte55
        {
            get => ReadByte(0x55 + Offset);
            set => Write(0x55 + Offset, value);
        }

        [Category("Pendulum")]
        public byte UnknownByte56
        {
            get => ReadByte(0x56 + Offset);
            set => Write(0x56 + Offset, value);
        }

        [Category("Pendulum")]
        public byte UnknownByte57
        {
            get => ReadByte(0x57 + Offset);
            set => Write(0x57 + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt58
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Pendulum")]
        public float MovementDistance
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Pendulum")]
        public float Steepness
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Pendulum")]
        public float MovementTime
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt68
        {
            get => ReadInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt6C
        {
            get => ReadInt(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt70
        {
            get => ReadInt(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt74
        {
            get => ReadInt(0x74 + Offset);
            set => Write(0x74 + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt78
        {
            get => ReadInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt7C
        {
            get => ReadInt(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Pendulum")]
        public int UnknownInt80
        {
            get => ReadInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

    }
}