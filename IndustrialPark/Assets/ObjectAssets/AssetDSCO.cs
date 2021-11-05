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

    public class DiscoPattern
    {
        public DiscoTileState[] Pattern { get; set; }
        public DiscoPattern()
        {
            Pattern = new DiscoTileState[0];
        }
        public DiscoPattern(int count)
        {
            Pattern = new DiscoTileState[count];
        }
        public void Fix(int amountOfTiles)
        {
            while (amountOfTiles % 4 != 0)
                amountOfTiles++;
            if (Pattern.Length != amountOfTiles)
            {
                var pattern = Pattern.ToList();
                while (pattern.Count < amountOfTiles)
                    pattern.Add(DiscoTileState.Off);
                while (pattern.Count > amountOfTiles)
                    pattern.RemoveAt(pattern.Count - 1);
                Pattern = pattern.ToArray();
            }
        }
    }

    public class AssetDSCO : BaseAsset
    {
        private const string categoryName = "Disco Floor";
        private const string discoDesc = "You can add/remove patterns as you want. Make sure each pattern has the correct amount of tiles. When saving, for each pattern, missing tiles will be added (set to Off) and extra tiles will be removed.";

        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor("Loop", "Enabled");
        [Category(categoryName)]
        public float TimeYellow { get; set; }
        [Category(categoryName)]
        public float TimeRed { get; set; }
        [Category(categoryName), Description(discoDesc)]
        public int AmountOfTiles { get; set; }
        [Category(categoryName)]
        public string TileName_FirstWhite { get; set; }
        [Category(categoryName)]
        public string TileName_FirstYellow { get; set; }
        [Category(categoryName)]
        public string TileName_FirstRed { get; set; }
        [Category(categoryName), Description(discoDesc)]
        public DiscoPattern[] Patterns { get; set; }

        public AssetDSCO(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Flags.FlagValueInt = reader.ReadUInt32();
                TimeYellow = reader.ReadSingle();
                TimeRed = reader.ReadSingle();
                int OffPrefixOffset = reader.ReadInt32() + 8;
                int TransitionPrefixOffset = reader.ReadInt32() + 8;
                int OnPrefixOffset = reader.ReadInt32() + 8;
                AmountOfTiles = reader.ReadInt32();
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

                Patterns = new DiscoPattern[AmountOfPhases];
                for (int i = 0; i < Patterns.Length; i++)
                {
                    reader.BaseStream.Position = PhaseOffsets[i];

                    var counter = AmountOfTiles;
                    while (counter % 4 != 0)
                        counter++;

                    Patterns[i] = new DiscoPattern(counter);
                    int k = 0;
                    while (k < AmountOfTiles)
                    {
                        byte entry = reader.ReadByte();
                        for (int j = 0; j < 4; j++)
                        {
                            int mask = 0b00000011 << (2 * j);
                            Patterns[i].Pattern[k++] = (DiscoTileState)((entry & mask) >> (2 * j));
                        }
                    }
                }
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            byte fillerByte = 0;
            if (game == Game.Incredibles)
                fillerByte = 0xCD;
            
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));

                for (int i = 0; i < 9; i++)
                    writer.Write(0);

                int OffPrefixOffset = (int)writer.BaseStream.Position - 8;
                WriteString(TileName_FirstWhite, writer, fillerByte);

                int TransitionPrefixOffset = (int)writer.BaseStream.Position - 8;
                WriteString(TileName_FirstYellow, writer, fillerByte);

                int OnPrefixOffset = (int)writer.BaseStream.Position - 8;
                WriteString(TileName_FirstRed, writer, fillerByte);

                int StatesOffset = (int)writer.BaseStream.Position - 8;

                for (int i = 0; i < Patterns.Length; i++)
                    writer.Write(0); // int[] Phases

                int[] PhaseOffsets = new int[Patterns.Length];

                for (int i = 0; i < Patterns.Length; i++)
                {
                    PhaseOffsets[i] = (int)writer.BaseStream.Position - 8;

                    Patterns[i].Fix(AmountOfTiles);

                    for (int j = 0; j < Patterns[i].Pattern.Length; j += 4)
                    {
                        byte entry = 0;
                        for (int k = 0; k < 4; k++)
                            if (j + k < Patterns[i].Pattern.Length)
                                entry |= (byte)(((int)Patterns[i].Pattern[j + k]) << (2 * k));
                        writer.Write(entry);
                    }
                }

                while (writer.BaseStream.Position % 4 != 0)
                    writer.Write(fillerByte);

                writer.BaseStream.Position = baseHeaderEndPosition;
                writer.Write(Flags.FlagValueInt);
                writer.Write(TimeYellow);
                writer.Write(TimeRed);
                writer.Write(OffPrefixOffset);
                writer.Write(TransitionPrefixOffset);
                writer.Write(OnPrefixOffset);
                writer.Write(AmountOfTiles);
                writer.Write(StatesOffset);
                writer.Write(Patterns.Length);

                writer.BaseStream.Position = StatesOffset + 8;

                for (int i = 0; i < PhaseOffsets.Length; i++)
                    writer.Write(PhaseOffsets[i]);

                writer.BaseStream.Position = writer.BaseStream.Length;
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
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

        private void WriteString(string writeString, EndianBinaryWriter writer, byte fillerByte)
        {
            foreach (char i in writeString)
                writer.Write(i);

            writer.Write((byte)0);
            while (writer.BaseStream.Length % 4 != 0)
                writer.Write(fillerByte);
        }
    }
}