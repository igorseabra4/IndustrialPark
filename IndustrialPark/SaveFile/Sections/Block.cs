using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public abstract class Block
    {
        [Browsable(false)]
        public SaveFileSection sectionIdentifier { get; protected set; }
        protected int blockSize;
        protected int bytesUsed;

        protected string sceneSectionIdentifier;
        
        public void SetBytes(ref List<byte> listBytes)
        {
            int position = listBytes.Count();
            listBytes.AddRange(new byte[] {
                0, 0, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 0
            });

            SetListBytes(ref listBytes);
            
            if (sectionIdentifier != SaveFileSection.Scene)
                sceneSectionIdentifier = sectionIdentifier.ToString();

            if (Extensions.dontReverse)
            {
                listBytes[position + 0] = (byte)sceneSectionIdentifier[3];
                listBytes[position + 1] = (byte)sceneSectionIdentifier[2];
                listBytes[position + 2] = (byte)sceneSectionIdentifier[1];
                listBytes[position + 3] = (byte)sceneSectionIdentifier[0];
                listBytes[position + 4] = BitConverter.GetBytes(blockSize)[0];
                listBytes[position + 5] = BitConverter.GetBytes(blockSize)[1];
                listBytes[position + 6] = BitConverter.GetBytes(blockSize)[2];
                listBytes[position + 7] = BitConverter.GetBytes(blockSize)[3];
                listBytes[position + 8] = BitConverter.GetBytes(bytesUsed)[0];
                listBytes[position + 9] = BitConverter.GetBytes(bytesUsed)[1];
                listBytes[position + 10] = BitConverter.GetBytes(bytesUsed)[2];
                listBytes[position + 11] = BitConverter.GetBytes(bytesUsed)[3];
            }
            else
            {
                listBytes[position + 0] = (byte)sceneSectionIdentifier[0];
                listBytes[position + 1] = (byte)sceneSectionIdentifier[1];
                listBytes[position + 2] = (byte)sceneSectionIdentifier[2];
                listBytes[position + 3] = (byte)sceneSectionIdentifier[3];
                listBytes[position + 4] = BitConverter.GetBytes(blockSize)[3];
                listBytes[position + 5] = BitConverter.GetBytes(blockSize)[2];
                listBytes[position + 6] = BitConverter.GetBytes(blockSize)[1];
                listBytes[position + 7] = BitConverter.GetBytes(blockSize)[0];
                listBytes[position + 8] = BitConverter.GetBytes(bytesUsed)[3];
                listBytes[position + 9] = BitConverter.GetBytes(bytesUsed)[2];
                listBytes[position + 10] = BitConverter.GetBytes(bytesUsed)[1];
                listBytes[position + 11] = BitConverter.GetBytes(bytesUsed)[0];
            }
        }

        public abstract void SetListBytes(ref List<byte> listBytes);
    }
}
