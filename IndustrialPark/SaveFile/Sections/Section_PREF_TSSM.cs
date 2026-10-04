using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialPark.SaveFile
{
    public class Section_PREF_TSSM : Block
    {
        public SoundMode SoundMode { get; set; }
        [Description("0 to 1.")]
        public float MusicVolume { get; set; }
        [Description("0 to 1.")]
        public float SfxVolume { get; set; }
        public Rumble Rumble { get; set; }
        [Description("Unknown function.")]
        public int Unknown { get; set; }

        public Section_PREF_TSSM()
        {
            sectionIdentifier = SaveFileSection.PREF;
        }

        public Section_PREF_TSSM(BinaryReader binaryReader)
        {
            sectionIdentifier = SaveFileSection.PREF;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            SoundMode = (SoundMode)binaryReader.ReadInt32().Switch();
            MusicVolume = binaryReader.ReadSingle().Switch();
            SfxVolume = binaryReader.ReadSingle().Switch();
            Rumble = (Rumble)binaryReader.ReadInt32().Switch();
            Unknown = binaryReader.ReadInt32().Switch();

            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }

        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.PREF;

            int previousSize = listBytes.Count;
            
            listBytes.AddRange(((int)SoundMode).Reverse());
            listBytes.AddRange(MusicVolume.Reverse());
            listBytes.AddRange(SfxVolume.Reverse());
            listBytes.AddRange(((int)Rumble).Reverse());
            listBytes.AddRange(Unknown.Reverse());

            bytesUsed = listBytes.Count - previousSize;

            for (int i = bytesUsed; i < 0x28; i++)
                listBytes.Add(0xBF);

            blockSize = listBytes.Count - previousSize;
        }
    }
}
