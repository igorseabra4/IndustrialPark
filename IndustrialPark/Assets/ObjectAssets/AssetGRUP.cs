using HipHopFile;
using System;
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

    public class AssetGRUP : BaseAsset
    {
        private const string catName = "Group";

        [Category(catName)]
        public Delegation ReceiveEventDelegation { get; set; }
        [Category(catName)]
        public AssetID[] GroupItems { get; set; }

        public AssetGRUP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            var itemCount = reader.ReadUInt16();
            ReceiveEventDelegation = (Delegation)reader.ReadInt16();

            GroupItems = new AssetID[itemCount];
            for (int i = 0; i < GroupItems.Length; i++)
                GroupItems[i] = reader.ReadUInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(SerializeBase(platform));

            writer.Write((short)GroupItems.Length);
            writer.Write((short)ReceiveEventDelegation);
            foreach (var a in GroupItems)
                writer.Write(a);

            writer.Write(SerializeLinks(platform));

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => GroupItems.Any(a => a == assetID) || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (AssetID a in GroupItems)
            {
                if (a == 0)
                    result.Add("GRUP with group item set to 0");
                Verify(a, ref result);
            }
        }
    }
}