using System;
using HipHopFile;

namespace IndustrialPark
{
    public static class ConverterFunctions
    {
        public static Single Switch(Single a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
            {
                byte[] b = BitConverter.GetBytes(a);
                b = new byte[] { b[3], b[2], b[1], b[0] };
                return BitConverter.ToSingle(b, 0);
            }
            else return a;
        }

        public static Int32 Switch(Int32 a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
            {
                byte[] b = BitConverter.GetBytes(a);
                b = new byte[] { b[3], b[2], b[1], b[0] };
                return BitConverter.ToInt32(b, 0);
            }
            else return a;
        }

        public static UInt32 Switch(UInt32 a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
            {
                byte[] b = BitConverter.GetBytes(a);
                b = new byte[] { b[3], b[2], b[1], b[0] };
                return BitConverter.ToUInt32(b, 0);
            }
            else return a;
        }

        public static Int16 Switch(Int16 a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
            {
                byte[] b = BitConverter.GetBytes(a);
                b = new byte[] { b[1], b[0] };
                return BitConverter.ToInt16(b, 0);
            }
            else return a;
        }

        public static UInt16 Switch(UInt16 a)
        {
            if (Functions.currentPlatform == Platform.GameCube)
            {
                byte[] b = BitConverter.GetBytes(a);
                b = new byte[] { b[1], b[0] };
                return BitConverter.ToUInt16(b, 0);
            }
            else return a;
        }
    }
}
