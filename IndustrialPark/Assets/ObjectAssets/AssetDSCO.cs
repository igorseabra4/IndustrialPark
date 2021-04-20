using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public enum DiscoTileState
    {
        Off = 0,
        On = 1,
        Random = 2,
        Unused = 3
    }

    public class AssetDSCO : BaseAsset
    {
        private const string categoryName = "Disco Floor";

        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor("Loop", "Enabled");
        [Category(categoryName)]
        public float TimeYellow { get; set; }
        [Category(categoryName)]
        public float TimeRed { get; set; }
        [Category(categoryName)]
        public string TileName_FirstWhite { get; set; }
        [Category(categoryName)]
        public string TileName_FirstYellow { get; set; }
        [Category(categoryName)]
        public string TileName_FirstRed { get; set; }
        [Category(categoryName)]
        public DiscoTileState[][] PatternController { get; set; }

        public AssetDSCO(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseHeaderEndPosition;

            Flags.FlagValueInt = reader.ReadUInt32();
            TimeYellow = reader.ReadSingle();
            TimeRed = reader.ReadSingle();
            int OffPrefixOffset = reader.ReadInt32() + 8;
            int TransitionPrefixOffset = reader.ReadInt32() + 8;
            int OnPrefixOffset = reader.ReadInt32() + 8;
            int AmountOfTiles = reader.ReadInt32();
            int StatesOffset = reader.ReadInt32() + 8;
            int AmountOfPhases = reader.ReadInt32();

            reader.BaseStream.Position = OffPrefixOffset;
            TileName_FirstWhite = ReadString(reader);

            reader.BaseStream.Position = TransitionPrefixOffset;
            TileName_FirstYellow = ReadString(reader);

            reader.BaseStream.Position = OnPrefixOffset;
            TileName_FirstRed = ReadString(reader);

            reader.BaseStream.Position = StatesOffset;
            int[] PhaseOffsets = new int[AmountOfPhases];
            for (int i = 0; i < PhaseOffsets.Length; i++)
                PhaseOffsets[i] = reader.ReadInt32() + 8;

            PatternController = new DiscoTileState[AmountOfPhases][];
            for (int i = 0; i < PatternController.Length; i++)
            {
                reader.BaseStream.Position = PhaseOffsets[i];
                PatternController[i] = new DiscoTileState[AmountOfTiles];

                for (int j = 0; j < PatternController[i].Length; j += 4)
                {
                    byte entry = reader.ReadByte();
                    for (int k = 0; k < 4; k++, j++)
                        if (j + k < PatternController[i].Length)
                        {
                            int mask = 0b00000011 << (2 * k);
                            PatternController[i][j + k] = (DiscoTileState)((entry & mask) >> (2 * k));
                        }
                }
            }
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            for (int i = 0; i < 9; i++)
                writer.Write(0);

            int OffPrefixOffset = (int)writer.BaseStream.Position - 8;
            WriteString(TileName_FirstWhite, writer);

            int TransitionPrefixOffset = (int)writer.BaseStream.Position - 8;
            WriteString(TileName_FirstYellow, writer);

            int OnPrefixOffset = (int)writer.BaseStream.Position - 8;
            WriteString(TileName_FirstRed, writer);
            
            int StatesOffset = (int)writer.BaseStream.Position - 8;

            for (int i = 0; i < PatternController.Length; i++)
                writer.Write(0); // int[] Phases

            int[] PhaseOffsets = new int[PatternController.Length];

            int AmountOfTiles = -1;

            for (int i = 0; i < PatternController.Length; i++)
            {
                PhaseOffsets[i] = (int)writer.BaseStream.Position - 8;

                if (AmountOfTiles == -1)
                    AmountOfTiles = PatternController[i].Length;
                else if (AmountOfTiles != PatternController[i].Length)
                    throw new ArgumentException("Disco patterns with variable amount of tiles");

                for (int j = 0; j < PatternController[i].Length; j += 4)
                {
                    byte entry = 0;
                    for (int k = 0; k < 4; k++, j++)
                        if (j + k < PatternController[i].Length)
                            entry |= (byte)(((int)PatternController[i][j + k]) << (2 * k));
                    writer.Write(entry);
                }
                while (writer.BaseStream.Position % 4 != 0)
                    writer.Write((byte)0);
            }

            writer.BaseStream.Position = baseHeaderEndPosition;

            writer.Write(Flags.FlagValueInt);
            writer.Write(TimeYellow);
            writer.Write(TimeRed);
            writer.Write(OffPrefixOffset);
            writer.Write(TransitionPrefixOffset);
            writer.Write(OnPrefixOffset);
            writer.Write(AmountOfTiles);
            writer.Write(StatesOffset);
            writer.Write(PatternController.Length);

            writer.BaseStream.Position = StatesOffset + 8;

            for (int i = 0; i < PatternController.Length; i++)
                writer.Write(PhaseOffsets[i]);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        private string ReadString(EndianBinaryReader reader)
        {
            var charList = new List<char>();

            do
            {
                charList.Add(reader.ReadChar());
            }
            while (charList.Last() != '\0');
            charList.Remove('\0');

            return new string(charList.ToArray());
        }

        private void WriteString(string writeString, EndianBinaryWriter writer)
        {
            foreach (char i in writeString)
                writer.Write(i);

            do writer.Write((byte)0);
            while (writer.BaseStream.Length % 4 != 0);
        }
    }
}