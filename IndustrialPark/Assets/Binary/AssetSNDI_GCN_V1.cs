using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntrySoundInfo_GCN_V1
    {
        public byte[] SoundHeader { get; set; }
        public AssetID SoundAssetID { get; set; }

        public EntrySoundInfo_GCN_V1()
        {
            SoundHeader = new byte[0x60];
            SoundAssetID = 0;
        }

        public EntrySoundInfo_GCN_V1(byte[] soundHeader, uint soundAssetID)
        {
            SoundHeader = soundHeader;
            SoundAssetID = soundAssetID;
        }

        public EntrySoundInfo_GCN_V1(EndianBinaryReader reader)
        {
            SoundHeader = reader.ReadBytes(0x60);
            SoundAssetID = reader.ReadUInt32();
        }

        public byte[] Serialize()
        {
            var writer = new EndianBinaryWriter(Platform.GameCube);

            writer.Write(SoundHeader);
            writer.Write(SoundAssetID);

            return writer.ToArray();
        }

        public override string ToString()
        {
            return Program.MainForm.GetAssetNameFromID(SoundAssetID);
        }
    }

    public class AssetSNDI_GCN_V1 : Asset
    {
        private const string categoryName = "Sound Info: GCN V1";

        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_SND { get; set; }
        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_SNDS { get; set; }
        [Category(categoryName)]
        public EntrySoundInfo_GCN_V1[] Entries_Sound_CIN { get; set; }

        public AssetSNDI_GCN_V1(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, Platform.GameCube);

            int entriesSndAmount = reader.ReadInt32();
            reader.ReadInt32();
            int entriesSndsAmount = reader.ReadInt32();
            int entriesCinAmount = game == Game.BFBB ? reader.ReadInt32() : 0;

            Entries_SND = new EntrySoundInfo_GCN_V1[entriesSndAmount];
            for (int i = 0; i < Entries_SND.Length; i++)
                Entries_SND[i] = new EntrySoundInfo_GCN_V1(reader);

            Entries_SNDS = new EntrySoundInfo_GCN_V1[entriesSndsAmount];
            for (int i = 0; i < Entries_SNDS.Length; i++)
                Entries_SNDS[i] = new EntrySoundInfo_GCN_V1(reader);

            if (game == Game.BFBB)
            {
                Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[entriesCinAmount];
                for (int i = 0; i < Entries_Sound_CIN.Length; i++)
                    Entries_Sound_CIN[i] = new EntrySoundInfo_GCN_V1(reader);
            }
            else
                Entries_Sound_CIN = new EntrySoundInfo_GCN_V1[0];
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Entries_SND.Length);
            writer.Write(0);
            writer.Write(Entries_SNDS.Length);
            if (game == Game.BFBB)
                writer.Write(Entries_Sound_CIN.Length);

            foreach (var e in Entries_SND)
                writer.Write(e.Serialize());
            foreach (var e in Entries_SNDS)
                writer.Write(e.Serialize());
            if (game == Game.BFBB)
                foreach (var e in Entries_Sound_CIN)
                    writer.Write(e.Serialize());

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var a in Entries_SND)
                if (a.SoundAssetID == assetID)
                    return true;
            
            foreach (var a in Entries_SNDS)
                if (a.SoundAssetID == assetID)
                    return true;
            
            foreach (var a in Entries_Sound_CIN)
                if (a.SoundAssetID == assetID)
                    return true;

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntrySoundInfo_GCN_V1 a in Entries_SND)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }

            foreach (EntrySoundInfo_GCN_V1 a in Entries_SNDS)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }

            foreach (EntrySoundInfo_GCN_V1 a in Entries_Sound_CIN)
            {
                if (a.SoundAssetID == 0)
                    result.Add("SNDI entry with SoundAssetID set to 0");
                Verify(a.SoundAssetID, ref result);
            }
        }

        public void AddEntry(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            List<EntrySoundInfo_GCN_V1> entries;

            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entries.Remove(entries[i--]);

            entries.Add(new EntrySoundInfo_GCN_V1(soundData.Take(0x60).ToArray(), assetID));

            finalData = soundData.Skip(0x60).ToArray();

            if (assetType == AssetType.SND)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public void RemoveEntry(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_GCN_V1> entries;

            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    entries.Remove(entries[i--]);

            if (assetType == AssetType.SND)
                Entries_SND = entries.ToArray();
            else
                Entries_SNDS = entries.ToArray();
        }

        public byte[] GetHeader(uint assetID, AssetType assetType)
        {
            List<EntrySoundInfo_GCN_V1> entries;
            if (assetType == AssetType.SND)
                entries = Entries_SND.ToList();
            else
                entries = Entries_SNDS.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    return entries[i].SoundHeader;

            entries = Entries_Sound_CIN.ToList();

            for (int i = 0; i < entries.Count; i++)
                if (entries[i].SoundAssetID == assetID)
                    return entries[i].SoundHeader;

            throw new Exception($"Error: SNDI asset does not contain {assetType} sound header for asset [{assetID:X8}]");
        }

        public void Merge(AssetSNDI_GCN_V1 assetSNDI)
        {
            {
                // SND
                var entries = Entries_SND.ToList();
                var assetIDsAlreadyPresent = (from entry in entries select entry.SoundAssetID).Cast<uint>();

                foreach (var entry in assetSNDI.Entries_SND)
                    if (!assetIDsAlreadyPresent.Contains(entry.SoundAssetID))
                        entries.Add(entry);

                Entries_SND = entries.ToArray();
            }
            {
                // SNDS
                var entries = Entries_SNDS.ToList();
                var assetIDsAlreadyPresent = (from entry in entries select entry.SoundAssetID).Cast<uint>();

                foreach (var entry in assetSNDI.Entries_SNDS)
                    if (!assetIDsAlreadyPresent.Contains(entry.SoundAssetID))
                        entries.Add(entry);

                Entries_SNDS = entries.ToArray();
            }
            {
                // Sound_CIN
                var entries = Entries_Sound_CIN.ToList();
                var assetIDsAlreadyPresent = (from entry in entries select entry.SoundAssetID).Cast<uint>();

                foreach (var entry in assetSNDI.Entries_Sound_CIN)
                    if (!assetIDsAlreadyPresent.Contains(entry.SoundAssetID))
                        entries.Add(entry);

                Entries_Sound_CIN = entries.ToArray();
            }
        }

        public void Clean(IEnumerable<uint> assetIDs)
        {
            {
                // SND
                var entries = Entries_SND.ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (!assetIDs.Contains(entries[i].SoundAssetID))
                        entries.RemoveAt(i--);
                Entries_SND = entries.ToArray();
            }
            {
                // SNDS
                var entries = Entries_SNDS.ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (!assetIDs.Contains(entries[i].SoundAssetID))
                        entries.RemoveAt(i--);
                Entries_SNDS = entries.ToArray();
            }
            {
                // Sound_CIN
                var entries = Entries_Sound_CIN.ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (!assetIDs.Contains(entries[i].SoundAssetID))
                        entries.RemoveAt(i--);
                Entries_Sound_CIN = entries.ToArray();
            }
        }
    }
}