using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace IndustrialPark
{
    public class EntrySGRP
    {
        public AssetID Sound_AssetID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat04 { get; set; }
        public int UnknownInt08 { get; set; }
        public int UnknownInt0C { get; set; }

        public EntrySGRP()
        {
            Sound_AssetID = 0;
        }

        public static int SizeOfEntry => 0x10;

        public override string ToString() => $"[{Program.MainForm.GetAssetNameFromID(Sound_AssetID)}] - [{UnknownFloat04}]";
    }

    public class AssetSGRP : ObjectAsset
    {
        public AssetSGRP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySGRP i in SGRP_Entries)
                if (i.Sound_AssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (EntrySGRP i in SGRP_Entries)
            {
                if (i.Sound_AssetID == 0)
                    result.Add("SGRP entry with Sound_AssetID set to 0");
                Verify(i.Sound_AssetID, ref result);
            }
        }

        [Category("Sound Group")]
        public int UnknownInt08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category("Sound Group"), ReadOnly(true)]
        public byte AmountOfReferences
        {
            get => ReadByte(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte0D
        {
            get => ReadByte(0x0D);
            set => Write(0x0D, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte0E
        {
            get => ReadByte(0x0E);
            set => Write(0x0E, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte0F
        {
            get => ReadByte(0x0F);
            set => Write(0x0F, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte10
        {
            get => ReadByte(0x10);
            set => Write(0x10, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte11
        {
            get => ReadByte(0x11);
            set => Write(0x11, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte12
        {
            get => ReadByte(0x12);
            set => Write(0x12, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte13
        {
            get => ReadByte(0x13);
            set => Write(0x13, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Sound Group"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Sound Group")]
        public int UnknownInt1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        private int StartOfSGRPEntries => 0x20;

        [Category("Sound Group")]
        public EntrySGRP[] SGRP_Entries
        {
            get
            {
                List<EntrySGRP> entries = new List<EntrySGRP>();

                for (int i = 0; i < AmountOfReferences; i++)
                {
                    entries.Add(new EntrySGRP()
                    {
                        Sound_AssetID = ReadUInt(StartOfSGRPEntries + i * EntrySGRP.SizeOfEntry),
                        UnknownFloat04 = ReadFloat(StartOfSGRPEntries + i * EntrySGRP.SizeOfEntry + 0x04),
                        UnknownInt08 = ReadInt(StartOfSGRPEntries + i * EntrySGRP.SizeOfEntry + 0x08),
                        UnknownInt0C = ReadInt(StartOfSGRPEntries + i * EntrySGRP.SizeOfEntry + 0x0C)
                    });
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(Data.Take(StartOfSGRPEntries));

                foreach (EntrySGRP i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Sound_AssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.UnknownFloat04)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.UnknownInt08)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.UnknownInt0C)));
                }

                newData.AddRange(Data.Skip(StartOfSGRPEntries + AmountOfReferences * EntrySGRP.SizeOfEntry));

                Data = newData.ToArray();
                AmountOfReferences = (byte)value.Length;
            }
        }
    }
}