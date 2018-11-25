using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class Unknown2
    {
        public int Unknown_0 { get; set; }
        public int Unknown_4 { get; set; }
        public float Unknown_8 { get; set; }
        public short Unknown_C { get; set; }
        public short Unknown_E { get; set; }
        public int Unknown_10 { get; set; }
        public int Unknown_14 { get; set; }
        public int Unknown_18 { get; set; }
        public int Unknown_1C { get; set; }
    }

    public class Unknown3
    {
        public AssetID Unknown_0 { get; set; }
        public int Unknown_4 { get; set; }
        public int Unknown_8 { get; set; }
        public int Unknown_C { get; set; }
        public float Unknown_10 { get; set; }
        public int Unknown_14 { get; set; }
        public int Unknown_18 { get; set; }

        public Unknown3()
        {
            Unknown_0 = 0;
        }
    }

    public class AssetATBL : Asset
    {
        public AssetATBL(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (AssetID a in Animations)
                if (a == assetID)
                    return true;

            foreach (Unknown3 a in Entries_Unknown3)
                if (a.Unknown_0 == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        [Category("Animation Table")]
        public int ANIMCount
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category("Animation Table")]
        public int Count08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category("Animation Table")]
        public int Count0C
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Animation Table")]
        public int Unknown10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        private int AnimationsStart => 0x14;

        [Category("Animation Table")]
        public AssetID[] Animations
        {
            get
            {
                AssetID[] _otherMVPTs = new AssetID[ANIMCount];
                for (int i = 0; i < ANIMCount; i++)
                    _otherMVPTs[i] = ReadUInt(AnimationsStart + 4 * i);

                return _otherMVPTs;
            }
            set
            {
                List<byte> newData = Data.Take(AnimationsStart).ToList();
                List<byte> restOfOldData = Data.Skip(AnimationsStart + 4 * ANIMCount).ToList();

                foreach (AssetID i in value)
                {
                    if (currentPlatform == Platform.GameCube)
                        newData.AddRange(BitConverter.GetBytes(i).Reverse());
                    else
                        newData.AddRange(BitConverter.GetBytes(i));
                }

                newData.AddRange(restOfOldData);

                Data = newData.ToArray();

                ANIMCount = value.Length;
            }
        }

        private int ListUnknown2Start => AnimationsStart + ANIMCount * 4;

        [Category("Animation Table")]
        public Unknown2[] Entries_Unknown2
        {
            get
            {
                List<Unknown2> entries = new List<Unknown2>();

                for (int i = 0; i < Count08; i++)
                {
                    entries.Add(new Unknown2()
                    {
                        Unknown_0 = ReadInt(ListUnknown2Start + i * 0x20),
                        Unknown_4 = ReadInt(ListUnknown2Start + i * 0x20 + 0x4),
                        Unknown_8 = ReadFloat(ListUnknown2Start + i * 0x20 + 0x8),
                        Unknown_C = ReadShort(ListUnknown2Start + i * 0x20 + 0xC),
                        Unknown_E = ReadShort(ListUnknown2Start + i * 0x20 + 0xE),
                        Unknown_10 = ReadInt(ListUnknown2Start + i * 0x20 + 0x10),
                        Unknown_14 = ReadInt(ListUnknown2Start + i * 0x20 + 0x14),
                        Unknown_18 = ReadInt(ListUnknown2Start + i * 0x20 + 0x18),
                        Unknown_1C = ReadInt(ListUnknown2Start + i * 0x20 + 0x1C),
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(ListUnknown2Start).ToList();
                List<byte> restOfData = Data.Skip(ListUnknown3Start).ToList();

                foreach (Unknown2 i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_0)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_4)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_8)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_C)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_E)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_10)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_14)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_18)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_1C)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Count08 = value.Length;
            }
        }

        private int ListUnknown3Start => ListUnknown2Start + Count08 * 0x20;

        [Category("Animation Table")]
        public Unknown3[] Entries_Unknown3
        {
            get
            {
                List<Unknown3> entries = new List<Unknown3>();

                for (int i = 0; i < Count0C; i++)
                {
                    entries.Add(new Unknown3()
                    {
                        Unknown_0 = ReadUInt(ListUnknown3Start + i * 0x1C),
                        Unknown_4 = ReadInt(ListUnknown3Start + i * 0x1C + 0x4),
                        Unknown_8 = ReadInt(ListUnknown3Start + i * 0x1C + 0x8),
                        Unknown_C = ReadInt(ListUnknown3Start + i * 0x1C + 0xC),
                        Unknown_10 = ReadFloat(ListUnknown3Start + i * 0x1C + 0x10),
                        Unknown_14 = ReadInt(ListUnknown3Start + i * 0x1C + 0x14),
                        Unknown_18 = ReadInt(ListUnknown3Start + i * 0x1C + 0x18),
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(ListUnknown3Start).ToList();
                List<byte> restOfData = Data.Skip(ListUnknown4Start).ToList();

                foreach (Unknown3 i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_0)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_4)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_8)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_C)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_10)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_14)));
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i.Unknown_18)));
                }

                newData.AddRange(restOfData);

                Data = newData.ToArray();
                Count0C = value.Length;
            }
        }

        private int ListUnknown4Start => ListUnknown3Start + Count0C * 0x1C;

        [Category("Animation Table")]
        public int[] Entries_Unknown4
        {
            get
            {
                List<int> entries = new List<int>();

                for (int i = ListUnknown4Start; i < Data.Length; i += 4)
                    entries.Add(ReadInt(i));

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(ListUnknown4Start).ToList();

                foreach (int i in value)
                    newData.AddRange(BitConverter.GetBytes(ConverterFunctions.Switch(i)));
                
                Data = newData.ToArray();
            }
        }
    }
}