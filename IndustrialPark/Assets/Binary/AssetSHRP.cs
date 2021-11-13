using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSHRP : Asset
    {
        private const string categoryName = "Shrapnel";

        [Category(categoryName)]
        public int Unknown { get; set; }
        [Category(categoryName)]
        public AssetID AssetID_Internal { get; set; }
        [Category(categoryName)]
        public EntrySHRP[] SHRPEntries { get; set; }

        public AssetSHRP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                int amountOfEntries = reader.ReadInt32();
                AssetID_Internal = reader.ReadUInt32();
                Unknown = reader.ReadInt32();
                SHRPEntries = new EntrySHRP[amountOfEntries];

                for (int i = 0; i < SHRPEntries.Length; i++)
                {
                    int entryType = reader.ReadInt32();

                    EntrySHRP entry = null;

                    if (entryType == 3)
                    {
                        if (game == Game.BFBB)
                            entry = new EntrySHRP_Type3_BFBB(reader);
                        else if (game == Game.Incredibles)
                            entry = new EntrySHRP_Type3_TSSM(reader);
                    }
                    else if (entryType == 4)
                    {
                        if (game == Game.BFBB)
                            entry = new EntrySHRP_Type4_BFBB(reader);
                        else if (game == Game.Incredibles)
                            entry = new EntrySHRP_Type4_TSSM(reader);
                    }
                    else if (entryType == 5)
                    {
                        if (game == Game.BFBB)
                            entry = new EntrySHRP_Type5_BFBB(reader);
                        else if (game == Game.Incredibles)
                            entry = new EntrySHRP_Type5_TSSM(reader);
                    }
                    else if (entryType == 6)
                    {
                        if (game == Game.BFBB)
                            entry = new EntrySHRP_Type6_BFBB(reader);
                        else if (game == Game.Incredibles)
                            entry = new EntrySHRP_Type6_TSSM(reader);
                    }
                    else if (entryType == 8)
                        entry = new EntrySHRP_Type8(reader);
                    else if (entryType == 9)
                        entry = new EntrySHRP_Type9(reader);
                    else
                        throw new Exception("Unknown SHRP entry type " + entryType.ToString() + " found in asset " + ToString() + ". This SHRP asset cannot be edited by Industrial Park.");

                    SHRPEntries[i] = entry;
                }
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SHRPEntries.Length);
                writer.Write(AssetID_Internal);
                writer.Write(Unknown);
                foreach (var e in SHRPEntries)
                    writer.Write(e.Serialize(game, endianness));

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            try
            {
                foreach (EntrySHRP a in SHRPEntries)
                    if (a.HasReference(assetID))
                        return true;
            }
#if DEBUG
            catch (Exception e)
            {
                MessageBox.Show("Error searching for references: " + e.Message + ". It will be skipped on the search.");
            }
#else
            catch
            {
            }
#endif
            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntrySHRP a in SHRPEntries)
                switch (a.Type)
                {
                    case 3:
                        if (game == Game.BFBB)
                            Verify(((EntrySHRP_Type3_BFBB)a).PARE_AssetID, ref result);
                        else if (game == Game.Incredibles)
                            Verify(((EntrySHRP_Type3_TSSM)a).Unknown1F0, ref result);
                        break;
                    case 4:
                        if (game == Game.BFBB)
                        {
                            Verify(((EntrySHRP_Type4_BFBB)a).ModelAssetID, ref result);
                            Verify(((EntrySHRP_Type4_BFBB)a).UnknownAssetID74, ref result);
                        }
                        else if (game == Game.Incredibles)
                            Verify(((EntrySHRP_Type4_TSSM)a).ModelAssetID, ref result);
                        break;
                    case 6:
                        if (game == Game.BFBB)
                            Verify(((EntrySHRP_Type6_BFBB)a).SoundAssetID, ref result);
                        else if (game == Game.Incredibles)
                            Verify(((EntrySHRP_Type6_TSSM)a).SoundAssetID, ref result);
                        break;
                    case 9:
                        Verify(((EntrySHRP_Type9)a).UnknownAssetID18, ref result);
                        break;
                }
        }

        public void AddEntry(int type)
        {
            List<EntrySHRP> list = SHRPEntries.ToList();

            switch (type)
            {
                case 3:
                    if (game == Game.Incredibles)
                        list.Add(new EntrySHRP_Type3_TSSM());
                    else
                        list.Add(new EntrySHRP_Type3_BFBB());
                    break;
                case 4:
                    if (game == Game.Incredibles)
                        list.Add(new EntrySHRP_Type4_TSSM());
                    else
                        list.Add(new EntrySHRP_Type4_BFBB());
                    break;
                case 5:
                    if (game == Game.Incredibles)
                        list.Add(new EntrySHRP_Type5_TSSM());
                    else
                        list.Add(new EntrySHRP_Type5_BFBB());
                    break;
                case 6:
                    if (game == Game.Incredibles)
                        list.Add(new EntrySHRP_Type6_TSSM());
                    else
                        list.Add(new EntrySHRP_Type6_BFBB());
                    break;
                case 8:
                    list.Add(new EntrySHRP_Type8());
                    break;
                case 9:
                    list.Add(new EntrySHRP_Type9());
                    break;
            }

            SHRPEntries = list.ToArray();
        }
    }

    public abstract class EntrySHRP : GenericAssetDataContainer
    {
        [ReadOnly(true)]
        public int Type { get; set; }
        public AssetID Unknown04 { get; set; }
        public AssetID Unknown08 { get; set; }
        public AssetID Unknown0C { get; set; }
        public AssetSingle Unknown10 { get; set; }
        public AssetSingle Unknown14 { get; set; }

        public EntrySHRP(int type)
        {
            Type = type;
        }

        public EntrySHRP(int type, EndianBinaryReader reader)
        {
            Type = type;
            Unknown04 = reader.ReadUInt32();
            Unknown08 = reader.ReadUInt32();
            Unknown0C = reader.ReadUInt32();
            Unknown10 = reader.ReadSingle();
            Unknown14 = reader.ReadSingle();
        }

        public byte[] SerializeEntryShrpBase(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Type);
                writer.Write(Unknown04);
                writer.Write(Unknown08);
                writer.Write(Unknown0C);
                writer.Write(Unknown10);
                writer.Write(Unknown14);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (Unknown04 == assetID)
                return true;
            if (Unknown08 == assetID)
                return true;
            if (Unknown0C == assetID)
                return true;

            return false;
        }

        private const byte padB = 0xCD;

        protected void ReadPad(EndianBinaryReader reader, int count)
        {
            for (int i = 0; i < count; i++)
                if (reader.ReadByte() != padB)
                    throw new Exception("Error reading SHRP padding: non-padding byte found at " + (reader.BaseStream.Position - 1).ToString());
        }

        protected void WritePad(EndianBinaryWriter writer, int count)
        {
            for (int i = 0; i < count; i++)
                writer.Write(padB);
        }
    }

    public class EntrySHRP_Type3_BFBB : EntrySHRP
    {
        public int Unknown18 { get; set; }
        public int Unknown1C { get; set; }
        public AssetSingle Unknown20 { get; set; }
        public AssetSingle Unknown24 { get; set; }
        public AssetSingle Unknown28 { get; set; }
        public int Unknown3C { get; set; }
        public int Unknown40 { get; set; }
        public int Unknown44 { get; set; }
        public AssetSingle Unknown48 { get; set; }
        public AssetSingle Unknown4C { get; set; }
        public byte Unknown64 { get; set; }
        public short Unknown198 { get; set; }
        public short Unknown19A { get; set; }
        public AssetID PARE_AssetID { get; set; }
        public int Unknown1D0 { get; set; }

        public EntrySHRP_Type3_BFBB() : base(3) { }
        public EntrySHRP_Type3_BFBB(EndianBinaryReader reader) : base(3, reader)
        {
            Unknown18 = reader.ReadInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadSingle();
            Unknown24 = reader.ReadSingle();
            Unknown28 = reader.ReadSingle();
            ReadPad(reader, 0x10);
            Unknown3C = reader.ReadInt32();
            Unknown40 = reader.ReadInt32();
            Unknown44 = reader.ReadInt32();
            Unknown48 = reader.ReadSingle();
            Unknown4C = reader.ReadSingle();
            ReadPad(reader, 0x14);
            Unknown64 = reader.ReadByte();
            ReadPad(reader, 0x133);
            Unknown198 = reader.ReadInt16();
            Unknown19A = reader.ReadInt16();
            ReadPad(reader, 0x30);
            PARE_AssetID = reader.ReadUInt32();
            Unknown1D0 = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(Unknown18);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);
                WritePad(writer, 0x10);
                writer.Write(Unknown3C);
                writer.Write(Unknown40);
                writer.Write(Unknown44);
                writer.Write(Unknown48);
                writer.Write(Unknown4C);
                WritePad(writer, 0x14);
                writer.Write(Unknown64);
                WritePad(writer, 0x133);
                writer.Write(Unknown198);
                writer.Write(Unknown19A);
                WritePad(writer, 0x30);
                writer.Write(PARE_AssetID);
                writer.Write(Unknown1D0);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => PARE_AssetID == assetID || base.HasReference(assetID);
    }

    public class EntrySHRP_Type3_TSSM : EntrySHRP
    {
        public int Unknown18 { get; set; }
        public int Unknown1C { get; set; }
        public AssetSingle Unknown20 { get; set; }
        public AssetSingle Unknown24 { get; set; }
        public AssetSingle Unknown28 { get; set; }
        public int Unknown3C { get; set; }
        public int Unknown40 { get; set; }
        public int Unknown44 { get; set; }
        public AssetSingle Unknown48 { get; set; }
        public AssetSingle Unknown4C { get; set; }
        public int Unknown50 { get; set; }
        public int Unknown64 { get; set; }
        public byte Unknown6C { get; set; }
        public short Unknown1A0 { get; set; }
        public short Unknown1A2 { get; set; }
        public byte Unknown1EC { get; set; }
        public byte Unknown1ED { get; set; }
        public byte Unknown1EE { get; set; }
        public byte Unknown1EF { get; set; }
        public AssetID Unknown1F0 { get; set; }
        public int Unknown1F4 { get; set; }

        public EntrySHRP_Type3_TSSM() : base(3) { }
        public EntrySHRP_Type3_TSSM(EndianBinaryReader reader) : base(3, reader)
        {
            Unknown18 = reader.ReadInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadSingle();
            Unknown24 = reader.ReadSingle();
            Unknown28 = reader.ReadSingle();
            ReadPad(reader, 0x10);
            Unknown3C = reader.ReadInt32();
            Unknown40 = reader.ReadInt32();
            Unknown44 = reader.ReadInt32();
            Unknown48 = reader.ReadSingle();
            Unknown4C = reader.ReadSingle();
            Unknown50 = reader.ReadInt32();
            ReadPad(reader, 0x10);
            Unknown64 = reader.ReadInt32();
            ReadPad(reader, 0x4);
            Unknown6C = reader.ReadByte();
            ReadPad(reader, 0x133);
            Unknown1A0 = reader.ReadInt16();
            Unknown1A2 = reader.ReadInt16();
            ReadPad(reader, 0x48);
            Unknown1EC = reader.ReadByte();
            Unknown1ED = reader.ReadByte();
            Unknown1EE = reader.ReadByte();
            Unknown1EF = reader.ReadByte();
            Unknown1F0 = reader.ReadUInt32();
            Unknown1F4 = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(Unknown18);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);
                WritePad(writer, 0x10);
                writer.Write(Unknown3C);
                writer.Write(Unknown40);
                writer.Write(Unknown44);
                writer.Write(Unknown48);
                writer.Write(Unknown4C);
                writer.Write(Unknown50);
                WritePad(writer, 0x10);
                writer.Write(Unknown64);
                WritePad(writer, 0x4);
                writer.Write(Unknown6C);
                WritePad(writer, 0x133);
                writer.Write(Unknown1A0);
                writer.Write(Unknown1A2);
                WritePad(writer, 0x48);
                writer.Write(Unknown1EC);
                writer.Write(Unknown1ED);
                writer.Write(Unknown1EE);
                writer.Write(Unknown1EF);
                writer.Write(Unknown1F0);
                writer.Write(Unknown1F4);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => Unknown1F0 == assetID || base.HasReference(assetID);
    }

    public class EntrySHRP_Type4_BFBB : EntrySHRP
    {
        public static int SizeOfEntry => 0x90;

        public AssetID ModelAssetID { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public AssetSingle OffsetX { get; set; }
        public AssetSingle OffsetY { get; set; }
        public AssetSingle OffsetZ { get; set; }
        public int Unknown44 { get; set; }
        public int Unknown48 { get; set; }
        public AssetSingle Unknown4C { get; set; }
        public AssetSingle Unknown50 { get; set; }
        public AssetSingle Unknown54 { get; set; }
        public AssetSingle Unknown68 { get; set; }
        public int Unknown6C { get; set; }
        public int Unknown70 { get; set; }
        public AssetID UnknownAssetID74 { get; set; }
        public AssetSingle Unknown78 { get; set; }
        public AssetSingle Unknown7C { get; set; }
        public AssetSingle Unknown80 { get; set; }
        public AssetSingle Unknown84 { get; set; }
        public AssetSingle Unknown88 { get; set; }
        public AssetSingle Gravity { get; set; }

        public EntrySHRP_Type4_BFBB() : base(4) { }
        public EntrySHRP_Type4_BFBB(EndianBinaryReader reader) : base(4, reader)
        {
            ModelAssetID = reader.ReadUInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadInt32();
            Unknown24 = reader.ReadInt32();
            OffsetX = reader.ReadSingle();
            OffsetY = reader.ReadSingle();
            OffsetZ = reader.ReadSingle();
            ReadPad(reader, 0x10);
            Unknown44 = reader.ReadInt32();
            Unknown48 = reader.ReadInt32();
            Unknown4C = reader.ReadSingle();
            Unknown50 = reader.ReadSingle();
            Unknown54 = reader.ReadSingle();
            ReadPad(reader, 0x10);
            Unknown68 = reader.ReadSingle();
            Unknown6C = reader.ReadInt32();
            Unknown70 = reader.ReadInt32();
            UnknownAssetID74 = reader.ReadUInt32();
            Unknown78 = reader.ReadSingle();
            Unknown7C = reader.ReadSingle();
            Unknown80 = reader.ReadSingle();
            Unknown84 = reader.ReadSingle();
            Unknown88 = reader.ReadSingle();
            Gravity = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(ModelAssetID);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(OffsetX);
                writer.Write(OffsetY);
                writer.Write(OffsetZ);
                WritePad(writer, 0x10);
                writer.Write(Unknown44);
                writer.Write(Unknown48);
                writer.Write(Unknown4C);
                writer.Write(Unknown50);
                writer.Write(Unknown54);
                WritePad(writer, 0x10);
                writer.Write(Unknown68);
                writer.Write(Unknown6C);
                writer.Write(Unknown70);
                writer.Write(UnknownAssetID74);
                writer.Write(Unknown78);
                writer.Write(Unknown7C);
                writer.Write(Unknown80);
                writer.Write(Unknown84);
                writer.Write(Unknown88);
                writer.Write(Gravity);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (ModelAssetID == assetID)
                return true;
            if (UnknownAssetID74 == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type4_TSSM : EntrySHRP
    {
        public AssetID ModelAssetID { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public AssetSingle OffsetX { get; set; }
        public AssetSingle OffsetY { get; set; }
        public AssetSingle OffsetZ { get; set; }
        public int UnknownInt44 { get; set; }
        public int UnknownInt48 { get; set; }
        public int UnknownInt4C { get; set; }
        public int UnknownInt50 { get; set; }
        public int UnknownInt54 { get; set; }
        public int UnknownInt58 { get; set; }
        public int UnknownInt6C { get; set; }
        public byte UnknownByte70 { get; set; }
        public byte UnknownByte71 { get; set; }
        public byte UnknownByte72 { get; set; }
        public byte UnknownByte73 { get; set; }
        public int UnknownInt74 { get; set; }
        public AssetSingle UnknownFloat78 { get; set; }
        public AssetSingle UnknownFloat7C { get; set; }
        public AssetSingle UnknownFloat80 { get; set; }
        public int UnknownInt94 { get; set; }
        public int UnknownInt98 { get; set; }
        public int UnknownInt9C { get; set; }
        public int UnknownIntA0 { get; set; }
        public int UnknownIntA4 { get; set; }
        public int UnknownIntA8 { get; set; }
        public int UnknownIntBC { get; set; }
        public byte UnknownByteC0 { get; set; }
        public byte UnknownByteC1 { get; set; }
        public byte UnknownByteC2 { get; set; }
        public byte UnknownByteC3 { get; set; }
        public int UnknownIntC4 { get; set; }
        public AssetSingle UnknownFloatC8 { get; set; }
        public AssetSingle UnknownFloatCC { get; set; }
        public AssetSingle UnknownFloatD0 { get; set; }
        public int UnknownIntE4 { get; set; }
        public AssetSingle UnknownFloatE8 { get; set; }
        public int UnknownIntEC { get; set; }
        public int UnknownIntF0 { get; set; }
        public int UnknownIntF4 { get; set; }
        public int UnknownIntF8 { get; set; }
        public AssetSingle UnknownFloatFC { get; set; }
        public AssetSingle UnknownFloat100 { get; set; }
        public int UnknownInt104 { get; set; }
        public int UnknownInt108 { get; set; }
        public AssetSingle Gravity { get; set; }

        public EntrySHRP_Type4_TSSM() : base(4) { }
        public EntrySHRP_Type4_TSSM(EndianBinaryReader reader) : base(4, reader)
        {
            ModelAssetID = reader.ReadUInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadInt32();
            Unknown24 = reader.ReadInt32();
            OffsetX = reader.ReadSingle();
            OffsetY = reader.ReadSingle();
            OffsetZ = reader.ReadSingle();
            ReadPad(reader, 0x10);
            UnknownInt44 = reader.ReadInt32();
            UnknownInt48 = reader.ReadInt32();
            UnknownInt4C = reader.ReadInt32();
            UnknownInt50 = reader.ReadInt32();
            UnknownInt54 = reader.ReadInt32();
            UnknownInt58 = reader.ReadInt32();
            ReadPad(reader, 0x10);
            UnknownInt6C = reader.ReadInt32();
            UnknownByte70 = reader.ReadByte();
            UnknownByte71 = reader.ReadByte();
            UnknownByte72 = reader.ReadByte();
            UnknownByte73 = reader.ReadByte();
            UnknownInt74 = reader.ReadInt32();
            UnknownFloat78 = reader.ReadSingle();
            UnknownFloat7C = reader.ReadSingle();
            UnknownFloat80 = reader.ReadSingle();
            ReadPad(reader, 0x10);
            UnknownInt94 = reader.ReadInt32();
            UnknownInt98 = reader.ReadInt32();
            UnknownInt9C = reader.ReadInt32();
            UnknownIntA0 = reader.ReadInt32();
            UnknownIntA4 = reader.ReadInt32();
            UnknownIntA8 = reader.ReadInt32();
            ReadPad(reader, 0x10);
            UnknownIntBC = reader.ReadInt32();
            UnknownByteC0 = reader.ReadByte();
            UnknownByteC1 = reader.ReadByte();
            UnknownByteC2 = reader.ReadByte();
            UnknownByteC3 = reader.ReadByte();
            UnknownIntC4 = reader.ReadInt32();
            UnknownFloatC8 = reader.ReadSingle();
            UnknownFloatCC = reader.ReadSingle();
            UnknownFloatD0 = reader.ReadSingle();
            ReadPad(reader, 0x10);
            UnknownIntE4 = reader.ReadInt32();
            UnknownFloatE8 = reader.ReadSingle();
            UnknownIntEC = reader.ReadInt32();
            UnknownIntF0 = reader.ReadInt32();
            UnknownIntF4 = reader.ReadInt32();
            UnknownIntF8 = reader.ReadInt32();
            UnknownFloatFC = reader.ReadSingle();
            UnknownFloat100 = reader.ReadSingle();
            UnknownInt104 = reader.ReadInt32();
            UnknownInt108 = reader.ReadInt32();
            Gravity = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(ModelAssetID);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(OffsetX);
                writer.Write(OffsetY);
                writer.Write(OffsetZ);
                WritePad(writer, 0x10);
                writer.Write(UnknownInt44);
                writer.Write(UnknownInt48);
                writer.Write(UnknownInt4C);
                writer.Write(UnknownInt50);
                writer.Write(UnknownInt54);
                writer.Write(UnknownInt58);
                WritePad(writer, 0x10);
                writer.Write(UnknownInt6C);
                writer.Write(UnknownByte70);
                writer.Write(UnknownByte71);
                writer.Write(UnknownByte72);
                writer.Write(UnknownByte73);
                writer.Write(UnknownInt74);
                writer.Write(UnknownFloat78);
                writer.Write(UnknownFloat7C);
                writer.Write(UnknownFloat80);
                WritePad(writer, 0x10);
                writer.Write(UnknownInt94);
                writer.Write(UnknownInt98);
                writer.Write(UnknownInt9C);
                writer.Write(UnknownIntA0);
                writer.Write(UnknownIntA4);
                writer.Write(UnknownIntA8);
                WritePad(writer, 0x10);
                writer.Write(UnknownIntBC);
                writer.Write(UnknownByteC0);
                writer.Write(UnknownByteC1);
                writer.Write(UnknownByteC2);
                writer.Write(UnknownByteC3);
                writer.Write(UnknownIntC4);
                writer.Write(UnknownFloatC8);
                writer.Write(UnknownFloatCC);
                writer.Write(UnknownFloatD0);
                WritePad(writer, 0x10);
                writer.Write(UnknownIntE4);
                writer.Write(UnknownFloatE8);
                writer.Write(UnknownIntEC);
                writer.Write(UnknownIntF0);
                writer.Write(UnknownIntF4);
                writer.Write(UnknownIntF8);
                writer.Write(UnknownFloatFC);
                writer.Write(UnknownFloat100);
                writer.Write(UnknownInt104);
                writer.Write(UnknownInt108);
                writer.Write(Gravity);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (ModelAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type5_BFBB : EntrySHRP
    {
        public int Unknown18 { get; set; }
        public AssetID Unknown1C { get; set; }
        public AssetSingle Unknown20 { get; set; }
        public AssetID Unknown24 { get; set; }
        public AssetID Unknown28 { get; set; }
        public int Unknown3C { get; set; }
        public AssetID Unknown40 { get; set; }
        public AssetSingle Unknown44 { get; set; }
        public AssetID Unknown48 { get; set; }
        public AssetID Unknown4C { get; set; }

        public EntrySHRP_Type5_BFBB() : base(5) { }
        public EntrySHRP_Type5_BFBB(EndianBinaryReader reader) : base(5, reader)
        {
            Unknown18 = reader.ReadInt32();
            Unknown1C = reader.ReadUInt32();
            Unknown20 = reader.ReadSingle();
            Unknown24 = reader.ReadUInt32();
            Unknown28 = reader.ReadUInt32();
            ReadPad(reader, 0x10);
            Unknown3C = reader.ReadInt32();
            Unknown40 = reader.ReadUInt32();
            Unknown44 = reader.ReadSingle();
            Unknown48 = reader.ReadUInt32();
            Unknown4C = reader.ReadUInt32();
            ReadPad(reader, 0x18);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(Unknown18);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);
                WritePad(writer, 0x10);
                writer.Write(Unknown3C);
                writer.Write(Unknown40);
                writer.Write(Unknown44);
                writer.Write(Unknown48);
                writer.Write(Unknown4C);
                WritePad(writer, 0x18);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (Unknown1C == assetID)
                return true;
            if (Unknown24 == assetID)
                return true;
            if (Unknown28 == assetID)
                return true;
            if (Unknown40 == assetID)
                return true;
            if (Unknown48 == assetID)
                return true;
            if (Unknown4C == assetID)
                return true;
            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type5_TSSM : EntrySHRP
    {
        public int Unknown18 { get; set; }
        public int Unknown1C { get; set; }
        public AssetSingle Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public int Unknown28 { get; set; }
        public int Unknown3C { get; set; }
        public int Unknown40 { get; set; }
        public AssetSingle Unknown44 { get; set; }
        public int Unknown48 { get; set; }
        public int Unknown4C { get; set; }
        public int Unknown50 { get; set; }
        public int Unknown64 { get; set; }

        public EntrySHRP_Type5_TSSM() : base(5) { }
        public EntrySHRP_Type5_TSSM(EndianBinaryReader reader) : base(5, reader)
        {
            Unknown18 = reader.ReadInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadSingle();
            Unknown24 = reader.ReadInt32();
            Unknown28 = reader.ReadInt32();
            ReadPad(reader, 0x10);
            Unknown3C = reader.ReadInt32();
            Unknown40 = reader.ReadInt32();
            Unknown44 = reader.ReadSingle();
            Unknown48 = reader.ReadInt32();
            Unknown4C = reader.ReadInt32();
            Unknown50 = reader.ReadInt32();
            ReadPad(reader, 0x10);
            Unknown64 = reader.ReadInt32();
            ReadPad(reader, 0x8);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(Unknown18);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);
                WritePad(writer, 0x10);
                writer.Write(Unknown3C);
                writer.Write(Unknown40);
                writer.Write(Unknown44);
                writer.Write(Unknown48);
                writer.Write(Unknown4C);
                writer.Write(Unknown50);
                WritePad(writer, 0x10);
                writer.Write(Unknown64);
                WritePad(writer, 0x8);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (Unknown1C == assetID)
                return true;
            if (Unknown24 == assetID)
                return true;
            if (Unknown28 == assetID)
                return true;
            if (Unknown40 == assetID)
                return true;
            if (Unknown48 == assetID)
                return true;
            if (Unknown4C == assetID)
                return true;
            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type6_BFBB : EntrySHRP
    {
        public AssetID SoundAssetID { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public int Unknown28 { get; set; }
        public int Unknown2C { get; set; }
        public AssetSingle Unknown40 { get; set; }
        public AssetSingle Unknown44 { get; set; }
        public AssetSingle Unknown48 { get; set; }

        public EntrySHRP_Type6_BFBB() : base(6) { }
        public EntrySHRP_Type6_BFBB(EndianBinaryReader reader) : base(6, reader)
        {
            SoundAssetID = reader.ReadUInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadInt32();
            Unknown24 = reader.ReadInt32();
            Unknown28 = reader.ReadInt32();
            Unknown2C = reader.ReadInt32();
            ReadPad(reader, 0x10);
            Unknown40 = reader.ReadSingle();
            Unknown44 = reader.ReadSingle();
            Unknown48 = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(SoundAssetID);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);
                writer.Write(Unknown2C);
                WritePad(writer, 0x10);
                writer.Write(Unknown40);
                writer.Write(Unknown44);
                writer.Write(Unknown48);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (SoundAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type6_TSSM : EntrySHRP
    {
        public AssetID SoundAssetID { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public int Unknown28 { get; set; }
        public int Unknown2C { get; set; }
        public AssetSingle Unknown40 { get; set; }

        public EntrySHRP_Type6_TSSM() : base(6) { }
        public EntrySHRP_Type6_TSSM(EndianBinaryReader reader) : base(6, reader)
        {
            SoundAssetID = reader.ReadUInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadInt32();
            Unknown24 = reader.ReadInt32();
            Unknown28 = reader.ReadInt32();
            Unknown2C = reader.ReadInt32();
            ReadPad(reader, 0x10);
            Unknown40 = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(SoundAssetID);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);
                writer.Write(Unknown2C);
                WritePad(writer, 0x10);
                writer.Write(Unknown40);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (SoundAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type8 : EntrySHRP
    {
        public static int SizeOfEntry => 0x48;

        public AssetID UnknownAssetID18 { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public AssetID UnknownAssetID24 { get; set; }
        public AssetID UnknownAssetID28 { get; set; }
        public AssetID UnknownAssetID2C { get; set; }
        public AssetSingle Unknown40 { get; set; }
        public AssetSingle Unknown44 { get; set; }

        public EntrySHRP_Type8() : base(8) { }
        public EntrySHRP_Type8(EndianBinaryReader reader) : base(8, reader)
        {
            UnknownAssetID18 = reader.ReadUInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadInt32();
            UnknownAssetID24 = reader.ReadUInt32();
            UnknownAssetID28 = reader.ReadUInt32();
            UnknownAssetID2C = reader.ReadUInt32();
            ReadPad(reader, 0x10);
            Unknown40 = reader.ReadSingle();
            Unknown44 = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(UnknownAssetID18);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(UnknownAssetID24);
                writer.Write(UnknownAssetID28);
                writer.Write(UnknownAssetID2C);
                WritePad(writer, 0x10);
                writer.Write(Unknown40);
                writer.Write(Unknown44);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (UnknownAssetID18 == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type9 : EntrySHRP
    {
        public AssetID UnknownAssetID18 { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public AssetSingle Unknown28 { get; set; }
        public int Unknown2C { get; set; }
        public int Unknown40 { get; set; }
        public int Unknown44 { get; set; }
        public AssetSingle Unknown48 { get; set; }
        public AssetSingle Unknown4C { get; set; }
        public AssetSingle Unknown50 { get; set; }
        public AssetSingle Unknown54 { get; set; }
        public AssetSingle Unknown58 { get; set; }

        public EntrySHRP_Type9() : base(9) { }
        public EntrySHRP_Type9(EndianBinaryReader reader) : base(9, reader)
        {
            UnknownAssetID18 = reader.ReadUInt32();
            Unknown1C = reader.ReadInt32();
            Unknown20 = reader.ReadInt32();
            Unknown24 = reader.ReadInt32();
            Unknown28 = reader.ReadSingle();
            Unknown2C = reader.ReadInt32();
            ReadPad(reader, 0x10);
            Unknown40 = reader.ReadInt32();
            Unknown44 = reader.ReadInt32();
            Unknown48 = reader.ReadSingle();
            Unknown4C = reader.ReadSingle();
            Unknown50 = reader.ReadSingle();
            Unknown54 = reader.ReadSingle();
            Unknown58 = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntryShrpBase(endianness));

                writer.Write(UnknownAssetID18);
                writer.Write(Unknown1C);
                writer.Write(Unknown20);
                writer.Write(Unknown24);
                writer.Write(Unknown28);
                writer.Write(Unknown2C);
                WritePad(writer, 0x10);
                writer.Write(Unknown40);
                writer.Write(Unknown44);
                writer.Write(Unknown48);
                writer.Write(Unknown4C);
                writer.Write(Unknown50);
                writer.Write(Unknown54);
                writer.Write(Unknown58);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (UnknownAssetID18 == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }
}