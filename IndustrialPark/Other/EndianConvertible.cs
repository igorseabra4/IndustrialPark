using AssetEditorColors;
using HipHopFile;
using System;
using System.ComponentModel;
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
        public Endianness endianness { get; private set; }

        public EndianBinaryReader(byte[] data, Platform platform) : this(data, platform.Endianness()) { }

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
            BitConverter.ToInt16(BitConverter.GetBytes(base.ReadInt16()).Reverse().ToArray(), 0);

        public override int ReadInt32() =>
            (endianness == Endianness.Little) ?
            base.ReadInt32() :
            BitConverter.ToInt32(BitConverter.GetBytes(base.ReadInt32()).Reverse().ToArray(), 0);

        public override ushort ReadUInt16() =>
            (endianness == Endianness.Little) ?
            base.ReadUInt16() :
            BitConverter.ToUInt16(BitConverter.GetBytes(base.ReadUInt16()).Reverse().ToArray(), 0);

        public override uint ReadUInt32() =>
            (endianness == Endianness.Little) ?
            base.ReadUInt32() :
            BitConverter.ToUInt32(BitConverter.GetBytes(base.ReadUInt32()).Reverse().ToArray(), 0);

        public bool ReadByteBool() => base.ReadByte() != 0;

        public bool ReadInt16Bool() => base.ReadInt16() != 0;

        public bool ReadInt32Bool() => base.ReadInt32() != 0;

        public AssetColor ReadColor() => new AssetColor(ReadByte(), ReadByte(), ReadByte(), ReadByte());
        
        public bool EndOfStream => BaseStream.Position == BaseStream.Length;
    }

    public class EndianBinaryWriter : BinaryWriter
    {
        public Endianness endianness { get; private set; }

        public EndianBinaryWriter(Platform platform) : base(new MemoryStream())
        {
            endianness = platform.Endianness();
        }

        public EndianBinaryWriter(Endianness endianness) : base(new MemoryStream())
        {
            this.endianness = endianness;
        }

        public byte[] ToArray() => ((MemoryStream)BaseStream).ToArray();
        
        public override void Write(float f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                base.Write(BitConverter.GetBytes(f).Reverse().ToArray());
        }

        public override void Write(int f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                base.Write(BitConverter.GetBytes(f).Reverse().ToArray());
        }

        public override void Write(short f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                base.Write(BitConverter.GetBytes(f).Reverse().ToArray());
        }

        public override void Write(uint f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                base.Write(BitConverter.GetBytes(f).Reverse().ToArray());
        }

        public override void Write(ushort f)
        {
            if (endianness == Endianness.Little)
                base.Write(f);
            else
                base.Write(BitConverter.GetBytes(f).Reverse().ToArray());
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

    public abstract class EndianConvertibleWithData : EndianConvertible
    {
        public abstract byte[] Data { get; set; }

        [Browsable(false)]
        public Platform platform { get; protected set; }
        [Browsable(false)]
        public Game game { get; protected set; }
        public void SetGamePlatform(Game game, Platform platform)
        {
            this.game = game;
            this.platform = platform;
            endianness = platform.Endianness();
        }

        public EndianConvertibleWithData(Endianness endianness) : base(endianness) { }
        public EndianConvertibleWithData(Game game, Platform platform) : base(platform)
        {
            this.game = game;
            this.platform = platform;
        }

        public float ReadFloat(int j)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToSingle(new byte[] {
                Data[j + 3],
                Data[j + 2],
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToSingle(Data, j);
        }

        public byte ReadByte(int j)
        {
            return Data[j];
        }

        public short ReadShort(int j)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToInt16(new byte[] {
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToInt16(Data, j);
        }

        public ushort ReadUShort(int j)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToUInt16(new byte[] {
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToUInt16(Data, j);
        }

        public int ReadInt(int j)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToInt32(new byte[] {
                Data[j + 3],
                Data[j + 2],
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToInt32(Data, j);
        }

        public uint ReadUInt(int j)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToUInt32(new byte[] {
                Data[j + 3],
                Data[j + 2],
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToUInt32(Data, j);
        }

        public virtual void Write(int j, float value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                Data[j + i] = split[i];
        }

        protected virtual void Write(int j, byte value)
        {
            Data[j] = value;
        }

        protected virtual void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                Data[j + i] = split[i];
        }

        protected virtual void Write(int j, ushort value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                Data[j + i] = split[i];
        }

        protected virtual void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                Data[j + i] = split[i];
        }

        protected virtual void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                Data[j + i] = split[i];
        }

        protected static uint Mask(uint bit)
        {
            return (uint)Math.Pow(2, bit);
        }

        protected static uint InvMask(uint bit)
        {
            return uint.MaxValue - Mask(bit);
        }
    }

    public class EndianConvertible
    {
        protected Endianness endianness;

        public EndianConvertible(Endianness endianness)
        {
            this.endianness = endianness;
        }

        public EndianConvertible(Platform platform)
        {
            endianness = platform.Endianness();
        }

        public float Switch(float a)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToSingle(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public int Switch(int a)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToInt32(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public uint Switch(uint a)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToUInt32(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public short Switch(short a)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToInt16(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public ushort Switch(ushort a)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToUInt16(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public string Switch(string a)
        {
            if (endianness == Endianness.Big)
                return new string(a.Reverse().ToArray());
            return a;
        }
    }
}
