using System;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public static class ConverterFunctions
    {
        public static float Switch(float a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
                return BitConverter.ToSingle(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public static int Switch(int a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
                return BitConverter.ToInt32(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public static uint Switch(uint a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt32(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public static short Switch(short a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
                return BitConverter.ToInt16(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }

        public static ushort Switch(ushort a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
                return BitConverter.ToUInt16(BitConverter.GetBytes(a).Reverse().ToArray(), 0);
            return a;
        }
    }
}
