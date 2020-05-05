using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace IndustrialPark
{
    public class AssetDSCO : BaseAsset
    {
        public AssetDSCO(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        [Category("Disco Floor")]
        public int UnknownInt08
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Disco Floor"), TypeConverter(typeof(FloatTypeConverter))]
        public float Speed_YellowToRed
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }

        [Category("Disco Floor"), TypeConverter(typeof(FloatTypeConverter))]
        public float Time_RedToWhite
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Disco Floor")]
        public int UnknownInt14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Disco Floor")]
        public int UnknownInt18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category("Disco Floor")]
        public int UnknownInt1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Disco Floor")]
        public int AmountOfVisibleTiles
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }

        [Category("Disco Floor")]
        public int UnknownInt24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }

        [Category("Disco Floor"), ReadOnly(true)]
        public int AmountOfPhases
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }

        private string ReadString(int position)
        {
            List<char> charList = new List<char>();

            do
            {
                charList.Add((char)Data[position]);
                position++;
            }
            while (charList.Last() != '\0');
            charList.Remove('\0');

            return new string(charList.ToArray());
        }

        private void WriteString(int position, string writeString)
        {
            string oldString = ReadString(position);
            int oldLength = oldString.Length + 4 - oldString.Length % 4;

            List<byte> before = Data.Take(position).ToList();
            List<byte> after = Data.Skip(position + oldLength).ToList();

            foreach (char i in writeString)
                before.Add((byte)i);

            if (writeString.Length % 4 == 0) before.AddRange(new byte[] { 0, 0, 0, 0 });
            if (writeString.Length % 4 == 1) before.AddRange(new byte[] { 0, 0, 0 });
            if (writeString.Length % 4 == 2) before.AddRange(new byte[] { 0, 0 });
            if (writeString.Length % 4 == 3) before.AddRange(new byte[] { 0 });

            before.AddRange(after);

            Data = before.ToArray();
        }

        private int TileName_FirstWhite_Position => 0x2C;

        [Category("Disco Floor")]
        public string TileName_FirstWhite
        {
            get => ReadString(TileName_FirstWhite_Position);
            set => WriteString(TileName_FirstWhite_Position, value);
        }

        private int TileName_FirstYellow_Position => TileName_FirstWhite_Position + TileName_FirstWhite.Length + 4 - TileName_FirstWhite.Length % 4;

        [Category("Disco Floor")]
        public string TileName_FirstYellow
        {
            get => ReadString(TileName_FirstYellow_Position);
            set => WriteString(TileName_FirstYellow_Position, value);
        }

        private int TileName_FirstRed_Position => TileName_FirstYellow_Position + TileName_FirstYellow.Length + 4 - TileName_FirstYellow.Length % 4;

        [Category("Disco Floor")]
        public string TileName_FirstRed
        {
            get => ReadString(TileName_FirstRed_Position);
            set => WriteString(TileName_FirstRed_Position, value);
        }

        private int AmountOfPhases_Position => TileName_FirstRed_Position + TileName_FirstRed.Length + 4 - TileName_FirstRed.Length % 4;

        [Category("Disco Floor")]
        public int[] Phases
        {
            get
            {
                List<int> ints = new List<int>();
                for (int i = AmountOfPhases_Position; i < AmountOfPhases_Position + 4 * AmountOfPhases; i += 4)
                    ints.Add(ReadInt(i));

                return ints.ToArray();
            }

            set
            {
                List<byte> before = Data.Take(AmountOfPhases_Position).ToList();
                List<byte> after = Data.Skip(PatternController_Position).ToList();

                foreach (int i in value)
                    before.AddRange(BitConverter.GetBytes(Switch(i)));

                before.AddRange(after);

                Data = before.ToArray();

                AmountOfPhases = value.Length;
            }
        }

        private int PatternController_Position => AmountOfPhases_Position + AmountOfPhases * 4;

        [Category("Disco Floor")]
        public byte[] PatternController
        {
            get
            {
                List<byte> bytes = new List<byte>();
                for (int i = PatternController_Position; i < Data.Length; i++)
                    bytes.Add(ReadByte(i));

                return bytes.ToArray();
            }

            set
            {
                List<byte> before = Data.Take(PatternController_Position).ToList();

                foreach (byte i in value)
                    before.Add(i);

                Data = before.ToArray();
            }
        }
    }
}