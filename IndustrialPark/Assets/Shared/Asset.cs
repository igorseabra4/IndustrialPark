using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class Asset : EndianConvertible
    {
        [Browsable(false)]
        public int Offset => currentGame == Game.BFBB ? 0x00 : -0x04;
        public static int DataSizeOffset(Game currentGame) => currentGame == Game.BFBB ? 0x00 : -0x04;

        public Section_AHDR AHDR;
        public bool isSelected;
        public bool isInvisible = false;
        public Game currentGame;
        public Platform currentPlatform;

        public Asset(Section_AHDR AHDR, Game game, Platform platform) : base(EndianConverter.PlatformEndianness(platform))
        {
            this.AHDR = AHDR;
            this.currentGame = game;
            this.currentPlatform = platform;
        }

        [Category("Data")]
        public byte[] Data
        {
            get => AHDR.data;
            set => AHDR.data = value; 
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
                Data[j + 3],
                Data[j + 2],
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToSingle(Data, j);
        }

        protected byte ReadByte(int j)
        {
            return Data[j];
        }

        protected short ReadShort(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToInt16(new byte[] {
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToInt16(Data, j);
        }

        protected ushort ReadUShort(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt16(new byte[] {
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToUInt16(Data, j);
        }

        protected int ReadInt(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToInt32(new byte[] {
                Data[j + 3],
                Data[j + 2],
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToInt32(Data, j);
        }

        protected uint ReadUInt(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt32(new byte[] {
                Data[j + 3],
                Data[j + 2],
                Data[j + 1],
                Data[j] }, 0);

            return BitConverter.ToUInt32(Data, j);
        }
        
        protected void Write(int j, float value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                Data[j + i] = split[i];
        }

        protected void Write(int j, byte value)
        {
            Data[j] = value;
        }

        protected void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                Data[j + i] = split[i];
        }

        protected void Write(int j, ushort value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                Data[j + i] = split[i];
        }

        protected void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                Data[j + i] = split[i];
        }

        protected void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
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

        public virtual bool HasReference(uint assetID) => false;

        public virtual void Verify(ref List<string> result) { }

        public static void Verify(uint assetID, ref List<string> result)
        {
            if (assetID != 0 && !Program.MainForm.AssetExists(assetID))
                result.Add("Referenced asset 0x" + assetID.ToString("X8") + " was not found in any open archive.");
        }
    }
}