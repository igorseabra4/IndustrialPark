using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public class Section_CNTR : Block
    {
        private readonly int dataSize;

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

        private Section_CNTR()
        {
        }

        public Section_CNTR(SaveFileGame game)
        {
            if (game == SaveFileGame.BFBB)
                dataSize = 0x1E0;
            else if (game != SaveFileGame.Scooby)
                dataSize = 0x190;
            else
                throw new Exception("Cannot create CNTR block for Scooby.");

            _data = new byte[dataSize];
        }

        public Section_CNTR(BinaryReader binaryReader, SaveFileGame game)
        {
            if (game == SaveFileGame.BFBB)
                dataSize = 0x1E0;
            else if (game != SaveFileGame.Scooby)
                dataSize = 0x190;
            else
                throw new Exception("Cannot create CNTR block for Scooby.");

            sectionIdentifier = SaveFileSection.CNTR;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            _data = binaryReader.ReadBytes(dataSize);
            
            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }
        
        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.CNTR;

            int previousSize = listBytes.Count;
            
            listBytes.AddRange(_data);

            bytesUsed = listBytes.Count - previousSize;
            blockSize = bytesUsed;
        }
    }
}
