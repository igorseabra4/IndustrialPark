﻿using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public enum Delegation
    {
        AllItems = 0,
        RandomItem = 1,
        InOrder = 2
    }

    public class AssetGRUP : BaseAsset, IAssetAddSelected
    {
        public override string AssetInfo => $"{Items.Length} items";

        private const string categoryName = "Group";

        [Category(categoryName)]
        public Delegation ReceiveEventDelegation { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID[] Items { get; set; }

        public AssetGRUP(string assetName) : base(assetName, AssetType.Group, BaseAssetType.Group)
        {
            Items = new AssetID[0];
        }

        public AssetGRUP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                var itemCount = reader.ReadUInt16();
                ReceiveEventDelegation = (Delegation)reader.ReadInt16();

                Items = new AssetID[itemCount];
                for (int i = 0; i < Items.Length; i++)
                    Items[i] = reader.ReadUInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

            base.Serialize(writer);
            writer.Write((short)Items.Length);
            writer.Write((short)ReceiveEventDelegation);
            foreach (var a in Items)
                writer.Write(a);
            SerializeLinks(writer);


        }

        [Browsable(false)]
        public string GetItemsText => "group";

        public void AddItems(List<uint> newItems)
        {
            var items = Items.ToList();
            foreach (uint i in newItems)
                if (!items.Contains(i))
                    items.Add(i);
            Items = items.ToArray();
        }
    }
}