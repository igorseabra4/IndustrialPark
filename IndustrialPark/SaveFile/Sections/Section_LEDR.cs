using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public class Section_LEDR : Block
    {
        [Description("Text displayed on file select menu.")]
        public string LevelLabel { get; set; }
        [Description("Percentage of game complete. Can be any number.")]
        public int GameProgress { get; set; }
        [Description("Unknown function.")]
        public int Unknown1 { get; set; }
        [Description("Unknown function.")]
        public int Unknown2 { get; set; }
        [Description("Unknown function.")]
        public int Unknown3 { get; set; }
        [Description("Image displayed as thumbnail of save file. Always set to None in Scooby.")]
        public ThumbIcon ThumbnailIcon { get; set; }
        [Description("Unknown function.")]
        public int Unknown4 { get; set; }
        [Description("Unknown function.")]
        public string UnknownText { get; set; }

        private Section_LEDR()
        {
            sectionIdentifier = SaveFileSection.LEDR;
            LevelLabel = "None";
            UnknownText = "--TakeMeToYourLeader--";
        }

        public Section_LEDR(SaveFileGame game)
        {
            if (game == SaveFileGame.Scooby)
                ThumbnailIcon = ThumbIcon.None;
            sectionIdentifier = SaveFileSection.LEDR;
            LevelLabel = "None";
            UnknownText = "--TakeMeToYourLeader--";
        }

        public Section_LEDR(BinaryReader binaryReader, SaveFileGame game)
        {
            sectionIdentifier = SaveFileSection.LEDR;
            blockSize = binaryReader.ReadInt32().Switch();
            bytesUsed = binaryReader.ReadInt32().Switch();

            int sectionStart = (int)binaryReader.BaseStream.Position;

            LevelLabel = "";

            byte c = binaryReader.ReadByte();
            while (c != 0)
            {
                LevelLabel += (char)c;
                c = binaryReader.ReadByte();
            }

            binaryReader.BaseStream.Position = sectionStart + 64;

            GameProgress = binaryReader.ReadInt32().Switch();
            Unknown1 = binaryReader.ReadInt32().Switch();
            Unknown2 = binaryReader.ReadInt32().Switch();
            Unknown3 = binaryReader.ReadInt32().Switch();
            if (game == SaveFileGame.Scooby)
                ThumbnailIcon = ThumbIcon.None;
            else
            {
                ThumbnailIcon = (ThumbIcon)binaryReader.ReadByte();
                binaryReader.BaseStream.Position += 3;
                Unknown4 = binaryReader.ReadInt32().Switch();
            }

            UnknownText = "";
            
            c = binaryReader.ReadByte();
            while (c != 0xBF)
            {
                UnknownText += (char)c;
                c = binaryReader.ReadByte();
            }

            binaryReader.BaseStream.Position = blockSize + sectionStart;
        }
        
        public override void SetListBytes(ref List<byte> listBytes)
        {
            sectionIdentifier = SaveFileSection.LEDR;

            int previousSize = listBytes.Count;

            if (LevelLabel.Length > 64)
                LevelLabel = new string(LevelLabel.Take(64).ToArray());
            foreach (char c in LevelLabel)
                listBytes.Add((byte)c);
            for (int i = LevelLabel.Length; i < 64; i++)
                listBytes.Add(0);

            listBytes.AddRange(GameProgress.Reverse());
            listBytes.AddRange(Unknown1.Reverse());
            listBytes.AddRange(Unknown2.Reverse());
            listBytes.AddRange(Unknown3.Reverse());

            if (ThumbnailIcon != ThumbIcon.None)
            {
                listBytes.Add((byte)ThumbnailIcon);
                listBytes.Add(0);
                listBytes.Add(0);
                listBytes.Add(0);
            }

            listBytes.AddRange(Unknown4.Reverse());

            if (UnknownText.Length > 0xA8)
                UnknownText = new string(UnknownText.Take(0xA8).ToArray());
            foreach (char c in UnknownText)
                listBytes.Add((byte)c);

            bytesUsed = listBytes.Count - previousSize;

            for (int i = bytesUsed; i < 0x100; i++)
                listBytes.Add(0xBF);

            blockSize = listBytes.Count - previousSize;
        }
    }
}
