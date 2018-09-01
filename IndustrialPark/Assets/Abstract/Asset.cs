using HipHopFile;
using System;
using System.Collections.Generic;
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

        public byte[] Data
        {
            get { return AHDR.containedFile; }
            set { AHDR.containedFile = value; }
        }

        public override string ToString()
        {
            return MainForm.alternateNamingMode ?
                "[" + AHDR.assetID.ToString("X8") + "] " + AHDR.ADBG.assetName :
                AHDR.ADBG.assetName + " [" + AHDR.assetID.ToString("X8") + "]";
        }

        protected float ReadFloat(int j)
        {
            return BitConverter.ToSingle(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        protected byte ReadByte(int j)
        {
            return AHDR.containedFile[j];
        }

        protected short ReadShort(int j)
        {
            return BitConverter.ToInt16(new byte[] {
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        protected ushort ReadUshort(int j)
        {
            return BitConverter.ToUInt16(new byte[] {
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        protected int ReadInt(int j)
        {
            return BitConverter.ToInt32(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);
        }

        protected uint ReadUInt(int j)
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

        protected void Write(int j, float value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, byte value)
        {
            AHDR.containedFile[j] = value;
        }

        protected void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 2; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, ushort value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 2; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();
            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected AssetEvent[] ReadEvents(int eventsStart)
        {
            byte amount = ReadByte(0x05);
            AssetEvent[] events = new AssetEvent[amount];
            for (int i = 0; i < amount; i++)
                events[i] = AssetEvent.From(Data, eventsStart + i * AssetEvent.sizeOfStruct);
            return events;
        }

        protected void WriteEvents(int eventsStart, AssetEvent[] value)
        {
            Write(0x05, (byte)value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                List<byte> bytes = Data.ToList();
                value[i].WriteTo(ref bytes, eventsStart + i * AssetEvent.sizeOfStruct);
                Data = bytes.ToArray();
            }
        }
    }
}