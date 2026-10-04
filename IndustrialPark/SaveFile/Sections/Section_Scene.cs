using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public class Section_Scene : Block
    {
        private const int dataSize = 0x190;

        [Description("Scene this entry refers to. Must have 4 characters.")]
        public string SceneID
        { 
            get => sceneSectionIdentifier; 
            set
            {
                if (value.Length == 4)
                    sceneSectionIdentifier = value;
                else
                    throw new InvalidDataException("Scene ID must have 4 characters");
            }
        }

        private byte[] _data;
        public byte[] Data
        {
            get => _data;
            set
            {
                if (value.Length == dataSize)
                    _data = value;
                else
                    throw new InvalidDataException($"Data must be {dataSize} bytes long");
            }
        }

        public string BinaryData
        {
            get
            {
                string result = "";
                foreach (byte b in _data)
                    result += Convert.ToString(b, 2).PadLeft(8, '0');
                return result;
            }
            set
            {
                List<byte> result = new List<byte>(dataSize);
                if (value.Length == 8 * dataSize)
                {
                    for (int i = 0; i < 8 * dataSize; i += 8)
                        result.Add(Convert.ToByte(new string(value.Skip(i).Take(8).ToArray()), 2));
                    _data = result.ToArray();
                }
                else
                    throw new InvalidDataException($"Data must be {dataSize} bytes long");
            }
        }

        public Section_Scene()
        {
            sectionIdentifier = SaveFileSection.Scene;
            sceneSectionIdentifier = "AA00";
            _data = new byte[dataSize];
        }

        public Section_Scene(BinaryReader binaryReader, string sceneID)
        {
            sectionIdentifier = SaveFileSection.Scene;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            sceneSectionIdentifier = sceneID;
            _data = binaryReader.ReadBytes(dataSize);
            
            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }
        
        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.Scene;

            int previousSize = listBytes.Count;
            
            listBytes.AddRange(_data);

            bytesUsed = listBytes.Count - previousSize;
            blockSize = bytesUsed;
        }
    }
}
