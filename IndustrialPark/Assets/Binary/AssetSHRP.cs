using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class AssetSHRP : Asset
    {
        public AssetSHRP(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (AssetID == assetID)
                return true;

            foreach (EntrySHRP a in SHRPEntries)
                if (a.HasReference(assetID))
                    return true;

            return false;
        }

        [Category("Shrapnel"), ReadOnly(true)]
        public int AmountOfEntries
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }

        [Category("Shrapnel")]
        public AssetID AssetID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }

        [Category("Shrapnel")]
        public int UnknownInt08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category("Shrapnel")]
        public EntrySHRP[] SHRPEntries
        {
            get
            {
                List<EntrySHRP> entries = new List<EntrySHRP>();
                BinaryReader binaryReader = new BinaryReader(new MemoryStream(Data.Skip(0xC).ToArray()));

                for (int i = 0; i < AmountOfEntries; i++)
                {
                    int Type = Switch(binaryReader.ReadInt32());
                    binaryReader.BaseStream.Position -= 4;

                    EntrySHRP entry = null;

                    if (Type == 3)
                        entry = new EntrySHRP_Type3(binaryReader.ReadBytes(0x1D4));
                    else if (Type == 4)
                        entry = new EntrySHRP_Type4(binaryReader.ReadBytes(0x90));
                    else if (Type == 5)
                        entry = new EntrySHRP_Type5(binaryReader.ReadBytes(0x68));
                    else if (Type == 6)
                        entry = new EntrySHRP_Type6(binaryReader.ReadBytes(0x4C));
                    else
                        System.Windows.Forms.MessageBox.Show("Unknown SHRP entry type found: " + Type.ToString() + " in asset " + ToString());

                    entries.Add(entry);
                }

                return entries.ToArray();
            }

            set
            {
                List<byte> newData = Data.Take(0xC).ToList();
                foreach (EntrySHRP entry in value)
                    newData.AddRange(entry.ToByteArray());
                Data = newData.ToArray();
                AmountOfEntries = value.Length;
            }
        }
    }

    public abstract class EntrySHRP
    {
        [ReadOnly(true)]
        public int Type { get; set; }
        public AssetID Unknown04 { get; set; }
        public AssetID Unknown08 { get; set; }
        public AssetID Unknown0C { get; set; }
        public float Unknown10 { get; set; }
        public float Unknown14 { get; set; }

        public EntrySHRP()
        {
            Unknown04 = 0;
            Unknown08 = 0;
            Unknown0C = 0;
        }

        public EntrySHRP(byte[] data)
        {
            Type = Switch(BitConverter.ToInt32(data, 0x00));
            Unknown04 = Switch(BitConverter.ToUInt32(data, 0x04));
            Unknown08 = Switch(BitConverter.ToUInt32(data, 0x08));
            Unknown0C = Switch(BitConverter.ToUInt32(data, 0x0C));
            Unknown10 = Switch(BitConverter.ToSingle(data, 0x10));
            Unknown14 = Switch(BitConverter.ToSingle(data, 0x14));
        }

        public virtual List<byte> ToByteArray()
        {
            List<byte> list = new List<byte>();

            list.AddRange(BitConverter.GetBytes(Switch(Type)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown04)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown08)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown0C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown10)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown14)));

            return list;
        }

        public virtual bool HasReference(uint assetID)
        {
            if (Unknown04 == assetID)
                return true;

            return false;
        }
    }

    public class EntrySHRP_Type3 : EntrySHRP
    {
        public int Unknown18 { get; set; }
        public int Unknown1C { get; set; }
        public float Unknown20 { get; set; }
        public float Unknown24 { get; set; }
        public float Unknown28 { get; set; }
        public int Unknown3C { get; set; }
        public int Unknown40 { get; set; }
        public int Unknown44 { get; set; }
        public float Unknown48 { get; set; }
        public float Unknown4C { get; set; }
        public AssetID PARE_AssetID { get; set; }
        public int Unknown1D0 { get; set; }

        public EntrySHRP_Type3() : base()
        {
            Type = 3;
            PARE_AssetID = 0;
        }

        public EntrySHRP_Type3(byte[] data) : base(data)
        {
            Unknown18 = Switch(BitConverter.ToInt32(data, 0x18));
            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
            Unknown20 = Switch(BitConverter.ToSingle(data, 0x20));
            Unknown24 = Switch(BitConverter.ToSingle(data, 0x24));
            Unknown28 = Switch(BitConverter.ToSingle(data, 0x28));
            Unknown3C = Switch(BitConverter.ToInt32(data, 0x3C));
            Unknown40 = Switch(BitConverter.ToInt32(data, 0x40));
            Unknown44 = Switch(BitConverter.ToInt32(data, 0x44));
            Unknown48 = Switch(BitConverter.ToSingle(data, 0x48));
            Unknown4C = Switch(BitConverter.ToSingle(data, 0x4C));
            PARE_AssetID = Switch(BitConverter.ToUInt32(data, 0x1CC));
            Unknown1D0 = Switch(BitConverter.ToInt32(data, 0x1D0));
        }

        public override List<byte> ToByteArray()
        {
            List<byte> list = base.ToByteArray();

            list.AddRange(BitConverter.GetBytes(Switch(Unknown18)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
            for (int i = 0; i < 0x10; i++)
                list.Add(0xCD);
            list.AddRange(BitConverter.GetBytes(Switch(Unknown3C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
            for (int i = 0; i < 0x17C; i++)
                list.Add(0xCD);
            list.AddRange(BitConverter.GetBytes(Switch(PARE_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown1D0)));

            return list;
        }

        public override bool HasReference(uint assetID)
        {
            if (PARE_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }

    public class EntrySHRP_Type4 : EntrySHRP
    {
        public AssetID ModelAssetID { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public float Unknown28 { get; set; }
        public float Unknown2C { get; set; }
        public float Unknown30 { get; set; }
        public float Unknown68 { get; set; }
        public int Unknown6C { get; set; }
        public int Unknown70 { get; set; }
        public AssetID UnknownAssetID74 { get; set; }
        public float Unknown78 { get; set; }
        public float Unknown7C { get; set; }
        public float Unknown80 { get; set; }
        public float Unknown84 { get; set; }
        public float Unknown88 { get; set; }
        public float Unknown8C { get; set; }

        public EntrySHRP_Type4() : base()
        {
            Type = 4;
            ModelAssetID = 0;
            UnknownAssetID74 = 0;
        }

        public EntrySHRP_Type4(byte[] data) : base(data)
        {
            ModelAssetID = Switch(BitConverter.ToUInt32(data, 0x18));
            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
            Unknown28 = Switch(BitConverter.ToSingle(data, 0x28));
            Unknown2C = Switch(BitConverter.ToSingle(data, 0x2C));
            Unknown30 = Switch(BitConverter.ToSingle(data, 0x30));
            Unknown68 = Switch(BitConverter.ToSingle(data, 0x68));
            Unknown6C = Switch(BitConverter.ToInt32(data, 0x6C));
            Unknown70 = Switch(BitConverter.ToInt32(data, 0x70));
            UnknownAssetID74 = Switch(BitConverter.ToUInt32(data, 0x74));
            Unknown78 = Switch(BitConverter.ToSingle(data, 0x78));
            Unknown7C = Switch(BitConverter.ToSingle(data, 0x7C));
            Unknown80 = Switch(BitConverter.ToSingle(data, 0x80));
            Unknown84 = Switch(BitConverter.ToSingle(data, 0x84));
            Unknown88 = Switch(BitConverter.ToSingle(data, 0x88));
            Unknown8C = Switch(BitConverter.ToSingle(data, 0x8C));
        }

        public override List<byte> ToByteArray()
        {
            List<byte> list = base.ToByteArray();

            list.AddRange(BitConverter.GetBytes(Switch(ModelAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown2C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown30)));
            for (int i = 0; i < 0x34; i++)
                list.Add(0xCD);
            list.AddRange(BitConverter.GetBytes(Switch(Unknown68)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown6C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown70)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownAssetID74)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown78)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown7C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown80)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown84)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown88)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown8C)));

            return list;
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

    public class EntrySHRP_Type5 : EntrySHRP
    {
        public int Unknown18 { get; set; }
        public AssetID Unknown1C { get; set; }
        public float Unknown20 { get; set; }
        public AssetID Unknown24 { get; set; }
        public AssetID Unknown28 { get; set; }
        public int Unknown3C { get; set; }
        public AssetID Unknown40 { get; set; }
        public float Unknown44 { get; set; }
        public AssetID Unknown48 { get; set; }
        public AssetID Unknown4C { get; set; }
        
        public EntrySHRP_Type5() : base()
        {
            Type = 5;
            Unknown1C = 0;
            Unknown24 = 0;
            Unknown28 = 0;
            Unknown40 = 0;
            Unknown48 = 0;
            Unknown4C = 0;
        }

        public EntrySHRP_Type5(byte[] data) : base(data)
        {
            Unknown18 = Switch(BitConverter.ToInt32(data, 0x18));
            Unknown1C = Switch(BitConverter.ToUInt32(data, 0x1C));
            Unknown20 = Switch(BitConverter.ToSingle(data, 0x20));
            Unknown24 = Switch(BitConverter.ToUInt32(data, 0x24));
            Unknown28 = Switch(BitConverter.ToUInt32(data, 0x28));
            Unknown3C = Switch(BitConverter.ToInt32(data, 0x3C));
            Unknown40 = Switch(BitConverter.ToUInt32(data, 0x40));
            Unknown44 = Switch(BitConverter.ToSingle(data, 0x44));
            Unknown48 = Switch(BitConverter.ToUInt32(data, 0x48));
            Unknown4C = Switch(BitConverter.ToUInt32(data, 0x4C));
        }

        public override List<byte> ToByteArray()
        {
            List<byte> list = base.ToByteArray();

            list.AddRange(BitConverter.GetBytes(Switch(Unknown18)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
            for (int i = 0; i < 0x10; i++)
                list.Add(0xCD);
            list.AddRange(BitConverter.GetBytes(Switch(Unknown3C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown4C)));
            for (int i = 0; i < 0x18; i++)
                list.Add(0xCD);
            
            return list;
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

    public class EntrySHRP_Type6 : EntrySHRP
    { 
        public AssetID SoundAssetID { get; set; }
        public int Unknown1C { get; set; }
        public int Unknown20 { get; set; }
        public int Unknown24 { get; set; }
        public int Unknown28 { get; set; }
        public int Unknown2C { get; set; }
        public float Unknown40 { get; set; }
        public float Unknown44 { get; set; }
        public float Unknown48 { get; set; }

        public EntrySHRP_Type6() : base()
        {
            Type = 6;
            SoundAssetID = 0;
        }

        public EntrySHRP_Type6(byte[] data) : base(data)
        {
            SoundAssetID = Switch(BitConverter.ToUInt32(data, 0x18));
            Unknown1C = Switch(BitConverter.ToInt32(data, 0x1C));
            Unknown20 = Switch(BitConverter.ToInt32(data, 0x20));
            Unknown24 = Switch(BitConverter.ToInt32(data, 0x24));
            Unknown28 = Switch(BitConverter.ToInt32(data, 0x28));
            Unknown2C = Switch(BitConverter.ToInt32(data, 0x2C));
            Unknown40 = Switch(BitConverter.ToSingle(data, 0x40));
            Unknown44 = Switch(BitConverter.ToSingle(data, 0x44));
            Unknown48 = Switch(BitConverter.ToSingle(data, 0x48));
        }

        public override List<byte> ToByteArray()
        {
            List<byte> list = base.ToByteArray();

            list.AddRange(BitConverter.GetBytes(Switch(SoundAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown1C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown20)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown24)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown28)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown2C)));
            for (int i = 0; i < 0x10; i++)
                list.Add(0xCD);
            list.AddRange(BitConverter.GetBytes(Switch(Unknown40)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown44)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown48)));

            return list;
        }

        public override bool HasReference(uint assetID)
        {
            if (SoundAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }
    }
}