using System;
using System.Collections.Generic;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public struct PICKentry
    {
        public int unknown1;
        public int unknown2;
        public int unknown3;
        public int unknown4;
        public int unknown5;
    }

    public class AssetPICK : Asset
    {
        public int pickAmount;
        public Dictionary<int, PICKentry> pickEntries;
        public static AssetPICK pick;

        public AssetPICK(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(bool defaultMode = true)
        {
            pickEntries = new Dictionary<int, PICKentry>();
            pickAmount = Switch(BitConverter.ToInt32(AHDR.containedFile, 0x4));
            for (int i = 0; i < pickAmount; i++)
            {
                PICKentry entry = new PICKentry()
                {
                    unknown1 = Switch(BitConverter.ToInt32(AHDR.containedFile, 8 + i * 0x14)),
                    unknown2 = Switch(BitConverter.ToInt32(AHDR.containedFile, 12 + i * 0x14)),
                    unknown3 = Switch(BitConverter.ToInt32(AHDR.containedFile, 16 + i * 0x14)),
                    unknown4 = Switch(BitConverter.ToInt32(AHDR.containedFile, 20 + i * 0x14)),
                    unknown5 = Switch(BitConverter.ToInt32(AHDR.containedFile, 24 + i * 0x14)),
                };

                pickEntries.Add(entry.unknown1, entry);
                pick = this;
            }
        }
    }
}