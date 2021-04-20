//using HipHopFile;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;

//namespace IndustrialPark
//{
//    public class AssetSHRP : Asset
//    {
//        public AssetSHRP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
//        {
//            if (AssetID != AHDR.assetID)
//                AssetID = AHDR.assetID;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (AssetID == assetID)
//                return true;

//            try
//            {
//                foreach (EntrySHRP a in SHRPEntries)
//                    if (a.HasReference(assetID))
//                        return true;
//            }
//#if DEBUG
//            catch (Exception e)
//            {
//                MessageBox.Show("Error searching for references: " + e.Message + ". It will be skipped on the search.");
//            }
//#else
//            catch
//            {
//            }
//#endif
//            return false;
//        }

//        public override void Verify(ref List<string> result)
//        {
//            foreach (EntrySHRP a in SHRPEntries)
//                switch (a.Type)
//                {
//                    case 3:
//                        if (game == Game.BFBB)
//                            Verify(((EntrySHRP_Type3_BFBB)a).PARE_AssetID, ref result);
//                        else if (game == Game.Incredibles)
//                            Verify(((EntrySHRP_Type3_TSSM)a).Unknown1F0, ref result);
//                        break;
//                    case 4:
//                        if (game == Game.BFBB)
//                        {
//                            Verify(((EntrySHRP_Type4_BFBB)a).ModelAssetID, ref result);
//                            Verify(((EntrySHRP_Type4_BFBB)a).UnknownAssetID74, ref result);
//                        }
//                        else if (game == Game.Incredibles)
//                            Verify(((EntrySHRP_Type4_TSSM)a).ModelAssetID, ref result);
//                        break;
//                    case 6:
//                        if (game == Game.BFBB)
//                            Verify(((EntrySHRP_Type6_BFBB)a).SoundAssetID, ref result);
//                        else if (game == Game.Incredibles)
//                            Verify(((EntrySHRP_Type6_TSSM)a).SoundAssetID, ref result);
//                        break;
//                    case 9:
//                        Verify(((EntrySHRP_Type9)a).UnknownAssetID18, ref result);
//                        break;
//                }
//        }

//        [Category("Shrapnel"), ReadOnly(true)]
//        public int AmountOfEntries
//        {
//            get => ReadInt(0x00);
//            set => Write(0x00, value);
//        }

//        [Category("Shrapnel")]
//        public AssetID AssetID
//        {
//            get => ReadUInt(0x04);
//            set => Write(0x04, value);
//        }

//        [Category("Shrapnel")]
//        public int UnknownInt08
//        {
//            get => ReadInt(0x08);
//            set => Write(0x08, value);
//        }

//        [Category("Shrapnel")]
//        public EntrySHRP[] SHRPEntries
//        {
//            get => GetEntries();

//            set
//            {
//                List<byte> newData = Data.Take(0xC).ToList();
//                foreach (EntrySHRP entry in value)
//                    newData.AddRange(entry.ToByteArray());
//                Data = newData.ToArray();
//                AmountOfEntries = value.Length;
//            }
//        }

//        public EntrySHRP[] GetEntries()
//        {
//            List<EntrySHRP> entries = new List<EntrySHRP>();
//            BinaryReader binaryReader = new BinaryReader(new MemoryStream(Data.Skip(0xC).ToArray()));

//            for (int i = 0; i < AmountOfEntries; i++)
//            {
//                int Type = Switch(binaryReader.ReadInt32());
//                binaryReader.BaseStream.Position -= 4;

//                EntrySHRP entry = null;

//                if (Type == 3)
//                {
//                    if (game == Game.BFBB)
//                        entry = new EntrySHRP_Type3_BFBB(binaryReader.ReadBytes(EntrySHRP_Type3_BFBB.SizeOfEntry), platform);
//                    else if (game == Game.Incredibles)
//                        entry = new EntrySHRP_Type3_TSSM(binaryReader.ReadBytes(EntrySHRP_Type3_TSSM.SizeOfEntry), platform);
//                }
//                else if (Type == 4)
//                {
//                    if (game == Game.BFBB)
//                        entry = new EntrySHRP_Type4_BFBB(binaryReader.ReadBytes(EntrySHRP_Type4_BFBB.SizeOfEntry), platform);
//                    else if (game == Game.Incredibles)
//                        entry = new EntrySHRP_Type4_TSSM(binaryReader.ReadBytes(EntrySHRP_Type4_TSSM.SizeOfEntry), platform);
//                }
//                else if (Type == 5)
//                {
//                    if (game == Game.BFBB)
//                        entry = new EntrySHRP_Type5_BFBB(binaryReader.ReadBytes(EntrySHRP_Type5_BFBB.SizeOfEntry), platform);
//                    else if (game == Game.Incredibles)
//                        entry = new EntrySHRP_Type5_TSSM(binaryReader.ReadBytes(EntrySHRP_Type5_TSSM.SizeOfEntry), platform);
//                }
//                else if (Type == 6)
//                {
//                    if (game == Game.BFBB)
//                        entry = new EntrySHRP_Type6_BFBB(binaryReader.ReadBytes(EntrySHRP_Type6_BFBB.SizeOfEntry), platform);
//                    else if (game == Game.Incredibles)
//                        entry = new EntrySHRP_Type6_TSSM(binaryReader.ReadBytes(EntrySHRP_Type6_TSSM.SizeOfEntry), platform);
//                }
//                else if (Type == 8)
//                    entry = new EntrySHRP_Type8(binaryReader.ReadBytes(EntrySHRP_Type8.SizeOfEntry), platform);
//                else if (Type == 9)
//                    entry = new EntrySHRP_Type9(binaryReader.ReadBytes(EntrySHRP_Type9.SizeOfEntry), platform);
//                else
//                    throw new Exception("Unknown SHRP entry type " + Type.ToString() + " found in asset " + ToString() + ". This SHRP asset cannot be edited by Industrial Park.");

//                entries.Add(entry);
//            }

//            return entries.ToArray();
//        }

//        public void AddEntry(int type)
//        {
//            List<EntrySHRP> list = SHRPEntries.ToList();

//            switch (type)
//            {
//                case 3:
//                    if (game == Game.Incredibles)
//                        list.Add(new EntrySHRP_Type3_TSSM(platform));
//                    else
//                        list.Add(new EntrySHRP_Type3_BFBB(platform));
//                    break;
//                case 4:
//                    if (game == Game.Incredibles)
//                        list.Add(new EntrySHRP_Type4_TSSM(platform));
//                    else
//                        list.Add(new EntrySHRP_Type4_BFBB(platform));
//                    break;
//                case 5:
//                    if (game == Game.Incredibles)
//                        list.Add(new EntrySHRP_Type5_TSSM(platform));
//                    else
//                        list.Add(new EntrySHRP_Type5_BFBB(platform));
//                    break;
//                case 6:
//                    if (game == Game.Incredibles)
//                        list.Add(new EntrySHRP_Type6_TSSM(platform));
//                    else
//                        list.Add(new EntrySHRP_Type6_BFBB(platform));
//                    break;
//                case 8:
//                    list.Add(new EntrySHRP_Type8(platform));
//                    break;
//                case 9:
//                    list.Add(new EntrySHRP_Type9(platform));
//                    break;
//            }

//            SHRPEntries = list.ToArray();
//        }
//    }

//    public abstract class EntrySHRP : EndianConvertible
//    {
//        [ReadOnly(true)]
//        public int Type { get; set; }
//        public AssetID Unknown04 { get; set; }
//        public AssetID Unknown08 { get; set; }
//        public AssetID Unknown0C { get; set; }
//        public float Unknown10 { get; set; }
//        public float Unknown14 { get; set; }

//        public EntrySHRP(Platform platform) : base(platform)
//        {
//            Unknown04 = 0;
//            Unknown08 = 0;
//            Unknown0C = 0;
//        }

//        public EntrySHRP(byte[] data, Platform platform) : base(platform)
//        {
//            Type = Switch(BitConverter.ToInt32(data, 0x00));
//            Unknown04 = Switch(BitConverter.ToUInt32(data, 0x04));
//            Unknown08 = Switch(BitConverter.ToUInt32(data, 0x08));
//            Unknown0C = Switch(BitConverter.ToUInt32(data, 0x0C));
//            Unknown10 = Switch(BitConverter.ToSingle(data, 0x10));
//            Unknown14 = Switch(BitConverter.ToSingle(data, 0x14));
//        }

//        public virtual List<byte> ToByteArray()
//        {
//            List<byte> list = new List<byte>();

//            list.AddRange(BitConverter.GetBytes(Switch(Type)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown04)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown08)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown0C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown10)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown14)));

//            return list;
//        }

//        public virtual bool HasReference(uint assetID)
//        {
//            if (Unknown04 == assetID)
//                return true;
//            if (Unknown08 == assetID)
//                return true;
//            if (Unknown0C == assetID)
//                return true;

//            return false;
//        }

//        internal IEnumerable<byte> ToReverseByteArray()
//        {
//            endianness = endianness == Endianness.Big ? Endianness.Little : Endianness.Big;
//            return ToByteArray();
//        }
//    }

//    public class EntrySHRP_Type3_BFBB : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x1D4;

//        public int Unknown18 { get; set; }
//        public int Unknown1C { get; set; }
//        public float Unknown20 { get; set; }
//        public float Unknown24 { get; set; }
//        public float Unknown28 { get; set; }
//        public int Unknown3C { get; set; }
//        public int Unknown40 { get; set; }
//        public int Unknown44 { get; set; }
//        public float Unknown48 { get; set; }
//        public float Unknown4C { get; set; }
//        public AssetID PARE_AssetID { get; set; }
//        public int Unknown1D0 { get; set; }

//        public EntrySHRP_Type3_BFBB(Platform platform) : base(platform)
//        {
//            Type = 3;
//            PARE_AssetID = 0;
//        }

//        public EntrySHRP_Type3_BFBB(byte[] data, Platform platform) : base(data, platform)
//        {
//            Unknown18 = Switch(BitConverter.ToInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToSingle(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToSingle(data, 0x24));
//            Unknown28 = Switch(BitConverter.ToSingle(data, 0x28));
//            Unknown3C = Switch(BitConverter.ToInt32(data, 0x3C));
//            Unknown40 = Switch(BitConverter.ToInt32(data, 0x40));
//            Unknown44 = Switch(BitConverter.ToInt32(data, 0x44));
//            Unknown48 = Switch(BitConverter.ToSingle(data, 0x48));
//            Unknown4C = Switch(BitConverter.ToSingle(data, 0x4C));
//            PARE_AssetID = Switch(BitConverter.ToUInt32(data, 0x1CC));
//            Unknown1D0 = Switch(BitConverter.ToInt32(data, 0x1D0));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(Unknown18)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown3C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
//            for (int i = 0; i < 0x17C; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(PARE_AssetID)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1D0)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (PARE_AssetID == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type3_TSSM : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x1F8;

//        public int Unknown18 { get; set; }
//        public int Unknown1C { get; set; }
//        public float Unknown20 { get; set; }
//        public float Unknown24 { get; set; }
//        public float Unknown28 { get; set; }
//        public int Unknown3C { get; set; }
//        public int Unknown40 { get; set; }
//        public int Unknown44 { get; set; }
//        public float Unknown48 { get; set; }
//        public float Unknown4C { get; set; }
//        public int Unknown50 { get; set; }
//        public short Unknown1A0 { get; set; }
//        public short Unknown1A2 { get; set; }
//        public byte Unknown1EC { get; set; }
//        public byte Unknown1ED { get; set; }
//        public byte Unknown1EE { get; set; }
//        public byte Unknown1EF { get; set; }
//        public AssetID Unknown1F0 { get; set; }
//        public int Unknown1F4 { get; set; }

//        public EntrySHRP_Type3_TSSM(Platform platform) : base(platform)
//        {
//            Type = 3;
//            Unknown1F0 = 0;
//        }

//        public EntrySHRP_Type3_TSSM(byte[] data, Platform platform) : base(data, platform)
//        {
//            Unknown18 = Switch(BitConverter.ToInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToSingle(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToSingle(data, 0x24));
//            Unknown28 = Switch(BitConverter.ToSingle(data, 0x28));
//            Unknown3C = Switch(BitConverter.ToInt32(data, 0x3C));
//            Unknown40 = Switch(BitConverter.ToInt32(data, 0x40));
//            Unknown44 = Switch(BitConverter.ToInt32(data, 0x44));
//            Unknown48 = Switch(BitConverter.ToSingle(data, 0x48));
//            Unknown4C = Switch(BitConverter.ToSingle(data, 0x4C));
//            Unknown50 = Switch(BitConverter.ToInt32(data, 0x50));
//            Unknown1A0 = Switch(BitConverter.ToInt16(data, 0x1A0));
//            Unknown1A2 = Switch(BitConverter.ToInt16(data, 0x1A2));
//            Unknown1EC = data[0x1EC];
//            Unknown1ED = data[0x1ED];
//            Unknown1EE = data[0x1EE];
//            Unknown1EF = data[0x1EF];
//            Unknown1F0 = Switch(BitConverter.ToUInt32(data, 0x1F0));
//            Unknown1F4 = Switch(BitConverter.ToInt32(data, 0x1F4));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(Unknown18)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown3C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown50)));
//            for (int i = 0; i < 0x14C; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1A0)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1A2)));
//            for (int i = 0; i < 0x48; i++)
//                list.Add(0xCD);
//            list.Add(Unknown1EC);
//            list.Add(Unknown1ED);
//            list.Add(Unknown1EE);
//            list.Add(Unknown1EF);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1F0)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1F4)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (Unknown1F0 == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type4_BFBB : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x90;

//        public AssetID ModelAssetID { get; set; }
//        public int Unknown1C { get; set; }
//        public int Unknown20 { get; set; }
//        public int Unknown24 { get; set; }
//        public float OffsetX { get; set; }
//        public float OffsetY { get; set; }
//        public float OffsetZ { get; set; }
//        public float Unknown68 { get; set; }
//        public int Unknown6C { get; set; }
//        public int Unknown70 { get; set; }
//        public AssetID UnknownAssetID74 { get; set; }
//        public float Unknown78 { get; set; }
//        public float Unknown7C { get; set; }
//        public float Unknown80 { get; set; }
//        public float Unknown84 { get; set; }
//        public float Unknown88 { get; set; }
//        public float Gravity { get; set; }

//        public EntrySHRP_Type4_BFBB(Platform platform) : base(platform)
//        {
//            Type = 4;
//            ModelAssetID = 0;
//            UnknownAssetID74 = 0;
//        }

//        public EntrySHRP_Type4_BFBB(byte[] data, Platform platform) : base(data, platform)
//        {
//            ModelAssetID = Switch(BitConverter.ToUInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
//            OffsetX = Switch(BitConverter.ToSingle(data, 0x28));
//            OffsetY = Switch(BitConverter.ToSingle(data, 0x2C));
//            OffsetZ = Switch(BitConverter.ToSingle(data, 0x30));
//            Unknown68 = Switch(BitConverter.ToSingle(data, 0x68));
//            Unknown6C = Switch(BitConverter.ToInt32(data, 0x6C));
//            Unknown70 = Switch(BitConverter.ToInt32(data, 0x70));
//            UnknownAssetID74 = Switch(BitConverter.ToUInt32(data, 0x74));
//            Unknown78 = Switch(BitConverter.ToSingle(data, 0x78));
//            Unknown7C = Switch(BitConverter.ToSingle(data, 0x7C));
//            Unknown80 = Switch(BitConverter.ToSingle(data, 0x80));
//            Unknown84 = Switch(BitConverter.ToSingle(data, 0x84));
//            Unknown88 = Switch(BitConverter.ToSingle(data, 0x88));
//            Gravity = Switch(BitConverter.ToSingle(data, 0x8C));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(ModelAssetID)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(OffsetX)));
//            list.AddRange(BitConverter.GetBytes(Switch(OffsetY)));
//            list.AddRange(BitConverter.GetBytes(Switch(OffsetZ)));
//            for (int i = 0; i < 0x34; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown68)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown6C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown70)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID74)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown78)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown7C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown80)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown84)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown88)));
//            list.AddRange(BitConverter.GetBytes(Switch(Gravity)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (ModelAssetID == assetID)
//                return true;
//            if (UnknownAssetID74 == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type4_TSSM : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x110;

//        public AssetID ModelAssetID { get; set; }
//        public int Unknown1C { get; set; }
//        public int Unknown20 { get; set; }
//        public int Unknown24 { get; set; }
//        public float OffsetX { get; set; }
//        public float OffsetY { get; set; }
//        public float OffsetZ { get; set; }
//        public int UnknownInt44 { get; set; }
//        public int UnknownInt48 { get; set; }
//        public int UnknownInt4C { get; set; }
//        public int UnknownInt50 { get; set; }
//        public int UnknownInt54 { get; set; }
//        public int UnknownInt58 { get; set; }
//        public int UnknownInt98 { get; set; }
//        public int UnknownInt9C { get; set; }
//        public int UnknownIntA0 { get; set; }
//        public int UnknownIntA4 { get; set; }
//        public int UnknownIntA8 { get; set; }
//        public float UnknownFloatE8 { get; set; }
//        public int UnknownIntEC { get; set; }
//        public int UnknownIntF0 { get; set; }
//        public int UnknownIntF4 { get; set; }
//        public int UnknownIntF8 { get; set; }
//        public float UnknownFloatFC { get; set; }
//        public float UnknownFloat100 { get; set; }
//        public int UnknownInt104 { get; set; }
//        public int UnknownInt108 { get; set; }
//        public float Gravity { get; set; }

//        public EntrySHRP_Type4_TSSM(Platform platform) : base(platform)
//        {
//            Type = 4;
//            ModelAssetID = 0;
//        }

//        public EntrySHRP_Type4_TSSM(byte[] data, Platform platform) : base(data, platform)
//        {
//            ModelAssetID = Switch(BitConverter.ToUInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
//            OffsetX = Switch(BitConverter.ToSingle(data, 0x28));
//            OffsetY = Switch(BitConverter.ToSingle(data, 0x2C));
//            OffsetZ = Switch(BitConverter.ToSingle(data, 0x30));
//            UnknownInt44 = Switch(BitConverter.ToInt32(data, 0x44));
//            UnknownInt48 = Switch(BitConverter.ToInt32(data, 0x48));
//            UnknownInt4C = Switch(BitConverter.ToInt32(data, 0x4C));
//            UnknownInt50 = Switch(BitConverter.ToInt32(data, 0x50));
//            UnknownInt54 = Switch(BitConverter.ToInt32(data, 0x54));
//            UnknownInt58 = Switch(BitConverter.ToInt32(data, 0x58));
//            UnknownInt98 = Switch(BitConverter.ToInt32(data, 0x98));
//            UnknownInt9C = Switch(BitConverter.ToInt32(data, 0x9C));
//            UnknownIntA0 = Switch(BitConverter.ToInt32(data, 0xA0));
//            UnknownIntA4 = Switch(BitConverter.ToInt32(data, 0xA4));
//            UnknownIntA8 = Switch(BitConverter.ToInt32(data, 0xA8));
//            UnknownFloatE8 = Switch(BitConverter.ToSingle(data, 0xE8));
//            UnknownIntEC = Switch(BitConverter.ToInt32(data, 0xEC));
//            UnknownIntF0 = Switch(BitConverter.ToInt32(data, 0xF0));
//            UnknownIntF4 = Switch(BitConverter.ToInt32(data, 0xF4));
//            UnknownIntF8 = Switch(BitConverter.ToInt32(data, 0xF8));
//            UnknownFloatFC = Switch(BitConverter.ToSingle(data, 0xFC));
//            UnknownFloat100 = Switch(BitConverter.ToSingle(data, 0x100));
//            UnknownInt104 = Switch(BitConverter.ToInt32(data, 0x104));
//            UnknownInt108 = Switch(BitConverter.ToInt32(data, 0x108));
//            Gravity = Switch(BitConverter.ToSingle(data, 0x10C));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(ModelAssetID)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(OffsetX)));
//            list.AddRange(BitConverter.GetBytes(Switch(OffsetY)));
//            list.AddRange(BitConverter.GetBytes(Switch(OffsetZ)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt44)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt48)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt4C)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt50)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt54)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt58)));
//            for (int i = 0; i < 0x3C; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt98)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt9C)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownIntA0)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownIntA4)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownIntA8)));
//            for (int i = 0; i < 0x3C; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloatE8)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownIntEC)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownIntF0)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownIntF4)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownIntF8)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloatFC)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat100)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt104)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt108)));
//            list.AddRange(BitConverter.GetBytes(Switch(Gravity)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (ModelAssetID == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type5_BFBB : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x68;

//        public int Unknown18 { get; set; }
//        public AssetID Unknown1C { get; set; }
//        public float Unknown20 { get; set; }
//        public AssetID Unknown24 { get; set; }
//        public AssetID Unknown28 { get; set; }
//        public int Unknown3C { get; set; }
//        public AssetID Unknown40 { get; set; }
//        public float Unknown44 { get; set; }
//        public AssetID Unknown48 { get; set; }
//        public AssetID Unknown4C { get; set; }

//        public EntrySHRP_Type5_BFBB(Platform platform) : base(platform)
//        {
//            Type = 5;
//            Unknown1C = 0;
//            Unknown24 = 0;
//            Unknown28 = 0;
//            Unknown40 = 0;
//            Unknown48 = 0;
//            Unknown4C = 0;
//        }

//        public EntrySHRP_Type5_BFBB(byte[] data, Platform platform) : base(data, platform)
//        {
//            Unknown18 = Switch(BitConverter.ToInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToUInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToSingle(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToUInt32(data, 0x24));
//            Unknown28 = Switch(BitConverter.ToUInt32(data, 0x28));
//            Unknown3C = Switch(BitConverter.ToInt32(data, 0x3C));
//            Unknown40 = Switch(BitConverter.ToUInt32(data, 0x40));
//            Unknown44 = Switch(BitConverter.ToSingle(data, 0x44));
//            Unknown48 = Switch(BitConverter.ToUInt32(data, 0x48));
//            Unknown4C = Switch(BitConverter.ToUInt32(data, 0x4C));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(Unknown18)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown3C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
//            for (int i = 0; i < 0x18; i++)
//                list.Add(0xCD);

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (Unknown1C == assetID)
//                return true;
//            if (Unknown24 == assetID)
//                return true;
//            if (Unknown28 == assetID)
//                return true;
//            if (Unknown40 == assetID)
//                return true;
//            if (Unknown48 == assetID)
//                return true;
//            if (Unknown4C == assetID)
//                return true;
//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type5_TSSM : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x70;

//        public int Unknown18 { get; set; }
//        public int Unknown1C { get; set; }
//        public float Unknown20 { get; set; }
//        public int Unknown24 { get; set; }
//        public int Unknown28 { get; set; }
//        public int Unknown3C { get; set; }
//        public int Unknown40 { get; set; }
//        public float Unknown44 { get; set; }
//        public int Unknown48 { get; set; }
//        public int Unknown4C { get; set; }
//        public int Unknown50 { get; set; }
//        public int Unknown64 { get; set; }

//        public EntrySHRP_Type5_TSSM(Platform platform) : base(platform)
//        {
//            Type = 5;
//        }

//        public EntrySHRP_Type5_TSSM(byte[] data, Platform platform) : base(data, platform)
//        {
//            Unknown18 = Switch(BitConverter.ToInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToSingle(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
//            Unknown28 = Switch(BitConverter.ToInt32(data, 0x28));
//            Unknown3C = Switch(BitConverter.ToInt32(data, 0x3C));
//            Unknown40 = Switch(BitConverter.ToInt32(data, 0x40));
//            Unknown44 = Switch(BitConverter.ToSingle(data, 0x44));
//            Unknown48 = Switch(BitConverter.ToInt32(data, 0x48));
//            Unknown4C = Switch(BitConverter.ToInt32(data, 0x4C));
//            Unknown50 = Switch(BitConverter.ToInt32(data, 0x50));
//            Unknown64 = Switch(BitConverter.ToInt32(data, 0x64));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(Unknown18)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown3C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown50)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown64)));
//            for (int i = 0; i < 0x8; i++)
//                list.Add(0xCD);

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (Unknown1C == assetID)
//                return true;
//            if (Unknown24 == assetID)
//                return true;
//            if (Unknown28 == assetID)
//                return true;
//            if (Unknown40 == assetID)
//                return true;
//            if (Unknown48 == assetID)
//                return true;
//            if (Unknown4C == assetID)
//                return true;
//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type6_BFBB : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x4C;

//        public AssetID SoundAssetID { get; set; }
//        public int Unknown1C { get; set; }
//        public int Unknown20 { get; set; }
//        public int Unknown24 { get; set; }
//        public int Unknown28 { get; set; }
//        public int Unknown2C { get; set; }
//        public float Unknown40 { get; set; }
//        public float Unknown44 { get; set; }
//        public float Unknown48 { get; set; }

//        public EntrySHRP_Type6_BFBB(Platform platform) : base(platform)
//        {
//            Type = 6;
//            SoundAssetID = 0;
//        }

//        public EntrySHRP_Type6_BFBB(byte[] data, Platform platform) : base(data, platform)
//        {
//            SoundAssetID = Switch(BitConverter.ToUInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
//            Unknown28 = Switch(BitConverter.ToInt32(data, 0x28));
//            Unknown2C = Switch(BitConverter.ToInt32(data, 0x2C));
//            Unknown40 = Switch(BitConverter.ToSingle(data, 0x40));
//            Unknown44 = Switch(BitConverter.ToSingle(data, 0x44));
//            Unknown48 = Switch(BitConverter.ToSingle(data, 0x48));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(SoundAssetID)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown2C)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (SoundAssetID == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type6_TSSM : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x44;

//        public AssetID SoundAssetID { get; set; }
//        public int Unknown1C { get; set; }
//        public int Unknown20 { get; set; }
//        public int Unknown24 { get; set; }
//        public int Unknown28 { get; set; }
//        public int Unknown2C { get; set; }
//        public float Unknown40 { get; set; }

//        public EntrySHRP_Type6_TSSM(Platform platform) : base(platform)
//        {
//            Type = 6;
//            SoundAssetID = 0;
//        }

//        public EntrySHRP_Type6_TSSM(byte[] data, Platform platform) : base(data, platform)
//        {
//            SoundAssetID = Switch(BitConverter.ToUInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
//            Unknown28 = Switch(BitConverter.ToInt32(data, 0x28));
//            Unknown2C = Switch(BitConverter.ToInt32(data, 0x2C));
//            Unknown40 = Switch(BitConverter.ToSingle(data, 0x40));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(SoundAssetID)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown2C)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (SoundAssetID == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type8 : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x48;

//        public AssetID UnknownAssetID18 { get; set; }
//        public int Unknown1C { get; set; }
//        public int Unknown20 { get; set; }
//        public AssetID UnknownAssetID24 { get; set; }
//        public AssetID UnknownAssetID28 { get; set; }
//        public AssetID UnknownAssetID2C { get; set; }
//        public float Unknown40 { get; set; }
//        public float Unknown44 { get; set; }

//        public EntrySHRP_Type8(Platform platform) : base(platform)
//        {
//            Type = 8;
//            UnknownAssetID18 = 0;
//            UnknownAssetID24 = 0;
//            UnknownAssetID28 = 0;
//            UnknownAssetID2C = 0;
//        }

//        public EntrySHRP_Type8(byte[] data, Platform platform) : base(data, platform)
//        {
//            UnknownAssetID18 = Switch(BitConverter.ToUInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
//            UnknownAssetID24 = Switch(BitConverter.ToUInt32(data, 0x24));
//            UnknownAssetID28 = Switch(BitConverter.ToUInt32(data, 0x28));
//            UnknownAssetID2C = Switch(BitConverter.ToUInt32(data, 0x2C));
//            Unknown40 = Switch(BitConverter.ToSingle(data, 0x40));
//            Unknown44 = Switch(BitConverter.ToSingle(data, 0x44));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID18)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID24)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID28)));
//            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID2C)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (UnknownAssetID18 == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }

//    public class EntrySHRP_Type9 : EntrySHRP
//    {
//        public static int SizeOfEntry => 0x5C;
        
//        public AssetID UnknownAssetID18 { get; set; }
//        public int Unknown1C { get; set; }
//        public int Unknown20 { get; set; }
//        public int Unknown24 { get; set; }
//        public float Unknown28 { get; set; }
//        public int Unknown2C { get; set; }
//        public int Unknown40 { get; set; }
//        public int Unknown44 { get; set; }
//        public float Unknown48 { get; set; }
//        public float Unknown4C { get; set; }
//        public float Unknown50 { get; set; }
//        public float Unknown54 { get; set; }
//        public float Unknown58 { get; set; }

//        public EntrySHRP_Type9(Platform platform) : base(platform)
//        {
//            Type = 8;
//            UnknownAssetID18 = 0;
//        }

//        public EntrySHRP_Type9(byte[] data, Platform platform) : base(data, platform)
//        {
//            UnknownAssetID18 = Switch(BitConverter.ToUInt32(data, 0x18));
//            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
//            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
//            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
//            Unknown28 = Switch(BitConverter.ToSingle(data, 0x28));
//            Unknown2C = Switch(BitConverter.ToInt32(data, 0x2C));
//            Unknown40 = Switch(BitConverter.ToInt32(data, 0x40));
//            Unknown44 = Switch(BitConverter.ToInt32(data, 0x44));
//            Unknown48 = Switch(BitConverter.ToSingle(data, 0x48));
//            Unknown4C = Switch(BitConverter.ToSingle(data, 0x4C));
//            Unknown50 = Switch(BitConverter.ToSingle(data, 0x50));
//            Unknown54 = Switch(BitConverter.ToSingle(data, 0x54));
//            Unknown58 = Switch(BitConverter.ToSingle(data, 0x58));
//        }

//        public override List<byte> ToByteArray()
//        {
//            List<byte> list = base.ToByteArray();

//            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID18)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown2C)));
//            for (int i = 0; i < 0x10; i++)
//                list.Add(0xCD);
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown50)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown54)));
//            list.AddRange(BitConverter.GetBytes(Switch(Unknown58)));

//            return list;
//        }

//        public override bool HasReference(uint assetID)
//        {
//            if (UnknownAssetID18 == assetID)
//                return true;

//            return base.HasReference(assetID);
//        }
//    }
//}