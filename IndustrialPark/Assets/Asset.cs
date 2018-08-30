using HipHopFile;
using System;
using System.Linq;

namespace IndustrialPark
{
    public abstract class Asset
    {
        public Section_AHDR AHDR;
        public bool isSelected;

        public Asset(Section_AHDR AHDR)
        {
            this.AHDR = AHDR;
        }

        public abstract void Setup(SharpRenderer renderer, bool defaultMode);

        public override string ToString()
        {
            return AHDR.ADBG.assetName + " [" + AHDR.assetID.ToString("X8") + "]";
        }
        
        public float ReadFloat(int j)
        {
            return BitConverter.ToSingle(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        public byte ReadByte(int j)
        {
            return AHDR.containedFile[j];
        }

        public short ReadShort(int j)
        {
            return BitConverter.ToInt16(new byte[] {
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        public ushort ReadUshort(ushort j)
        {
            return BitConverter.ToUInt16(new byte[] {
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        public int ReadInt(int j)
        {
            return BitConverter.ToInt32(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        public uint ReadUInt(uint j)
        {
            try
            {
                return BitConverter.ToUInt32(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
            }
            catch
            {
                return 0;
            }
        }

        public void Write(int j, float value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        public void Write(int j, byte value)
        {
            AHDR.containedFile[j] = value;
        }

        public void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 2; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        public void Write(int j, ushort value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 2; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        public void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        public void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }
    }
}