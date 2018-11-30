using HipHopFile;
using System;
using System.ComponentModel;
using System.Linq;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class Asset
    {
        public static int Offset { get { return currentGame == Game.BFBB ? 0x00 : -0x04; } }

        public Section_AHDR AHDR;
        public bool isSelected;
        public bool isInvisible = false;

        public Asset(Section_AHDR AHDR)
        {
            this.AHDR = AHDR;
        }
        
        [Category("Data")]
        public byte[] Data
        {
            get { return AHDR.data; }
            set { AHDR.data = value; }
        }

        public override string ToString()
        {
            return MainForm.alternateNamingMode ?
                "[" + AHDR.assetID.ToString("X8") + "] " + AHDR.ADBG.assetName :
                AHDR.ADBG.assetName + " [" + AHDR.assetID.ToString("X8") + "]";
        }
        
        protected float ReadFloat(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToSingle(new byte[] {
                AHDR.data[j + 3],
                AHDR.data[j + 2],
                AHDR.data[j + 1],
                AHDR.data[j] }, 0);

            return BitConverter.ToSingle(AHDR.data, j);
        }

        protected byte ReadByte(int j)
        {
            return AHDR.data[j];
        }

        protected short ReadShort(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToInt16(new byte[] {
                AHDR.data[j + 1],
                AHDR.data[j] }, 0);

            return BitConverter.ToInt16(AHDR.data, j);
        }

        protected ushort ReadUshort(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt16(new byte[] {
                AHDR.data[j + 1],
                AHDR.data[j] }, 0);

            return BitConverter.ToUInt16(AHDR.data, j);
        }

        protected int ReadInt(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToInt32(new byte[] {
                AHDR.data[j + 3],
                AHDR.data[j + 2],
                AHDR.data[j + 1],
                AHDR.data[j] }, 0);

            return BitConverter.ToInt32(AHDR.data, j);
        }

        protected uint ReadUInt(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt32(new byte[] {
                AHDR.data[j + 3],
                AHDR.data[j + 2],
                AHDR.data[j + 1],
                AHDR.data[j] }, 0);

            return BitConverter.ToUInt32(AHDR.data, j);
        }
        
        protected void Write(int j, float value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                AHDR.data[j + i] = split[i];
        }

        protected void Write(int j, byte value)
        {
            AHDR.data[j] = value;
        }

        protected void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                AHDR.data[j + i] = split[i];
        }

        protected void Write(int j, ushort value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                AHDR.data[j + i] = split[i];
        }

        protected void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                AHDR.data[j + i] = split[i];
        }

        protected void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                AHDR.data[j + i] = split[i];
        }

        protected static uint Mask(uint bit)
        {
            return (uint)Math.Pow(2, bit);
        }

        protected static uint InvMask(uint bit)
        {
            return uint.MaxValue - Mask(bit);
        }

        public virtual bool HasReference(uint assetID) => false;
    }
}