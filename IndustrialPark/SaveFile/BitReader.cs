using System;

namespace IndustrialPark.SaveFile
{
    public class BitReader
    {
        private readonly bool[] buffer;

        private int bitPosition;

        public BitReader(byte[] buffer, Endianness endian = Endianness.Little)
        {
            this.buffer = new bool[buffer.Length * 8];
            using (EndianBinaryReader reader = new EndianBinaryReader(buffer, endian))
            {
                int bitPosition = 0;
                while (!reader.EndOfStream)
                {
                    int value = reader.ReadInt32();
                    for (int i = 0; i < 32; i++)
                    {
                        this.buffer[bitPosition++] = (value & (1 << i)) != 0;
                    }
                }
            }
        }

        public bool[] Buffer => buffer;

        public int BitPosition => bitPosition;

        public bool ReadBit()
        {
            if (bitPosition >= buffer.Length)
                throw new InvalidOperationException("End of buffer.");
            return buffer[bitPosition++];
        }

        public uint ReadBits(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            uint value = 0;

            for (int i = 0; i < count; i++)
            {
                if (ReadBit())
                    value |= 1u << i;

                bitPosition++;
            }

            return value;
        }

        private byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];
            
            for (int i = 0; i < count; i++)
                bytes[i] = (byte)ReadBits(8);

            return bytes;
        }

        public byte ReadByte()
        {
            return (byte)ReadBits(8);
        }

        public ushort ReadUInt16()
        {
            byte[] bytes = ReadBytes(2);

            return (ushort)(
                bytes[0] |
                ((uint)bytes[1] << 8)
            );
        }

        public uint ReadUInt32()
        {
            byte[] bytes = ReadBytes(4);

            return
                ((uint)bytes[0]) |
                ((uint)bytes[1] << 8) |
                ((uint)bytes[2] << 16) |
                ((uint)bytes[3] << 24);
        }

        public float ReadFloat()
        {
            byte[] bytes = ReadBytes(4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public uint ReadB1()
        {
            return ReadBits(1);
        }

        public uint ReadB7()
        {
            return ReadBits(7);
        }
    }
}