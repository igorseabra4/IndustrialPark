using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialPark.SaveFile
{
    public class Section_GDAT : Block
    {
        [Description("Game will only accept the save file if the checksum matches OR you use the AR code which disables the checksum.")]
        public int Checksum { get; set; }

        public Section_GDAT()
        {
            sectionIdentifier = SaveFileSection.GDAT;
        }

        public Section_GDAT(BinaryReader binaryReader)
        {
            sectionIdentifier = SaveFileSection.GDAT;

            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();
            Checksum = binaryReader.ReadInt32().Switch();
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.GDAT;
            blockSize = 1;
            // bytesUsed is set by the controller
            listBytes.AddRange(Checksum.Reverse());
        }
    }
}
