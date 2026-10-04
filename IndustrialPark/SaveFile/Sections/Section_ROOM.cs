using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public class Section_ROOM : Block
    {
        private string sceneID;
        [Description("Level to load game in. Must have 4 characters.")]
        public string SceneID
        {
            get => sceneID;
            set
            {
                if (value.Length == 4)
                    sceneID = value;
                else
                    throw new InvalidDataException("Scene ID must have 4 characters");
            }
        }

        public Section_ROOM()
        {
            sectionIdentifier = SaveFileSection.ROOM;
            SceneID = "AA00";
        }

        public Section_ROOM(BinaryReader binaryReader)
        {
            sectionIdentifier = SaveFileSection.ROOM;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            SceneID = new string(binaryReader.ReadChars(4));

            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.ROOM;

            int previousSize = listBytes.Count;

            foreach (char c in SceneID)
                listBytes.Add((byte)c);

            bytesUsed = listBytes.Count - previousSize;

            for (int i = bytesUsed; i < 0x0C; i++)
                listBytes.Add(0xBF);

            blockSize = listBytes.Count - previousSize;
        }
    }
}
