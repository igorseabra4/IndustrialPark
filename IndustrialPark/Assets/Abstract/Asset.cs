﻿using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public abstract class Asset
    {
        public static int Offset { get { return currentGame == Game.BFBB ? 0x00 : -0x04; } }

        public Section_AHDR AHDR;
        public bool isSelected;

        public Asset(Section_AHDR AHDR)
        {
            this.AHDR = AHDR;
        }
        
        [Category("Data")]
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
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToSingle(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);

            return BitConverter.ToSingle(AHDR.containedFile, j);
        }

        protected byte ReadByte(int j)
        {
            return AHDR.containedFile[j];
        }

        protected short ReadShort(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToInt16(new byte[] {
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);

            return BitConverter.ToInt16(AHDR.containedFile, j);
        }

        protected ushort ReadUshort(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt16(new byte[] {
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);

            return BitConverter.ToUInt16(AHDR.containedFile, j);
        }

        protected int ReadInt(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToInt32(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);

            return BitConverter.ToInt32(AHDR.containedFile, j);
        }

        protected uint ReadUInt(int j)
        {
            if (currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt32(new byte[] {
                AHDR.containedFile[j + 3],
                AHDR.containedFile[j + 2],
                AHDR.containedFile[j + 1],
                AHDR.containedFile[j] }, 0);

            return BitConverter.ToUInt32(AHDR.containedFile, j);
        }

        protected void Write(int j, float value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, byte value)
        {
            AHDR.containedFile[j] = value;
        }

        protected void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, ushort value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value).ToArray();

            if (currentPlatform == Platform.GameCube)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                AHDR.containedFile[j + i] = split[i];
        }

        protected AssetEvent[] ReadEvents(int eventsStart)
        {
            byte amount = ReadByte(0x05);
            AssetEvent[] events = new AssetEvent[amount];
            for (int i = 0; i < amount; i++)
                events[i] = AssetEvent.FromByteArray(Data, eventsStart + i * AssetEvent.sizeOfStruct);
            return events;
        }

        protected void WriteEvents(int eventsStart, AssetEvent[] value)
        {
            List<byte> newData = Data.Take(eventsStart).ToList();
            List<byte> bytesAfterEvents = Data.Skip(eventsStart + ReadByte(0x05) * AssetEvent.sizeOfStruct).ToList();

            for (int i = 0; i < value.Length; i++)
                newData.AddRange(value[i].ToByteArray());

            newData.AddRange(bytesAfterEvents);
            newData[0x05] = (byte)value.Length;

            Data = newData.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is Asset a)
                return a.GetHashCode() == GetHashCode();
            return false;
        }

        public override int GetHashCode()
        {
            return -1452371666 + AHDR.assetID.GetHashCode();
        }
    }
}