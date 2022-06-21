using HipHopFile;
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
        private const string catName = "Group";

        [Category(catName)]
        public Delegation ReceiveEventDelegation { get; set; }
        [Category(catName)]
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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write((short)Items.Length);
                writer.Write((short)ReceiveEventDelegation);
                foreach (var a in Items)
                    writer.Write(a);
                writer.Write(SerializeLinks(endianness));

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (AssetID a in Items)
            {
                if (a == 0)
                    result.Add("Group with item set to 0");
                Verify(a, ref result);
            }
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