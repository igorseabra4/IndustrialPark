using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace IndustrialPark
{
    public class AssetSNDI_GCN_V2 : Asset
    {
        public override string AssetInfo => $"GameCube {game}, {Entries.Sum(e => e.soundEntries.Length)} entries";

        private byte ReadByte(int j)
        {
            return Data[j];
        }

        private short ReadShort(int j)
        {
            return BitConverter.ToInt16(new byte[] {
                Data[j + 1],
                Data[j] }, 0);
        }

        private int ReadInt(int j)
        {
            return BitConverter.ToInt32(new byte[] {
                Data[j + 3],
                Data[j + 2],
                Data[j + 1],
                Data[j] }, 0);
        }

        private void Write(int j, byte value)
        {
            Data[j] = value;
        }

        private void Write(int j, short value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();

            for (int i = 0; i < 2; i++)
                Data[j + i] = split[i];
        }

        private void Write(int j, int value)
        {
            byte[] split = BitConverter.GetBytes(value).Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                Data[j + i] = split[i];
        }

        private Section_AHDR AHDR;
        private byte[] Data
        {
            get => AHDR.data;
            set => AHDR.data = value;
        }

        // most of this file is a very hacky temporary thing

        public AssetSNDI_GCN_V2(string assetName) : base(assetName, AssetType.SoundInfo)
        {
            AHDR = new Section_AHDR(assetID, assetType, flags, new Section_ADBG(0, assetName, assetFileName, checksum), new byte[0x20]);
        }

        public AssetSNDI_GCN_V2(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            this.AHDR = AHDR;
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            return AHDR.data;
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
            set => Write(0x1C, value);
        }

        private byte FileCount
        {
            get => ReadByte(0x1E);
            set => Write(0x1E, value);
        }

        private byte UnknownCount
        {
            set => Write(0x1F, value);
        }

        private int EOF => FooterOffset + 4 * FileCount + 8 * TotalSoundCount;

        [Category("Sound Info")]
        public FSB3_File[] Entries
        {
            get => DeserializeAsset(Data, this);
            set => Data = SerializeData(value, AHDR);
        }

        private uint Switch(uint a) => BitConverter.ToUInt32(BitConverter.GetBytes(a).Reverse().ToArray(), 0);

        private static FSB3_File[] DeserializeAsset(byte[] data, AssetSNDI_GCN_V2 asset)
        {
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(data));

            List<FSB3_File> entries = new List<FSB3_File>();

            for (int i = 0; i < asset.FileCount; i++)
            {
                binaryReader.BaseStream.Position = asset.FooterOffset + 4 * i;
                binaryReader.BaseStream.Position = asset.Switch(binaryReader.ReadUInt32()) + 0x20;

                entries.Add(new FSB3_File(binaryReader));
            }

            binaryReader.BaseStream.Position = asset.FooterOffset + 4 * asset.FileCount;

            for (int i = 0; i < asset.TotalSoundCount; i++)
            {
                EntrySoundInfo_GCN_V2 a = new EntrySoundInfo_GCN_V2();
                a.SetEntryPartTwo(binaryReader);
                entries[a.fileIndex].soundEntries[a.index].SetEntryPartTwo(a);
            }

            return entries.ToArray();
        }

        private static byte[] SerializeData(FSB3_File[] value, Section_AHDR AHDR)
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
                newData.AddRange(BitConverter.GetBytes(value[i].offset).Reverse());

                for (int j = 0; j < value[i].numSamples; j++)
                    listForPart2.Add(value[i].soundEntries[j]);
            }

            listForPart2 = listForPart2.OrderBy(f => f._assetID).ToList();

            foreach (EntrySoundInfo_GCN_V2 a in listForPart2)
                newData.AddRange(a.PartTwoToByteArray());

            //newData.AddRange(Data.Skip(EOF));

            byte unkCount = 0; //Data[0x1F];

            // hacky solution
            AssetSNDI_GCN_V2 asset = new AssetSNDI_GCN_V2(
                new Section_AHDR(AHDR.assetID, AHDR.assetType, AHDR.flags, AHDR.ADBG, newData.ToArray()), Game.Incredibles, Endianness.Big)
            {
                FooterOffset = footerOffset,
                assetID = AHDR.assetID,
                FileCount = (byte)value.Length,
                TotalSoundCount = (short)listForPart2.Count,
                SoundCountFirstFile = (short)(value.Length > 0 ? value[0].numSamples : 0),
                UnknownCount = unkCount
            };
            asset.SoundCountRest = (short)(asset.TotalSoundCount - asset.SoundCountFirstFile);

            return asset.AHDR.data;
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

            List<FSB3_File> newEntries = Entries.ToList();

            if (newEntries.Count == 0)
                newEntries.Add(new FSB3_File());

            FSB3_File temp = new FSB3_File(new BinaryReader(new MemoryStream(soundData)));
            temp.soundEntries[0].Sound = assetID;
            newEntries[0].Merge(temp);
            //newEntries.Add(temp);

            Entries = newEntries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            List<FSB3_File> entries = Entries.ToList();

            for (int i = 0; i < entries.Count; i++)
            {
                List<EntrySoundInfo_GCN_V2> soundEntries = entries[i].soundEntries.ToList();

                for (int j = 0; j < soundEntries.Count; j++)
                    if (soundEntries[j].Sound == assetID)
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
                    if (f.soundEntries[i].Sound == assetID)
                    {
                        var r = f;
                        r.soundEntries = new EntrySoundInfo_GCN_V2[] { f.soundEntries[i] };
                        return SerializeData(r, AHDR).Skip(0x20).ToArray();
                    }

            throw new Exception($"Error: SNDI asset does not contain sound header for asset [{assetID:X8}]");
        }

        private byte[] SerializeData(FSB3_File r, Section_AHDR AHDR) => SerializeData(new FSB3_File[] { r }, AHDR);

        public void Clean(IEnumerable<uint> assetIDs)
        {
            var fsb3s = Entries.ToList();

            for (int i = 0; i < fsb3s.Count; i++)
            {
                var entries = fsb3s[i].soundEntries.ToList();
                for (int j = 0; i < entries.Count; j++)
                    if (!assetIDs.Contains(entries[j].Sound))
                        entries.RemoveAt(j--);
                if (entries.Count == 0)
                    fsb3s.RemoveAt(i--);
                else
                    fsb3s[i].soundEntries = entries.ToArray();
            }

            Entries = fsb3s.ToArray();
        }
    }
}