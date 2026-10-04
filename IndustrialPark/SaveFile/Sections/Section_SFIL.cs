using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialPark.SaveFile
{
    public class Section_SFIL : Block
    {
        [Description("Unknown function.")]
        public string UnknownText { get; set; }
        [Description("Amount of bytes padding the end of the section. Recommended to leave untouched.")]
        public int PaddingSize
        {
            get => blockSize - bytesUsed;
            set => blockSize = value + bytesUsed;
        }

        public Section_SFIL()
        {
            sectionIdentifier = SaveFileSection.SFIL;
            UnknownText = "RyanNeil";
        }

        public Section_SFIL(BinaryReader binaryReader)
        {
            sectionIdentifier = SaveFileSection.SFIL;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            UnknownText = "";

            byte c = binaryReader.ReadByte();
            while (c != 0xBF)
            {
                UnknownText += (char)c;
                c = binaryReader.ReadByte();
            }

            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.SFIL;

            int previousSize = listBytes.Count;

            foreach (char c in UnknownText)
                listBytes.Add((byte)c);

            bytesUsed = listBytes.Count - previousSize;

            for (int i = bytesUsed; i < blockSize; i++)
                listBytes.Add(0xBF);

            blockSize = listBytes.Count - previousSize;
        }
    }
}
