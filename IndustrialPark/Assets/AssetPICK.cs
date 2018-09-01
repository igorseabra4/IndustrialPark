using System;
using System.Collections.Generic;
using HipHopFile;

namespace IndustrialPark
{
    public struct PICKentry
    {
        public uint unknown1;
        public uint unknown2;
        public uint unknown3;
        public uint unknown4;
        public uint unknown5;
    }

    public class AssetPICK : Asset
    {
        public int pickAmount;
        public Dictionary<uint, PICKentry> pickEntries;
        public static AssetPICK pick;

        public AssetPICK(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            pickEntries = new Dictionary<uint, PICKentry>();
            pickAmount = ReadInt(0x4);

            for (int i = 0; i < pickAmount; i++)
            {
                PICKentry entry = new PICKentry()
                {
                    unknown1 = ReadUInt(8 + i * 0x14),
                    unknown2 = ReadUInt(12 + i * 0x14),
                    unknown3 = ReadUInt(16 + i * 0x14),
                    unknown4 = ReadUInt(20 + i * 0x14),
                    unknown5 = ReadUInt(24 + i * 0x14)
                };

                pickEntries.Add(entry.unknown1, entry);
                pick = this;
            }
        }
    }
}