using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public class Section_PLYR : Block
    {
        private const int dataSize = 0x148;

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

        public Section_PLYR()
        {
            sectionIdentifier = SaveFileSection.PLYR;            
            _data = new byte[dataSize];
        }

        public Section_PLYR(BinaryReader binaryReader)
        {
            sectionIdentifier = SaveFileSection.PLYR;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            _data = binaryReader.ReadBytes(dataSize);
            
            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }
        
        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.PLYR;

            int previousSize = listBytes.Count;
            
            listBytes.AddRange(_data);

            bytesUsed = listBytes.Count - previousSize;
            blockSize = bytesUsed;
        }
    }
}
