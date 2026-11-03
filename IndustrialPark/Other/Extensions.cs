using HipHopFile;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IndustrialPark
{
    public static class Extensions
    {
        public static Endianness Endianness(this Platform platform) =>
            platform == Platform.GameCube ? IndustrialPark.Endianness.Big : IndustrialPark.Endianness.Little;

        public static string ReadNullTerminatedString(BinaryReader reader)
        {
            List<char> charList = new List<char>();
            do
                charList.Add((char)reader.ReadByte());
            while (charList.Last() != '\0');
            charList.Remove('\0');

            if (charList.Count % 2 == 0)
                reader.BaseStream.Position += 1;

            return new string(charList.ToArray());
        }

        public static System.Drawing.Color ToColor(this SharpDX.Vector4 color) =>
            System.Drawing.Color.FromArgb((int)(color.W * 255), (int)(color.X * 255), (int)(color.Y * 255), (int)(color.Z * 255));
    }
}
