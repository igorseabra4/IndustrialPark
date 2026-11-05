using IndustrialPark.AssetEditorColors;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Text;

namespace IndustrialPark;

public enum Endianness
{
    Little,
    Big,
    Unknown
}

public class EndianBinaryReader : BinaryReader
{
    public Endianness endianness;

    private static readonly Encoding Encoding1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252)!;

    public EndianBinaryReader(byte[] data, Endianness endianness) : base(new MemoryStream(data))
    {
        this.endianness = endianness;
    }

    public override float ReadSingle()
    {
        if (endianness == Endianness.Little)
            return base.ReadSingle();

        return BitConverter.Int32BitsToSingle(ReadInt32());
    }

    public override short ReadInt16()
    {
        if (endianness == Endianness.Little)
            return base.ReadInt16();

        Span<byte> buffer = stackalloc byte[2];
        BaseStream.ReadExactly(buffer);

        return BinaryPrimitives.ReadInt16BigEndian(buffer);
    }

    public override int ReadInt32()
    {
        if (endianness == Endianness.Little)
            return base.ReadInt32();

        Span<byte> buffer = stackalloc byte[4];
        BaseStream.ReadExactly(buffer);

        return BinaryPrimitives.ReadInt32BigEndian(buffer);
    }

    public override ushort ReadUInt16()
    {
        if (endianness == Endianness.Little)
            return base.ReadUInt16();

        Span<byte> buffer = stackalloc byte[2];
        BaseStream.ReadExactly(buffer);

        return BinaryPrimitives.ReadUInt16BigEndian(buffer);
    }

    public override uint ReadUInt32()
    {
        if (endianness == Endianness.Little)
            return base.ReadUInt32();

        Span<byte> buffer = stackalloc byte[4];
        BaseStream.ReadExactly(buffer);

        return BinaryPrimitives.ReadUInt32BigEndian(buffer);
    }

    public uint PeekUInt32()
    {
        uint value = ReadUInt32();
        BaseStream.Position -= 4;
        return value;
    }

    public bool ReadByteBool() => ReadByte() != 0;

    public bool ReadInt16Bool() => ReadInt16() != 0;

    public bool ReadInt32Bool() => ReadInt32() != 0;

    public string ReadString(int length) => Encoding1252.GetString(ReadBytes(length));

    public AssetColor ReadColor() => new AssetColor(ReadByte(), ReadByte(), ReadByte(), ReadByte());

    public bool EndOfStream => BaseStream.Position == BaseStream.Length;
}

public class EndianBinaryWriter : BinaryWriter
{
    public Endianness endianness;

    private static readonly Encoding Encoding1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252)!;

    public EndianBinaryWriter(Endianness endianness) : base(new MemoryStream())
    {
        this.endianness = endianness;
    }

    public byte[] ToArray() => ((MemoryStream)BaseStream).ToArray();

    public override void Write(float f)
    {
        if (endianness == Endianness.Little)
        {
            base.Write(f);
            return;
        }

        Write(BitConverter.SingleToInt32Bits(f));
    }

    public override void Write(int f)
    {
        if (endianness == Endianness.Little)
        {
            base.Write(f);
            return;
        }

        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(buffer, f);
        BaseStream.Write(buffer);
    }

    public override void Write(short f)
    {
        if (endianness == Endianness.Little)
        {
            base.Write(f);
            return;
        }

        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteInt16BigEndian(buffer, f);
        BaseStream.Write(buffer);
    }

    public override void Write(uint f)
    {
        if (endianness == Endianness.Little)
        {
            base.Write(f);
            return;
        }

        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(buffer, f);
        BaseStream.Write(buffer);
    }

    public override void Write(ushort f)
    {
        if (endianness == Endianness.Little)
        {
            base.Write(f);
            return;
        }

        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, f);
        BaseStream.Write(buffer);
    }

    public override void Write(string f)
    {
        byte[] bytes = Encoding1252.GetBytes(f);
        BaseStream.Write(bytes);
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

    public void WritePaddedString(string text, int count)
    {
        for (int i = 0; i < count; i++)
            Write((byte)(i < text.Length ? text[i] : 0));
    }
}