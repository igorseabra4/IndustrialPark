using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntrySoundInfo_GCN_V2
    {
        public int templengthcompressedbytes;

        public int lengthsamples { get; set; }
        public int lengthcompressedbytes => Data.Length;

        public uint UnknownUInt08 { get; set; }
        public uint UnknownUInt0C { get; set; }
        public uint UnknownUInt10 { get; set; }
        public uint UnknownUInt14 { get; set; }
        public uint UnknownUInt18 { get; set; }
        public uint UnknownUInt1C { get; set; }
        public uint UnknownUInt20 { get; set; }
        public uint UnknownUInt24 { get; set; }

        public byte UnknownByte28 { get; set; }
        public byte UnknownByte29 { get; set; }
        public byte UnknownByte2A { get; set; }
        public byte UnknownByte2B { get; set; }
        public byte UnknownByte2C { get; set; }
        public byte UnknownByte2D { get; set; }
        public byte UnknownByte2E { get; set; }
        public byte UnknownByte2F { get; set; }
        public byte UnknownByte30 { get; set; }
        public byte UnknownByte31 { get; set; }
        public byte UnknownByte32 { get; set; }
        public byte UnknownByte33 { get; set; }

        public byte UnknownByte34 { get; set; }
        public byte UnknownByte35 { get; set; }

        public byte[] Data { get; set; }

        public EntrySoundInfo_GCN_V2()
        {
            Data = new byte[0];
        }

        public void SetEntryPartOne(BinaryReader binaryReader)
        {
            lengthsamples = binaryReader.ReadInt32();
            templengthcompressedbytes = binaryReader.ReadInt32();

            UnknownUInt08 = binaryReader.ReadUInt32();
            UnknownUInt0C = binaryReader.ReadUInt32();
            UnknownUInt10 = binaryReader.ReadUInt32();
            UnknownUInt14 = binaryReader.ReadUInt32();
            UnknownUInt18 = binaryReader.ReadUInt32();
            UnknownUInt1C = binaryReader.ReadUInt32();
            UnknownUInt20 = binaryReader.ReadUInt32();
            UnknownUInt24 = binaryReader.ReadUInt32();

            UnknownByte28 = binaryReader.ReadByte();
            UnknownByte29 = binaryReader.ReadByte();
            UnknownByte2A = binaryReader.ReadByte();
            UnknownByte2B = binaryReader.ReadByte();
            UnknownByte2C = binaryReader.ReadByte();
            UnknownByte2D = binaryReader.ReadByte();
            UnknownByte2E = binaryReader.ReadByte();
            UnknownByte2F = binaryReader.ReadByte();
            UnknownByte30 = binaryReader.ReadByte();
            UnknownByte31 = binaryReader.ReadByte();
            UnknownByte32 = binaryReader.ReadByte();
            UnknownByte33 = binaryReader.ReadByte();

            UnknownByte34 = binaryReader.ReadByte();
            UnknownByte35 = binaryReader.ReadByte();
        }

        public IEnumerable<byte> PartOneToByteArray(int index)
        {
            List<byte> list = new List<byte>(0x36);

            if (index == 0)
            {
                list.AddRange(BitConverter.GetBytes(0));
                list.AddRange(BitConverter.GetBytes(0));
            }
            else
            {
                list.AddRange(BitConverter.GetBytes(lengthsamples));
                list.AddRange(BitConverter.GetBytes(lengthcompressedbytes));
            }

            list.AddRange(BitConverter.GetBytes(UnknownUInt08));
            list.AddRange(BitConverter.GetBytes(UnknownUInt0C));
            list.AddRange(BitConverter.GetBytes(UnknownUInt10));
            list.AddRange(BitConverter.GetBytes(UnknownUInt14));
            list.AddRange(BitConverter.GetBytes(UnknownUInt18));
            list.AddRange(BitConverter.GetBytes(UnknownUInt1C));
            list.AddRange(BitConverter.GetBytes(UnknownUInt20));
            list.AddRange(BitConverter.GetBytes(UnknownUInt24));

            list.Add(UnknownByte28);
            list.Add(UnknownByte29);
            list.Add(UnknownByte2A);
            list.Add(UnknownByte2B);
            list.Add(UnknownByte2C);
            list.Add(UnknownByte2D);
            list.Add(UnknownByte2E);
            list.Add(UnknownByte2F);
            list.Add(UnknownByte30);
            list.Add(UnknownByte31);
            list.Add(UnknownByte32);
            list.Add(UnknownByte33);

            list.Add(UnknownByte34);
            list.Add(UnknownByte35);

            return list;
        }

        public uint _assetID;
        public AssetID SoundAssetID { get => _assetID; set => _assetID = value; }
        public byte unk1 { get; set; }
        public byte index { get; set; }
        public byte fileIndex { get; set; }
        public byte unk2 { get; set; }

        public void SetEntryPartTwo(BinaryReader binaryReader)
        {
            SoundAssetID = Switch(binaryReader.ReadUInt32());
            unk1 = binaryReader.ReadByte();
            index = binaryReader.ReadByte();
            fileIndex = binaryReader.ReadByte();
            unk2 = binaryReader.ReadByte();
        }

        public void SetEntryPartTwo(EntrySoundInfo_GCN_V2 tempEntry)
        {
            SoundAssetID = tempEntry.SoundAssetID;
            unk1 = tempEntry.unk1;
            index = tempEntry.index;
            fileIndex = tempEntry.fileIndex;
            unk2 = tempEntry.unk2;
        }

        public IEnumerable<byte> PartTwoToByteArray()
        {
            List<byte> list = new List<byte>(8);

            list.AddRange(BitConverter.GetBytes(Switch(SoundAssetID)));
            list.Add(unk1);
            list.Add(index);
            list.Add(fileIndex);
            list.Add(unk2);

            return list;
        }

        public byte[] Serialize()
        {
            List<byte> list = new List<byte>();

            list.AddRange(PartOneToByteArray(1));
            list.AddRange(PartTwoToByteArray());
            list.AddRange(Data);

            return list.ToArray();
        }

        public static EntrySoundInfo_GCN_V2 Deserialize(byte[] soundData)
        {
            EntrySoundInfo_GCN_V2 temp = new EntrySoundInfo_GCN_V2();

            BinaryReader binaryReader = new BinaryReader(new MemoryStream(soundData));

            temp.SetEntryPartOne(binaryReader);
            temp.SetEntryPartTwo(binaryReader);
            temp.Data = binaryReader.ReadBytes(temp.templengthcompressedbytes);

            return temp;
        }

        public override string ToString()
        {
            return Program.MainForm.GetAssetNameFromID(SoundAssetID);
        }
    }

    public class FSB3_File
    {
        // header
        // char4
        [Category("FSB3 Header")]
        public int numSamples => soundEntries.Length;
        [Category("FSB3 Header")]
        public int totalHeadersSize => 72 + numSamples * 0x36;
        [Category("FSB3 Header")]
        public int totalDataSize
        {
            get
            {
                int acc = 0;

                if (numSamples > 0)
                    acc += soundEntries[0].lengthcompressedbytes;
                for (int i = 1; i < numSamples; i++)
                    acc += soundEntries[i].lengthcompressedbytes;

                return acc;
            }
        }
        [Category("FSB3 Header")]
        public int version { get; set; }
        [Category("FSB3 Header")]
        public int mode { get; set; }

        // sample header
        [Category("Sample Header")]
        public ushort size { get; set; }
        [Category("Sample Header")]
        public string name { get; set; }
        [Category("Sample Header")]
        public int lengthsamples
        {
            get
            {
                if (soundEntries.Length > 0)
                    return soundEntries[0].lengthsamples;
                return 0;
            }
        }
        [Category("Sample Header")]
        public int lengthcompressedbytes
        {
            get
            {
                if (soundEntries.Length > 0)
                    return soundEntries[0].lengthcompressedbytes;
                return 0;
            }
        }
        [Category("Sample Header")]
        public uint loopstart { get; set; }
        [Category("Sample Header")]
        public uint loopend
        {
            get
            {
                if (soundEntries.Length > 0)
                    return (uint)(soundEntries[0].lengthsamples - 1);
                return 0;
            }
        }
        [Category("Sample Header")]
        public uint sampleHeaderMode { get; set; }
        [Category("Sample Header")]
        public int deffreq { get; set; }
        [Category("Sample Header")]
        public ushort defvol { get; set; }
        [Category("Sample Header")]
        public short defpan { get; set; }
        [Category("Sample Header")]
        public ushort defpri { get; set; }
        [Category("Sample Header")]
        public ushort numchannels { get; set; }
        [Category("Sample Header")]
        public float mindistance { get; set; }
        [Category("Sample Header")]
        public float maxdistance { get; set; }

        [Category("Other")]
        public EntrySoundInfo_GCN_V2[] soundEntries { get; set; }

        public FSB3_File()
        {
            mode = 2;
            version = 196609;

            deffreq = 32000;
            defpan = 128;
            defpri = 255;
            defvol = 255;
            mindistance = 1f;
            maxdistance = 1000000f;
            name = "empty";
            numchannels = 1;
            sampleHeaderMode = 33558561;
            size = 126;

            soundEntries = new EntrySoundInfo_GCN_V2[0];
        }

        public FSB3_File(BinaryReader binaryReader)
        {
            if ((binaryReader.ReadChar() != 'F') ||
                (binaryReader.ReadChar() != 'S') ||
                (binaryReader.ReadChar() != 'B') ||
                (binaryReader.ReadChar() != '3'))
                throw new Exception("Error reading FSB3 file");

            int numSamples = binaryReader.ReadInt32();
            int totalHeadersSize = binaryReader.ReadInt32();
            int totalDataSize = binaryReader.ReadInt32();
            version = binaryReader.ReadInt32();
            mode = binaryReader.ReadInt32();

            size = binaryReader.ReadUInt16();
            name = new string(binaryReader.ReadChars(30));
            int lengthsamples = binaryReader.ReadInt32();
            int templengthcompressedbytes = binaryReader.ReadInt32();
            loopstart = binaryReader.ReadUInt32();
            uint loopend = binaryReader.ReadUInt32();
            sampleHeaderMode = binaryReader.ReadUInt32();
            deffreq = binaryReader.ReadInt32();
            defvol = binaryReader.ReadUInt16();
            defpan = binaryReader.ReadInt16();
            defpri = binaryReader.ReadUInt16();
            numchannels = binaryReader.ReadUInt16();
            mindistance = binaryReader.ReadSingle();
            maxdistance = binaryReader.ReadSingle();

            soundEntries = new EntrySoundInfo_GCN_V2[numSamples];
            for (int i = 0; i < numSamples; i++)
            {
                soundEntries[i] = new EntrySoundInfo_GCN_V2();
                soundEntries[i].SetEntryPartOne(binaryReader);
            }

            if (numSamples > 0)
            {
                soundEntries[0].lengthsamples = lengthsamples;
                soundEntries[0].templengthcompressedbytes = templengthcompressedbytes;
            }
            
            for (int i = 0; i < numSamples; i++)
                soundEntries[i].Data = binaryReader.ReadBytes(soundEntries[i].templengthcompressedbytes);
        }

        public IEnumerable<byte> ToByteArray(int index)
        {
            List<byte> list = new List<byte>(totalHeadersSize + totalDataSize + 0x18);

            list.AddRange(new byte[] { (byte)'F', (byte)'S', (byte)'B', (byte)'3', });
            list.AddRange(BitConverter.GetBytes(numSamples));
            list.AddRange(BitConverter.GetBytes(totalHeadersSize));
            list.AddRange(BitConverter.GetBytes(totalDataSize));
            list.AddRange(BitConverter.GetBytes(version));
            list.AddRange(BitConverter.GetBytes(mode));

            list.AddRange(BitConverter.GetBytes(size));
            foreach (char c in name)
                list.Add((byte)c);
            for (int i = name.Length; i < 30; i++)
                list.Add(0);
            list.AddRange(BitConverter.GetBytes(lengthsamples));
            list.AddRange(BitConverter.GetBytes(lengthcompressedbytes));
            list.AddRange(BitConverter.GetBytes(loopstart));
            list.AddRange(BitConverter.GetBytes(loopend));
            list.AddRange(BitConverter.GetBytes(sampleHeaderMode));
            list.AddRange(BitConverter.GetBytes(deffreq));
            list.AddRange(BitConverter.GetBytes(defvol));
            list.AddRange(BitConverter.GetBytes(defpan));
            list.AddRange(BitConverter.GetBytes(defpri));
            list.AddRange(BitConverter.GetBytes(numchannels));
            list.AddRange(BitConverter.GetBytes(mindistance));
            list.AddRange(BitConverter.GetBytes(maxdistance));
            
            for (int i = 0; i < numSamples; i++)
            {
                list.AddRange(soundEntries[i].PartOneToByteArray(i));
                soundEntries[i].index = (byte)i;
                soundEntries[i].fileIndex = (byte)index;
            }

            foreach (EntrySoundInfo_GCN_V2 i in soundEntries)
                list.AddRange(i.Data);

            return list;
        }

        [Category("Other"), ReadOnly(true)]
        public int offset { get; set; }

        public void Merge(FSB3_File file)
        {
            List<EntrySoundInfo_GCN_V2> list = soundEntries.ToList();

            List<uint> existingSounds = new List<uint>(soundEntries.Length);
            foreach (EntrySoundInfo_GCN_V2 s in soundEntries)
                existingSounds.Add(s.SoundAssetID);

            foreach (EntrySoundInfo_GCN_V2 s in file.soundEntries)
                if (!existingSounds.Contains(s.SoundAssetID))
                    list.Add(s);
            
            soundEntries = list.ToArray();
        }
    }

    public class AssetSNDI_GCN_V2 : Asset
    {
        public AssetSNDI_GCN_V2(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            foreach (FSB3_File a in Entries)
                foreach (EntrySoundInfo_GCN_V2 e in a.soundEntries)
                    if (e.SoundAssetID == assetID)
                        return true;

            return base.HasReference(assetID);
        }

        private AssetID AssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }

        private int FooterOffset
        {
            get => ReadInt(0x04) + 0x20;
            set => Write(0x04, value - 0x20);
        }

        private short TotalSoundCount
        {
            get => ReadShort(0x18);
            set => Write(0x18, value);
        }

        private short SoundCountFirstFile
        {
            get => ReadShort(0x1A);
            set => Write(0x1A, value);
        }

        private short SoundCountRest
        {
            get => ReadShort(0x1C);
            set => Write(0x1C, value);
        }

        private byte FileCount
        {
            get => ReadByte(0x1E);
            set => Write(0x1E, value);
        }

        private byte UnknownCount
        {
            get => ReadByte(0x1F);
            set => Write(0x1F, value);
        }

        private int EOF => FooterOffset + 4 * FileCount + 8 * TotalSoundCount;

        [Category("Sound Info")]
        public FSB3_File[] Entries
        {
            get
            {
                BinaryReader binaryReader = new BinaryReader(new MemoryStream(Data));
                
                List<FSB3_File> entries = new List<FSB3_File>();

                for (int i = 0; i < FileCount; i++)
                {
                    binaryReader.BaseStream.Position = FooterOffset + 4 * i;
                    binaryReader.BaseStream.Position = Switch(binaryReader.ReadUInt32()) + 0x20;

                    entries.Add(new FSB3_File(binaryReader));
                }

                binaryReader.BaseStream.Position = FooterOffset + 4 * FileCount;

                for (int i = 0; i < TotalSoundCount; i++)
                {
                    EntrySoundInfo_GCN_V2 a = new EntrySoundInfo_GCN_V2();
                    a.SetEntryPartTwo(binaryReader);
                    entries[a.fileIndex].soundEntries[a.index].SetEntryPartTwo(a);
                }

                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(new byte[0x20]);
                
                for (int i = 0; i < value.Length; i++)
                {
                    value[i].offset = newData.Count - 0x20;
                    newData.AddRange(value[i].ToByteArray(i));
                    while (newData.Count % 0x20 != 0)
                        newData.Add(0);
                }

                int footerOffset = newData.Count;

                List<EntrySoundInfo_GCN_V2> listForPart2 = new List<EntrySoundInfo_GCN_V2>();

                for (int i = 0; i < value.Length; i++)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(value[i].offset)));

                    for (int j = 0; j < value[i].numSamples; j++)
                        listForPart2.Add(value[i].soundEntries[j]);
                }

                listForPart2 = listForPart2.OrderBy(f => f._assetID).ToList();

                foreach (EntrySoundInfo_GCN_V2 a in listForPart2)
                    newData.AddRange(a.PartTwoToByteArray());
                
                //newData.AddRange(Data.Skip(EOF));

                byte unkCount = 0; //Data[0x1F];

                Data = newData.ToArray();

                FooterOffset = footerOffset;
                AssetID = AHDR.assetID;
                FileCount = (byte)value.Length;
                TotalSoundCount = (short)listForPart2.Count;
                SoundCountFirstFile = (short)(value.Length > 0 ? value[0].numSamples : 0);
                SoundCountRest = (short)(TotalSoundCount - SoundCountFirstFile);
                UnknownCount = unkCount;
            }
        }
        
        public void Merge(AssetSNDI_GCN_V2 assetSNDI)
        {
            List<FSB3_File> newEntries = new List<FSB3_File>();

            FSB3_File first = Entries[0];
            first.Merge(assetSNDI.Entries[0]);

            newEntries.Add(first);

            for (int i = 1; i < Entries.Length; i++)
                newEntries.Add(Entries[i]);
            for (int i = 1; i < assetSNDI.Entries.Length; i++)
                newEntries.Add(assetSNDI.Entries[i]);

            Entries = newEntries.ToArray();
        }

        public void AddEntry(byte[] soundData, uint assetID)
        {
            RemoveEntry(assetID);

            List<FSB3_File> entries = Entries.ToList();

            if (entries.Count == 0)
                entries.Add(new FSB3_File());

            List<EntrySoundInfo_GCN_V2> entries2 = entries[0].soundEntries.ToList();
            entries2.Add(EntrySoundInfo_GCN_V2.Deserialize(soundData));
            entries[0].soundEntries = entries2.ToArray();

            Entries = entries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            List<FSB3_File> entries = Entries.ToList();

            for (int i = 0; i < entries.Count; i++)
            {
                List<EntrySoundInfo_GCN_V2> soundEntries = entries[i].soundEntries.ToList();

                for (int j = 0; j < soundEntries.Count; j++)
                    if (soundEntries[j].SoundAssetID == assetID)
                    {
                        soundEntries.RemoveAt(j);
                        j--;
                    }

                entries[i].soundEntries = soundEntries.ToArray();

                if (entries[i].numSamples == 0)
                {
                    entries.RemoveAt(i);
                    i--;
                }
            }

            Entries = entries.ToArray();
        }

        public byte[] GetHeader(uint assetID)
        {
            foreach (FSB3_File f in Entries)
                for (int i = 0; i < f.numSamples; i++)
                    if (f.soundEntries[i].SoundAssetID == assetID)
                        return f.soundEntries[i].Serialize();

            throw new Exception($"Error: SNDI asset does not contain sound header for asset [{assetID.ToString("X8")}]");
        }
    }
}