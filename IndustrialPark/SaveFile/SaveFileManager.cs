using HipHopFile;
using IndustrialPark.SaveFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.SaveFile
{
    public class SaveFileManager
    {
        private byte[] gciData;
        private byte[] xboxPostData;
        public List<Block> Blocks { get; set; }
        public bool IsGciEmpty => gciData == null || gciData.Length == 0;

        public void ReadFile(string fileName, SaveFileGame game, Platform platform)
        {
            ReadFile(new BinaryReader(new FileStream(fileName, FileMode.Open)), game, platform);
        }

        public void ReadFile(byte[] file, SaveFileGame game, Platform platform)
        {
            ReadFile(new BinaryReader(new MemoryStream(file)), game, platform);
        }

        public int gciDataSize(SaveFileGame game)
        {
            switch (game)
            {
                case SaveFileGame.Movie: 
                case SaveFileGame.Scooby:
                    return 0x4040;
                case SaveFileGame.BFBB: 
                case SaveFileGame.Incredibles:
                    return 0x6040;
            }
            return 0;
        }

        public void ReadFile(BinaryReader binaryReader, SaveFileGame game, Platform platform)
        {
            if (platform == Platform.GameCube)
            {
                Extensions.dontReverse = false;

                switch (game)
                {
                    case SaveFileGame.BFBB:
                    case SaveFileGame.Incredibles:
                        gciData = binaryReader.ReadBytes(0x6040);
                        break;
                    case SaveFileGame.Movie:
                    case SaveFileGame.Scooby:
                        gciData = binaryReader.ReadBytes(0x4040);
                        break;
                }
            }
            else
            {
                gciData = null;
                Extensions.dontReverse = true; 
            }

            Blocks = new List<Block>();

            string currentSection;

            bool continueLoop = true;
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length && continueLoop)
            {
                currentSection = new string(binaryReader.ReadChars(4));
                if (platform != Platform.GameCube)
                    currentSection = new string(currentSection.Reverse().ToArray());

                switch (currentSection)
                {
                    case "CNTR":
                        Blocks.Add(new Section_CNTR(binaryReader, game));
                        break;
                    case "GDAT":
                        Blocks.Add(new Section_GDAT(binaryReader));
                        break;
                    case "LEDR":
                        Blocks.Add(new Section_LEDR(binaryReader, game));
                        break;
                    case "PLYR":
                        Blocks.Add(new Section_PLYR(binaryReader));
                        break;
                    case "PREF":
                        switch (game)
                        {
                            case SaveFileGame.Scooby:
                                Blocks.Add(new Section_PREF_Scoo(binaryReader));
                                break;
                            case SaveFileGame.Movie:
                            case SaveFileGame.Incredibles:
                                Blocks.Add(new Section_PREF_TSSM(binaryReader));
                                break;
                            case SaveFileGame.BFBB:
                                Blocks.Add(new Section_PREF_BFBB(binaryReader));
                                break;
                        }
                        break;
                    case "ROOM":
                        Blocks.Add(new Section_ROOM(binaryReader));
                        break;
                    case "SFIL":
                        Blocks.Add(new Section_SFIL(binaryReader));
                        if (platform == Platform.Xbox)
                        {
                            xboxPostData = binaryReader.ReadBytes(20);
                            continueLoop = false;
                        }
                        break;
                    case "SVID":
                        Blocks.Add(new Section_SVID(binaryReader));
                        break;
                    default:
                        if (currentSection != "\0\0\0\0")
                            Blocks.Add(new Section_Scene(binaryReader, currentSection));
                        else
                            continueLoop = false;
                        break;
                }
            }

            binaryReader.Close();
        }

        public void WriteFile(string fileName, SaveFileGame game, Platform platform, out string comment)
        {
            comment = null;
            BinaryWriter writer = new BinaryWriter(new FileStream(fileName, FileMode.Create));

            if (platform == Platform.GameCube)
            {
                Extensions.dontReverse = false;
                if (gciData != null)
                    writer.Write(gciData);
                else
                    comment = "Note: you are saving a file in the GameCube format, but since you're converting from another platform, the GCI data of the file is not present. " +
                    "You must add it yourself with a hex editor before the file is useable. " +
                    "Use an existing GCI file and replace the data starting at 0x" + gciDataSize(game).ToString("X4") + " with this file.";
            }
            else
                Extensions.dontReverse = true;

            int gdatUsedPos = 0;

            List<byte> bytes = new List<byte>();
            foreach (Block b in Blocks)
            {
                if (b is Section_GDAT)
                    gdatUsedPos = bytes.Count + 8;
                b.SetBytes(ref bytes);
            }

            if (platform == Platform.GameCube)
            {
                bytes[gdatUsedPos] = BitConverter.GetBytes(bytes.Count)[3];
                bytes[gdatUsedPos + 1] = BitConverter.GetBytes(bytes.Count)[2];
                bytes[gdatUsedPos + 2] = BitConverter.GetBytes(bytes.Count)[1];
                bytes[gdatUsedPos + 3] = BitConverter.GetBytes(bytes.Count)[0];
            }
            else
            {
                bytes[gdatUsedPos] = BitConverter.GetBytes(bytes.Count)[0];
                bytes[gdatUsedPos + 1] = BitConverter.GetBytes(bytes.Count)[1];
                bytes[gdatUsedPos + 2] = BitConverter.GetBytes(bytes.Count)[2];
                bytes[gdatUsedPos + 3] = BitConverter.GetBytes(bytes.Count)[3];
            }
            
            writer.Write(bytes.ToArray());

            if (platform == Platform.GameCube)
                for (int i = bytes.Count; i < 0xE000; i++)
                    writer.Write((byte)0);
            else if (platform == Platform.Xbox)
            {
                if (xboxPostData != null)
                    writer.Write(xboxPostData);
                else
                    comment = "Note: you are saving a file in the Xbox format, but since you're converting from another platform, the footer (20 bytes at the end of the file) is not present. " +
                    "You must add it yourself with a hex editor before the file is useable.";
            }

            writer.Close();
        }

        public void AddNew(SaveFileSection section, SaveFileGame game)
        {
            switch (section)
            {
                case SaveFileSection.CNTR:
                    Blocks.Add(new Section_CNTR(game));
                    break;
                case SaveFileSection.GDAT:
                    Blocks.Add(new Section_GDAT());
                    break;
                case SaveFileSection.LEDR:
                    Blocks.Add(new Section_LEDR(game));
                    break;
                case SaveFileSection.PLYR:
                    Blocks.Add(new Section_PLYR());
                    break;
                case SaveFileSection.PREF:
                    switch (game)
                    {
                        case SaveFileGame.Scooby:
                            Blocks.Add(new Section_PREF_Scoo());
                            break;
                        case SaveFileGame.Movie:
                        case SaveFileGame.Incredibles:
                            Blocks.Add(new Section_PREF_TSSM());
                            break;
                        case SaveFileGame.BFBB:
                            Blocks.Add(new Section_PREF_BFBB());
                            break;
                    }
                    break;
                case SaveFileSection.ROOM:
                    Blocks.Add(new Section_ROOM());
                    break;
                case SaveFileSection.SFIL:
                    Blocks.Add(new Section_SFIL());
                    break;
                case SaveFileSection.SVID:
                    Blocks.Add(new Section_SVID());
                    break;
                case SaveFileSection.Scene:
                    Blocks.Add(new Section_Scene());
                    break;
                default:
                    throw new Exception("Unknown section type");
            }
        }
    }
}
