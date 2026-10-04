using DiscordRPC;
using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using static Assimp.Metadata;

namespace IndustrialPark
{
    public class EntryJAW : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Sound { get; set; }
        public int Flags { get; set; }
        public byte[] JawData { get; set; }

        public EntryJAW()
        {
            JawData = new byte[0];
        }

        public EntryJAW(Game game) : this()
        {
            _game = game;
        }

        public EntryJAW(AssetID soundAssetID, byte[] jawData)
        {
            Sound = soundAssetID;
            JawData = jawData;
        }

        public EntryJAW(AssetID soundAssetID, int flags, byte[] jawData) : this(soundAssetID, jawData)
        {
            Flags = flags;
        }

        public override string ToString()
        {
            return $"[{HexUIntTypeConverter.StringFromAssetID(Sound)}] - [{JawData.Length}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryJAW entryJAW)
                return Sound.Equals(entryJAW.Sound);
            return false;
        }

        public override int GetHashCode()
        {
            return Sound.GetHashCode();
        }

        public override void Serialize(EndianBinaryWriter writer) { }
    }

    public class AssetJAW : Asset, IControllerAsset
    {
        public override string AssetInfo => $"{JAW_Entries.Length} entries";

        private List<EntryJAW> _entries;
        [Category("Jaw Data"), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor)), AssetPropertyCollectionOptions(allowAdd: false, allowCopy: false)]
        public EntryJAW[] JAW_Entries { get => [.. _entries]; set => _entries = [.. value]; }

        public AssetJAW(string assetName) : base(assetName, AssetType.JawDataTable)
        {
            _entries = [];
        }

        public AssetJAW(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
            var len = reader.ReadInt32();
            _entries = [with(len)];

            int startOfJawData = 4 + 12 * JAW_Entries.Length;

            for (int i = 0; i < len; i++)
            {
                reader.endianness = endianness;
                uint soundAssetID = reader.ReadUInt32();
                int offset = reader.ReadInt32();
                reader.ReadInt32();

                long returnPos = reader.BaseStream.Position;

                reader.BaseStream.Position = startOfJawData + offset;

                if (game >= Game.ROTU)
                {
                    int length = reader.ReadInt32();
                    int flags = reader.ReadInt32();
                    byte[] jawData = reader.ReadBytes(length);
                    _entries.Add(new EntryJAW(soundAssetID, flags, jawData));
                }
                else
                {
                    reader.endianness = Endianness.Little;
                    int length = reader.ReadInt32();
                    byte[] jawData = reader.ReadBytes(length);
                    _entries.Add(new EntryJAW(soundAssetID, jawData));
                }
                reader.BaseStream.Position = returnPos;
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            List<byte> newJawData = new List<byte>();

            writer.Write(JAW_Entries.Length);

            foreach (var i in JAW_Entries)
            {
                writer.Write(i.Sound);
                writer.Write(newJawData.Count);

                if (game >= Game.ROTU)
                {
                    byte[] length = writer.endianness == Endianness.Little ? BitConverter.GetBytes(i.JawData.Length) : BitConverter.GetBytes(i.JawData.Length).Reverse().ToArray();
                    writer.Write(i.JawData.Length + 8);
                    newJawData.AddRange(length);
                    byte[] flags = writer.endianness == Endianness.Little ? BitConverter.GetBytes(i.Flags) : BitConverter.GetBytes(i.Flags).Reverse().ToArray();
                    newJawData.AddRange(flags);
                }
                else
                {
                    writer.Write(i.JawData.Length + 4);
                    newJawData.AddRange(BitConverter.GetBytes(i.JawData.Length));
                }
                newJawData.AddRange(i.JawData);

                while (newJawData.Count % 4 != 0)
                    newJawData.Add(0);
            }

            writer.Write(newJawData.ToArray());
        }

        public void Merge(AssetJAW asset)
        {
            _entries.RemoveAll(e => asset._entries.Any(entry => e.Sound == entry.Sound));
            _entries.AddRange(asset._entries);
        }

        public void AddEntry(byte[] jawData, uint assetID)
        {
            RemoveEntry(assetID);
            _entries.Add(new EntryJAW(assetID, jawData));
        }

        public void RemoveEntry(uint assetID)
        {
            _entries.RemoveAll(e => e.Sound == assetID);
        }
    }
}