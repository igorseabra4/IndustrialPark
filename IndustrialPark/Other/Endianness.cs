using AssetEditorColors;
using HipHopFile;
using System;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public enum Endianness
    {
        Little,
        Big,
        Unknown
    }

    public static class Extensions
    {
        public static Endianness Endianness(this Platform platform) =>
            platform == Platform.GameCube ? IndustrialPark.Endianness.Big : IndustrialPark.Endianness.Little;
    }

    public class EndianBinaryReader : BinaryReader
    {
        public readonly Endianness endianness;

        public EndianBinaryReader(byte[] data, Endianness endianness) : base(new MemoryStream(data))
        {
            this.endianness = endianness;
        }

        public override float ReadSingle() =>
            (endianness == Endianness.Little) ?
            base.ReadSingle() :
            BitConverter.ToSingle(base.ReadBytes(4).Reverse().ToArray(), 0);

        public override short ReadInt16() =>
            (endianness == Endianness.Little) ?
            base.ReadInt16() :
            BitConverter.ToInt16(base.ReadBytes(2).Reverse().ToArray(), 0);

        public override int ReadInt32() =>
            (endianness == Endianness.Little) ?
            base.ReadInt32() :
            BitConverter.ToInt32(base.ReadBytes(4).Reverse().ToArray(), 0);

        public override ushort ReadUInt16() =>
            (endianness == Endianness.Little) ?
            base.ReadUInt16() :
            BitConverter.ToUInt16(base.ReadBytes(2).Reverse().ToArray(), 0);

        public override uint ReadUInt32() =>
            (endianness == Endianness.Little) ?
            base.ReadUInt32() :
            BitConverter.ToUInt32(base.ReadBytes(4).Reverse().ToArray(), 0);

        public bool ReadByteBool() => ReadByte() != 0;

        public bool ReadInt16Bool() => ReadInt16() != 0;

        public bool ReadInt32Bool() => ReadInt32() != 0;

        public string ReadString(int length) => System.Text.Encoding.GetEncoding(1252).GetString(ReadBytes(length));

        public AssetColor ReadColor() => new AssetColor(ReadByte(), ReadByte(), ReadByte(), ReadByte());

        public bool EndOfStream => BaseStream.Position == BaseStream.Length;
    }

    public class EndianBinaryWriter : BinaryWriter
    {
        public readonly Endianness endianness;

        public EndianBinaryWriter(Endianness endianness) : base(new MemoryStream())
        {
            this.endianness = endianness;
        }

        public byte[] ToArray() => ((MemoryStream)BaseStream).ToArray();

        public override void Write(float f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else WriteReverse(BitConverter.GetBytes(f));
        }

        public override void Write(int f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else WriteReverse(BitConverter.GetBytes(f));
        }

        public override void Write(short f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else WriteReverse(BitConverter.GetBytes(f));
        }

        public override void Write(uint f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else WriteReverse(BitConverter.GetBytes(f));
        }

        public override void Write(ushort f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else WriteReverse(BitConverter.GetBytes(f));
        }

        private void WriteReverse(byte[] bytes) => base.Write(bytes.Reverse().ToArray());

        public override void Write(string f)
        {
            foreach (byte c in System.Text.Encoding.GetEncoding(1252).GetBytes(f))
                Write(c);
        }

        public void Write(AssetColor color)
        {
            base.Write(color.R);
            base.Write(color.G);
            base.Write(color.B);
            base.Write(color.A);
        }

        public void WriteMagic(string magic)
        {
            if (magic.Length != 4)
                throw new ArgumentException("Magic word must have 4 characters");
            var chars = magic.ToCharArray();
            Write(endianness == Endianness.Little ? chars : chars.Reverse().ToArray());
        }
    }
}
