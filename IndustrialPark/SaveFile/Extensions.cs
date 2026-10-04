using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public static class Extensions
    {
        public static bool dontReverse;

        public static IEnumerable<byte> Reverse(this int value) => dontReverse ?
            BitConverter.GetBytes(value) :
            BitConverter.GetBytes(value).Reverse();

        public static IEnumerable<byte> Reverse(this float value) => dontReverse ? 
            BitConverter.GetBytes(value) : 
            BitConverter.GetBytes(value).Reverse();

        public static int Switch(this int value) => dontReverse ? 
            value :
            BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

        public static float Switch(this float value) => dontReverse ? 
            value : 
            BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
    }
}
